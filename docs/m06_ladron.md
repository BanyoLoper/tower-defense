# 06 · Ladron

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Como subir cambios](04_como_subir_cambios.md) · [Troubleshooting](05_troubleshooting.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Alta**  **Enemigo**

No pelea: va al granero, roba gemas y huye. Si escapa con la carga, el jugador pierde las gemas; si lo matan, las gemas regresan. Es la máquina que más conversa con la economía.

Peso de la máquina: **27.0 pts** · Steering a cargo: **0 pts** (ninguno) · Total del cupo 06: **27.0 pts** · Estados: **8** · Transiciones: **7**

## 1. Diagrama de estados

![Diagrama de estados de Ladron](img/m06_ladron_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

> Documentos de apoyo: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md) · [Guía de inicio](03_como_empezar.md).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main; git pull; git checkout -b clase-1/06-ladron`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: oleadas 3 y 5.*
3. **Crea estos archivos** en `Assets/_Alumnos/Ladron/`: `LadronController.cs`, `LadronMerodear.cs`, `LadronAcercarse.cs`, `LadronMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Sigilo`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `LadronController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `LadronMerodear` | Al aparecer, tantea el terreno 1.5-3 s con movimiento errante. | Sigilo | Wander |
| `LadronAcercarse` | Va al granero por la ruta y frena al llegar. | Correr (3.4 m/s) | PathFollowing + Arrive |
| `LadronRobar` | Anim RobarInicio -> RobarLoop (2.6 s). Al terminar: Economia.Robar(25). | RobarInicio / RobarLoop | — |
| `LadronHuir` | Corre de regreso al inicio con la carga, esquivando guardias. | Sprint | Evade |
| `LadronEscapar` | Llego al inicio con la carga: desaparece (las gemas se pierden). | — | — |
| `LadronAturdido` | Reaccion al daño (suelta el paso). | Dano1 | — |
| `LadronFestejo` (global) | Global. | Celebrar1 | — |
| `LadronMuerto` (global) | Global: devuelve la carga con Economia.Agregar. | Muerte1 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Merodear | Acercarse | 1.5-3 s |
| Acercarse | Robar | borde del granero &lt; 1.2 |
| Robar | Huir | 2.6 s (carga = 25) |
| Huir | Escapar | llego al inicio |
| Acercarse | Aturdido | daño |
| Aturdido | Acercarse | 0.4 s |
| Robar | Huir | daño (abandona) |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |
| *cualquiera* | Festejo | fin de partida (`Juego.AlCambiarFase`) |

**Eventos que escucha:** `dano`, `muerte`, `fin_juego`, `carga_robada`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Ladron](img/m06_ladron_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `Economia` | Robar(n) y Agregar(n) |
| `Ruta` | Ruta.Inicio / Ruta.Granero |
| `Steering` | Wander, Arrive, Evade, PathFollowing |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Economia | envía a | Economia.Robar(25) al robar; Economia.Agregar(carga) si muere con carga. |
| Director | recibe de | Lo crea desde la oleada 3; si escapa el Director lo cuenta como enemigo eliminado (Reserva). |
| Guardia | recibe de | Es su principal amenaza. |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.Wander`](02_steering_behaviors.md#wander-vagar-6-pts) | Cupo 10 (Aldeano) | Clase 3 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.PathFollowing`](02_steering_behaviors.md#path-following-seguir-camino-7-pts) | Cupo 01 (Soldado) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Arrive`](02_steering_behaviors.md#arrive-llegar-4-pts) | Cupo 02 (Bruto) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Evade`](02_steering_behaviors.md#evade-escapar-con-predicción-6-pts) | Cupo 12 (Reparador) | Clase 3 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Entregas por clase

![Linea de tiempo de entregas](img/m06_ladron_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase y día | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** Lun 9 nov | Merodear, Acercarse, Muerto | 3/8 · 38% | Wander inicial, llega al granero y muere (con carga devuelta). | — |
| **2** Mié 11 nov | Robar | 4/8 · 50% | Ciclo de robo con animaciones y Economia.Robar. | — |
| **3** Mié 18 nov | Huir | 5/8 · 62% | Huida con carga hacia Ruta.Inicio. | — |
| **4** Lun 23 nov | Escapar | 6/8 · 75% | Desaparicion al escapar y reglas de la carga (perdida / devuelta). | — |
| **5** Mié 25 nov | Aturdido, Festejo | 8/8 · 100% | Reacciones y festejo. | — |
| **6** Lun 30 nov | (pulido) | 8/8 · 100% | Pulido: Evade con guardias, cero basura, demo. | — |

Guías: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md).

### Detalle de cada entrega (qué entregar, cómo comprobarlo y qué comandos usar)

> Antes de tu primera entrega lee **[Cómo subir tus cambios](04_como_subir_cambios.md)**. Si el pipeline marca algo en rojo: **[Troubleshooting (si el pipeline falla)](05_troubleshooting.md)**. Todas las fechas son el día de clase; el lunes 16 de noviembre es puente (no hay clase).

### Clase 1 · Lun 9 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-1/06-ladron`  ·  **Avance esperado:** 3 de 8 estados (38%)

![Diagrama de estados de Ladron](img/m06_ladron_4.svg)

| Archivo (en `Assets/_Alumnos/Ladron/`) | Qué debe tener |
| --- | --- |
| `LadronController.cs` | **Nuevo.** `class LadronController : MonoBehaviour` dentro de `namespace Alumnos.Ladron`. Crea el `Contexto`, la `Maquina` con tus estados e inicia en `Merodear`. |
| `LadronMerodear.cs` | **Nuevo.** `class LadronMerodear : Estado`. Al aparecer, tantea el terreno 1.5-3 s con movimiento errante. Animación: `Sigilo`. |
| `LadronAcercarse.cs` | **Nuevo.** `class LadronAcercarse : Estado`. Va al granero por la ruta y frena al llegar. Animación: `Correr (3.4 m/s)`. |
| `LadronMuerto.cs` | **Nuevo.** `class LadronMuerto : Estado`. Global: devuelve la carga con Economia.Agregar. Animación: `Muerte1`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Wander inicial, llega al granero y muere (con carga devuelta).
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 3 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `LadronMerodear`, `LadronAcercarse`, `LadronMuerto`.

Todavía NO se pide: `LadronRobar`, `LadronHuir`, `LadronEscapar`, `LadronAturdido`, `LadronFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Ladron/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Ladron` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-1/06-ladron` y el PR se crea hoy (máximo el Lun 9 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-1/06-ladron
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Ladron
git commit -m "clase 1: Ladron - Merodear, Acercarse, Muerto"
git push -u origin clase-1/06-ladron
```

### Clase 2 · Mié 11 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-2/06-ladron`  ·  **Avance esperado:** 4 de 8 estados (50%)

![Diagrama de estados de Ladron](img/m06_ladron_5.svg)

| Archivo (en `Assets/_Alumnos/Ladron/`) | Qué debe tener |
| --- | --- |
| `LadronController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new LadronRobar()`. |
| `LadronRobar.cs` | **Nuevo.** `class LadronRobar : Estado`. Anim RobarInicio -> RobarLoop (2.6 s). Al terminar: Economia.Robar(25). Animación: `RobarInicio / RobarLoop`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Ciclo de robo con animaciones y Economia.Robar.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 3 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `LadronMerodear`, `LadronAcercarse`, `LadronMuerto`, `LadronRobar`.

Todavía NO se pide: `LadronHuir`, `LadronEscapar`, `LadronAturdido`, `LadronFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Ladron/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Ladron` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-2/06-ladron` y el PR se crea hoy (máximo el Mié 11 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-2/06-ladron
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Ladron
git commit -m "clase 2: Ladron - Robar"
git push -u origin clase-2/06-ladron
```

### Clase 3 · Mié 18 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-3/06-ladron`  ·  **Avance esperado:** 5 de 8 estados (62%)

![Diagrama de estados de Ladron](img/m06_ladron_6.svg)

| Archivo (en `Assets/_Alumnos/Ladron/`) | Qué debe tener |
| --- | --- |
| `LadronController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new LadronHuir()`. |
| `LadronHuir.cs` | **Nuevo.** `class LadronHuir : Estado`. Corre de regreso al inicio con la carga, esquivando guardias. Animación: `Sprint`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Huida con carga hacia Ruta.Inicio.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 3 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `LadronMerodear`, `LadronAcercarse`, `LadronMuerto`, `LadronRobar`, `LadronHuir`.

Todavía NO se pide: `LadronEscapar`, `LadronAturdido`, `LadronFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Ladron/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Ladron` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-3/06-ladron` y el PR se crea hoy (máximo el Mié 18 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-3/06-ladron
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Ladron
git commit -m "clase 3: Ladron - Huir"
git push -u origin clase-3/06-ladron
```

### Clase 4 · Lun 23 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-4/06-ladron`  ·  **Avance esperado:** 6 de 8 estados (75%)

![Diagrama de estados de Ladron](img/m06_ladron_7.svg)

| Archivo (en `Assets/_Alumnos/Ladron/`) | Qué debe tener |
| --- | --- |
| `LadronController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new LadronEscapar()`. |
| `LadronEscapar.cs` | **Nuevo.** `class LadronEscapar : Estado`. Llego al inicio con la carga: desaparece (las gemas se pierden). |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Desaparicion al escapar y reglas de la carga (perdida / devuelta).
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 3 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `LadronMerodear`, `LadronAcercarse`, `LadronMuerto`, `LadronRobar`, `LadronHuir`, `LadronEscapar`.

Todavía NO se pide: `LadronAturdido`, `LadronFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Ladron/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Ladron` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-4/06-ladron` y el PR se crea hoy (máximo el Lun 23 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-4/06-ladron
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Ladron
git commit -m "clase 4: Ladron - Escapar"
git push -u origin clase-4/06-ladron
```

### Clase 5 · Mié 25 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-5/06-ladron`  ·  **Avance esperado:** 8 de 8 estados (100%)

![Diagrama de estados de Ladron](img/m06_ladron_8.svg)

| Archivo (en `Assets/_Alumnos/Ladron/`) | Qué debe tener |
| --- | --- |
| `LadronController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new LadronAturdido()`, `new LadronFestejo()`. |
| `LadronAturdido.cs` | **Nuevo.** `class LadronAturdido : Estado`. Reaccion al daño (suelta el paso). Animación: `Dano1`. |
| `LadronFestejo.cs` | **Nuevo.** `class LadronFestejo : Estado`. Global. Animación: `Celebrar1`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Reacciones y festejo.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 3 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `LadronMerodear`, `LadronAcercarse`, `LadronMuerto`, `LadronRobar`, `LadronHuir`, `LadronEscapar`, `LadronAturdido`, `LadronFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Ladron/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Ladron` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-5/06-ladron` y el PR se crea hoy (máximo el Mié 25 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-5/06-ladron
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Ladron
git commit -m "clase 5: Ladron - Aturdido, Festejo"
git push -u origin clase-5/06-ladron
```

### Clase 6 · Lun 30 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-6/06-ladron`  ·  **Avance esperado:** 8 de 8 estados (100%)

![Diagrama de estados de Ladron](img/m06_ladron_9.svg)

| Archivo (en `Assets/_Alumnos/Ladron/`) | Qué debe tener |
| --- | --- |
| `LadronController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: ninguno nuevo (solo ajustes). |
| (todos tus archivos) | Pulido final: ver la lista de comprobación de abajo. No hay estados nuevos. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Pulido: Evade con guardias, cero basura, demo.
- Una oleada completa de principio a fin sin errores ni avisos en la consola.
- Profiler: `GC Alloc` en 0 B dentro de tus `Actualizar` (medido en Play).
- Pooling correcto: el segundo agente que reaparece se comporta igual que el primero (todo se reinicia en `OnEnable`/`Entrar`).
- Demo en vivo de 3 minutos mostrando cada estado y cada transición de tu diagrama.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `LadronMerodear`, `LadronAcercarse`, `LadronMuerto`, `LadronRobar`, `LadronHuir`, `LadronEscapar`, `LadronAturdido`, `LadronFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Ladron/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Ladron` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-6/06-ladron` y el PR se crea hoy (máximo el Lun 30 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-6/06-ladron
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Ladron
git commit -m "clase 6: Ladron - pulido final"
git push -u origin clase-6/06-ladron
```

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 6. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `Ladron<Estado>`.

```csharp
// LadronController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Ladron
{
public class LadronController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new LadronMerodear(), new LadronAcercarse(), new LadronRobar(), new LadronHuir(), new LadronEscapar(), new LadronAturdido(), new LadronFestejo(), new LadronMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("LadronMerodear");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("LadronMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// LadronMerodear.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.Ladron
{
public class LadronMerodear : Estado
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
