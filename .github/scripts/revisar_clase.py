#!/usr/bin/env python3
# Revision MANUAL de todas las entregas de una clase: busca los PR de ramas clase-N/* y los revisa.
import argparse, csv, json, os, subprocess, sys
sys.path.insert(0, os.path.dirname(__file__))
import revisar_entrega as R

ap = argparse.ArgumentParser()
ap.add_argument("--clase", type=int, required=True)
ap.add_argument("--base", default="origin/main")
a = ap.parse_args()

prs = json.loads(subprocess.run(["gh", "pr", "list", "--state", "all", "--limit", "300", "--json",
                                 "number,headRefName,createdAt,author,url,state,mergedAt"], capture_output=True, text=True, check=True).stdout)
prefijo = "clase-%d/" % a.clase
por_cupo = {}
for p in prs:
    h = p["headRefName"]
    if not h.startswith(prefijo) or (p["state"] == "CLOSED" and not p.get("mergedAt")):
        continue
    m = R.RAMA_RE.match(h)
    cupo = m.group(2) if m else "??"
    por_cupo.setdefault(cupo, []).append(p)

cl = R.CAL["clases"][str(a.clase)]
filas, md = [], ["# Revision de la clase %d  (limite: %s %s)\n" % (a.clase, cl["fecha_limite"], cl.get("hora_limite", "23:59")),
                 "| Cupo | Maquina | PR | Autor | Estados | Steering | A tiempo | Faltas | Avisos | Resultado |", "|---|---|---|---|---|---|---|---|---|---|"]
detalle = []
for cupo in sorted(R.CRIT):
    c = R.CRIT[cupo]
    ps = sorted(por_cupo.get(cupo, []), key=lambda p: p["createdAt"])
    if not ps:
        md.append("| %s | %s | - | - | 0/%d | - | - | - | - | ❌ SIN ENTREGA |" % (cupo, c["clave"], len([e for e in c["estados"] if e["clase"] <= a.clase])))
        filas.append([cupo, c["clave"], "", "", 0, "", "", "", "", "SIN ENTREGA"])
        continue
    p = ps[0]   # el primer PR cuenta para la fecha limite
    subprocess.run(["git", "fetch", "-q", "origin", "+refs/pull/%d/head:refs/remotes/origin/pr-%d" % (p["number"], p["number"])])
    ref = "origin/pr-%d" % p["number"]
    r = R.revisar(ref, p["headRefName"], p["createdAt"], a.base)
    aut = (p.get("author") or {}).get("login", "?")
    md.append("| %s | %s | [#%d](%s) | %s | %d/%d | %d/%d | %s | %d | %d | %s |" % (
        cupo, c["clave"], p["number"], p["url"], aut, r["est_ok"], R.esperados(r), r["st_ok"], r["st_tot"],
        "no" if r["tarde"] else "si", r["falta"], r["aviso"], "✅ CUMPLE" if r["resultado"] == "CUMPLE" else "❌ INCOMPLETA"))
    filas.append([cupo, c["clave"], p["number"], aut, "%d/%d" % (r["est_ok"], R.esperados(r)), "%d/%d" % (r["st_ok"], r["st_tot"]), "no" if r["tarde"] else "si", r["falta"], r["aviso"], r["resultado"]])
    detalle.append("<details><summary>Cupo %s - %s - PR #%d</summary>\n\n%s\n\n</details>" % (cupo, c["clave"], p["number"], R.markdown(r).replace("<!-- revision-entrega -->", "")))

md.append("\n## Detalle por entrega\n")
md += detalle
out = "\n".join(md)
open("reporte_clase.md", "w", encoding="utf-8").write(out)
with open("entregas_clase%d.csv" % a.clase, "w", newline="", encoding="utf-8") as f:
    w = csv.writer(f)
    w.writerow(["cupo", "maquina", "pr", "autor", "estados", "steering", "a_tiempo", "faltas", "avisos", "resultado"])
    w.writerows(filas)
print(out)
sm = os.environ.get("GITHUB_STEP_SUMMARY")
if sm:
    open(sm, "a", encoding="utf-8").write(out + "\n")
