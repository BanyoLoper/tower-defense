using UnityEngine;

public static partial class Steering
{
    public static Vector3 Hide(Vector3 pos, Vector3 amenaza, Vector3[] escondites, float[] radios, float vMax, out Vector3 mejorPunto)
    {
        mejorPunto = pos;
        if (escondites == null || radios == null) return Vector3.zero;

        float mejorDistancia = float.MaxValue;

        for (int i = 0; i < escondites.Length && i < radios.Length; i++)
        {
            Vector3 direccion = escondites[i] - amenaza;
            direccion.y = 0f;
            Vector3 punto = escondites[i] + direccion.normalized * (radios[i] + 1f);
            punto.y = pos.y;
            float distancia = (punto - pos).sqrMagnitude;
            if (distancia < mejorDistancia)
            {
                mejorDistancia = distancia;
                mejorPunto = punto;
            }
        }
        return Arrive(pos, mejorPunto, vMax, 3f);
    }
}
