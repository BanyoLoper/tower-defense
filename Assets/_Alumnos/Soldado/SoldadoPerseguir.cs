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
            objetivo = Sensor.HostilMasCercano(agente, 6f);
            agente.Animar(Anim.Correr);
        }

        public override string Actualizar(float dt)
        {
            if (objetivo == null || !objetivo.Vivo)
            {
                return "SoldadoAvanzar";
            }

            float distancia = Sensor.DistanciaPlana(agente.Pos,objetivo.Pos);

            if (distancia > 6f)
            {
                return "SoldadoAvanzar";
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

