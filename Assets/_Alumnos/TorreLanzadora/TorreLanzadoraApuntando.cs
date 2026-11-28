using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraApuntando : Estado
    {
        private float velocidadLanza;
        Agente agent;
        Agente objetivo;

        public override void Entrar(string anterior)
        {
            agent = Ctx.Obtener<Agente>("agente");
            objetivo = Ctx.Obtener<Agente>("objetivo");
            velocidadLanza = Juego.I.proyectilLanza.GetComponent<ProyectilBase>().velocidad;
            agent.Animar(Anim.IdleCombate);
        }

        public override string Actualizar(float dt)
        {
            if (objetivo == null || !objetivo.Vivo || Sensor.DistanciaPlana(agent.Pos, objetivo.Pos) > 14f) return "TorreLanzadoraVigilando";

            Vector3 puntoFuturo;
            Steering.Pursuit(agent.Pos, objetivo.Pos, objetivo.cuerpo.Velocidad, velocidadLanza, out puntoFuturo);

            Vector3 direccion = puntoFuturo - agent.Pos;
            direccion.y = 0f;
            agent.cuerpo.Mirar(direccion, dt);

            if (Vector3.Angle(agent.transform.forward, direccion) < 10f) return "TorreLanzadoraDisparando";

            return null;
        }
    }
}
