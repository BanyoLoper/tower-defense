
using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroMuerto : Estado
    {
        Agente ag;
        float tiempo;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            tiempo = 0f;

            Ctx.Fijar("carga", 0);

            if (ag == null)
                return;

            ag.cuerpo.Detener();
            ag.Animar(Anim.Muerte1, 0.1f, true);

            Debug.Log("Minero: ha muerto y perdio su carga");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null)
                return null;

            tiempo += dt;

            if (tiempo >= 3f)
            {
                Reserva.Devolver(ag.gameObject);
            }

            return null;
        }
    }
}

