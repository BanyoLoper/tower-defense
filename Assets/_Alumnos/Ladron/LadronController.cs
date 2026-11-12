using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Ladron
{
    public class LadronController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new LadronMerodear() }, ctx);   // por ahora solo este estado
        }

        void OnEnable() { maq.Iniciar("LadronMerodear"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}