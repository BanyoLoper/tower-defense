using UnityEngine;
using TMPro;

// HUD por eventos: los textos se reconstruyen SOLO cuando algo cambia (sin basura por frame).
public class Hud : MonoBehaviour
{
    public TMP_Text txtGemas, txtGranero, txtOleada, txtMensaje, txtAyuda, txtHover;
    public RectTransform panelHover;       // cartel que sigue al mouse sobre una plataforma
    Juego juego;
    int ultimaVida = -1;

    void Start()
    {
        juego = Juego.I;
        Economia.AlCambiar += AlGemas;
        juego.AlCambiar += Refrescar;
        juego.granero.salud.AlDanar += AlVida;
        juego.granero.salud.AlCurar += AlCurar;
        ColocadorTorres c = FindColocador();
        if (c != null) c.AlCambiarHover += AlHover;
        if (panelHover != null) panelHover.gameObject.SetActive(false);
        AlGemas(Economia.Gemas);
        Vida();
        Refrescar();
    }

    void OnDestroy()
    {
        if (colocador != null) colocador.AlCambiarHover -= AlHover;
        Economia.AlCambiar -= AlGemas;
        if (juego != null)
        {
            juego.AlCambiar -= Refrescar;
            juego.granero.salud.AlDanar -= AlVida;
            juego.granero.salud.AlCurar -= AlCurar;
        }
    }

    void AlGemas(int g) { if (txtGemas != null) txtGemas.text = "Gemas: " + g; }
    void AlVida(float c, GameObject o) { Vida(); }
    void AlCurar(float c) { Vida(); }

    void Vida()
    {
        int v = Mathf.CeilToInt(juego.granero.salud.vida);
        if (v == ultimaVida) return;
        ultimaVida = v;
        txtGranero.text = "Cristal del granero: " + v + " / " + Mathf.CeilToInt(juego.granero.salud.vidaMax);
    }

    void Refrescar()
    {
        int nivel = Mathf.Min(juego.Nivel + 1, Juego.TotalNiveles);
        txtOleada.text = "Oleada " + nivel + " / " + Juego.TotalNiveles + "   Enemigos: " + juego.EnemigosVivos;
        txtMensaje.text = juego.Aviso == null ? "" : juego.Aviso;

        ColocadorTorres c = FindColocador();
        string ayuda = "WASD: camara   Clic: elegir plataforma   1: torre lanzadora   2: torre maga   X: vender   U: desbloquear   ESPACIO: oleada   R: reiniciar";
        if (c != null && c.Elegido != null)
        {
            SlotTorre e = c.Elegido;
            if (!e.Desbloqueado) ayuda = "Plataforma BLOQUEADA.  Se abre al superar la oleada " + e.nivelRequerido + "  o con  U: desbloquear (" + e.precioDesbloqueo + " gemas)";
            else ayuda = "Plataforma elegida.  1: lanzadora (" + c.costoLanzadora + ")   2: maga (" + c.costoMaga + ")   X: vender";
        }
        txtAyuda.text = ayuda;
    }

    SlotTorre hoverActual;
    void AlHover(SlotTorre s)
    {
        hoverActual = s;
        if (panelHover == null) return;
        panelHover.gameObject.SetActive(s != null);
        if (s == null) return;
        ColocadorTorres c = FindColocador();
        string t;
        switch (s.Estado)
        {
            case EstadoSlot.Bloqueado: t = "BLOQUEADA\nse abre en la oleada " + s.nivelRequerido + " o con U (" + s.precioDesbloqueo + " gemas)"; break;
            case EstadoSlot.Comprable: t = "BLOQUEADA\npulsa U: desbloquear por " + s.precioDesbloqueo + " gemas"; break;
            case EstadoSlot.Ocupado: t = "TORRE LISTA\nX: vender"; break;
            default: t = "PLATAFORMA LIBRE\n1: lanzadora (" + (c != null ? c.costoLanzadora : 0) + ")   2: maga (" + (c != null ? c.costoMaga : 0) + ")"; break;
        }
        txtHover.text = t;
    }

    // el cartel sigue al mouse (sin basura: solo mueve un RectTransform)
    void LateUpdate()
    {
        if (hoverActual == null || panelHover == null) return;
        UnityEngine.InputSystem.Mouse m = UnityEngine.InputSystem.Mouse.current;
        if (m == null) return;
        Vector2 p = m.position.ReadValue();
        panelHover.position = new Vector3(p.x + 24f, p.y - 24f, 0f);
    }

    ColocadorTorres colocador;
    ColocadorTorres FindColocador()
    {
        if (colocador == null) colocador = FindFirstObjectByType<ColocadorTorres>();
        return colocador;
    }
}
