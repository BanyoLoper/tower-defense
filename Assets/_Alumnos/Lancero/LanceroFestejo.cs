using UnityEngine;

namespace Alumnos.Lancero
{
    public class LanceroFestejo : Estado
    {
        Agente agente;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            agente.cuerpo.Detener();
            agente.Animar(Anim.Celebrar1);
        }

        public override string Actualizar(float dt)
        {
            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}