using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

// Construye Animator Controllers "tontos" (solo estados, sin transiciones ni parametros)
// y prefabs de agentes para los roles humanoides.
//   run GraneroTools.ConstruirAgentes.Todo
namespace GraneroTools
{
    public static class ConstruirAgentes
    {
        const string KI = "Assets/Third Party Assets/Kevin Iglesias/Human Animations/";
        const string Armas = KI + "Unity Demo Scenes/Human Melee Animations/Prefabs/Weapons/";
        const string Herr = KI + "Unity Demo Scenes/Human Crafting Animations/Prefabs/Tools/";
        const string Magia = KI + "Unity Demo Scenes/Human Spellcasting Animations/Prefabs/";
        const string Modelo = KI + "Models/HumanM_Model.fbx";
        const string Mat = "Assets/_Granero/Materiales/";

        class Def
        {
            public string rol; public string rolEnum; public string bando; public string mat;
            public float vida, vel, radio = 0.5f, escala = 1f, vista = 12f;
            public int gemas;
            public string der, izq;   // prefabs de armas
            public string[] anims;    // pares "Estado=NombreArchivo"
        }

        static string[] Comunes = {
            "Idle=HumanM@Idle01", "Caminar=HumanM@Walk01_Forward", "Correr=HumanM@Run01_Forward",
            "Dano1=HumanM@CombatDamage01", "Dano2=HumanM@CombatDamage02",
            "Muerte1=HumanM@CombatDeath01", "Muerte2=HumanM@CombatDeath02", "Muerte3=HumanM@CombatDeath03", "Muerte4=HumanM@CombatDeath04",
            "Aturdido=HumanM@Stun01", "IdleCombate=HumanM@CombatIdle01"
        };

        static string[] Unir(string[] a, params string[] b)
        {
            var l = new List<string>(a); l.AddRange(b); return l.ToArray();
        }

        static List<Def> Defs()
        {
            var d = new List<Def>();
            d.Add(new Def { rol = "Soldado", rolEnum = "Soldado", bando = "Enemigo", mat = "Enemigo_Soldado_Rojo", vida = 80, vel = 2.2f, gemas = 5,
                der = Armas + "Human_Sword.prefab", izq = Armas + "Human_Shield.prefab",
                anims = Unir(Comunes, "Atacar1=HumanM@Attack1H01_R", "Atacar2=HumanM@Attack1H02_R", "Bloquear=HumanM@BlockShield01 - Loop", "BloqueoGolpe=HumanM@BlockShield01 - Hit") });
            d.Add(new Def { rol = "Bruto", rolEnum = "Bruto", bando = "Enemigo", mat = "Enemigo_Bruto_Naranja", vida = 300, vel = 1.4f, radio = 0.7f, escala = 1.2f, gemas = 15,
                der = Armas + "Human_Greatsword.prefab",
                anims = Unir(Comunes, "IdleCombate=HumanM@CombatIdle2H01", "Atacar1=HumanM@Attack2H01", "Atacar2=HumanM@Attack2H02", "Atacar3=HumanM@Attack2H03",
                    "DerribadoCaida=HumanM@Knockdown01 - Fall", "DerribadoSuelo=HumanM@Knockdown01 - Ground", "DerribadoLevantar=HumanM@Knockdown01 - StandUp", "Enojado=HumanM@Angry01") });
            d.Add(new Def { rol = "Jefe", rolEnum = "Jefe", bando = "Enemigo", mat = "Enemigo_Jefe_Negro", vida = 1500, vel = 1.2f, radio = 1.0f, escala = 1.7f, gemas = 100,
                der = Armas + "Human_Warhammer.prefab",
                anims = Unir(Comunes, "IdleCombate=HumanM@CombatIdle2H01", "Atacar1=HumanM@Attack2H01", "Atacar2=HumanM@Attack2H02", "Atacar3=HumanM@Attack2H04",
                    "DerribadoCaida=HumanM@Knockdown01 - Fall", "DerribadoSuelo=HumanM@Knockdown01 - Ground", "DerribadoLevantar=HumanM@Knockdown01 - StandUp", "Enojado=HumanM@Angry02") });
            d.Add(new Def { rol = "Ladron", rolEnum = "Ladron", bando = "Enemigo", mat = "Enemigo_Ladron_Verde", vida = 50, vel = 3.2f, gemas = 8,
                der = Armas + "Human_Dagger.prefab",
                anims = Unir(Comunes, "Sigilo=HumanM@Crouch01_Walk_Forward", "Agachado=HumanM@Crouch01_Idle", "Sprint=HumanM@Sprint01_Forward",
                    "RobarInicio=HumanM@Loot01 - Begin", "RobarLoop=HumanM@Loot01 - Loop", "RobarFin=HumanM@Loot01 - Stop", "Atacar1=HumanM@Attack1H01_L", "Esquivar=HumanM@Dodge01") });
            d.Add(new Def { rol = "Lancero", rolEnum = "Lancero", bando = "Enemigo", mat = "Enemigo_Lancero_Amarillo", vida = 90, vel = 2.0f, gemas = 6,
                der = Armas + "Human_Polearm.prefab",
                anims = Unir(Comunes, "IdleCombate=HumanM@CombatIdlePolearm01", "Atacar1=HumanM@AttackPolearm01", "Atacar2=HumanM@AttackPolearm02", "Atacar3=HumanM@AttackPolearm03",
                    "Bloquear=HumanM@ParryPolearm01 - Loop", "BloqueoGolpe=HumanM@ParryPolearm01 - Hit") });
            d.Add(new Def { rol = "MagoEnemigo", rolEnum = "MagoEnemigo", bando = "Enemigo", mat = "Enemigo_Mago_Morado", vida = 60, vel = 1.6f, gemas = 10, vista = 14f,
                der = Magia + "Human_MageBook.prefab",
                anims = Unir(Comunes, "HechizoIdle=HumanM@CastingIdle01", "HechizoCargar=HumanM@MagicAttackDirect1H01_R - Load", "HechizoLanzar=HumanM@MagicAttackDirect1H01_R - Cast",
                    "HechizoOmni=HumanM@MagicAttackOmni01") });
            d.Add(new Def { rol = "TorreLanzadora", rolEnum = "TorreLanzadora", bando = "Defensor", mat = "Defensor_Azul", vida = 120, vel = 0f, vista = 16f,
                der = Armas + "Human_Polearm.prefab",
                anims = new[] { "Idle=HumanM@Idle01", "IdleCombate=HumanM@CombatIdle01", "LanzarPreparar=HumanM@ThrowSpear01_R - Hold", "Lanzar1=HumanM@ThrowSpear01_R",
                    "Lanzar2=HumanM@ThrowSpear02_R", "CambiarArma=HumanM@SpearChange01_R", "Dano1=HumanM@CombatDamage01", "Muerte1=HumanM@CombatDeath01" } });
            d.Add(new Def { rol = "TorreMaga", rolEnum = "TorreMaga", bando = "Defensor", mat = "Defensor_Azul", vida = 100, vel = 0f, vista = 16f,
                der = Magia + "Human_MageBook.prefab",
                anims = new[] { "Idle=HumanM@Idle01", "HechizoIdle=HumanM@CastingIdle01", "HechizoCargar=HumanM@MagicAttackDirect1H01_R - Load", "HechizoLanzar=HumanM@MagicAttackDirect1H01_R - Cast",
                    "HechizoOmni=HumanM@MagicAttackOmni01", "Dano1=HumanM@CombatDamage01", "Muerte1=HumanM@CombatDeath01" } });
            d.Add(new Def { rol = "Guardia", rolEnum = "Guardia", bando = "Defensor", mat = "Defensor_Azul", vida = 120, vel = 2.8f, gemas = 0,
                der = Armas + "Human_Sword.prefab", izq = Armas + "Human_Shield.prefab",
                anims = Unir(Comunes, "Atacar1=HumanM@Attack1H01_R", "Atacar2=HumanM@Attack1H02_R", "Bloquear=HumanM@BlockShield01 - Loop", "BloqueoGolpe=HumanM@BlockShield01 - Hit") });
            d.Add(new Def { rol = "Aldeano", rolEnum = "Aldeano", bando = "Aldeano", mat = "Aldeano_Gris", vida = 30, vel = 1.8f,
                anims = new[] { "Idle=HumanM@Idle01", "Caminar=HumanM@Walk01_Forward", "Correr=HumanM@Run01_Forward", "Miedo=HumanM@Fear01", "Celebrar1=HumanM@Cheer01", "Celebrar2=HumanM@Cheer02",
                    "Saludar=HumanM@HandWave01", "Llorar=HumanM@Cry01 - Loop", "Dano1=HumanM@CombatDamage01", "Muerte1=HumanM@CombatDeath01", "Herido=HumanM@IdleWounded01", "Recolectar=HumanM@Gathering01" } });
            d.Add(new Def { rol = "Minero", rolEnum = "Minero", bando = "Aldeano", mat = "Aldeano_Gris", vida = 40, vel = 2.0f,
                der = Herr + "Human_Pickaxe.prefab",
                anims = new[] { "Idle=HumanM@Idle01", "Caminar=HumanM@Walk01_Forward", "Correr=HumanM@Run01_Forward", "MinarSuelo=HumanM@MiningOneHand01_R - Ground", "MinarPared=HumanM@MiningOneHand01_R - Wall",
                    "Recolectar=HumanM@Gathering01", "Miedo=HumanM@Fear01", "Dano1=HumanM@CombatDamage01", "Muerte1=HumanM@CombatDeath01" } });
            d.Add(new Def { rol = "Reparador", rolEnum = "Reparador", bando = "Aldeano", mat = "Aldeano_Gris", vida = 40, vel = 2.2f,
                der = Herr + "Human_Hammer.prefab",
                anims = new[] { "Idle=HumanM@Idle01", "Caminar=HumanM@Walk01_Forward", "Correr=HumanM@Run01_Forward", "MartillarInicio=HumanM@HammeringGround01_R - Begin", "MartillarLoop=HumanM@HammeringGround01_R - Loop",
                    "MartillarFin=HumanM@HammeringGround01_R - Stop", "Miedo=HumanM@Fear01", "Dano1=HumanM@CombatDamage01", "Muerte1=HumanM@CombatDeath01" } });
            return d;
        }

        static AnimationClip BuscarClip(string nombre, StringBuilder log)
        {
            foreach (string g in AssetDatabase.FindAssets(nombre + " t:Model", new[] { KI + "Animations/Male" }))
            {
                string p = AssetDatabase.GUIDToAssetPath(g);
                if (p.Contains("RootMotion") || p.Contains("[RM]")) continue;
                if (System.IO.Path.GetFileNameWithoutExtension(p) != nombre) continue;
                foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(p))
                {
                    AnimationClip c = o as AnimationClip;
                    if (c != null && !c.name.StartsWith("__preview__")) return c;
                }
            }
            log.AppendLine("  FALTA clip: " + nombre);
            return null;
        }

        static Transform Hueso(Transform raiz, string nombre)
        {
            foreach (Transform t in raiz.GetComponentsInChildren<Transform>(true)) if (t.name == nombre) return t;
            return null;
        }

        static void CrearCarpeta(string ruta)
        {
            string[] partes = ruta.Split('/');
            string acum = partes[0];
            for (int i = 1; i < partes.Length; i++)
            {
                string sig = acum + "/" + partes[i];
                if (!AssetDatabase.IsValidFolder(sig)) AssetDatabase.CreateFolder(acum, partes[i]);
                acum = sig;
            }
        }

        public static string Todo(string arg)
        {
            var log = new StringBuilder();
            CrearCarpeta("Assets/_Granero/Animadores");
            CrearCarpeta("Assets/_Granero/Prefabs/Agentes");
            GameObject modelo = AssetDatabase.LoadAssetAtPath<GameObject>(Modelo);
            if (modelo == null) return "ERROR no se encontro " + Modelo;

            foreach (Def d in Defs())
            {
                if (!string.IsNullOrEmpty(arg) && arg != d.rol) continue;
                log.AppendLine(d.rol);

                // ---- controller sin transiciones
                string cp = "Assets/_Granero/Animadores/" + d.rol + ".controller";
                AssetDatabase.DeleteAsset(cp);
                AnimatorController ctrl = AnimatorController.CreateAnimatorControllerAtPath(cp);
                AnimatorStateMachine sm = ctrl.layers[0].stateMachine;
                var nombres = new List<string>(); var durs = new List<float>();
                int idx = 0;
                foreach (string par in d.anims)
                {
                    string[] kv = par.Split('=');
                    AnimationClip clip = BuscarClip(kv[1], log);
                    if (clip == null) continue;
                    AnimatorState st = sm.AddState(kv[0], new Vector3(300 + (idx % 4) * 220, (idx / 4) * 60, 0));
                    st.motion = clip;
                    if (idx == 0) sm.defaultState = st;
                    nombres.Add(kv[0]); durs.Add(clip.length);
                    if ((kv[0] == "Idle" || kv[0] == "Caminar" || kv[0] == "Correr") && !clip.isLooping) log.AppendLine("  no loopea: " + kv[0]);
                    idx++;
                }
                EditorUtility.SetDirty(ctrl);

                // ---- prefab
                GameObject raiz = new GameObject(d.rol);
                GameObject m = (GameObject)PrefabUtility.InstantiatePrefab(modelo);
                m.name = "Modelo";
                m.transform.SetParent(raiz.transform, false);
                m.transform.localScale = Vector3.one * d.escala;

                Material color = AssetDatabase.LoadAssetAtPath<Material>(Mat + d.mat + ".mat");
                foreach (Renderer r in m.GetComponentsInChildren<Renderer>(true))
                {
                    var mats = new Material[r.sharedMaterials.Length];
                    for (int i = 0; i < mats.Length; i++) mats[i] = color;
                    r.sharedMaterials = mats;
                }

                Animator an = m.GetComponentInChildren<Animator>();
                an.runtimeAnimatorController = ctrl;
                an.applyRootMotion = false;

                AnadirArma(m.transform, "B-handProp.R", d.der, log);
                AnadirArma(m.transform, "B-handProp.L", d.izq, log);

                Salud s = raiz.AddComponent<Salud>(); s.vidaMax = d.vida; s.vida = d.vida;
                Cuerpo c = raiz.AddComponent<Cuerpo>(); c.velocidadMax = d.vel > 0 ? d.vel : 1f;
                Agente a = raiz.AddComponent<Agente>();
                a.rol = (Rol)System.Enum.Parse(typeof(Rol), d.rolEnum);
                a.bando = (Bando)System.Enum.Parse(typeof(Bando), d.bando);
                a.radio = d.radio; a.recompensaGemas = d.gemas; a.vistaMetros = d.vista;
                a.alturaPecho = 1.3f * d.escala;
                a.estadosAnimacion = nombres.ToArray(); a.duracionesAnimacion = durs.ToArray();
                BarraVida bv = raiz.AddComponent<BarraVida>(); bv.altura = 2.2f * d.escala;
                raiz.AddComponent<ControladorAuto>();

                string pp = "Assets/_Granero/Prefabs/Agentes/" + d.rol + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(raiz, pp);
                Object.DestroyImmediate(raiz);
                log.AppendLine("  OK " + nombres.Count + " estados -> " + pp);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            System.IO.File.WriteAllText("BridgeCmd/agentes.log", log.ToString());
            return "listo, ver BridgeCmd/agentes.log";
        }

        static void AnadirArma(Transform modelo, string hueso, string prefab, StringBuilder log)
        {
            if (string.IsNullOrEmpty(prefab)) return;
            Transform h = Hueso(modelo, hueso);
            GameObject w = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
            if (h == null) { log.AppendLine("  FALTA hueso " + hueso); return; }
            if (w == null) { log.AppendLine("  FALTA arma " + prefab); return; }
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(w);
            inst.transform.SetParent(h, false);
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localRotation = Quaternion.identity;
        }
    }
}
