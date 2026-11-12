using UnityEngine;

public static partial class Steering
{
    public static Vector3 Arrive(Vector3 pos, Vector3 destino, float vMax, float radioFrenado)
    {
        Vector3 distanciaVector = destino - pos;
        float distancia = distanciaVector.magnitude;

        if (distancia == 0) return Vector3.zero;

        float velocidadDeseada = vMax;
        if (distancia < radioFrenado)
        {
            velocidadDeseada = vMax * (distancia / radioFrenado);
        }

        return (distanciaVector / distancia) * velocidadDeseada;
    }
}
