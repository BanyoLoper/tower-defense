using System.Collections.Generic;
using UnityEngine;

// Consultas de percepcion sobre el registro Agente.Todos. Sin fisica, sin colliders.
// Las versiones sin lambdas NO generan basura: usalas en codigo que corre seguido.
public static class Sensor
{
    // ---------------------------------------------------------------- sin basura
    public static Agente HostilMasCercano(Agente yo, float radio)
    {
        return HostilMasCercanoDeBando(yo.Pos, radio, yo.bando);
    }

    public static Agente HostilMasCercanoDeBando(Vector3 desde, float radio, Bando bandoPropio)
    {
        Agente mejor = null;
        float mejorD = radio * radio;
        List<Agente> todos = Agente.Todos;
        for (int i = 0; i < todos.Count; i++)
        {
            Agente a = todos[i];
            if (!a.Vivo || !Agente.Hostiles(bandoPropio, a.bando)) continue;
            Vector3 d3 = a.Pos - desde;
            d3.y = 0f;
            float d = d3.sqrMagnitude;
            if (a.rol == Rol.Granero) { float b = a.DistanciaBorde(desde); d = b * b; }   // el granero es enorme: se mide al borde
            if (d < mejorD) { mejorD = d; mejor = a; }
        }
        return mejor;
    }

    // Llena 'salida' (se limpia antes) con los hostiles al bando dentro del radio.
    public static void HostilesEnRadio(Vector3 centro, float radio, Bando bandoPropio, List<Agente> salida)
    {
        salida.Clear();
        float r2 = radio * radio;
        List<Agente> todos = Agente.Todos;
        for (int i = 0; i < todos.Count; i++)
        {
            Agente a = todos[i];
            if (!a.Vivo || !Agente.Hostiles(bandoPropio, a.bando)) continue;
            Vector3 d3 = a.Pos - centro; d3.y = 0f;
            float dd = d3.sqrMagnitude;
            if (a.rol == Rol.Granero) { float b = a.DistanciaBorde(centro); dd = b * b; }
            if (dd <= r2) salida.Add(a);
        }
    }

    public static int Contar(Bando bando)
    {
        int n = 0;
        List<Agente> todos = Agente.Todos;
        for (int i = 0; i < todos.Count; i++) if (todos[i].Vivo && todos[i].bando == bando) n++;
        return n;
    }

    public static float DistanciaPlana(Vector3 a, Vector3 b)
    {
        a.x -= b.x; a.z -= b.z;
        return Mathf.Sqrt(a.x * a.x + a.z * a.z);
    }

    // ---------------------------------------------------------------- con filtro (crean basura: usar poco)
    public static Agente MasCercano(Vector3 desde, float radio, System.Func<Agente, bool> filtro)
    {
        Agente mejor = null;
        float mejorD = radio * radio;
        List<Agente> todos = Agente.Todos;
        for (int i = 0; i < todos.Count; i++)
        {
            Agente a = todos[i];
            if (a == null || !a.Vivo) continue;
            if (filtro != null && !filtro(a)) continue;
            float d = (a.Pos - desde).sqrMagnitude;
            if (d < mejorD) { mejorD = d; mejor = a; }
        }
        return mejor;
    }

    public static Agente MasCercanoDeRol(Vector3 desde, float radio, Rol rol)
    {
        return MasCercano(desde, radio, a => a.rol == rol);
    }

    public static Agente MasCercanoDeBando(Vector3 desde, float radio, Bando bando)
    {
        return MasCercano(desde, radio, a => a.bando == bando);
    }

    public static List<Agente> EnRadio(Vector3 centro, float radio, System.Func<Agente, bool> filtro)
    {
        List<Agente> res = new List<Agente>();
        float r2 = radio * radio;
        List<Agente> todos = Agente.Todos;
        for (int i = 0; i < todos.Count; i++)
        {
            Agente a = todos[i];
            if (a == null || !a.Vivo) continue;
            if (filtro != null && !filtro(a)) continue;
            if ((a.Pos - centro).sqrMagnitude <= r2) res.Add(a);
        }
        return res;
    }
}
