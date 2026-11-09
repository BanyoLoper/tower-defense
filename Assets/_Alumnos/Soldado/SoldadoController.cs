
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Soldado
{
    public class SoldadoController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

       // void Awake()
        //{
            //ctx.Fijar("agente", GetComponent<Agente>());
            //ctx.Fijar("ctrl", this);

           // maq = new Maquina(
                //new List<Estado> { new SoldadoAvanzar() },
                //ctx
            //);
        //}

        void OnEnable()
        {
            maq.Iniciar("SoldadoAvanzar");
        }

        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}
