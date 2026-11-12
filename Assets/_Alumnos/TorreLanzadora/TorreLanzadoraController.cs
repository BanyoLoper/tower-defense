using UnityEngine;
using System.Collections.Generic;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new TorreLanzadoraVigilando(), new TorreLanzadoraDestruida(), new TorreLanzadoraApuntando() }, ctx);   // por ahora solo este estado

            Salud salud = GetComponent<Salud>();
            salud.AlMorir += Destruido;
        }

        void Destruido(GameObject origen)
        {
           maq.CambiarA("TorreLanzadoraDestruida");
        }
        void OnEnable() { maq.Iniciar("TorreLanzadoraVigilando"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}
