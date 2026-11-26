using UnityEngine;
namespace Alumnos.Bruto
{
    public class BrutoAvanzar : Estado
    {
        Agente ag;
        Agente meta;
        float tSensor;
        const float VELOCIDAD = 1.2f;
        const float RANGO_DETECCION = 6.0f;
        public override void Entrar(string anterior)
        {
            tSensor = 0.2f;
            meta = null;
            ag = Ctx.Obtener<Agente>("agente");
            if (ag != null) ag.Animar(Anim.Caminar);
        }
        public override string Actualizar(float dt)
        {
            if (ag == null || ag.cuerpo == null) return null;
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
                meta = Sensor.HostilMasCercano(ag, 200f);
            }
            if (meta != null && meta.Vivo)
            {
                Vector3 dir = meta.Pos - ag.Pos;
                dir.y = 0f;
                float mag = dir.magnitude;
                if (mag > 0.1f)
                {
                    float mult = Ctx.Obtener<float>("multVel");
                    if (mult <= 0f) mult = 1f;
                    ag.cuerpo.Mover((dir / mag) * (VELOCIDAD * mult), dt);
                    ag.cuerpo.Mirar(dir, dt);
                }
            }
            return null;
        }
        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}