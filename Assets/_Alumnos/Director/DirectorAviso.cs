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
                if (Juego.I.Nivel == 4)
                {
                    return "DirectorJefe";
                }
                return "DirectorOleada";
            }
            return null;
        }
    }
}