using UnityEngine;
using UnityEngine.InputSystem;

namespace Alumnos.Tutorial
{
    public class TutorialElegirPlataforma : Estado
    {
        ColocadorTorres colocador;

        public override void Entrar(string anterior)
        {
            colocador = Object.FindFirstObjectByType<ColocadorTorres>();
            TextoCentral.I.Mostrar("TUTORIAL", "Haz clic en una plataforma.\nPulsa T para saltar.", Color.white, 4f);
        }

        public override string Actualizar(float dt)
        {
            if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
            {
                return "TutorialFin";
            }

            if (colocador != null && colocador.Elegido != null)
            {
                return "TutorialConstruir";
            }

            return null;
        }
    }
}