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
            TextoCentral.I.Mostrar("TUTORIAL", "Pulsa 1 o 2 para construir una torre.", Color.white, 4f);
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