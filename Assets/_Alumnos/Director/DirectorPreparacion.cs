
using UnityEngine;
using UnityEngine.InputSystem;

namespace Alumnos.Director
{
    public class DirectorPreparacion : Estado
    {
        float tiempo;
        int nivel;

        public override void Entrar(string anterior)
        {
            tiempo = 0f;

            nivel = Juego.I.Nivel;

            Juego.I.Preparar(nivel);

            Debug.Log("Director: entre a Preparacion");
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            Keyboard teclado = Keyboard.current;

            if (teclado != null && teclado.spaceKey.wasPressedThisFrame)
            {
                return "DirectorAviso";
            }

            if (tiempo >= 20f)
            {
                return "DirectorAviso";
            }

            return null;
        }
    }
}
