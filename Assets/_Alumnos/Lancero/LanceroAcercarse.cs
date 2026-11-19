using UnityEngine;
using System;

namespace Alumnos.Lancero
{
    public class LanceroAcercarse : Estado
    {
        Agente agente;

        public override void Entrar(string anterior)
        {
            agente = Ctx.Obtener<Agente>("agente");
            agente.Animar(Anim.Correr);
        }

        public override string Actualizar(float dt)
        {
            var hostilObj = agente.ObtenerHostilCercano();
            if (hostilObj == null)
            {
                return "LanceroAvanzar";
            }

            Transform hostilTransform = hostilObj as Transform;
            if (hostilTransform == null)
            {
                GameObject go = hostilObj as GameObject;
                if (go != null)
                {
                    hostilTransform = go.transform;
                }

                if (hostilTransform == null)
                {
                    return "LanceroAvanzar";
                }
            }

            float distancia = Vector3.Distance(agente.transform.position, hostilTransform.position);

            if (distancia <= 2.8f)
            {
                return "LanceroEstocada";
            }

            return null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}