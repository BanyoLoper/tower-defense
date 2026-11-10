using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoAvanzar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Bruto: entre a Avanzar");   // prueba de vida: debe salir en la consola al hacer Play
        }

        public override string Actualizar(float dt) { return null; }   // null = quedarse en este estado

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}