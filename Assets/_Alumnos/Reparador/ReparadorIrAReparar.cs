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
            if (agente == null) return null;

            // Ajusta el destino o llamadas segun los metodos de tu clase Agente / Steering
            Vector3 destino = agente.Objetivo;
            Vector3 haciaDestino = destino - agente.transform.position;
            float distancia = haciaDestino.magnitude;
            float radioFrenado = 2.0f;

            if (distancia > 0.05f)
            {
                float velocidad = agente.VelocidadMax;
                if (distancia < radioFrenado)
                {
                    velocidad *= (distancia / radioFrenado);
                }
                agente.Mover(haciaDestino.normalized * velocidad, dt);
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}