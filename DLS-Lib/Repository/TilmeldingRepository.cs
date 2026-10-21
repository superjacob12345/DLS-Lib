using System;
using System.Collections.Generic;
using System.Text;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class TilmeldingRepository
    {
        // Listen med alle tilmeldinger
        private List<Tilmelding> _tilmeldinger = new List<Tilmelding>();

        public List<Tilmelding> HentAlle()
        {
            return _tilmeldinger;
        }

        // Alle tilmeldinger for én spejder (spejderen ser sine aktiviteter)
        public List<Tilmelding> HentForSpejder(int spejderId)
        {
            List<Tilmelding> resultat = new List<Tilmelding>();
            foreach (Tilmelding t in _tilmeldinger)
            {
                if (t.SpejderId == spejderId)
                {
                    resultat.Add(t);
                }
            }
            return resultat;
        }

        // Alle tilmeldinger til én aktivitet (lederen ser deltagerne)
        public List<Tilmelding> HentForAktivitet(int aktivitetId)
        {
            List<Tilmelding> resultat = new List<Tilmelding>();
            foreach (Tilmelding t in _tilmeldinger)
            {
                if (t.AktivitetId == aktivitetId)
                {
                    resultat.Add(t);
                }
            }
            return resultat;
        }

        // Er spejderen allerede tilmeldt aktiviteten?
        public bool ErTilmeldt(int spejderId, int aktivitetId)
        {
            foreach (Tilmelding t in _tilmeldinger)
            {
                if (t.SpejderId == spejderId && t.AktivitetId == aktivitetId)
                {
                    return true;
                }
            }
            return false;
        }

        // Tjekker reglerne og tilmelder spejderen.
        // Returnerer en fejlbesked, eller "" hvis det gik godt.
        public string Tilmeld(Spejder spejder, Aktivitet aktivitet)
        {
            if (ErTilmeldt(spejder.Id, aktivitet.Id))
            {
                return "Du er allerede tilmeldt denne aktivitet";
            }

            if (HentForAktivitet(aktivitet.Id).Count >= aktivitet.MaksDeltagere)
            {
                return "Aktiviteten er fuld";
            }

            if (spejder.Alder < aktivitet.MinAlder)
            {
                return "Du er for ung til denne aktivitet";
            }

            DateOnly idag = DateOnly.FromDateTime(DateTime.Today);
            _tilmeldinger.Add(new Tilmelding(spejder.Id, aktivitet.Id, idag));
            return "";
        }

        // Fjerner spejderens tilmelding til aktiviteten
        public void Frameld(int spejderId, int aktivitetId)
        {
            foreach (Tilmelding t in _tilmeldinger)
            {
                if (t.SpejderId == spejderId && t.AktivitetId == aktivitetId)
                {
                    _tilmeldinger.Remove(t);
                    return; // stop løkken, ellers giver Remove en fejl
                }
            }
        }
    }
}
