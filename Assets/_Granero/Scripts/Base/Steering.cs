using UnityEngine;

// Biblioteca de steering behaviours del proyecto. Cada alumno con un behaviour asignado crea SU archivo:
//     SteeringSeek.cs   ->   public static partial class Steering { public static Vector3 Seek(...) { ... } }
// Reglas del contrato (ver docs "Steering behaviors"):
//   - Devuelven la VELOCIDAD DESEADA (plano XZ, magnitud <= vMax). Se usa con Cuerpo.Mover(v, dt).
//   - Sin basura: sin new de clases/arreglos, sin LINQ. Vector3 es struct, es gratis.
//   - Sin estado oculto: si necesitan memoria (Wander) la recibe el llamador por parametro (ref).
public static partial class Steering
{
    // Utilidad comun a todos: limita la magnitud en el plano XZ.
    public static Vector3 Limitar(Vector3 v, float max)
    {
        v.y = 0f;
        float m = v.magnitude;
        return m > max && m > 0.0001f ? v * (max / m) : v;
    }
}
