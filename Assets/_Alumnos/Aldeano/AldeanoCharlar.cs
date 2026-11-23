using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoCharlar : Estado
    {
        Agente ag;
        Agente companero;
        AldeanoController ctrl;
        float restante;
        float consultaEn;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            ctrl = Ctx.Obtener<AldeanoController>("ctrl");
            companero = Ctx.Obtener<Agente>("companero");
            restante = 3f;
            consultaEn = 0.2f;
            ag.cuerpo.Detener();
            ag.Animar(Anim.Saludar);
        }

        public override string Actualizar(float dt)
        {
            consultaEn -= dt;
            if (consultaEn <= 0f)
            {
                consultaEn = 0.2f;
                Agente hostil = Sensor.HostilMasCercano(ag, 7.001f);
                if (hostil != null && Sensor.DistanciaPlana(ag.Pos, hostil.Pos) <= 7f)
                {
                    ctrl.RecordarAmenaza(hostil.Pos);
                    return "AldeanoAsustarse";
                }
            }

            if (companero != null && companero.Vivo) ag.cuerpo.MirarA(companero.Pos, dt);
            restante -= dt;
            return restante <= 0f ? "AldeanoVagar" : null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return trigger == "alerta" || trigger == "enemigo_visto"
                ? "AldeanoAsustarse" : null;
        }
    }
}
