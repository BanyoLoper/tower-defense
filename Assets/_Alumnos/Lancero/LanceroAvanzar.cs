// Assets/_Alumnos/Lancero/LanceroAvanzar.cs
using UnityEngine;

namespace Alumnos.Lancero
{
    public class LanceroAvanzar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Lancero: entre a Avanzar");   // prueba de vida: debe salir en la consola al hacer Play
        }

        public override string Actualizar(float dt) { return null; }   // null = quedarse en este estado

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}