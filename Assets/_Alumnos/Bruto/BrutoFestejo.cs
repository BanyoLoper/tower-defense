using UnityEngine;
namespace Alumnos.Bruto
{
    public class BrutoFestejo : Estado
    {
        public override void Entrar(string anterior)
        {
            Agente ag = Ctx.Obtener<Agente>("agente");
            if (ag != null && ag.cuerpo != null) ag.cuerpo.Detener();
            if (ag != null) ag.Animar(Anim.Celebrar1);
        }
        public override string Actualizar(float dt) { return null; }
        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}