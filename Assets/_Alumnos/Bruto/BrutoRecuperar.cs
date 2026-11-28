using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoRecuperar : Estado
    {
        const float DURACION = 1f;
        const float RANGO_DETECCION = 6f;

        float t;
        Agente ag;

        public override void Entrar(string anterior)
        {
            t = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            ag.cuerpo.Detener();
            ag.Animar(Anim.IdleCombate);
        }

        public override string Actualizar(float dt)
        {
            t += dt;
            if (t < DURACION) return null;

            Agente objetivo = Ctx.Obtener<Agente>("objetivo");
            if (objetivo == null || !objetivo.Vivo)
            {
                objetivo = Sensor.HostilMasCercano(ag, RANGO_DETECCION);
            }

            if (objetivo != null && objetivo.Vivo)
            {
                Ctx.Fijar("objetivo", objetivo);
                return "BrutoPerseguir";
            }

            Ctx.Fijar<Agente>("objetivo", null);
            return "BrutoAvanzar";
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}