#!/usr/bin/env python3
# Revisa UNA entrega (una rama) contra .github/entregas/criterios.json.
# Solo LEE archivos de texto con git (nunca ejecuta codigo de los alumnos).
import argparse, json, os, re, subprocess, sys
from datetime import datetime, timezone
try:
    from zoneinfo import ZoneInfo
except ImportError:  # pragma: no cover
    ZoneInfo = None

CRIT = json.load(open(".github/entregas/criterios.json", encoding="utf-8"))["cupos"]
CAL = json.load(open(".github/entregas/calendario.json", encoding="utf-8"))
RAMA_RE = re.compile(r"^clase-([1-6])/(0[1-9]|1[0-4])-([a-z]+)$")
BASE = "Assets/_Alumnos/"


def git(*args, check=False):
    r = subprocess.run(["git"] + list(args), capture_output=True)
    if check and r.returncode != 0:
        raise RuntimeError(r.stderr.decode("utf-8", "replace"))
    return r.stdout


def listar(ref):
    out = git("ls-tree", "-r", "--name-only", ref, "--", "Assets/_Alumnos").decode("utf-8", "replace")
    return set(l for l in out.splitlines() if l)


def leer(ref, path):
    return git("show", "%s:%s" % (ref, path))


def cambiados(base, ref):
    out = git("diff", "--name-status", "-M", "%s...%s" % (base, ref)).decode("utf-8", "replace")
    res = []
    for l in out.splitlines():
        p = l.split("\t")
        if len(p) >= 2:
            res.append(p[-1])
            if p[0].startswith("R"):
                res.append(p[1])
    return sorted(set(res))


TIPO_RE = re.compile(r"\b(?:class|struct|enum|interface)\s+([A-Za-z_]\w*)")


def tipos_instructor(base):
    out = git("ls-tree", "-r", "--name-only", base, "--", "Assets/_Granero").decode("utf-8", "replace")
    nombres = set()
    for p in out.splitlines():
        if p.endswith(".cs"):
            nombres.update(TIPO_RE.findall(leer(base, p).decode("utf-8", "replace")))
    return nombres


def cuerpo(src, metodo):
    m = re.search(r"\b" + metodo + r"\s*\([^)]*\)\s*\{", src)
    if not m:
        return ""
    i, d = m.end(), 1
    while i < len(src) and d > 0:
        d += (src[i] == "{") - (src[i] == "}")
        i += 1
    return src[m.end():i]


def parse_iso(s):
    if not s:
        return None
    return datetime.fromisoformat(s.replace("Z", "+00:00"))


def revisar(ref, rama, creado=None, base="origin/main"):
    items = []   # (nivel, texto)  nivel: ok | falta | aviso | info
    add = lambda n, t: items.append((n, t))
    res = dict(rama=rama, valido=False, cupo=None, clave=None, clase=None, tarde=False, ok=0, falta=0, aviso=0,
               est_ok=0, est_tot=0, st_ok=0, st_tot=0, items=items)

    m = RAMA_RE.match(rama)
    if not m:
        add("falta", "Nombre de rama invalido: `%s`. Debe ser `clase-<1..6>/<cupo 01..14>-<maquina en minusculas>`, por ejemplo `clase-2/05-magoenemigo`." % rama)
        return cerrar(res)
    clase, cupo, nombre = int(m.group(1)), m.group(2), m.group(3)
    c = CRIT[cupo]
    res.update(valido=True, cupo=cupo, clave=c["clave"], clase=clase)
    if nombre != c["clave"].lower():
        add("falta", "El cupo %s es **%s**: la rama debe terminar en `-%s` (esta termina en `-%s`)." % (cupo, c["clave"], c["clave"].lower(), nombre))
        return cerrar(res)
    add("ok", "Nombre de rama correcto (clase %d, cupo %s, %s)." % (clase, cupo, c["clave"]))

    # ---- fecha limite
    cl = CAL["clases"][str(clase)]
    tz = ZoneInfo(CAL.get("zona_horaria", "America/Mexico_City")) if ZoneInfo else timezone.utc
    lim = datetime.strptime(cl["fecha_limite"] + " " + cl.get("hora_limite", "23:59"), "%Y-%m-%d %H:%M").replace(tzinfo=tz)
    c0 = parse_iso(creado)
    manual = c0 is None
    if manual:
        c0 = datetime.now(timezone.utc)
    c0 = c0.astimezone(tz)
    txt = "PR creado %s; limite de la clase %d: %s." % (c0.strftime("%Y-%m-%d %H:%M"), clase, lim.strftime("%Y-%m-%d %H:%M"))
    if manual:
        txt += " (revision manual sin PR: se uso la hora actual)"
    if c0 <= lim:
        add("ok", "Entrega a tiempo. " + txt)
    else:
        res["tarde"] = True
        add("falta", "Entrega TARDIA. " + txt)

    tree = listar(ref)
    carpeta = BASE + c["carpeta"] + "/"
    propios = set(f for f in tree if f.startswith(carpeta) and f.endswith(".cs"))

    # ---- archivos modificados fuera de lo permitido
    permitidos = lambda p: p.startswith(carpeta) or p == BASE + c["carpeta"] + ".meta" or any(
        p in (BASE + "Steering/Steering%s.cs" % s["nombre"], BASE + "Steering/Steering%s.cs.meta" % s["nombre"]) for s in c["steering"])
    ajenos = [p for p in cambiados(base, ref) if not permitidos(p)]
    if ajenos:
        lst = ", ".join("`%s`" % p for p in ajenos[:12]) + (" ..." if len(ajenos) > 12 else "")
        add("falta", "Modificaste %d archivo(s) que no son tuyos (solo puedes tocar `%s` y tus steering): %s. Usa `git restore` para quitarlos del PR." % (len(ajenos), carpeta, lst))
    else:
        add("ok", "Solo modificaste archivos de tu carpeta.")

    # ---- fuentes
    src = {}
    for f in propios:
        raw = leer(ref, f)
        src[f] = raw.decode("utf-8", "replace")
        if any(b > 127 for b in raw):
            add("aviso", "`%s` tiene caracteres no ASCII (acentos, enes, simbolos). El codigo va en ASCII." % f)

    # ---- controller
    ctrl_path = carpeta + c["controller"] + ".cs"
    ctrl = src.get(ctrl_path)
    if ctrl is None:
        add("falta", "Falta el controller `%s`." % ctrl_path)
    else:
        if not re.search(r"class\s+%s\s*:\s*MonoBehaviour" % c["controller"], ctrl):
            add("falta", "`%s` debe declarar `class %s : MonoBehaviour`." % (ctrl_path, c["controller"]))
        elif "new Maquina(" not in ctrl or ".Iniciar(" not in ctrl:
            add("falta", "El controller debe crear la maquina (`new Maquina(...)`) e iniciarla (`Iniciar(...)`).")
        else:
            add("ok", "Controller `%s` presente y arma la maquina." % c["controller"])

    # ---- estados esperados hasta esta clase
    esp = [e for e in c["estados"] if e["clase"] <= clase]
    nuevos = [e for e in esp if e["clase"] == clase]
    res["est_tot"] = len(c["estados"])
    todo = "\n".join(src.values())
    for e in esp:
        cn = c["clave"] + e["nombre"]
        marca = " (nuevo esta clase)" if e in nuevos else ""
        decl = re.search(r"class\s+%s\s*:\s*Estado\b" % cn, todo)
        if not decl:
            add("falta", "Falta el estado `%s`%s: debe ser `class %s : Estado` en `%s%s.cs`." % (cn, marca, cn, carpeta, cn))
            continue
        res["est_ok"] += 1
        extras = []
        if (carpeta + cn + ".cs") not in src:
            extras.append("el archivo deberia llamarse `%s.cs` (un archivo por estado)" % cn)
        if ctrl is not None and ("new %s(" % cn) not in ctrl:
            add("falta", "`%s` existe pero no esta registrado en la lista de estados del controller (`new %s()`)." % (cn, cn))
            res["est_ok"] -= 1
            continue
        add("ok", "Estado `%s`%s." % (cn, marca))
        if extras:
            add("aviso", "%s: %s." % (cn, "; ".join(extras)))
    for e in c["estados"]:
        if e["clase"] > clase:
            pass  # todavia no se pide

    # ---- steering propios
    for s in c["steering"]:
        if s["clase"] > clase:
            continue
        res["st_tot"] += 1
        p = BASE + "Steering/Steering%s.cs" % s["nombre"]
        if p not in tree:
            add("falta", "Falta el steering `%s` (clase %d): `%s`." % (s["nombre"], s["clase"], p))
            continue
        t = leer(ref, p).decode("utf-8", "replace")
        if any(b > 127 for b in leer(ref, p)):
            add("aviso", "`%s` tiene caracteres no ASCII." % p)
        if re.search(r"\bnamespace\b", t):
            add("falta", "`%s` NO debe tener namespace: la clase `Steering` es global (partial)." % p)
            continue
        if not re.search(r"partial\s+class\s+Steering\b", t) or not re.search(r"static\s+Vector3\s+%s\s*\(" % s["nombre"], t):
            add("falta", "`%s` debe ser `public static partial class Steering` con `public static Vector3 %s(...)`." % (p, s["nombre"]))
            continue
        res["st_ok"] += 1
        add("ok", "Steering `%s` presente." % s["nombre"])

    # ---- namespace y nombres de clases
    ns = "Alumnos." + c["clave"]
    sin_ns = [f for f, t in sorted(src.items()) if not re.search(r"namespace\s+%s\b" % re.escape(ns), t)]
    if sin_ns:
        add("falta", "Estos archivos deben estar dentro de `namespace %s { ... }`: %s." % (ns, ", ".join("`%s`" % f for f in sin_ns[:8])))
    elif src:
        add("ok", "Todo tu codigo esta en `namespace %s`." % ns)
    try:
        reservados = tipos_instructor(base)
    except Exception:
        reservados = set()
    propios_tipos = {}
    for f, t in sorted(src.items()):
        for n in TIPO_RE.findall(t):
            propios_tipos.setdefault(n, f)
    choques = sorted(n for n in propios_tipos if n in reservados)
    if choques:
        add("falta", "Nombres de clase que ya existen en el juego base (tapan a la del instructor): %s. Renombralas con el prefijo `%s`." % (", ".join("`%s`" % n for n in choques), c["clave"]))
    sin_prefijo = sorted(n for n in propios_tipos if not (n.startswith(c["clave"]) or n.startswith("Probe")) and n not in reservados)
    if sin_prefijo:
        add("aviso", "Clases sin el prefijo `%s`: %s." % (c["clave"], ", ".join("`%s`" % n for n in sin_prefijo[:8])))

    # ---- calidad (avisos, no bloquean)
    for f, t in sorted(src.items()):
        if "System.Linq" in t:
            add("aviso", "`%s` usa LINQ (genera basura)." % f)
        act = cuerpo(t, "Actualizar") + cuerpo(t, "Update")
        if re.search(r"\bGetComponent\b|\bFindObject|GameObject\.Find|\bFind\(|FindFirstObjectByType", act):
            add("aviso", "`%s`: GetComponent/Find dentro de Actualizar. Guarda la referencia en Entrar o en el Contexto." % f)
        if re.search(r"\bnew\s+(List|Dictionary|HashSet|string|[A-Z]\w*\s*\[)", act) or re.search(r"\.To(List|Array)\(", act):
            add("aviso", "`%s`: crea objetos (`new`) dentro de Actualizar. Reutiliza memoria." % f)
        if "+=" in t and re.search(r"\.Al\w+\s*\+=", t) and not re.search(r"\.Al\w+\s*-=", t):
            add("aviso", "`%s` se suscribe a eventos (`.AlAlgo +=`) pero no se desuscribe (`-=`)." % f)
    for f in sorted(propios):
        if (f + ".meta") not in tree:
            add("aviso", "Falta `%s.meta`. Sube tambien el .meta (Unity lo genera solo al abrir el proyecto)." % f)
    return cerrar(res)


def cerrar(res):
    for n, _ in res["items"]:
        if n in res:
            res[n] += 1
    res["resultado"] = "CUMPLE" if res["falta"] == 0 and res["valido"] else "INCOMPLETA"
    return res


ICON = {"ok": "✅", "falta": "❌", "aviso": "⚠️", "info": "ℹ️"}


def markdown(res, base_url=None):
    L = ["<!-- revision-entrega -->"]
    if not res["valido"]:
        L.append("## Revision de entrega: rama no valida\n")
    else:
        t = "CUMPLE" if res["resultado"] == "CUMPLE" else "INCOMPLETA"
        L.append("## Revision de entrega: clase %d - cupo %s (%s) - **%s**\n" % (res["clase"], res["cupo"], res["clave"], t))
        ne = res["est_ok"]
        L.append("**Estados:** %d/%d esperados hasta esta clase &nbsp;|&nbsp; **Steering:** %d/%d &nbsp;|&nbsp; **A tiempo:** %s &nbsp;|&nbsp; Faltas: %d &nbsp;|&nbsp; Avisos: %d\n" % (
            ne, esperados(res), res["st_ok"], res["st_tot"], "no" if res["tarde"] else "si", res["falta"], res["aviso"]))
    faltas = [t for n, t in res["items"] if n == "falta"]
    if faltas:
        L.append("### Falta o no cumple")
        L += ["- ❌ " + t for t in faltas]
        L.append("")
    oks = [t for n, t in res["items"] if n == "ok"]
    if oks:
        L.append("### Cumple")
        L += ["- ✅ " + t for t in oks]
        L.append("")
    av = [t for n, t in res["items"] if n == "aviso"]
    if av:
        L.append("### Avisos (no bloquean, pero se califican en codigo y rendimiento)")
        L += ["- ⚠️ " + t for t in av]
        L.append("")
    L.append("<sub>Revision automatica de la parte estructural. La calificacion final (funciona en el juego, 50 pts) la da el instructor al probar en Unity.</sub>")
    return "\n".join(L)


def esperados(res):
    c = CRIT.get(res["cupo"]) if res["cupo"] else None
    if not c:
        return 0
    return len([e for e in c["estados"] if e["clase"] <= res["clase"]])


if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--ref", required=True, help="commit o ref de la rama (ej. origin/pr-5)")
    ap.add_argument("--rama", required=True, help="nombre de la rama (ej. clase-2/05-magoenemigo)")
    ap.add_argument("--creado", default="", help="fecha ISO de creacion del PR")
    ap.add_argument("--base", default="origin/main")
    ap.add_argument("--salida", default="reporte.md")
    a = ap.parse_args()
    r = revisar(a.ref, a.rama, a.creado or None, a.base)
    md = markdown(r)
    open(a.salida, "w", encoding="utf-8").write(md)
    print(md)
    sm = os.environ.get("GITHUB_STEP_SUMMARY")
    if sm:
        open(sm, "a", encoding="utf-8").write(md + "\n")
    sys.exit(0 if r["resultado"] == "CUMPLE" else 1)
