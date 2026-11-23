using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroHuir : Estado
    {
        Agente ag;
        Agente enemigo;
        float tiempoSensor;
        const float radioPeligro = 12f;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            enemigo = null;
            tiempoSensor = 0f;

            if (ag == null) return;

            ag.Animar(Anim.Correr);
            Debug.Log("Minero: huyendo del enemigo");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null) return null;

            tiempoSensor -= dt;

            if (tiempoSensor <= 0f)
            {
                tiempoSensor = 0.2f;
                enemigo = Sensor.HostilMasCercano(
                    ag, radioPeligro);
            }

            if (enemigo == null || !enemigo.Vivo)
            {
                ag.cuerpo.Detener();
                Debug.Log("Minero: peligro terminado");
                return "MineroEsconderse";
            }

            Vector3 velocidad = Steering.Flee(ag.Pos,enemigo.Pos,ag.cuerpo.velocidadMax,radioPeligro);
            ag.cuerpo.Mover(velocidad, dt);
            return null;
        }
    }
}

