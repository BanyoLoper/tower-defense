using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialObservar : Estado
    {
        public override void Entrar(string anterior)
        {
            TextoCentral.I.Mostrar("TUTORIAL", "Las torres atacan a los enemigos.\nEspera a superar la primera oleada.", Color.white, 4f);
        }

        public override string Actualizar(float dt)
        {
            if (Juego.I.Nivel >= 1)
            {
                return "TutorialFin";
            }

            return null;
        }
    }
}