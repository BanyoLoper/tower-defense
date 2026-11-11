
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();
        Agente ag;

        void Awake()
        {
            ag = GetComponent<Agente>();

            ctx.Fijar("agente", ag);
            ctx.Fijar("ctrl", this);

            maq = new Maquina(
                new List<Estado>
                {
                    new MineroIrAMina(),
                    new MineroMinar(),
                    new MineroMuerto()
                },
                ctx
            );
        }

        void OnEnable()
        {
            ctx.Fijar("carga", 0);

            maq.Iniciar("MineroIrAMina");

            ag.salud.AlMorir += AlMorir;
        }

        void OnDisable()
        {
            if (ag != null && ag.salud != null)
            {
                ag.salud.AlMorir -= AlMorir;
            }
        }

        void AlMorir(GameObject origen)
        {
            maq.CambiarA("MineroMuerto");
        }

        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}


