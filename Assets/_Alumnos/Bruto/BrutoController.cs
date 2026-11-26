using System.Collections.Generic;
using UnityEngine;
namespace Alumnos.Bruto
{
    public class BrutoController : MonoBehaviour
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
            {
                new BrutoAvanzar(),
                new BrutoMuerto(),
                new BrutoPerseguir(),
                new BrutoCargarGolpe(),
                new BrutoGolpePesado(),
                new BrutoRecuperar(),
                new BrutoFuria()
            }, ctx);
        }
        void OnEnable()
        {
            ctx.Fijar<Agente>("objetivo", null);
            ctx.Fijar("furiaUsada", false);
            ctx.Fijar("multVel", 1f);
            maq.Iniciar("BrutoAvanzar");
            ag.salud.AlMorir += AlMorir;
            ag.salud.AlDanar += AlDanar;
        }
        void OnDisable()
        {
            ag.salud.AlMorir -= AlMorir;
            ag.salud.AlDanar -= AlDanar;
        }
        void AlMorir(GameObject origen)
        {
            maq.CambiarA("BrutoMuerto");
        }
        void AlDanar(float cantidad, GameObject origen)
        {
            if (ag.salud.vida <= 0f) return;
            if (ctx.Obtener<bool>("furiaUsada")) return;
            if (ag.salud.Porcentaje < 0.4f) maq.CambiarA("BrutoFuria");
        }
        void Update()
        {
            maq.Actualizar(Time.deltaTime);
        }
    }
}
