using System.Collections.Generic;

// ============================================================
//  Contexto.cs  -  Datos compartidos entre los estados
// ============================================================
public class Contexto
{
    readonly Dictionary<string, object> datos = new Dictionary<string, object>();

    public void Fijar<T>(string clave, T valor)
    {
        datos[clave] = valor;
    }

    // Devuelve el dato, o 'porDefecto' si no existe o es de otro tipo.
    public T Obtener<T>(string clave, T porDefecto = default(T))
    {
        object v;
        if (datos.TryGetValue(clave, out v) && v is T) return (T)v;
        return porDefecto;
    }

    // Para listas y similares: si no existe, la crea y la guarda.
    public T ObtenerOCrear<T>(string clave) where T : new()
    {
        object v;
        if (datos.TryGetValue(clave, out v) && v is T) return (T)v;
        T nuevo = new T();
        datos[clave] = nuevo;
        return nuevo;
    }

    public bool Tiene(string clave) { return datos.ContainsKey(clave); }

    public IEnumerable<KeyValuePair<string, object>> Todos { get { return datos; } }

    public void Limpiar() { datos.Clear(); }
}
