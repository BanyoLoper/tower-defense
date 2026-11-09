// ============================================================
//  Estado.cs  -  El contrato que cumple CADA estado
// ============================================================
//
//  Un estado es una clase que hereda de Estado y sobreescribe
//  solo lo que necesita. Todo lo demas ya viene hecho.
//
//  Regla de los metodos que devuelven string:
//     return "NombreDeOtroEstado";  ->  quiero irme a ese estado
//     return null;                  ->  no pasa nada, me quedo
//  (devolver tu propio nombre tambien significa "me quedo")
//
public abstract class Estado
{
    // Datos compartidos con los demas estados
    public Contexto Ctx { get; set; }

    // El nombre es el de la clase: class Grande -> "Grande"
    public string Nombre { get { return GetType().Name; } }

    // Se llama UNA vez al entrar. 'anterior' es el estado de donde vengo
    // (null si es el estado inicial).
    public virtual void Entrar(string anterior) { }

    // Se llama UNA vez al salir. 'siguiente' es el estado al que voy.
    public virtual void Salir(string siguiente) { }

    // Se llama en cada frame mientras este es el estado actual.
    // dt = segundos desde el frame anterior. Util para temporizadores.
    public virtual string Actualizar(float dt) { return null; }

    // Se llama cuando ocurre un evento (dano, enemigo visto, orden...).
    // 'valor' acompana al evento. Si no hay, es 0.
    public virtual string AlRecibir(string trigger, int valor) { return null; }
}
