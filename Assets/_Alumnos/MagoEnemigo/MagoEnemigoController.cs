using UnityEngine;
using System.Collections.Generic;




namespace Alumnos.MagoEnemigo
{


    public class MagoEnemigoController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();
        Agente ag;
        void Awake()
        {
            ag = GetComponent<Agente>();
            ctx.Fijar("agente", ag);
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new MagoEnemigoAvanzar() }, ctx);
        }


        void OnEnable()
        {
            maq.Iniciar("MagoEnemigoAvanzar");
        }

        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}
