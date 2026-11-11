
using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroIrAMina : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Minero: entre a IrAMina");
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

