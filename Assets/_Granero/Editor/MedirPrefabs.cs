using System.Text;
using UnityEditor;
using UnityEngine;

namespace GraneroTools
{
    // Reporta el tamano (bounds) de prefabs de entorno para planear el mapa. Salida: BridgeCmd/medidas.txt
    public static class MedirPrefabs
    {
        const string F = "Assets/Third Party Assets/Pandazole_Lowpoly_Asset_Bundle/Pandazole Farm Ranch Pack/Prefabs/";
        const string N = "Assets/Third Party Assets/Pandazole_Lowpoly_Asset_Bundle/Pandazole Nature Environment Pack/Prefabs/";

        public static string Todo(string arg)
        {
            string[] rutas = {
                F + "Env_GrassLand_Plain.prefab", F + "Env_GrassLand_Stright.prefab", F + "Env_GrassLand_Cornor.prefab", F + "Env_GrassLand_Cross.prefab",
                F + "Env_WaterLand_01.prefab", F + "Bld_Barn_01.prefab", F + "Bld_Barn_02.prefab", F + "Bld_Silo_01.prefab", F + "Bld_FarmMill_01.prefab",
                F + "Env_WoodFence_01.prefab", F + "Env_Tree_01.prefab", F + "Prop_WoodenCrates_01.prefab", F + "Prop_Haystack_01.prefab",
                N + "Jem_01.prefab", N + "HardRock_01.prefab", N + "Log_01.prefab"
            };
            var sb = new StringBuilder();
            foreach (string r in rutas)
            {
                GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>(r);
                if (g == null) { sb.AppendLine("NO EXISTE " + r); continue; }
                Bounds b = new Bounds(Vector3.zero, Vector3.zero); bool primero = true;
                foreach (Renderer rd in g.GetComponentsInChildren<Renderer>())
                {
                    Bounds lb = rd.bounds;
                    if (primero) { b = lb; primero = false; } else b.Encapsulate(lb);
                }
                sb.AppendLine(System.IO.Path.GetFileName(r) + "  centro=" + b.center + " tam=" + b.size + " hijos=" + g.transform.childCount);
            }
            System.IO.File.WriteAllText("BridgeCmd/medidas.txt", sb.ToString());
            return "ok";
        }
    }
}
