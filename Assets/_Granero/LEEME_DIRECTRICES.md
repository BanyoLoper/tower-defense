# Granero de Cristales - Directrices del proyecto

## Reglas de codigo (para los alumnos)
- C# sin namespace, solo ASCII (sin acentos ni enie en el codigo; si en comentarios).
- Un estado = una clase que hereda de `Estado`. Un controller = un MonoBehaviour llamado `<Rol>Controller`.
- La animacion la pide el estado: `agente.Animar(Anim.Correr)`. El Animator no tiene transiciones.
- Movimiento solo con `Cuerpo.Mover(velocidadDeseada, dt)`. No hay NavMeshAgent.

## Directriz de rendimiento (meta: 60 fps estables, sin picos de GC)
Medido en el demo: la Galeria con 12 agentes corre a ~59 fps promedio en el Editor, p99 de 19 ms,
sin degradacion en 100 s. Para que siga asi cuando haya 14 maquinas corriendo:

1. **Cero basura en Update/Actualizar.** Prohibido dentro de `Actualizar(dt)`, `AlRecibir` y `Update`:
   - `new` de clases, listas o arreglos (reutiliza campos o guardalos en el `Contexto`).
   - concatenar cadenas (`"a" + b`), `ToString()`, `string.Format`, interpolacion `$""`.
   - LINQ (`Where`, `Select`, `ToList`, `FirstOrDefault`...).
   - lambdas o delegados que capturan variables, y `foreach` sobre colecciones que no sean `List<T>` o arreglos.
   - `GetComponent`, `Find`, `FindObjectOfType`, `Camera.main` en cada frame: guardalos en `Awake`/`Entrar`.
2. **Textos de UI y rotulos:** actualizalos solo cuando el valor cambia, nunca cada frame. No usar `OnGUI`.
3. **Consultas de percepcion** (`Sensor`) no cada frame: cada 0.2 s con un temporizador (`dt`) y guarda el resultado.
4. **Rutas:** `Camino.Calcular` solo al entrar a un estado o cuando el objetivo cambie de verdad, no cada frame.
5. **Proyectiles y efectos:** no `Instantiate`/`Destroy` en masa. Si un prefab se dispara mas de ~5 veces por segundo, usar pool.
6. **Logs:** ningun `Debug.Log` en `Actualizar`. Un log por frame mata el rendimiento (y llena la consola).
7. **Tiempo:** todo con `dt`; jamas dependas del numero de frames.

## Como medir
- Archivo `Scripts/Base/PerfMonitor.cs` (solo Editor): crea `BridgeCmd/perf.flag` con los segundos a medir y entra a Play.
- Lo que cuenta: `fps promedio >= 58`, `p99 <= 20 ms`, y `GC alloc/frame` de TU codigo cerca de 0.
- En el Editor hay ~100 KB/frame de basura que NO es del juego (es el Editor). Compara siempre contra una escena vacia.
- Para medir bien: Play con la ventana Game maximizada, Gizmos apagados y sin el Profiler abierto.
- `AjustesRendimiento.cs` fija 60 fps, sin vSync, GC incremental.

## Ruta del proyecto (Windows)
Windows limita las rutas a 260 caracteres. Clona/abre el proyecto en una ruta corta, por ejemplo `D:\TD\`.
Si la ruta es larga, Unity puede dar `DirectoryNotFoundException` en `Library\PackageCache\...`.

## Plataformas de torres (desbloqueo)
- Estados (`EstadoSlot`): Bloqueado (rojo + candado), Comprable (naranja, alcanzan las gemas), Libre (azul), Elegido (amarillo), Ocupado (gris).
- 4 plataformas abiertas al inicio; cada oleada superada abre 2 mas (`nivelRequerido`). Con la plataforma elegida, `U` la abre pagando `precioDesbloqueo`.
- Los colores se actualizan por eventos (gemas / cambio de juego), nunca por frame.
