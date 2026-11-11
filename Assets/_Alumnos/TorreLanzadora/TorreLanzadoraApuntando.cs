using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraApuntando : Estado
    {
        private float anticipacion = 0.25f;
        Agente agent;
        Agente objetivo;
        public override void Entrar(string anterior)
        {
            agent = Ctx.Obtener<Agente>("agente");
            objetivo = Ctx.Obtener<Agente>("objetivo");
            agent.Animar(Anim.IdleCombate);
        }


        public override string Actualizar(float dt)
        {
            if (objetivo == null || !objetivo.Vivo || Sensor.DistanciaPlana(agent.Pos, objetivo.Pos) > 14f)
            {
                return "TorreLanzadoraVigilando";
            }

            Vector3 puntoFuturo = objetivo.Pos + objetivo.cuerpo.Velocidad * anticipacion;

            agent.cuerpo.MirarA(puntoFuturo, dt);
            return null;
        }
    }
}
