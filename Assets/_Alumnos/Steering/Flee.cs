using UnityEngine;

public class Flee
{
    public Vector3 SFlee(Vector3 pos, Vector3 amenaza, float vMax, float radioPanico)
    { 
        Vector3 deseada = pos - amenaza;
        float dist = Vector3.Distance(pos, amenaza);
        if(dist > radioPanico)
        {
            return Vector3.zero;
        }
        Vector3 regreso = deseada.normalized * vMax;
        return regreso;
    }
}
