using System.Collections.Generic;
using UnityEngine;

// Marca que indica de que prefab viene un objeto reutilizable.
public class Reservable : MonoBehaviour
{
    [HideInInspector] public GameObject origen;
    [HideInInspector] public float devolverEn = -1f;

    void Update()
    {
        if (devolverEn > 0f)
        {
            devolverEn -= Time.deltaTime;
            if (devolverEn <= 0f) { devolverEn = -1f; Reserva.Devolver(gameObject); }
        }
    }
}

// Pool de objetos. En un tower defense salen cientos de agentes y proyectiles: Instantiate/Destroy
// en masa dispara el GC. Con la reserva se crean una vez y se reutilizan.
//     GameObject g = Reserva.Obtener(prefab, pos, rot);   // lo activa y le hace Reiniciar() a la Salud
//     Reserva.Devolver(g);                                 // lo apaga y lo guarda
public static class Reserva
{
    static readonly Dictionary<GameObject, Stack<GameObject>> libres = new Dictionary<GameObject, Stack<GameObject>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Limpiar() { libres.Clear(); }

    public static GameObject Obtener(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        Stack<GameObject> pila;
        if (!libres.TryGetValue(prefab, out pila)) { pila = new Stack<GameObject>(); libres[prefab] = pila; }

        GameObject go = null;
        while (pila.Count > 0 && go == null) go = pila.Pop();

        bool nuevo = false;
        if (go == null)
        {
            go = (GameObject)Object.Instantiate(prefab, pos, rot);
            Reservable r = go.GetComponent<Reservable>();
            if (r == null) r = go.AddComponent<Reservable>();
            r.origen = prefab;
            nuevo = true;
        }
        else
        {
            go.transform.SetPositionAndRotation(pos, rot);
            go.SetActive(true);
        }

        if (!nuevo)
        {
            Salud s = go.GetComponent<Salud>();
            if (s != null) s.Reiniciar();
        }
        return go;
    }

    public static void Devolver(GameObject go)
    {
        if (go == null) return;
        Reservable r = go.GetComponent<Reservable>();
        if (r == null || r.origen == null) { Object.Destroy(go); return; }
        go.SetActive(false);
        Stack<GameObject> pila;
        if (!libres.TryGetValue(r.origen, out pila)) { pila = new Stack<GameObject>(); libres[r.origen] = pila; }
        pila.Push(go);
    }

    // Lo devuelve a la reserva despues de 'seg' segundos (efectos).
    public static void DevolverEn(GameObject go, float seg)
    {
        Reservable r = go.GetComponent<Reservable>();
        if (r != null) r.devolverEn = seg;
    }
}
