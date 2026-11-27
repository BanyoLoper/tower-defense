// Assets/_Alumnos/Aldeano/AldeanoVagar.cs
using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoVagar : Estado
    {
        Agente ag;
        float consultaEn;
        AldeanoController ctrl;
        float angulo;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            ctrl = Ctx.Obtener<AldeanoController>("ctrl");
            angulo = Random.Range(-Mathf.PI, Mathf.PI);
            consultaEn = 0.2f;
            ag.cuerpo.velocidadMax = 1.6f;
            ag.Animar(Anim.Caminar);
            Debug.Log("[" + ag.name + "] Aldeano: entre a Vagar desde " + anterior, ag);
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

                Agente companero = BuscarCompanero();
                if (companero != null)
                {
                    Ctx.Fijar("companero", companero);
                    return "AldeanoCharlar";
                }
            }

            Vector3 velocidad = Steering.Wander(ag.Pos, ag.transform.forward,
                ref angulo, 2f, 1.5f, 0.18f, ag.cuerpo.velocidadMax);
            Vector3 aCasa = ctrl.Casa - ag.Pos;
            aCasa.y = 0f;
            if (aCasa.sqrMagnitude > 100f)
                velocidad = (velocidad + aCasa.normalized * 3f).normalized * ag.cuerpo.velocidadMax;
            ag.cuerpo.Mover(velocidad, dt);
            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return trigger == "alerta" || trigger == "enemigo_visto"
                ? "AldeanoAsustarse" : null;
        }

        Agente BuscarCompanero()
        {
            foreach (Agente otro in Agente.Todos)
            {
                if (otro == ag || otro.rol != Rol.Aldeano || !otro.Vivo) continue;
                if (Sensor.DistanciaPlana(ag.Pos, otro.Pos) < 3f) return otro;
            }
            return null;
        }
    }
}
