using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoPerseguir : Estado
    {
        Agente agente;
        Agente objetivo;
        
        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            objetivo = Ctx.Obtener<Agente>("objetivo");
            Ctx.Fijar("objetivo", objetivo);
            agente.Animar(Anim.Correr);

            if (objetivo == null || !objetivo.Vivo)
            {
                objetivo = Sensor.HostilMasCercano(agente, 6f);
            }
        }
        
        public override string Actualizar(float dt)
        {
            if (objetivo == null || !objetivo.Vivo)
            {
                return "SoldadoAvanzar";
            }
            
            float distancia = agente.DistanciaBorde(objetivo.Pos);

            if (distancia > 6f)
            {
                return "SoldadoAvanzar";
            }

            if (distancia <= 2f)
            {
                agente.cuerpo.Detener();
                return "SoldadoAtacar";
            }


            Vector3 direccion = objetivo.Pos - agente.Pos;
            direccion.y = 0f;
            Vector3 velocidad = direccion.normalized *agente.cuerpo.velocidadMax;
            agente.cuerpo.Mover(velocidad, dt);
            agente.cuerpo.Mirar(velocidad, dt);
            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}

