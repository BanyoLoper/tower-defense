# 07 · Torre lanzadora

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Como subir cambios](04_como_subir_cambios.md) · [Troubleshooting](05_troubleshooting.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Ligera**  **Defensa**

Torre de lanzas: vigila, apunta con anticipacion (le gana al objetivo que se mueve), dispara y recarga. Cuando la danan mucho dispara más lento y echa humo.

Peso de la máquina: **19.5 pts** · Steering a cargo: **8 pts** (Hide (8 pts)) · Total del cupo 07: **27.5 pts** · Estados: **6** · Transiciones: **9**

## 1. Diagrama de estados

![Diagrama de estados de Torre lanzadora](img/m07_torrelanzadora_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

> Documentos de apoyo: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md) · [Guía de inicio](03_como_empezar.md).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main; git pull; git checkout -b clase-1/07-torrelanzadora`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: la construyes tu: elige plataforma y pulsa 1.*
3. **Crea estos archivos** en `Assets/_Alumnos/TorreLanzadora/`: `TorreLanzadoraController.cs`, `TorreLanzadoraVigilando.cs`.
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

| Clase y día | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** Lun 9 nov | Vigilando | 1/6 · 17% | Onboarding y primer commit: el Controller crea la Maquina e inicia en Vigilando (puede ser un esqueleto que solo escribe un mensaje en consola). Compila, se ve en Play y el PR pasa la revision. | — |
| **2** Mié 11 nov | Destruida, Apuntando | 3/6 · 50% | Destruida: Global: Fx.Explosion, libera la plataforma (Reserva.Devolver). Apuntando: Gira hacia el punto futuro del objetivo. | — |
| **3** Mié 18 nov | Disparando, Recargando | 5/6 · 83% | Disparando: Lanza la lanza (ProyectilBase.Lanzar) a mitad de la animación. Recargando: Cadencia 1.3 s. | — |
| **4** Lun 23 nov | (colch&oacute;n) | 5/6 · 83% | Sin estados nuevos: terminas el steering de esta clase y ajustas lo anterior. | Hide |
| **5** Mié 25 nov | Danada | 6/6 · 100% | Danada: Vida &lt; 40%: cadencia x2 más lenta + Fx.Fuego como humo. Sale al curarse > 60%. Ademas: Prediccion de puntería (Pursuit) y ajuste de daño/alcance. | — |
| **6** Lun 30 nov | (colch&oacute;n) | 6/6 · 100% | Colchon (opcional): se usa solo para corregir pendientes o para ponerse al corriente. No hay estados nuevos. | — |

Guías: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md).

### Detalle de cada entrega (qué entregar, cómo comprobarlo y qué comandos usar)

> Antes de tu primera entrega lee **[Cómo subir tus cambios](04_como_subir_cambios.md)**. Si el pipeline marca algo en rojo: **[Troubleshooting (si el pipeline falla)](05_troubleshooting.md)**. Todas las fechas son el día de clase; el lunes 16 de noviembre es puente (no hay clase).

### Clase 1 · Lun 9 nov · Onboarding y primer commit · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-1/07-torrelanzadora`  ·  **Avance esperado:** 1 de 6 estados (17%)

![Diagrama de estados de Torre lanzadora](img/m07_torrelanzadora_4.svg)

#### Onboarding (2 horas): recorre todo y sube tu primer commit

![Linea de tiempo del onboarding de 2 horas](img/m07_torrelanzadora_5.svg)

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
git checkout -b clase-1/07-torrelanzadora
```

Es la rama de hoy; el nombre debe ser exacto.

##### 5 · Controller + estado inicial (60-95 min)

Crea los dos archivos de abajo en `Assets/_Alumnos/TorreLanzadora/` (Unity genera los `.meta`). Es la versión mínima del esqueleto completo que aparece más abajo en esta ficha: solo registra `TorreLanzadoraVigilando`. Pulsa Play y, si hace falta, llama oleadas con ESPACIO hasta que aparezca tu agente (la construyes tu: elige plataforma y pulsa 1): la consola debe mostrar *TorreLanzadora: entre a Vigilando*.

##### 6 · Primer commit y push (95-110 min)

```bash
git status
git add Assets/_Alumnos/TorreLanzadora
git commit -m "clase 1: TorreLanzadora - controller y Vigilando"
git push -u origin clase-1/07-torrelanzadora
```

`git status` debe mostrar solo tu carpeta (y los `.meta`).

##### 7 · Pull Request y reporte (110-120 min)

```bash
gh pr create --base main --title "Clase 1 - TorreLanzadora (cupo 07)" --body "Onboarding: controller y Vigilando"
gh pr view --comments
```

Sin la CLI, crea el PR en la web ([Cómo subir tus cambios](04_como_subir_cambios.md)). En 1-2 minutos el Action comenta; si hay ❌, corrige y haz `git push`.

**Código del paso 5** (copia y pega; los nombres ya son los de tu máquina):

```csharp
// Assets/_Alumnos/TorreLanzadora/TorreLanzadoraController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new TorreLanzadoraVigilando() }, ctx);   // por ahora solo este estado
        }

        void OnEnable() { maq.Iniciar("TorreLanzadoraVigilando"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}

// Assets/_Alumnos/TorreLanzadora/TorreLanzadoraVigilando.cs
using UnityEngine;

namespace Alumnos.TorreLanzadora
{
    public class TorreLanzadoraVigilando : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("TorreLanzadora: entre a Vigilando");   // prueba de vida: debe salir en la consola al hacer Play
        }

        public override string Actualizar(float dt) { return null; }   // null = quedarse en este estado

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}
```

¿Terminaste antes? Es opcional y no se califica hoy: empieza el siguiente estado (la clase 2 trae `TorreLanzadoraDestruida`, `TorreLanzadoraApuntando`).

| Archivo (en `Assets/_Alumnos/TorreLanzadora/`) | Qué debe tener |
| --- | --- |
| `TorreLanzadoraController.cs` | **Nuevo.** `class TorreLanzadoraController : MonoBehaviour` dentro de `namespace Alumnos.TorreLanzadora`. Crea el `Contexto`, la `Maquina` con tus estados e inicia en `Vigilando`. |
| `TorreLanzadoraVigilando.cs` | **Nuevo.** `class TorreLanzadoraVigilando : Estado`. Idle; revisa el Sensor cada 0.2 s (alcance 14 m). Animación: `IdleCombate`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Onboarding y primer commit: el Controller crea la Maquina e inicia en Vigilando (puede ser un esqueleto que solo escribe un mensaje en consola). Compila, se ve en Play y el PR pasa la revision.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: la construyes tu: elige plataforma y pulsa 1.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `TorreLanzadoraVigilando`.

Todavía NO se pide: `TorreLanzadoraApuntando`, `TorreLanzadoraDisparando`, `TorreLanzadoraRecargando`, `TorreLanzadoraDanada`, `TorreLanzadoraDestruida`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/TorreLanzadora/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.TorreLanzadora` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-1/07-torrelanzadora` y el PR se crea hoy (máximo el Lun 9 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-1/07-torrelanzadora
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/TorreLanzadora
git commit -m "clase 1: TorreLanzadora - Vigilando"
git push -u origin clase-1/07-torrelanzadora
```

### Clase 2 · Mié 11 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-2/07-torrelanzadora`  ·  **Avance esperado:** 3 de 6 estados (50%)

![Diagrama de estados de Torre lanzadora](img/m07_torrelanzadora_6.svg)

| Archivo (en `Assets/_Alumnos/TorreLanzadora/`) | Qué debe tener |
| --- | --- |
| `TorreLanzadoraController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new TorreLanzadoraDestruida()`, `new TorreLanzadoraApuntando()`. |
| `TorreLanzadoraDestruida.cs` | **Nuevo.** `class TorreLanzadoraDestruida : Estado`. Global: Fx.Explosion, libera la plataforma (Reserva.Devolver). |
| `TorreLanzadoraApuntando.cs` | **Nuevo.** `class TorreLanzadoraApuntando : Estado`. Gira hacia el punto futuro del objetivo. Animación: `IdleCombate`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Destruida: Global: Fx.Explosion, libera la plataforma (Reserva.Devolver). Apuntando: Gira hacia el punto futuro del objetivo.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: la construyes tu: elige plataforma y pulsa 1.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `TorreLanzadoraVigilando`, `TorreLanzadoraDestruida`, `TorreLanzadoraApuntando`.

Todavía NO se pide: `TorreLanzadoraDisparando`, `TorreLanzadoraRecargando`, `TorreLanzadoraDanada`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/TorreLanzadora/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.TorreLanzadora` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-2/07-torrelanzadora` y el PR se crea hoy (máximo el Mié 11 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-2/07-torrelanzadora
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/TorreLanzadora
git commit -m "clase 2: TorreLanzadora - Destruida, Apuntando"
git push -u origin clase-2/07-torrelanzadora
```

### Clase 3 · Mié 18 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-3/07-torrelanzadora`  ·  **Avance esperado:** 5 de 6 estados (83%)

![Diagrama de estados de Torre lanzadora](img/m07_torrelanzadora_7.svg)

| Archivo (en `Assets/_Alumnos/TorreLanzadora/`) | Qué debe tener |
| --- | --- |
| `TorreLanzadoraController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new TorreLanzadoraDisparando()`, `new TorreLanzadoraRecargando()`. |
| `TorreLanzadoraDisparando.cs` | **Nuevo.** `class TorreLanzadoraDisparando : Estado`. Lanza la lanza (ProyectilBase.Lanzar) a mitad de la animación. Animación: `Lanzar1 / Lanzar2`. |
| `TorreLanzadoraRecargando.cs` | **Nuevo.** `class TorreLanzadoraRecargando : Estado`. Cadencia 1.3 s. Animación: `Idle`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Disparando: Lanza la lanza (ProyectilBase.Lanzar) a mitad de la animación. Recargando: Cadencia 1.3 s.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: la construyes tu: elige plataforma y pulsa 1.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `TorreLanzadoraVigilando`, `TorreLanzadoraDestruida`, `TorreLanzadoraApuntando`, `TorreLanzadoraDisparando`, `TorreLanzadoraRecargando`.

Todavía NO se pide: `TorreLanzadoraDanada`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/TorreLanzadora/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.TorreLanzadora` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-3/07-torrelanzadora` y el PR se crea hoy (máximo el Mié 18 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-3/07-torrelanzadora
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/TorreLanzadora
git commit -m "clase 3: TorreLanzadora - Disparando, Recargando"
git push -u origin clase-3/07-torrelanzadora
```

### Clase 4 · Lun 23 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-4/07-torrelanzadora`  ·  **Avance esperado:** 5 de 6 estados (83%)  ·  **Steering:** Hide

![Diagrama de estados de Torre lanzadora](img/m07_torrelanzadora_8.svg)

| Archivo (en `Assets/_Alumnos/TorreLanzadora/`) | Qué debe tener |
| --- | --- |
| `TorreLanzadoraController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: ninguno nuevo (solo ajustes). |
| `../Steering/SteeringHide.cs` | **Nuevo.** `public static partial class Steering` (SIN namespace) con `public static Vector3 Hide(Vector3 pos, Vector3 amenaza, Vector3[] escondites, float[] radios, float vMax, out Vector3 mejorPunto)`. Prueba: El aldeano termina siempre con una roca/casa entre el y el enemigo. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Sin estados nuevos: terminas el steering de esta clase y ajustas lo anterior.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: la construyes tu: elige plataforma y pulsa 1.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `TorreLanzadoraVigilando`, `TorreLanzadoraDestruida`, `TorreLanzadoraApuntando`, `TorreLanzadoraDisparando`, `TorreLanzadoraRecargando` y las del steering de esta clase.

Todavía NO se pide: `TorreLanzadoraDanada`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/TorreLanzadora/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.TorreLanzadora` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-4/07-torrelanzadora` y el PR se crea hoy (máximo el Lun 23 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-4/07-torrelanzadora
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/TorreLanzadora  Assets/_Alumnos/Steering
git commit -m "clase 4: TorreLanzadora - correcciones"
git push -u origin clase-4/07-torrelanzadora
```

### Clase 5 · Mié 25 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-5/07-torrelanzadora`  ·  **Avance esperado:** 6 de 6 estados (100%)

![Diagrama de estados de Torre lanzadora](img/m07_torrelanzadora_9.svg)

| Archivo (en `Assets/_Alumnos/TorreLanzadora/`) | Qué debe tener |
| --- | --- |
| `TorreLanzadoraController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new TorreLanzadoraDanada()`. |
| `TorreLanzadoraDanada.cs` | **Nuevo.** `class TorreLanzadoraDanada : Estado`. Vida &lt; 40%: cadencia x2 más lenta + Fx.Fuego como humo. Sale al curarse > 60%. Animación: `Idle`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Danada: Vida &lt; 40%: cadencia x2 más lenta + Fx.Fuego como humo. Sale al curarse > 60%. Ademas: Prediccion de puntería (Pursuit) y ajuste de daño/alcance.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y mira a tus agentes desde el inicio. Tu agente aparece: la construyes tu: elige plataforma y pulsa 1.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `TorreLanzadoraVigilando`, `TorreLanzadoraDestruida`, `TorreLanzadoraApuntando`, `TorreLanzadoraDisparando`, `TorreLanzadoraRecargando`, `TorreLanzadoraDanada`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/TorreLanzadora/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.TorreLanzadora` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-5/07-torrelanzadora` y el PR se crea hoy (máximo el Mié 25 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-5/07-torrelanzadora
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/TorreLanzadora
git commit -m "clase 5: TorreLanzadora - Danada"
git push -u origin clase-5/07-torrelanzadora
```

### Clase 6 · Lun 30 nov · Colchón (opcional, no se califica) · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-6/07-torrelanzadora`  ·  **Avance esperado:** 6 de 6 estados (100%)

![Diagrama de estados de Torre lanzadora](img/m07_torrelanzadora_10.svg)

| Archivo (en `Assets/_Alumnos/TorreLanzadora/`) | Qué debe tener |
| --- | --- |
| `TorreLanzadoraController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: ninguno nuevo (solo ajustes). |
| (ninguno obligatorio) | Colchón: sin contenido nuevo. Solo corriges lo que marcó el Action o terminas lo que se quedó atrás. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Esta clase **no tiene contenido nuevo y no cuenta para la calificación** (el promedio es de las clases 1 a 5).
- Úsala para: corregir los ❌ que dejó el Action, terminar estados atrasados y probar tu máquina junto con las otras.
- Si necesitas entregar correcciones, crea la rama `clase-6/07-torrelanzadora` y su PR; si no, no hace falta.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `TorreLanzadoraVigilando`, `TorreLanzadoraDestruida`, `TorreLanzadoraApuntando`, `TorreLanzadoraDisparando`, `TorreLanzadoraRecargando`, `TorreLanzadoraDanada`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/TorreLanzadora/` y de `Assets/_Alumnos/Steering/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.TorreLanzadora` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-6/07-torrelanzadora` y el PR se crea hoy (máximo el Lun 30 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-6/07-torrelanzadora
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/TorreLanzadora
git commit -m "clase 6: TorreLanzadora - correcciones"
git push -u origin clase-6/07-torrelanzadora
```

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 5 entregas de las clases 1 a 5 (cada una pesa igual). La clase 6 es colchón y no cuenta. |

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
