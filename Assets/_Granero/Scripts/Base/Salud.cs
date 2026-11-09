using System;
using UnityEngine;

// Sistema de vida. Lo usan TODOS los agentes (y el granero).
// Los alumnos se suscriben a los eventos desde su controller:
//     salud.AlDanar += (cantidad, origen) => maquina.Disparar("dano", (int)cantidad);
//     salud.AlMorir += (origen) => maquina.Disparar("muerte");
public class Salud : MonoBehaviour
{
    public float vidaMax = 100f;
    public float vida = 100f;
    public bool invulnerable;

    public bool EstaMuerto { get; private set; }
    public float Porcentaje { get { return vidaMax <= 0f ? 0f : Mathf.Clamp01(vida / vidaMax); } }

    public event Action<float, GameObject> AlDanar;   // cantidad, quien lo hizo (puede ser null)
    public event Action<float> AlCurar;               // cantidad curada
    public event Action<GameObject> AlMorir;          // quien dio el golpe final (puede ser null)

    void Awake()
    {
        vida = vidaMax;
    }

    public void Danar(float cantidad, GameObject origen = null)
    {
        if (EstaMuerto || invulnerable || cantidad <= 0f) return;

        vida = Mathf.Max(0f, vida - cantidad);
        if (AlDanar != null) AlDanar(cantidad, origen);

        if (vida <= 0f)
        {
            EstaMuerto = true;
            if (AlMorir != null) AlMorir(origen);
        }
    }

    public void Curar(float cantidad)
    {
        if (EstaMuerto || cantidad <= 0f) return;
        float antes = vida;
        vida = Mathf.Min(vidaMax, vida + cantidad);
        if (AlCurar != null && vida > antes) AlCurar(vida - antes);
    }

    public void Revivir()
    {
        EstaMuerto = false;
        vida = vidaMax;
    }

    // Para objetos reutilizados desde la Reserva.
    public void Reiniciar()
    {
        Revivir();
    }
}
