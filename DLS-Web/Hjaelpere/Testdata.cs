using DLS_Lib.Repository;
using DLS_Lib.UserStories;

namespace DLS_Web.Hjaelpere
{
    // Fylder repositories med eksempeldata, så man kan prøve siden med det samme.
    // Alle brugere har adgangskoden "1234".
    public static class Testdata
    {
        public static void Opret(IServiceProvider services)
        {
            TegnRepository tegnRepo = services.GetRequiredService<TegnRepository>();
            LederRepository lederRepo = services.GetRequiredService<LederRepository>();
            SpejderRepository spejderRepo = services.GetRequiredService<SpejderRepository>();
            AktivitetRepository aktivitetRepo = services.GetRequiredService<AktivitetRepository>();
            TilmeldingRepository tilmeldingRepo = services.GetRequiredService<TilmeldingRepository>();

            // Tegn får Id 1-5
            tegnRepo.Opret(new Tegn("Førstehjælp"));
            tegnRepo.Opret(new Tegn("Bålmester"));
            tegnRepo.Opret(new Tegn("Svømmer"));
            tegnRepo.Opret(new Tegn("Knob og reb"));
            tegnRepo.Opret(new Tegn("Kanosejler"));

            // Ledere får Id 1-3
            lederRepo.Opret(new Leder("Mette Hansen", "mette", "1234"));
            lederRepo.Opret(new Leder("Jonas Berg", "jonas", "1234"));
            lederRepo.Opret(new Leder("Aisha Khan", "aisha", "1234"));

            // Spejdere får Id 1-4
            spejderRepo.Opret(LavSpejder("Ida Larsen", "ida", new DateOnly(2014, 3, 12), new List<int> { 2, 3 }));
            spejderRepo.Opret(LavSpejder("Oliver Nielsen", "oliver", new DateOnly(2017, 8, 2), new List<int>()));
            spejderRepo.Opret(LavSpejder("Noah Pedersen", "noah", new DateOnly(2011, 1, 20), new List<int> { 1, 3, 5 }));
            spejderRepo.Opret(LavSpejder("Freja Madsen", "freja", new DateOnly(2013, 11, 5), new List<int> { 4 }));

            // Lejren starter førstkommende mandag
            int dageTilMandag = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
            if (dageTilMandag == 0)
            {
                dageTilMandag = 7;
            }
            DateTime mandag = DateTime.Today.AddDays(dageTilMandag);

            aktivitetRepo.Opret(LavAktivitet("Mad over bål", "Bålpladsen", "Vi laver snobrød og pandekager over åben ild. Medbring egen kop!",
                mandag.AddHours(17), 12, 8, new List<int> { 1 }, new List<int>()));
            aktivitetRepo.Opret(LavAktivitet("Ridetur på Hedegården", "Hedegården", "Besøg gården, mød dyrene og tag en tur på hesteryg rundt om markerne.",
                mandag.AddDays(1).AddHours(10), 6, 10, new List<int> { 2 }, new List<int>()));
            aktivitetRepo.Opret(LavAktivitet("Fang dyr i vandhullet", "Vandhullet", "Med net og spande undersøger vi hvad der lever i vandet – haletudser, vandkalve og meget mere.",
                mandag.AddDays(1).AddHours(14), 15, 6, new List<int> { 3 }, new List<int>()));
            aktivitetRepo.Opret(LavAktivitet("Kanotur på søen", "Hedeland Sø", "En hel formiddag på vandet. Kræver at man kan svømme og har kanotegnet.",
                mandag.AddDays(2).AddHours(9).AddMinutes(30), 8, 12, new List<int> { 2, 3 }, new List<int> { 3, 5 }));
            aktivitetRepo.Opret(LavAktivitet("Byg en svævebane", "Skoven", "Vi bygger en svævebane mellem træerne med reb og knob.",
                mandag.AddDays(3).AddHours(13), 10, 11, new List<int> { 3 }, new List<int> { 4 }));
            aktivitetRepo.Opret(LavAktivitet("Natløb med lommelygter", "Lejrens indgang", "Find posterne i mørket og løs opgaverne undervejs.",
                mandag.AddDays(3).AddHours(21), 20, 11, new List<int> { 1, 2 }, new List<int>()));
            aktivitetRepo.Opret(LavAktivitet("Lejrbål og sang", "Det store bål", "Ugens afslutning med sange, sketches og skumfiduser for hele lejren.",
                mandag.AddDays(4).AddHours(20), 200, 0, new List<int> { 1, 2, 3 }, new List<int>()));

            // Et par tilmeldinger, så der er noget at se
            tilmeldingRepo.Tilmeld(spejderRepo.HentEn(1)!, aktivitetRepo.HentEn(1)!);
            tilmeldingRepo.Tilmeld(spejderRepo.HentEn(1)!, aktivitetRepo.HentEn(3)!);
            tilmeldingRepo.Tilmeld(spejderRepo.HentEn(3)!, aktivitetRepo.HentEn(4)!);
            tilmeldingRepo.Tilmeld(spejderRepo.HentEn(4)!, aktivitetRepo.HentEn(5)!);
        }

        private static Spejder LavSpejder(string navn, string brugerNavn, DateOnly foedselsdato, List<int> tegnIds)
        {
            Spejder spejder = new Spejder(0, navn, brugerNavn, "1234", foedselsdato);
            spejder.TegnIds = tegnIds;
            return spejder;
        }

        private static Aktivitet LavAktivitet(string navn, string sted, string beskrivelse, DateTime tidspunkt,
            int maksDeltagere, int minAlder, List<int> lederIds, List<int> kraevedeTegnIds)
        {
            Aktivitet aktivitet = new Aktivitet(navn, tidspunkt, maksDeltagere, minAlder);
            aktivitet.Sted = sted;
            aktivitet.Beskrivelse = beskrivelse;
            aktivitet.LederIds = lederIds;
            aktivitet.KraevedeTegnIds = kraevedeTegnIds;
            return aktivitet;
        }
    }
}
