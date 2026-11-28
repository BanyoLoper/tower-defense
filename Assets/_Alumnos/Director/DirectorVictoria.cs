using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorVictoria : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Director: Victoria");
            Juego.I.Terminar(true);
        }

        public override string Actualizar(float dt)
        {
            return null;
        }
    }
}