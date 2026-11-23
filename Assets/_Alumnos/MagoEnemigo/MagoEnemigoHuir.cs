using UnityEngine;
namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoHuir : Estado
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
                ag.Animar(Anim.Correr);
                objetivo = Sensor.HostilMasCercano(ag, 15f);
            }
            Debug.Log("MagoEnemigo : entra a  Huir");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null) return null;
            t += dt;
            if(t >= 1.5f)
            {
                return "MagoEnemigoPosicionarse";
            }
            if(objetivo != null)
            {
                Vector3 lejos = ag.transform.position - objetivo.transform.position;
                lejos.y = 0f;
                if(lejos.sqrMagnitude > 0.01f)
                {
                    ag.cuerpo.Mover(lejos.normalized * ag.cuerpo.velocidadMax, dt);
                }
            }
            return null;
        }
        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
     }
}
