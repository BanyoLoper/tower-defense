using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

// Construye la escena jugable "Granero" completa.
//   run GraneroTools.ConstruirMapa.Todo
namespace GraneroTools
{
    public static class ConstruirMapa
    {
        const string P = "Assets/Third Party Assets/Pandazole_Lowpoly_Asset_Bundle/";
        const string Farm = P + "Pandazole Farm Ranch Pack/Prefabs/";
        const string Nat = P + "Pandazole Nature Environment Pack/Prefabs/";
        const string Spells = "Assets/Third Party Assets/Kevin Iglesias/Human Animations/Unity Demo Scenes/Human Spellcasting Animations/Prefabs/Spells/";
        const string Cristal = "Assets/Third Party Assets/Aztech Games/GemsAndCrystals/Prefabs/";
        const string Mats = "Assets/_Granero/Materiales/";
        const string Pref = "Assets/_Granero/Prefabs/";

        static Vector3[] ruta = {
            new Vector3(-33, 0.55f, -12), new Vector3(-14, 0.55f, -12), new Vector3(-14, 0.55f, 12), new Vector3(6, 0.55f, 12),
            new Vector3(6, 0.55f, -8), new Vector3(22, 0.55f, -8), new Vector3(22, 0.55f, 2), new Vector3(30, 0.55f, 2)
        };

        static Vector2[] slots = {
            new Vector2(-24, -7), new Vector2(-22, -17), new Vector2(-8, -6), new Vector2(-19, 0), new Vector2(-9, 6), new Vector2(-4, 17),
            new Vector2(0, 6), new Vector2(12, 2), new Vector2(12, -13), new Vector2(16, -3), new Vector2(27, -5), new Vector2(26, 7)
        };

        static GameObject Cargar(string ruta) { return AssetDatabase.LoadAssetAtPath<GameObject>(ruta); }

        static GameObject Poner(string ruta, Vector3 pos, Vector3 euler, float escala, Transform padre)
        {
            GameObject pf = Cargar(ruta);
            if (pf == null) { Debug.LogWarning("falta " + ruta); return null; }
            GameObject g = (GameObject)PrefabUtility.InstantiatePrefab(pf);
            g.transform.SetParent(padre, false);
            g.transform.position = pos;
            g.transform.rotation = Quaternion.Euler(euler);
            g.transform.localScale = Vector3.one * escala;
            return g;
        }

        static Material Mat(string nombre, Color c)
        {
            string p = Mats + nombre + ".mat";
            Material m = AssetDatabase.LoadAssetAtPath<Material>(p);
            if (m == null) { m = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(m, p); }
            m.SetColor("_BaseColor", c);
            m.SetFloat("_Smoothness", 0.1f);
            EditorUtility.SetDirty(m);
            return m;
        }

        static float DistSegmento(Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a; ab.y = 0; Vector3 ap = p - a; ap.y = 0;
            float t = Mathf.Clamp01(Vector3.Dot(ap, ab) / ab.sqrMagnitude);
            Vector3 q = a + ab * t; q.y = p.y;
            return Vector3.Distance(p, q);
        }

        static float DistRuta(Vector3 p)
        {
            float m = 1e9f;
            for (int i = 0; i < ruta.Length - 1; i++) m = Mathf.Min(m, DistSegmento(p, ruta[i], ruta[i + 1]));
            return m;
        }

        public static string Todo(string arg)
        {
            Scene();
            return "escena Granero creada";
        }

        static void Scene()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Granero/Escenas")) AssetDatabase.CreateFolder("Assets/_Granero", "Escenas");
            var sc = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            string escenaRuta = "Assets/_Granero/Escenas/Granero.unity";

            // ---- materiales
            Material mTierra = Mat("Camino_Tierra", new Color(0.55f, 0.40f, 0.25f));
            Material mSlotN = Mat("Slot_Normal", new Color(0.55f, 0.60f, 0.72f));
            Material mSlotE = Mat("Slot_Elegido", new Color(1f, 0.85f, 0.2f));
            Material mSlotO = Mat("Slot_Ocupado", new Color(0.25f, 0.28f, 0.35f));
            Material mSlotB = Mat("Slot_Bloqueado", new Color(0.50f, 0.16f, 0.16f));
            Material mSlotC = Mat("Slot_Comprable", new Color(0.95f, 0.55f, 0.12f));
            Material mOro = Mat("Candado_Oro", new Color(0.92f, 0.70f, 0.15f));
            Material mHierro = Mat("Candado_Hierro", new Color(0.30f, 0.30f, 0.36f));
            TMP_FontAsset fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Granero/Fuentes/LilitaOne SDF.asset");
            Material mMadera = Mat("Proyectil_Madera", new Color(0.55f, 0.38f, 0.2f));

            // ---- luz
            GameObject luz = new GameObject("Sol");
            Light l = luz.AddComponent<Light>(); l.type = LightType.Directional; l.intensity = 1.15f; l.shadows = LightShadows.Soft;
            luz.transform.rotation = Quaternion.Euler(52, -35, 0);

            // ---- terreno visual (tiles de 5 m)
            GameObject terreno = new GameObject("Terreno");
            GameObject tile = Cargar(Farm + "Env_GrassLand_Plain.prefab");
            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 9; j++)
                {
                    GameObject t = (GameObject)PrefabUtility.InstantiatePrefab(tile);
                    t.transform.SetParent(terreno.transform, false);
                    t.transform.position = new Vector3(-32.5f + i * 5f, 0f, -20f + j * 5f);
                    GameObjectUtility.SetStaticEditorFlags(t, StaticEditorFlags.BatchingStatic);
                }

            // piso para el NavMesh (collider invisible a la altura de los tiles)
            GameObject piso = new GameObject("PisoNav");
            piso.transform.SetParent(terreno.transform, false);
            BoxCollider bc = piso.AddComponent<BoxCollider>();
            bc.center = new Vector3(5f, 0.05f, 0f); bc.size = new Vector3(80f, 1f, 45f);

            // camino de tierra
            GameObject camino = new GameObject("CaminoVisual");
            for (int i = 0; i < ruta.Length; i++)
            {
                if (i < ruta.Length - 1)
                {
                    Vector3 a = ruta[i], b = ruta[i + 1];
                    GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.DestroyImmediate(s.GetComponent<Collider>());
                    s.transform.SetParent(camino.transform, false);
                    s.transform.position = (a + b) * 0.5f + Vector3.up * 0.03f;
                    Vector3 d = b - a; float len = d.magnitude;
                    s.transform.rotation = Quaternion.LookRotation(d);
                    s.transform.localScale = new Vector3(3.4f, 0.06f, len + 3.4f);
                    s.GetComponent<MeshRenderer>().sharedMaterial = mTierra;
                    GameObjectUtility.SetStaticEditorFlags(s, StaticEditorFlags.BatchingStatic);
                }
            }

            // ---- ruta (puntos)
            GameObject rutaRaiz = new GameObject("Ruta");
            Transform[] puntos = new Transform[ruta.Length];
            for (int i = 0; i < ruta.Length; i++)
            {
                GameObject p = new GameObject("Ruta_" + i);
                p.transform.SetParent(rutaRaiz.transform, false);
                p.transform.position = ruta[i];
                puntos[i] = p.transform;
            }

            // ---- granero (prefab)
            Vector3 posGranero = new Vector3(38f, 0.5f, 2f);
            GameObject granero = ConstruirGranero(posGranero);
            GameObject silo = Poner(Farm + "Bld_Silo_01.prefab", new Vector3(37f, 0.5f, -8f), Vector3.zero, 1f, null);
            GameObject molino = Poner(Farm + "Bld_FarmMill_01.prefab", new Vector3(37f, 0.5f, 13f), new Vector3(0, 90, 0), 1f, null);
            Bloqueo(silo, new Vector3(3f, 6f, 3f));
            Bloqueo(molino, new Vector3(6f, 6f, 3f));

            // ---- aldea: puntos de aparicion y trabajo
            GameObject aldeaRaiz = new GameObject("Aldea");
            string[] nombres = { "Aldeano_1", "Aldeano_2", "Aldeano_3", "Aldeano_4", "Minero_1", "Minero_2", "Reparador_1", "Guardia_1", "Guardia_2" };
            Vector3[] posAldea = {
                new Vector3(34, 0.55f, 9), new Vector3(36, 0.55f, 10), new Vector3(33, 0.55f, -5), new Vector3(35, 0.55f, -4),
                new Vector3(29, 0.55f, 11), new Vector3(30, 0.55f, -11), new Vector3(29.5f, 0.55f, -4.5f), new Vector3(29, 0.55f, -2), new Vector3(29, 0.55f, 6)
            };
            Transform[] aldea = new Transform[nombres.Length];
            for (int i = 0; i < nombres.Length; i++)
            {
                GameObject g = new GameObject(nombres[i]);
                g.transform.SetParent(aldeaRaiz.transform, false);
                g.transform.position = posAldea[i];
                g.transform.rotation = Quaternion.LookRotation(Vector3.left);
                aldea[i] = g.transform;
            }
            GameObject minasRaiz = new GameObject("Minas");
            Vector3[] posMinas = { new Vector3(26, 0.55f, 14), new Vector3(26, 0.55f, -14), new Vector3(18, 0.55f, 16) };
            Transform[] minas = new Transform[posMinas.Length];
            string[] gemas = { "Gem_3.prefab", "Crystal_4.prefab", "Gem_7.prefab" };
            for (int i = 0; i < posMinas.Length; i++)
            {
                GameObject g = new GameObject("Mina_" + i);
                g.transform.SetParent(minasRaiz.transform, false);
                g.transform.position = posMinas[i];
                minas[i] = g.transform;
                Poner(Cristal + gemas[i], posMinas[i] + new Vector3(1f, 0.1f, 0f), new Vector3(0, 30 * i, 0), 1.6f, minasRaiz.transform);
            }
            GameObject rep = new GameObject("PuntoReparacion");
            rep.transform.position = new Vector3(29.5f, 0.55f, -2.5f);

            // decoracion de la aldea
            Poner(Farm + "Prop_Haystack_01.prefab", new Vector3(32, 0.55f, 12), Vector3.zero, 2f, aldeaRaiz.transform);
            Poner(Farm + "Prop_Haystack_02.prefab", new Vector3(34, 0.55f, 11), new Vector3(0, 40, 0), 2f, aldeaRaiz.transform);
            Poner(Farm + "Prop_WoodenCrates_01.prefab", new Vector3(32, 0.55f, -8), Vector3.zero, 1.5f, aldeaRaiz.transform);
            Poner(Farm + "Env_Well_01.prefab", new Vector3(31, 0.55f, 9.5f), Vector3.zero, 1.2f, aldeaRaiz.transform);
            for (int i = 0; i < 8; i++)
                Poner(Farm + "Env_WoodFence_01.prefab", new Vector3(41f, 0.55f, -16f + i * 2f), new Vector3(0, 90, 0), 1f, aldeaRaiz.transform);

            // ---- plataformas de torres
            GameObject slotsRaiz = new GameObject("Plataformas");
            SlotTorre[] listaSlots = new SlotTorre[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                GameObject s = new GameObject("Plataforma_" + (i + 1));
                s.transform.SetParent(slotsRaiz.transform, false);
                s.transform.position = new Vector3(slots[i].x, 0.55f, slots[i].y);
                GameObject anillo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Object.DestroyImmediate(anillo.GetComponent<Collider>());
                anillo.name = "Anillo";
                anillo.transform.SetParent(s.transform, false);
                anillo.transform.localPosition = new Vector3(0, 0.05f, 0);
                anillo.transform.localScale = new Vector3(2.6f, 0.05f, 2.6f);
                anillo.GetComponent<MeshRenderer>().sharedMaterial = mSlotN;
                SlotTorre st = s.AddComponent<SlotTorre>();
                st.anillo = anillo.GetComponent<MeshRenderer>();
                st.matNormal = mSlotN; st.matSeleccionado = mSlotE; st.matOcupado = mSlotO;
                st.matBloqueado = mSlotB; st.matComprable = mSlotC;
                // 4 abiertas al inicio; cada oleada superada abre 2 mas. Pagando gemas se abren antes.
                st.nivelRequerido = i < 4 ? 0 : (i - 4) / 2 + 1;
                st.precioDesbloqueo = 80 + 30 * st.nivelRequerido;
                CrearCandado(st, s.transform, mOro, mHierro, fuente);
                if (st.nivelRequerido == 0) st.candado.SetActive(false);
                else anillo.GetComponent<MeshRenderer>().sharedMaterial = mSlotB;
                listaSlots[i] = st;
            }

            // ---- naturaleza (arboles y rocas fuera del camino)
            GameObject natura = new GameObject("Naturaleza");
            Random.InitState(7);
            int arboles = 0, intentos = 0;
            while (arboles < 90 && intentos < 2000)
            {
                intentos++;
                Vector3 p = new Vector3(Random.Range(-34f, 44f), 0.55f, Random.Range(-21.5f, 21.5f));
                if (DistRuta(p) < 5.5f) continue;
                bool cerca = false;
                for (int i = 0; i < slots.Length; i++) if (Vector2.Distance(new Vector2(p.x, p.z), slots[i]) < 4f) { cerca = true; break; }
                if (cerca) continue;
                if (p.x > 28f && p.x < 44f && p.z > -18f && p.z < 17f) continue;      // la aldea
                if (Mathf.Abs(p.z) < 8f && p.x > -30f && p.x < 28f && DistRuta(p) < 9f) continue;
                string t = "Env_Tree_0" + Random.Range(1, 8) + ".prefab";
                Poner(Farm + t, p, new Vector3(0, Random.Range(0, 360), 0), Random.Range(0.9f, 1.3f), natura.transform);
                arboles++;
            }
            int rocas = 0; intentos = 0;
            while (rocas < 35 && intentos < 1000)
            {
                intentos++;
                Vector3 p = new Vector3(Random.Range(-34f, 44f), 0.55f, Random.Range(-21.5f, 21.5f));
                if (DistRuta(p) < 4f) continue;
                if (p.x > 28f && p.x < 44f && p.z > -18f && p.z < 17f) continue;
                Poner(Nat + "HardRock_0" + Random.Range(1, 10) + ".prefab", p, new Vector3(0, Random.Range(0, 360), 0), Random.Range(1.5f, 3f), natura.transform);
                rocas++;
            }
            for (int i = 0; i < 6; i++)
                Poner(Nat + "Log_0" + Random.Range(1, 8) + ".prefab", new Vector3(Random.Range(-33f, -26f), 0.55f, Random.Range(-20f, -14f)), new Vector3(0, Random.Range(0, 360), 0), 1.2f, natura.transform);

            // ---- proyectiles (prefabs)
            GameObject pLanza = CrearProyectilLanza(mMadera);
            GameObject pBola = CrearProyectilBola();

            // ---- camara
            GameObject cam = new GameObject("Main Camera"); cam.tag = "MainCamera";
            Camera c = cam.AddComponent<Camera>(); c.fieldOfView = 45; c.farClipPlane = 300f;
            cam.AddComponent<AudioListener>();
            cam.AddComponent<CamaraRTS>();
            cam.transform.position = new Vector3(5f, 50f, -36f);
            cam.transform.rotation = Quaternion.Euler(58f, 0f, 0f);

            // ---- juego
            GameObject gj = new GameObject("Juego");
            Juego juego = gj.AddComponent<Juego>();
            Rol[] roles = (Rol[])System.Enum.GetValues(typeof(Rol));
            juego.prefabsPorRol = new GameObject[roles.Length];
            foreach (Rol r in roles)
            {
                string pp = Pref + "Agentes/" + r + ".prefab";
                juego.prefabsPorRol[(int)r] = Cargar(pp);
            }
            juego.proyectilLanza = pLanza; juego.proyectilBola = pBola;
            juego.puntosRuta = puntos;
            juego.puntoGranero = granero.transform;
            juego.granero = granero.GetComponent<Agente>();
            juego.puntosMina = minas;
            juego.puntoReparacion = rep.transform;
            juego.aldea = aldea;

            ColocadorTorres col = gj.AddComponent<ColocadorTorres>();
            col.torreLanzadora = Cargar(Pref + "Agentes/TorreLanzadora.prefab");
            col.torreMaga = Cargar(Pref + "Agentes/TorreMaga.prefab");
            col.slots = listaSlots;

            // ---- catalogo de efectos
            GameObject fxObj = new GameObject("Efectos");
            Efectos fx = fxObj.AddComponent<Efectos>();
            string[] nombresFx = { "Human_Spell_Shockwave_Explosion", "Human_Spell_Fireball_Large", "Human_Spell_Heal", "Human_SpellAura_Heal", "Human_Spell_LightningStrike",
                                   "Human_Spell_Shield", "Human_SpellAura_Fire", "Human_Spell_Ice", "Human_Spell_Shockwave_Ground", "Human_SpellAura_Shockwave_Ground" };
            fx.prefabs = new GameObject[nombresFx.Length];
            for (int i = 0; i < nombresFx.Length; i++) fx.prefabs[i] = Cargar(Spells + nombresFx[i] + ".prefab");

            // ---- HUD
            ConstruirHud(gj);

            // ---- guardar y hornear NavMesh
            EditorSceneManager.SaveScene(sc, escenaRuta);
            NavMeshSurface surf = terreno.AddComponent<NavMeshSurface>();
            surf.collectObjects = CollectObjects.All;
            surf.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surf.agentTypeID = 0;
            surf.BuildNavMesh();
            EditorSceneManager.SaveScene(sc, escenaRuta);

            var cfg = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            bool ya = false;
            foreach (var e in cfg) if (e.path == escenaRuta) ya = true;
            if (!ya) cfg.Add(new EditorBuildSettingsScene(escenaRuta, true));
            EditorBuildSettings.scenes = cfg.ToArray();
            AssetDatabase.SaveAssets();
        }

        static void CrearCandado(SlotTorre st, Transform padre, Material oro, Material hierro, TMP_FontAsset fuente)
        {
            GameObject c = new GameObject("Candado");
            c.transform.SetParent(padre, false);
            c.transform.localPosition = new Vector3(0f, 1.1f, 0f);
            c.transform.localRotation = Quaternion.Euler(40f, 0f, 0f);
            c.transform.localScale = Vector3.one * 1.3f;
            Pieza(c.transform, PrimitiveType.Cube, "Cuerpo", new Vector3(0f, 0f, 0f), new Vector3(0.9f, 0.7f, 0.3f), Vector3.zero, oro);
            Pieza(c.transform, PrimitiveType.Cube, "Cerradura", new Vector3(0f, -0.02f, -0.16f), new Vector3(0.14f, 0.24f, 0.05f), Vector3.zero, hierro);
            Pieza(c.transform, PrimitiveType.Cylinder, "ArgollaIzq", new Vector3(-0.27f, 0.5f, 0f), new Vector3(0.11f, 0.17f, 0.11f), Vector3.zero, hierro);
            Pieza(c.transform, PrimitiveType.Cylinder, "ArgollaDer", new Vector3(0.27f, 0.5f, 0f), new Vector3(0.11f, 0.17f, 0.11f), Vector3.zero, hierro);
            Pieza(c.transform, PrimitiveType.Cylinder, "ArgollaTop", new Vector3(0f, 0.66f, 0f), new Vector3(0.11f, 0.3f, 0.11f), new Vector3(0, 0, 90), hierro);
            st.candado = c;

            GameObject t = new GameObject("Cartel");
            t.transform.SetParent(padre, false);
            t.transform.localPosition = new Vector3(0f, 2.6f, 0f);
            t.transform.localRotation = Quaternion.Euler(58f, 0f, 0f);
            TextMeshPro tm = t.AddComponent<TextMeshPro>();
            if (fuente != null) tm.font = fuente;
            tm.fontSize = 4f; tm.alignment = TextAlignmentOptions.Center;
            tm.color = new Color(1f, 0.9f, 0.5f);
            tm.text = "Oleada " + st.nivelRequerido + "\n" + st.precioDesbloqueo + " gemas";
            tm.rectTransform.sizeDelta = new Vector2(8f, 3f);
            st.txtPrecio = tm;
            t.transform.SetParent(c.transform, true);   // se oculta junto con el candado
        }

        static void Pieza(Transform padre, PrimitiveType tipo, string nombre, Vector3 pos, Vector3 esc, Vector3 euler, Material m)
        {
            GameObject g = GameObject.CreatePrimitive(tipo);
            Object.DestroyImmediate(g.GetComponent<Collider>());
            g.name = nombre;
            g.transform.SetParent(padre, false);
            g.transform.localPosition = pos; g.transform.localScale = esc; g.transform.localEulerAngles = euler;
            g.GetComponent<MeshRenderer>().sharedMaterial = m;
        }

        static void Bloqueo(GameObject objetivo, Vector3 tam)
        {
            if (objetivo == null) return;
            GameObject b = new GameObject("Bloqueo");
            b.transform.SetParent(objetivo.transform, false);
            b.transform.position = objetivo.transform.position + Vector3.up * 3f;
            BoxCollider bc = b.AddComponent<BoxCollider>(); bc.size = tam;
            NavMeshModifier mod = b.AddComponent<NavMeshModifier>();
            mod.overrideArea = true; mod.area = 1;
        }

        public static string MedirGranero(string arg)
        {
            GameObject g = GameObject.Find("Granero");
            Transform e = g.transform.Find("Edificio");
            Bounds b = new Bounds(e.position, Vector3.zero);
            foreach (Renderer r in e.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);
            // perfil del techo: altura maxima del mesh a varias x locales
            var sb = new System.Text.StringBuilder("raiz=" + g.transform.position + " bounds min=" + b.min + " max=" + b.max + "\n");
            foreach (MeshFilter mf in e.GetComponentsInChildren<MeshFilter>())
            {
                Mesh m = mf.sharedMesh; if (m == null) continue;
                float[] top = new float[13]; for (int i = 0; i < 13; i++) top[i] = -999f;
                foreach (Vector3 v in m.vertices)
                {
                    Vector3 w = g.transform.InverseTransformPoint(mf.transform.TransformPoint(v));
                    if (Mathf.Abs(w.x) > 4.5f) continue; int k = Mathf.RoundToInt((w.z + 6f)); if (k < 0 || k > 12) continue;
                    if (w.y > top[k]) top[k] = w.y;
                }
                sb.Append(mf.name + ": ");
                for (int i = 0; i < 13; i++) sb.Append("z" + (i - 6) + "=" + top[i].ToString("0.0") + " ");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        const float TECHO_Y = 7.4f;
        static void AddTecho(Transform raiz, string prefab, Vector3 local, float escala, float inclinacionX)
        {
            GameObject pf = Cargar(Cristal + prefab);
            if (pf == null) return;
            GameObject g = (GameObject)PrefabUtility.InstantiatePrefab(pf);
            g.name = "GemaTecho";
            g.transform.SetParent(raiz, false);
            g.transform.localPosition = local;
            // la punta de la gema sigue la normal del techo; giro libre sobre su propio eje
            g.transform.localRotation = Quaternion.Euler(inclinacionX, 0f, 0f) * Quaternion.Euler(Random.Range(-8f, 8f), Random.Range(0f, 360f), Random.Range(-8f, 8f));
            g.transform.localScale = Vector3.one * escala;
        }

        // Copias de los materiales del granero con el efecto Hit de AllIn13D activado (_HIT_ON) y en 0.
        // GraneroEstados lo enciende por MaterialPropertyBlock: no se duplican materiales en juego.
        static void PrepararHit(GameObject raiz)
        {
            var mapa = new Dictionary<Material, Material>();
            foreach (Renderer r in raiz.GetComponentsInChildren<Renderer>(true))
            {
                if (r is ParticleSystemRenderer) continue;
                Material[] mats = r.sharedMaterials;
                bool cambio = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    Material src = mats[i];
                    if (src == null || !src.HasProperty("_HitBlend")) continue;
                    Material copia;
                    if (!mapa.TryGetValue(src, out copia))
                    {
                        string p = Mats + "Granero_" + src.name + ".mat";
                        copia = AssetDatabase.LoadAssetAtPath<Material>(p);
                        if (copia == null) { copia = new Material(src); AssetDatabase.CreateAsset(copia, p); }
                        else { copia.shader = src.shader; copia.CopyPropertiesFromMaterial(src); }
                        copia.SetFloat("_Hit", 1f); copia.EnableKeyword("_HIT_ON");
                        copia.SetColor("_HitColor", new Color(1f, 0.08f, 0.05f, 1f));
                        copia.SetFloat("_HitGlow", 2f); copia.SetFloat("_HitBlend", 0f);
                        EditorUtility.SetDirty(copia);
                        mapa[src] = copia;
                    }
                    mats[i] = copia; cambio = true;
                }
                if (cambio) r.sharedMaterials = mats;
            }
        }

        static GameObject ConstruirGranero(Vector3 pos)
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Granero/Prefabs/Entorno")) AssetDatabase.CreateFolder("Assets/_Granero/Prefabs", "Entorno");
            GameObject raiz = new GameObject("Granero");
            raiz.AddComponent<Salud>().vidaMax = 600f;
            Agente a = raiz.AddComponent<Agente>();
            a.rol = Rol.Granero; a.bando = Bando.Defensor; a.radio = 5f; a.alturaPecho = 3f;
            raiz.AddComponent<ControladorAuto>();
            BarraVida bv = raiz.AddComponent<BarraVida>(); bv.altura = 10f; bv.ancho = 7f; bv.ocultarSiLlena = false;

            GameObject pb = Cargar(Farm + "Bld_Barn_02.prefab");
            GameObject b = (GameObject)PrefabUtility.InstantiatePrefab(pb);
            b.name = "Edificio"; b.transform.SetParent(raiz.transform, false);
            b.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);   // la puerta mira al camino (-x)

            GameObject cr = Cargar(Cristal + "Crystal_6.prefab");
            GameObject c = (GameObject)PrefabUtility.InstantiatePrefab(cr);
            c.name = "CristalDeLaAldea"; c.transform.SetParent(raiz.transform, false);
            c.transform.localPosition = new Vector3(0f, 9f, 0f);
            c.transform.localScale = Vector3.one * 3f;

            // gemas sobre el techo: una hilera en la cumbrera y una por cada ladera
            string[] techo = { "Gem_3.prefab", "Crystal_4.prefab", "Gem_7.prefab", "Crystal_8.prefab", "Gem_5.prefab", "Crystal_3.prefab" };
            Random.InitState(11);
            // el techo es un arco: paredes a y=2.8, cumbrera a y=7.7, ancho +-4.66 (el eje largo es X)
            float[] angulos = { 0f, 28f, -28f, 55f, -55f };
            int[] cuantas = { 8, 6, 6, 4, 4 };
            for (int f = 0; f < angulos.Length; f++)
            {
                float th = angulos[f] * Mathf.Deg2Rad;
                for (int i = 0; i < cuantas[f]; i++)
                {
                    float x = -5.6f + (i + 0.5f) * (11.2f / cuantas[f]) + Random.Range(-0.25f, 0.25f);
                    Vector3 p = new Vector3(x, 2.8f + 4.9f * Mathf.Cos(th) * 1.0f + 0.15f, 4.66f * Mathf.Sin(th) * 1.0f);
                    float esc = (f == 0 ? 2.2f : 1.7f) + 0.5f * Random.value;
                    AddTecho(raiz.transform, techo[(i + f) % techo.Length], p, esc, angulos[f]);
                }
            }

            GameObject bl = new GameObject("Bloqueo");
            bl.transform.SetParent(raiz.transform, false);
            bl.transform.localPosition = new Vector3(0, 3f, 0);
            bl.AddComponent<BoxCollider>().size = new Vector3(14f, 6f, 9.6f);
            NavMeshModifier mod = bl.AddComponent<NavMeshModifier>(); mod.overrideArea = true; mod.area = 1;

            PrepararHit(raiz);
            raiz.AddComponent<GraneroEstados>();

            string ruta = "Assets/_Granero/Prefabs/Entorno/Granero.prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(raiz, ruta, InteractionMode.AutomatedAction);
            raiz.transform.position = pos;   // la instancia de la escena (no el prefab)
            return raiz;
        }

        static GameObject CrearProyectilLanza(Material madera)
        {
            GameObject raiz = new GameObject("Proyectil_Lanza");
            ProyectilBase p = raiz.AddComponent<ProyectilBase>();
            p.velocidad = 17f; p.radioImpacto = 0.6f; p.guiado = true;
            GameObject v = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Object.DestroyImmediate(v.GetComponent<Collider>());
            v.transform.SetParent(raiz.transform, false);
            v.transform.localRotation = Quaternion.Euler(90, 0, 0);
            v.transform.localScale = new Vector3(0.09f, 0.7f, 0.09f);
            v.GetComponent<MeshRenderer>().sharedMaterial = madera;
            string ruta = "Assets/_Granero/Prefabs/Proyectil_Lanza.prefab";
            GameObject g = PrefabUtility.SaveAsPrefabAsset(raiz, ruta);
            Object.DestroyImmediate(raiz);
            return g;
        }

        static GameObject CrearProyectilBola()
        {
            GameObject raiz = new GameObject("Proyectil_Bola");
            ProyectilBase p = raiz.AddComponent<ProyectilBase>();
            p.velocidad = 11f; p.radioImpacto = 0.8f; p.radioExplosion = 2.4f; p.guiado = true;
            GameObject f = Cargar(Spells + "Human_Spell_Fireball.prefab");
            if (f != null)
            {
                GameObject i = (GameObject)PrefabUtility.InstantiatePrefab(f);
                i.transform.SetParent(raiz.transform, false);
                i.transform.localPosition = Vector3.zero;
            }
            p.efectoImpacto = Cargar(Spells + "Human_Spell_Shockwave_Explosion.prefab");
            string ruta = "Assets/_Granero/Prefabs/Proyectil_Bola.prefab";
            GameObject g = PrefabUtility.SaveAsPrefabAsset(raiz, ruta);
            Object.DestroyImmediate(raiz);
            return g;
        }

        // ------------------------------------------------------------------ HUD
        static TMP_Text Texto(Transform padre, string nombre, Vector2 anclaMin, Vector2 anclaMax, Vector2 pivote, Vector2 pos, Vector2 tam, TextAlignmentOptions alin, int fuenteTam, Color color)
        {
            GameObject g = new GameObject(nombre, typeof(RectTransform));
            g.transform.SetParent(padre, false);
            RectTransform rt = g.GetComponent<RectTransform>();
            rt.anchorMin = anclaMin; rt.anchorMax = anclaMax; rt.pivot = pivote;
            rt.anchoredPosition = pos; rt.sizeDelta = tam;
            TextMeshProUGUI t = g.AddComponent<TextMeshProUGUI>();
            TMP_FontAsset f = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Granero/Fuentes/LilitaOne SDF.asset");
            if (f != null) t.font = f;
            t.fontSize = fuenteTam; t.alignment = alin; t.color = color;
            t.textWrappingMode = TextWrappingModes.NoWrap; t.overflowMode = TextOverflowModes.Overflow;
            t.raycastTarget = false;
            return t;
        }

        static void ConstruirHud(GameObject juegoObj)
        {
            GameObject cv = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler));
            Canvas canvas = cv.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler cs = cv.GetComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; cs.referenceResolution = new Vector2(1920, 1080);
            Hud hud = cv.AddComponent<Hud>();
            hud.txtGemas = Texto(cv.transform, "Gemas", new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -20), new Vector2(500, 50), TextAlignmentOptions.TopLeft, 44, new Color(1f, 0.85f, 0.3f));
            hud.txtOleada = Texto(cv.transform, "Oleada", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(900, 60), TextAlignmentOptions.Top, 44, Color.white);
            hud.txtGranero = Texto(cv.transform, "Granero", new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-30, -20), new Vector2(700, 50), TextAlignmentOptions.TopRight, 40, new Color(0.65f, 0.95f, 1f));
            hud.txtMensaje = Texto(cv.transform, "Mensaje", new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 110), new Vector2(1500, 70), TextAlignmentOptions.Bottom, 48, new Color(1f, 0.95f, 0.7f));
            hud.txtAyuda = Texto(cv.transform, "Ayuda", new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(1800, 50), TextAlignmentOptions.Bottom, 30, new Color(0.9f, 0.9f, 0.95f));

            // cartel de hover sobre las plataformas
            GameObject ph = new GameObject("CartelHover", typeof(RectTransform), typeof(Image));
            ph.transform.SetParent(cv.transform, false);
            RectTransform prt = ph.GetComponent<RectTransform>();
            prt.anchorMin = prt.anchorMax = Vector2.zero; prt.pivot = new Vector2(0, 1); prt.sizeDelta = new Vector2(700, 110);
            Image im = ph.GetComponent<Image>(); im.color = new Color(0.05f, 0.04f, 0.1f, 0.82f); im.raycastTarget = false;
            hud.panelHover = prt;
            hud.txtHover = Texto(ph.transform, "Texto", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, TextAlignmentOptions.Center, 30, new Color(1f, 0.95f, 0.75f));
            ph.SetActive(false);

            // texto central animado (oleadas, victoria, derrota)
            GameObject tc = new GameObject("TextoCentral", typeof(RectTransform), typeof(CanvasGroup));
            tc.transform.SetParent(cv.transform, false);
            RectTransform trt = tc.GetComponent<RectTransform>();
            trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 0.5f); trt.sizeDelta = new Vector2(1600, 400); trt.anchoredPosition = new Vector2(0, 120);
            CanvasGroup cg = tc.GetComponent<CanvasGroup>(); cg.alpha = 0f; cg.blocksRaycasts = false; cg.interactable = false;
            TextoCentral txc = tc.AddComponent<TextoCentral>();
            txc.grupo = cg;
            txc.titulo = Texto(tc.transform, "Titulo", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(1600, 190), TextAlignmentOptions.Center, 150, Color.white);
            txc.subtitulo = Texto(tc.transform, "Subtitulo", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -90), new Vector2(1600, 80), TextAlignmentOptions.Center, 56, new Color(1f, 0.97f, 0.88f));
        }
    }
}
