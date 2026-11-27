using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoCargarGolpe : Estado
    {
        const float TIEMPO_CARGA = 0.7f;

        float t;
        Agente ag;

        public override void Entrar(string anterior)
        {
            t = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            ag.cuerpo.Detener();
            ag.Animar(Anim.Atacar1);
        }

        public override string Actualizar(float dt)
        {
            t += dt;

            Agente objetivo = Ctx.Obtener<Agente>("objetivo");
            if (objetivo != null && objetivo.Vivo)
            {
                Vector3 dir = objetivo.Pos - ag.Pos;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                {
                    ag.cuerpo.Mirar(dir, dt);
                }
            }

            if (t >= TIEMPO_CARGA)
            {
                return "BrutoGolpePesado";
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}