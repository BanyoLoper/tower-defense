using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialLlamarOleada : Estado
    {
        public override void Entrar(string anterior)
        {
            TextoCentral.I.Mostrar("TUTORIAL", "Pulsa ESPACIO para llamar la oleada.", Color.white, 4f);
        }

        public override string Actualizar(float dt)
        {
            if (Juego.I.Fase == FaseJuego.Oleada)
            {
                return "TutorialObservar";
            }

            return null;
        }
    }
}