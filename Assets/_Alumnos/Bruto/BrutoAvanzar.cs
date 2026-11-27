using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoAvanzar : Estado
    {
        Agente ag;
        Cuerpo cuerpo;
        float tSensor;
        const float RANGO_DETECCION = 6.0f;

        public override void Entrar(string anterior)
        {
            tSensor = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            cuerpo = Ctx.Obtener<Cuerpo>("cuerpo");

            if (ag != null)
            {
                ag.Animar(Anim.Caminar);
            }
        }

        public override string Actualizar(float dt)
        {
            if (ag == null)
            {
                return null;
            }

            tSensor += dt;
            if (tSensor >= 0.2f)
            {
                tSensor = 0f;
                Agente hostil = Sensor.HostilMasCercano(ag, RANGO_DETECCION);
                if (hostil != null)
                {
                    Ctx.Fijar("objetivo", hostil);
                    return "BrutoPerseguir";
                }
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}