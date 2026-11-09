using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EstadoSlot { Bloqueado, Comprable, Libre, Elegido, Ocupado }

// Plataforma donde se puede construir una torre. El ColocadorTorres la selecciona con clic.
// Empieza bloqueada (candado) y se desbloquea al superar oleadas (nivelRequerido) o pagando gemas.
public class SlotTorre : MonoBehaviour
{
    public Renderer anillo;
    public Material matNormal, matSeleccionado, matOcupado, matBloqueado, matComprable;
    public GameObject candado;               // modelo del candado (solo visible mientras esta bloqueada)
    public TMP_Text txtPrecio;               // cartel junto al candado
    public int nivelRequerido;               // se abre sola cuando Juego.Nivel >= nivelRequerido (0 = abierta desde el inicio)
    public int precioDesbloqueo = 100;       // gemas para abrirla antes de tiempo
    [HideInInspector] public Agente torre;
    [HideInInspector] public int costoPagado;
    bool seleccionado, comprada, eraBloqueado;
    Maquina visual;
    readonly Contexto vctx = new Contexto();

    // --- para los estados visuales
    [HideInInspector] public bool animando;
    [HideInInspector] public Vector3 escalaAnillo, posCandado, escCandado;

    public bool Ocupado { get { return torre != null && torre.gameObject.activeSelf; } }
    public Vector3 Pos { get { return transform.position; } }
    public bool Desbloqueado { get { return comprada || nivelRequerido <= 0 || (Juego.I != null && Juego.I.Nivel >= nivelRequerido); } }

    public EstadoSlot Estado
    {
        get
        {
            if (seleccionado) return EstadoSlot.Elegido;
            if (!Desbloqueado) return Economia.Gemas >= precioDesbloqueo ? EstadoSlot.Comprable : EstadoSlot.Bloqueado;
            return Ocupado ? EstadoSlot.Ocupado : EstadoSlot.Libre;
        }
    }

    void Awake()
    {
        vctx.Fijar("dueno", this);
        visual = new Maquina(new List<Estado> { new SlotReposo(), new SlotHover(), new SlotDesbloqueo() }, vctx);
        visual.Iniciar("SlotReposo");
    }

    void Update() { visual.Actualizar(Time.deltaTime); }

    // El ColocadorTorres avisa cuando el mouse entra o sale de la plataforma.
    public void Hover(bool si) { visual.Disparar(si ? "hover" : "fuera"); }

    void Start()
    {
        if (anillo != null) escalaAnillo = anillo.transform.localScale;
        if (candado != null) { posCandado = candado.transform.localPosition; escCandado = candado.transform.localScale; }
        eraBloqueado = !Desbloqueado;
        if (txtPrecio != null) txtPrecio.text = "Oleada " + nivelRequerido + "\n" + precioDesbloqueo + " gemas";
        Pintar();
    }

    public void Seleccionar(bool si)
    {
        seleccionado = si;
        Pintar();
    }

    // Abre la plataforma pagando. Devuelve false si ya estaba abierta o faltan gemas.
    public bool Comprar()
    {
        if (Desbloqueado) return false;
        if (!Economia.Gastar(precioDesbloqueo)) return false;
        comprada = true;
        Pintar();
        return true;
    }

    public void Pintar()
    {
        bool abierta = Desbloqueado;
        if (eraBloqueado && abierta) { eraBloqueado = false; animando = true; visual.Disparar("desbloquear"); }
        if (candado != null && !animando && candado.activeSelf == abierta) candado.SetActive(!abierta);
        if (anillo == null) return;
        Material m;
        switch (Estado)
        {
            case EstadoSlot.Elegido: m = matSeleccionado; break;
            case EstadoSlot.Ocupado: m = matOcupado; break;
            case EstadoSlot.Bloqueado: m = matBloqueado; break;
            case EstadoSlot.Comprable: m = matComprable; break;
            default: m = matNormal; break;
        }
        if (m != null && anillo.sharedMaterial != m) anillo.sharedMaterial = m;
    }
}

// ---- estados visuales de la plataforma (otra maquina pequena de referencia) ----
public class SlotReposo : Estado
{
    public override void Entrar(string anterior)
    {
        var s = Ctx.Obtener<SlotTorre>("dueno");
        if (s.anillo != null && s.escalaAnillo != Vector3.zero) s.anillo.transform.localScale = s.escalaAnillo;
    }
    public override string AlRecibir(string trigger, int valor)
    {
        if (trigger == "hover") return "SlotHover";
        if (trigger == "desbloquear") return "SlotDesbloqueo";
        return null;
    }
}

public class SlotHover : Estado
{
    float t;
    public override void Entrar(string anterior) { t = 0f; }
    public override void Salir(string siguiente)
    {
        var s = Ctx.Obtener<SlotTorre>("dueno");
        if (s.anillo != null && s.escalaAnillo != Vector3.zero) s.anillo.transform.localScale = s.escalaAnillo;
    }
    public override string Actualizar(float dt)
    {
        t += dt;
        var s = Ctx.Obtener<SlotTorre>("dueno");
        if (s.anillo != null) s.anillo.transform.localScale = s.escalaAnillo * (1.1f + 0.05f * Mathf.Sin(t * 8f));
        return null;
    }
    public override string AlRecibir(string trigger, int valor)
    {
        if (trigger == "fuera") return "SlotReposo";
        if (trigger == "desbloquear") return "SlotDesbloqueo";
        return null;
    }
}

// El candado sale volando y se encoge; el anillo da un pulso y suena un efecto.
public class SlotDesbloqueo : Estado
{
    float t; const float Dur = 0.7f;
    public override void Entrar(string anterior)
    {
        t = 0f;
        var s = Ctx.Obtener<SlotTorre>("dueno");
        Efectos.Reproducir(Fx.Desbloqueo, s.Pos + Vector3.up * 0.3f, 1.2f, 2.5f);
    }
    public override void Salir(string siguiente)
    {
        var s = Ctx.Obtener<SlotTorre>("dueno");
        if (s.candado != null) { s.candado.SetActive(false); s.candado.transform.localPosition = s.posCandado; s.candado.transform.localScale = s.escCandado; }
        if (s.anillo != null) s.anillo.transform.localScale = s.escalaAnillo;
        s.animando = false;
    }
    public override string Actualizar(float dt)
    {
        t += dt;
        var s = Ctx.Obtener<SlotTorre>("dueno");
        float k = Mathf.Clamp01(t / Dur);
        if (s.anillo != null) s.anillo.transform.localScale = s.escalaAnillo * (1f + 0.55f * Mathf.Sin(k * Mathf.PI));
        if (s.candado != null)
        {
            s.candado.transform.localPosition = s.posCandado + Vector3.up * (2.2f * k * k);
            s.candado.transform.localScale = s.escCandado * (1f - k);
            s.candado.transform.Rotate(0f, 540f * dt, 0f, Space.Self);
        }
        return k >= 1f ? "SlotReposo" : null;
    }
    public override string AlRecibir(string trigger, int valor) { return null; }
}
