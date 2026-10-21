using System;
using System.Collections.Generic;
using System.Text;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class TegnRepository
    {
        // Listen med alle tegn
        private List<Tegn> _tegn = new List<Tegn>();

        // Næste Id der bliver givet ud
        private int _næsteId = 1;

        public List<Tegn> HentAlle()
        {
            return _tegn;
        }

        // Giver tegnet et Id og gemmer det
        public void Opret(Tegn tegn)
        {
            tegn.Id = _næsteId;
            _næsteId++;
            _tegn.Add(tegn);
        }

        // Finder ét tegn ud fra Id (null hvis det ikke findes)
        public Tegn? HentEn(int id)
        {
            foreach (Tegn t in _tegn)
            {
                if (t.Id == id)
                {
                    return t;
                }
            }
            return null;
        }

        public void Slet(int id)
        {
            foreach (Tegn t in _tegn)
            {
                if (t.Id == id)
                {
                    _tegn.Remove(t);
                    return; // stop løkken, ellers giver Remove en fejl
                }
            }
        }

        public void Opdater(Tegn tegn)
        {
            foreach (Tegn t in _tegn)
            {
                if (t.Id == tegn.Id)
                {
                    t.Navn = tegn.Navn;
                    return;
                }
            }
        }
    }
}
