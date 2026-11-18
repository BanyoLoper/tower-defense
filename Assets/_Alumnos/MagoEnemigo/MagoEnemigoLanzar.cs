using UnityEngine;
namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoLanzar : Estado
    {
        Agente ag;
        Agente objetivo;
        ProyectilBase p;
        float t;
     public override void Entrar(string anterior)
        {
            t = 0f;
            p = null;
            ag = Ctx.Obtener<Agente>("agente");
            if (ag != null)
            {
                ag.cuerpo.Detener();
                ag.Animar(Anim.HechizoLanzar);
                objetivo = Sensor.HostilMasCercano(ag, 15f);
                p = Reserva.Obtener(Juego.I.proyectilBola, ag.PuntoPecho, ag.transform.rotation).GetComponent<ProyectilBase>();
            }
            Debug.Log("MagoEnemigo: entra a Lanzar");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null) return null;
            if (objetivo == null)
            {
                return "MagoEnemigoAvanzar";
            }
            Vector3 hacia = objetivo.transform.position - ag.transform.position;
            hacia.y = 0f;
            ag.cuerpo.Mirar(hacia, dt);
            t += dt;
            if (t >= 0.4f)
            {
                if (p != null) p.Lanzar(ag, objetivo, ag.bando, 15f);
                return "MagoEnemigoRecargar";
            }
            return null;
        }
        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }

    }
}
