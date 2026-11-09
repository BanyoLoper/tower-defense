using UnityEngine;

// COMPORTAMIENTO DE REEMPLAZO ("demo"). Hace que cada rol se pueda jugar desde el dia uno.
// Cuando existe la clase <Rol>Controller de un alumno, ControladorAuto NO crea este componente.
// Es codigo plano a proposito (sin maquinas de estados) y sin basura: sirve de referencia de rendimiento.
public class CerebroDemo : MonoBehaviour
{
    Agente ag;
    Cuerpo cu;
    Salud sa;
    bool iniciado, muerto, alterna;
    float tMuerte;

    // parametros por rol
    float vel, alcance, vista, persecucion, cadencia, dano, tGolpe;
    bool aDistancia;
    string animAtaque1, animAtaque2, animIdle, animCombate;

    // estado de trabajo
    Agente objetivo;
    float tBusca, cd, golpeEn, animFin;
    int idxRuta, fase;
    float tFase, espera;
    Vector3 desvio, casa, meta;
    bool caminando;
    int carga, idxMina;

    void Awake()
    {
        ag = GetComponent<Agente>();
        cu = GetComponent<Cuerpo>();
        sa = GetComponent<Salud>();
        sa.AlMorir += AlMorir;
    }

    void Start() { Reiniciar(); iniciado = true; }
    void OnEnable() { if (iniciado) Reiniciar(); }

    void Reiniciar()
    {
        muerto = false; objetivo = null; cd = 0f; golpeEn = 0f; animFin = 0f; idxRuta = 0; fase = 0; carga = 0;
        tBusca = Random.value * 0.3f; caminando = false; espera = Random.value * 2f; tFase = 0f;
        casa = transform.position;
        desvio = new Vector3(Random.Range(-1.3f, 1.3f), 0f, Random.Range(-1.3f, 1.3f));
        animIdle = Anim.Idle; animCombate = Anim.IdleCombate;
        animAtaque1 = Anim.Atacar1; animAtaque2 = Anim.Atacar2;
        aDistancia = false; tGolpe = 0.4f; persecucion = 5f;

        switch (ag.rol)
        {
            case Rol.Soldado: vel = 2.2f; alcance = 1.6f; vista = 6f; cadencia = 1.0f; dano = 8f; break;
            case Rol.Bruto: vel = 1.4f; alcance = 2.2f; vista = 6f; cadencia = 2.0f; dano = 28f; tGolpe = 0.7f; break;
            case Rol.Jefe: vel = 1.2f; alcance = 2.8f; vista = 7f; cadencia = 2.4f; dano = 70f; tGolpe = 0.8f; animAtaque2 = Anim.Atacar3; break;
            case Rol.Lancero: vel = 2.0f; alcance = 2.8f; vista = 6f; cadencia = 1.3f; dano = 11f; break;
            case Rol.Ladron: vel = 3.4f; alcance = 1.2f; vista = 3f; cadencia = 1f; dano = 3f; break;
            case Rol.MagoEnemigo:
                vel = 1.6f; alcance = 9f; vista = 11f; persecucion = 11f; cadencia = 2.4f; dano = 16f; aDistancia = true; tGolpe = 0.6f;
                animIdle = Anim.HechizoIdle; animCombate = Anim.HechizoIdle; animAtaque1 = Anim.HechizoLanzar; animAtaque2 = Anim.HechizoLanzar; break;
            case Rol.TorreLanzadora:
                vel = 0f; alcance = 14f; vista = 14f; cadencia = 1.3f; dano = 20f; aDistancia = true; tGolpe = 0.55f;
                animAtaque1 = Anim.Lanzar1; animAtaque2 = Anim.Lanzar2; break;
            case Rol.TorreMaga:
                vel = 0f; alcance = 12f; vista = 12f; cadencia = 2.2f; dano = 26f; aDistancia = true; tGolpe = 0.6f;
                animIdle = Anim.HechizoIdle; animCombate = Anim.HechizoIdle; animAtaque1 = Anim.HechizoLanzar; animAtaque2 = Anim.HechizoLanzar; break;
            case Rol.Guardia: vel = 2.8f; alcance = 1.7f; vista = 10f; cadencia = 0.9f; dano = 12f; break;
            case Rol.Aldeano: vel = 1.6f; vista = 7f; break;
            case Rol.Minero: vel = 1.8f; vista = 7f; fase = 0; idxMina = Random.Range(0, 100); break;
            case Rol.Reparador: vel = 2.0f; vista = 7f; break;
            default: vel = 0f; break;
        }
        cu.velocidadMax = vel > 0f ? vel : 1f;
        cu.Detener();
    }

    void AlMorir(GameObject origen)
    {
        muerto = true;
        tMuerte = 3f;
        if (carga > 0) { Economia.Agregar(carga); carga = 0; }
        cu.Detener();
        string m = (Random.value < 0.5f && ag.TieneAnimacion(Anim.Muerte2)) ? Anim.Muerte2 : Anim.Muerte1;
        ag.Animar(m, 0.1f, true);
    }

    void Update()
    {
        if (!iniciado || ag.rol == Rol.Granero) return;
        float dt = Time.deltaTime;

        if (muerto)
        {
            tMuerte -= dt;
            if (tMuerte <= 0f) { muerto = false; Reserva.Devolver(gameObject); }
            return;
        }

        if (Juego.I != null && Juego.I.Terminado) { Fin(); return; }

        tBusca -= dt; cd -= dt;
        if (golpeEn > 0f) { golpeEn -= dt; if (golpeEn <= 0f) AplicarGolpe(); }

        switch (ag.rol)
        {
            case Rol.Soldado: case Rol.Bruto: case Rol.Jefe: case Rol.Lancero: case Rol.MagoEnemigo: Enemigo(dt); break;
            case Rol.Ladron: Ladron(dt); break;
            case Rol.TorreLanzadora: case Rol.TorreMaga: Torre(dt); break;
            case Rol.Guardia: Guardia(dt); break;
            case Rol.Aldeano: case Rol.Minero: case Rol.Reparador: Civil(dt); break;
        }
    }

    // Fin de la partida: los ganadores festejan, los demas lloran. Los alumnos pueden hacer lo mismo
    // suscribiendose a Juego.I.AlCambiarFase y disparando un trigger a su maquina ("victoria" / "derrota").
    void Fin()
    {
        cu.Detener();
        bool aliado = ag.bando == Bando.Defensor || ag.bando == Bando.Aldeano;
        bool victoria = Juego.I.Fase == FaseJuego.Victoria;
        bool par = (GetInstanceID() & 1) == 0;
        string a;
        if (victoria == aliado) a = par ? Anim.Celebrar1 : Anim.Celebrar2;           // el bando ganador festeja
        else a = ag.TieneAnimacion(Anim.Llorar) ? Anim.Llorar : Anim.Miedo;            // el perdedor llora
        if (!ag.TieneAnimacion(a)) a = animIdle;
        ag.Animar(a);
    }

    // ------------------------------------------------------------------ utilidades
    bool Buscar(float radio)
    {
        if (tBusca > 0f) return objetivo != null && objetivo.Vivo;
        tBusca = 0.2f + Random.value * 0.1f;
        objetivo = Sensor.HostilMasCercano(ag, radio);
        return objetivo != null;
    }

    float Dist(Agente o)
    {
        return o.DistanciaBorde(transform.position) - ag.radio;
    }

    void Ir(Vector3 destino, float v)
    {
        Vector3 d = destino - transform.position; d.y = 0f;
        if (d.sqrMagnitude < 0.01f) { cu.Detener(); return; }
        cu.velocidadMax = v;
        cu.Mover(d.normalized * v, Time.deltaTime);
        if (v > 3f && ag.TieneAnimacion(Anim.Sprint)) ag.Animar(Anim.Sprint);
        else ag.Animar(v > 2.5f ? Anim.Correr : Anim.Caminar);
    }

    void Esperar(float dt, bool combate)
    {
        if (vel > 0f) cu.Mover(Vector3.zero, dt);
        if (animFin > 0f) { animFin -= dt; return; }
        ag.Animar(combate ? animCombate : animIdle);
    }

    void Atacar(float dt)
    {
        cu.MirarA(objetivo.Pos, dt * 3f);
        if (cd <= 0f && golpeEn <= 0f)
        {
            cd = cadencia;
            golpeEn = tGolpe;
            alterna = !alterna;
            string a = alterna ? animAtaque2 : animAtaque1;
            ag.Animar(a, 0.08f, true);
            animFin = Mathf.Clamp(ag.Duracion(a), 0.5f, 1.2f);
        }
        Esperar(dt, true);
    }

    void AplicarGolpe()
    {
        if (objetivo == null || !objetivo.Vivo) return;
        if (aDistancia)
        {
            Juego j = Juego.I;
            if (j == null) return;
            bool bola = ag.rol == Rol.TorreMaga || ag.rol == Rol.MagoEnemigo;
            GameObject pf = bola ? j.proyectilBola : j.proyectilLanza;
            if (pf == null) return;
            Vector3 origen = ag.PuntoPecho + transform.forward * 0.5f;
            GameObject g = Reserva.Obtener(pf, origen, transform.rotation);
            ProyectilBase p = g.GetComponent<ProyectilBase>();
            p.Lanzar(ag, objetivo, ag.bando, dano);
        }
        else if (Dist(objetivo) <= alcance + 0.9f)
        {
            objetivo.salud.Danar(dano, gameObject);
        }
    }

    // ------------------------------------------------------------------ enemigos
    void Enemigo(float dt)
    {
        if (Buscar(vista))
        {
            float d = Dist(objetivo);
            if (d <= alcance) { Atacar(dt); return; }
            if (d <= persecucion) { Ir(objetivo.Pos, vel); return; }
        }
        SeguirRuta();
    }

    void SeguirRuta()
    {
        Vector3[] r = Ruta.Puntos;
        if (r == null || r.Length == 0) { Esperar(Time.deltaTime, false); return; }
        if (idxRuta >= r.Length) { Ir(Ruta.Granero, vel); return; }
        Vector3 t = r[idxRuta] + desvio;
        if (Sensor.DistanciaPlana(transform.position, t) < 1.3f) idxRuta++;
        Ir(t, vel);
    }

    void Ladron(float dt)
    {
        Vector3[] r = Ruta.Puntos;
        Juego j = Juego.I;
        if (fase == 0)
        {
            float d = j != null ? j.granero.DistanciaBorde(transform.position) : 4f;
            if (d < 1.2f) { fase = 1; tFase = 2.6f; cu.Detener(); ag.Animar(Anim.RobarInicio, 0.1f, true); return; }
            SeguirRuta();
        }
        else if (fase == 1)
        {
            tFase -= dt;
            if (tFase < 2.0f) ag.Animar(Anim.RobarLoop);
            if (tFase <= 0f)
            {
                carga = Economia.Robar(25);
                fase = 2; idxRuta = r.Length - 1;
                if (j != null) j.Mensaje("Un ladron robo " + carga + " gemas. Matalo para recuperarlas");
            }
        }
        else
        {
            Vector3 t = r[Mathf.Max(idxRuta, 0)] + desvio;
            Ir(t, vel * 1.25f);
            if (Sensor.DistanciaPlana(transform.position, t) < 1.3f) idxRuta--;
            if (idxRuta < 0) { carga = 0; Reserva.Devolver(gameObject); }
        }
    }

    // ------------------------------------------------------------------ defensores
    void Torre(float dt)
    {
        if (Buscar(vista) && Dist(objetivo) <= alcance) { Atacar(dt); return; }
        Esperar(dt, false);
    }

    void Guardia(float dt)
    {
        if (Buscar(vista) && Sensor.DistanciaPlana(objetivo.Pos, casa) < 16f)
        {
            if (Dist(objetivo) <= alcance) { Atacar(dt); return; }
            Ir(objetivo.Pos, vel);
            return;
        }
        if (Sensor.DistanciaPlana(transform.position, casa) > 1.5f) Ir(casa, vel * 0.7f);
        else Esperar(dt, false);
    }

    // ------------------------------------------------------------------ civiles
    void Civil(float dt)
    {
        if (Buscar(vista))
        {
            Vector3 lejos = transform.position - objetivo.Pos; lejos.y = 0f;
            Vector3 hacia = Ruta.Granero - transform.position; hacia.y = 0f;
            Vector3 dir = (lejos.normalized + hacia.normalized * 0.4f).normalized;
            Ir(transform.position + dir * 4f, vel * 1.6f);
            caminando = false; fase = 0;
            return;
        }
        switch (ag.rol)
        {
            case Rol.Minero: Minero(dt); break;
            case Rol.Reparador: Reparador(dt); break;
            default: Vagar(dt); break;
        }
    }

    void Vagar(float dt)
    {
        if (caminando)
        {
            if (Sensor.DistanciaPlana(transform.position, meta) < 0.6f) { caminando = false; espera = 1.5f + Random.value * 3f; }
            else { Ir(meta, vel); return; }
        }
        espera -= dt;
        Esperar(dt, false);
        if (espera <= 0f)
        {
            Vector2 c = Random.insideUnitCircle * 6f;
            meta = casa + new Vector3(c.x, 0f, c.y);
            caminando = true;
        }
    }

    void Minero(float dt)
    {
        Juego j = Juego.I;
        if (j == null || j.puntosMina == null || j.puntosMina.Length == 0) { Vagar(dt); return; }
        Transform mina = j.puntosMina[idxMina % j.puntosMina.Length];
        if (fase == 0)
        {
            if (Sensor.DistanciaPlana(transform.position, mina.position) < 1.0f) { fase = 1; tFase = 5f; cu.Detener(); cu.MirarA(mina.position, 1f); ag.Animar(Anim.MinarSuelo, 0.2f, true); return; }
            Ir(mina.position, vel);
        }
        else
        {
            tFase -= dt;
            if (tFase <= 0f) { Economia.Agregar(6); fase = 0; idxMina++; }
        }
    }

    void Reparador(float dt)
    {
        Juego j = Juego.I;
        if (j == null || j.granero.salud.vida >= j.granero.salud.vidaMax - 0.5f || j.puntoReparacion == null)
        {
            if (fase != 0) { fase = 0; }
            Vagar(dt);
            return;
        }
        Vector3 p = j.puntoReparacion.position;
        if (fase == 0)
        {
            if (Sensor.DistanciaPlana(transform.position, p) < 1.2f) { fase = 1; tFase = 0.6f; cu.Detener(); ag.Animar(Anim.MartillarInicio, 0.15f, true); return; }
            Ir(p, vel);
        }
        else
        {
            tFase -= dt;
            if (tFase <= 0f) ag.Animar(Anim.MartillarLoop);
            j.granero.salud.Curar(4f * dt);
        }
    }
}
