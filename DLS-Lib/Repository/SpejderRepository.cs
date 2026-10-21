using System;
using System.Collections.Generic;
using System.Text;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class SpejderRepository
    {
        // Listen med alle spejdere
        private List<Spejder> _spejdere = new List<Spejder>();

        // Næste Id der bliver givet ud
        private int _næsteId = 1;

        public List<Spejder> HentAlle()
        {
            return _spejdere;
        }

        // Giver spejderen et Id og gemmer den
        public void Opret(Spejder spejder)
        {
            spejder.Id = _næsteId;
            _næsteId++;
            _spejdere.Add(spejder);
        }

        // Finder en spejder ud fra Id (null hvis den ikke findes)
        public Spejder? HentEn(int id)
        {
            foreach (Spejder s in _spejdere)
            {
                if (s.Id == id)
                {
                    return s;
                }
            }
            return null;
        }

        public void Slet(int id)
        {
            foreach (Spejder s in _spejdere)
            {
                if (s.Id == id)
                {
                    _spejdere.Remove(s);
                    return; // stop løkken, ellers giver Remove en fejl
                }
            }
        }

        public void Opdater(Spejder spejder)
        {
            foreach (Spejder s in _spejdere)
            {
                if (s.Id == spejder.Id)
                {
                    s.Navn = spejder.Navn;
                    s.BrugerNavn = spejder.BrugerNavn;
                    s.Adgangskode = spejder.Adgangskode;
                    s.Foedselsdato = spejder.Foedselsdato;
                    return;
                }
            }
        }
    }
}
