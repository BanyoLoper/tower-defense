using UnityEngine;

namespace Alumnos.Guardia
{
    public class GuardiaMuerto : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Muerto");
            Agente agente = Ctx.Obtener<Agente>("agente");

            if (agente != null)
            {
                agente.Animar("Muerte1");
            }
        }
        public override string Actualizar(float dt) { return null; }
        public override string AlRecibir(string trigger, int valor) {  return null; }
    }
}