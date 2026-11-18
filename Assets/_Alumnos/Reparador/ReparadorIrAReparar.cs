using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorIrAReparar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Reparador: entre a IrAReparar");

            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente != null)
            {
                agente.Animar("Caminar");
            }
        }

        public override string Actualizar(float dt)
        {
            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente == null) return null;

          
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}