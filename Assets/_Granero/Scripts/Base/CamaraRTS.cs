using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Camara de estrategia + maquina de estados de camara.
//   CamLibre      : WASD / flechas para mover, rueda para acercar.
//   CamSacudida   : temblor corto cuando el granero recibe un golpe fuerte (vuelve sola a CamLibre).
//   CamVictoria   : se acerca al granero y orbita despacio.
//   CamDerrota    : se acerca al granero que cae, con temblor leve.
// Disparadores: "sacudir" (valor = fuerza), "victoria", "derrota", "libre".
public class CamaraRTS : MonoBehaviour
{
    public static CamaraRTS I;

    public float velocidad = 25f;
    public Vector2 limitesX = new Vector2(-40f, 40f);
    public Vector2 limitesZ = new Vector2(-45f, 10f);
    public Vector2 alturas = new Vector2(14f, 55f);

    Maquina maquina;
    readonly Contexto ctx = new Contexto();

    void Awake()
    {
        I = this;
        ctx.Fijar("dueno", this);
        maquina = new Maquina(new List<Estado> { new CamLibre(), new CamSacudida(), new CamVictoria(), new CamDerrota() }, ctx);
        maquina.Iniciar("CamLibre");
    }

    void Start()
    {
        if (Juego.I != null) Juego.I.AlCambiarFase += AlFase;
    }

    void OnDestroy()
    {
        if (I == this) I = null;
        if (Juego.I != null) Juego.I.AlCambiarFase -= AlFase;
    }

    void AlFase(FaseJuego f)
    {
        if (f == FaseJuego.Victoria) maquina.Disparar("victoria");
        else if (f == FaseJuego.Derrota) maquina.Disparar("derrota");
    }

    public void Sacudir(float fuerza) { maquina.Disparar("sacudir", Mathf.RoundToInt(fuerza)); }

    void Update() { maquina.Actualizar(Time.unscaledDeltaTime); }

    // movimiento del jugador (lo usan CamLibre y CamSacudida)
    public void MoverLibre(float dt)
    {
        Keyboard k = Keyboard.current;
        Mouse m = Mouse.current;
        if (k == null) return;
        float x = 0f, z = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
        if (k.sKey.isPressed || k.downArrowKey.isPressed) z -= 1f;
        if (k.wKey.isPressed || k.upArrowKey.isPressed) z += 1f;

        Vector3 p = transform.position;
        p.x += x * velocidad * dt;
        p.z += z * velocidad * dt;
        if (m != null)
        {
            float rueda = m.scroll.ReadValue().y;
            if (rueda != 0f) p.y -= Mathf.Sign(rueda) * 4f;
        }
        p.x = Mathf.Clamp(p.x, limitesX.x, limitesX.y);
        p.z = Mathf.Clamp(p.z, limitesZ.x, limitesZ.y);
        p.y = Mathf.Clamp(p.y, alturas.x, alturas.y);
        transform.position = p;
    }

    // Punto del granero hacia donde mira la camara en las escenas finales.
    public Vector3 PuntoGranero()
    {
        Juego j = Juego.I;
        return (j != null && j.granero != null ? j.granero.transform.position : new Vector3(38f, 0.5f, 2f)) + Vector3.up * 4f;
    }
}

public class CamLibre : Estado
{
    public override string Actualizar(float dt)
    {
        Ctx.Obtener<CamaraRTS>("dueno").MoverLibre(dt);
        return null;
    }
    public override string AlRecibir(string trigger, int valor)
    {
        if (trigger == "sacudir") { Ctx.Fijar("fuerza", valor); return "CamSacudida"; }
        if (trigger == "victoria") return "CamVictoria";
        if (trigger == "derrota") return "CamDerrota";
        return null;
    }
}

public class CamSacudida : Estado
{
    float t; Quaternion baseRot; const float Dur = 0.35f;
    public override void Entrar(string anterior) { t = Dur; baseRot = Ctx.Obtener<CamaraRTS>("dueno").transform.rotation; }
    public override void Salir(string siguiente) { Ctx.Obtener<CamaraRTS>("dueno").transform.rotation = baseRot; }
    public override string Actualizar(float dt)
    {
        var c = Ctx.Obtener<CamaraRTS>("dueno");
        c.MoverLibre(dt);
        t -= dt;
        if (t <= 0f) return "CamLibre";
        float a = Mathf.Clamp(Ctx.Obtener<int>("fuerza") / 70f, 0.25f, 1f) * 1.2f * (t / Dur);
        c.transform.rotation = baseRot * Quaternion.Euler(Random.Range(-a, a), Random.Range(-a, a), 0f);
        return null;
    }
    public override string AlRecibir(string trigger, int valor)
    {
        if (trigger == "victoria") return "CamVictoria";
        if (trigger == "derrota") return "CamDerrota";
        if (trigger == "sacudir") { t = Dur; return null; }
        return null;
    }
}

// Vuelo hacia el granero (2.5 s) y orbita lenta.
public class CamVictoria : Estado
{
    float t, ang; Vector3 p0; Quaternion r0;
    public override void Entrar(string anterior)
    {
        var c = Ctx.Obtener<CamaraRTS>("dueno");
        t = 0f; p0 = c.transform.position; r0 = c.transform.rotation;
        Vector3 d = p0 - c.PuntoGranero(); d.y = 0f;
        ang = Mathf.Atan2(d.z, d.x);
    }
    public override string Actualizar(float dt)
    {
        var c = Ctx.Obtener<CamaraRTS>("dueno");
        t += dt;
        Vector3 centro = c.PuntoGranero();
        float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / 2.5f));
        if (t > 2.5f) ang += dt * 0.25f;
        Vector3 meta = centro + new Vector3(Mathf.Cos(ang) * 26f, 12f, Mathf.Sin(ang) * 26f);
        c.transform.position = Vector3.Lerp(p0, meta, k);
        Quaternion mira = Quaternion.LookRotation(centro - c.transform.position);
        c.transform.rotation = Quaternion.Slerp(r0, mira, k);
        return null;
    }
}

// Se acerca al granero herido con un temblor leve.
public class CamDerrota : Estado
{
    float t; Vector3 p0; Quaternion r0;
    public override void Entrar(string anterior)
    {
        var c = Ctx.Obtener<CamaraRTS>("dueno");
        t = 0f; p0 = c.transform.position; r0 = c.transform.rotation;
    }
    public override string Actualizar(float dt)
    {
        var c = Ctx.Obtener<CamaraRTS>("dueno");
        t += dt;
        Vector3 centro = c.PuntoGranero();
        float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / 3f));
        Vector3 meta = centro + new Vector3(-17f, 10f, -9f);
        c.transform.position = Vector3.Lerp(p0, meta, k);
        Quaternion mira = Quaternion.LookRotation(centro - c.transform.position);
        float a = 0.35f * k;
        c.transform.rotation = Quaternion.Slerp(r0, mira, k) * Quaternion.Euler(Random.Range(-a, a), Random.Range(-a, a), 0f);
        return null;
    }
}
