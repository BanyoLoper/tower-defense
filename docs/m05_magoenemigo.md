# 05 · Mago enemigo

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Alta**  **Enemigo**

Lanza bolas de fuego desde lejos (9 m). Busca la distancia ideal, lanza, recarga y huye si lo alcanzan. Usa proyectiles con Reserva.

Peso de la máquina: **27.5 pts** · Steering a cargo: **0 pts** (ninguno) · Total del cupo 05: **27.5 pts** · Estados: **8** · Transiciones: **10**

## 1. Diagrama de estados

![Diagrama de estados de Mago enemigo](img/m05_magoenemigo_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main && git pull && git checkout -b clase-1/05-magoenemigo`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: desde la oleada 3.*
3. **Crea estos archivos** en `Assets/_Alumnos/MagoEnemigo/`: `MagoEnemigoController.cs`, `MagoEnemigoAvanzar.cs`, `MagoEnemigoMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Caminar`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `MagoEnemigoController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `MagoEnemigoAvanzar` | Sigue la ruta. | Caminar | PathFollowing |
| `MagoEnemigoPosicionarse` | Se coloca a ~9 m del objetivo y frena suave. | Caminar | Arrive |
| `MagoEnemigoLanzar` | Cast con proyectil: ProyectilBase.Lanzar(...) a mitad de la animación. | HechizoLanzar | — |
| `MagoEnemigoRecargar` | Espera 2.4 s entre hechizos. | HechizoIdle | — |
| `MagoEnemigoHuir` | Si un hostil esta a &lt; 3 m, se escapa 1.5 s anticipando su trayectoria. | Correr | Evade |
| `MagoEnemigoAturdido` | Reaccion al daño. | Dano1 | — |
| `MagoEnemigoFestejo` (global) | Global. | Celebrar1 | — |
| `MagoEnemigoMuerto` (global) | Global. | Muerte1 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Avanzar | Posicionarse | hostil a &lt;= 11 m |
| Posicionarse | Lanzar | dist ~ 9 m |
| Lanzar | Recargar | hechizo lanzado |
| Recargar | Lanzar | 2.4 s y hay objetivo |
| Recargar | Posicionarse | objetivo se movio |
| Lanzar | Huir | hostil &lt; 3 m |
| Recargar | Huir | hostil &lt; 3 m |
| Huir | Posicionarse | 1.5 s |
| Posicionarse | Aturdido | daño |
| Aturdido | Posicionarse | 0.4 s |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |
| *cualquiera* | Festejo | fin de partida (`Juego.AlCambiarFase`) |

**Eventos que escucha:** `dano`, `muerte`, `fin_juego`, `hostil_visto`, `hostil_muy_cerca`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Mago enemigo](img/m05_magoenemigo_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `ProyectilBase` | Lanzar(quien,destino,bando,daño) con Reserva |
| `Steering` | Arrive, Evade, PathFollowing |
| `Ruta` | Ruta.Puntos |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Director | recibe de | Lo crea desde la oleada 3. |
| Torre / Guardia | envía a | Es su objetivo prioritario por ser fragil. |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.PathFollowing`](02_steering_behaviors.md#path-following-seguir-camino-7-pts) | Cupo 01 (Soldado) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Arrive`](02_steering_behaviors.md#arrive-llegar-4-pts) | Cupo 02 (Bruto) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Evade`](02_steering_behaviors.md#evade-escapar-con-predicción-6-pts) | Cupo 12 (Reparador) | Clase 3 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Entregas por clase

![Linea de tiempo de entregas](img/m05_magoenemigo_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** | Avanzar, Muerto | 2/8 · 25% | Esqueleto, ruta y muerte. | — |
| **2** | Posicionarse | 3/8 · 38% | Se detiene a la distancia ideal con Arrive. | — |
| **3** | Lanzar, Recargar | 5/8 · 62% | Dispara proyectiles con cooldown; el daño llega al impacto. | — |
| **4** | Huir | 6/8 · 75% | Evade/escape al ser alcanzado y regreso a Posicionarse. | — |
| **5** | Aturdido, Festejo | 8/8 · 100% | Reacciones y festejo. | — |
| **6** | (pulido) | 8/8 · 100% | Pulido, pooling de proyectiles, cero basura, demo. | — |

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 6. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `MagoEnemigo<Estado>`.

```csharp
// MagoEnemigoController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.MagoEnemigo
{
public class MagoEnemigoController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new MagoEnemigoAvanzar(), new MagoEnemigoPosicionarse(), new MagoEnemigoLanzar(), new MagoEnemigoRecargar(), new MagoEnemigoHuir(), new MagoEnemigoAturdido(), new MagoEnemigoFestejo(), new MagoEnemigoMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("MagoEnemigoAvanzar");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("MagoEnemigoMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// MagoEnemigoAvanzar.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.MagoEnemigo
{
public class MagoEnemigoAvanzar : Estado
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
