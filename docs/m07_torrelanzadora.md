# 07 · Torre lanzadora

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Ligera**  **Defensa**

Torre de lanzas: vigila, apunta con anticipacion (le gana al objetivo que se mueve), dispara y recarga. Cuando la danan mucho dispara más lento y echa humo.

Peso de la máquina: **19.5 pts** · Steering a cargo: **8 pts** (Hide (8 pts)) · Total del cupo 07: **27.5 pts** · Estados: **6** · Transiciones: **9**

## 1. Diagrama de estados

![Diagrama de estados de Torre lanzadora](img/m07_torrelanzadora_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main && git pull && git checkout -b clase-1/07-torrelanzadora`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: la construyes tu: elige plataforma y pulsa 1.*
3. **Crea estos archivos** en `Assets/_Alumnos/TorreLanzadora/`: `TorreLanzadoraController.cs`, `TorreLanzadoraVigilando.cs`, `TorreLanzadoraDestruida.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`IdleCombate`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `TorreLanzadoraController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `TorreLanzadoraVigilando` | Idle; revisa el Sensor cada 0.2 s (alcance 14 m). | IdleCombate | — |
| `TorreLanzadoraApuntando` | Gira hacia el punto futuro del objetivo. | IdleCombate | Pursuit (prediccion) |
| `TorreLanzadoraDisparando` | Lanza la lanza (ProyectilBase.Lanzar) a mitad de la animación. | Lanzar1 / Lanzar2 | — |
| `TorreLanzadoraRecargando` | Cadencia 1.3 s. | Idle | — |
| `TorreLanzadoraDanada` | Vida &lt; 40%: cadencia x2 más lenta + Fx.Fuego como humo. Sale al curarse > 60%. | Idle | — |
| `TorreLanzadoraDestruida` (global) | Global: Fx.Explosion, libera la plataforma (Reserva.Devolver). | — | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Vigilando | Apuntando | hostil a &lt;= 14 m |
| Apuntando | Disparando | alineada &lt; 10 grados |
| Apuntando | Vigilando | objetivo perdido |
| Disparando | Recargando | disparo hecho |
| Recargando | Apuntando | 1.3 s y hay objetivo |
| Recargando | Vigilando | sin objetivo |
| Vigilando | Danada | vida &lt; 40% |
| Recargando | Danada | vida &lt; 40% |
| Danada | Vigilando | vida > 60% |
| *cualquiera* | Destruida | la vida llega a 0 (`Salud.AlMorir`) |

**Eventos que escucha:** `dano`, `muerte`, `curacion (Salud.AlCurar)`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Torre lanzadora](img/m07_torrelanzadora_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `ProyectilBase` | Lanzar(...) con Reserva |
| `Efectos` | Fx.Fuego, Fx.Explosion |
| `Steering` | Pursuit (predicción de puntería) |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Plataforma (SlotTorre) | recibe de | La plataforma la crea y se libera cuando esta Destruida. |
| Reparador | recibe de | El Reparador puede curarla (Salud.Curar): su evento la saca de Danada. |
| Soldado..Jefe | envía a | Dispara a los enemigos (bando Enemigo). |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.Pursuit`](02_steering_behaviors.md#pursuit-perseguir-con-predicción-6-pts) | Cupo 14 (Tutorial) | Clase 3 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Steering que debes programar tú (cupo 07)

| Behaviour | Pts | Entrega | Lo usan |
| --- | --- | --- | --- |
| [Hide (esconderse)](02_steering_behaviors.md#hide-esconderse-8-pts) | 8 | Clase 4 | Aldeano |

### Cómo probar tu steering aislado

Crea en tu carpeta de máquina un script de prueba (no se usa en el juego final) y ponlo en un objeto con un `Cuerpo` sobre el NavMesh de la escena:

```csharp
using UnityEngine;

namespace Alumnos.TorreLanzadora   // el namespace de tu MAQUINA (el probe no es parte de Steering)
{
public class ProbeHide : MonoBehaviour   // nombre unico: Probe + tu steering
{
    public Transform objetivo;
    public float vMax = 3f;
    Cuerpo cuerpo;
    void Awake() { cuerpo = GetComponent<Cuerpo>(); }
    void Update()
    {
        Vector3 v = Steering.Hide(transform.position, objetivo.position, vMax);   // ajusta los parametros a tu firma
        cuerpo.Mover(v, Time.deltaTime);
    }
}
}
```

## 6. Entregas por clase

![Linea de tiempo de entregas](img/m07_torrelanzadora_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** | Vigilando, Destruida | 2/6 · 33% | Controller, Maquina, vigila y se destruye liberando la plataforma. | — |
| **2** | Apuntando | 3/6 · 50% | Elige objetivo con Sensor y gira hacia el. | — |
| **3** | Disparando, Recargando | 5/6 · 83% | Dispara proyectiles con cooldown; el daño llega. | — |
| **4** | Danada | 6/6 · 100% | Estado Danada con efecto y cadencia reducida. | Hide |
| **5** | (pulido) | 6/6 · 100% | Prediccion de puntería (Pursuit) y ajuste de daño/alcance. | — |
| **6** | (pulido) | 6/6 · 100% | Pulido, cero basura, pooling de proyectiles, demo. | — |

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 7. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `TorreLanzadora<Estado>`.

```csharp
// TorreLanzadoraController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.TorreLanzadora
{
public class TorreLanzadoraController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new TorreLanzadoraVigilando(), new TorreLanzadoraApuntando(), new TorreLanzadoraDisparando(), new TorreLanzadoraRecargando(), new TorreLanzadoraDanada(), new TorreLanzadoraDestruida() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("TorreLanzadoraVigilando");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("TorreLanzadoraDestruida"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// TorreLanzadoraVigilando.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.TorreLanzadora
{
public class TorreLanzadoraVigilando : Estado
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
