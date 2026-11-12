
using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoAvanzar : Estado
    {
        Agente agente;
        int indice;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            indice = 0;
            agente.Animar(Anim.Caminar);
            Debug.Log("Soldado: entre a Avanzar");
        }

        public override string Actualizar(float dt)
        {
            Vector3 velocidad = Steering.PathFollowing(agente.Pos,Ruta.Puntos,ref indice,0.6f,agente.cuerpo.velocidadMax);
            agente.cuerpo.Mover(velocidad, dt);

            if (velocidad != Vector3.zero)
            {
                agente.cuerpo.Mirar(velocidad, dt);
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
