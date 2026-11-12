using UnityEngine;

namespace Alumnos.TorreMaga
{
    public class TorreMagaCargando : Estado
    {
        float tiempo;
        public override void Entrar(string anterior)
        {
            tiempo = 0f;
            Debug.Log("TorreMaga: entre a Cargando");
            Agente agente = Ctx.Obtener<Agente>("agente");
            if (agente != null)
            {
                agente.Animar(Anim.HechizoIdle);
                Efectos.Reproducir(
                    Fx.Aura,
                    agente.transform.position + Vector3.up * 1.5f,
                    1f,
                    1.2f
                );
            }
        }
        public override string Actualizar(float dt)
        {
            tiempo += dt;
            if (tiempo >= 1.2f)
            {
                Debug.Log("TorreMaga: termine de cargar");
                return "TorreMagaVigilando";
            }
            return null;
        }
        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
