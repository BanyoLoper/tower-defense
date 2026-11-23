using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoAtacar : Estado
    {
        Agente agente;
        Agente objetivo;
        float tiempo;
        bool golpeo;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            objetivo = Sensor.HostilMasCercano(agente, 2f);
            tiempo = 0f;
            golpeo = false;
            agente.cuerpo.Detener();
            agente.Animar(Anim.Atacar1);
        }

        public override string Actualizar(float dt)
        {
            if (objetivo == null || !objetivo.Vivo)
            {
                return "SoldadoAvanzar";
            }

            float distancia = agente.DistanciaBorde(objetivo.Pos);

            if (distancia > 2f)
            {
                return "SoldadoPerseguir";
            }

            tiempo += dt;

            if (tiempo >= 0.5f && !golpeo)
            {
                objetivo.salud.Danar(10f, agente.gameObject);
                golpeo = true;
            }

            if (tiempo >= 1f)
            {
                tiempo = 0f;
                golpeo = false;
                agente.Animar(Anim.Atacar1, reiniciar: true);
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
