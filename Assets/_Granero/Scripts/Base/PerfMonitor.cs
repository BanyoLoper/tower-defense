using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Profiling;
using UnityEngine;

// Mide rendimiento real en juego: tiempos de frame, basura (GC) por frame y crecimiento de memoria.
// Solo en el Editor, y solo si existe el archivo BridgeCmd/perf.flag (lo crea el puente de pruebas).
// Escribe BridgeCmd/perf.txt al terminar.
public class PerfMonitor : MonoBehaviour
{
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Arrancar()
    {
        string raizP = Path.GetDirectoryName(Application.dataPath);
        string uiFlag = Path.Combine(raizP, "BridgeCmd/uishot.flag");
        if (File.Exists(uiFlag))
        {
            GameObject u = new GameObject("UiShots");
            DontDestroyOnLoad(u);
            u.AddComponent<UiShots>().segundos = File.ReadAllText(uiFlag).Trim().Split(',');
            File.Delete(uiFlag);
        }
        string flag = Path.Combine(Path.GetDirectoryName(Application.dataPath), "BridgeCmd/perf.flag");
        if (!File.Exists(flag)) return;
        float seg = 15f;
        float.TryParse(File.ReadAllText(flag).Trim(), out seg);
        if (seg < 3f) seg = 15f;
        GameObject g = new GameObject("PerfMonitor");
        DontDestroyOnLoad(g);
        g.AddComponent<PerfMonitor>().duracion = seg;
        File.Delete(flag);
    }

    public float duracion = 15f;
    ProfilerRecorder gcFrame, gcCount, mainThread;
    readonly List<float> frames = new List<float>(4096);
    readonly List<long> gcBytes = new List<long>(4096);
    float t, calentamiento = 2f;
    int picoAgentes;
    long memIni, memFin;
    int gcColecciones0;

    void OnEnable()
    {
        if (File.Exists(Path.Combine(Path.GetDirectoryName(Application.dataPath), "BridgeCmd/perf.sinpuente"))) UnityEditor.SessionState.SetBool("Granero.SinPuente", true);
        gcFrame = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        gcCount = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocation In Frame Count");
        mainThread = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
    }

    void OnDisable()
    {
        gcFrame.Dispose(); gcCount.Dispose(); mainThread.Dispose();
    }

    void Update()
    {
        if (calentamiento > 0f)
        {
            calentamiento -= Time.unscaledDeltaTime;
            if (calentamiento <= 0f) { memIni = System.GC.GetTotalMemory(false); gcColecciones0 = System.GC.CollectionCount(0); }
            return;
        }
        if (Agente.Todos.Count > picoAgentes) picoAgentes = Agente.Todos.Count;
        frames.Add(Time.unscaledDeltaTime * 1000f);
        gcBytes.Add(gcFrame.Valid ? gcFrame.LastValue : -1);
        t += Time.unscaledDeltaTime;
        if (t >= duracion) Terminar();
    }

    void Terminar()
    {
        memFin = System.GC.GetTotalMemory(false);
        int colecc = System.GC.CollectionCount(0) - gcColecciones0;
        var orden = new List<float>(frames); orden.Sort();
        float suma = 0f; int lentos = 0, muyLentos = 0;
        foreach (float f in frames) { suma += f; if (f > 16.7f) lentos++; if (f > 33.4f) muyLentos++; }
        long gcTotal = 0, gcMax = 0; int framesConGc = 0;
        foreach (long b in gcBytes) { if (b > 0) { gcTotal += b; framesConGc++; if (b > gcMax) gcMax = b; } }

        var sb = new StringBuilder();
        sb.AppendLine("agentes simultaneos (pico): " + picoAgentes + "   fase final: " + (Juego.I != null ? Juego.I.Fase.ToString() + " nivel " + Juego.I.Nivel : "-"));
        if (Juego.I != null) sb.AppendLine("granero vida: " + Juego.I.granero.salud.vida);
        sb.AppendLine("frames: " + frames.Count + " en " + t.ToString("F1") + " s");
        sb.AppendLine("fps promedio: " + (frames.Count / t).ToString("F1"));
        sb.AppendLine("frame ms  prom=" + (suma / frames.Count).ToString("F2") + "  p50=" + orden[orden.Count / 2].ToString("F2") +
            "  p99=" + orden[(int)(orden.Count * 0.99f)].ToString("F2") + "  max=" + orden[orden.Count - 1].ToString("F2"));
        sb.AppendLine("frames > 16.7 ms: " + lentos + "   > 33.4 ms: " + muyLentos);
        int q = frames.Count / 4; float ini = 0f, fin = 0f;
        for (int k = 0; k < q; k++) { ini += frames[k]; fin += frames[frames.Count - 1 - k]; }
        sb.AppendLine("tendencia: primer cuarto " + (ini / q).ToString("F2") + " ms -> ultimo cuarto " + (fin / q).ToString("F2") + " ms");
        sb.AppendLine("GC alloc/frame: prom="+ (gcTotal / (float)frames.Count).ToString("F0") + " B  max=" + gcMax + " B  frames con alloc=" + framesConGc + "/" + frames.Count);
        sb.AppendLine("colecciones GC gen0 en la prueba: " + colecc);
        sb.AppendLine("memoria manejada: " + (memIni / 1024) + " KB -> " + (memFin / 1024) + " KB (delta " + ((memFin - memIni) / 1024) + " KB)");
        sb.AppendLine("targetFrameRate=" + Application.targetFrameRate + " vSync=" + QualitySettings.vSyncCount);
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), "BridgeCmd/perf.txt"), sb.ToString());
        UnityEditor.SessionState.SetBool("Granero.SinPuente", false);
        UnityEditor.EditorApplication.isPlaying = false;
        Destroy(gameObject);
    }
#endif
}

#if UNITY_EDITOR
// Solo Editor: captura el Game view (con el HUD) en los segundos indicados en BridgeCmd/uishot.flag ("20,40,60").
public class UiShots : MonoBehaviour
{
    public string[] segundos;
    int i; float t;
    void Update()
    {
        t += Time.unscaledDeltaTime;
        if (i < segundos.Length && t >= float.Parse(segundos[i]))
        {
            ScreenCapture.CaptureScreenshot(Path.Combine(Path.GetDirectoryName(Application.dataPath), "BridgeCmd/ui_" + i + ".png"));
            i++;
        }
    }
}
#endif
