using UnityEngine;

public class DirectorPreparacion : Estado
{
    public override void Entrar(string anterior)
    {
        Debug.Log("Inicia el juego");
    }
}
