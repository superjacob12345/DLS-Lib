using System;
using System.Collections.Generic;
using System.Text;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class AktivitetRepository
    {
        private List<Aktivitet> _aktiviteter = new List<Aktivitet>();
        private int _næsteId = 1;

        public List<Aktivitet> HentAlle()
        {
            return _aktiviteter;
        }

        public void Opret(Aktivitet aktivitet)
        {
            aktivitet.Id = _næsteId;
            _næsteId++;
            _aktiviteter.Add(aktivitet);
        }

        public Aktivitet? HentEn(int id)
        {
            foreach (Aktivitet a in _aktiviteter)
            {
                if (a.Id == id)
                {
                    return a;
                }
            }
            return null;
        }

        public void Slet(int id)
        {
            foreach (Aktivitet a in _aktiviteter)
            {
                if (a.Id == id)
                {
                    _aktiviteter.Remove(a);
                    return; // stop løkken, ellers giver Remove en fejl
                }
            }
        }

        public void Opdater(Aktivitet aktivitet)
        {
            foreach (Aktivitet a in _aktiviteter)
            {
                if (a.Id == aktivitet.Id)
                {
                    a.Navn = aktivitet.Navn;
                    a.Tidspunkt = aktivitet.Tidspunkt;
                    a.MaksDeltagere = aktivitet.MaksDeltagere;
                    a.MinAlder = aktivitet.MinAlder;
                    return;
                }
            }
        }
    }
}
