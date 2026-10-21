using System;
using System.Collections.Generic;
using System.Text;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class LederRepository
    {
        // Listen med alle ledere
        private List<Leder> _ledere = new List<Leder>();

        // Næste Id der bliver givet 
        private int _næsteId = 1;

        public List<Leder> HentAlle()
        {
            return _ledere;
        }

        // Giver lederen et Id og gemmer den
        public void Opret(Leder leder)
        {
            leder.Id = _næsteId;
            _næsteId++;
            _ledere.Add(leder);
        }

        // Finder en leder ud fra Id (null hvis den ikke findes)
        public Leder? HentEn(int id)
        {
            foreach (Leder l in _ledere)
            {
                if (l.Id == id)
                {
                    return l;
                }
            }
            return null;
        }

        public void Slet(int id)
        {
            foreach (Leder l in _ledere)
            {
                if (l.Id == id)
                {
                    _ledere.Remove(l);
                    return; // stop løkken, ellers giver Remove en fejl
                }
            }
        }

        public void Opdater(Leder leder)
        {
            foreach (Leder l in _ledere)
            {
                if (l.Id == leder.Id)
                {
                    l.Navn = leder.Navn;
                    l.BrugerNavn = leder.BrugerNavn;
                    l.Adgangskode = leder.Adgangskode;
                    return;
                }
            }
        }
    }
}
