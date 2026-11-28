using UnityEngine;
namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoFestejo : Estado
    {
        Agente ag;
        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("gente");
            if (ag != null)
            {
                ag.cuerpo.Detener();
                ag.Animar(Anim.Celebrar1);
            }
            Debug.Log("MagoEnemigo: entra a Festejo");
        }

        public override string Actualizar(float dt)
        {
            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
