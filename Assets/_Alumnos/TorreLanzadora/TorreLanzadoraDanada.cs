using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraDanada : Estado
    {
        private Agente agent;
        private float tiempoHumo, tiempoDisparo, tiempoBusqueda;

        public override void Entrar(string anterior)
        {
            agent = Ctx.Obtener<Agente>("agente");
            agent.Animar(Anim.Idle);
            tiempoHumo = 1.5f;
            tiempoDisparo = 0f;
            tiempoBusqueda = 0f;
        }
        public override string Actualizar(float dt)
        {
            if (agent.salud.Porcentaje > 0.6f) return "TorreLanzadoraVigilando";

            tiempoHumo += dt;
            if (tiempoHumo >= 1.5f)
            {
                tiempoHumo = 0f;
                Efectos.Reproducir(Fx.Fuego, agent.Pos, 7f, 1.5f);
            }

            tiempoDisparo += dt;
            if (tiempoDisparo < 2.6f) return null;
            tiempoDisparo += dt;
            if (tiempoBusqueda < 0.2f) return null;

            tiempoBusqueda = 0f;
            Agente objetivo = Sensor.HostilMasCercano(agent, 14f);

            if (objetivo == null) return null;
            Ctx.Fijar("objetivo", objetivo);
            return "TorreLanzadoraApuntando";
        }
    }
}
