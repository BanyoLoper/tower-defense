using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Guardia
{
    public class GuardiaController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new GuardiaPatrullar(), new GuardiaMuerto() }, ctx);   // por ahora solo este estado
        }

        void OnEnable() { maq.Iniciar("GuardiaPatrullar"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}
