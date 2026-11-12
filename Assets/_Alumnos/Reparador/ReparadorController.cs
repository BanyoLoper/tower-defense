using UnityEngine;
using System.Collections.Generic;

namespace Alumnos.Reparador
{
    public class ReparadorController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado>
            {
                new ReparadorEsperar(),
                new ReparadorMuerto()
            }, ctx);
        }

        void OnEnable() { maq.Iniciar("ReparadorEsperar"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}