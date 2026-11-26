using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorFestejo : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Reparador: entre a Festejo");

            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente != null)
            {
                if (agente.cuerpo != null) agente.cuerpo.Detener();
                agente.Animar("Celebrar1");
            }
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