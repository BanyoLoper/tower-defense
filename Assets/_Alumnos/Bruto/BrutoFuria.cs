using UnityEngine;
namespace Alumnos.Bruto
{
    public class BrutoFuria : Estado
    {
        const float DURACION = 1.2f;
        Agente ag;
        float t;
        public override void Entrar(string anterior)
        {
            t = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            Ctx.Fijar("furiaUsada", true);
            Ctx.Fijar("multVel", 1.5f);
            if (ag != null && ag.cuerpo != null) ag.cuerpo.Detener();
            if (ag != null) ag.Animar(Anim.Enojado);
        }
        public override string Actualizar(float dt)
        {
            t += dt;
            if (t >= DURACION) return "BrutoPerseguir";
            return null;
        }
        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}