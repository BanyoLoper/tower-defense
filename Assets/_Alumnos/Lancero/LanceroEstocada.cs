using UnityEngine;

namespace Alumnos.Lancero
{
    public class LanceroEstocada : Estado
    {
        Agente agente;
        float tiempoAtaque;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            agente.cuerpo.Detener();
            agente.Animar(Anim.Atacar1);
            tiempoAtaque = 0f;

            // Aplicar daño al hostil cercano si esta en rango
            var hostil = agente.ObtenerHostilCercano();
            if (hostil != null && Vector3.Distance(agente.transform.position, hostil.transform.position) <= 3.2f)
            {
                hostil.RecibirDanio(11);
            }
        }

        public override string Actualizar(float dt)
        {
            tiempoAtaque += dt;
            var hostil = agente.ObtenerHostilCercano();

            // Si el hostil se aleja mas de 3.2m, vuelve a acercarse
            if (hostil == null || Vector3.Distance(agente.transform.position, hostil.transform.position) > 3.2f)
            {
                return "LanceroAcercarse";
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}