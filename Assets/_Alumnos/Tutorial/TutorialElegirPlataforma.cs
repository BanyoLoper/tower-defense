using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialElegirPlataforma : Estado
    {
        ColocadorTorres colocador;

        public override void Entrar(string anterior)
        {
            colocador = Object.FindFirstObjectByType<ColocadorTorres>();
            TextoCentral.I.Mostrar("TUTORIAL", "Haz clic en una plataforma.", Color.white, 4f);
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