using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Convierte los materiales al shader toon con contorno de AllIn13DShader y configura el renderer URP.
//   run GraneroTools.ConvertirToon.Renderer      (una vez)
//   run GraneroTools.ConvertirToon.Todo
namespace GraneroTools
{
    public static class ConvertirToon
    {
        const string ShOutline = "AllIn13DShader/AllIn13DShaderOutline";
        const string Mats = "Assets/_Granero/Materiales/";

        // ------------------------------------------------------------------ renderer URP
        public static string Renderer(string arg)
        {
            var log = new StringBuilder();
            foreach (string g in AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset"))
            {
                string p = AssetDatabase.GUIDToAssetPath(g);
                var pipe = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(p);
                FieldInfo f = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
                var datos = f.GetValue(pipe) as ScriptableRendererData[];
                foreach (var d in datos)
                {
                    var urd = d as UniversalRendererData;
                    if (urd == null) continue;
                    CrearFeature(urd, "Render Feature - Outline Opaque", RenderQueueType.Opaque, RenderPassEvent.AfterRenderingOpaques);
                    CrearFeature(urd, "Render Feature - Outline Transparent", RenderQueueType.Transparent, RenderPassEvent.AfterRenderingTransparents);
                    log.AppendLine("renderer listo: " + AssetDatabase.GetAssetPath(urd));
                }
            }
            AssetDatabase.SaveAssets();
            return log.ToString();
        }

        static void CrearFeature(UniversalRendererData urd, string nombre, RenderQueueType cola, RenderPassEvent evento)
        {
            RenderObjects feat = null;
            foreach (var sf in urd.rendererFeatures)
            {
                var ro = sf as RenderObjects;
                if (ro == null) continue;
                foreach (string pn in ro.settings.filterSettings.PassNames)
                    if (pn == "OutlinePass" && ro.settings.filterSettings.RenderQueueType == cola) feat = ro;
            }
            if (feat == null)
            {
                feat = ScriptableObject.CreateInstance<RenderObjects>();
                feat.name = nombre;
                urd.rendererFeatures.Add(feat);
                FieldInfo mapa = typeof(UniversalRendererData).GetField("m_RendererFeatureMap", BindingFlags.Instance | BindingFlags.NonPublic);
                var lista = mapa.GetValue(urd) as List<long>;
                string guid; long local;
                AssetDatabase.AddObjectToAsset(feat, urd);
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feat, out guid, out local);
                lista.Add(local);
            }
            feat.name = nombre;
            feat.settings.filterSettings.RenderQueueType = cola;
            feat.settings.filterSettings.LayerMask = ~0;
            feat.settings.Event = evento;
            feat.settings.filterSettings.PassNames = new[] { "OutlinePass" };
            feat.SetActive(true);
            EditorUtility.SetDirty(feat);
            EditorUtility.SetDirty(urd);
        }

        // ------------------------------------------------------------------ materiales
        // grosores/colores por categoria
        class Estilo { public float grosor; public Color borde; public bool contorno = true; }

        static Estilo EstiloDe(string nombre)
        {
            if (nombre.StartsWith("Enemigo")) return new Estilo { grosor = 4.5f, borde = new Color(0.12f, 0.0f, 0.0f) };       // grueso y oscuro: amenaza
            if (nombre.StartsWith("Defensor")) return new Estilo { grosor = 3.0f, borde = new Color(0.75f, 0.92f, 1f) };        // claro: aliado
            if (nombre.StartsWith("Aldeano")) return new Estilo { grosor = 3.5f, borde = new Color(1f, 1f, 0.85f) };           // claro y grueso: proteger
            return new Estilo { grosor = 1.3f, borde = new Color(0.05f, 0.05f, 0.08f) };
        }

        public static string Todo(string arg)
        {
            var log = new StringBuilder();
            Shader sh = Shader.Find(ShOutline);
            if (sh == null) return "ERROR no existe el shader " + ShOutline;
            int n = 0, saltados = 0;
            var rutas = new List<string>();
            foreach (string g in AssetDatabase.FindAssets("t:Material", new[] { "Assets/_Granero/Materiales", "Assets/Third Party Assets/Pandazole_Lowpoly_Asset_Bundle", "Assets/Third Party Assets/Kevin Iglesias", "Assets/Third Party Assets/PurePoly", "Assets/Third Party Assets/Aztech Games" }))
                rutas.Add(AssetDatabase.GUIDToAssetPath(g));

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (string p in rutas)
                {
                    Material m = AssetDatabase.LoadAssetAtPath<Material>(p);
                    if (m == null || m.shader == null) continue;
                    string nombre = System.IO.Path.GetFileNameWithoutExtension(p);
                    bool yaToon = m.shader.name.StartsWith("AllIn13DShader");
                    if (yaToon && m.shader != sh) { saltados++; continue; }
                    if (m.renderQueue >= 3000 || m.shader.name.Contains("Particles") || p.Contains("/Spells/") || nombre.StartsWith("Slot_") || nombre.StartsWith("Proyectil") || nombre.StartsWith("Suelo") || nombre.StartsWith("Camino")) { saltados++; continue; }
                    Estilo e = EstiloDe(nombre);
                    // el suelo y el camino de tierra: sin contorno
                    if (nombre.ToLower().Contains("grassland") || nombre.ToLower().Contains("terrain")) e.contorno = false;
                    Aplicar(m, sh, e);
                    n++;
                }
            }
            finally { AssetDatabase.StopAssetEditing(); }
            AssetDatabase.SaveAssets();
            return "convertidos=" + n + " saltados=" + saltados;
        }

        static void Aplicar(Material m, Shader sh, Estilo e)
        {
            Texture tex = null; Color col = Color.white; Vector2 esc = Vector2.one, off = Vector2.zero;
            if (m.HasProperty("_BaseMap")) { tex = m.GetTexture("_BaseMap"); esc = m.GetTextureScale("_BaseMap"); off = m.GetTextureOffset("_BaseMap"); }
            else if (m.HasProperty("_MainTex")) { tex = m.GetTexture("_MainTex"); esc = m.GetTextureScale("_MainTex"); off = m.GetTextureOffset("_MainTex"); }
            if (m.HasProperty("_BaseColor")) col = m.GetColor("_BaseColor");
            else if (m.HasProperty("_Color")) col = m.GetColor("_Color");

            if (m.shader != sh)
            {
                m.shader = sh;
                m.SetTexture("_MainTex", tex);
                m.SetTextureScale("_MainTex", esc); m.SetTextureOffset("_MainTex", off);
                m.SetColor("_Color", col);
            }

            // modelo de luz toon
            m.SetFloat("_LightModel", 2f);
            foreach (string k in new[] { "_LIGHTMODEL_NONE", "_LIGHTMODEL_CLASSIC", "_LIGHTMODEL_TOONRAMP", "_LIGHTMODEL_HALFLAMBERT", "_LIGHTMODEL_FAKEGI", "_LIGHTMODEL_FASTLIGHTING" }) m.DisableKeyword(k);
            m.EnableKeyword("_LIGHTMODEL_TOON");
            m.SetFloat("_ToonCutoff", 0.5f);
            m.SetFloat("_ToonSmoothness", 0.06f);

            // contorno
            foreach (string k in new[] { "_OUTLINETYPE_NONE", "_OUTLINETYPE_SIMPLE", "_OUTLINETYPE_CONSTANT", "_OUTLINETYPE_FADEWITHDISTANCE" }) m.DisableKeyword(k);
            if (e.contorno)
            {
                m.SetFloat("_OutlineType", 2f);                 // Constant: el grosor se mantiene al alejar la camara
                m.EnableKeyword("_OUTLINETYPE_CONSTANT");
                m.SetFloat("_MaxCameraDistance", 1000f);
                m.SetColor("_OutlineColor", e.borde);
                m.SetFloat("_OutlineThickness", e.grosor);
                m.SetFloat("_OutlineMode", 8f);
                m.SetFloat("_StencilRef", 1f);
            }
            else
            {
                m.SetFloat("_OutlineType", 0f);
                m.EnableKeyword("_OUTLINETYPE_NONE");
            }
            EditorUtility.SetDirty(m);
        }
    }
}
