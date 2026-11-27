
using UnityEngine;

namespace Alumnos.Jefe
{
    public class JefeAvanzar : Estado
    {
        Agente agente;
        Cuerpo cuerpo;
        int puntoActual;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            cuerpo = agente.cuerpo;
            puntoActual = 0;

            agente.Animar(Anim.Caminar);

            Debug.Log("Jefe: entre a Avanzar");
        }

        public override string Actualizar(float dt)
        {
            if (agente == null || cuerpo == null)
                return null;

            Vector3 destino;

            if (Ruta.Puntos != null &&
                puntoActual < Ruta.Puntos.Length)
            {
                destino = Ruta.Puntos[puntoActual];

                Vector3 distancia = destino - agente.Pos;
                distancia.y = 0f;

                if (distancia.magnitude < 0.5f)
                {
                    puntoActual++;
                }
            }

            if (Ruta.Puntos != null &&
                puntoActual < Ruta.Puntos.Length)
            {
                destino = Ruta.Puntos[puntoActual];
            }
            else
            {
                destino = Ruta.Granero;
            }

            Vector3 direccion = destino - agente.Pos;
            direccion.y = 0f;

            if (direccion.magnitude < 0.5f)
            {
                cuerpo.Detener();
                return null;
            }

            cuerpo.Mover(
                direccion.normalized * cuerpo.velocidadMax,
                dt
            );

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
