# 05 · Mago enemigo

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Como subir cambios](04_como_subir_cambios.md) · [Troubleshooting](05_troubleshooting.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

**Alta**  **Enemigo**

Lanza bolas de fuego desde lejos (9 m). Busca la distancia ideal, lanza, recarga y huye si lo alcanzan. Usa proyectiles con Reserva.

Peso de la máquina: **27.5 pts** · Steering a cargo: **0 pts** (ninguno) · Total del cupo 05: **27.5 pts** · Estados: **8** · Transiciones: **10**

## 1. Diagrama de estados

![Diagrama de estados de Mago enemigo](img/m05_magoenemigo_1.svg)

Las flechas se leen "desde → hasta" con la condición que dispara el cambio. El estado inicial es el de borde grueso. Cada estado devuelve el nombre del siguiente estado en `Actualizar` o `AlRecibir` (o `null` para quedarse).

> Documentos de apoyo: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md) · [Guía de inicio](03_como_empezar.md).

## Cómo empezar (primeros 30 minutos)

1. **Rama:** `git checkout main; git pull; git checkout -b clase-1/05-magoenemigo`
2. **Mira tu agente en el juego:** abre `Assets/_Granero/Escenas/Granero.unity` y pulsa Play. Observa cómo se mueve con el cerebro demo; tu máquina lo va a reemplazar. *Aparece: desde la oleada 3.*
3. **Crea estos archivos** en `Assets/_Alumnos/MagoEnemigo/`: `MagoEnemigoController.cs`, `MagoEnemigoAvanzar.cs`.
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

| Clase y día | Estados nuevos | Acumulado | Qué debe funcionar | Steering |
| --- | --- | --- | --- | --- |
| **1** Lun 9 nov | Avanzar | 1/8 · 12% | Onboarding y primer commit: el Controller crea la Maquina e inicia en Avanzar (puede ser un esqueleto que solo escribe un mensaje en consola). Compila, se ve en Play y el PR pasa la revision. | — |
| **2** Mié 11 nov | Muerto, Posicionarse | 3/8 · 38% | Muerto: Global. Posicionarse: Se coloca a ~9 m del objetivo y frena suave. | — |
| **3** Mié 18 nov | Lanzar, Recargar | 5/8 · 62% | Lanzar: Cast con proyectil: ProyectilBase.Lanzar(...) a mitad de la animación. Recargar: Espera 2.4 s entre hechizos. | — |
| **4** Lun 23 nov | Huir, Aturdido | 7/8 · 88% | Huir: Si un hostil esta a &lt; 3 m, se escapa 1.5 s anticipando su trayectoria. Aturdido: Reaccion al daño. | — |
| **5** Mié 25 nov | Festejo | 8/8 · 100% | Festejo: Global. | — |
| **6** Lun 30 nov | (colch&oacute;n) | 8/8 · 100% | Colchon (opcional): se usa solo para corregir pendientes o para ponerse al corriente. No hay estados nuevos. | — |

Guías: [Cómo subir tus cambios](04_como_subir_cambios.md) · [Troubleshooting (si el pipeline falla)](05_troubleshooting.md).

### Detalle de cada entrega (qué entregar, cómo comprobarlo y qué comandos usar)

> Antes de tu primera entrega lee **[Cómo subir tus cambios](04_como_subir_cambios.md)**. Si el pipeline marca algo en rojo: **[Troubleshooting (si el pipeline falla)](05_troubleshooting.md)**. Todas las fechas son el día de clase; el lunes 16 de noviembre es puente (no hay clase).

### Clase 1 · Lun 9 nov · Onboarding y primer commit · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-1/05-magoenemigo`  ·  **Avance esperado:** 1 de 8 estados (12%)

![Diagrama de estados de Mago enemigo](img/m05_magoenemigo_4.svg)

#### Onboarding (2 horas): recorre todo y sube tu primer commit

![Linea de tiempo del onboarding de 2 horas](img/m05_magoenemigo_5.svg)

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
git checkout -b clase-1/05-magoenemigo
```

Es la rama de hoy; el nombre debe ser exacto.

##### 5 · Controller + estado inicial (60-95 min)

Crea los dos archivos de abajo en `Assets/_Alumnos/MagoEnemigo/` (Unity genera los `.meta`). Es la versión mínima del esqueleto completo que aparece más abajo en esta ficha: solo registra `MagoEnemigoAvanzar`. Pulsa Play y, si hace falta, llama oleadas con ESPACIO hasta que aparezca tu agente (desde la oleada 3): la consola debe mostrar *MagoEnemigo: entre a Avanzar*.

##### 6 · Primer commit y push (95-110 min)

```bash
git status
git add Assets/_Alumnos/MagoEnemigo
git commit -m "clase 1: MagoEnemigo - controller y Avanzar"
git push -u origin clase-1/05-magoenemigo
```

`git status` debe mostrar solo tu carpeta (y los `.meta`).

##### 7 · Pull Request y reporte (110-120 min)

```bash
gh pr create --base main --title "Clase 1 - MagoEnemigo (cupo 05)" --body "Onboarding: controller y Avanzar"
gh pr view --comments
```

Sin la CLI, crea el PR en la web ([Cómo subir tus cambios](04_como_subir_cambios.md)). En 1-2 minutos el Action comenta; si hay ❌, corrige y haz `git push`.

**Código del paso 5** (copia y pega; los nombres ya son los de tu máquina):

```csharp
// Assets/_Alumnos/MagoEnemigo/MagoEnemigoController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoController : MonoBehaviour
    {
        Maquina maq;
        readonly Contexto ctx = new Contexto();

        void Awake()
        {
            ctx.Fijar("agente", GetComponent<Agente>());
            ctx.Fijar("ctrl", this);
            maq = new Maquina(new List<Estado> { new MagoEnemigoAvanzar() }, ctx);   // por ahora solo este estado
        }

        void OnEnable() { maq.Iniciar("MagoEnemigoAvanzar"); }
        void Update() { maq.Actualizar(Time.deltaTime); }
    }
}

// Assets/_Alumnos/MagoEnemigo/MagoEnemigoAvanzar.cs
using UnityEngine;

namespace Alumnos.MagoEnemigo
{
    public class MagoEnemigoAvanzar : Estado
    {
        public override void Entrar(string anterior)
        {
            Debug.Log("MagoEnemigo: entre a Avanzar");   // prueba de vida: debe salir en la consola al hacer Play
        }

        public override string Actualizar(float dt) { return null; }   // null = quedarse en este estado

        public override string AlRecibir(string trigger, int valor) { return null; }
    }
}
```

¿Terminaste antes? Es opcional y no se califica hoy: empieza el siguiente estado (la clase 2 trae `MagoEnemigoMuerto`, `MagoEnemigoPosicionarse`).

| Archivo (en `Assets/_Alumnos/MagoEnemigo/`) | Qué debe tener |
| --- | --- |
| `MagoEnemigoController.cs` | **Nuevo.** `class MagoEnemigoController : MonoBehaviour` dentro de `namespace Alumnos.MagoEnemigo`. Crea el `Contexto`, la `Maquina` con tus estados e inicia en `Avanzar`. |
| `MagoEnemigoAvanzar.cs` | **Nuevo.** `class MagoEnemigoAvanzar : Estado`. Sigue la ruta. Animación: `Caminar`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Onboarding y primer commit: el Controller crea la Maquina e inicia en Avanzar (puede ser un esqueleto que solo escribe un mensaje en consola). Compila, se ve en Play y el PR pasa la revision.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: desde la oleada 3.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `MagoEnemigoAvanzar`.

Todavía NO se pide: `MagoEnemigoPosicionarse`, `MagoEnemigoLanzar`, `MagoEnemigoRecargar`, `MagoEnemigoHuir`, `MagoEnemigoAturdido`, `MagoEnemigoFestejo`, `MagoEnemigoMuerto`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/MagoEnemigo/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.MagoEnemigo` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-1/05-magoenemigo` y el PR se crea hoy (máximo el Lun 9 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-1/05-magoenemigo
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/MagoEnemigo
git commit -m "clase 1: MagoEnemigo - Avanzar"
git push -u origin clase-1/05-magoenemigo
```

### Clase 2 · Mié 11 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-2/05-magoenemigo`  ·  **Avance esperado:** 3 de 8 estados (38%)

![Diagrama de estados de Mago enemigo](img/m05_magoenemigo_6.svg)

| Archivo (en `Assets/_Alumnos/MagoEnemigo/`) | Qué debe tener |
| --- | --- |
| `MagoEnemigoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new MagoEnemigoMuerto()`, `new MagoEnemigoPosicionarse()`. |
| `MagoEnemigoMuerto.cs` | **Nuevo.** `class MagoEnemigoMuerto : Estado`. Global. Animación: `Muerte1`. |
| `MagoEnemigoPosicionarse.cs` | **Nuevo.** `class MagoEnemigoPosicionarse : Estado`. Se coloca a ~9 m del objetivo y frena suave. Animación: `Caminar`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Muerto: Global. Posicionarse: Se coloca a ~9 m del objetivo y frena suave.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: desde la oleada 3.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `MagoEnemigoAvanzar`, `MagoEnemigoMuerto`, `MagoEnemigoPosicionarse`.

Todavía NO se pide: `MagoEnemigoLanzar`, `MagoEnemigoRecargar`, `MagoEnemigoHuir`, `MagoEnemigoAturdido`, `MagoEnemigoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/MagoEnemigo/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.MagoEnemigo` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-2/05-magoenemigo` y el PR se crea hoy (máximo el Mié 11 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-2/05-magoenemigo
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/MagoEnemigo
git commit -m "clase 2: MagoEnemigo - Muerto, Posicionarse"
git push -u origin clase-2/05-magoenemigo
```

### Clase 3 · Mié 18 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-3/05-magoenemigo`  ·  **Avance esperado:** 5 de 8 estados (62%)

![Diagrama de estados de Mago enemigo](img/m05_magoenemigo_7.svg)

| Archivo (en `Assets/_Alumnos/MagoEnemigo/`) | Qué debe tener |
| --- | --- |
| `MagoEnemigoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new MagoEnemigoLanzar()`, `new MagoEnemigoRecargar()`. |
| `MagoEnemigoLanzar.cs` | **Nuevo.** `class MagoEnemigoLanzar : Estado`. Cast con proyectil: ProyectilBase.Lanzar(...) a mitad de la animación. Animación: `HechizoLanzar`. |
| `MagoEnemigoRecargar.cs` | **Nuevo.** `class MagoEnemigoRecargar : Estado`. Espera 2.4 s entre hechizos. Animación: `HechizoIdle`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Lanzar: Cast con proyectil: ProyectilBase.Lanzar(...) a mitad de la animación. Recargar: Espera 2.4 s entre hechizos.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: desde la oleada 3.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `MagoEnemigoAvanzar`, `MagoEnemigoMuerto`, `MagoEnemigoPosicionarse`, `MagoEnemigoLanzar`, `MagoEnemigoRecargar`.

Todavía NO se pide: `MagoEnemigoHuir`, `MagoEnemigoAturdido`, `MagoEnemigoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/MagoEnemigo/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.MagoEnemigo` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-3/05-magoenemigo` y el PR se crea hoy (máximo el Mié 18 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-3/05-magoenemigo
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/MagoEnemigo
git commit -m "clase 3: MagoEnemigo - Lanzar, Recargar"
git push -u origin clase-3/05-magoenemigo
```

### Clase 4 · Lun 23 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-4/05-magoenemigo`  ·  **Avance esperado:** 7 de 8 estados (88%)

![Diagrama de estados de Mago enemigo](img/m05_magoenemigo_8.svg)

| Archivo (en `Assets/_Alumnos/MagoEnemigo/`) | Qué debe tener |
| --- | --- |
| `MagoEnemigoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new MagoEnemigoHuir()`, `new MagoEnemigoAturdido()`. |
| `MagoEnemigoHuir.cs` | **Nuevo.** `class MagoEnemigoHuir : Estado`. Si un hostil esta a &lt; 3 m, se escapa 1.5 s anticipando su trayectoria. Animación: `Correr`. |
| `MagoEnemigoAturdido.cs` | **Nuevo.** `class MagoEnemigoAturdido : Estado`. Reaccion al daño. Animación: `Dano1`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Huir: Si un hostil esta a &lt; 3 m, se escapa 1.5 s anticipando su trayectoria. Aturdido: Reaccion al daño.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: desde la oleada 3.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `MagoEnemigoAvanzar`, `MagoEnemigoMuerto`, `MagoEnemigoPosicionarse`, `MagoEnemigoLanzar`, `MagoEnemigoRecargar`, `MagoEnemigoHuir`, `MagoEnemigoAturdido`.

Todavía NO se pide: `MagoEnemigoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/MagoEnemigo/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.MagoEnemigo` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-4/05-magoenemigo` y el PR se crea hoy (máximo el Lun 23 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-4/05-magoenemigo
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/MagoEnemigo
git commit -m "clase 4: MagoEnemigo - Huir, Aturdido"
git push -u origin clase-4/05-magoenemigo
```

### Clase 5 · Mié 25 nov · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-5/05-magoenemigo`  ·  **Avance esperado:** 8 de 8 estados (100%)

![Diagrama de estados de Mago enemigo](img/m05_magoenemigo_9.svg)

| Archivo (en `Assets/_Alumnos/MagoEnemigo/`) | Qué debe tener |
| --- | --- |
| `MagoEnemigoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: `new MagoEnemigoFestejo()`. |
| `MagoEnemigoFestejo.cs` | **Nuevo.** `class MagoEnemigoFestejo : Estado`. Global. Animación: `Celebrar1`. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Festejo: Global.
- Cómo probarlo: abre `Granero.unity`, pulsa Play y espera la oleada con ESPACIO. Tu agente aparece: desde la oleada 3.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `MagoEnemigoAvanzar`, `MagoEnemigoMuerto`, `MagoEnemigoPosicionarse`, `MagoEnemigoLanzar`, `MagoEnemigoRecargar`, `MagoEnemigoHuir`, `MagoEnemigoAturdido`, `MagoEnemigoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/MagoEnemigo/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.MagoEnemigo` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-5/05-magoenemigo` y el PR se crea hoy (máximo el Mié 25 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-5/05-magoenemigo
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/MagoEnemigo
git commit -m "clase 5: MagoEnemigo - Festejo"
git push -u origin clase-5/05-magoenemigo
```

### Clase 6 · Lun 30 nov · Colchón (opcional, no se califica) · PR a más tardar ese día, 23:59 (CDMX)

**Rama:** `clase-6/05-magoenemigo`  ·  **Avance esperado:** 8 de 8 estados (100%)

![Diagrama de estados de Mago enemigo](img/m05_magoenemigo_10.svg)

| Archivo (en `Assets/_Alumnos/MagoEnemigo/`) | Qué debe tener |
| --- | --- |
| `MagoEnemigoController.cs` | **Lo modificas.** Registra en la lista de la máquina los estados nuevos: ninguno nuevo (solo ajustes). |
| (ninguno obligatorio) | Colchón: sin contenido nuevo. Solo corriges lo que marcó el Action o terminas lo que se quedó atrás. |
| `*.meta` | Cada archivo nuevo trae su `.meta` (Unity lo crea al abrir el proyecto). Sube los dos. |

**Qué debe verse en Play al terminar esta clase**

- Esta clase **no tiene contenido nuevo y no cuenta para la calificación** (el promedio es de las clases 1 a 5).
- Úsala para: corregir los ❌ que dejó el Action, terminar estados atrasados y probar tu máquina junto con las otras.
- Si necesitas entregar correcciones, crea la rama `clase-6/05-magoenemigo` y su PR; si no, no hace falta.

**Lo que revisa el Action en esta clase** (debe existir cada clase, hereda de `Estado` y estar registrada en el controller): `MagoEnemigoAvanzar`, `MagoEnemigoMuerto`, `MagoEnemigoPosicionarse`, `MagoEnemigoLanzar`, `MagoEnemigoRecargar`, `MagoEnemigoHuir`, `MagoEnemigoAturdido`, `MagoEnemigoFestejo`.

**Lista de comprobación antes del PR**

- [ ] Unity compila: la consola está vacía (sin errores rojos).
- [ ] Probado en Play (lo de arriba funciona).
- [ ] `git status` solo muestra archivos de `Assets/_Alumnos/MagoEnemigo/` (y sus `.meta`).
- [ ] Todo tu código está dentro de `namespace Alumnos.MagoEnemigo` (los `Steering*.cs` no llevan namespace) y es ASCII (sin acentos ni ñ).
- [ ] La rama se llama exactamente `clase-6/05-magoenemigo` y el PR se crea hoy (máximo el Lun 30 nov).
- [ ] Leíste el comentario del Action en tu PR y corregiste cada ❌.

**Comandos de esta entrega** (copiar y pegar; si algo falla ve a [Troubleshooting (si el pipeline falla)](05_troubleshooting.md)):

```bash
git checkout main
git pull
git checkout -b clase-6/05-magoenemigo
# ... programas y pruebas en Unity ...
git status
git add Assets/_Alumnos/MagoEnemigo
git commit -m "clase 6: MagoEnemigo - correcciones"
git push -u origin clase-6/05-magoenemigo
```

### Rúbrica (igual en todas las clases)

| Criterio | Puntos | Qué se revisa |
| --- | --- | --- |
| Funciona en el juego | 50 | El agente/sistema hace lo pedido en Play, sin errores en consola, con el prefab real. |
| Estados entregados | 30 | Cumple los estados de ESA clase (ver tabla). Cada estado que falta resta proporcionalmente. |
| Codigo y rendimiento | 20 | Una clase por estado con prefijo del rol, ASCII, sin new/LINQ en Update, eventos desuscritos, cero basura medido. |
| Total por clase | 100 | La calificación final es el promedio de las 5 entregas de las clases 1 a 5 (cada una pesa igual). La clase 6 es colchón y no cuenta. |

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
