using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraDestruida : Estado
    {
        public override void Entrar(string anterior)
        {
            Agente agent = Ctx.Obtener<Agente>("agente");

            Efectos.Reproducir(Fx.Explosion, agent.Pos);
            Reserva.Devolver(agent.gameObject);
        }
    }
}
