using UnityEngine;

namespace Alumnos.Lancero
{
    public class LanceroRetroceder : Estado
    {
        Agente agente;
        float tiempoRetroceso;
        const float DURACION_RETROCESO = 1.0f;
        const float VELOCIDAD_RETROCESO = 4f;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            agente.Animar(Anim.Caminar);
            tiempoRetroceso = 0f;
        }

        public override string Actualizar(float dt)
        {
            tiempoRetroceso += dt;

            Transform hostilTransform = ObtenerHostilTransform();
            if (hostilTransform != null)
            {
                Vector3 desde = agente.transform.position;
                Vector3 hacia = hostilTransform.position;
                Vector3 alejarse = new Vector3(desde.x - hacia.x, 0f, desde.z - hacia.z).normalized * VELOCIDAD_RETROCESO;
                agente.cuerpo.Mover(alejarse, dt);
            }

            if (tiempoRetroceso >= DURACION_RETROCESO)
            {
                return "LanceroAcercarse";
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }

        Transform ObtenerHostilTransform()
        {
            var hostilObj = agente.ObtenerHostilCercano();
            if (hostilObj == null)
            {
                return null;
            }

            Transform t = hostilObj as Transform;
            if (t == null)
            {
                GameObject go = hostilObj as GameObject;
                if (go != null)
                {
                    t = go.transform;
                }
            }

            return t;
        }
    }
}
