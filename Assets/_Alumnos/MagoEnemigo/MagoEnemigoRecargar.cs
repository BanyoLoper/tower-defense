using UnityEngine;
namespace Alumnos.MagoEnemigo
{
    public class MagoEenemigoRecargar : Estado
    {
        Agente ag;
        Agente objetivo;
        float t;
        public override void Entrar(string anterior)
        {
            t = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            if(ag != null)
            {
                ag.cuerpo.Detener();
                ag.Animar(Anim.HechizoIdle);
            }
            Debug.Log("MagoEnemigo: entra a Recargar");
        }

    }
}
