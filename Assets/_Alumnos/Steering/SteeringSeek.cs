using UnityEngine;

public static partial class Steering
{
    public static Vector3 Seek(Vector3 pos, Vector3 destino, float vMax)
    {
        Vector3 deseada = new Vector3(destino.x - pos.x, 0f, destino.z - pos.z);
        return deseada.normalized * vMax;
    }
}