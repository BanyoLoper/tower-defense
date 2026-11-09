# 01 · Soldado enemigo

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Media**  **Enemigo**

El enemigo básico y más numeroso. Sigue la ruta hacia el granero, pelea contra lo que se le cruce y ataca el granero cuando llega. Es la máquina de referencia: si entiendes esta, entiendes todas.

Peso de la máquina: **21.5 pts** · Steering a cargo: **7 pts** (PathFollowing (7 pts)) · Total del cupo 01: **28.5 pts** · Estados: **6** · Transiciones: **10**

## 1. Diagrama de estados

![Diagrama de estados de Soldado enemigo](img/m01_soldado_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main && git pull && git checkout -b clase-1/01-soldado`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: oleadas 1 a 5 (ESPACIO llama cada oleada).*
3. **Crea estos archivos** en `Assets/_Alumnos/Soldado/`: `SoldadoController.cs`, `SoldadoAvanzar.cs`, `SoldadoMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Caminar`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `SoldadoController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `SoldadoAvanzar` | Sigue Ruta.Puntos hacia el granero. Revisa el Sensor cada 0.2 s. | Caminar | PathFollowing |
| `SoldadoPerseguir` | Corre hacia el hostil visto (aldeano, guardia, torre). | Correr | Seek |
| `SoldadoAtacar` | Golpea con cadencia de 1 s; el daño se aplica a mitad de la animación. | Atacar1 / Atacar2 | — |
| `SoldadoAturdido` | Reaccion corta al daño (0.4 s). Vuelve al estado que tenia (se guarda en Ctx). | Dano1 | — |
| `SoldadoFestejo` (global) | Estado global: la partida termino y los enemigos ganaron. | Celebrar1 / Celebrar2 | — |
| `SoldadoMuerto` (global) | Estado global: cae, espera 3 s y se devuelve a la Reserva. | Muerte1 / Muerte2 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Avanzar | Perseguir | hostil a &lt;= 6 m |
| Perseguir | Avanzar | objetivo perdido |
| Perseguir | Atacar | dist &lt;= alcance |
| Atacar | Perseguir | objetivo se aleja |
| Avanzar | Atacar | granero en alcance |
| Atacar | Avanzar | objetivo muerto |
| Avanzar | Aturdido | daño |
| Perseguir | Aturdido | daño |
| Atacar | Aturdido | daño |
| Aturdido | Avanzar | 0.4 s -> anterior |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |
| *cualquiera* | Festejo | fin de partida (`Juego.AlCambiarFase`) |

**Eventos que escucha:** `dano`, `muerte`, `fin_juego`, `hostil_visto (Sensor)`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Soldado enemigo](img/m01_soldado_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `Ruta` | Ruta.Puntos: la ruta del nivel |
| `Juego` | Juego.I.Fase para el festejo |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Director | recibe de | El Director lo crea con Juego.Aparecer(Rol.Soldado,...). Tu máquina no sabe de oleadas. |
| Guardia / Torres | envía a | Es su objetivo: ellos lo detectan con Sensor. |
| Aldeano | envía a | Provoca su huida (Sensor + Alertas). |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.PathFollowing`](02_steering_behaviors.md#path-following-seguir-camino-7-pts) | Cupo 01 (Soldado) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Seek`](02_steering_behaviors.md#seek-buscar-2-pts) | Cupo 04 (Lancero) | Clase 1 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Steering que debes programar tú (cupo 01)

| Behaviour | Pts | Entrega | Lo usan |
| --- | --- | --- | --- |
| [Path Following (seguir camino)](02_steering_behaviors.md#path-following-seguir-camino-7-pts) | 7 | Clase 2 | Soldado, Bruto, Jefe, Lancero, MagoEnemigo, Ladron, Guardia, Minero |

### Cómo probar tu steering aislado

Crea en tu carpeta de máquina un script de prueba (no se usa en el juego final) y ponlo en un objeto con un `Cuerpo` sobre el NavMesh de la escena:

```csharp
using UnityEngine;

namespace Alumnos.Soldado   // el namespace de tu MAQUINA (el probe no es parte de Steering)
{
public class ProbePathFollowing : MonoBehaviour   // nombre unico: Probe + tu steering
{
    public Transform objetivo;
    public float vMax = 3f;
    Cuerpo cuerpo;
    void Awake() { cuerpo = GetComponent<Cuerpo>(); }
    void Update()
    {
        Vector3 v = Steering.PathFollowing(transform.position, objetivo.position, vMax);   // ajusta los parametros a tu firma
        cuerpo.Mover(v, Time.deltaTime);
    }
}
}
```

## 6. Entregas por clase

![Linea de tiempo de entregas](img/m01_soldado_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** | Avanzar, Muerto | 2/6 · 33% | Controller + Maquina + 2 estados. El soldado camina la ruta y muere (animación + Reserva.Devolver). | — |
| **2** | Perseguir | 3/6 · 50% | Detecta hostiles con Sensor (cada 0.2 s) y los persigue. Usa Seek (o Cuerpo.Mover con dirección directa si Seek aun no llega). | PathFollowing |
| **3** | Atacar | 4/6 · 67% | Ataca con cadencia y aplica daño a Salud del objetivo. Puede atacar el granero (DistanciaBorde). | — |
| **4** | Aturdido | 5/6 · 83% | Reaccion al daño con regreso al estado anterior guardado en Contexto. | — |
| **5** | Festejo | 6/6 · 100% | Festejo al terminar la partida (suscrito a Juego.AlCambiarFase). Integracion completa con el juego. | — |
| **6** | (pulido) | 6/6 · 100% | Pulido final: cero basura (medir), pooling correcto (OnEnable/OnDisable), demo en vivo. | — |

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 7. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `Soldado<Estado>`.

```csharp
// SoldadoController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Soldado
{
public class SoldadoController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new SoldadoAvanzar(), new SoldadoPerseguir(), new SoldadoAtacar(), new SoldadoAturdido(), new SoldadoFestejo(), new SoldadoMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("SoldadoAvanzar");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("SoldadoMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// SoldadoAvanzar.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.Soldado
{
public class SoldadoAvanzar : Estado
{
    float t;
    public override void Entrar(string anterior)
    {
        t = 0f;
        var ag = Ctx.Obtener<Agente>("agente");
        ag.Animar(Anim.Caminar);   // usa las constantes de la clase Anim
    }
    public override string Actualizar(float dt)
    {
        t += dt;
        // TODO: logica del estado; devuelve el NOMBRE del siguiente estado o null para quedarse
        return null;
    }
    public override string AlRecibir(string trigger, int valor) { return null; }
}
}
```

## 8. Reglas que se califican siempre

- Todo tu código va dentro de `namespace Alumnos.<TuMaquina>` (ej. `Alumnos.MagoEnemigo`). Los archivos `Steering*.cs` NO llevan namespace.
- Una clase por estado, hereda de `Estado`, nombre con prefijo del rol. Ninguna clase puede llamarse igual que una del juego base.
- Nada de `new`, LINQ, `GetComponent` ni `Find` dentro de `Actualizar`: guarda referencias en `Entrar` o en el Contexto.
- El Sensor se consulta cada 0.2 s, no cada frame.
- Suscripciones a eventos en `OnEnable`, baja en `OnDisable` (los agentes se reutilizan).
- Todo el código en ASCII (sin acentos ni ñ).
