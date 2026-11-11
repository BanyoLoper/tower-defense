
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("ctrl", this);

            maq = new Maquina(new List<Estado>
                {
                    new DirectorPreparacion(),
                    new DirectorAviso(),
                    new DirectorOleada()
                },
                ctx
            );
        }

        void OnEnable()
        {
            maq.Iniciar("DirectorPreparacion");
        }

        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}
