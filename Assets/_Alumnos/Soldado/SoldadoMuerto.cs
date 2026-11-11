using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoMuerto : Estado
    {
        float tiempo;
        bool devuelto;
        Agente agente;

        public override void Entrar(string anterior)
        {
            tiempo = 0f;
            devuelto = false;
            agente = Ctx.Obtener<Agente>("agente");
            agente.cuerpo.Detener();
            agente.Animar(Anim.Muerte1);
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (tiempo >= 3f && !devuelto)
            {
                devuelto = true;
                Reserva.Devolver(agente.gameObject);
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}

