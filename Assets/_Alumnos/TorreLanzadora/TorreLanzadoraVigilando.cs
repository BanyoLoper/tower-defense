using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraVigilando : Estado
    {
        private float tiempo;
        private Agente agent;

        public override void Entrar(string anterior)
        {
            tiempo = 0f;
            agent = Ctx.Obtener<Agente>("agente");
            agent.Animar(Anim.IdleCombate);
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (tiempo < 0.2f)
            {
                return null;
            }

            tiempo = 0f;
            Agente objetivo = Sensor.HostilMasCercano(agent, 14f);

            if (objetivo == null)
            {
                return null;
            }
            Ctx.Fijar("objetivo", objetivo);
            return "TorreLanzadoraApuntando";
        }

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}
