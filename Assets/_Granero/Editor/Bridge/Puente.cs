// Puente entre el editor abierto y el asistente. Solo editor. No es parte del juego.
// Lee BridgeCmd/cmd.txt (linea 1 = id, linea 2 = comando) y responde en BridgeCmd/result.txt.
using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GraneroBridge
{
    [InitializeOnLoad]
    public static class Puente
    {
        static string dir;
        static double proximoSondeo;
        static readonly object candado = new object();

        static Puente()
        {
            dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "BridgeCmd"));
            Directory.CreateDirectory(dir);

            EditorApplication.update -= Sondear;
            EditorApplication.update += Sondear;
            CompilationPipeline.assemblyCompilationFinished -= AlCompilarAssembly;
            CompilationPipeline.assemblyCompilationFinished += AlCompilarAssembly;
            CompilationPipeline.compilationStarted -= AlIniciarCompilacion;
            CompilationPipeline.compilationStarted += AlIniciarCompilacion;
            Application.logMessageReceivedThreaded -= AlLog;
            Application.logMessageReceivedThreaded += AlLog;

            Escribir("alive.txt", DateTime.Now.ToString("o") + " dominio cargado | playing=" + EditorApplication.isPlaying);
        }

        // ------------------------------------------------------------ archivos
        static void Escribir(string nombre, string texto)
        {
            try { lock (candado) { File.WriteAllText(Path.Combine(dir, nombre), texto, new UTF8Encoding(false)); } }
            catch { }
        }

        static void Anexar(string nombre, string texto)
        {
            try
            {
                lock (candado)
                {
                    string ruta = Path.Combine(dir, nombre);
                    if (File.Exists(ruta) && new FileInfo(ruta).Length > 2000000) File.WriteAllText(ruta, "");
                    File.AppendAllText(ruta, texto + "\n", new UTF8Encoding(false));
                }
            }
            catch { }
        }

        static void Responder(string id, string estado, string mensaje)
        {
            Escribir("result.txt", id + "\n" + estado + "\n" + mensaje);
        }

        // ------------------------------------------------------------ eventos
        static void AlLog(string condicion, string pila, LogType tipo)
        {
            string linea = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + tipo + ": " + condicion;
            if (tipo == LogType.Error || tipo == LogType.Exception || tipo == LogType.Assert) linea += "\n" + pila;
            Anexar("console.log", linea);
        }

        static void AlIniciarCompilacion(object o)
        {
            Anexar("compile.log", "[" + DateTime.Now.ToString("HH:mm:ss") + "] --- compilacion iniciada");
        }

        static void AlCompilarAssembly(string ruta, CompilerMessage[] mensajes)
        {
            int errores = 0;
            var sb = new StringBuilder();
            foreach (var m in mensajes)
            {
                if (m.type == CompilerMessageType.Error) errores++;
                if (m.type == CompilerMessageType.Error || m.type == CompilerMessageType.Warning)
                    sb.AppendLine("  " + m.type + " " + m.file + "(" + m.line + "): " + m.message);
            }
            Anexar("compile.log", "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + Path.GetFileName(ruta) + " errores=" + errores + "\n" + sb);
        }

        // ------------------------------------------------------------ sondeo
        static void Sondear()
        {
            if (EditorApplication.timeSinceStartup < proximoSondeo) return;
            proximoSondeo = EditorApplication.timeSinceStartup + 0.25;

            string cmd = Path.Combine(dir, "cmd.txt");
            if (!File.Exists(cmd)) return;

            string[] lineas;
            try { lineas = File.ReadAllLines(cmd); }
            catch { return; }
            if (lineas.Length < 2) return;
            try { File.Delete(cmd); } catch { return; }

            Ejecutar(lineas[0].Trim(), lineas[1].Trim());
        }

        static void Ejecutar(string id, string comando)
        {
            string[] p = comando.Split(new[] { ' ' }, 2);
            string verbo = p[0];
            string resto = p.Length > 1 ? p[1] : "";
            string res;
            try
            {
                switch (verbo)
                {
                    case "ping": res = "pong " + DateTime.Now.ToString("HH:mm:ss"); break;
                    case "refresh":
                        Responder(id, "OK", "refrescando (puede recargar el dominio)");
                        AssetDatabase.Refresh();
                        return;
                    case "run": res = Correr(resto); break;
                    case "dump": res = Volcar(resto); break;
                    case "play": EditorApplication.isPlaying = true; res = "play"; break;
                    case "stop": EditorApplication.isPlaying = false; res = "stop"; break;
                    case "openscene": EditorSceneManager.OpenScene(resto); res = "abierta " + resto; break;
                    case "save": AssetDatabase.SaveAssets(); res = "guardado"; break;
                    case "shot": res = Captura(resto); break;
                    default: Responder(id, "ERROR", "comando desconocido: " + verbo); return;
                }
                Responder(id, "OK", res);
            }
            catch (TargetInvocationException e)
            {
                Responder(id, "ERROR", (e.InnerException ?? e).ToString());
            }
            catch (Exception e)
            {
                Responder(id, "ERROR", e.ToString());
            }
        }

        // ------------------------------------------------------------ run Tipo.Metodo|argumento
        static string Correr(string spec)
        {
            string arg = null;
            int barra = spec.IndexOf('|');
            if (barra >= 0) { arg = spec.Substring(barra + 1); spec = spec.Substring(0, barra); }

            int punto = spec.LastIndexOf('.');
            if (punto < 0) throw new Exception("Formato: Tipo.Metodo|arg");
            string tipo = spec.Substring(0, punto);
            string metodo = spec.Substring(punto + 1);

            Type t = null;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = a.GetType(tipo);
                if (t != null) break;
            }
            if (t == null) throw new Exception("No encuentro el tipo " + tipo);

            MethodInfo m = t.GetMethod(metodo, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (m == null) throw new Exception("No encuentro el metodo " + metodo + " en " + tipo);

            object r = m.GetParameters().Length == 0 ? m.Invoke(null, null) : m.Invoke(null, new object[] { arg });
            return r == null ? "ok" : r.ToString();
        }

        // ------------------------------------------------------------ dump ruta (prefab, fbx, etc.)
        static string Volcar(string ruta)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(ruta);
            if (go == null) throw new Exception("No hay GameObject en " + ruta);
            var sb = new StringBuilder();
            VolcarNodo(go.transform, 0, sb);

            sb.AppendLine("--- clips:");
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(ruta))
            {
                var c = o as AnimationClip;
                if (c != null && !c.name.StartsWith("__preview__")) sb.AppendLine("  " + c.name + " len=" + c.length.ToString("0.00") + " loop=" + c.isLooping);
            }
            Escribir("dump.txt", sb.ToString());
            return "dump.txt escrito (" + sb.Length + " caracteres)";
        }

        static void VolcarNodo(Transform t, int nivel, StringBuilder sb)
        {
            var comps = new StringBuilder();
            foreach (var c in t.GetComponents<Component>())
            {
                if (c == null || c is Transform) continue;
                if (comps.Length > 0) comps.Append(",");
                comps.Append(c.GetType().Name);
            }
            sb.AppendLine(new string(' ', nivel * 2) + t.name + (comps.Length > 0 ? " [" + comps + "]" : "")
                + " lp=" + t.localPosition.ToString("0.###") + " lr=" + t.localEulerAngles.ToString("0.#") + " ls=" + t.localScale.ToString("0.###"));
            for (int i = 0; i < t.childCount; i++) VolcarNodo(t.GetChild(i), nivel + 1, sb);
        }

        // ------------------------------------------------------------ shot ruta w h px py pz rx ry fov
        static string Captura(string args)
        {
            string[] a = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (a.Length < 9) throw new Exception("shot ruta w h px py pz rx ry fov");
            var ci = System.Globalization.CultureInfo.InvariantCulture;
            string ruta = a[0];
            int w = int.Parse(a[1]); int h = int.Parse(a[2]);
            Vector3 pos = new Vector3(float.Parse(a[3], ci), float.Parse(a[4], ci), float.Parse(a[5], ci));
            Vector3 rot = new Vector3(float.Parse(a[6], ci), float.Parse(a[7], ci), 0f);
            float fov = float.Parse(a[8], ci);

            var go = new GameObject("__CamCaptura");
            var rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            rt.antiAliasing = 4;
            try
            {
                var cam = go.AddComponent<Camera>();
                cam.transform.position = pos;
                cam.transform.eulerAngles = rot;
                cam.fieldOfView = fov;
                cam.nearClipPlane = 0.1f;
                cam.farClipPlane = 800f;
                cam.clearFlags = CameraClearFlags.Skybox;
                cam.targetTexture = rt;
                cam.Render();

                var previo = RenderTexture.active;
                RenderTexture.active = rt;
                var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                tex.Apply();
                RenderTexture.active = previo;

                Directory.CreateDirectory(Path.GetDirectoryName(ruta));
                File.WriteAllBytes(ruta, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
                rt.Release();
                UnityEngine.Object.DestroyImmediate(rt);
            }
            return "captura " + ruta;
        }
    }
}
