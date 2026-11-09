using UnityEngine;

// Catalogo de efectos visuales ya listos. Se llaman con una linea:
//     Efectos.Reproducir(Fx.Explosion, posicion);
//     Efectos.Reproducir(Fx.Curacion, transform.position, 1.5f);   // escala 1.5
// Usan la Reserva (pool): no generan basura. Los alumnos NO crean particulas: solo eligen cual llamar.
public enum Fx { Impacto, Explosion, Curacion, Desbloqueo, Rayo, Escudo, Fuego, Hielo, OndaSuelo, Aura }

public class Efectos : MonoBehaviour
{
    public static Efectos I;
    public GameObject[] prefabs = new GameObject[10];   // indexado por (int)Fx; lo llena el constructor de la escena
    public float[] escalas = { 0.6f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };

    void Awake() { I = this; }
    void OnDestroy() { if (I == this) I = null; }

    public static GameObject Reproducir(Fx fx, Vector3 pos, float escala = 1f, float segundos = 2.5f)
    {
        if (I == null) return null;
        int i = (int)fx;
        if (i < 0 || i >= I.prefabs.Length || I.prefabs[i] == null) return null;
        GameObject g = Reserva.Obtener(I.prefabs[i], pos, Quaternion.identity);
        if (g == null) return null;
        g.transform.localScale = Vector3.one * escala * (i < I.escalas.Length ? I.escalas[i] : 1f);
        Reserva.DevolverEn(g, segundos);
        return g;
    }
}
