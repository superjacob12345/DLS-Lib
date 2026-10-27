using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Spejder
    {
        private int _id;
        private string _navn = "";
        private string _brugerNavn = "";
        private string _adgangskode = "";
        private DateOnly _foeselsdag;
        private List<int> _tegnIds = new List<int>();

        public Spejder() { }

        public Spejder(string navn, string brugerNavn, string adgangskode, DateOnly foeselsdag)
        {
            _navn = navn;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
            _foeselsdag = foeselsdag;
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
        public DateOnly Foeselsdag
        {
            get { return _foeselsdag; }
            set { _foeselsdag = value; }
        }

        // De tegn (færdigheder) spejderen har taget
        public List<int> TegnIds
        {
            get { return _tegnIds; }
            set { _tegnIds = value; }
        }

        // Alder regnes ud fra fødselsdatoen, så den altid passer
        public int Alder
        {
            get
            {
                DateOnly idag = DateOnly.FromDateTime(DateTime.Today);
                int alder = idag.Year - _foeselsdag.Year;
                if (_foeselsdag > idag.AddYears(-alder))
                {
                    alder--;
                }
                return alder;
            }
        }

        public override string ToString()
        {
            return $"Id: {_id}, Navn: {_navn}, Alder: {Alder}, BrugerNavn: {_brugerNavn}, Fødselsdato: {_foeselsdag}";
        }
    }
}
