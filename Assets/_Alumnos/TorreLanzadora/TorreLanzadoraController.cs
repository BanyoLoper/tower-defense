using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraController : MonoBehaviour
    {
        Maquina maq;
        Salud salud;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new TorreLanzadoraVigilando(), new TorreLanzadoraDestruida(), new TorreLanzadoraApuntando() }, ctx);   // por ahora solo este estado
  
        }
        void OnEnable() 
        { 
            maq.Iniciar("TorreLanzadoraVigilando");
            salud.AlMorir += Destruido;
        }
        void OnDisable() { salud.AlMorir -= Destruido;}
        void Destruido(GameObject origen) { maq.CambiarA("TorreLanzadoraDestruida"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}
