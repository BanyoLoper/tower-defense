using System;
using UnityEngine;

// Canal de avisos entre maquinas: el Guardia grita, los aldeanos/mineros/reparadores escuchan.
//     Alertas.Emitir(transform.position, 15f, agente);              // quien avisa
//     Alertas.AlAlerta += MiMetodo;   (y en OnDisable: -= MiMetodo)   // quien escucha
// IMPORTANTE (pooling): suscribete en OnEnable y desuscribete en OnDisable.
public static class Alertas
{
    public static event Action<Vector3, float, Agente> AlAlerta;   // posicion, radio, quien aviso
    public static void Emitir(Vector3 pos, float radio, Agente origen = null)
    {
        if (AlAlerta != null) AlAlerta(pos, radio, origen);
    }
}
