// Assets/_Alumnos/Aldeano/AldeanoVagar.cs
using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoVagar : Estado
    {
        Agente ag;
        float consultaEn;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            consultaEn = 0.2f;
            ag.Animar(Anim.Caminar);
        }

        public override string Actualizar(float dt)
        {
            consultaEn -= dt;
            if (consultaEn > 0f) return null;
            consultaEn = 0.2f;
            // HostilMasCercano usa < radio; incluir tambien el limite de 7 m.
            Agente hostil = Sensor.HostilMasCercano(ag, 7.001f);
            return hostil != null && Sensor.DistanciaPlana(ag.Pos, hostil.Pos) <= 7f
                ? "AldeanoAsustarse" : null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return trigger == "alerta" || trigger == "enemigo_visto"
                ? "AldeanoAsustarse" : null;
        }
    }
}
