using System;
using System.Collections.Generic;
using System.Text;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class AktivitetRepository
    {
        private List<Aktivitet> _aktiviteter = new List<Aktivitet>();

        public List<Aktivitet> HentAlle()
        {
            return _aktiviteter;
        }

        public void Opret(Aktivitet aktivitet)
        {
            _aktiviteter.Add(aktivitet);
        }

        //  HentEn(int id)

        // Slet(int id)

        // Opdater(Aktivitet aktivitet)
    }
}
