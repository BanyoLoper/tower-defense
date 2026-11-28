using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialObservar : Estado
    {
        bool terminada;

        public override void Entrar(string anterior)
        {
            terminada = false;
            TextoCentral.I.Mostrar("TUTORIAL", "Las torres atacan a los enemigos. Espera a superar la primera oleada.", Color.white, 4f);
        }

        public override string Actualizar(float dt)
        {
            if (terminada)
            {
                return null;
            }

            if (Juego.I.Nivel >= 1)
            {
                terminada = true;
            }

            return null;
        }
    }
}