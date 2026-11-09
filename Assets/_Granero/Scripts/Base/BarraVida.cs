using UnityEngine;

// Barra de vida sobre la cabeza (dos quads que miran a la camara). Se crea sola en runtime.
// Materiales compartidos (3 colores) para que se agrupen en batches aunque haya cientos de agentes.
[RequireComponent(typeof(Salud))]
public class BarraVida : MonoBehaviour
{
    public float altura = 2.2f;
    public float ancho = 1.0f;
    public bool ocultarSiLlena = true;

    static Material matFondo, matVerde, matAmarillo, matRojo;
    static Mesh quad;

    Salud salud;
    Transform raiz, relleno;
    MeshRenderer rellenoRend;
    int cubo = -1;
    float ultimoPorcentaje = -1f;
    Transform camara;

    static Material Crear(Color c)
    {
        Shader sh = Shader.Find("Universal Render Pipeline/Unlit");
        if (sh == null) sh = Shader.Find("Sprites/Default");
        Material m = new Material(sh);
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        m.enableInstancing = true;
        return m;
    }

    void Start()
    {
        salud = GetComponent<Salud>();
        if (matFondo == null)
        {
            matFondo = Crear(new Color(0.08f, 0.08f, 0.08f));
            matVerde = Crear(new Color(0.2f, 0.85f, 0.25f));
            matAmarillo = Crear(new Color(0.95f, 0.8f, 0.15f));
            matRojo = Crear(new Color(0.9f, 0.15f, 0.15f));
            GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad = tmp.GetComponent<MeshFilter>().sharedMesh;
            Destroy(tmp);
        }

        raiz = new GameObject("BarraVida").transform;
        raiz.SetParent(transform, false);
        raiz.localPosition = new Vector3(0f, altura, 0f);

        MakeQuad("Fondo", matFondo, 0.02f, ancho + 0.06f, 0.16f);
        relleno = MakeQuad("Relleno", matVerde, 0f, ancho, 0.10f);
        rellenoRend = relleno.GetComponent<MeshRenderer>();
        camara = Camera.main != null ? Camera.main.transform : null;
    }

    Transform MakeQuad(string nombre, Material m, float z, float w, float h)
    {
        GameObject q = new GameObject(nombre);
        q.AddComponent<MeshFilter>().sharedMesh = quad;
        MeshRenderer mr = q.AddComponent<MeshRenderer>();
        mr.sharedMaterial = m;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        q.transform.SetParent(raiz, false);
        q.transform.localPosition = new Vector3(0f, 0f, z);
        q.transform.localScale = new Vector3(w, h, 1f);
        return q.transform;
    }

    void OnEnable() { ultimoPorcentaje = -1f; }

    void LateUpdate()
    {
        if (salud == null || raiz == null) return;
        float p = salud.Porcentaje;
        bool visible = !salud.EstaMuerto && !(ocultarSiLlena && p >= 0.999f);
        if (raiz.gameObject.activeSelf != visible) raiz.gameObject.SetActive(visible);
        if (!visible) return;

        if (camara == null && Camera.main != null) camara = Camera.main.transform;
        if (camara != null) raiz.rotation = camara.rotation;

        if (Mathf.Abs(p - ultimoPorcentaje) > 0.001f)
        {
            ultimoPorcentaje = p;
            float w = Mathf.Max(0.001f, ancho * p);
            relleno.localScale = new Vector3(w, 0.10f, 1f);
            relleno.localPosition = new Vector3(-(ancho - w) * 0.5f, 0f, 0f);
            int c = p > 0.6f ? 0 : (p > 0.3f ? 1 : 2);
            if (c != cubo)
            {
                cubo = c;
                rellenoRend.sharedMaterial = c == 0 ? matVerde : (c == 1 ? matAmarillo : matRojo);
            }
        }
    }
}
