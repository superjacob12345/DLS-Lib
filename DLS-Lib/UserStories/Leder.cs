using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Leder
    {
        // instance variables
        private string _navn;
        private string _brugerNavn;
        private string _adgangskode;


        public Leder(string navn, string brugerNavn, string adgangskode)
        {
            _navn = navn;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;

            navn="";
            brugerNavn = "";
            adgangskode = "";
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


        // CRUD methods for Leder
        public Leder opretLeder(string navn, string brugerNavn, string adgangskode)
        {
            return new Leder(navn, brugerNavn, adgangskode);
        }

        public Leder hentLeder(string brugerNavn, string adgangskode)
        {
            // This method would typically retrieve a Leder from a database or collection
            // For demonstration purposes, we will return a new Leder with the provided credentials
            return new Leder("Hentet Navn", brugerNavn, adgangskode);
        }

        public Leder opdaterLeder(string navn, string brugerNavn, string adgangskode)
        {
            _navn = navn;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
            return this;
        }

        public void sletLeder()
        {
            // This method would typically remove a Leder from a database or collection
            // For demonstration purposes, we will just clear the instance variables
            _navn = null;
            _brugerNavn = null;
            _adgangskode = null;
        }


        public override string ToString()
        {
            return $"Navn: {_navn}, BrugerNavn: {_brugerNavn}, Adgangskode: {_adgangskode}";
        }
    }
}
