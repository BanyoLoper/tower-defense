
using UnityEngine;

namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoPosicionarse : Estado
    {
        Agente ag;
        Agente objetivo;
        float t;


        public override void Entrar(string anterior)
        {
            t = 0.2f;
            Agente ag = Ctx.Obtener<Agente>("agente");
            if (ag != null)
            {
                ag.Animar(Anim.Caminar);
            }
            Debug.Log("MagoEnemigo: entra a Posicionarse");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null) return null;

            t += dt;
            if (t >= 0.2f)
            {
                t = 0f;
                objetivo = Sensor.HostilMasCercano(ag, 15f);
                if (objetivo == null)
                { return "MagoEnemigoAvanzar"; }
            }

            if (objetivo == null) return "MagoEnemigoAvanzar";
            Vector3 hacia = objetivo.transform.position - ag.transform.position;
            hacia.y = 0f;
            float dist = hacia.magnitude;
            if (dist > 9)
            {
                ag.cuerpo.Mover(hacia.normalized * ag.cuerpo.velocidadMax, dt);
            }
            else
            {
                ag.cuerpo.Detener();
                ag.cuerpo.MirarA(hacia, dt);

            }
                return null;
            }
        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }

    }

}


