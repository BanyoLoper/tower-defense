using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorJefe : Estado
    {
        List<Rol> cola = new List<Rol>();
        public int indice;
        public float tiempo;
        public float intervalo;
        public bool jefeAparecio;
        public bool jefeMurio;

        public override void Entrar(string anterior)
        {
            indice = 0;
            tiempo = 1f;
            jefeAparecio = false;
            jefeMurio = false;
            cola.Clear();
            // Escoltas del jefe.
            AgregarEnemigos(Rol.Soldado, 12);
            AgregarEnemigos(Rol.Ladron, 4);
            AgregarEnemigos(Rol.Bruto, 4);
            intervalo = 0.8f;
            // Escuchar cuando muere un agente.
            Agente.AlMorirAlguien -= AlMorir;
            Agente.AlMorirAlguien += AlMorir;
            Debug.Log("Director: comienza la oleada del jefe");
            // Avisar que comienza la quinta oleada.
            Juego.I.Lanzar(Juego.I.Nivel);
        }

        public void AgregarEnemigos(Rol rol, int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                cola.Add(rol);
            }
        }

        public void AlMorir(Agente agente)
        {
            if (agente.rol == Rol.Jefe && agente.bando == Bando.Enemigo)
            {
                jefeMurio = true;
                // Ya no necesitamos escuchar más muertes.
                Agente.AlMorirAlguien -= AlMorir;
                Debug.Log("Director: el jefe fue derrotado");
            }
        }

        public override string Actualizar(float dt)
        {
            // Primero aparecen las escoltas.
            if (indice < cola.Count)
            {
                tiempo -= dt;
                if (tiempo <= 0f)
                {
                    tiempo = intervalo;
                    Vector3 posicion = Ruta.Inicio;

                    posicion += new Vector3( Random.Range(-0.8f, 0.8f),0f,Random.Range(-0.8f, 0.8f));
                    Juego.I.Aparecer(cola[indice],posicion,Quaternion.LookRotation(Vector3.right));
                    indice++;
                }
                return null;
            }

            // Después aparece el jefe.
            if (!jefeAparecio)
            {
                tiempo -= dt;

                if (tiempo <= 0f)
                {
                    Vector3 posicion = Ruta.Inicio;
                    posicion += new Vector3(Random.Range(-0.8f, 0.8f),0f, Random.Range(-0.8f, 0.8f));
                    Juego.I.Aparecer( Rol.Jefe,posicion,Quaternion.LookRotation(Vector3.right));
                    jefeAparecio = true;
                    Debug.Log("Director: ¡el JEFE ha aparecido!");
                }
                return null;
            }

            // Esperar a que el jefe muera.
            if (jefeMurio)
            {
                return "DirectorLimpieza";
            }

            return null;
        }
    }
}