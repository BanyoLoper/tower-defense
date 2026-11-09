using UnityEngine;

// Escena de revision: recorre en bucle todas las animaciones de cada agente y muestra su nombre.
// Los rotulos son TextMesh creados una sola vez; el texto solo se cambia cuando cambia la animacion
// (sin OnGUI ni concatenar cadenas en cada frame, para no generar basura).
public class Galeria : MonoBehaviour
{
    public Agente[] agentes;
    int[] indice;
    float[] reloj;
    TextMesh[] rotulos;
    Transform camara;

    void Start()
    {
        camara = Camera.main != null ? Camera.main.transform : null;
        Font fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        indice = new int[agentes.Length];
        reloj = new float[agentes.Length];
        rotulos = new TextMesh[agentes.Length];
        for (int i = 0; i < agentes.Length; i++)
        {
            GameObject go = new GameObject("Rotulo");
            go.transform.SetParent(agentes[i].transform, false);
            go.transform.localPosition = new Vector3(0f, 2.7f, 0f);
            TextMesh tm = go.AddComponent<TextMesh>();
            tm.font = fuente;
            tm.fontSize = 48;
            tm.characterSize = 0.05f;
            tm.anchor = TextAnchor.LowerCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
            go.GetComponent<MeshRenderer>().sharedMaterial = fuente.material;
            rotulos[i] = tm;
            Siguiente(i);
        }
    }

    void Siguiente(int i)
    {
        Agente a = agentes[i];
        if (a == null || a.estadosAnimacion.Length == 0) return;
        indice[i] = (indice[i] + 1) % a.estadosAnimacion.Length;
        string e = a.estadosAnimacion[indice[i]];
        a.Animar(e, 0.1f, true);
        reloj[i] = Mathf.Clamp(a.Duracion(e), 1f, 4f);
        rotulos[i].text = a.rol.ToString() + "\n" + e;
    }

    void Update()
    {
        for (int i = 0; i < agentes.Length; i++)
        {
            reloj[i] -= Time.deltaTime;
            if (reloj[i] <= 0f) Siguiente(i);
        }
    }

    void LateUpdate()
    {
        if (camara == null) return;
        for (int i = 0; i < rotulos.Length; i++)
            rotulos[i].transform.rotation = camara.rotation;
    }
}
