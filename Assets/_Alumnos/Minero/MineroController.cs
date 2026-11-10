
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Minero
{
    public class MineroController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);

            maq = new Maquina(
                new List<Estado>
                {
                    new MineroIrAMina()
                },
                ctx
            );
        }

        void OnEnable()
        {
            maq.Iniciar("MineroIrAMina");
        }

        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}

