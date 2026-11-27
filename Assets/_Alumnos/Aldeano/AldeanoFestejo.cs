namespace Alumnos.Aldeano
{
    public class AldeanoFestejo : Estado
    {
        public override void Entrar(string anterior)
        {
            Agente ag = Ctx.Obtener<Agente>("agente");
            ag.cuerpo.Detener();
            bool victoria = Ctx.Obtener<bool>("victoria");
            ag.Animar(victoria ? Anim.Celebrar1 : Anim.Llorar);
        }
    }
}
