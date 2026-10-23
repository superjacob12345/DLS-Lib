using System.Globalization;

namespace DLS_Web.Hjaelpere
{
    // Små hjælpemetoder til at vise datoer pænt på dansk
    public static class Visning
    {
        private static CultureInfo _dansk = new CultureInfo("da-DK");

        // "Mandag 26. oktober"
        public static string Dag(DateTime tid)
        {
            string tekst = tid.ToString("dddd d. MMMM", _dansk);
            return char.ToUpper(tekst[0]) + tekst.Substring(1);
        }

        // "man. 26. okt. kl. 17:00"
        public static string Kort(DateTime tid)
        {
            return tid.ToString("ddd d. MMM 'kl.' HH:mm", _dansk);
        }

        public static string Klokken(DateTime tid)
        {
            return tid.ToString("HH:mm");
        }

        public static string Dato(DateOnly dato)
        {
            return dato.ToString("d. MMMM yyyy", _dansk);
        }
    }
}
