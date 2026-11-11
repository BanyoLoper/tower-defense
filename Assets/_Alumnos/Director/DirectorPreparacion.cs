// Assets/_Alumnos/Director/DirectorPreparacion.cs
using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorPreparacion : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Director: entre a Preparacion");   // prueba de vida: debe salir en la consola al hacer Play
        }

        public override string Actualizar(float dt) { return null; }   // null = quedarse en este estado

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}