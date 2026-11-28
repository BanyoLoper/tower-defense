using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new TutorialBienvenida(), new TutorialElegirPlataforma(), new TutorialConstruir(), new TutorialGemas(), new TutorialLlamarOleada(), new TutorialObservar(), new TutorialFin() }, ctx);
        }

        void OnEnable() { maq.Iniciar("TutorialBienvenida"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}
