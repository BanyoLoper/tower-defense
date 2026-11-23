using UnityEngine;

namespace Alumnos.Director
{
    public class DirectorBono : Estado
    {
        public override void Entrar(string anterior)
        {
            // Entregar las gemas al jugador.
            Economia.Agregar(Juego.I.bonoPorOleada);
            Debug.Log("Director: bono entregado");
            Debug.Log("Gemas actuales: " + Economia.Gemas);
            // Preparar el siguiente nivel.
            int nuevoNivel = Juego.I.Nivel + 1;
            Juego.I.Preparar(nuevoNivel);
            Debug.Log("Director: siguiente nivel " + (nuevoNivel + 1));
        }

        public override string Actualizar(float dt)
        {
            if (Juego.I.Nivel >= Juego.TotalNiveles)
            {
                return "DirectorVictoria";
            }
            return "DirectorPreparacion";
        }
    }
}