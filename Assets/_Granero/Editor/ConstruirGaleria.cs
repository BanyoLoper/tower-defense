using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GraneroTools
{
    public static class ConstruirGaleria
    {
        public static string Crear(string arg)
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Granero/Escenas")) AssetDatabase.CreateFolder("Assets/_Granero", "Escenas");
            Scene sc = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject luz = new GameObject("Sol");
            Light l = luz.AddComponent<Light>(); l.type = LightType.Directional; l.intensity = 1.2f;
            luz.transform.rotation = Quaternion.Euler(50, -30, 0);

            GameObject suelo = GameObject.CreatePrimitive(PrimitiveType.Plane);
            suelo.name = "Suelo"; suelo.transform.localScale = new Vector3(4, 1, 2);
            Material ms = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            ms.SetColor("_BaseColor", new Color(0.35f, 0.5f, 0.3f));
            AssetDatabase.CreateAsset(ms, "Assets/_Granero/Materiales/Suelo_Galeria.mat");
            suelo.GetComponent<MeshRenderer>().sharedMaterial = ms;

            string[] roles = { "Soldado", "Bruto", "Jefe", "Ladron", "Lancero", "MagoEnemigo", "TorreLanzadora", "TorreMaga", "Guardia", "Aldeano", "Minero", "Reparador" };
            var lista = new List<Agente>();
            for (int i = 0; i < roles.Length; i++)
            {
                GameObject p = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Granero/Prefabs/Agentes/" + roles[i] + ".prefab");
                GameObject g = (GameObject)PrefabUtility.InstantiatePrefab(p);
                g.transform.position = new Vector3((i - 5.5f) * 2.4f, 0, 0);
                g.transform.rotation = Quaternion.Euler(0, 180, 0);
                lista.Add(g.GetComponent<Agente>());
            }

            GameObject cam = new GameObject("Main Camera"); cam.tag = "MainCamera";
            cam.AddComponent<Camera>().fieldOfView = 50;
            cam.AddComponent<AudioListener>();
            cam.transform.position = new Vector3(0, 3.2f, -14f);
            cam.transform.rotation = Quaternion.Euler(8, 0, 0);

            GameObject gal = new GameObject("Galeria");
            Galeria comp = gal.AddComponent<Galeria>();
            comp.agentes = lista.ToArray();

            EditorSceneManager.SaveScene(sc, "Assets/_Granero/Escenas/Galeria.unity");
            return "Galeria creada con " + lista.Count + " agentes";
        }
    }
}
