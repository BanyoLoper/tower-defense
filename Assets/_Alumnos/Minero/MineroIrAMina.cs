using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroIrAMina : Estado
    {
        Agente ag;
        Transform mina;
        Camino camino;
        float tiempoSensor;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            mina = null;
            camino = null;
            tiempoSensor = 0f;

            if (ag == null || Juego.I == null) return;

            Transform[] minas = Juego.I.puntosMina;
            if (minas == null || minas.Length == 0) return;

            float mejorDistancia = float.MaxValue;

            foreach (Transform punto in minas)
            {
                if (punto == null) continue;

                Vector3 diferencia = punto.position - ag.Pos;
                diferencia.y = 0f;
                float distancia = diferencia.sqrMagnitude;

                if (distancia < mejorDistancia)
                {
                    mejorDistancia = distancia;
                    mina = punto;
                }
            }

            if (mina == null) return;

            camino = new Camino();
            camino.Calcular(ag.Pos, mina.position);
            ag.Animar(Anim.Caminar);
            Debug.Log("Minero: caminando hacia la mina");
        }

        public override string Actualizar(float dt)
        {
            if (ag == null || mina == null) return null;

            tiempoSensor -= dt;

            if (tiempoSensor <= 0f)
            {
                tiempoSensor = 0.2f;

                Agente enemigo = Sensor.HostilMasCercano(ag, 12f);

                if (enemigo != null)
                {
                    ag.cuerpo.Detener();
                    Debug.Log("Minero: enemigo detectado, huyendo");
                    return "MineroHuir";
                }
            }

            if (Sensor.DistanciaPlana(ag.Pos, mina.position) <= 1f)
            {
                ag.cuerpo.Detener();
                Debug.Log("Minero: llego a la mina");
                return "MineroMinar";
            }

            Vector3 destino = mina.position;

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