using UnityEngine;

// Ajustes globales de rendimiento. Se aplican solos al iniciar cualquier escena.
public static class AjustesRendimiento
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Aplicar()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
#if UNITY_EDITOR
        Application.runInBackground = true;   // para poder medir sin tener Unity al frente
#endif
        // El GC incremental reparte el trabajo en varios frames en vez de un pico grande.
        UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Enabled;
    }
}
