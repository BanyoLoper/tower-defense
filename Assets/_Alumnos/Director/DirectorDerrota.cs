using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorDerrota : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Director: DERROTA");
            Juego.I.Terminar(false);
        }

        public override string Actualizar(float dt)
        {
            return null;
        }
    }
}