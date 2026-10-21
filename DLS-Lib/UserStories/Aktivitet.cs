using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Aktivitet
    {
        private string _navn;
        private DateTime _tidspunkt;
        private int _maksDeltagere;
        private int _minAlder;

        public Aktivitet(string navn, DateTime tidspunkt, int maksDeltagere, int minAlder)
        {
            _navn = navn;
            _tidspunkt = tidspunkt;
            _maksDeltagere = maksDeltagere;
            _minAlder = minAlder;


            navn = "";
            tidspunkt = DateTime.Now;
            maksDeltagere = 0;
            minAlder = 0;
        }

        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        public DateTime Tidspunkt
        {
            get { return _tidspunkt; }
            set { _tidspunkt = value; }
        }

        public int MaksDeltagere
        {
            get { return _maksDeltagere; }
            set { _maksDeltagere = value; }
        }

        public int MinAlder
        {
            get { return _minAlder; }
            set { _minAlder = value; }
        }

        public Aktivitet opretAktivitet(string navn, DateTime tidspunkt, int maksDeltagere, int minAlder)
        {
            return new Aktivitet(navn, tidspunkt, maksDeltagere, minAlder);
        }

        public Aktivitet hentAktivitet(string navn)
        {
            // This method would typically retrieve an Aktivitet from a database or collection
            // For demonstration purposes, we will return a new Aktivitet with the provided name
            return new Aktivitet(navn, DateTime.Now, 10, 18);
        }

        public Aktivitet opdaterAktivitet(string navn, DateTime tidspunkt, int maksDeltagere, int minAlder)
        {
            _navn = navn;
            _tidspunkt = tidspunkt;
            _maksDeltagere = maksDeltagere;
            _minAlder = minAlder;
            return this;
        }
        public void sletAktivitet()
        {
            // This method would typically remove the Aktivitet from a database or collection
            // For demonstration purposes, we will just clear the properties
            _navn = "";
            _tidspunkt = DateTime.MinValue;
            _maksDeltagere = 0;
            _minAlder = 0;
        }

        public override string ToString()
        {
            return $"Aktivitet: {_navn}, Tidspunkt: {_tidspunkt}, Maks Deltagere: {_maksDeltagere}, Min Alder: {_minAlder}";
        }
    }
}
