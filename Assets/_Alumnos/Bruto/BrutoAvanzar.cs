using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoAvanzar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Bruto: entre a Avanzar");
        }

        public override string Actualizar(float dt) { return null; }

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}