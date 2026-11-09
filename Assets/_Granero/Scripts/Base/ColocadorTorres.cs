using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Clic en una plataforma para seleccionarla. 1 = torre lanzadora, 2 = torre maga, X = vender.
public class ColocadorTorres : MonoBehaviour
{
    public GameObject torreLanzadora, torreMaga;
    public int costoLanzadora = 60, costoMaga = 100;
    public float radioSeleccion = 2.6f;
    public SlotTorre[] slots;

    SlotTorre elegido, hover;
    public event Action<SlotTorre> AlCambiarHover;   // la plataforma bajo el mouse (o null)
    Camera cam;

    public SlotTorre Elegido { get { return elegido; } }
    public SlotTorre Hover { get { return hover; } }

    void Start()
    {
        cam = Camera.main;
        if (slots == null || slots.Length == 0) slots = FindObjectsByType<SlotTorre>(FindObjectsSortMode.None);
        Economia.AlCambiar += AlGemas;
        if (Juego.I != null) Juego.I.AlCambiar += RepintarTodo;
        RepintarTodo();
    }

    void OnDestroy()
    {
        Economia.AlCambiar -= AlGemas;
        if (Juego.I != null) Juego.I.AlCambiar -= RepintarTodo;
    }

    void AlGemas(int g) { RepintarTodo(); }

    // Solo cuando cambian las gemas o el juego: son 12 plataformas, sin basura.
    void RepintarTodo()
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++) slots[i].Pintar();
        if (hover != null && AlCambiarHover != null) AlCambiarHover(hover);   // el cartel de hover se actualiza con las gemas
    }

    void Update()
    {
        Mouse m = Mouse.current; Keyboard k = Keyboard.current;
        if (m == null || k == null) return;
        if (Juego.I != null && Juego.I.Terminado) return;

        ActualizarHover(m.position.ReadValue());
        if (m.leftButton.wasPressedThisFrame) Elegir(m.position.ReadValue());

        if (elegido != null)
        {
            if (k.digit1Key.wasPressedThisFrame) Construir(torreLanzadora, costoLanzadora);
            else if (k.digit2Key.wasPressedThisFrame) Construir(torreMaga, costoMaga);
            else if (k.xKey.wasPressedThisFrame) Vender();
            else if (k.uKey.wasPressedThisFrame) Desbloquear();
        }
    }

    // Plataforma mas cercana al punto del suelo bajo el cursor (null si ninguna esta a menos de radioSeleccion).
    SlotTorre Cercana(Vector2 pantalla)
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return null;
        Ray rayo = cam.ScreenPointToRay(pantalla);
        if (Mathf.Abs(rayo.direction.y) < 0.0001f) return null;
        float t = (0.55f - rayo.origin.y) / rayo.direction.y;
        if (t < 0f) return null;
        Vector3 p = rayo.origin + rayo.direction * t;

        SlotTorre mejor = null; float mejorD = radioSeleccion * radioSeleccion;
        for (int i = 0; i < slots.Length; i++)
        {
            Vector3 d = slots[i].Pos - p; d.y = 0f;
            float dd = d.sqrMagnitude;
            if (dd < mejorD) { mejorD = dd; mejor = slots[i]; }
        }
        return mejor;
    }

    void ActualizarHover(Vector2 pantalla)
    {
        SlotTorre h = Cercana(pantalla);
        if (h == hover) return;
        if (hover != null) hover.Hover(false);
        hover = h;
        if (hover != null) hover.Hover(true);
        if (AlCambiarHover != null) AlCambiarHover(hover);
    }

    void Elegir(Vector2 pantalla)
    {
        SlotTorre mejor = Cercana(pantalla);
        if (elegido != null) elegido.Seleccionar(false);
        elegido = mejor;
        if (elegido != null) elegido.Seleccionar(true);
        if (Juego.I != null) Juego.I.NotificarCambio();
    }

    // Para pruebas automaticas: construye en el primer slot libre. Devuelve false si no hay slot o gemas.
    public bool ConstruirAuto(bool maga)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Ocupado || !slots[i].Desbloqueado) continue;
            int costo = maga ? costoMaga : costoLanzadora;
            if (Economia.Gemas < costo) return false;
            elegido = slots[i];
            Construir(maga ? torreMaga : torreLanzadora, costo);
            return true;
        }
        return false;
    }

    void Construir(GameObject prefab, int costo)
    {
        if (elegido == null || elegido.Ocupado || prefab == null) return;
        if (!elegido.Desbloqueado)
        {
            if (Juego.I != null) Juego.I.Mensaje("Plataforma bloqueada: supera la oleada " + elegido.nivelRequerido + " o pulsa U (" + elegido.precioDesbloqueo + " gemas)");
            return;
        }
        if (!Economia.Gastar(costo)) { if (Juego.I != null) Juego.I.Mensaje("No alcanzan las gemas"); return; }

        Vector3 pos = elegido.Pos;
        // la torre mira hacia el camino mas cercano
        Quaternion rot = Quaternion.identity;
        Vector3[] r = Ruta.Puntos;
        if (r != null && r.Length > 0)
        {
            Vector3 cerca = r[0]; float md = 1e9f;
            for (int i = 0; i < r.Length; i++)
            {
                float d = Sensor.DistanciaPlana(pos, r[i]);
                if (d < md) { md = d; cerca = r[i]; }
            }
            Vector3 dir = cerca - pos; dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f) rot = Quaternion.LookRotation(dir);
        }

        GameObject g = Reserva.Obtener(prefab, pos, rot);
        elegido.torre = g.GetComponent<Agente>();
        elegido.costoPagado = costo;
        elegido.Pintar();
        if (Juego.I != null) Juego.I.NotificarCambio();
    }

    void Desbloquear()
    {
        if (elegido == null || elegido.Desbloqueado) return;
        if (elegido.Comprar()) { if (Juego.I != null) Juego.I.Mensaje("Plataforma desbloqueada"); }
        else if (Juego.I != null) Juego.I.Mensaje("Faltan gemas: cuesta " + elegido.precioDesbloqueo);
    }

    void Vender()
    {
        if (elegido == null || !elegido.Ocupado) return;
        Economia.Agregar(elegido.costoPagado / 2);
        Reserva.Devolver(elegido.torre.gameObject);
        elegido.torre = null;
        elegido.Pintar();
        if (Juego.I != null) Juego.I.NotificarCambio();
    }
}
