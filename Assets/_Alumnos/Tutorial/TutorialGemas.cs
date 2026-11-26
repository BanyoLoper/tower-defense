using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialGemas : Estado
    {
        public override void Entrar(string anterior)
        {
            TextoCentral.I.Mostrar("GEMAS", "Usa gemas para construir torres. Selecciona una plataforma bloqueada y pulsa U para desbloquearla pagando su costo.", Color.white, 6f);
        }
    }
}