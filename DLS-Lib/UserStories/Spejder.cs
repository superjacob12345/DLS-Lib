using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Spejder
    {
        private string _navn;
        private int _alder;
        private string _brugerNavn;
        private string _adgangskode;
        private DateOnly _foeselsdag;
    

    public Spejder(string navn, int alder, string brugerNavn, string adgangskode, DateOnly foeselsdag)
        {
            _navn = navn;
            _alder = alder;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
            _foeselsdag = foeselsdag;
            navn = "";
            alder = 0;
            brugerNavn = "";
            adgangskode = "";
            foeselsdag = DateOnly.MinValue;
        }

        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }
        public int Alder
        {
            get { return _alder; }
            set { _alder = value; }
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
        public Spejder opretSpejder(string navn, int alder, string brugerNavn, string adgangskode, DateOnly foeselsdag)
        {
            return new Spejder(navn, alder, brugerNavn, adgangskode, foeselsdag);
        }
        public Spejder hentSpejder(string brugerNavn, string adgangskode)
        {
            // This method would typically retrieve a Spejder from a database or collection
            // For demonstration purposes, we will return a new Spejder with the provided credentials
            return new Spejder("Hentet Navn", 0, brugerNavn, adgangskode, DateOnly.MinValue);
        }
        public Spejder opdaterSpejder(string navn, int alder, string brugerNavn, string adgangskode, DateOnly foeselsdag)
        {
            _navn = navn;
            _alder = alder;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
            _foeselsdag = foeselsdag;
            return this;
        }
        public void sletSpejder()
        {
            _navn = null;
            _alder = 0;
            _brugerNavn = null;
            _adgangskode = null;
            _foeselsdag = DateOnly.MinValue;
        }
        public override string ToString()
        {
            return $"Navn: {_navn}, Alder: {_alder}, BrugerNavn: {_brugerNavn}, Adgangskode: {_adgangskode}, Fødselsdag: {_foeselsdag}";
        }
    }
}
