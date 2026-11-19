using UnityEngine;

public static partial class Steering
{
    public static Vector3 Pursuit(Vector3 pos, Vector3 posObj, Vector3 velObj, float vMax, out Vector3 puntoFuturo)
    {
        pos.y = 0f;
        posObj.y = 0f;
        velObj.y = 0f;
        puntoFuturo = posObj;

        if (vMax <= 0f)
        {
            return Vector3.zero;
        }

        float distancia = Vector3.Distance(pos, posObj);
        float tiempo = distancia / (vMax + velObj.magnitude);
        puntoFuturo = posObj + velObj * tiempo;
        Vector3 direccion = puntoFuturo - pos;
        return direccion.normalized * vMax;
    }
}