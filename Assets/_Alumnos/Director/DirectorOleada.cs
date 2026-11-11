
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorOleada : Estado
    {
        List<Rol> cola = new List<Rol>();

        int indice;
        int nivel;
        float tiempo;
        float intervalo;

        public override void Entrar(string anterior)
        {
            indice = 0;
            tiempo = 1f;

            nivel = Juego.I.Nivel;

            cola.Clear();

            // Oleada 1
            if (nivel == 0)
            {
                AgregarEnemigos(Rol.Soldado, 6);
                intervalo = 1.6f;
            }

            // Oleada 2
            else if (nivel == 1)
            {
                AgregarEnemigos(Rol.Soldado, 8);
                AgregarEnemigos(Rol.Lancero, 4);
                intervalo = 1.3f;
            }

            // Oleada 3
            else if (nivel == 2)
            {
                AgregarEnemigos(Rol.Soldado, 8);
                AgregarEnemigos(Rol.Ladron, 5);
                AgregarEnemigos(Rol.MagoEnemigo, 2);
                intervalo = 1.1f;
            }

            // Oleada 4
            else if (nivel == 3)
            {
                AgregarEnemigos(Rol.Soldado, 8);
                AgregarEnemigos(Rol.Lancero, 6);
                AgregarEnemigos(Rol.MagoEnemigo, 3);
                AgregarEnemigos(Rol.Bruto, 3);
                intervalo = 1f;
            }

            // Oleada 5: escoltas.
            // El jefe se incorporara en DirectorJefe.
            else if (nivel == 4)
            {
                AgregarEnemigos(Rol.Soldado, 12);
                AgregarEnemigos(Rol.Ladron, 4);
                AgregarEnemigos(Rol.Bruto, 4);
                intervalo = 0.8f;
            }

            Debug.Log("Director: comienza la oleada " + (nivel + 1));

            Juego.I.Lanzar(nivel);
        }

        void AgregarEnemigos(Rol rol, int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                cola.Add(rol);
            }
        }

        public override string Actualizar(float dt)
        {
            if (indice >= cola.Count)
            {
                return null;
            }

            tiempo -= dt;

            if (tiempo <= 0f)
            {
                tiempo = intervalo;

                Vector3 posicion = Ruta.Inicio;

                posicion += new Vector3(
                    Random.Range(-0.8f, 0.8f),
                    0f,
                    Random.Range(-0.8f, 0.8f)
                );

                Juego.I.Aparecer(
                    cola[indice],
                    posicion,
                    Quaternion.LookRotation(Vector3.right)
                );

                indice++;
            }

            return null;
        }
    }
}
