
using UnityEngine;

public static partial class Steering
{
    public static Vector3 PathFollowing(Vector3 pos, Vector3[] puntos, ref int indice, float tolerancia, float vMax)
    {
        if (puntos == null || puntos.Length == 0)
        {
            return Vector3.zero;
        }

        indice = Mathf.Clamp(indice, 0, puntos.Length - 1);

        Vector3 direccion = puntos[indice] - pos;
        direccion.y = 0f;

        if (direccion.magnitude <= tolerancia)
        {
            if (indice < puntos.Length - 1)
            {
                indice++;
                direccion = puntos[indice] - pos;
                direccion.y = 0f;
            }
            else
            {
                return Vector3.zero;
            }
        }

        return direccion.normalized * vMax;
    }
}
