using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorIrAReparar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Reparador: entre a IrAReparar");

            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente != null)
            {
                agente.Animar("Caminar");
            }
        }

        public override string Actualizar(float dt)
        {
            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente == null || agente.cuerpo == null) return null;

            Agente granero = BuscarGranero();
            if (granero != null)
            {
                Vector3 direccion = (granero.Pos - agente.Pos).normalized;
                agente.cuerpo.Mover(direccion, dt);
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