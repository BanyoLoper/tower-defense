using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialElegirPlataforma : Estado
    {
        ColocadorTorres colocador;

        public override void Entrar(string anterior)
        {
            colocador = Object.FindFirstObjectByType<ColocadorTorres>();
            Juego.I.Mensaje("Haz clic en una plataforma.");
        }

        public override string Actualizar(float dt)
        {
            if (colocador != null && colocador.Elegido != null)
            {
                return "TutorialConstruir";
            }

            return null;
        }
    }
}