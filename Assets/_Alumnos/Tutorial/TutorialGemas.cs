using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialGemas : Estado
    {
        float tiempo;

        public override void Entrar(string anterior)
        {
            tiempo = 0f;
            TextoCentral.I.Mostrar("GEMAS", "Usa gemas para construir torres. Selecciona una plataforma bloqueada y pulsa U para desbloquearla pagando su costo.", Color.white, 6f);
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (tiempo >= 6f)
            {
                return "TutorialLlamarOleada";
            }

            return null;
        }
    }
}