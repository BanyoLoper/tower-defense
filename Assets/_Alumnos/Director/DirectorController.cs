using System.Collections.Generic;
using UnityEngine;

public class DirectorController : MonoBehaviour
{
    public Maquina maquina;
    public Contexto ctx;
    private string ultimo;

    void Start()
    {
        ctx = new Contexto();
        List<Estado> lista = new List<Estado>();

        lista.Add(new DirectorPreparacion());


        maquina = new Maquina(lista, ctx);
<<<<<<< HEAD
        maquina.Iniciar("Preparacion");
=======
>>>>>>> parent of bc952d9 (Revert "Cambios v.1")
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
