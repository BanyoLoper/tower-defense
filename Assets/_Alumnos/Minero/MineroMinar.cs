
using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroMinar : Estado
    {
        float tiempo;
        bool terminado;
        Agente ag;

        public override void Entrar(string anterior)
        {
            tiempo = 0f;
            terminado = false;

            ag = Ctx.Obtener<Agente>("agente");

            if (ag == null)
                return;

            ag.cuerpo.Detener();
            ag.Animar(Anim.MinarSuelo, 0.2f, true);

            Debug.Log("Minero: empezando a minar");
        }

        public override string Actualizar(float dt)
        {
            if (terminado)
                return null;

            tiempo += dt;

            if (tiempo >= 5f)
            {
                terminado = true;

                Ctx.Fijar("carga", 6);

                if (ag != null)
                    ag.Animar(Anim.Idle);

                Debug.Log("Minero: termino de minar");
            }

            return null;
        }
    }
}

