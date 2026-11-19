using UnityEngine;

public static partial class Steering
{
    // angulo se conserva por agente y se expresa en radianes.
    public static Vector3 Wander(Vector3 pos, Vector3 adelante, ref float angulo,
        float radio, float distancia, float jitter, float vMax)
    {
        float variacion = Mathf.Abs(jitter);
        angulo += Random.Range(-variacion, variacion);
        angulo = Mathf.Repeat(angulo + Mathf.PI, 2f * Mathf.PI) - Mathf.PI;

        adelante.y = 0f;
        if (adelante.sqrMagnitude < 0.0001f) adelante = Vector3.forward;

        Vector3 centro = pos + adelante.normalized * Mathf.Max(0f, distancia);
        Vector3 desplazamiento = new Vector3(Mathf.Cos(angulo), 0f, Mathf.Sin(angulo));
        Vector3 punto = centro + desplazamiento * Mathf.Max(0f, radio);

        Vector3 direccion = punto - pos;
        if (direccion.sqrMagnitude < 0.0001f) direccion = adelante;

        return direccion.normalized * Mathf.Max(0f, vMax);
    }
}
