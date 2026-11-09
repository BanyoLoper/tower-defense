using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum FaseJuego { Preparacion, Oleada, Victoria, Derrota }

// Director DEMO del juego: maneja las fases, las oleadas (niveles) y la aparicion de agentes.
// Es la version "tonta" para poder jugar desde el dia uno. El equipo de alumnos la reemplaza con
// una maquina de estados (GameManager y Director): ver LEEME_DIRECTRICES.md.
public class Juego : MonoBehaviour
{
    public static Juego I;

    [Header("Referencias (las pone el constructor de la escena)")]
    public GameObject[] prefabsPorRol;               // indexado por (int)Rol
    public GameObject proyectilLanza, proyectilBola;
    public Transform[] puntosRuta;
    public Transform puntoGranero;
    public Agente granero;
    public Transform[] puntosMina;
    public Transform puntoReparacion;
    public Transform[] aldea;                         // nombres "<Rol>_<n>": aqui nacen aldeanos y guardias

    [Header("Reglas")]
    public int gemasIniciales = 220;
    public int bonoPorOleada = 60;

    public FaseJuego Fase { get; private set; }
    public int Nivel { get; private set; }            // oleada actual, 0-based
    public int EnemigosVivos { get; private set; }
    public bool Terminado { get { return Fase == FaseJuego.Victoria || Fase == FaseJuego.Derrota; } }
    public string Aviso { get; private set; }
    public const int TotalNiveles = 5;

    public event Action AlCambiar;
    public event Action<int> AlAcercarse;            // se acerca la oleada n (0-based): fase de preparacion
    public event Action<int> AlComenzar;             // empieza la oleada n
    public event Action<FaseJuego> AlCambiarFase;    // Preparacion / Oleada / Victoria / Derrota

    void CambiarFase(FaseJuego f)
    {
        Fase = f;
        if (AlCambiarFase != null) AlCambiarFase(f);
    }

    struct Grupo { public Rol rol; public int n; public Grupo(Rol r, int c) { rol = r; n = c; } }

    static readonly Grupo[][] oleadas =
    {
        new[] { new Grupo(Rol.Soldado, 6) },
        new[] { new Grupo(Rol.Soldado, 8), new Grupo(Rol.Lancero, 4) },
        new[] { new Grupo(Rol.Soldado, 8), new Grupo(Rol.Ladron, 5), new Grupo(Rol.MagoEnemigo, 2) },
        new[] { new Grupo(Rol.Soldado, 8), new Grupo(Rol.Lancero, 6), new Grupo(Rol.MagoEnemigo, 3), new Grupo(Rol.Bruto, 3) },
        new[] { new Grupo(Rol.Soldado, 12), new Grupo(Rol.Ladron, 4), new Grupo(Rol.Bruto, 4), new Grupo(Rol.Jefe, 1) },
    };
    static readonly float[] intervalos = { 1.6f, 1.3f, 1.1f, 1.0f, 0.8f };

    readonly Rol[] cola = new Rol[128];
    int colaN, colaI;
    float tSpawn, tRevisa, tAviso;

    void Awake()
    {
        I = this;
        Ruta.Puntos = new Vector3[puntosRuta.Length];
        for (int i = 0; i < puntosRuta.Length; i++) Ruta.Puntos[i] = puntosRuta[i].position;
        Ruta.Granero = puntoGranero.position;
    }

    // ---- ganchos para las maquinas de los alumnos ----
    // Si existe la clase DirectorController, ella manda las oleadas y este director demo se apaga.
    // Si existe TutorialController, se agrega sola a este objeto.
    public bool ControlExterno { get; private set; }
    public void Preparar(int nivel) { Nivel = nivel; CambiarFase(FaseJuego.Preparacion); if (AlAcercarse != null) AlAcercarse(nivel); NotificarCambio(); }
    public void Lanzar(int nivel) { Nivel = nivel; CambiarFase(FaseJuego.Oleada); if (AlComenzar != null) AlComenzar(nivel); NotificarCambio(); }
    public void Terminar(bool victoria) { CambiarFase(victoria ? FaseJuego.Victoria : FaseJuego.Derrota); }
    public void FijarEnemigosVivos(int n) { if (n != EnemigosVivos) { EnemigosVivos = n; NotificarCambio(); } }

    void Start()
    {
        Type td = BuscadorTipos.Buscar("DirectorController");
        if (td != null && typeof(MonoBehaviour).IsAssignableFrom(td)) { ControlExterno = true; gameObject.AddComponent(td); }
        Type tt = BuscadorTipos.Buscar("TutorialController");
        if (tt != null && typeof(MonoBehaviour).IsAssignableFrom(tt)) gameObject.AddComponent(tt);
        Economia.Reiniciar(gemasIniciales);
        Fase = FaseJuego.Preparacion;
        Nivel = 0;
        if (!ControlExterno) granero.salud.AlMorir += AlMorirGranero;
        for (int i = 0; i < aldea.Length; i++)
        {
            string n = aldea[i].name;
            int u = n.IndexOf('_');
            Rol r;
            if (u > 0 && Enum.TryParse(n.Substring(0, u), out r)) Aparecer(r, aldea[i].position, aldea[i].rotation);
        }
        if (!ControlExterno) Mensaje("Pulsa ESPACIO para llamar la oleada 1");
#if UNITY_EDITOR
        if (!ControlExterno) AutoIniciar();
#endif
    }

    void OnDestroy() { if (I == this) I = null; }

    public GameObject Aparecer(Rol rol, Vector3 pos, Quaternion rot)
    {
        GameObject pf = prefabsPorRol[(int)rol];
        if (pf == null) return null;
        return Reserva.Obtener(pf, pos, rot);
    }

    void AlMorirGranero(GameObject origen)
    {
        CambiarFase(FaseJuego.Derrota);
        Mensaje("El granero cayo. R para reiniciar");
    }

    public void Mensaje(string txt)
    {
        Aviso = txt;
        tAviso = 4f;
        NotificarCambio();
    }

    public void NotificarCambio() { if (AlCambiar != null) AlCambiar(); }

    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k != null && k.rKey.wasPressedThisFrame) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); return; }

        if (tAviso > 0f) { tAviso -= Time.deltaTime; if (tAviso <= 0f) { Aviso = null; NotificarCambio(); } }

#if UNITY_EDITOR
        if (autoPrueba) { AutoPrueba(); }
#endif
        if (ControlExterno) return;
        switch (Fase)
        {
            case FaseJuego.Preparacion:
                if (k != null && k.spaceKey.wasPressedThisFrame) IniciarOleada();
                break;
            case FaseJuego.Oleada:
                ActualizarOleada();
                break;
        }
    }

#if UNITY_EDITOR
    // Modo de prueba automatica (solo Editor): existe BridgeCmd/auto.flag -> construye torres y lanza oleadas solo.
    bool autoPrueba, estres;
    int autoContador;
    float autoEspera;
    void AutoIniciar()
    {
        string f = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), "BridgeCmd/auto.flag");
        autoPrueba = System.IO.File.Exists(f);
    }
    void AutoPrueba()
    {
        if (Fase != FaseJuego.Preparacion) return;
        autoEspera -= Time.deltaTime;
        if (autoEspera > 0f) return;
        ColocadorTorres c = FindFirstObjectByType<ColocadorTorres>();
        bool sinTorres = System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), "BridgeCmd/sintorres.flag"));
        if (!sinTorres && c != null && c.ConstruirAuto(autoContador % 3 == 2)) { autoContador++; autoEspera = 0.05f; return; }
        autoEspera = 1.5f;
        IniciarOleada();
    }
#endif

    void IniciarOleada()
    {
        Grupo[] g = oleadas[Nivel];
        int[] resto = new int[g.Length];
        for (int i = 0; i < g.Length; i++) resto[i] = g[i].n;
        colaN = 0; colaI = 0;
        bool alguno = true;
        while (alguno)
        {
            alguno = false;
            for (int i = 0; i < g.Length; i++)
                if (resto[i] > 0 && g[i].rol != Rol.Jefe && colaN < cola.Length) { cola[colaN++] = g[i].rol; resto[i]--; alguno = true; }
        }
        for (int i = 0; i < g.Length; i++)
            while (resto[i] > 0 && colaN < cola.Length) { cola[colaN++] = g[i].rol; resto[i]--; }

#if UNITY_EDITOR
        estres = System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), "BridgeCmd/estres.flag"));
        if (estres)
        {
            Rol[] mezcla = { Rol.Soldado, Rol.Soldado, Rol.Lancero, Rol.MagoEnemigo, Rol.Bruto };
            colaN = cola.Length;
            for (int i = 0; i < colaN; i++) cola[i] = mezcla[i % mezcla.Length];
        }
#endif
        tSpawn = 1f;
        CambiarFase(FaseJuego.Oleada);
        if (AlComenzar != null) AlComenzar(Nivel);
        Mensaje("Oleada " + (Nivel + 1) + " en camino");
    }

    void ActualizarOleada()
    {
        tSpawn -= Time.deltaTime;
        if (colaI < colaN && tSpawn <= 0f)
        {
            tSpawn = intervalos[Nivel];
#if UNITY_EDITOR
            if (estres) tSpawn = 0.05f;
#endif
            Vector3 p = Ruta.Inicio + new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), 0f, UnityEngine.Random.Range(-0.8f, 0.8f));
            Aparecer(cola[colaI++], p, Quaternion.LookRotation(Vector3.right));
        }

        tRevisa -= Time.deltaTime;
        if (tRevisa <= 0f)
        {
            tRevisa = 0.4f;
            int n = Sensor.Contar(Bando.Enemigo);
            if (n != EnemigosVivos) { EnemigosVivos = n; NotificarCambio(); }
            if (colaI >= colaN && n == 0) TerminarOleada();
        }
    }

    void TerminarOleada()
    {
        Nivel++;
        if (Nivel >= TotalNiveles)
        {
            CambiarFase(FaseJuego.Victoria);
            Mensaje("VICTORIA. El granero esta a salvo. R para reiniciar");
            return;
        }
        Economia.Agregar(bonoPorOleada);
        CambiarFase(FaseJuego.Preparacion);
        if (AlAcercarse != null) AlAcercarse(Nivel);
        Mensaje("Oleada superada (+" + bonoPorOleada + " gemas). Se abren plataformas nuevas. ESPACIO para la siguiente");
    }
}
