using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorMuerto : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Reparador: entre a Muerto");

            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente != null)
            {
                agente.Animar("Muerte1");
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