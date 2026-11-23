using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoAtacar : Estado
    {
        Agente agente;
        float tiempo;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            tiempo = 0f;
            agente.cuerpo.Detener();
            agente.Animar(Anim.Atacar1);
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (tiempo >= 1f)
            {
                tiempo = 0f;
                agente.Animar(Anim.Atacar1);
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
