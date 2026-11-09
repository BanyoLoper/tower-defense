using System.Collections.Generic;
using UnityEngine;

public class Maquina
{
    string trigger;
    string destino;
    public Contexto Ctx;
    Dictionary<string, Estado> estados;
    public Estado Actual;

    public Maquina(List<Estado> lista, Contexto ctx)
    {
        Ctx = ctx;
        estados = new Dictionary<string, Estado>();
        foreach (Estado e in lista)
        {
            e.Ctx = ctx;
            estados[e.Nombre] = e;
        }
    }

    public void Iniciar(string nombre)
    {
        CambiarA(nombre);
    }

    public void CambiarA(string nombre)
    {
        if (!estados.ContainsKey(nombre))
        {
            Debug.LogError("No existe el estado '" + nombre + "'");
            return;
        }

        string anterior = null;
        if (Actual != null)
        {
            anterior = Actual.Nombre;
            Actual.Salir(nombre);
        }

        Actual = estados[nombre];
        Actual.Entrar(anterior);
    }

    public void Disparar(string trigger, int valor = 0)
    {
        if (Actual == null) { return; }

        this.trigger = trigger;
        destino = Actual.AlRecibir(trigger, valor);

        if (destino != null && destino != Actual.Nombre)
        {
            CambiarA(destino);
        }
    }

    public void Actualizar(float dt)
    {
        if (Actual == null) { return; }

        destino = Actual.Actualizar(dt);

        if (destino != null && destino != Actual.Nombre)
        {
            CambiarA(destino);
        }
    }
}
