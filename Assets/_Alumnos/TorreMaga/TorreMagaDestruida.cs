using UnityEngine;

namespace Alumnos.TorreMaga
{
    public class TorreMagaDestruida : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("TorreMaga: entre a Destruida");
        }
        public override string Actualizar(float dt)
        {
            return null;
        }
        public override string AlRecibir(string trigger, int valor)
        {
            return null;
        }
    }
}
