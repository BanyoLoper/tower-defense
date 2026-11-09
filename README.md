# Defender el Granero de Cristales

Proyecto del cuarto parcial: un juego de torres hecho en Unity (6000.3.10f1, URP) donde **cada alumno programa una maquina de estados** (y algunos, un steering behaviour). El instructor ya hizo todo lo visual; ustedes escriben solo codigo.


## Documentacion (empieza aqui)

Todo se lee directamente en GitHub (carpeta [`docs/`](docs/)). Hay una version HTML para la plataforma de la escuela en [`docs/html/`](docs/html/).

| Documento | Para que sirve |
|---|---|
| [Guia de inicio](docs/03_como_empezar.md) | **Leela primero**: como encajan las piezas, tu primera hora, flujo de Git y ejemplo completo |
| [Como subir tus cambios](docs/04_como_subir_cambios.md) | Paso a paso (consola y GitHub Desktop): rama, commit, push, PR y leer el reporte |
| [Troubleshooting](docs/05_troubleshooting.md) | **Si el pipeline falla**: cada mensaje del Action y el error de Git mas comun, con la solucion en consola |
| [Plan general](docs/00_indice_y_plan.md) | Pesos, reparto, calendario de las 5 clases + colchon, dependencias y rubrica |
| [Estado general](docs/01_estado_general.md) | `Estado`, `Contexto` y `Maquina` (la base de todas las maquinas) |
| [Steering behaviours](docs/02_steering_behaviors.md) | Los 9 behaviours con diagrama, firma y prueba de aceptacion |

## Reparto (cupos)

Cada alumno tiene su ficha con diagrama de estados, tablas, dependencias, entregas por clase y esqueleto de codigo. Cambia "Alumno NN" por tu nombre.

| Cupo | Alumno | Tu maquina (ficha) | Steering a cargo | Tu rama | Que entregar cada clase |
|---|---|---|---|---|---|
| 01 | Alumno 01 | [Soldado enemigo](docs/m01_soldado.md) | [PathFollowing](docs/02_steering_behaviors.md#path-following-seguir-camino-7-pts) | `clase-N/01-soldado` | [Entregas, archivos y comandos](docs/m01_soldado.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 02 | Alumno 02 | [Bruto (enemigo pesado)](docs/m02_bruto.md) | [Arrive](docs/02_steering_behaviors.md#arrive-llegar-4-pts) | `clase-N/02-bruto` | [Entregas, archivos y comandos](docs/m02_bruto.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 03 | Alumno 03 | [Jefe (final de la oleada 5)](docs/m03_jefe.md) | - | `clase-N/03-jefe` | [Entregas, archivos y comandos](docs/m03_jefe.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 04 | Alumno 04 | [Lancero enemigo](docs/m04_lancero.md) | [Seek](docs/02_steering_behaviors.md#seek-buscar-2-pts) | `clase-N/04-lancero` | [Entregas, archivos y comandos](docs/m04_lancero.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 05 | Alumno 05 | [Mago enemigo](docs/m05_magoenemigo.md) | - | `clase-N/05-magoenemigo` | [Entregas, archivos y comandos](docs/m05_magoenemigo.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 06 | Alumno 06 | [Ladron](docs/m06_ladron.md) | - | `clase-N/06-ladron` | [Entregas, archivos y comandos](docs/m06_ladron.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 07 | Alumno 07 | [Torre lanzadora](docs/m07_torrelanzadora.md) | [Hide](docs/02_steering_behaviors.md#hide-esconderse-8-pts) | `clase-N/07-torrelanzadora` | [Entregas, archivos y comandos](docs/m07_torrelanzadora.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 08 | Alumno 08 | [Torre maga](docs/m08_torremaga.md) | [ObstacleAvoidance](docs/02_steering_behaviors.md#obstacle-avoidance-evitar-obstaculos-10-pts) | `clase-N/08-torremaga` | [Entregas, archivos y comandos](docs/m08_torremaga.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 09 | Alumno 09 | [Guardia de la aldea](docs/m09_guardia.md) | [Flee](docs/02_steering_behaviors.md#flee-huir-2-pts) | `clase-N/09-guardia` | [Entregas, archivos y comandos](docs/m09_guardia.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 10 | Alumno 10 | [Aldeano](docs/m10_aldeano.md) | [Wander](docs/02_steering_behaviors.md#wander-vagar-6-pts) | `clase-N/10-aldeano` | [Entregas, archivos y comandos](docs/m10_aldeano.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 11 | Alumno 11 | [Minero](docs/m11_minero.md) | - | `clase-N/11-minero` | [Entregas, archivos y comandos](docs/m11_minero.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 12 | Alumno 12 | [Reparador](docs/m12_reparador.md) | [Evade](docs/02_steering_behaviors.md#evade-escapar-con-prediccion-6-pts) | `clase-N/12-reparador` | [Entregas, archivos y comandos](docs/m12_reparador.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 13 | Alumno 13 | [Director de oleadas (GameManager)](docs/m13_director.md) | - | `clase-N/13-director` | [Entregas, archivos y comandos](docs/m13_director.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |
| 14 | Alumno 14 | [Tutorial guiado](docs/m14_tutorial.md) | [Pursuit](docs/02_steering_behaviors.md#pursuit-perseguir-con-prediccion-6-pts) | `clase-N/14-tutorial` | [Entregas, archivos y comandos](docs/m14_tutorial.md#detalle-de-cada-entrega-qué-entregar-cómo-comprobarlo-y-qué-comandos-usar) |

> En la columna "Tu rama", `N` es el numero de clase (1 a 6): por ejemplo `clase-2/05-magoenemigo`.

## Calendario de entregas

Cinco entregas calificadas (clases 1 a 5) y una clase 6 de colchon. El PR se crea **ese mismo dia** (limite 23:59, hora CDMX). El lunes 16 de noviembre es puente: no hay clase.

| Clase | Dia | Rama | Que entregas | Como subirlo |
|---|---|---|---|---|
| 1 | Lun 9 nov | `clase-1/<cupo>-<maquina>` | **Onboarding**: recorres todo, Controller + estado inicial y tu primer commit/PR (paso a paso en tu ficha) | [Como subir cambios](docs/04_como_subir_cambios.md) |
| 2 | Mie 11 nov | `clase-2/<cupo>-<maquina>` | Estados de la clase 2 + steering de la clase 2 si te toca | [Como subir cambios](docs/04_como_subir_cambios.md) |
| 3 | Mie 18 nov | `clase-3/<cupo>-<maquina>` | Estados de la clase 3 + steering de la clase 3 si te toca | [Como subir cambios](docs/04_como_subir_cambios.md) |
| 4 | Lun 23 nov | `clase-4/<cupo>-<maquina>` | Estados de la clase 4 | [Como subir cambios](docs/04_como_subir_cambios.md) |
| 5 | Mie 25 nov | `clase-5/<cupo>-<maquina>` | Estados de la clase 5 (maquina completa) | [Como subir cambios](docs/04_como_subir_cambios.md) |
| 6 | Lun 30 nov | `clase-6/<cupo>-<maquina>` | **Colchon** (opcional, no se califica): corregir pendientes. Sin contenido nuevo | [Como subir cambios](docs/04_como_subir_cambios.md) |

Si algo falla en el pipeline: [Troubleshooting](docs/05_troubleshooting.md). Las fechas oficiales que usa el Action estan en [`.github/entregas/calendario.json`](.github/entregas/calendario.json).

## Donde va tu codigo

```
Assets/_Alumnos/<TuMaquina>/        <- tus estados y tu controller (ej. MagoEnemigo/)
Assets/_Alumnos/Steering/           <- solo tus archivos Steering<Nombre>.cs
```

- **Todo tu codigo va dentro de `namespace Alumnos.<TuMaquina>`** (ej. `namespace Alumnos.MagoEnemigo { ... }`). Excepcion: los archivos `Steering*.cs` **no** llevan namespace (`Steering` es una clase global parcial).
- Una clase por estado, en su propio archivo, con el prefijo del rol: `MagoEnemigoLanzar.cs` -> `class MagoEnemigoLanzar : Estado`. Ninguna clase puede llamarse igual que una del juego base (`Sensor`, `Agente`, `Juego`...): taparia la del instructor.
- Tu controller se llama `<Maquina>Controller` (ej. `MagoEnemigoController`), hereda de `MonoBehaviour` y registra todos tus estados. El juego lo encuentra solo por su nombre, aunque este dentro de tu namespace.
- **Solo puedes modificar archivos de tu carpeta.** Si Unity cambia otros archivos (escenas, ProjectSettings...), NO los subas (ver "Problemas comunes").
- Codigo en ASCII (sin acentos ni enes), sin LINQ y sin `new`/`GetComponent`/`Find` dentro de `Actualizar`.

## Reglas del repositorio

1. **Nadie sube directo a `main`.** Todo entra por Pull Request (PR).
2. Cada PR necesita la **aprobacion del instructor** (@BanyoLoper). Sin su aprobacion no se puede hacer merge.
3. Una rama y un PR **por clase**.
4. El PR se crea **el mismo dia de la clase**, a mas tardar en la fecha limite (ver [`.github/entregas/calendario.json`](.github/entregas/calendario.json)). Un PR creado despues se marca como entrega tardia.

### Nombre de la rama

```
clase-<N>/<cupo>-<maquina en minusculas>
```

| Ejemplo | Significa |
|---|---|
| `clase-1/05-magoenemigo` | clase 1, cupo 05 (MagoEnemigo) |
| `clase-3/10-aldeano` | clase 3, cupo 10 (Aldeano) |
| `clase-6/13-director` | clase 6, cupo 13 (Director) |

El nombre de la rama le dice a la revision automatica **quien eres y que debes entregar** esa clase. Si esta mal escrito, la revision falla.

## Revision automatica del PR

Al crear (o actualizar) tu PR, GitHub Actions deja un comentario con:

- ✅ lo que si cumple y ❌ lo que falta (estados de esa clase, controller, steering, archivos ajenos, fecha limite);
- ⚠️ avisos de calidad (ASCII, LINQ, basura en `Actualizar`, eventos sin desuscribir, falta de `.meta`).

Corrige y vuelve a hacer `push` a la misma rama: el comentario se actualiza solo. La nota de "funciona en el juego" la pone el instructor al probar en Unity.

> Guia completa con todos los comandos: [Como subir tus cambios](docs/04_como_subir_cambios.md). Si el Action marca un error: [Troubleshooting](docs/05_troubleshooting.md).

## Primera vez: clonar el repo

**Con consola (Git instalado):**

```bash
git clone https://github.com/BanyoLoper/tower-defense.git
cd tower-defense
git config user.name "Tu Nombre"
git config user.email "tu-correo@ejemplo.com"
```

Abre la carpeta con Unity Hub (Unity 6000.3.10f1). La primera vez tarda porque regenera `Library/` (no se sube a Git).

**Con GitHub Desktop:** `File > Clone repository > URL` y pega `https://github.com/BanyoLoper/tower-defense.git`. Elige una carpeta **con ruta corta** (ej. `C:\proyectos\`): las rutas largas rompen Unity en Windows.

## Cada clase: crear rama, trabajar y entregar

### Opcion A: consola

```bash
# 1. Actualiza main y crea tu rama de la clase
git checkout main
git pull
git checkout -b clase-2/05-magoenemigo

# 2. Trabaja en Unity/tu editor... y cuando quieras guardar avance:
git status                              # revisa QUE cambio (solo debe ser tu carpeta)
git add Assets/_Alumnos/MagoEnemigo/    # agrega SOLO tu carpeta (incluye los .meta)
git commit -m "MagoEnemigo: estados Posicionarse y Lanzar"

# 3. Sube tu rama (la primera vez con -u)
git push -u origin clase-2/05-magoenemigo
```

Despues entra a GitHub: aparece el boton **Compare & pull request**. Dale clic, llena la plantilla y crea el PR hacia `main`.

Para seguir subiendo avances a la misma clase: `git add ...`, `git commit ...`, `git push`.

### Opcion B: GitHub Desktop

1. `Current Branch > New Branch`, nombre: `clase-2/05-magoenemigo`, basada en `main` (antes haz `Fetch origin` y `Pull`).
2. Trabaja en Unity. En GitHub Desktop veras los cambios a la izquierda: **desmarca todo lo que no sea de tu carpeta** `Assets/_Alumnos/<TuMaquina>/`.
3. Escribe el resumen del commit y pulsa `Commit to clase-2/05-magoenemigo`.
4. `Publish branch` (la primera vez) o `Push origin`.
5. `Create Pull Request`: se abre GitHub con el PR hacia `main`. Llena la plantilla y crealo.

### Opcion C: solo desde la pagina de GitHub (archivos sueltos)

Util para corregir algo pequeno: abre el archivo en tu rama > icono de lapiz > *Commit changes* > *Commit directly to the clase-... branch*. No recomendado para trabajo grande (los `.meta` se te olvidan).

## Despues de entregar

- El instructor revisa el PR, prueba en Unity y lo **aprueba o te pide cambios**.
- Si te pide cambios: corrige en tu rama y haz `push`; el PR se actualiza solo.
- Cuando se aprueba, el instructor hace el merge. Para la siguiente clase: `git checkout main`, `git pull` y crea la nueva rama desde ahi (otro nombre `clase-3/...`).

## Problemas comunes

> Para cualquier mensaje del Action (ramas, archivos ajenos, estados que faltan, namespace...) y errores de Git, ve a [**Troubleshooting**](docs/05_troubleshooting.md): esta ordenado por el mensaje exacto y todo se resuelve por consola.

| Problema | Solucion |
|---|---|
| El PR muestra archivos que no son mios (escenas, `ProjectSettings`...) | `git restore <archivo>` (o en GitHub Desktop: clic derecho > *Discard changes*), luego commit y push |
| `git push` dice *rejected* / *protected branch* | Estas en `main`. Crea tu rama: `git checkout -b clase-N/...` y vuelve a hacer commit/push |
| "Mi rama esta atras de main" / conflictos | `git checkout main && git pull && git checkout mi-rama && git merge main`. Como solo tocas tu carpeta, casi no hay conflictos |
| Unity tarda o da errores raros al abrir | Cierra Unity, borra la carpeta `Library/` y vuelve a abrirlo |
| Falta un `.meta` | Abre el proyecto en Unity (lo genera) y haz `git add` del `.meta` de tus archivos |
| Escribi mal el nombre de la rama | `git branch -m clase-2/05-magoenemigo` (renombra) y `git push -u origin clase-2/05-magoenemigo`; cierra el PR anterior |

## Para el instructor

Ver [`INSTRUCTOR.md`](INSTRUCTOR.md): configuracion del repo, regla de proteccion de `main`, fechas limite y como revisar todas las entregas de una clase.
