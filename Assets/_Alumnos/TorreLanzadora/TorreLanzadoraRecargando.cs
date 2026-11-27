using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraRecargando : Estado
    {
        private Agente agent;
        private float tiempo;
        public override void Entrar(string anterior)
        {
            agent = Ctx.Obtener<Agente>("agente");
            tiempo = 0f;
            agent.Animar(Anim.Idle);
        }
        public override string Actualizar(float dt)
        {
            if (agent.salud.Porcentaje < 0.4f) return "TorreLanzadoraDanada";

            tiempo += dt;

            if (tiempo < 1.3f) return null;

            Agente objetivo = Sensor.HostilMasCercano(agent, 14f);

            if (objetivo != null)
            {
                Ctx.Fijar("objetivo", objetivo);
                return "TorreLanzadoraApuntando";
            }
            return "TorreLanzadoraVigilando";
        }
    }
}