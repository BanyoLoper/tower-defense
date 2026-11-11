using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorAviso : Estado
    {
        float tiempo;

        public override void Entrar(string anterior)
        {
            tiempo = 0f;

            Debug.Log("Director: mostrando aviso de oleada");
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (tiempo >= 3f)
            {
                return "DirectorOleada";
            }

            return null;
        }
    }
}
