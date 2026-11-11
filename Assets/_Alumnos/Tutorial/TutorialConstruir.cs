using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialConstruir : Estado
    {
        ColocadorTorres colocador;
        bool construida;

        public override void Entrar(string anterior)
        {
            colocador = Object.FindFirstObjectByType<ColocadorTorres>();
            construida = false;
            Juego.I.Mensaje("Pulsa 1 o 2 para construir una torre.");
        }

        public override string Actualizar(float dt)
        {
            if (construida)
            {
                return null;
            }

            if (colocador != null && colocador.Elegido != null)
            {
                construida = colocador.Elegido.Ocupado;
            }

            return null;
        }
    }
}