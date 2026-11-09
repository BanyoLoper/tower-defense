using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Busca un controller por su NOMBRE CORTO (ej. "SoldadoController") aunque el alumno lo haya puesto dentro de
// un namespace (ej. Alumnos.Soldado). Solo mira el ensamblado del juego y se arma una vez (no cada frame).
public static class BuscadorTipos
{
    static Dictionary<string, Type> cache;

    public static Type Buscar(string nombreCorto)
    {
        if (cache == null) Construir();
        Type t;
        cache.TryGetValue(nombreCorto, out t);
        return t;
    }

    static void Construir()
    {
        cache = new Dictionary<string, Type>();
        Type[] tipos = typeof(BuscadorTipos).Assembly.GetTypes();
        for (int i = 0; i < tipos.Length; i++)
        {
            Type t = tipos[i];
            if (t.IsAbstract || !typeof(MonoBehaviour).IsAssignableFrom(t) || !t.Name.EndsWith("Controller")) continue;
            Type previo;
            if (cache.TryGetValue(t.Name, out previo))
                Debug.LogWarning("[BuscadorTipos] Clase repetida: " + previo.FullName + " y " + t.FullName + ". Usa nombres unicos con el prefijo del rol.");
            else
                cache[t.Name] = t;
        }
    }
}
