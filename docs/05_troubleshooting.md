# Troubleshooting: el pipeline falló o Git se queja

[Plan general](00_indice_y_plan.md) · [Guia de inicio](03_como_empezar.md) · [Como subir cambios](04_como_subir_cambios.md) · [Troubleshooting](05_troubleshooting.md) · [Estado general](01_estado_general.md) · [Steering](02_steering_behaviors.md) · [README](../README.md)

Todo se resuelve desde la consola. Busca el mensaje **tal como aparece** en el comentario del Action o en tu terminal. Usa **Git Bash** (viene con Git para Windows): todos los comandos funcionan ahí. En PowerShell los de `grep`, `ls` y `sed` tienen equivalente indicado o usa `Select-String`/`dir`. Los ejemplos usan MagoEnemigo (cupo 05, clase 2): cambia los nombres por los tuyos.

## 0. Antes de nada: lee el reporte desde la consola

```bash
gh pr view --comments       # el comentario del Action con lo que falta (X) y los avisos
gh pr checks                # estado del check (verde / rojo)
gh run list --limit 5       # ejecuciones recientes del Action
gh run view --log-failed    # el log si el Action mismo se rompio
```

| Icono | Significa | ¿Bloquea? |
| --- | --- | --- |
| ✅ | Cumple | - |
| ❌ | Falta algo obligatorio: el instructor no aprobará el PR así | Sí (para aprobar) |
| ⚠️ | Aviso de calidad: baja puntos en “Código y rendimiento” pero no impide el merge | No |

El check sale en rojo mientras haya al menos un ❌. Cuando corriges y haces push, se ejecuta otra vez solo.

## 1. Mensajes del pipeline (❌ obligatorios)

### Nombre de rama invalido: ...

**Por qué:** la rama no sigue `clase-<1..6>/<cupo 01..14>-<maquina en minúsculas>`. El Action usa el nombre para saber quién eres y qué revisar.

```bash
git branch --show-current                     # ve el nombre actual
git branch -m clase-2/05-magoenemigo          # renombra la rama local
git push -u origin clase-2/05-magoenemigo     # sube la rama con el nombre bueno
git push origin --delete nombre-viejo         # borra la rama mala del remoto (cierra el PR viejo)
gh pr create --base main --title "Clase 2 - MagoEnemigo (cupo 05)" --fill
```

Un PR no puede cambiar de rama: se crea uno nuevo. Si ya pasó el límite avisa al instructor; el PR original muestra cuándo entregaste.

### El cupo NN es X: la rama debe terminar en -x

**Por qué:** el número de cupo y el nombre de la máquina no coinciden (por ejemplo `clase-2/05-ladron`: el cupo 05 es MagoEnemigo).

```bash
# mismo procedimiento que 'Nombre de rama invalido'
git branch -m clase-2/05-magoenemigo
git push -u origin clase-2/05-magoenemigo
git push origin --delete clase-2/05-ladron
```

### Entrega TARDIA

**Por qué:** el PR se creó después de la fecha límite de esa clase (23:59 hora CDMX). Se mide al **crear** el PR, no al último commit.

```bash
gh pr view --json createdAt,headRefName     # cuando se creo tu PR
# no hay arreglo tecnico: habla con el instructor. Para la siguiente clase crea el PR el mismo dia.
```

### Modificaste N archivo(s) que no son tuyos

**Por qué:** tu rama cambia archivos fuera de tu carpeta (casi siempre Unity toca `ProjectSettings`, `Packages` o escenas al abrir el proyecto).

```bash
# 1) mira cuales son
git fetch origin
git diff --name-only origin/main...HEAD

# 2) los que ya existen en main: vuelve a dejarlos como estan en main
git checkout origin/main -- ProjectSettings Packages Assets/_Granero

# 3) los que NO existen en main (archivos nuevos ajenos): quitalos
git rm -r -f --ignore-unmatch ruta/del/archivo/nuevo

# 4) guarda y sube
git commit -m "quitar archivos ajenos"
git push
```

Para listar solo lo ajeno de una vez (Git Bash): `git diff --name-only origin/main...HEAD | grep -v '^Assets/_Alumnos/MagoEnemigo/'`. En PowerShell: `git diff --name-only origin/main...HEAD | Select-String -NotMatch '^Assets/_Alumnos/MagoEnemigo/'`. Para que no vuelva a pasar, antes de cada commit usa `git add Assets/_Alumnos/MagoEnemigo` (nunca `git add .`).

### Falta el controller ...

**Por qué:** no existe `Assets/_Alumnos/<Maquina>/<Maquina>Controller.cs` en tu rama.

```bash
ls Assets/_Alumnos/MagoEnemigo/                      # comprueba el nombre exacto (mayusculas incluidas)
git add Assets/_Alumnos/MagoEnemigo/MagoEnemigoController.cs Assets/_Alumnos/MagoEnemigo/MagoEnemigoController.cs.meta
git commit -m "agregar controller"
git push
```

Si el archivo existe pero el Action no lo ve, el nombre está mal escrito (`MagoEnemigoControler.cs`) o está en otra carpeta.

### ... debe declarar class XController : MonoBehaviour

**Por qué:** la clase no se llama exactamente `<Maquina>Controller` o no hereda de `MonoBehaviour`.

```bash
grep -n "class" Assets/_Alumnos/MagoEnemigo/MagoEnemigoController.cs     # PowerShell: Select-String -Path ... -Pattern "class"
```

Debe leerse así: `public class MagoEnemigoController : MonoBehaviour`.

### El controller debe crear la maquina (new Maquina(...)) e iniciarla (Iniciar(...))

**Por qué:** falta alguna de las dos llamadas en tu controller.

```bash
grep -n "new Maquina\|Iniciar(" Assets/_Alumnos/MagoEnemigo/MagoEnemigoController.cs
```

Debes ver una línea con `new Maquina(` y otra con `.Iniciar(`.

### Falta el estado X: debe ser class X : Estado en XY.cs

**Por qué:** el archivo del estado no existe, se llama distinto o la clase no hereda de `Estado`.

```bash
ls Assets/_Alumnos/MagoEnemigo/
grep -rn ": Estado" Assets/_Alumnos/MagoEnemigo/
# si hay que renombrar un archivo, hazlo con git para no perder el historial y renombra tambien su .meta:
git mv Assets/_Alumnos/MagoEnemigo/MagoPosicion.cs Assets/_Alumnos/MagoEnemigo/MagoEnemigoPosicionarse.cs
git mv Assets/_Alumnos/MagoEnemigo/MagoPosicion.cs.meta Assets/_Alumnos/MagoEnemigo/MagoEnemigoPosicionarse.cs.meta
```

El nombre de la clase y del archivo es `<Maquina><Estado>` (ej. `MagoEnemigoPosicionarse`). Cambia también el nombre de la clase dentro del archivo.

### X existe pero no esta registrado en la lista de estados del controller

**Por qué:** escribiste la clase pero falta `new X()` en la lista de `new Maquina(...)`.

```bash
grep -n "new MagoEnemigo" Assets/_Alumnos/MagoEnemigo/MagoEnemigoController.cs
```

Agrega una línea `new MagoEnemigoPosicionarse(),` a la lista.

### Falta el steering X / NO debe tener namespace / debe ser public static partial class Steering

**Por qué:** el archivo `Assets/_Alumnos/Steering/SteeringX.cs` no existe, tiene namespace o no tiene la firma de `Steering`.

```bash
head -20 Assets/_Alumnos/Steering/SteeringSeek.cs     # PowerShell: Get-Content ... -TotalCount 20
```

Debe empezar con `public static partial class Steering` directamente (sin `namespace`) y contener `public static Vector3 Seek(...)` con la firma del documento de steering.

### Estos archivos deben estar dentro de namespace Alumnos.X

**Por qué:** tus `.cs` (menos los `Steering*.cs`) no están dentro de `namespace Alumnos.<Maquina>`.

```bash
grep -L "namespace Alumnos.MagoEnemigo" Assets/_Alumnos/MagoEnemigo/*.cs     # lista los archivos SIN namespace
```

Envuelve el contenido: `namespace Alumnos.MagoEnemigo { ...clases... }`.

### Nombres de clase que ya existen en el juego base

**Por qué:** una de tus clases se llama igual que una del instructor (`Sensor`, `Agente`, `Juego`...) y la tapa.

```bash
grep -rn "class Sensor\b" Assets/_Alumnos/MagoEnemigo/
# renombra la clase (y el archivo) con el prefijo de tu maquina
git mv Assets/_Alumnos/MagoEnemigo/Sensor.cs Assets/_Alumnos/MagoEnemigo/MagoEnemigoSensor.cs
git mv Assets/_Alumnos/MagoEnemigo/Sensor.cs.meta Assets/_Alumnos/MagoEnemigo/MagoEnemigoSensor.cs.meta
```

## 2. Avisos (⚠️) y cómo quitarlos

### ... tiene caracteres no ASCII

**Por qué:** hay acentos, ñ, símbolos o un BOM (marca invisible que algunos editores ponen al inicio del archivo).

```bash
# Git Bash: encuentra la linea
grep -nP "[^\x00-\x7F]" Assets/_Alumnos/MagoEnemigo/*.cs
# PowerShell:
Select-String -Path Assets\_Alumnos\MagoEnemigo\*.cs -Pattern "[^\x00-\x7F]"

# quitar un BOM sin tocar el resto (Git Bash)
sed -i '1s/^\xEF\xBB\xBF//' Assets/_Alumnos/MagoEnemigo/MagoEnemigoLanzar.cs
```

Cambia "cañón" por "canon" en comentarios y textos. En Visual Studio: *Guardar con codificación* → UTF-8 sin firma.

### usa LINQ / GetComponent o Find dentro de Actualizar / crea objetos (new) dentro de Actualizar

**Por qué:** generan basura de memoria (GC) cada frame.

```bash
grep -n "using System.Linq\|\.Select(\|\.Where(\|GetComponent\|FindObject\|Find(" Assets/_Alumnos/MagoEnemigo/*.cs
```

Guarda la referencia una vez en `Entrar` o en el `Contexto`; reutiliza listas y arreglos en lugar de crearlos.

### se suscribe a eventos (.AlAlgo +=) pero no se desuscribe (-=)

**Por qué:** el agente vuelve a la Reserva y sigue escuchando el evento; el siguiente agente reaccionará dos veces.

```bash
grep -n "+=\|-=" Assets/_Alumnos/MagoEnemigo/*.cs
```

Cada `+=` en `Entrar` necesita su `-=` en `Salir` (o en `OnDisable`).

### Falta X.meta

**Por qué:** subiste el `.cs` sin su `.meta`. Unity lo genera al abrir el proyecto; sin él, los enlaces se rompen.

```bash
git status --short Assets/_Alumnos/MagoEnemigo/     # los .meta aparecen con ??
git add Assets/_Alumnos/MagoEnemigo
git commit -m "agregar .meta"
git push
```

### Clases sin el prefijo

**Por qué:** los nombres de tus clases deben empezar con `MagoEnemigo...` para no chocar con las de otros.

```bash
grep -rn "^\s*\(public \)\?class " Assets/_Alumnos/MagoEnemigo/*.cs
```

## 3. El Action no aparece, no corre o parece pegado

### No hay comentario en mi PR

**Por qué:** el Action solo revisa PR cuya rama empieza con `clase-` y cuyo destino es `main`.

```bash
gh pr view --json headRefName,baseRefName     # head debe empezar con clase-, base debe ser main
gh run list --limit 5
```

### Quiero volver a ejecutar la revisión sin cambiar código

**Por qué:** a veces basta un empujoncito.

```bash
git commit --allow-empty -m "volver a ejecutar la revision"
git push
# o edita el titulo del PR (tambien dispara la revision)
gh pr edit --title "Clase 2 - MagoEnemigo (cupo 05) - revisado"
```

### El check sigue en rojo pero ya corregí

**Por qué:** mira la fecha del último comentario: el reporte se actualiza al hacer push, no al guardar en tu disco.

```bash
git status
git log --oneline -3
git push           # si dice 'Everything up-to-date' ya estaba subido; espera 1-2 minutos y revisa gh pr view --comments
```

## 4. Errores de Git

### ! [rejected] ... (non-fast-forward) / Updates were rejected

**Por qué:** la rama remota tiene commits que tu copia local no tiene.

```bash
git pull origin clase-2/05-magoenemigo     # trae e integra lo remoto
git push
```

Si aparece un editor de mensaje de merge, guarda y cierra (en vim: `:wq` + Enter). No uses `--force` salvo que el instructor te lo pida.

### remote: error: GH006: Protected branch update failed for refs/heads/main

**Por qué:** intentaste subir a `main`. Está protegida: solo se entra por PR.

```bash
# mueve tus commits a una rama nueva y deja main como estaba
git branch clase-2/05-magoenemigo        # crea la rama en el commit actual (con tus commits)
git reset --hard origin/main             # main local vuelve a ser igual que el remoto
git checkout clase-2/05-magoenemigo
git push -u origin clase-2/05-magoenemigo
```

`git reset --hard` borra cambios sin commit: haz primero `git status` y confirma que todo quedó en commits.

### remote: Permission denied / fatal: unable to access ... 403

**Por qué:** no eres colaborador (falta aceptar la invitación) o estás con otra cuenta.

```bash
gh auth status
git remote -v                  # debe apuntar a BanyoLoper/tower-defense
gh repo view BanyoLoper/tower-defense --json viewerPermission
```

Si `viewerPermission` no es `WRITE`, acepta la invitación en `github.com/BanyoLoper/tower-defense/invitations` o pide al instructor que te agregue.

### fatal: You are not currently on a branch / HEAD detached

**Por qué:** hiciste `checkout` de un commit en lugar de una rama.

```bash
git switch -c clase-2/05-magoenemigo     # crea la rama aqui mismo y conserva tus commits
```

### error: Your local changes would be overwritten by checkout / pull

**Por qué:** tienes cambios sin guardar que chocan con lo que traes.

```bash
git stash                      # guarda tus cambios en un cajon
git checkout main
git pull
git checkout clase-2/05-magoenemigo
git stash pop                  # los recupera
```

### CONFLICT (content): Merge conflict in ...

**Por qué:** tu rama y `main` cambiaron el mismo archivo (suele pasar si tocaste algo ajeno).

```bash
git status                       # lista los archivos en conflicto
# abre cada archivo, busca <<<<<<< ======= >>>>>>> y deja la version correcta
git add ruta/del/archivo
git commit -m "resolver conflicto"
git push
```

Si el archivo en conflicto no es tuyo, quédate con la versión de `main`: `git checkout origin/main -- ruta/del/archivo`.

### error: open("..."): Filename too long / fatal: unable to create file

**Por qué:** límite de 260 caracteres de Windows (el proyecto trae FBX con nombres muy largos).

```bash
git config --global core.longpaths true
# si sigue fallando, clona en una ruta corta
cd C:\
git clone https://github.com/BanyoLoper/tower-defense.git td
```

Windows también tiene el ajuste `LongPathsEnabled` en el registro (requiere administrador).

### fatal: Unable to create '.git/index.lock': File exists

**Por qué:** otro Git (o un cuelgue anterior) dejó el candado.

```bash
# 1) cierra GitHub Desktop, VS Code y cualquier terminal con git
# 2) borra el candado
rm .git/index.lock                    # PowerShell: Remove-Item .git\index.lock
```

### nothing to commit, working tree clean (y tu codigo no sube)

**Por qué:** los archivos están ignorados o los guardaste en otra carpeta.

```bash
git status --ignored | head -20
git check-ignore -v Assets/_Alumnos/MagoEnemigo/MagoEnemigoLanzar.cs     # dice que regla los ignora, si alguna
pwd                                                                       # estas en la carpeta del repo?
```

### Hice commit en main por error

**Por qué:** los commits están en `main` local y no se pueden subir.

```bash
git branch clase-2/05-magoenemigo      # nueva rama con tus commits
git reset --hard origin/main            # main local vuelve a igualar el remoto
git checkout clase-2/05-magoenemigo
git push -u origin clase-2/05-magoenemigo
```

### Me equivoque en el ultimo commit (mensaje o archivo)

**Por qué:** aún no lo subiste: se puede rehacer sin dejar rastro.

```bash
git commit --amend -m "mensaje correcto"          # solo el mensaje
git reset --soft HEAD~1                            # deshace el commit y deja los cambios listos para recomitear
```

Si ya hiciste `push`, no reescribas el historial: haz un commit nuevo que corrija y sube normal.

### warning: LF will be replaced by CRLF

**Por qué:** solo es un aviso de finales de línea de Windows. No es un error; ignóralo.

### Se subieron Library/ o Temp/ o archivos enormes

**Por qué:** alguien quitó el `.gitignore` o forzó el add.

```bash
git rm -r --cached Library Temp Logs obj
git commit -m "quitar carpetas generadas"
git push
```

## 5. Estoy perdido: reiniciar limpio sin perder tu codigo

Si nada de lo anterior funciona, guarda tus archivos fuera del repo, vuelve a clonar y vuelve a poner tu carpeta:

```bash
cd C:\
move td td_viejo                                  # PowerShell: Rename-Item td td_viejo
git clone https://github.com/BanyoLoper/tower-defense.git td
cd td
git checkout -b clase-2/05-magoenemigo
# copia SOLO tu carpeta desde el repo viejo
robocopy td_viejo\Assets\_Alumnos\MagoEnemigo td\Assets\_Alumnos\MagoEnemigo /E
git status
```

Después sigue los pasos 5–8 de [Cómo subir tus cambios](04_como_subir_cambios.md). Si aún así no avanzas, copia el mensaje de error completo y pásalo al instructor.
