using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();
        Agente ag;

        void Awake()
        {
            ag = GetComponent<Agente>();
            ctx.Fijar("agente", ag);
            ctx.Fijar("ctrl", this);
            ctx.Fijar("cuerpo", GetComponent<Cuerpo>());

            maq = new Maquina(new List<Estado>
            {
                new BrutoAvanzar(),
                new BrutoMuerto(),
                new BrutoPerseguir(),
                new BrutoCargarGolpe()
            }, ctx);
        }

        void OnEnable()
        {
            maq.Iniciar("BrutoAvanzar");
            if (ag != null && ag.salud != null)
            {
                ag.salud.AlMorir += AlMorir;
            }
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
            maq.CambiarA("BrutoMuerto");
        }

        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}