using UnityEngine;

namespace Alumnos.TorreMaga
{
    public class TorreMagaEnfriando : Estado
    {
        Agente agente;
        float tiempo;
        const float DURACION = 2.2f;
        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            tiempo = 0f;
            agente.Animar(Anim.HechizoIdle);
            Debug.Log("TorreMaga: entre a Enfriando");
        }
        public override string Actualizar(float dt)
        {
            tiempo += dt;
            if (tiempo >= DURACION)
            {
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