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
            maq = new Maquina(new List<Estado>
            {
                new BrutoAvanzar(),
                new BrutoMuerto(),
                new BrutoPerseguir(),
                new BrutoCargarGolpe(),
                new BrutoGolpePesado()
            }, ctx);
        }

        void OnEnable()
        {
            ctx.Fijar<Agente>("objetivo", null);
            maq.Iniciar("BrutoAvanzar");
            ag.salud.AlMorir += AlMorir;
        }

        void OnDisable()
        {
            ag.salud.AlMorir -= AlMorir;
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