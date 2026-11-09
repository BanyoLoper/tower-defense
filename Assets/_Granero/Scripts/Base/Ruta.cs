using UnityEngine;

// Datos del mapa que comparten todos los agentes. Los llena Juego al iniciar.
public static class Ruta
{
    public static Vector3[] Puntos;        // camino del bosque al granero, en orden
    public static Vector3 Granero;         // centro del granero
    public static Vector3 Inicio { get { return Puntos != null && Puntos.Length > 0 ? Puntos[0] : Vector3.zero; } }
}
