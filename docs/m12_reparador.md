# 12 · Reparador

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Como subir cambios](04_como_subir_cambios.md) · [Troubleshooting](05_troubleshooting.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Media**  **Civil**

Mantiene el granero: cuando el cristal esta dañado va al punto de reparacion y martilla (cura 4 de vida por segundo, y los cristales rotos reaparecen). Huye si hay enemigos cerca.

Peso de la máquina: **23.0 pts** · Steering a cargo: **6 pts** (Evade (6 pts)) · Total del cupo 12: **29.0 pts** · Estados: **6** · Transiciones: **7**

## 1. Diagrama de estados

![Diagrama de estados de Reparador](img/m12_reparador_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

> Documentos de apoyo: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md) · [Guía de inicio](03_como_empezar.md).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main; git pull; git checkout -b clase-1/12-reparador`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: desde el inicio (nacen en la aldea).*
3. **Crea estos archivos** en `Assets/_Alumnos/Reparador/`: `ReparadorController.cs`, `ReparadorEsperar.cs`.
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
| [`Steering.Flee`](02_steering_behaviors.md#flee-huir-2-pts) | Cupo 09 (Guardia) | Clase 2 | usa `Cuerpo.Mover` con dirección directa hacia el objetivo |

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

| Clase y día | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** Lun 9 nov | Esperar | 1/6 · 17% | Onboarding y primer commit: el Controller crea la Maquina e inicia en Esperar (puede ser un esqueleto que solo escribe un mensaje en consola). Compila, se ve en Play y el PR pasa la revision. | — |
| **2** Mié 11 nov | Muerto | 2/6 · 33% | Muerto: Global. | — |
| **3** Mié 18 nov | IrAReparar | 3/6 · 50% | IrAReparar: Se acerca al punto de reparacion frenando suave. | Evade |
| **4** Lun 23 nov | Martillar, Huir | 5/6 · 83% | Martillar: Martilla: Salud.Curar(4*dt) al granero + Fx.Curacion cada 2 s. Huir: Se aleja del enemigo. | — |
| **5** Mié 25 nov | Festejo | 6/6 · 100% | Festejo: Global. | — |
| **6** Lun 30 nov | (colch&oacute;n) | 6/6 · 100% | Colchon (opcional): se usa solo para corregir pendientes o para ponerse al corriente. No hay estados nuevos. | — |

Guías: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md).

### Detalle de cada entrega (qué entregar, cómo comprobarlo y qué comandos usar)

> Antes de tu primera entrega lee **[Cómo subir tus cambios](04_como_subir_cambios.md)**. Si el pipeline marca algo en rojo: **[Troubleshooting (si el pipeline falla)](05_troubleshooting.md)**. Todas las fechas son el día de clase; el lunes 16 de noviembre es puente (no hay clase).

### Clase 1 · Lun 9 nov · Onboarding y primer commit · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-1/12-reparador`  ·  **Avance esperado:** 1 de 6 estados (17%)

![Diagrama de estados de Reparador](img/m12_reparador_4.svg)

#### Onboarding (2 horas): recorre todo y sube tu primer commit

![Linea de tiempo del onboarding de 2 horas](img/m12_reparador_5.svg)

> Hoy no se califica cuánto programaste, sino que **recorriste el flujo completo una vez**: clonar, correr el juego, leer tu ficha, crear tu rama, hacer que tu máquina arranque y llegar hasta un PR revisado. Todo lo demás se construye sobre esto.

##### 1 · Clonar y abrir (0-15 min)

```bash
git clone https://github.com/BanyoLoper/tower-defense.git td
cd td
git status
```

Abre la carpeta `td` con Unity `6000.3.10f1` (la primera vez tarda). Debe decir `On branch main` y *working tree clean*. Si falla: [Troubleshooting (si el pipeline falla)](05_troubleshooting.md).

##### 2 · Jugar la base (15-30 min)

Abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Pulsa ESPACIO para llamar oleadas; mira cómo aparecen enemigos y torres y recorre la Hierarchy. Así sabes qué hace el juego sin tu código.

##### 3 · Leer tu ficha y el Estado base (30-50 min)

Lee de esta ficha el diagrama de estados, *Qué es tu máquina* y las reglas. Abre `Assets/_Granero/Scripts/Maquina/` y mira `Estado.cs`, `Maquina.cs` y `Contexto.cs`; la explicación está en [Estado general](01_estado_general.md).

##### 4 · Crear tu rama (50-60 min)

```bash
git checkout main
git pull
git checkout -b clase-1/12-reparador
```

Es la rama de hoy; el nombre debe ser exacto.

##### 5 · Controller + estado inicial (60-95 min)

Crea los dos archivos de abajo en `Assets/_Alumnos/Reparador/` (Unity genera los `.meta`). Es la versión mínima del esqueleto completo que aparece más abajo en esta ficha: solo registra `ReparadorEsperar`. Pulsa Play y, si hace falta, llama oleadas con ESPACIO hasta que aparezca tu agente (desde el inicio (nacen en la aldea)): la consola debe mostrar *Reparador: entre a Esperar*.

##### 6 · Primer commit y push (95-110 min)

```bash
git status
git add Assets/_Alumnos/Reparador
git commit -m "clase 1: Reparador - controller y Esperar"
git push -u origin clase-1/12-reparador
```

`git status` debe mostrar solo tu carpeta (y los `.meta`).

##### 7 · Pull Request y reporte (110-120 min)

```bash
gh pr create --base main --title "Clase 1 - Reparador (cupo 12)" --body "Onboarding: controller y Esperar"
gh pr view --comments
```

Sin la CLI, crea el PR en la web ([Cómo subir tus cambios](04_como_subir_cambios.md)). En 1-2 minutos el Action comenta; si hay ❌, corrige y haz `git push`.

**Código del paso 5** (copia y pega; los nombres ya son los de tu máquina):

```csharp
// Assets/_Alumnos/Reparador/ReparadorController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new ReparadorEsperar() }, ctx);   // por ahora solo este estado
        }

        void OnEnable() { maq.Iniciar("ReparadorEsperar"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}

// Assets/_Alumnos/Reparador/ReparadorEsperar.cs
using UnityEngine;

namespace Alumnos.Reparador
{
    public class ReparadorEsperar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("Reparador: entre a Esperar");   // prueba de vida: debe salir en la consola al hacer Play
        }

        public override string Actualizar(float dt) { return null; }   // null = quedarse en este estado

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}
```

¿Terminaste antes? Es opcional y no se califica hoy: empieza el siguiente estado (la clase 2 trae `ReparadorMuerto`).

| Archivo (en `Assets/_Alumnos/Reparador/`) | Qué debe tener |
| --- | --- |
| `ReparadorController.cs` | **Nuevo.** `class ReparadorController : MonoBehaviour` dentro de `namespace Alumnos.Reparador`. Crea el `Contexto`, la `Maquina` con tus estados e inicia en `Esperar`. |
| `ReparadorEsperar.cs` | **Nuevo.** `class ReparadorEsperar : Estado`. Descansa cerca del punto de reparacion. Animación: `Idle`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Onboarding y primer commit: el Controller crea la Maquina e inicia en Esperar (puede ser un esqueleto que solo escribe un mensaje en consola). Compila, se ve en Play y el PR pasa la revision.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: desde el inicio (nacen en la aldea).

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `ReparadorEsperar`.

Todavía NO se pide: `ReparadorIrAReparar`, `ReparadorMartillar`, `ReparadorHuir`, `ReparadorFestejo`, `ReparadorMuerto`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Reparador/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Reparador` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-1/12-reparador` y el PR se crea hoy (máximo el Lun 9 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-1/12-reparador
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Reparador
git commit -m "clase 1: Reparador - Esperar"
git push -u origin clase-1/12-reparador
```

### Clase 2 · Mié 11 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-2/12-reparador`  ·  **Avance esperado:** 2 de 6 estados (33%)

![Diagrama de estados de Reparador](img/m12_reparador_6.svg)

| Archivo (en `Assets/_Alumnos/Reparador/`) | Qué debe tener |
| --- | --- |
| `ReparadorController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new ReparadorMuerto()`. |
| `ReparadorMuerto.cs` | **Nuevo.** `class ReparadorMuerto : Estado`. Global. Animación: `Muerte1`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Muerto: Global.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: desde el inicio (nacen en la aldea).

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `ReparadorEsperar`, `ReparadorMuerto`.

Todavía NO se pide: `ReparadorIrAReparar`, `ReparadorMartillar`, `ReparadorHuir`, `ReparadorFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Reparador/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Reparador` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-2/12-reparador` y el PR se crea hoy (máximo el Mié 11 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-2/12-reparador
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Reparador
git commit -m "clase 2: Reparador - Muerto"
git push -u origin clase-2/12-reparador
```

### Clase 3 · Mié 18 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-3/12-reparador`  ·  **Avance esperado:** 3 de 6 estados (50%)  ·  **Steering:** Evade

![Diagrama de estados de Reparador](img/m12_reparador_7.svg)

| Archivo (en `Assets/_Alumnos/Reparador/`) | Qué debe tener |
| --- | --- |
| `ReparadorController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new ReparadorIrAReparar()`. |
| `ReparadorIrAReparar.cs` | **Nuevo.** `class ReparadorIrAReparar : Estado`. Se acerca al punto de reparacion frenando suave. Animación: `Caminar`. |
| `../Steering/SteeringEvade.cs` | **Nuevo.** `public static partial class Steering` (SIN namespace) con `public static Vector3 Evade(Vector3 pos, Vector3 posPer, Vector3 velPer, float vMax)`. Prueba: Dura más tiempo sin ser alcanzado por un perseguidor con Pursuit que con Flee simple. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- IrAReparar: Se acerca al punto de reparacion frenando suave.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: desde el inicio (nacen en la aldea).

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `ReparadorEsperar`, `ReparadorMuerto`, `ReparadorIrAReparar` y las del steering de esta clase.

Todavía NO se pide: `ReparadorMartillar`, `ReparadorHuir`, `ReparadorFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Reparador/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Reparador` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-3/12-reparador` y el PR se crea hoy (máximo el Mié 18 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-3/12-reparador
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Reparador  Assets/_Alumnos/Steering
git commit -m "clase 3: Reparador - IrAReparar"
git push -u origin clase-3/12-reparador
```

### Clase 4 · Lun 23 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-4/12-reparador`  ·  **Avance esperado:** 5 de 6 estados (83%)

![Diagrama de estados de Reparador](img/m12_reparador_8.svg)

| Archivo (en `Assets/_Alumnos/Reparador/`) | Qué debe tener |
| --- | --- |
| `ReparadorController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new ReparadorMartillar()`, `new ReparadorHuir()`. |
| `ReparadorMartillar.cs` | **Nuevo.** `class ReparadorMartillar : Estado`. Martilla: Salud.Curar(4*dt) al granero + Fx.Curacion cada 2 s. Animación: `MartillarLoop`. |
| `ReparadorHuir.cs` | **Nuevo.** `class ReparadorHuir : Estado`. Se aleja del enemigo. Animación: `Correr`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Martillar: Martilla: Salud.Curar(4*dt) al granero + Fx.Curacion cada 2 s. Huir: Se aleja del enemigo.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: desde el inicio (nacen en la aldea).

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `ReparadorEsperar`, `ReparadorMuerto`, `ReparadorIrAReparar`, `ReparadorMartillar`, `ReparadorHuir`.

Todavía NO se pide: `ReparadorFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Reparador/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Reparador` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-4/12-reparador` y el PR se crea hoy (máximo el Lun 23 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-4/12-reparador
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Reparador
git commit -m "clase 4: Reparador - Martillar, Huir"
git push -u origin clase-4/12-reparador
```

### Clase 5 · Mié 25 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-5/12-reparador`  ·  **Avance esperado:** 6 de 6 estados (100%)

![Diagrama de estados de Reparador](img/m12_reparador_9.svg)

| Archivo (en `Assets/_Alumnos/Reparador/`) | Qué debe tener |
| --- | --- |
| `ReparadorController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new ReparadorFestejo()`. |
| `ReparadorFestejo.cs` | **Nuevo.** `class ReparadorFestejo : Estado`. Global. Animación: `Celebrar1`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Festejo: Global.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: desde el inicio (nacen en la aldea).

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `ReparadorEsperar`, `ReparadorMuerto`, `ReparadorIrAReparar`, `ReparadorMartillar`, `ReparadorHuir`, `ReparadorFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Reparador/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Reparador` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-5/12-reparador` y el PR se crea hoy (máximo el Mié 25 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-5/12-reparador
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Reparador
git commit -m "clase 5: Reparador - Festejo"
git push -u origin clase-5/12-reparador
```

### Clase 6 · Lun 30 nov · Colchón (opcional, no se califica) · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-6/12-reparador`  ·  **Avance esperado:** 6 de 6 estados (100%)

![Diagrama de estados de Reparador](img/m12_reparador_10.svg)

| Archivo (en `Assets/_Alumnos/Reparador/`) | Qué debe tener |
| --- | --- |
| `ReparadorController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: ninguno nuevo (solo ajustes). |
| (ninguno obligatorio) | Colchón: sin contenido nuevo. Solo corriges lo que marcó el Action o terminas lo que se quedó atrás. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Esta clase **no tiene contenido nuevo y no cuenta para la calificación** (el promedio es de las clases 1 a 5).
- Úsala para: corregir los ❌ que dejó el Action, terminar estados atrasados y probar tu máquina junto con las otras.
- Si necesitas entregar correcciones, crea la rama `clase-6/12-reparador` y su PR; si no, no hace falta.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `ReparadorEsperar`, `ReparadorMuerto`, `ReparadorIrAReparar`, `ReparadorMartillar`, `ReparadorHuir`, `ReparadorFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/Reparador/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.Reparador` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-6/12-reparador` y el PR se crea hoy (máximo el Lun 30 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-6/12-reparador
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/Reparador
git commit -m "clase 6: Reparador - correcciones"
git push -u origin clase-6/12-reparador
```

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 5 entregas de las clases 1 a 5 (cada una pesa igual). La clase 6 es colchón y no cuenta. |

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
