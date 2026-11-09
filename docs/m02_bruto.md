# 02 · Bruto (enemigo pesado)

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Como subir cambios](04_como_subir_cambios.md) · [Troubleshooting](05_troubleshooting.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Media**  **Enemigo**

Enemigo lento y resistente. Sus golpes son lentos pero muy fuertes: se prepara, golpea y queda vulnerable. Al bajar de 40% de vida entra en furia (más rápido, una sola vez).

Peso de la máquina: **25.0 pts** · Steering a cargo: **4 pts** (Arrive (4 pts)) · Total del cupo 02: **29.0 pts** · Estados: **8** · Transiciones: **9**

## 1. Diagrama de estados

![Diagrama de estados de Bruto (enemigo pesado)](img/m02_bruto_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

> Documentos de apoyo: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md) · [Guía de inicio](03_como_empezar.md).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main; git pull; git checkout -b clase-1/02-bruto`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: oleadas 4 y 5.*
3. **Crea estos archivos** en `Assets/_Alumnos/Bruto/`: `BrutoController.cs`, `BrutoAvanzar.cs`, `BrutoMuerto.cs`.
4. **Copia el esqueleto** de la sección de código más abajo (controller + primer estado) y registra tus estados en `new Maquina(new List<Estado> {...})`.
5. **Primer estado:** en `Entrar` pide la animación (`Caminar`) y en `Actualizar` haz lo mínimo (mover con `Cuerpo.Mover`).
6. **Play:** en cuanto existe `BrutoController`, el sistema lo usa en lugar del demo. Si ves tu primer estado funcionando, sigue con el siguiente.
7. **Commit y PR** el mismo día (flujo en la [guía de inicio](03_como_empezar.md)).

Guía completa con diagramas: [guía de inicio](03_como_empezar.md).

## 2. Estados

| Estado (clase) | Qué hace | Animación | Steering |
| --- | --- | --- | --- |
| `BrutoAvanzar` | Camina la ruta hacia el granero. | Caminar | PathFollowing |
| `BrutoPerseguir` | Va hacia el hostil (lento, 1.4 m/s). | Caminar | Seek + ObstacleAvoidance |
| `BrutoCargarGolpe` | Se detiene y levanta el arma 0.7 s (aviso visual al jugador). | Atacar1 (inicio) | — |
| `BrutoGolpePesado` | Aplica 28 de daño en área chica frente a el. | Atacar2 | — |
| `BrutoRecuperar` | Queda vulnerable 1 s tras el golpe. | IdleCombate | — |
| `BrutoFuria` | Grito (Enojado) y velocidad x1.5 de ahí en adelante. Solo se entra una vez (flag en Ctx). | Enojado | — |
| `BrutoFestejo` (global) | Global: fin de partida. | Celebrar1 | — |
| `BrutoMuerto` (global) | Global: muerte + Reserva. | Muerte1 | — |

## 3. Transiciones

| Desde | Hacia | Condición |
| --- | --- | --- |
| Avanzar | Perseguir | hostil a &lt;= 6 m |
| Perseguir | Avanzar | objetivo perdido |
| Perseguir | CargarGolpe | dist &lt;= 2.2 |
| CargarGolpe | GolpePesado | 0.7 s |
| GolpePesado | Recuperar | golpe aplicado |
| Recuperar | Perseguir | 1 s y hay objetivo |
| Avanzar | Furia | vida &lt; 40% (1 vez) |
| Recuperar | Furia | vida &lt; 40% |
| Furia | Perseguir | 1.2 s |
| *cualquiera* | Muerto | la vida llega a 0 (`Salud.AlMorir`) |
| *cualquiera* | Festejo | fin de partida (`Juego.AlCambiarFase`) |

**Eventos que escucha:** `dano`, `muerte`, `fin_juego`, `hostil_visto`, `vida_baja (Salud.Porcentaje)`

## 4. Dependencias

### Mapa de dependencias

![Dependencias de Bruto (enemigo pesado)](img/m02_bruto_2.svg)

### 4.1 API base (ya hecha por el instructor)

| Clase | Qué usas |
| --- | --- |
| `Sensor` | HostilMasCercano / Contar, cada 0.2 s |
| `Cuerpo` | Mover / Mirar / Detener |
| `Agente` | Animar(Anim.X), Hostiles, DistanciaBorde |
| `Salud` | eventos AlDanar / AlMorir |
| `Reserva` | devolver el objeto al morir (pooling) |
| `Ruta` | Ruta.Puntos |
| `Juego` | Juego.I.Fase |

### 4.2 Comunicación con otras máquinas

| Con quién | Sentido | Cómo se comunican |
| --- | --- | --- |
| Director | recibe de | Lo crea desde la oleada 4. |
| Jefe | ambos | El Jefe reutiliza parte de esta máquina (ControladorAuto usa BrutoController si no hay JefeController). |

> Regla de oro: las máquinas **no se llaman entre sí**. Se comunican solo por la API base y sus eventos (`Agente.AlMorirAlguien`, `Juego.AlCambiarFase`, `Alertas.AlAlerta`, `Economia`, `Sensor`...). Así nadie queda bloqueado esperando el código de un compañero.

### 4.3 Steering behaviours que usa (los programa otro compañero)

| Behaviour | Responsable | Se entrega en | Mientras tanto |
| --- | --- | --- | --- |
| [`Steering.PathFollowing`](02_steering_behaviors.md#path-following-seguir-camino-7-pts) | Cupo 01 (Soldado) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.Seek`](02_steering_behaviors.md#seek-buscar-2-pts) | Cupo 04 (Lancero) | Clase 1 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |
| [`Steering.ObstacleAvoidance`](02_steering_behaviors.md#obstacle-avoidance-evitar-obstaculos-10-pts) | Cupo 08 (TorreMaga) | Clase 4 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

El archivo `Steering.cs` ya existe como clase parcial vacía: cada responsable agrega SU archivo `Steering<Nombre>.cs`. Tu máquina solo llama a `Steering.<Nombre>(...)` cuando exista.

## 5. Steering que debes programar tú (cupo 02)

| Behaviour | Pts | Entrega | Lo usan |
| --- | --- | --- | --- |
| [Arrive (llegar)](02_steering_behaviors.md#arrive-llegar-4-pts) | 4 | Clase 2 | Lancero, MagoEnemigo, Ladron, Guardia, Aldeano, Minero, Reparador |

### Cómo probar tu steering aislado

Crea en tu carpeta de máquina un script de prueba (no se usa en el juego final) y ponlo en un objeto con un `Cuerpo` sobre el NavMesh de la escena:

```csharp
using UnityEngine;

namespace Alumnos.Bruto   // el namespace de tu MAQUINA (el probe no es parte de Steering)
{
public class ProbeArrive : MonoBehaviour   // nombre unico: Probe + tu steering
{
    public Transform objetivo;
    public float vMax = 3f;
    Cuerpo cuerpo;
    void Awake() { cuerpo = GetComponent<Cuerpo>(); }
    void Update()
    {
        Vector3 v = Steering.Arrive(transform.position, objetivo.position, vMax);   // ajusta los parametros a tu firma
        cuerpo.Mover(v, Time.deltaTime);
    }
}
}
```

## 6. Entregas por clase

![Linea de tiempo de entregas](img/m02_bruto_3.svg)

Azul: se entrega ese día. Celeste: se mantiene funcionando en las clases siguientes (se vuelve a revisar).

| Clase y día | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** Lun 9 nov | Avanzar, Muerto | 2/8 · 25% | Esqueleto y camino por la ruta; muerte con Reserva. | — |
| **2** Mié 11 nov | Perseguir | 3/8 · 38% | Deteccion y persecución lenta con Seek. | Arrive |
| **3** Mié 18 nov | CargarGolpe, GolpePesado | 5/8 · 62% | Ciclo de golpe pesado con tiempos (windup 0.7 s) y daño 28. | — |
| **4** Lun 23 nov | Recuperar | 6/8 · 75% | Ventana de vulnerabilidad; el ciclo completo ataca-descansa-ataca funciona. | — |
| **5** Mié 25 nov | Furia, Festejo | 8/8 · 100% | Furia una sola vez (flag en Ctx) y festejo de fin de partida. | — |
| **6** Lun 30 nov | (pulido) | 8/8 · 100% | Pulido: ObstacleAvoidance integrado, cero basura, demo. | — |

Guías: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md).

### Detalle de cada entrega (qué entregar, cómo comprobarlo y qué comandos usar)

> Antes de tu primera entrega lee **[Cómo subir tus cambios](04_como_subir_cambios.md)**. Si el pipeline marca algo en rojo: **[Troubleshooting (si el pipeline falla)](05_troubleshooting.md)**. Todas las fechas son el día de clase; el lunes 16 de noviembre es puente (no hay clase).

### Clase 1 · Lun 9 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-1/02-bruto`  ·  **Avance esperado:** 2 de 8 estados (25%)

![Diagrama de estados de Bruto (enemigo pesado)](img/m02_bruto_4.svg)

| Archivo (en `Assets/_Alumnos/Bruto/`) | Qué debe tener |
| --- | --- |
| `BrutoController.cs` | **Nuevo.** `class BrutoController : MonoBehaviour` dentro de `namespace Alumnos.Bruto`. Crea el `Contexto`, la `Maquina` con tus estados e inicia en `Avanzar`. |
| `BrutoAvanzar.cs` | **Nuevo.** `class BrutoAvanzar : Estado`. Camina la ruta hacia el granero. Animación: `Caminar`. |
| `BrutoMuerto.cs` | **Nuevo.** `class BrutoMuerto : Estado`. Global: muerte + Reserva. Animación: `Muerte1`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Esqueleto y camino por la ruta; muerte con Reserva.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 4 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `BrutoAvanzar`, `BrutoMuerto`.

Todavía NO se pide: `BrutoPerseguir`, `BrutoCargarGolpe`, `BrutoGolpePesado`, `BrutoRecuperar`, `BrutoFuria`, `BrutoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Bruto/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Bruto` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-1/02-bruto` y el PR se crea hoy (máximo el Lun 9 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-1/02-bruto
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Bruto
git commit -m "clase 1: Bruto - Avanzar, Muerto"
git push -u origin clase-1/02-bruto
```

### Clase 2 · Mié 11 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-2/02-bruto`  ·  **Avance esperado:** 3 de 8 estados (38%)  ·  **Steering:** Arrive

![Diagrama de estados de Bruto (enemigo pesado)](img/m02_bruto_5.svg)

| Archivo (en `Assets/_Alumnos/Bruto/`) | Qué debe tener |
| --- | --- |
| `BrutoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new BrutoPerseguir()`. |
| `BrutoPerseguir.cs` | **Nuevo.** `class BrutoPerseguir : Estado`. Va hacia el hostil (lento, 1.4 m/s). Animación: `Caminar`. |
| `../Steering/SteeringArrive.cs` | **Nuevo.** `public static partial class Steering` (SIN namespace) con `public static Vector3 Arrive(Vector3 pos, Vector3 destino, float vMax, float radioFrenado)`. Prueba: El agente se detiene en el punto sin vibrar (a menos de 0.1 m) y sin oscilar. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Deteccion y persecución lenta con Seek.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 4 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `BrutoAvanzar`, `BrutoMuerto`, `BrutoPerseguir` y las del steering de esta clase.

Todavía NO se pide: `BrutoCargarGolpe`, `BrutoGolpePesado`, `BrutoRecuperar`, `BrutoFuria`, `BrutoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Bruto/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Bruto` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-2/02-bruto` y el PR se crea hoy (máximo el Mié 11 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-2/02-bruto
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Bruto  Assets/_Alumnos/Steering
git commit -m "clase 2: Bruto - Perseguir"
git push -u origin clase-2/02-bruto
```

### Clase 3 · Mié 18 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-3/02-bruto`  ·  **Avance esperado:** 5 de 8 estados (62%)

![Diagrama de estados de Bruto (enemigo pesado)](img/m02_bruto_6.svg)

| Archivo (en `Assets/_Alumnos/Bruto/`) | Qué debe tener |
| --- | --- |
| `BrutoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new BrutoCargarGolpe()`, `new BrutoGolpePesado()`. |
| `BrutoCargarGolpe.cs` | **Nuevo.** `class BrutoCargarGolpe : Estado`. Se detiene y levanta el arma 0.7 s (aviso visual al jugador). Animación: `Atacar1 (inicio)`. |
| `BrutoGolpePesado.cs` | **Nuevo.** `class BrutoGolpePesado : Estado`. Aplica 28 de daño en área chica frente a el. Animación: `Atacar2`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Ciclo de golpe pesado con tiempos (windup 0.7 s) y daño 28.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 4 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `BrutoAvanzar`, `BrutoMuerto`, `BrutoPerseguir`, `BrutoCargarGolpe`, `BrutoGolpePesado`.

Todavía NO se pide: `BrutoRecuperar`, `BrutoFuria`, `BrutoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Bruto/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Bruto` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-3/02-bruto` y el PR se crea hoy (máximo el Mié 18 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-3/02-bruto
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Bruto
git commit -m "clase 3: Bruto - CargarGolpe, GolpePesado"
git push -u origin clase-3/02-bruto
```

### Clase 4 · Lun 23 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-4/02-bruto`  ·  **Avance esperado:** 6 de 8 estados (75%)

![Diagrama de estados de Bruto (enemigo pesado)](img/m02_bruto_7.svg)

| Archivo (en `Assets/_Alumnos/Bruto/`) | Qué debe tener |
| --- | --- |
| `BrutoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new BrutoRecuperar()`. |
| `BrutoRecuperar.cs` | **Nuevo.** `class BrutoRecuperar : Estado`. Queda vulnerable 1 s tras el golpe. Animación: `IdleCombate`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Ventana de vulnerabilidad; el ciclo completo ataca-descansa-ataca funciona.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 4 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `BrutoAvanzar`, `BrutoMuerto`, `BrutoPerseguir`, `BrutoCargarGolpe`, `BrutoGolpePesado`, `BrutoRecuperar`.

Todavía NO se pide: `BrutoFuria`, `BrutoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Bruto/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Bruto` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-4/02-bruto` y el PR se crea hoy (máximo el Lun 23 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-4/02-bruto
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Bruto
git commit -m "clase 4: Bruto - Recuperar"
git push -u origin clase-4/02-bruto
```

### Clase 5 · Mié 25 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-5/02-bruto`  ·  **Avance esperado:** 8 de 8 estados (100%)

![Diagrama de estados de Bruto (enemigo pesado)](img/m02_bruto_8.svg)

| Archivo (en `Assets/_Alumnos/Bruto/`) | Qué debe tener |
| --- | --- |
| `BrutoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new BrutoFuria()`, `new BrutoFestejo()`. |
| `BrutoFuria.cs` | **Nuevo.** `class BrutoFuria : Estado`. Grito (Enojado) y velocidad x1.5 de ahí en adelante. Solo se entra una vez (flag en Ctx). Animación: `Enojado`. |
| `BrutoFestejo.cs` | **Nuevo.** `class BrutoFestejo : Estado`. Global: fin de partida. Animación: `Celebrar1`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Furia una sola vez (flag en Ctx) y festejo de fin de partida.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: oleadas 4 y 5.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `BrutoAvanzar`, `BrutoMuerto`, `BrutoPerseguir`, `BrutoCargarGolpe`, `BrutoGolpePesado`, `BrutoRecuperar`, `BrutoFuria`, `BrutoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Bruto/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Bruto` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-5/02-bruto` y el PR se crea hoy (máximo el Mié 25 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-5/02-bruto
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Bruto
git commit -m "clase 5: Bruto - Furia, Festejo"
git push -u origin clase-5/02-bruto
```

### Clase 6 · Lun 30 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-6/02-bruto`  ·  **Avance esperado:** 8 de 8 estados (100%)

![Diagrama de estados de Bruto (enemigo pesado)](img/m02_bruto_9.svg)

| Archivo (en `Assets/_Alumnos/Bruto/`) | Qué debe tener |
| --- | --- |
| `BrutoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: ninguno nuevo (solo ajustes). |
| (todos tus archivos) | Pulido final: ver la lista de comprobación de abajo. No hay estados nuevos. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Pulido: ObstacleAvoidance integrado, cero basura, demo.
- Una oleada completa de principio a fin sin errores ni avisos en la consola.
- Profiler: `GC Alloc` en 0 B dentro de tus `Actualizar` (medido en Play).
- Pooling correcto: el segundo agente que reaparece se comporta igual que el primero (todo se reinicia en `OnEnable`/`Entrar`).
- Demo en vivo de 3 minutos mostrando cada estado y cada transición de tu diagrama.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `BrutoAvanzar`, `BrutoMuerto`, `BrutoPerseguir`, `BrutoCargarGolpe`, `BrutoGolpePesado`, `BrutoRecuperar`, `BrutoFuria`, `BrutoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Bruto/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Bruto` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-6/02-bruto` y el PR se crea hoy (máximo el Lun 30 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-6/02-bruto
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Bruto
git commit -m "clase 6: Bruto - pulido final"
git push -u origin clase-6/02-bruto
```

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 6 entregas (cada clase pesa igual). |

## 7. Esqueleto de código

Punto de partida. Copia el patrón del `SoldadoController`/estados de referencia; el resto de estados siguen la misma forma. Nombres de clase únicos: `Bruto<Estado>`.

```csharp
// BrutoController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Bruto
{
public class BrutoController : MonoBehaviour
{
    Maquina maq;
    readonly Contexto ctx = new Contexto();
    Agente ag;

    void Awake()
    {
        ag = GetComponent<Agente>();
        ctx.Fijar("agente", ag);
        ctx.Fijar("ctrl", this);
        maq = new Maquina(new List<Estado> { new BrutoAvanzar(), new BrutoPerseguir(), new BrutoCargarGolpe(), new BrutoGolpePesado(), new BrutoRecuperar(), new BrutoFuria(), new BrutoFestejo(), new BrutoMuerto() }, ctx);
    }

    void OnEnable()   // OJO: los agentes vienen de la Reserva (se reutilizan): reinicia todo aqui
    {
        maq.Iniciar("BrutoAvanzar");
        ag.salud.AlMorir += AlMorir;
    }

    void OnDisable()
    {
        ag.salud.AlMorir -= AlMorir;   // siempre desuscribirse
    }

    void AlMorir(GameObject origen) { maq.CambiarA("BrutoMuerto"); }

    void Update() { maq.Actualizar(Time.deltaTime); }
}
}

// BrutoAvanzar.cs  (un archivo por estado; mismo namespace y prefijo del rol)
using UnityEngine;

namespace Alumnos.Bruto
{
public class BrutoAvanzar : Estado
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
