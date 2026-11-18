using UnityEngine;

namespace Alumnos.Jefe
{
    public class JefeAparicion : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Jefe: entre a Aparicion");
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