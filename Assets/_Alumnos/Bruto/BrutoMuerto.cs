using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoMuerto : Estado
    {
        public override void Entrar(string anterior)
        {
            var ag = Ctx.Obtener<Agente>("agente");
            ag.Animar(Anim.Muerte1);

            // Devuelve el objeto a la reserva al morir
            Reserva.Devolver(ag.gameObject);
        }

        public override string Actualizar(float dt) { return null; }
        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}