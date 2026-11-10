using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorEsperar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Reparador: entre a Esperar");   
        }

        public override string Actualizar(float dt) { return null; }   

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}