using UnityEngine;

public static partial class Steering
{
    public static Vector3 Evade(Vector3 pos, Vector3 posPer, Vector3 velPer, float vMax)
    {
        Vector3 distancia = posPer - pos;
        float tiempoPrediccion = (vMax > 0f) ? (distancia.magnitude / vMax) : 0f;
        Vector3 puntoFuturo = posPer + velPer * tiempoPrediccion;

        Vector3 deseado = pos - puntoFuturo;
        if (deseado.sqrMagnitude > 0.0001f)
        {
            return deseado.normalized * vMax;
        }
        return Vector3.zero;
    }
}