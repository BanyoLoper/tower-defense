# 03 · Jefe (final de la oleada 5)

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Muy alta**  **Enemigo**

El jefe final. Tiene fases: aparece con un rugido, pelea, invoca refuerzos y lanza una onda de choque. Por debajo del 50% de vida entra en furia. Su muerte es la condición de victoria.

Peso de la máquina: **29.0 pts** · Steering a cargo: **0 pts** (ninguno) · Total del cupo 03: **29.0 pts** · Estados: **7** · Transiciones: **9**

## 1. Diagrama de estados

![Diagrama de estados de Jefe (final de la oleada 5)](img/m03_jefe_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main && git pull && git checkout -b clase-1/03-jefe`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: oleada 5.*
3. **Crea estos archivos** en `Assets/_Alumnos/Jefe/`: `JefeController.cs`, `JefeAparicion.cs`, `JefeAvanzar.cs`, `JefeMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Enojado`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `JefeController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `JefeAparicion` | Rugido de entrada (2 s): Fx.OndaSuelo + CamaraRTS.I.Sacudir. | Enojado | — |
| `JefeAvanzar` | Camina la ruta hacia el granero. | Caminar | PathFollowing + ObstacleAvoidance |
| `JefeAtacar` | Combo de 3 golpes (Atacar1-3) con daño 70. | Atacar1-3 | Seek |
| `JefeInvocar` | Cada 15 s llama 3 Soldados con Juego.Aparecer (max 6 vivos). | Enojado + Fx.Aura | — |
| `JefeOndaChoque` | Ataque en área (radio 5): daño a todos los hostiles cercanos + Fx.Explosion. | Atacar3 + Fx | — |
| `JefeFuria` | Vida &lt; 50%: más rápido y combos más cortos. Una sola vez. | Enojado | — |
| `JefeMuerto` (global) | Global: Fx.Explosion y aviso al Director. | Muerte1 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Aparicion | Avanzar | 2 s |
| Avanzar | Atacar | dist &lt;= 2.8 |
| Atacar | Avanzar | objetivo lejos |
| Avanzar | Invocar | 15 s y &lt; 6 soldados |
| Invocar | Avanzar | 2.5 s |
| Atacar | OndaChoque | 3 golpes o >= 3 hostiles |
| OndaChoque | Atacar | 1.5 s |
| Atacar | Furia | vida &lt; 50% |
| Furia | Atacar | 1.5 s |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |

**Eventos que escucha:** `dano`, `muerte`, `vida_baja`, `timer_invocar`, `fin_juego`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Jefe (final de la oleada 5)](img/m03_jefe_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `Juego.Aparecer` | crear los refuerzos |
| `Efectos` | Fx.OndaSuelo, Fx.Aura, Fx.Explosion |
| `CamaraRTS` | CamaraRTS.I.Sacudir(fuerza) |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Director | recibe de | El Director lo crea (oleada 5) y espera su muerte. |
| Director | envía a | Al morir, Agente.AlMorirAlguien avisa; el Director termina con victoria. |
| Soldado | envía a | Invoca soldados (los controla su propia máquina). |
| Camara | envía a | Pide temblor en Aparicion y OndaChoque. |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.PathFollowing`](02_steering_behaviors.md#path-following-seguir-camino-7-pts) | Cupo 01 (Soldado) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Seek`](02_steering_behaviors.md#seek-buscar-2-pts) | Cupo 04 (Lancero) | Clase 1 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.ObstacleAvoidance`](02_steering_behaviors.md#obstacle-avoidance-evitar-obstaculos-10-pts) | Cupo 08 (TorreMaga) | Clase 4 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Entregas por clase

![Linea de tiempo de entregas](img/m03_jefe_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** | Aparicion, Avanzar, Muerto | 3/7 · 43% | Entrada con rugido (Fx + cámara), camina la ruta y muere. | — |
| **2** | Atacar | 4/7 · 57% | Combo de 3 golpes con daño y alcance correctos. | — |
| **3** | Invocar | 5/7 · 71% | Invoca soldados con Juego.Aparecer, con límite y cooldown. | — |
| **4** | OndaChoque | 6/7 · 86% | Ataque en área con Efectos y daño por radio (sin basura). | — |
| **5** | Furia | 7/7 · 100% | Fase de furia y avisos al Director/Camara. Integracion con la victoria. | — |
| **6** | (pulido) | 7/7 · 100% | Balance (que sea ganable), pulido, cero basura, demo. | — |

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 6. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `Jefe<Estado>`.

```csharp
// JefeController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Jefe
{
public class JefeController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new JefeAparicion(), new JefeAvanzar(), new JefeAtacar(), new JefeInvocar(), new JefeOndaChoque(), new JefeFuria(), new JefeMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("JefeAparicion");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("JefeMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// JefeAparicion.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.Jefe
{
public class JefeAparicion : Estado
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

## 7. Reglas que se califican siempre

- Todo tu código va dentro de `namespace Alumnos.<TuMaquina>` (ej. `Alumnos.MagoEnemigo`). Los archivos `Steering*.cs` NO llevan namespace.
- Una clase por estado, hereda de `Estado`, nombre con prefijo del rol. Ninguna clase puede llamarse igual que una del juego base.
- Nada de `new`, LINQ, `GetComponent` ni `Find` dentro de `Actualizar`: guarda referencias en `Entrar` o en el Contexto.
- El Sensor se consulta cada 0.2 s, no cada frame.
- Suscripciones a eventos en `OnEnable`, baja en `OnDisable` (los agentes se reutilizan).
- Todo el código en ASCII (sin acentos ni ñ).
