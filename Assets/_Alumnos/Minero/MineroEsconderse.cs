using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroEsconderse : Estado
    {
        Agente ag;
        float tiempoSeguro;
        float tiempoSensor;
        bool peligro;

        const float radioPeligro = 12f;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            tiempoSeguro = 0f;
            tiempoSensor = 0f;
            peligro = false;

            if (ag == null) return;

            ag.cuerpo.Detener();
            ag.Animar(Anim.Agachado);
            Debug.Log("Minero: escondiendose");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null) return null;

            tiempoSensor -= dt;

            if (tiempoSensor <= 0f)
            {
                tiempoSensor = 0.2f;

                peligro = Sensor.HostilMasCercano(ag, radioPeligro) != null;
            }

            if (peligro)
            {
                tiempoSeguro = 0f;
                return "MineroHuir";
            }

            tiempoSeguro += dt;

            if (tiempoSeguro >= 4f)
            {
                Debug.Log("Minero: vuelve a trabajar");
                return "MineroIrAMina";
            }

            return null;
        }
    }
}

