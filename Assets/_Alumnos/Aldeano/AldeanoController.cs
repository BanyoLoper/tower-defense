// Assets/_Alumnos/Aldeano/AldeanoController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();
        Agente ag;
        Salud salud;
        public Vector3 Amenaza { get; private set; }
        public Vector3 Casa { get; private set; }

        void Awake()
        {
            ag = GetComponent<Agente>();
            salud = GetComponent<Salud>();
            ctx.Fijar("agente", ag);
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado>
            {
                new AldeanoVagar(),
                new AldeanoMuerto(),
                new AldeanoAsustarse(),
                new AldeanoHuir()
            }, ctx);
        }

        void OnEnable()
        {
            Casa = transform.position;
            Amenaza = Casa;
            maq.Iniciar("AldeanoVagar");
            salud.AlMorir += AlMorir;
            Alertas.AlAlerta += AlAlerta;
        }

        void OnDisable()
        {
            salud.AlMorir -= AlMorir;
            Alertas.AlAlerta -= AlAlerta;
        }

        void AlMorir(GameObject origen)
        {
            maq.CambiarA("AldeanoMuerto");
        }

        void AlAlerta(Vector3 posicion, float radio, Agente origen)
        {
            if (!ag.Vivo || radio < 0f) return;
            if (Sensor.DistanciaPlana(ag.Pos, posicion) > radio) return;

            Amenaza = posicion;
            maq.Disparar("alerta");
        }

        public void RecordarAmenaza(Vector3 posicion) { Amenaza = posicion; }

        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}

