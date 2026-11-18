// Assets/_Alumnos/Aldeano/AldeanoController.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            if (Sensor.DistanciaPlana(ag.Pos, posicion) <= radio)
            {
                Amenaza = posicion;
                maq.Disparar("alerta");
            }
        }

        public void RecordarAmenaza(Vector3 posicion) { Amenaza = posicion; }

        // Busca una direccion libre antes de que Cuerpo choque con el borde.
        public void MoverSeguro(Vector3 deseada, float dt)
        {
            deseada.y = 0f;
            if (ag.cuerpo.pegarANavMesh && deseada.sqrMagnitude > 0.001f)
            {
                Vector3 direccion = deseada.normalized;
                NavMeshHit hit;
                if (NavMesh.Raycast(ag.Pos, ag.Pos + direccion * 1.2f, out hit, NavMesh.AllAreas))
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        float grados = ((i + 1) / 2) * 60f * (i % 2 == 0 ? -1f : 1f);
                        Vector3 alternativa = Quaternion.AngleAxis(grados, Vector3.up) * direccion;
                        if (NavMesh.Raycast(ag.Pos, ag.Pos + alternativa * 1.2f, out hit, NavMesh.AllAreas)) continue;
                        deseada = alternativa * deseada.magnitude;
                        break;
                    }
                }
            }
            ag.cuerpo.Mover(deseada, dt);
        }

        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}

