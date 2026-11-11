namespace Alumnos.Aldeano
{
    public class AldeanoAsustarse : Estado
    {
        float restante;

        public override void Entrar(string anterior)
        {
            restante = 0.8f;
            Agente ag = Ctx.Obtener<Agente>("agente");
            ag.cuerpo.Detener();
            ag.Animar(Anim.Miedo, 0.1f, true);
        }

        public override string Actualizar(float dt)
        {
            restante -= dt;
            // Clase 2: volver a Vagar hasta implementar Huir en la clase 3.
            return restante <= 0f ? "AldeanoVagar" : null;
        }
    }
}
