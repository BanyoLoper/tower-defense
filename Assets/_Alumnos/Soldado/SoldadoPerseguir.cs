using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoPerseguir : Estado
    {
        Agente agente;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            agente.Animar(Anim.Correr);
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

