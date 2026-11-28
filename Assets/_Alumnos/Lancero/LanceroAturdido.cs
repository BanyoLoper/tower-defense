using UnityEngine;

namespace Alumnos.Lancero
{
    public class LanceroAturdido : Estado
    {
        Agente agente;
        float tiempoAturdido;
        const float DURACION_ATURDIDO = 0.4f;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            agente.Animar(Anim.Dano1);
            tiempoAturdido = 0f;
        }

        public override string Actualizar(float dt)
        {
            tiempoAturdido += dt;

            if (tiempoAturdido >= DURACION_ATURDIDO)
            {
                return "LanceroAcercarse";
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}