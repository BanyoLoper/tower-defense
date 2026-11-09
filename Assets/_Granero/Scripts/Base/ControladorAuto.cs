using System;
using UnityEngine;

// Va en cada prefab de agente. Busca la clase del alumno "<Rol>Controller" (el Jefe usa BrutoController).
// Puede estar dentro de un namespace (Alumnos.<Maquina>): se busca por nombre corto con BuscadorTipos.
//   - Si existe: la agrega al objeto y NO usa el comportamiento demo.
//   - Si no existe: usa CerebroDemo para que el juego sea jugable desde el principio.
public class ControladorAuto : MonoBehaviour
{
    void Awake()
    {
        Agente a = GetComponent<Agente>();
        string nombre = a.rol.ToString() + "Controller";
        Type t = BuscadorTipos.Buscar(nombre);
        if (t == null && a.rol == Rol.Jefe) t = BuscadorTipos.Buscar("BrutoController");   // el Jefe puede reutilizar al Bruto
        if (t != null && typeof(MonoBehaviour).IsAssignableFrom(t))
        {
            if (GetComponent(t) == null) gameObject.AddComponent(t);
        }
        else if (a.rol != Rol.Granero)
        {
            gameObject.AddComponent<CerebroDemo>();
        }
    }
}
