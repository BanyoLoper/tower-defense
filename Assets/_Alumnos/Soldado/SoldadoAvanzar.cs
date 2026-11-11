
using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoAvanzar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Soldado: entre a Avanzar");
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

