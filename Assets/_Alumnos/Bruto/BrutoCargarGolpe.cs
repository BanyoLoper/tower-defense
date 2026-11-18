using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoCargarGolpe : Estado
    {
        Agente ag;
        Cuerpo cuerpo;
        Agente objetivo;
        float t;
        const float TIEMPO_CARGA = 0.7f;
        public override void Entrar(string anterior)
        {
            t = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            cuerpo = Ctx.Obtener<Cuerpo>("cuerpo");
            objetivo = Ctx.Obtener<Agente>("objetivo");

            if (cuerpo != null)
            {
                cuerpo.Detener();
            }

            if (ag != null)
            {
                ag.Animar(Anim.Atacar1);
            }
        }
        public override string Actualizar(float dt)
        {
            t += dt;

            if (cuerpo != null)
            {
                cuerpo.Detener();
            }

            if (ag != null && cuerpo != null && objetivo != null && objetivo.gameObject.activeInHierarchy)
            {
                Vector3 dir = objetivo.transform.position - ag.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                {
                    cuerpo.Mirar(dir, dt);
                }
            }

            if (t >= TIEMPO_CARGA)
            {
                return "BrutoPerseguir";
            }
            return null;
        }
        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
