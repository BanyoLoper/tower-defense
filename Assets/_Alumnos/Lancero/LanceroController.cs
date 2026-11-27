// Assets/_Alumnos/Lancero/LanceroController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Lancero
{
    public class LanceroController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);

            maq = new Maquina(new List<Estado> { new LanceroAvanzar(), new LanceroMuerto(),  new LanceroAcercarse(), new LanceroEstocada(), new LanceroAturdido(), new LanceroRetroceder(), new LanceroFestejo() }, ctx);  

          

        }

        void OnEnable() { maq.Iniciar("LanceroAvanzar"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}


