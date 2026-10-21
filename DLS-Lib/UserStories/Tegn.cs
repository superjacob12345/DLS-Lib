using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Tegn
    {
        private string _navn;

        public Tegn(string navn)
        {
            _navn = navn;

            navn = "";
        }


        public string Navn
        {
            get { return _navn; }
        }

        // string to string metode 
        public override string ToString()
        {
            return _navn;
        }


        public Tegn opretTegn(string navn)
        {
            return new Tegn(navn);
        }

        public Tegn hentTegn(string navn)
        {
            return new Tegn(navn);
        }

        public void opderTegn(string navn)
        {
            _navn = navn;
        }
        public void sletTegn()
        {
            _navn = null;
        }
    }
}
