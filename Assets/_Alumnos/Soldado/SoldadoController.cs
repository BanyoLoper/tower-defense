
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();
        Agente agente;

        void Awake()
        {
            agente = GetComponent<Agente>();

            ctx.Fijar("agente", agente);
            ctx.Fijar("ctrl", this);

            maq = new Maquina(new List<Estado> {new SoldadoAvanzar(),new SoldadoMuerto()},ctx);
        }

        void OnEnable()
        {
            maq.Iniciar("SoldadoAvanzar");
            agente.salud.AlMorir += AlMorir;
        }

        void OnDisable()
        {
            agente.salud.AlMorir -= AlMorir;
        }

        void AlMorir(GameObject origen)
        {
            maq.CambiarA("SoldadoMuerto");
        }

        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}

