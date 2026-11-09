using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

// Instala la fuente del juego (Lilita One, licencia OFL) como TextMeshPro SDF con contorno y sombra.
//   run GraneroTools.ConfigurarFuente.Importar   (importa TMP Essentials, una vez)
//   run GraneroTools.ConfigurarFuente.Crear
namespace GraneroTools
{
    public static class ConfigurarFuente
    {
        public const string Ttf = "Assets/_Granero/Fuentes/LilitaOne-Regular.ttf";
        public const string Asset = "Assets/_Granero/Fuentes/LilitaOne SDF.asset";

        public static string Importar(string arg)
        {
            string raiz = Path.GetDirectoryName(Application.dataPath);
            string[] dirs = Directory.GetDirectories(Path.Combine(raiz, "Library/PackageCache"), "com.unity.ugui@*");
            if (dirs.Length == 0) return "ERROR no encuentro com.unity.ugui";
            string pkg = Path.Combine(dirs[0], "Package Resources/TMP Essential Resources.unitypackage");
            if (!File.Exists(pkg)) return "ERROR no existe " + pkg;
            AssetDatabase.ImportPackage(pkg, false);
            return "importando TMP Essentials";
        }

        public static string Crear(string arg)
        {
            Font f = AssetDatabase.LoadAssetAtPath<Font>(Ttf);
            if (f == null) return "ERROR no esta la fuente " + Ttf;
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Asset) != null) AssetDatabase.DeleteAsset(Asset);

            TMP_FontAsset fa = TMP_FontAsset.CreateFontAsset(f, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (fa == null) return "ERROR CreateFontAsset devolvio null";
            fa.name = "LilitaOne SDF";
            AssetDatabase.CreateAsset(fa, Asset);
            fa.atlasTextures[0].name = "LilitaOne Atlas";
            AssetDatabase.AddObjectToAsset(fa.atlasTextures[0], fa);
            fa.material.name = "LilitaOne Material";
            AssetDatabase.AddObjectToAsset(fa.material, fa);

            // caracteres de uso comun (ASCII + acentos + signos)
            string chars = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,:;!?()[]{}+-*/=<>%$#@&_'\"|/\\" +
                           "áéíóúñÁÉÍÓÚÑ¿¡üÜ";
            string perdidos;
            fa.TryAddCharacters(chars, out perdidos);

            // shader SDF con contorno oscuro y sombra suave
            Material m = fa.material;
            m.EnableKeyword("OUTLINE_ON");
            m.EnableKeyword("UNDERLAY_ON");
            m.SetFloat("_OutlineWidth", 0.22f);
            m.SetColor("_OutlineColor", new Color(0.06f, 0.04f, 0.10f, 1f));
            m.SetFloat("_UnderlayOffsetX", 0.6f);
            m.SetFloat("_UnderlayOffsetY", -0.6f);
            m.SetFloat("_UnderlaySoftness", 0.35f);
            m.SetFloat("_UnderlayDilate", 0.3f);
            m.SetColor("_UnderlayColor", new Color(0f, 0f, 0f, 0.55f));
            m.SetFloat("_FaceDilate", 0.05f);
            EditorUtility.SetDirty(m);
            EditorUtility.SetDirty(fa);
            AssetDatabase.SaveAssets();
            return "fuente creada: " + Asset + " glifos=" + fa.characterTable.Count;
        }
    }
}
