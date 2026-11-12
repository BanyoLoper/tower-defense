using UnityEngine;
namespace Alumnos.MagoEnemigo
{ 
    public class MagoEnemigoMuerto : Estado
    {
        Agente ag;
        public override void Entrar(string anterior)
        {
           Agente  ag = Ctx.Obtener<Agente>("agente");
            if (ag != null)
            {
                ag.cuerpo.Detener();
                ag.Animar(Anim.Muerte1);
            }
            Debug.Log(" MagoEnemigo entra a: Muerto");
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
