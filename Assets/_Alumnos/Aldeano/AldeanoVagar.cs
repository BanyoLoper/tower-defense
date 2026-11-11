// Assets/_Alumnos/Aldeano/AldeanoVagar.cs
using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoVagar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Aldeano: entre a Vagar");   // prueba de vida: debe salir en la consola al hacer Play
        }

        public override string Actualizar(float dt) { return null; }   // null = quedarse en este estado

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}