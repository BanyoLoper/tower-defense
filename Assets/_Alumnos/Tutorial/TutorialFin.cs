using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialFin : Estado
    {
        public override void Entrar(string anterior)
        {
            TextoCentral.I.Mostrar("TUTORIAL", "Tutorial terminado. Sigue jugando.", Color.white, 4f);

            TutorialController controller = Ctx.Obtener<TutorialController>("ctrl");
            controller.enabled = false;
        }
    }
}