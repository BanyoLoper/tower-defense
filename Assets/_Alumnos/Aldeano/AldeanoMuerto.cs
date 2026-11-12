using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoMuerto : Estado
    {
        Agente ag;
        float restante;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            Debug.Log("[" + ag.name + "] Aldeano: entre a Muerto desde " + anterior, ag);
            ag.cuerpo.Detener();
            ag.Animar(Anim.Muerte1, 0.1f, true);
            restante = Mathf.Max(3f, ag.Duracion(Anim.Muerte1));
        }

        public override string Actualizar(float dt)
        {
            restante -= dt;
            if (restante <= 0f) Reserva.Devolver(ag.gameObject);
            return null;
        }
    }
}
