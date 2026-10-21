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
        private DateOnly _foedselsdato;

        public Spejder() { }

        public Spejder(string navn, string brugerNavn, string adgangskode, DateOnly foedselsdato)
        {
            _navn = navn;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
            _foedselsdato = foedselsdato;
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
        public DateOnly Foedselsdato
        {
            get { return _foedselsdato; }
            set { _foedselsdato = value; }
        }

        // Alder regnes ud fra fødselsdatoen, så den altid passer
        public int Alder
        {
            get
            {
                DateOnly idag = DateOnly.FromDateTime(DateTime.Today);
                int alder = idag.Year - _foedselsdato.Year;
                if (_foedselsdato > idag.AddYears(-alder))
                {
                    alder--;
                }
                return alder;
            }
        }

        public override string ToString()
        {
            return $"Id: {_id}, Navn: {_navn}, Alder: {Alder}, BrugerNavn: {_brugerNavn}, Fødselsdato: {_foedselsdato}";
        }
    }
}
