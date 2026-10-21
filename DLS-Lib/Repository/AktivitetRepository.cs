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
            Aktivitet? aktivitet = HentEn(id);
            if (aktivitet != null)
            {
                _aktiviteter.Remove(aktivitet);
            }
        }

        public void Opdater(Aktivitet aktivitet)
        {
            Aktivitet? gammel = HentEn(aktivitet.Id);
            if (gammel != null)
            {
                gammel.Navn = aktivitet.Navn;
                gammel.Tidspunkt = aktivitet.Tidspunkt;
                gammel.MaksDeltagere = aktivitet.MaksDeltagere;
                gammel.MinAlder = aktivitet.MinAlder;
            }
        }
    }
}
