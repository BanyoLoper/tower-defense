using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorHuir : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Reparador: entre a Huir");

            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente != null)
            {
                agente.Animar("Correr");
            }
        }

        public override string Actualizar(float dt)
        {
            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente == null || agente.cuerpo == null) return null;

            Agente enemigo = Sensor.HostilMasCercano(agente, agente.vistaMetros);
            if (enemigo == null)
            {
                agente.cuerpo.Detener();
                return "ReparadorEsperar";
            }

            Vector3 direccionHuida = (agente.Pos - enemigo.Pos).normalized;
            agente.cuerpo.Mover(direccionHuida, dt);

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}