using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialConstruir : Estado
    {
        SlotTorre[] plataformas;
        bool[] ocupadas;

        public override void Entrar(string anterior)
        {
            plataformas = Object.FindObjectsByType<SlotTorre>(FindObjectsSortMode.None);
            ocupadas = new bool[plataformas.Length];

            for (int i = 0; i < plataformas.Length; i++)
            {
                ocupadas[i] = plataformas[i].Ocupado;
            }

            TextoCentral.I.Mostrar("TUTORIAL", "Pulsa 1 o 2 para construir una torre.", Color.white, 4f);
        }

        public override string Actualizar(float dt)
        {
            for (int i = 0; i < plataformas.Length; i++)
            {
                bool ocupada = plataformas[i].Ocupado;

                if (ocupadas[i] == false && ocupada)
                {
                    return "TutorialGemas";
                }

                ocupadas[i] = ocupada;
            }

            return null;
        }
    }
}