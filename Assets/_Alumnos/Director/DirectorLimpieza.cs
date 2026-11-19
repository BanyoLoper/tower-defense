using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorLimpieza : Estado
    {
        float tiempo;
        public override void Entrar(string anterior)
        {
            tiempo = 0.4f;
            Debug.Log("Director: esperando a que mueran los enemigos");
        }

        public override string Actualizar(float dt)
        {
            tiempo -= dt;
            if (tiempo <= 0f)
            {
                tiempo = 0.4f;
                int enemigos = Sensor.Contar(Bando.Enemigo);
                Juego.I.FijarEnemigosVivos(enemigos);
                Debug.Log("Enemigos restantes: " + enemigos);
                if (enemigos == 0)
                {
                    Debug.Log("Director: todos los enemigos fueron eliminados");

                    return "DirectorBono";
                }
            }
            return null;
        }
    }
}