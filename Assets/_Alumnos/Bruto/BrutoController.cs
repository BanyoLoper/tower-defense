using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Bruto
{
    public class BrutoController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new BrutoAvanzar() }, ctx);   // por ahora solo este estado
        }

        void OnEnable() { maq.Iniciar("BrutoAvanzar"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}