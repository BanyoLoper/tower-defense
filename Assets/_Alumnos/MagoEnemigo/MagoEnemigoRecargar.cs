using UnityEngine;
namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoRecargar : Estado
    {
        Agente ag;
        Agente objetivo;
        float t;

        public override void Entrar(string anterior)
        {
            t = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            if(ag != null)
            {
                ag.cuerpo.Detener();
                ag.Animar(Anim.HechizoIdle);
                objetivo = Sensor.HostilMasCercano(ag, 15f);
            }
            Debug.Log("MagoEnemigo: entra a Recargar");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null) return null;
            if(objetivo == null)
            {
                return "MagoEnemigoAvanzar";
            }
            Vector3 hacia = objetivo.transform.position - ag.transform.position;
            hacia.y = 0f;
            ag.cuerpo.Mirar(hacia, dt);
            t += dt;
            if(t >= 2.4f)
            {
                objetivo = Sensor.HostilMasCercano(ag, 15f);
                if(objetivo == null)
                {
                    return "MagoEnemigoAvanzar";
                }
                float dist = Vector3.Distance(ag.transform.position, objetivo.transform.position);
                if(dist >9.2f)
                {
                    return "MagoEnemigoPosicionarse";
                }
                return "MagoEnemigoLanzar";
            }
            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
