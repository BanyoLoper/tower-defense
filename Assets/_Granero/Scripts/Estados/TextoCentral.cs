using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Texto grande animado en el centro de la pantalla ("OLEADA 2 SE ACERCA", "VICTORIA"...).
// Maquina de estados: TextoOculto -> TextoEntrando -> TextoMantener -> TextoSaliendo -> (siguiente mensaje | TextoOculto)
//     TextoCentral.I.Mostrar("TITULO", "subtitulo", Color.yellow, 2f);
// Es el ejemplo de referencia para maquinas "sencillas": cada estado es una clase pequena.
public class TextoCentral : MonoBehaviour
{
    public static TextoCentral I;
    public TMP_Text titulo, subtitulo;
    public CanvasGroup grupo;

    Maquina maquina;
    readonly Contexto ctx = new Contexto();

    // cola circular de mensajes (sin basura por frame)
    struct Msg { public string t, s; public Color c; public float hold; public bool fijo; }
    readonly Msg[] cola = new Msg[8];
    int ini, n;

    void Awake()
    {
        I = this;
        ctx.Fijar("dueno", this);
        maquina = new Maquina(new List<Estado> { new TextoOculto(), new TextoEntrando(), new TextoMantener(), new TextoSaliendo() }, ctx);
        grupo.alpha = 0f;
        maquina.Iniciar("TextoOculto");
    }

    void Start()
    {
        Juego j = Juego.I;
        if (j == null) return;
        j.AlAcercarse += AlAcercarse;
        j.AlComenzar += AlComenzar;
        j.AlCambiarFase += AlFase;
        AlAcercarse(j.Nivel);
    }

    void OnDestroy()
    {
        if (I == this) I = null;
        Juego j = Juego.I;
        if (j != null) { j.AlAcercarse -= AlAcercarse; j.AlComenzar -= AlComenzar; j.AlCambiarFase -= AlFase; }
    }

    void AlAcercarse(int nivel)
    {
        Mostrar("OLEADA " + (nivel + 1), "se acerca...  prepara tus defensas", new Color(1f, 0.82f, 0.3f), 2.6f);
    }

    void AlComenzar(int nivel)
    {
        bool jefe = nivel == Juego.TotalNiveles - 1;
        Mostrar("¡OLEADA " + (nivel + 1) + "!", jefe ? "¡llega el JEFE!" : "¡que vienen!", new Color(1f, 0.3f, 0.25f), 1.6f);
    }

    void AlFase(FaseJuego f)
    {
        if (f == FaseJuego.Victoria) Mostrar("VICTORIA", "el granero esta a salvo", new Color(0.55f, 1f, 0.5f), 0f, true);
        else if (f == FaseJuego.Derrota) Mostrar("DERROTA", "el granero cayo", new Color(1f, 0.25f, 0.2f), 0f, true);
    }

    // hold = segundos en pantalla. fijo = se queda hasta reiniciar.
    public void Mostrar(string t, string s, Color c, float hold, bool fijo = false)
    {
        if (fijo) { n = 0; }   // los mensajes finales cancelan los pendientes
        if (n >= cola.Length) { ini = (ini + 1) % cola.Length; n--; }
        int k = (ini + n) % cola.Length;
        cola[k].t = t; cola[k].s = s; cola[k].c = c; cola[k].hold = hold; cola[k].fijo = fijo;
        n++;
        // si hay uno en pantalla y este es final, sale rapido
        if (maquina.Actual.Nombre != "TextoOculto" && fijo) maquina.Disparar("cortar");
    }

    void Update() { maquina.Actualizar(Time.unscaledDeltaTime); }

    // --- usado por los estados
    public bool HayPendiente { get { return n > 0; } }
    public void Poner(float alfa, float escala)
    {
        grupo.alpha = alfa;
        titulo.rectTransform.localScale = new Vector3(escala, escala, 1f);
        subtitulo.rectTransform.localScale = new Vector3(escala, escala, 1f);
    }
    public void Tomar(out float hold, out bool fijo)
    {
        Msg m = cola[ini];
        ini = (ini + 1) % cola.Length; n--;
        titulo.text = m.t; subtitulo.text = m.s;
        titulo.color = m.c;
        hold = m.hold; fijo = m.fijo;
    }
}

public class TextoOculto : Estado
{
    public override void Entrar(string anterior) { Ctx.Obtener<TextoCentral>("dueno").Poner(0f, 1f); }
    public override string Actualizar(float dt)
    {
        return Ctx.Obtener<TextoCentral>("dueno").HayPendiente ? "TextoEntrando" : null;
    }
}

public class TextoEntrando : Estado
{
    float t; const float Dur = 0.45f;
    public override void Entrar(string anterior)
    {
        t = 0f;
        float h; bool f;
        if (anterior == "TextoOculto" || anterior == "TextoSaliendo")
        {
            var d = Ctx.Obtener<TextoCentral>("dueno");
            d.Tomar(out h, out f);
            Ctx.Fijar("hold", h); Ctx.Fijar("fijo", f);
        }
    }
    public override string Actualizar(float dt)
    {
        t += dt;
        float k = Mathf.Clamp01(t / Dur);
        // rebote suave: llega grande y se asienta
        float e = 1f + 1.2f * (1f - k) * (1f - k) + 0.08f * Mathf.Sin(k * Mathf.PI);
        Ctx.Obtener<TextoCentral>("dueno").Poner(k, e);
        return k >= 1f ? "TextoMantener" : null;
    }
    public override string AlRecibir(string trigger, int valor) { return trigger == "cortar" ? "TextoSaliendo" : null; }
}

public class TextoMantener : Estado
{
    float t;
    public override void Entrar(string anterior) { t = 0f; }
    public override string Actualizar(float dt)
    {
        t += dt;
        // latido leve
        Ctx.Obtener<TextoCentral>("dueno").Poner(1f, 1f + 0.03f * Mathf.Sin(t * 6f));
        if (Ctx.Obtener<bool>("fijo")) return null;
        return t >= Ctx.Obtener<float>("hold") ? "TextoSaliendo" : null;
    }
    public override string AlRecibir(string trigger, int valor) { return trigger == "cortar" ? "TextoSaliendo" : null; }
}

public class TextoSaliendo : Estado
{
    float t; const float Dur = 0.35f;
    public override void Entrar(string anterior) { t = 0f; }
    public override string Actualizar(float dt)
    {
        t += dt;
        float k = Mathf.Clamp01(t / Dur);
        var d = Ctx.Obtener<TextoCentral>("dueno");
        d.Poner(1f - k, 1f + 0.35f * k);
        if (k < 1f) return null;
        return d.HayPendiente ? "TextoEntrando" : "TextoOculto";
    }
}
