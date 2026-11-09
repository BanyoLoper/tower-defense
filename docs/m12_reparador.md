# 12 · Reparador

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Media**  **Civil**

Mantiene el granero: cuando el cristal esta dañado va al punto de reparacion y martilla (cura 4 de vida por segundo, y los cristales rotos reaparecen). Huye si hay enemigos cerca.

Peso de la máquina: **23.0 pts** · Steering a cargo: **6 pts** (Evade (6 pts)) · Total del cupo 12: **29.0 pts** · Estados: **6** · Transiciones: **7**

## 1. Diagrama de estados

![Diagrama de estados de Reparador](img/m12_reparador_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main && git pull && git checkout -b clase-1/12-reparador`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: desde el inicio (nacen en la aldea).*
3. **Crea estos archivos** en `Assets/_Alumnos/Reparador/`: `ReparadorController.cs`, `ReparadorEsperar.cs`, `ReparadorMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Idle`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `ReparadorController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `ReparadorEsperar` | Descansa cerca del punto de reparacion. | Idle | — |
| `ReparadorIrAReparar` | Se acerca al punto de reparacion frenando suave. | Caminar | Arrive |
| `ReparadorMartillar` | Martilla: Salud.Curar(4*dt) al granero + Fx.Curacion cada 2 s. | MartillarLoop | — |
| `ReparadorHuir` | Se aleja del enemigo. | Correr | Flee |
| `ReparadorFestejo` (global) | Global. | Celebrar1 | — |
| `ReparadorMuerto` (global) | Global. | Muerte1 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Esperar | IrAReparar | granero con vida &lt; max |
| IrAReparar | Martillar | llego |
| Martillar | Esperar | granero lleno |
| Esperar | Huir | enemigo cerca |
| Martillar | Huir | enemigo cerca |
| IrAReparar | Huir | enemigo cerca |
| Huir | Esperar | a salvo |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |
| *cualquiera* | Festejo | fin de partida (`Juego.AlCambiarFase`) |

**Eventos que escucha:** `dano`, `muerte`, `fin_juego`, `granero_danado (Salud.AlDanar)`, `enemigo_cerca`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Reparador](img/m12_reparador_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `Salud (granero)` | Curar(cantidad), AlDanar |
| `Efectos` | Fx.Curacion |
| `Juego.puntoReparacion` | donde martilla |
| `Steering` | Arrive, Flee |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Granero (GraneroEstados) | envía a | Al curar, el estado del granero cambia solo (Sano/Danado/Critico) y los cristales reaparecen. |
| Torres | envía a | Extension: también puede curar torres en Danada. |
| Guardia | recibe de | Alertas.AlAlerta. |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.Arrive`](02_steering_behaviors.md#arrive-llegar-4-pts) | Cupo 02 (Bruto) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Flee`](02_steering_behaviors.md#flee-huir-2-pts) | Cupo 09 (Guardia) | Clase 1 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Steering que debes programar tú (cupo 12)

| Behaviour | Pts | Entrega | Lo usan |
| --- | --- | --- | --- |
| [Evade (escapar con predicción)](02_steering_behaviors.md#evade-escapar-con-predicción-6-pts) | 6 | Clase 3 | MagoEnemigo, Ladron |

### Cómo probar tu steering aislado

Crea en tu carpeta de máquina un script de prueba (no se usa en el juego final) y ponlo en un objeto con un `Cuerpo` sobre el NavMesh de la escena:

```csharp
using UnityEngine;

namespace Alumnos.Reparador   // el namespace de tu MAQUINA (el probe no es parte de Steering)
{
public class ProbeEvade : MonoBehaviour   // nombre unico: Probe + tu steering
{
    public Transform objetivo;
    public float vMax = 3f;
    Cuerpo cuerpo;
    void Awake() { cuerpo = GetComponent<Cuerpo>(); }
    void Update()
    {
        Vector3 v = Steering.Evade(transform.position, objetivo.position, vMax);   // ajusta los parametros a tu firma
        cuerpo.Mover(v, Time.deltaTime);
    }
}
}
```

## 6. Entregas por clase

![Linea de tiempo de entregas](img/m12_reparador_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** | Esperar, Muerto | 2/6 · 33% | Esqueleto y espera; muerte. | — |
| **2** | IrAReparar | 3/6 · 50% | Se suscribe a Salud del granero y camina al punto de reparacion. | — |
| **3** | Martillar | 4/6 · 67% | Cura al granero con Salud.Curar; se ve en el HUD y en los cristales. | Evade |
| **4** | Huir | 5/6 · 83% | Huye de enemigos y regresa a trabajar. | — |
| **5** | Festejo | 6/6 · 100% | Festejo + integración completa. | — |
| **6** | (pulido) | 6/6 · 100% | Extension (curar torres), pulido, cero basura, demo. | — |

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 7. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `Reparador<Estado>`.

```csharp
// ReparadorController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Reparador
{
public class ReparadorController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new ReparadorEsperar(), new ReparadorIrAReparar(), new ReparadorMartillar(), new ReparadorHuir(), new ReparadorFestejo(), new ReparadorMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("ReparadorEsperar");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("ReparadorMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// ReparadorEsperar.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.Reparador
{
public class ReparadorEsperar : Estado
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
