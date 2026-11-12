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
            if (construida == false && colocador != null && colocador.Elegido != null && colocador.Elegido.Ocupado)
            {
                construida = true;
                Juego.I.Mensaje("Torre construida.");
            }
            return null;
        }
    }
}