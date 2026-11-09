# 11 · Minero

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Muy alta**  **Civil**

Genera la economía: va a su mina, pica, regresa al granero y deposita las gemas. Si hay peligro huye y se esconde; al volver retoma el trabajo.

Peso de la máquina: **28.5 pts** · Steering a cargo: **0 pts** (ninguno) · Total del cupo 11: **28.5 pts** · Estados: **8** · Transiciones: **9**

## 1. Diagrama de estados

![Diagrama de estados de Minero](img/m11_minero_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main && git pull && git checkout -b clase-1/11-minero`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: desde el inicio (nacen en la aldea).*
3. **Crea estos archivos** en `Assets/_Alumnos/Minero/`: `MineroController.cs`, `MineroIrAMina.cs`, `MineroMinar.cs`, `MineroMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Caminar`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `MineroController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `MineroIrAMina` | Camina a su mina (puntosMina) con Camino. | Caminar | Arrive + PathFollowing |
| `MineroMinar` | Pica 5 s en su sitio. | Minar / MinarLoop | — |
| `MineroIrAlGranero` | Lleva las gemas al granero. | Caminar | PathFollowing |
| `MineroDepositar` | Entrega: Economia.Agregar(6). | Recolectar | — |
| `MineroHuir` | Se aleja del enemigo (conserva la carga). | Correr | Flee |
| `MineroEsconderse` | Espera escondido 4 s a que pase el peligro. | Agachado | — |
| `MineroFestejo` (global) | Global. | Celebrar1 | — |
| `MineroMuerto` (global) | Global: la carga se pierde. | Muerte1 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| IrAMina | Minar | llego a la mina |
| Minar | IrAlGranero | 5 s |
| IrAlGranero | Depositar | llego (Camino.Llego) |
| Depositar | IrAMina | 1 s |
| IrAMina | Huir | enemigo / alerta |
| Minar | Huir | enemigo / alerta |
| IrAlGranero | Huir | enemigo / alerta |
| Huir | Esconderse | a salvo |
| Esconderse | IrAMina | 4 s sin peligro |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |
| *cualquiera* | Festejo | fin de partida (`Juego.AlCambiarFase`) |

**Eventos que escucha:** `dano`, `muerte`, `fin_juego`, `enemigo_visto`, `Alertas.AlAlerta`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Minero](img/m11_minero_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `Economia` | Agregar(n) |
| `Camino` | Calcular(desde,hasta) solo al entrar al estado |
| `Juego.puntosMina` | su mina asignada |
| `Steering` | Arrive, PathFollowing, Flee |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Economia | envía a | Economia.Agregar(6) por viaje; sube las gemas del jugador (HUD por evento). |
| Guardia | recibe de | Alertas.AlAlerta. |
| Tutorial | envía a | El Tutorial puede explicar de donde salen las gemas. |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.Arrive`](02_steering_behaviors.md#arrive-llegar-4-pts) | Cupo 02 (Bruto) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.PathFollowing`](02_steering_behaviors.md#path-following-seguir-camino-7-pts) | Cupo 01 (Soldado) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Flee`](02_steering_behaviors.md#flee-huir-2-pts) | Cupo 09 (Guardia) | Clase 1 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Entregas por clase

![Linea de tiempo de entregas](img/m11_minero_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** | IrAMina, Minar, Muerto | 3/8 · 38% | Esqueleto: va a la mina y pica en ciclo; muerte. | — |
| **2** | IrAlGranero, Depositar | 5/8 · 62% | Ciclo completo con Economia.Agregar visible en el HUD. | — |
| **3** | Huir | 6/8 · 75% | Deteccion de peligro y huida con Flee. | — |
| **4** | Esconderse | 7/8 · 88% | Escondite y retorno al trabajo con la carga. | — |
| **5** | Festejo | 8/8 · 100% | Festejo + escucha de Alertas. Camino calculado solo al entrar al estado. | — |
| **6** | (pulido) | 8/8 · 100% | Pulido, cero basura, demo. | — |

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 6. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `Minero<Estado>`.

```csharp
// MineroController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Minero
{
public class MineroController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new MineroIrAMina(), new MineroMinar(), new MineroIrAlGranero(), new MineroDepositar(), new MineroHuir(), new MineroEsconderse(), new MineroFestejo(), new MineroMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("MineroIrAMina");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("MineroMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// MineroIrAMina.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.Minero
{
public class MineroIrAMina : Estado
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
