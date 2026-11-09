using System.Collections.Generic;
using UnityEngine;

// Proyectil generico (lanzas, rocas, bolas de hechizo). Vuela hacia el objetivo y le hace dano.
// Version base: "homing" simple. La torre lanzadora de los alumnos puede reemplazar la guia con un
// Seek propio (guiado = false y mover con SetVelocidad). Usa Reserva: no se destruye, se devuelve.
public class ProyectilBase : MonoBehaviour
{
    public float velocidad = 14f;
    public float dano = 15f;
    public float vidaMaxSeg = 6f;
    public float radioImpacto = 0.6f;
    public float radioExplosion = 0f;     // >0: dano en area
    public bool guiado = true;
    public Bando bandoOrigen;
    public GameObject efectoImpacto;

    [HideInInspector] public Agente objetivo;
    [HideInInspector] public GameObject origen;
    Vector3 dir = Vector3.forward;
    float edad;
    static readonly List<Agente> temporal = new List<Agente>(32);

    public void Lanzar(Agente quien, Agente destino, Bando bando, float danoFinal)
    {
        origen = quien != null ? quien.gameObject : null;
        bandoOrigen = bando;
        objetivo = destino;
        dano = danoFinal;
        edad = 0f;
        if (destino != null) dir = (destino.PuntoPecho - transform.position).normalized;
        else dir = transform.forward;
        if (dir.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetVelocidad(Vector3 v)
    {
        dir = v.normalized;
        velocidad = v.magnitude;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        edad += dt;
        if (edad > vidaMaxSeg) { Terminar(); return; }

        if (guiado && objetivo != null && objetivo.Vivo)
        {
            Vector3 hacia = (objetivo.PuntoPecho - transform.position).normalized;
            dir = Vector3.RotateTowards(dir, hacia, 6f * dt, 0f).normalized;
        }

        transform.position += dir * velocidad * dt;
        if (dir.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(dir);

        // impacto por distancia (sin colliders, sin lambdas)
        Agente a = Sensor.HostilMasCercanoDeBando(transform.position, radioImpacto + 1.5f, bandoOrigen);
        if (a != null)
        {
            Vector3 d = a.PuntoPecho - transform.position;
            if (d.magnitude < radioImpacto + a.radio) { Impactar(a); return; }
        }
        if (transform.position.y < -0.5f) Impactar(null);
    }

    void Impactar(Agente a)
    {
        if (radioExplosion > 0f)
        {
            Sensor.HostilesEnRadio(transform.position, radioExplosion, bandoOrigen, temporal);
            for (int i = 0; i < temporal.Count; i++) temporal[i].salud.Danar(dano, origen);
        }
        else if (a != null) a.salud.Danar(dano, origen);

        if (efectoImpacto != null)
        {
            GameObject fx = Reserva.Obtener(efectoImpacto, transform.position, Quaternion.identity);
            Reserva.DevolverEn(fx, 2f);
        }
        Terminar();
    }

    void Terminar()
    {
        Reserva.Devolver(gameObject);
    }
}
