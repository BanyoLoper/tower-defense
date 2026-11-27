using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoGolpePesado : Estado
    {
        const float DANO = 28f;
        const float RADIO_GOLPE = 2.6f;
        const float TIEMPO_IMPACTO = 0.25f;

        float t;
        bool golpeAplicado;
        Agente ag;

        public override void Entrar(string anterior)
        {
            t = 0f;
            golpeAplicado = false;
            ag = Ctx.Obtener<Agente>("agente");
            ag.cuerpo.Detener();
            ag.Animar(Anim.Atacar2);
        }

        public override string Actualizar(float dt)
        {
            t += dt;

            if (!golpeAplicado && t >= TIEMPO_IMPACTO)
            {
                AplicarGolpe();
                golpeAplicado = true;
                return "BrutoRecuperar";
            }

            return null;
        }

        void AplicarGolpe()
        {
            Vector3 frente = ag.transform.forward;
            frente.y = 0f;
            if (frente.sqrMagnitude > 0.001f) frente.Normalize();

            Agente[] agentes = Object.FindObjectsByType<Agente>(FindObjectsSortMode.None);
            foreach (Agente h in agentes)
            {
                if (h == null || h == ag || !h.Vivo) continue;
                if (!Agente.Hostiles(ag.bando, h.bando)) continue;
                if (h.DistanciaBorde(ag.Pos) > RADIO_GOLPE) continue;

                Vector3 dir = h.Pos - ag.Pos;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f && Vector3.Dot(frente, dir.normalized) < 0f) continue;

                h.salud.Danar(DANO, ag.gameObject);
            }
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}