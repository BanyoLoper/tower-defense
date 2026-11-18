using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoPerseguir : Estado
    {
        Agente ag;
        Cuerpo cuerpo;
        Agente objetivo;
        float tSensor;
        const float VELOCIDAD = 1.4f;
        const float RANGO_CARGAR = 2.2f;
        const float RANGO_PERDER = 8.0f;
        public override void Entrar(string anterior)
        {
            tSensor = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            cuerpo = Ctx.Obtener<Cuerpo>("cuerpo");
            objetivo = Ctx.Obtener<Agente>("objetivo");

            if (ag != null)
            {
                ag.Animar(Anim.Caminar);
            }
        }
        public override string Actualizar(float dt)
        {
            if (ag == null || cuerpo == null)
            {
                return null;
            }

            tSensor += dt;
            if (tSensor >= 0.2f)
            {
                tSensor = 0f;
                Agente cercano = Sensor.HostilMasCercano(ag, RANGO_PERDER);
                if (cercano != null)
                {
                    objetivo = cercano;
                    Ctx.Fijar("objetivo", objetivo);
                }
            }

            if (objetivo == null || !objetivo.Vivo)
            {
                Ctx.Fijar<Agente>("objetivo", null);
                return "BrutoAvanzar";
            }

            Vector3 dir = objetivo.Pos - ag.Pos;
            dir.y = 0f;
            float dist = (objetivo.rol == Rol.Granero) ? objetivo.DistanciaBorde(ag.Pos) : dir.magnitude;

            if (dist > RANGO_PERDER)
            {
                Ctx.Fijar<Agente>("objetivo", null);
                return "BrutoAvanzar";
            }

            if (dist <= RANGO_CARGAR)
            {
                return "BrutoCargarGolpe";
            }

            float mag = dir.magnitude;
            if (mag > 0.01f)
            {
                Vector3 vel = (dir / mag) * VELOCIDAD;
                cuerpo.Mover(vel, dt);
                cuerpo.Mirar(dir, dt);
            }
            return null;
        }
        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}