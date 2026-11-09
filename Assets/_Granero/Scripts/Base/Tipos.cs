// Tipos basicos del juego "Granero de Cristales".

public enum Bando { Enemigo, Defensor, Aldeano, Neutral }

// Un Rol = una maquina de estados (un controller). El nombre del controller es <Rol>Controller,
// salvo Jefe, que comparte BrutoController, y los sistemas (GameManager y Director).
public enum Rol
{
    Soldado, Bruto, Jefe, Ladron, Lancero, MagoEnemigo,
    TorreLanzadora, TorreMaga, Guardia,
    Aldeano, Minero, Reparador,
    Granero
}

// Nombres de los estados del Animator. Los Animator Controllers de los prefabs NO tienen
// transiciones: el estado de codigo decide que animacion se ve con agente.Animar(Anim.Xxx).
// Cada rol solo tiene los estados que usa (ver agente.estadosAnimacion en el inspector).
public static class Anim
{
    // Comunes
    public const string Idle = "Idle";
    public const string IdleCombate = "IdleCombate";
    public const string Caminar = "Caminar";
    public const string Correr = "Correr";
    public const string Sprint = "Sprint";
    public const string Sigilo = "Sigilo";
    public const string Agachado = "Agachado";
    public const string Dano1 = "Dano1";
    public const string Dano2 = "Dano2";
    public const string Aturdido = "Aturdido";
    public const string DerribadoCaida = "DerribadoCaida";
    public const string DerribadoSuelo = "DerribadoSuelo";
    public const string DerribadoLevantar = "DerribadoLevantar";
    public const string Muerte1 = "Muerte1";
    public const string Muerte2 = "Muerte2";
    public const string Muerte3 = "Muerte3";
    public const string Muerte4 = "Muerte4";
    public const string Esquivar = "Esquivar";
    public const string Herido = "Herido";

    // Combate cuerpo a cuerpo
    public const string Atacar1 = "Atacar1";
    public const string Atacar2 = "Atacar2";
    public const string Atacar3 = "Atacar3";
    public const string Bloquear = "Bloquear";
    public const string BloqueoGolpe = "BloqueoGolpe";

    // Social
    public const string Miedo = "Miedo";
    public const string Celebrar1 = "Celebrar1";
    public const string Celebrar2 = "Celebrar2";
    public const string Enojado = "Enojado";
    public const string Saludar = "Saludar";
    public const string Hablar = "Hablar";
    public const string Llorar = "Llorar";

    // Ladron
    public const string RobarInicio = "RobarInicio";
    public const string RobarLoop = "RobarLoop";
    public const string RobarFin = "RobarFin";

    // Trabajo
    public const string MinarSuelo = "MinarSuelo";
    public const string MinarPared = "MinarPared";
    public const string Recolectar = "Recolectar";
    public const string MartillarInicio = "MartillarInicio";
    public const string MartillarLoop = "MartillarLoop";
    public const string MartillarFin = "MartillarFin";

    // Lanzamiento
    public const string Lanzar1 = "Lanzar1";
    public const string Lanzar2 = "Lanzar2";
    public const string LanzarPreparar = "LanzarPreparar";
    public const string CambiarArma = "CambiarArma";

    // Hechizos
    public const string HechizoIdle = "HechizoIdle";
    public const string HechizoEntrar = "HechizoEntrar";
    public const string HechizoSalir = "HechizoSalir";
    public const string HechizoCargar = "HechizoCargar";
    public const string HechizoLanzar = "HechizoLanzar";
    public const string HechizoOmni = "HechizoOmni";
}
