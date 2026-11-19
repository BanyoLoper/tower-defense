using UnityEngine;

namespace Alumnos.Jefe
{
    public class JefeAparicion : Estado
    {
        float tiempo;

        public override void Entrar(string anterior)
        {
            Debug.Log("Jefe: entre a Aparicion");
            tiempo = 0f;
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (tiempo >= 2f)
            {
                return "JefeAvanzar";
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}