using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraDisparando : Estado
    {
        private Agente torre, objetivo;
        private string animacion;
        private float tiempo, duracion, dano;
        private bool disparo, alternar;
        private GameObject lanza;

        public override void Entrar(string anterior)
        {
            tiempo = 0f; 
            lanza = Juego.I.proyectilLanza;
            dano = lanza.GetComponent<ProyectilBase>().dano;
            torre = Ctx.Obtener<Agente>("agente"); 
            objetivo = Ctx.Obtener<Agente>("objetivo");

            if(alternar) animacion = Anim.Lanzar2;
            else animacion = Anim.Lanzar1;
            alternar = !alternar;

            torre.Animar(animacion);
            duracion = torre.Duracion(animacion);
            if (duracion <= 0f) duracion = 0.8f;
            disparo = false;

        }
        public override string Actualizar(float dt)
        {
            tiempo += dt;

            if (!disparo && tiempo >= duracion / 2f)
            {
                disparo = true;
                Lanzar();
            }

            if (tiempo >= duracion) return "TorreLanzadoraRecargando";

            return null;

        }
        void Lanzar()
        {
            if (objetivo == null || !objetivo.Vivo) return;

            GameObject objetoLanza = Reserva.Obtener(lanza, torre.PuntoPecho, torre.transform.rotation);
            ProyectilBase proyectil = objetoLanza.GetComponent<ProyectilBase>();
            proyectil.Lanzar(torre, objetivo, torre.bando, dano);
        }
    }
}