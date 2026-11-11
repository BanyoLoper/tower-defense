// Assets/_Alumnos/Aldeano/AldeanoController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new AldeanoVagar() }, ctx);   // por ahora solo este estado
        }

        void OnEnable() { maq.Iniciar("AldeanoVagar"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}

