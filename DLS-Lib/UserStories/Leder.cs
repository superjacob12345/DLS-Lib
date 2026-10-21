using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Leder
    {
        // instance variables
        private int _id;
        private string _navn = "";
        private string _brugerNavn = "";
        private string _adgangskode = "";

        public Leder() { }

        public Leder(string navn, string brugerNavn, string adgangskode)
        {
            _navn = navn;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        public string BrugerNavn
        {
            get { return _brugerNavn; }
            set { _brugerNavn = value; }
        }

        public string Adgangskode
        {
            get { return _adgangskode; }
            set { _adgangskode = value; }
        }

        public override string ToString()
        {
            return $"Id: {_id}, Navn: {_navn}, BrugerNavn: {_brugerNavn}";
        }
    }
}
