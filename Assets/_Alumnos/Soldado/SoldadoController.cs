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
            maq = new Maquina(new List<Estado>{new SoldadoAvanzar(),new SoldadoMuerto(),new SoldadoPerseguir(),new SoldadoAtacar(),new SoldadoAturdido(),new SoldadoFestejo()},ctx);
        }

        void OnEnable()
        {
            maq.Iniciar("SoldadoAvanzar");
            agente.salud.AlMorir += AlMorir;
            agente.salud.AlDanar += AlDanar;
        }
        
        void OnDisable()
        {
            agente.salud.AlMorir -= AlMorir;
            agente.salud.AlDanar -= AlDanar;
        }
        
        void AlMorir(GameObject origen)
        {
            maq.CambiarA("SoldadoMuerto");
        }
        
        void AlDanar(float cantidad, GameObject origen)
        {
            if (agente.salud.vida <= 0f)
            {
                return;
            }

            if (maq.Actual.Nombre == "SoldadoAturdido")
            {
                return;
            }

            maq.CambiarA("SoldadoAturdido");
        }
        
        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}

