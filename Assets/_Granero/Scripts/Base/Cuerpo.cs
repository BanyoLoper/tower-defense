using UnityEngine;
using UnityEngine.AI;

// Motor de movimiento SIN NavMeshAgent. Los steering behaviours calculan una fuerza/velocidad
// deseada y la mandan aqui con Mover(). El cuerpo la limita, la suaviza, gira al personaje y
// lo mantiene sobre el NavMesh (NavMesh.Raycast + SamplePosition).
//
//     cuerpo.Mover(velocidadDeseada, dt);   // desde Actualizar() de un estado o del controller
//     cuerpo.Detener();
public class Cuerpo : MonoBehaviour
{
    public float velocidadMax = 3f;
    public float aceleracion = 12f;
    public float giroGradosPorSeg = 540f;
    public bool pegarANavMesh = true;
    public float alturaSuelo = 0f;

    public Vector3 Velocidad { get; private set; }
    public float VelocidadActual { get { return Velocidad.magnitude; } }

    void Reset() { }

    // 'deseada' = velocidad que quiero (m/s, plano XZ). Se limita a velocidadMax.
    public void Mover(Vector3 deseada, float dt)
    {
        deseada.y = 0f;
        if (deseada.magnitude > velocidadMax) deseada = deseada.normalized * velocidadMax;

        Velocidad = Vector3.MoveTowards(Velocidad, deseada, aceleracion * dt);

        Vector3 destino = transform.position + Velocidad * dt;

        if (pegarANavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.Raycast(transform.position, destino, out hit, NavMesh.AllAreas))
            {
                // choco con un borde: nos quedamos en el borde y matamos esa componente
                destino = hit.position;
                Velocidad = Vector3.zero;
            }
            if (NavMesh.SamplePosition(destino, out hit, 1.5f, NavMesh.AllAreas)) destino.y = hit.position.y + alturaSuelo;
        }

        transform.position = destino;

        Vector3 plano = Velocidad; plano.y = 0f;
        if (plano.sqrMagnitude > 0.04f) Mirar(plano, dt);
    }

    public void Detener() { Velocidad = Vector3.zero; }

    public void Mirar(Vector3 direccion, float dt)
    {
        direccion.y = 0f;
        if (direccion.sqrMagnitude < 0.0001f) return;
        Quaternion q = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, giroGradosPorSeg * dt);
    }

    public void MirarA(Vector3 punto, float dt)
    {
        Mirar(punto - transform.position, dt);
    }

    // Coloca al personaje en un punto del NavMesh (para spawns).
    public void Colocar(Vector3 punto)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(punto, out hit, 3f, NavMesh.AllAreas)) punto = hit.position;
        transform.position = punto;
        Velocidad = Vector3.zero;
    }
}
