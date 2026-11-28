using UnityEngine;
namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoAturdido : Estado
    {
        Agente ag;
        float t;
        public override void Entrar(string anterior)
        {
            t = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            if (ag != null)
            {
                ag.cuerpo.Detener();
                ag.Animar(Anim.Dano1);
            }
            Debug.Log("MagoEnemigo: entra a Aturdido");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null) return null;
            t += dt;
            if (t >= 0.4f)
            {
                return "MagoEnemigoPosicionarse";
            }
            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
