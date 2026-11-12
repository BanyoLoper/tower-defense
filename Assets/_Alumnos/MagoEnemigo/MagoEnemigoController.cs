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
            maq = new Maquina(new List<Estado>
            { new MagoEnemigoAvanzar(),
            new MagoEnemigoPosicionarse(),
            new MagoEnemigoMuerto()
            }, ctx);
        }
        void OnEnable()
        {
            maq.Iniciar("MagoEnemigoAvanzar");
            ag.salud.AlMorir += AlMorir;
        }

         void OnDisable()
        {

            ag.salud.AlMorir -= AlMorir;
            
        }
        void AlMorir(GameObject origen)
        {
            maq.CambiarA("MagoEnemigoMuerto");
        }
        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}
