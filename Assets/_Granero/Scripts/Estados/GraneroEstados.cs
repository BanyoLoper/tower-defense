using System.Collections.Generic;
using UnityEngine;

// Aspecto del granero segun su vida, como maquina de estados.
//   GraneroSano (>66%) -> GraneroDanado (>33%) -> GraneroCritico (>0%) -> GraneroDestruido
//   GraneroDestello: destello rojo a todo el edificio cuando recibe un golpe; despues vuelve al estado que toque.
// El destello usa el efecto "Hit" del shader AllIn13D (_HitColor / _HitGlow / _HitBlend) por MaterialPropertyBlock.
// Los cristales del techo se rompen conforme baja la vida (y reaparecen si el reparador cura).
public class GraneroEstados : MonoBehaviour
{
    Maquina maquina;
    readonly Contexto ctx = new Contexto();
    Salud salud;
    Renderer[] rends;          // todo el edificio + cristales (los que tienen shader con Hit)
    GameObject[] gemas;        // cristales en el orden en que se rompen (el grande, al final)
    bool[] rota;
    int rotas;
    MaterialPropertyBlock mpb;
    static readonly int IdBlend = Shader.PropertyToID("_HitBlend");
    static readonly int IdColor = Shader.PropertyToID("_HitColor");
    static readonly int IdGlow = Shader.PropertyToID("_HitGlow");
    float tCamara;

    public string EstadoActual { get { return maquina != null && maquina.Actual != null ? maquina.Actual.Nombre : ""; } }

    void Awake()
    {
        salud = GetComponent<Salud>();
        mpb = new MaterialPropertyBlock();
        ctx.Fijar("dueno", this);
        maquina = new Maquina(new List<Estado> { new GraneroSano(), new GraneroDanado(), new GraneroCritico(), new GraneroDestello(), new GraneroDestruido() }, ctx);
    }

    void Start()
    {
        // renderers
        var lista = new List<Renderer>();
        foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
        {
            if (r is ParticleSystemRenderer) continue;
            Material m = r.sharedMaterial;
            if (m != null && m.HasProperty(IdBlend)) lista.Add(r);
        }
        rends = lista.ToArray();

        // cristales: los chicos "GemaTecho" en orden pseudoaleatorio fijo, el grande al final
        var g = new List<GameObject>();
        GameObject grande = null;
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
        {
            if (t.name == "GemaTecho") g.Add(t.gameObject);
            else if (t.name == "CristalDeLaAldea") grande = t.gameObject;
        }
        var rng = new System.Random(5);
        for (int i = g.Count - 1; i > 0; i--) { int j = rng.Next(i + 1); GameObject tmp = g[i]; g[i] = g[j]; g[j] = tmp; }
        if (grande != null) g.Add(grande);
        gemas = g.ToArray();
        rota = new bool[gemas.Length];

        salud.AlDanar += AlDanar;
        salud.AlCurar += AlCurar;
        salud.AlMorir += AlMorir;
        ActualizarGemas();
        maquina.Iniciar(NombrePorSalud());
    }

    void OnDestroy()
    {
        if (salud != null) { salud.AlDanar -= AlDanar; salud.AlCurar -= AlCurar; salud.AlMorir -= AlMorir; }
    }

    void Update() { maquina.Actualizar(Time.deltaTime); }

    // ---- eventos
    void AlDanar(float cantidad, GameObject origen)
    {
        ActualizarGemas();
        if (cantidad >= 20f && Time.time > tCamara && CamaraRTS.I != null) { tCamara = Time.time + 0.5f; CamaraRTS.I.Sacudir(cantidad); }
        maquina.Disparar("dano", Mathf.RoundToInt(cantidad));
    }
    void AlCurar(float cantidad) { ActualizarGemas(); maquina.Disparar("salud"); }
    void AlMorir(GameObject origen) { ActualizarGemas(); maquina.Disparar("muerte"); }

    // ---- servicios para los estados
    public int Porcentaje { get { return Mathf.CeilToInt(salud.Porcentaje * 100f); } }

    public string NombrePorSalud()
    {
        int p = Porcentaje;
        if (salud.EstaMuerto || p <= 0) return "GraneroDestruido";
        if (p > 66) return "GraneroSano";
        if (p > 33) return "GraneroDanado";
        return "GraneroCritico";
    }

    // Pinta el efecto Hit en todo el edificio. blend 0 = normal (y se quita el bloque: vuelve el batching).
    public void Pintar(float blend, Color color, float brillo)
    {
        if (rends == null) return;
        if (blend <= 0.001f)
        {
            for (int i = 0; i < rends.Length; i++) rends[i].SetPropertyBlock(null);
            return;
        }
        mpb.SetColor(IdColor, color);
        mpb.SetFloat(IdGlow, brillo);
        mpb.SetFloat(IdBlend, blend);
        for (int i = 0; i < rends.Length; i++)
            if (rends[i] != null && rends[i].gameObject.activeInHierarchy) rends[i].SetPropertyBlock(mpb);
    }

    // Rompe o repone cristales segun la vida.
    void ActualizarGemas()
    {
        if (gemas == null) return;
        int objetivo = Mathf.RoundToInt((1f - salud.Porcentaje) * gemas.Length);
        if (salud.EstaMuerto) objetivo = gemas.Length;
        while (rotas < objetivo)
        {
            GameObject g = gemas[rotas];
            Efectos.Reproducir(Fx.Impacto, g.transform.position, rotas == gemas.Length - 1 ? 3f : 1f, 1.5f);
            g.SetActive(false); rota[rotas] = true; rotas++;
        }
        while (rotas > objetivo)
        {
            rotas--;
            GameObject g = gemas[rotas];
            g.SetActive(true); rota[rotas] = false;
            Efectos.Reproducir(Fx.Curacion, g.transform.position, 0.6f, 1.5f);
        }
    }
}

public class GraneroSano : Estado
{
    public override void Entrar(string anterior) { Ctx.Obtener<GraneroEstados>("dueno").Pintar(0f, Color.white, 1f); }
    public override string AlRecibir(string trigger, int valor)
    {
        if (trigger == "dano") return "GraneroDestello";
        if (trigger == "salud" || trigger == "muerte") return Ctx.Obtener<GraneroEstados>("dueno").NombrePorSalud();
        return null;
    }
}

public class GraneroDanado : Estado
{
    public override void Entrar(string anterior) { Ctx.Obtener<GraneroEstados>("dueno").Pintar(0.04f, new Color(0.9f, 0.15f, 0.1f), 1.2f); }
    public override string AlRecibir(string trigger, int valor)
    {
        if (trigger == "dano") return "GraneroDestello";
        if (trigger == "salud" || trigger == "muerte") return Ctx.Obtener<GraneroEstados>("dueno").NombrePorSalud();
        return null;
    }
}

// Poca vida: palpita en rojo como alarma (a 15 actualizaciones por segundo).
public class GraneroCritico : Estado
{
    float t, paso;
    public override void Entrar(string anterior) { t = 0f; paso = 0f; }
    public override string Actualizar(float dt)
    {
        t += dt; paso -= dt;
        if (paso <= 0f)
        {
            paso = 1f / 15f;
            float b = 0.06f + 0.14f * (0.5f + 0.5f * Mathf.Sin(t * 5f));
            Ctx.Obtener<GraneroEstados>("dueno").Pintar(b, new Color(1f, 0.08f, 0.05f), 1.5f);
        }
        return null;
    }
    public override string AlRecibir(string trigger, int valor)
    {
        if (trigger == "dano") return "GraneroDestello";
        if (trigger == "salud" || trigger == "muerte") return Ctx.Obtener<GraneroEstados>("dueno").NombrePorSalud();
        return null;
    }
}

// Destello rojo intenso que se apaga rapido. Un golpe nuevo lo reinicia.
public class GraneroDestello : Estado
{
    float t; const float Dur = 0.4f;
    public override void Entrar(string anterior) { t = Dur; }
    public override string Actualizar(float dt)
    {
        t -= dt;
        if (t <= 0f) return Ctx.Obtener<GraneroEstados>("dueno").NombrePorSalud();
        Ctx.Obtener<GraneroEstados>("dueno").Pintar(0.45f * (t / Dur), new Color(1f, 0.05f, 0.02f), 1.3f);
        return null;
    }
    public override string AlRecibir(string trigger, int valor)
    {
        if (trigger == "dano") { t = Dur; return null; }
        if (trigger == "muerte") return "GraneroDestruido";
        return null;
    }
}

// Quemado y oscuro: sin cristales.
public class GraneroDestruido : Estado
{
    public override void Entrar(string anterior)
    {
        var d = Ctx.Obtener<GraneroEstados>("dueno");
        d.Pintar(0.55f, new Color(0.05f, 0.04f, 0.04f), 1f);
        Efectos.Reproducir(Fx.Explosion, d.transform.position + Vector3.up * 4f, 3f, 3f);
    }
}
