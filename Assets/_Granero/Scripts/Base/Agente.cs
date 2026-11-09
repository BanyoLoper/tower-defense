using System;
using System.Collections.Generic;
using UnityEngine;

// Identidad de cualquier cosa "viva" del juego (enemigo, defensor, aldeano, granero).
// Une Salud + Cuerpo + Animator y ofrece Animar(). Los controllers de los alumnos viven en el
// mismo GameObject y usan GetComponent<Agente>().
[RequireComponent(typeof(Salud))]
public class Agente : MonoBehaviour
{
    public static readonly List<Agente> Todos = new List<Agente>();
    public static event Action<Agente> AlMorirAlguien;

    public Rol rol;
    public Bando bando;
    public float alturaPecho = 1.3f;       // a donde apuntan los proyectiles
    public float radio = 0.5f;             // tamano para alcances de ataque
    public int recompensaGemas = 0;        // gemas que suelta al morir
    public float vistaMetros = 12f;        // radio de percepcion sugerido

    // Los llena el constructor de prefabs. Sirven para Animar() y para la galeria.
    public string[] estadosAnimacion = new string[0];
    public float[] duracionesAnimacion = new float[0];

    [HideInInspector] public Animator animador;
    [HideInInspector] public Salud salud;
    [HideInInspector] public Cuerpo cuerpo;

    string estadoActual;
    Collider bordeCol;
    bool bordeBuscado;
    static readonly HashSet<string> avisados = new HashSet<string>();

    public bool Vivo { get { return salud != null && !salud.EstaMuerto; } }
    public Vector3 Pos { get { return transform.position; } }
    public Vector3 PuntoPecho { get { return transform.position + Vector3.up * alturaPecho; } }
    public string EstadoAnimacion { get { return estadoActual; } }

    // Distancia plana desde un punto hasta el BORDE de este agente (usa el collider si es un edificio grande).
    public float DistanciaBorde(Vector3 p)
    {
        if (!bordeBuscado) { bordeBuscado = true; if (rol == Rol.Granero) bordeCol = GetComponentInChildren<BoxCollider>(); }
        if (bordeCol != null)
        {
            Vector3 c = bordeCol.ClosestPoint(new Vector3(p.x, bordeCol.bounds.center.y, p.z));
            float dx = c.x - p.x, dz = c.z - p.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }
        return Mathf.Max(0f, Sensor.DistanciaPlana(p, transform.position) - radio);
    }

    void Awake()
    {
        salud = GetComponent<Salud>();
        cuerpo = GetComponent<Cuerpo>();
        animador = GetComponentInChildren<Animator>();
        if (animador != null) animador.applyRootMotion = false;
        salud.AlMorir += AlMorirSalud;
    }

    void OnEnable()
    {
        if (!Todos.Contains(this)) Todos.Add(this);
        estadoActual = null;
    }

    void OnDisable()
    {
        Todos.Remove(this);
    }

    void AlMorirSalud(GameObject origen)
    {
        if (recompensaGemas > 0) Economia.Agregar(recompensaGemas);
        if (AlMorirAlguien != null) AlMorirAlguien(this);
    }

    // ---------------------------------------------------------------- bandos
    public static bool Hostiles(Bando a, Bando b)
    {
        if (a == Bando.Enemigo) return b == Bando.Defensor || b == Bando.Aldeano;
        if (a == Bando.Defensor || a == Bando.Aldeano) return b == Bando.Enemigo;
        return false;
    }

    public bool EsHostil(Agente otro)
    {
        if (otro == null || otro == this) return false;
        return Hostiles(bando, otro.bando);
    }

    // ---------------------------------------------------------------- animacion
    public bool TieneAnimacion(string estado)
    {
        return Array.IndexOf(estadosAnimacion, estado) >= 0;
    }

    // Pide una animacion por el nombre de estado del Animator (usa las constantes de Anim).
    // Si ya esta sonando esa animacion no la reinicia, salvo reiniciar = true (ataques repetidos).
    public void Animar(string estado, float fade = 0.15f, bool reiniciar = false)
    {
        if (animador == null) return;

        if (!TieneAnimacion(estado))
        {
            string clave = rol + ":" + estado;
            if (avisados.Add(clave)) Debug.LogWarning("[" + name + "] el rol " + rol + " no tiene la animacion '" + estado + "'");
            return;
        }

        if (!reiniciar && estadoActual == estado) return;
        estadoActual = estado;
        animador.CrossFadeInFixedTime(estado, fade, 0, 0f);
    }

    // Duracion en segundos del clip de ese estado (0 si no existe). Util para temporizadores.
    public float Duracion(string estado)
    {
        int i = Array.IndexOf(estadosAnimacion, estado);
        if (i < 0 || i >= duracionesAnimacion.Length) return 0f;
        return duracionesAnimacion[i];
    }
}
