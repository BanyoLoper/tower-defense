using UnityEditor;
using UnityEditor.SceneManagement;

namespace GraneroTools
{
    // Escena vacia solo con camara y luz, para saber cuanta basura/frame es del Editor y no del juego.
    public static class MedirBase
    {
        public static string EscenaVacia(string arg)
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            return "escena vacia lista";
        }
    }
}
