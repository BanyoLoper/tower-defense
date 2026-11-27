using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoAturdido : Estado
    {
        Agente agente;
        float tiempo;
        string estadoAnterior;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            estadoAnterior = anterior;
            tiempo = 0f;
            agente.cuerpo.Detener();
            agente.Animar(Anim.Dano1);
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (tiempo >= 0.4f)
            {
                return estadoAnterior;
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
