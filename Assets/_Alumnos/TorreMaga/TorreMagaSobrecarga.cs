using UnityEngine;

namespace Alumnos.TorreMaga
{
    public class TorreMagaSobrecarga : Estado
    {
        Agente agente;
        float tiempo;
        float tiempoFuego;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");

            tiempo = 0f;
            tiempoFuego = 0f;

            agente.Animar(Anim.Idle);

            Debug.Log("TorreMaga: entre a Sobrecarga");
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;
            tiempoFuego += dt;

            if (tiempoFuego >= 1.5f)
            {
                tiempoFuego = 0f;

                Efectos.Reproducir(
                    Fx.Fuego,
                    agente.PuntoPecho,
                    7f,
                    1.5f
                );
            }

            if (tiempo >= 4f)
            {
                Ctx.Fijar("contador_lanzamientos", 0);
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
