using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroIrAlGranero : Estado
    {
        Agente ag;
        Transform granero;
        Camino camino;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            if (ag == null || Juego.I == null) return;

            granero = null;
            camino = null;
            granero = Juego.I.puntoGranero;
            if (granero == null) return;

            granero = Juego.I.puntoGranero;
            if (granero == null) return;
            camino = new Camino();
            camino.Calcular(ag.Pos, granero.position);
            ag.Animar(Anim.Caminar);
            Debug.Log("Minero: regresando al granero");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null || granero == null) return null;

            if ((Juego.I.granero != null && Juego.I.granero.DistanciaBorde(ag.Pos) <= 1f) || (camino != null && camino.Valido && camino.Llego(ag.Pos)))
            {
                ag.cuerpo.Detener();
                Debug.Log("Minero: llego al granero");
                return "MineroDepositar";
            }

            Vector3 destino = granero.position;
            if (camino != null && camino.Valido)
            {
                destino = camino.Siguiente(ag.Pos);
            }

            Vector3 direccion = destino - ag.Pos;
            direccion.y = 0f;
            if (direccion.sqrMagnitude > 0.01f)
            {
                ag.cuerpo.Mover(direccion.normalized * 1.8f, dt);
            }
            else
            {
                ag.cuerpo.Detener();
            }
            return null;
        }
    }
}