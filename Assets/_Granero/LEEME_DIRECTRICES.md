# Granero de Cristales - Directrices del proyecto

## Reglas de codigo (para los alumnos)
- C# solo ASCII (sin acentos ni enie en el codigo; si en comentarios). El codigo del instructor (`_Granero`) no usa namespace; el de los alumnos va en `namespace Alumnos.<Maquina>` (los `Steering*.cs` sin namespace). `BuscadorTipos` encuentra los controllers por nombre corto.
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

## Maquinas de estados que YA trae la base (ejemplos de referencia)
Todas usan `Estado` / `Contexto` / `Maquina` del proyecto. Son pequenas a proposito: leelas antes de escribir las tuyas.
| Maquina | Archivo | Estados | Quien la dispara |
|---|---|---|---|
| Aspecto del granero | `Scripts/Estados/GraneroEstados.cs` | GraneroSano, GraneroDanado, GraneroCritico, GraneroDestello, GraneroDestruido | eventos de `Salud` del granero. Usa el efecto Hit de AllIn13D y rompe cristales |
| Texto central | `Scripts/Estados/TextoCentral.cs` | TextoOculto, TextoEntrando, TextoMantener, TextoSaliendo | `Juego.AlAcercarse / AlComenzar / AlCambiarFase`; tambien `TextoCentral.I.Mostrar(...)` |
| Camara | `Scripts/Base/CamaraRTS.cs` | CamLibre, CamSacudida, CamVictoria, CamDerrota | `CamaraRTS.I.Sacudir(fuerza)` y `Juego.AlCambiarFase` |
| Plataforma (visual) | `Scripts/Base/SlotTorre.cs` | SlotReposo, SlotHover, SlotDesbloqueo | `ColocadorTorres` (mouse encima) y el desbloqueo |

Eventos de `Juego` para tus maquinas: `AlAcercarse(nivel)`, `AlComenzar(nivel)`, `AlCambiarFase(fase)`.
Festejo: en `FaseJuego.Victoria` celebran los defensores/aldeanos; en `Derrota` celebran los enemigos y los aliados lloran
(ver `CerebroDemo.Fin()`; tu maquina solo necesita un estado "Festejo" y un trigger "victoria"/"derrota").

### Ideas para ampliar o crear maquinas (candidatas para los alumnos)
- Director de oleadas (GameManager): Preparacion -> Aviso -> Oleada -> Pausa -> Jefe -> Victoria/Derrota. Reemplaza al `Juego` demo.
- Torre: Vigilando -> Apuntando -> Disparando -> Recargando -> Danada -> Destruida (+ mejora de nivel).
- Plataforma (logica): Bloqueada -> Comprable -> Libre -> Construyendo -> Ocupada -> Vendiendo.
- Tienda/Economia: Normal -> Ahorrando -> Gastando -> Quiebra (cambia el color del contador de gemas).
- Musica/ambiente: Calma -> Tension -> Combate -> Jefe -> Fin (cuando exista audio).
- Tutorial: pasos guiados con `TextoCentral` (clic en plataforma, construir, llamar oleada).
- Ladron: Acechar -> Robar -> Huir -> Derrotado (ya esta en el demo sin maquina).

## Catalogo de efectos (`Efectos.Reproducir`)
Una linea, sin crear particulas: `Efectos.Reproducir(Fx.Explosion, posicion, escala);`
| Fx | Que es | Uso sugerido |
|---|---|---|
| Impacto | onda de choque corta | golpe, romper cristal |
| Explosion | bola de fuego grande | muerte del jefe, bomba |
| Curacion | destello de curacion | reparador, curar |
| Desbloqueo | aura brillante | desbloquear plataforma, subir de nivel |
| Rayo | rayo desde el cielo | ataque especial |
| Escudo | escudo magico | bloqueo, proteccion |
| Fuego / Hielo | aura de fuego / hielo | estados alterados, torre maga |
| OndaSuelo / Aura | onda en el suelo | aparicion de jefe, aviso |
Usan la `Reserva` (pool): no hay basura. Escalas y prefabs se ajustan en el objeto `Efectos` de la escena.
