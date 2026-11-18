using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Jefe
{
    public class JefeController : MonoBehaviour
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
                    new JefeAparicion()
                },
                ctx
            );
        }

        void OnEnable()
        {
            maq.Iniciar("JefeAparicion");
        }

        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}
