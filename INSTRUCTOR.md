# Guia del instructor

## 1. Configuracion unica del repositorio

1. **Colaboradores:** `Settings > Collaborators` -> agrega a los 14 alumnos con permiso *Write* (para que puedan subir ramas; `main` queda protegido por la regla).
2. **Actions:** `Settings > Actions > General` -> *Allow all actions*, y en *Workflow permissions* deja *Read and write permissions* (el workflow comenta en los PR).
3. **Regla de `main`:** `Settings > Rules > Rulesets > New ruleset > Import a ruleset` y sube `.github/ruleset-main.json`. O con GitHub CLI:
   ```bash
   gh api repos/BanyoLoper/tower-defense/rulesets --input .github/ruleset-main.json
   ```
   Hace que: nadie suba directo a `main` (solo PR), cada PR requiera **1 aprobacion de un code owner** (`.github/CODEOWNERS` = solo @BanyoLoper), se descarten aprobaciones si llegan commits nuevos, y no se pueda borrar `main` ni reescribir su historial.
   Como GitHub no deja aprobar tus propios PR, el ruleset te da *bypass* (rol Admin, modo *pull request*): tu puedes hacer merge de PR tuyos desde el boton *Merge without waiting for requirements*.
4. **Importante:** en repos **privados** con plan Free, GitHub no aplica rulesets ni proteccion de ramas. Opciones: hacer el repo publico, o usar GitHub Pro / GitHub Education (gratis para docentes).

## 2. Fechas limite por clase

Edita `.github/entregas/calendario.json` (las fechas que trae son de ejemplo). La regla: el PR debe **crearse** a mas tardar en `fecha_limite` a la `hora_limite` (hora de CDMX).

## 3. Revisiones

- **Automatica por PR:** `.github/workflows/revisar-entrega.yml` se ejecuta en cada PR hacia `main` y deja un comentario con lo que cumple y lo que falta. Tambien se puede lanzar a mano con el nombre de una rama (`Actions > Revisar entrega (PR) > Run workflow`).
- **Manual por clase:** `Actions > Revisar entregas de una clase (manual) > Run workflow`, elige la clase. Genera una tabla con todos los cupos (PR, autor, estados, steering, a tiempo, faltas, avisos), marca **SIN ENTREGA** y guarda un CSV como artefacto.
- El workflow lee el criterio de `.github/entregas/criterios.json` (estados y steering por cupo y clase, generado desde `docs/`). Si cambias los estados de alguna maquina, actualiza ese archivo.
- Seguridad: el workflow usa `pull_request_target`, toma el script siempre de `main` y solo *lee* el contenido de la rama del alumno; nunca ejecuta su codigo. Cualquier PR que toque `.github/` aparece como archivo ajeno y requiere tu aprobacion.
- El check NO es obligatorio para el merge: una entrega incompleta se puede aprobar si lo decides.

## 4. Convencion de ramas

`clase-<N>/<cupo 01-14>-<maquina en minusculas>`, por ejemplo `clase-2/05-magoenemigo`. El cupo y la maquina deben corresponder a la tabla de `criterios.json`.

## 5. Estructura de carpetas

- `Assets/_Granero/` base del juego (instructor). Los alumnos no la tocan.
- `Assets/_Alumnos/<Maquina>/` y `Assets/_Alumnos/Steering/` codigo de los alumnos (con sus `.meta` ya creados, para evitar conflictos de GUID).
- `docs/` paquete HTML con el plan, las 14 maquinas y los steering.
