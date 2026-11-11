using JetBrains.Annotations;
using UnityEngine;

namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoAvanzar : Estado
    {
        Agente ag;
        float t;

    public override void Entrar(string anterior)
        {
            t = 0f;
            ag = Ctx.Obtener<Agente>("agente");
            if ( ag != null)
            {
                ag.Animar(Anim.Caminar);
            }
            Debug.Log("MagoEnemigo: entra a Avnazar");
        }


        public override string Actualizar(float dt)
        {

            if (ag == null)
                return null;


            t += dt;
            if(t >= 0.2f)

            {
                t = 0f;
                Agente hostil = Sensor.HostilMasCercano(ag, 11f);
                if (hostil != null)
                {
                    float dist = Vector3.Distance(ag.transform.position, hostil.transform.position);
                    if (dist <= 11f)
                    {
                        return "MagoEnemigoPosicionarse";
                    }
                }

            }
            if ( Ruta.Puntos != null && Ruta.Puntos.Length > 0)
            {
                Vector3 destino = Ruta.Puntos[Ruta.Puntos.Length - 1];
                Vector3 dir = destino - ag.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.01f)
                {
                    ag.cuerpo.Mover(dir.normalized , dt);
                }
               
            }
            return null;
        }
           public override string AlRecibir(string trigger , int valor)
        {
            return null;
        }



    }
        

        
    
}


