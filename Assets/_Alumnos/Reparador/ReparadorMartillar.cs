using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorMartillar : Estado
    {
        float timerFx;

        public override void Entrar(string anterior)
        {
            Debug.Log("Reparador: entre a Martillar");
            timerFx = 0f;

            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente != null)
            {
                if (agente.cuerpo != null) agente.cuerpo.Detener();
                agente.Animar("MartillarLoop");
            }
        }

        public override string Actualizar(float dt)
        {
            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente == null) return null;

            Agente enemigo = Sensor.HostilMasCercano(agente, agente.vistaMetros);
            if (enemigo != null)
            {
                return "ReparadorHuir";
            }

            Agente granero = BuscarGranero();
            if (granero == null || granero.salud == null)
            {
                return "ReparadorEsperar";
            }

            if (granero.salud.vida >= granero.salud.vidaMax)
            {
                return "ReparadorEsperar";
            }

            granero.salud.Curar(4f * dt);

            timerFx += dt;
            if (timerFx >= 2f)
            {
                timerFx = 0f;
                Efectos.Reproducir(Fx.Curacion, granero.Pos);
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }

        Agente BuscarGranero()
        {
            for (int i = 0; i < Agente.Todos.Count; i++)
            {
                if (Agente.Todos[i] != null && Agente.Todos[i].rol == Rol.Granero)
                {
                    return Agente.Todos[i];
                }
            }
            return null;
        }
    }
}