# 10 · Aldeano

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Media**  **Civil**

Civil de la aldea. Pasea y conversa; al ver enemigos o oir una alarma se asusta, huye y se esconde cerca del granero. Pasada la amenaza, vuelve a su vida.

Peso de la máquina: **25.0 pts** · Steering a cargo: **6 pts** (Wander (6 pts)) · Total del cupo 10: **31.0 pts** · Estados: **7** · Transiciones: **7**

## 1. Diagrama de estados

![Diagrama de estados de Aldeano](img/m10_aldeano_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main && git pull && git checkout -b clase-1/10-aldeano`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: desde el inicio (nacen en la aldea).*
3. **Crea estos archivos** en `Assets/_Alumnos/Aldeano/`: `AldeanoController.cs`, `AldeanoVagar.cs`, `AldeanoMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Caminar`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `AldeanoController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `AldeanoVagar` | Pasea sin rumbo por la aldea evitando arboles. | Caminar | Wander |
| `AldeanoCharlar` | Se detiene 3 s con otro aldeano cercano. | Hablar / Saludar | — |
| `AldeanoAsustarse` | Reaccion de susto 0.8 s al ver un enemigo o oir Alertas. | Miedo | — |
| `AldeanoHuir` | Se aleja del enemigo (con sesgo hacia el granero). | Correr | Flee |
| `AldeanoEsconderse` | Se pega a un escondite (casa/granero/rocas) fuera de la línea de visión del enemigo. | Agachado | Hide + Arrive |
| `AldeanoFestejo` (global) | Global: celebra en victoria, llora en derrota. | Celebrar / Llorar | — |
| `AldeanoMuerto` (global) | Global. | Muerte1 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Vagar | Charlar | otro aldeano &lt; 3 m |
| Charlar | Vagar | 3 s |
| Vagar | Asustarse | enemigo &lt;= 7 m o alerta |
| Charlar | Asustarse | alerta / enemigo |
| Asustarse | Huir | 0.8 s |
| Huir | Esconderse | a salvo (> 10 m) o llego |
| Esconderse | Vagar | 4 s sin peligro |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |
| *cualquiera* | Festejo | fin de partida (`Juego.AlCambiarFase`) |

**Eventos que escucha:** `dano`, `muerte`, `fin_juego`, `enemigo_visto`, `Alertas.AlAlerta`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Aldeano](img/m10_aldeano_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `Alertas` | AlAlerta: suscribirse en OnEnable, desuscribirse en OnDisable |
| `Steering` | Wander, Flee, Hide, Arrive |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Guardia | recibe de | Alertas.AlAlerta (posicion, radio). |
| Soldado..Jefe | recibe de | Su presencia dispara el susto (Sensor). |
| Director | recibe de | Nace al inicio con Juego.aldea. |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.Wander`](02_steering_behaviors.md#wander-vagar-6-pts) | Cupo 10 (Aldeano) | Clase 3 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Flee`](02_steering_behaviors.md#flee-huir-2-pts) | Cupo 09 (Guardia) | Clase 1 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Hide`](02_steering_behaviors.md#hide-esconderse-8-pts) | Cupo 07 (TorreLanzadora) | Clase 4 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Arrive`](02_steering_behaviors.md#arrive-llegar-4-pts) | Cupo 02 (Bruto) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Steering que debes programar tú (cupo 10)

| Behaviour | Pts | Entrega | Lo usan |
| --- | --- | --- | --- |
| [Wander (vagar)](02_steering_behaviors.md#wander-vagar-6-pts) | 6 | Clase 3 | Ladron, Aldeano |

### Cómo probar tu steering aislado

Crea en tu carpeta de máquina un script de prueba (no se usa en el juego final) y ponlo en un objeto con un `Cuerpo` sobre el NavMesh de la escena:

```csharp
using UnityEngine;

namespace Alumnos.Aldeano   // el namespace de tu MAQUINA (el probe no es parte de Steering)
{
public class ProbeWander : MonoBehaviour   // nombre unico: Probe + tu steering
{
    public Transform objetivo;
    public float vMax = 3f;
    Cuerpo cuerpo;
    void Awake() { cuerpo = GetComponent<Cuerpo>(); }
    void Update()
    {
        Vector3 v = Steering.Wander(transform.position, objetivo.position, vMax);   // ajusta los parametros a tu firma
        cuerpo.Mover(v, Time.deltaTime);
    }
}
}
```

## 6. Entregas por clase

![Linea de tiempo de entregas](img/m10_aldeano_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** | Vagar, Muerto | 2/7 · 29% | Esqueleto y paseo con Wander; muerte. | — |
| **2** | Asustarse, Huir | 4/7 · 57% | Detecta enemigos y huye con Flee. | — |
| **3** | Esconderse | 5/7 · 71% | Se esconde (Hide/Arrive) y espera a que pase el peligro. | Wander |
| **4** | Charlar | 6/7 · 86% | Interaccion entre aldeanos (comunicación por Sensor/Agente.Todos). | — |
| **5** | Festejo | 7/7 · 100% | Celebra o llora segun el resultado. Escucha Alertas del Guardia. | — |
| **6** | (pulido) | 7/7 · 100% | Pulido, cero basura, demo. | — |

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 7. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `Aldeano<Estado>`.

```csharp
// AldeanoController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Aldeano
{
public class AldeanoController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new AldeanoVagar(), new AldeanoCharlar(), new AldeanoAsustarse(), new AldeanoHuir(), new AldeanoEsconderse(), new AldeanoFestejo(), new AldeanoMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("AldeanoVagar");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("AldeanoMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// AldeanoVagar.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.Aldeano
{
public class AldeanoVagar : Estado
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
