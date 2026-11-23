using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Aldeano
{
    public class AldeanoEsconderse : Estado
    {
        Agente ag;
        AldeanoController ctrl;
        readonly List<Collider> escondites = new List<Collider>();
        readonly List<Collider> obstaculos = new List<Collider>();
        Collider cobertura;
        Vector3 destino;
        readonly Vector3[] ruta = new Vector3[3];
        int siguiente;
        int pasosRuta;
        float consultaEn;
        float sinPeligro;
        bool peligro;

        public override void Entrar(string anterior)
        {
            ag = Ctx.Obtener<Agente>("agente");
            ctrl = Ctx.Obtener<AldeanoController>("ctrl");
            consultaEn = 0f;
            sinPeligro = 0f;
            peligro = false;
            cobertura = null;
            ag.cuerpo.Detener();
            ag.cuerpo.velocidadMax = 1.6f;
            ag.Animar(Anim.Agachado);

            // Bloqueo identifica edificios; HardRock identifica las rocas del mapa.
            escondites.Clear();
            obstaculos.Clear();
            foreach (Collider col in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))
            {
                if (col.isTrigger || !col.enabled) continue;
                if (col.bounds.size.y > 1.1f && col.GetComponentInParent<Agente>() == null)
                    obstaculos.Add(col);
                if (col.name == "Bloqueo" && !obstaculos.Contains(col)) obstaculos.Add(col);
                if (col.name == "Bloqueo" || col.name.StartsWith("HardRock")) escondites.Add(col);
            }
        }

        public override string Actualizar(float dt)
        {
            consultaEn -= dt;
            if (consultaEn <= 0f)
            {
                consultaEn = 0.2f;
                Agente hostil = Sensor.HostilMasCercano(ag, 10.001f);
                peligro = hostil != null;
                if (hostil != null) ctrl.RecordarAmenaza(hostil.Pos);
                if (cobertura == null || !TapaAmenaza(cobertura, destino)) ElegirEscondite();
            }

            if (cobertura == null)
            {
                ag.cuerpo.Detener();
                sinPeligro = 0f;
                return null;
            }

            Vector3 objetivo = ruta[siguiente];
            bool llego = Sensor.DistanciaPlana(ag.Pos, objetivo) <= 0.3f;
            if (llego && siguiente < pasosRuta - 1)
            {
                siguiente++;
                objetivo = ruta[siguiente];
                llego = false;
            }
            if (!llego)
            {
                Vector3 velocidad = Steering.Arrive(ag.Pos, objetivo, ag.cuerpo.velocidadMax, 2f);
                ag.cuerpo.Mover(velocidad, dt);
                sinPeligro = 0f;
                return null;
            }

            ag.cuerpo.Detener();
            sinPeligro = peligro ? 0f : sinPeligro + dt;
            return sinPeligro >= 4f ? "AldeanoVagar" : null;
        }

        public override string AlRecibir(string trigger, int valor)
        {
            if (trigger == "alerta" || trigger == "enemigo_visto")
            {
                sinPeligro = 0f;
                peligro = true;
            }
            return null;
        }

        void ElegirEscondite()
        {
            cobertura = null;
            float mejorDistancia = float.PositiveInfinity;
            foreach (Collider col in escondites)
            {
                if (col == null || !col.enabled) continue;
                Bounds limites = col.bounds;
                Vector3 lejos = limites.center - ctrl.Amenaza;
                lejos.y = 0f;
                if (lejos.sqrMagnitude < 0.001f) continue;
                float radio = Mathf.Max(limites.extents.x, limites.extents.z);
                Vector3 punto = limites.center + lejos.normalized * (radio + 1.2f);
                punto.y = ag.Pos.y;
                ConsiderarPunto(col, punto, ref mejorDistancia);

                // Las esquinas permiten alcanzar cobertura sin atravesar el edificio.
                for (int i = 0; i < 4; i++)
                {
                    punto.x = (i % 2 == 0 ? limites.min.x - 1.2f : limites.max.x + 1.2f);
                    punto.z = (i < 2 ? limites.min.z - 1.2f : limites.max.z + 1.2f);
                    ConsiderarPunto(col, punto, ref mejorDistancia);
                }
            }
        }

        void ConsiderarPunto(Collider col, Vector3 punto, ref float mejorDistancia)
        {
            float distancia = Sensor.DistanciaPlana(ag.Pos, punto);
            if (distancia >= mejorDistancia || !TapaAmenaza(col, punto)) return;
            Vector3 primero;
            Vector3 segundo;
            int pasos;
            if (!BuscarRuta(col.bounds, punto, out primero, out segundo, out pasos)) return;

            cobertura = col;
            destino = punto;
            mejorDistancia = distancia;
            siguiente = 0;
            pasosRuta = pasos;
            ruta[0] = primero;
            ruta[1] = segundo;
            ruta[pasos - 1] = destino;
        }

        // Para alcanzar la cara oculta, rodea el escondite por una o dos esquinas.
        bool BuscarRuta(Bounds limites, Vector3 punto, out Vector3 primero, out Vector3 segundo, out int pasos)
        {
            primero = punto;
            segundo = punto;
            pasos = 1;
            if (CaminoDespejado(ag.Pos, punto)) return true;

            for (int i = 0; i < 4; i++)
            {
                Vector3 esquina = Esquina(limites, i);
                if (!CaminoDespejado(ag.Pos, esquina)) continue;
                if (CaminoDespejado(esquina, punto))
                {
                    primero = esquina;
                    pasos = 2;
                    return true;
                }

                Vector3 adyacente;
                if (!RodearDesde(limites, i, punto, out adyacente)) continue;
                primero = esquina;
                segundo = adyacente;
                pasos = 3;
                return true;
            }
            return false;
        }

        bool RodearDesde(Bounds limites, int indice, Vector3 punto, out Vector3 adyacente)
        {
            Vector3 esquina = Esquina(limites, indice);
            for (int i = 0; i < 4; i++)
            {
                adyacente = Esquina(limites, i);
                if (!CaminoDespejado(esquina, adyacente)) continue;
                if (CaminoDespejado(adyacente, punto)) return true;
            }
            adyacente = punto;
            return false;
        }

        Vector3 Esquina(Bounds limites, int indice)
        {
            float x = indice % 2 == 0 ? limites.min.x - 1.2f : limites.max.x + 1.2f;
            float z = indice < 2 ? limites.min.z - 1.2f : limites.max.z + 1.2f;
            return new Vector3(x, ag.Pos.y, z);
        }

        bool CaminoDespejado(Vector3 desde, Vector3 punto)
        {
            Vector3 origen = desde + Vector3.up * ag.alturaPecho;
            Vector3 direccion = punto + Vector3.up * ag.alturaPecho - origen;
            Ray camino = new Ray(origen, direccion.normalized);
            foreach (Collider col in obstaculos)
            {
                if (col == null || !col.enabled) continue;
                Bounds limites = col.bounds;
                limites.Expand(ag.radio * 2f);
                if (limites.Contains(origen)) continue;
                float distancia;
                if (limites.IntersectRay(camino, out distancia) && distancia <= direccion.magnitude) return false;
            }
            return true;
        }

        bool TapaAmenaza(Collider col, Vector3 punto)
        {
            Vector3 origen = ctrl.Amenaza + Vector3.up * ag.alturaPecho;
            Vector3 haciaPunto = punto + Vector3.up * ag.alturaPecho - origen;
            RaycastHit hit;
            return col.Raycast(new Ray(origen, haciaPunto.normalized), out hit, haciaPunto.magnitude);
        }
    }
}
