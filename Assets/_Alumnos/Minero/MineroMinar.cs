using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroMinar : Estado
    {
        float tiempo;
        bool terminado;
        Agente ag;
        float tiempoSensor;

        public override void Entrar(string anterior)
        {
            tiempo = 0f;
            terminado = false;
            tiempoSensor = 0f;
            ag = Ctx.Obtener<Agente>("agente");

            if (ag == null) return;

            ag.cuerpo.Detener();
            ag.Animar(Anim.MinarSuelo, 0.2f, true);
            Debug.Log("Minero: empezando a minar");
        }
        public override string Actualizar(float dt)
        {
            if (terminado) return null;
            if (ag == null) return null;

            tiempoSensor -= dt;

            if (tiempoSensor <= 0f)
            {
                tiempoSensor = 0.2f;
                Agente enemigo = Sensor.HostilMasCercano(ag, 12f);

                if (enemigo != null)
                {
                    ag.cuerpo.Detener();
                    Debug.Log("Minero: enemigo detectado mientras minaba");
                    return "MineroHuir";
                }
            }

            tiempo += dt;

            if (tiempo >= 5f)
            {
                terminado = true;
                Ctx.Fijar("carga", 6);
                if (ag != null) ag.Animar(Anim.Idle);
                Debug.Log("Minero: termino de minar");
                return "MineroIrAlGranero";
            }
            return null;
        }
    }
}