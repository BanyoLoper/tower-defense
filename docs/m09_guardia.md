# 09 · Guardia de la aldea

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Alta**  **Defensa**

Patrulla alrededor de su puesto. Si ve un enemigo da la alarma (los aldeanos la oyen), lo persigue, pelea y regresa a descansar. Es el emisor de la alarma del juego.

Peso de la máquina: **26.5 pts** · Steering a cargo: **2 pts** (Flee (2 pts)) · Total del cupo 09: **28.5 pts** · Estados: **8** · Transiciones: **9**

## 1. Diagrama de estados

![Diagrama de estados de Guardia de la aldea](img/m09_guardia_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main && git pull && git checkout -b clase-1/09-guardia`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: desde el inicio (nacen en la aldea).*
3. **Crea estos archivos** en `Assets/_Alumnos/Guardia/`: `GuardiaController.cs`, `GuardiaPatrullar.cs`, `GuardiaMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Caminar`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `GuardiaController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `GuardiaPatrullar` | Recorre puntos alrededor de su casa en ciclo. | Caminar | PathFollowing |
| `GuardiaAlertar` | Grito de alarma 1.2 s: Alertas.Emitir(pos, 15). | Enojado | — |
| `GuardiaPerseguir` | Corre tras el enemigo anticipando su movimiento (max 16 m de su casa). | Correr | Pursuit |
| `GuardiaCombatir` | Ataca (daño 12, cadencia 0.9 s). | Atacar1 / Atacar2 | — |
| `GuardiaRegresar` | Vuelve a su puesto frenando suave. | Caminar | Arrive |
| `GuardiaDescansar` | Espera 3 s en su puesto. | Idle | — |
| `GuardiaFestejo` (global) | Global: gana si hay Victoria. | Celebrar1 | — |
| `GuardiaMuerto` (global) | Global. | Muerte1 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Patrullar | Alertar | enemigo visto (10 m) |
| Alertar | Perseguir | 1.2 s |
| Perseguir | Combatir | dist &lt;= 1.7 |
| Combatir | Perseguir | objetivo se aleja |
| Perseguir | Regresar | objetivo > 16 m de casa |
| Combatir | Regresar | objetivo muerto |
| Regresar | Descansar | llego a casa |
| Descansar | Patrullar | 3 s |
| Regresar | Alertar | enemigo visto |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |
| *cualquiera* | Festejo | fin de partida (`Juego.AlCambiarFase`) |

**Eventos que escucha:** `dano`, `muerte`, `fin_juego`, `enemigo_visto`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Guardia de la aldea](img/m09_guardia_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `Alertas` | Emitir(pos, radio, origen) |
| `Steering` | PathFollowing, Pursuit, Arrive |
| `Juego.aldea` | su punto de casa (nombre Guardia_n) |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Aldeano / Minero / Reparador | envía a | Alertas.AlAlerta: les avisa del peligro. |
| Ladron | envía a | Es quien más lo persigue. |
| Director | recibe de | Nace al inicio de la partida con Juego.aldea. |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.PathFollowing`](02_steering_behaviors.md#path-following-seguir-camino-7-pts) | Cupo 01 (Soldado) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Pursuit`](02_steering_behaviors.md#pursuit-perseguir-con-predicción-6-pts) | Cupo 14 (Tutorial) | Clase 3 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Arrive`](02_steering_behaviors.md#arrive-llegar-4-pts) | Cupo 02 (Bruto) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Steering que debes programar tú (cupo 09)

| Behaviour | Pts | Entrega | Lo usan |
| --- | --- | --- | --- |
| [Flee (huir)](02_steering_behaviors.md#flee-huir-2-pts) | 2 | Clase 1 | Lancero, Aldeano, Minero, Reparador |

### Cómo probar tu steering aislado

Crea en tu carpeta de máquina un script de prueba (no se usa en el juego final) y ponlo en un objeto con un `Cuerpo` sobre el NavMesh de la escena:

```csharp
using UnityEngine;

namespace Alumnos.Guardia   // el namespace de tu MAQUINA (el probe no es parte de Steering)
{
public class ProbeFlee : MonoBehaviour   // nombre unico: Probe + tu steering
{
    public Transform objetivo;
    public float vMax = 3f;
    Cuerpo cuerpo;
    void Awake() { cuerpo = GetComponent<Cuerpo>(); }
    void Update()
    {
        Vector3 v = Steering.Flee(transform.position, objetivo.position, vMax);   // ajusta los parametros a tu firma
        cuerpo.Mover(v, Time.deltaTime);
    }
}
}
```

## 6. Entregas por clase

![Linea de tiempo de entregas](img/m09_guardia_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** | Patrullar, Muerto | 2/8 · 25% | Esqueleto y patrulla en ciclo por puntos propios. | Flee |
| **2** | Perseguir | 3/8 · 38% | Deteccion y persecución con límite de distancia a casa. | — |
| **3** | Combatir | 4/8 · 50% | Combate con daño y cadencia. | — |
| **4** | Alertar | 5/8 · 62% | Emite la alerta (contrato Alertas) y la alerta se ve en Aldeano de un companero. | — |
| **5** | Regresar, Descansar, Festejo | 8/8 · 100% | Regreso con Arrive, descanso y festejo. | — |
| **6** | (pulido) | 8/8 · 100% | Pulido, Pursuit con predicción, cero basura, demo. | — |

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 7. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `Guardia<Estado>`.

```csharp
// GuardiaController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Guardia
{
public class GuardiaController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new GuardiaPatrullar(), new GuardiaAlertar(), new GuardiaPerseguir(), new GuardiaCombatir(), new GuardiaRegresar(), new GuardiaDescansar(), new GuardiaFestejo(), new GuardiaMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("GuardiaPatrullar");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("GuardiaMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// GuardiaPatrullar.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.Guardia
{
public class GuardiaPatrullar : Estado
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
