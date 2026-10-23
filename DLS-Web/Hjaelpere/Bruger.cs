using DLS_Lib.Repository;
using DLS_Lib.UserStories;

namespace DLS_Web.Hjaelpere
{
    // Holder styr på hvem der er logget ind. Oplysningerne gemmes i session.
    public static class Bruger
    {
        public const string Spejder = "Spejder";
        public const string Leder = "Leder";

        public static void LogInd(ISession session, int id, string navn, string rolle)
        {
            session.SetInt32("BrugerId", id);
            session.SetString("Navn", navn);
            session.SetString("Rolle", rolle);
        }

        public static void LogUd(ISession session)
        {
            session.Clear();
        }

        // "Spejder", "Leder" eller null hvis ingen er logget ind
        public static string? Rolle(ISession session)
        {
            return session.GetString("Rolle");
        }

        public static int Id(ISession session)
        {
            int? id = session.GetInt32("BrugerId");
            if (id == null)
            {
                return 0;
            }
            return id.Value;
        }

        public static string Navn(ISession session)
        {
            string? navn = session.GetString("Navn");
            if (navn == null)
            {
                return "";
            }
            return navn;
        }

        public static bool ErLeder(ISession session)
        {
            return Rolle(session) == Leder;
        }

        public static bool ErSpejder(ISession session)
        {
            return Rolle(session) == Spejder;
        }

        // Brugernavne skal være unikke på tværs af spejdere og ledere.
        // egenSpejderId / egenLederId er den man er ved at rette (så man ikke kolliderer med sig selv).
        public static bool BrugerNavnOptaget(string brugerNavn, SpejderRepository spejdere, LederRepository ledere, int egenSpejderId, int egenLederId)
        {
            foreach (Spejder s in spejdere.HentAlle())
            {
                if (s.Id != egenSpejderId && s.BrugerNavn.ToLower() == brugerNavn.ToLower())
                {
                    return true;
                }
            }
            foreach (Leder l in ledere.HentAlle())
            {
                if (l.Id != egenLederId && l.BrugerNavn.ToLower() == brugerNavn.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
