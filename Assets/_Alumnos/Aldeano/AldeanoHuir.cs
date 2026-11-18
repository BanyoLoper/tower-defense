using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoHuir : Estado
    {
        Agente ag;
        AldeanoController ctrl;
        float consultaEn;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            ctrl = Ctx.Obtener<AldeanoController>("ctrl");
            consultaEn = 0f;
            ag.cuerpo.velocidadMax = 3.2f;
            ag.Animar(Anim.Correr);
            Debug.Log("[" + ag.name + "] Aldeano: entre a Huir desde " + anterior, ag);
        }

        public override string Actualizar(float dt)
        {
            consultaEn -= dt;
            if (consultaEn <= 0f)
            {
                consultaEn = 0.2f;
                Agente hostil = Sensor.HostilMasCercano(ag, 10.001f);
                if (hostil != null) ctrl.RecordarAmenaza(hostil.Pos);
                else if (Sensor.DistanciaPlana(ag.Pos, ctrl.Amenaza) > 10f)
                {
                    // Clase 3: Esconderse se agrega en la siguiente entrega.
                    ag.cuerpo.Detener();
                    return "AldeanoVagar";
                }
            }

            Vector3 lejos = ag.Pos - ctrl.Amenaza;
            lejos.y = 0f;
            if (lejos.sqrMagnitude < 0.001f) lejos = ag.transform.forward;
            Vector3 alGranero = Ruta.Granero - ag.Pos;
            alGranero.y = 0f;
            // La huida domina: el sesgo al granero no puede invertirla.
            Vector3 direccion = lejos.normalized + alGranero.normalized * 0.35f;
            ctrl.MoverSeguro(direccion.normalized * ag.cuerpo.velocidadMax, dt);
            return null;
        }
    }
}
