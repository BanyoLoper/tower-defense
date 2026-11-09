using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Convierte materiales de Built-in (Standard, Legacy, Particles, Unlit) a URP.
// Se ejecuta por el puente:  run GraneroTools.ConvertirURP.Todo
namespace GraneroTools
{
    public static class ConvertirURP
    {
        public static string Todo(string arg)
        {
            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Third Party Assets" });
            int convertidos = 0, saltados = 0;
            var sinMapa = new HashSet<string>();
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (string g in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(g);
                    Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (m == null || m.shader == null) continue;
                    string sn = m.shader.name;
                    if (sn.StartsWith("Universal Render Pipeline") || sn.StartsWith("Shader Graphs")) { saltados++; continue; }
                    if (Convertir(m, sn, sinMapa)) { EditorUtility.SetDirty(m); convertidos++; }
                    else saltados++;
                }
            }
            finally { AssetDatabase.StopAssetEditing(); }
            AssetDatabase.SaveAssets();
            return "convertidos=" + convertidos + " saltados=" + saltados + " sinMapa=[" + string.Join(", ", new List<string>(sinMapa).ToArray()) + "]";
        }

        static bool Convertir(Material m, string sn, HashSet<string> sinMapa)
        {
            Texture tex = m.HasProperty("_MainTex") ? m.GetTexture("_MainTex") : null;
            Color col = m.HasProperty("_Color") ? m.GetColor("_Color") : Color.white;
            Vector2 esc = m.HasProperty("_MainTex") ? m.GetTextureScale("_MainTex") : Vector2.one;
            Vector2 off = m.HasProperty("_MainTex") ? m.GetTextureOffset("_MainTex") : Vector2.zero;
            Texture normal = m.HasProperty("_BumpMap") ? m.GetTexture("_BumpMap") : null;
            Texture emis = m.HasProperty("_EmissionMap") ? m.GetTexture("_EmissionMap") : null;
            Color emisCol = m.HasProperty("_EmissionColor") ? m.GetColor("_EmissionColor") : Color.black;
            float glossy = m.HasProperty("_Glossiness") ? m.GetFloat("_Glossiness") : 0.2f;
            float metal = m.HasProperty("_Metallic") ? m.GetFloat("_Metallic") : 0f;
            bool transp = m.renderQueue >= 3000 || sn.Contains("Transparent") || sn.Contains("Fade") || sn.Contains("Alpha");
            bool particula = sn.Contains("Particles") || sn.Contains("Particle") || sn.Contains("Additive") || sn.Contains("Mobile/Particles");
            bool aditivo = sn.Contains("Additive") || sn.Contains("Add");
            bool cutout = sn.Contains("Cutout") || sn.Contains("Cutoff") || sn.Contains("TransparentCutout") || (m.HasProperty("_Cutoff") && m.IsKeywordEnabled("_ALPHATEST_ON"));
            bool unlit = sn.Contains("Unlit");

            Shader nuevo;
            if (particula) nuevo = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            else if (unlit) nuevo = Shader.Find("Universal Render Pipeline/Unlit");
            else nuevo = Shader.Find("Universal Render Pipeline/Lit");
            if (nuevo == null) { sinMapa.Add(sn); return false; }

            float corte = m.HasProperty("_Cutoff") ? m.GetFloat("_Cutoff") : 0.5f;
            int cola = m.renderQueue;

            m.shader = nuevo;
            if (m.HasProperty("_BaseMap")) { m.SetTexture("_BaseMap", tex); m.SetTextureScale("_BaseMap", esc); m.SetTextureOffset("_BaseMap", off); }
            if (m.HasProperty("_MainTex")) m.SetTexture("_MainTex", tex);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", col);
            if (m.HasProperty("_Color")) m.SetColor("_Color", col);

            if (!particula && !unlit)
            {
                if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", glossy);
                if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metal);
                if (normal != null && m.HasProperty("_BumpMap")) { m.SetTexture("_BumpMap", normal); m.EnableKeyword("_NORMALMAP"); }
                if (emis != null || emisCol.maxColorComponent > 0.01f)
                {
                    m.EnableKeyword("_EMISSION");
                    if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", emisCol);
                    if (emis != null && m.HasProperty("_EmissionMap")) m.SetTexture("_EmissionMap", emis);
                }
            }

            if (cutout)
            {
                if (m.HasProperty("_AlphaClip")) m.SetFloat("_AlphaClip", 1f);
                if (m.HasProperty("_Cutoff")) m.SetFloat("_Cutoff", corte);
                m.EnableKeyword("_ALPHATEST_ON");
                m.renderQueue = 2450;
            }
            else if (transp || (particula && aditivo))
            {
                if (m.HasProperty("_Surface")) m.SetFloat("_Surface", 1f);
                if (m.HasProperty("_Blend")) m.SetFloat("_Blend", aditivo ? 2f : 0f);
                if (m.HasProperty("_SrcBlend")) m.SetFloat("_SrcBlend", aditivo ? 1f : 5f);
                if (m.HasProperty("_DstBlend")) m.SetFloat("_DstBlend", aditivo ? 1f : 10f);
                if (m.HasProperty("_ZWrite")) m.SetFloat("_ZWrite", 0f);
                m.SetOverrideTag("RenderType", "Transparent");
                m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.renderQueue = 3000;
            }
            return true;
        }

        // Materiales planos de colores por bando.
        public static string CrearColores(string arg)
        {
            string dir = "Assets/_Granero/Materiales";
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/_Granero", "Materiales");
            Dictionary<string, Color> c = new Dictionary<string, Color>
            {
                { "Aldeano_Gris", new Color(0.55f, 0.55f, 0.58f) },
                { "Defensor_Azul", new Color(0.15f, 0.40f, 0.95f) },
                { "Enemigo_Soldado_Rojo", new Color(0.85f, 0.12f, 0.12f) },
                { "Enemigo_Bruto_Naranja", new Color(0.95f, 0.45f, 0.05f) },
                { "Enemigo_Ladron_Verde", new Color(0.15f, 0.70f, 0.20f) },
                { "Enemigo_Lancero_Amarillo", new Color(0.95f, 0.85f, 0.10f) },
                { "Enemigo_Mago_Morado", new Color(0.55f, 0.15f, 0.80f) },
                { "Enemigo_Jefe_Negro", new Color(0.10f, 0.08f, 0.10f) },
            };
            Shader lit = Shader.Find("Universal Render Pipeline/Lit");
            foreach (var kv in c)
            {
                string p = dir + "/" + kv.Key + ".mat";
                Material m = AssetDatabase.LoadAssetAtPath<Material>(p);
                if (m == null) { m = new Material(lit); AssetDatabase.CreateAsset(m, p); }
                m.shader = lit;
                m.SetColor("_BaseColor", kv.Value);
                m.SetFloat("_Smoothness", 0.25f);
                EditorUtility.SetDirty(m);
            }
            AssetDatabase.SaveAssets();
            return "materiales de color: " + c.Count;
        }
    }
}
