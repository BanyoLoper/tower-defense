using System;

// Gemas del jugador. Estatico para que cualquiera la consulte: Economia.Gemas
public static class Economia
{
    public static int Gemas { get; private set; }
    public static event Action<int> AlCambiar;

    public static void Reiniciar(int inicial)
    {
        Gemas = inicial;
        if (AlCambiar != null) AlCambiar(Gemas);
    }

    public static void Agregar(int n)
    {
        Gemas += n;
        if (AlCambiar != null) AlCambiar(Gemas);
    }

    public static bool Gastar(int n)
    {
        if (Gemas < n) return false;
        Gemas -= n;
        if (AlCambiar != null) AlCambiar(Gemas);
        return true;
    }

    // Un ladron roba: devuelve cuanto pudo robar realmente.
    public static int Robar(int n)
    {
        int real = Math.Min(n, Gemas);
        if (real <= 0) return 0;
        Gemas -= real;
        if (AlCambiar != null) AlCambiar(Gemas);
        return real;
    }
}
