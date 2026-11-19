using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.TorreMaga
{
    public class TorreMagaController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();
        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);

            maq = new Maquina(
                new List<Estado>
                {
                    new TorreMagaVigilando(),
                    new TorreMagaCargando(),
                    new TorreMagaDestruida()
                },
                ctx
            );
        }
        void OnEnable()
        {
            maq.Iniciar("TorreMagaVigilando");
        }
        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}
