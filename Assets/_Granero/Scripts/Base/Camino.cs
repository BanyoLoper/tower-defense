using UnityEngine;
using UnityEngine.AI;

// Ruta sobre el NavMesh sin NavMeshAgent. Calcula las esquinas y entrega el siguiente punto.
// Base para el steering "Path Following".
public class Camino
{
    public Vector3[] Esquinas { get; private set; }
    public int Indice { get; private set; }
    public bool Valido { get { return Esquinas != null && Esquinas.Length > 0; } }
    public bool Terminado { get { return !Valido || Indice >= Esquinas.Length; } }

    public bool Calcular(Vector3 desde, Vector3 hasta)
    {
        NavMeshPath p = new NavMeshPath();
        if (NavMesh.CalculatePath(desde, hasta, NavMesh.AllAreas, p) && p.corners.Length > 0)
        {
            Esquinas = p.corners;
            Indice = 1 < Esquinas.Length ? 1 : 0;
            return true;
        }
        Esquinas = null;
        Indice = 0;
        return false;
    }

    // Punto actual al que hay que ir. Avanza solo al llegar a menos de 'tolerancia'.
    public Vector3 Siguiente(Vector3 posicion, float tolerancia = 0.6f)
    {
        if (!Valido) return posicion;
        while (Indice < Esquinas.Length - 1 && Sensor.DistanciaPlana(posicion, Esquinas[Indice]) < tolerancia) Indice++;
        return Esquinas[Mathf.Min(Indice, Esquinas.Length - 1)];
    }

    public Vector3 Final { get { return Valido ? Esquinas[Esquinas.Length - 1] : Vector3.zero; } }

    public bool Llego(Vector3 posicion, float tolerancia = 0.8f)
    {
        return !Valido || Sensor.DistanciaPlana(posicion, Final) < tolerancia;
    }

    public void DibujarGizmos()
    {
        if (!Valido) return;
        for (int i = 0; i < Esquinas.Length - 1; i++) Debug.DrawLine(Esquinas[i] + Vector3.up * 0.2f, Esquinas[i + 1] + Vector3.up * 0.2f, Color.cyan);
    }
}
