using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroDepositar : Estado
    {
        Agente ag;
        float tiempo;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            if (ag == null) return;

            tiempo = 0f;
            ag.cuerpo.Detener();
            ag.Animar(Anim.Recolectar);

            ag.cuerpo.Detener();
            ag.Animar(Anim.Recolectar);
            if (Ctx.Obtener<int>("carga") == 6)
            {
                Economia.Agregar(6);
                Ctx.Fijar("carga", 0);
                Debug.Log("Minero: deposito 6 gemas");
            }
        }

        public override string Actualizar(float dt)
        {
            tiempo += dt;
            if (tiempo >= 1f) return "MineroIrAMina";
            return null;
        }
    }
}