using UnityEngine;

namespace Alumnos.TorreMaga
{
    public class TorreMagaLanzando : Estado
    {
        Agente agente;
        float tiempo;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");

            tiempo = 0f;

            agente.Animar(Anim.HechizoLanzar);

            Debug.Log("TorreMaga: lanzando hechizo");
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (tiempo >= 0.8f)
            {
                return "TorreMagaEnfriando";
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
