using UnityEngine;

namespace Alumnos.Guardia
{
    public class GuardiaMuerto : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Muerto");
        }
    }
}