using UnityEngine;

namespace Alumnos.TorreMaga
{
    public static partial class TorreMagaSteering
    {
        static RaycastHit[] hits = new RaycastHit[10];

        public static Vector3 ObstacleAvoidance(
            Vector3 pos,
            Vector3 adelante,
            Vector3 deseada,
            float largo,
            float vMax,
            LayerMask capas)
        {
            if (deseada.sqrMagnitude < 0.001f)
                return Vector3.zero;

            Vector3 direccion = deseada.normalized;

            int cantidad = Physics.RaycastNonAlloc(
                pos,
                direccion,
                hits,
                largo,
                capas
            );

            if (cantidad == 0)
                return direccion * vMax;

            Vector3 evitar = Vector3.zero;

            for (int i = 0; i < cantidad; i++)
            {
                evitar += hits[i].normal;
            }

            if (evitar.sqrMagnitude < 0.001f)
                evitar = Vector3.Cross(adelante, Vector3.up);

            evitar.Normalize();

            Vector3 resultado = direccion + evitar;

            if (resultado.sqrMagnitude < 0.001f)
                resultado = evitar;

            return resultado.normalized * vMax;
        }
    }
}
