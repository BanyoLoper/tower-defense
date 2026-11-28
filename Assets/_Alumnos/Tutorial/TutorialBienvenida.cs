using UnityEngine;
using UnityEngine.InputSystem;

namespace Alumnos.Tutorial
{
    public class TutorialBienvenida : Estado
    {
        float tiempo;

        public override void Entrar(string anterior)
        {
            tiempo = 0f;
            Debug.Log("Tutorial: entre a Bienvenida");
            TextoCentral.I.Mostrar("TUTORIAL", "Bienvenido al tutorial.\nPulsa T para saltar.", Color.white, 4f);
        }

        public override string Actualizar(float dt)
        {
            if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
            {
                return "TutorialFin";
            }

            tiempo += dt;

            if (tiempo >= 4f)
            {
                return "TutorialElegirPlataforma";
            }

            return null;
        }
    }
}