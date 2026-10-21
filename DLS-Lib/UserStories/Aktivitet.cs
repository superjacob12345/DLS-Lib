using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Aktivitet
    {
        private int _id;
        private string _navn = "";
        private DateTime _tidspunkt;
        private int _maksDeltagere;
        private int _minAlder;

        public Aktivitet() { }

        public Aktivitet(string navn, DateTime tidspunkt, int maksDeltagere, int minAlder)
        {
            _navn = navn;
            _tidspunkt = tidspunkt;
            _maksDeltagere = maksDeltagere;
            _minAlder = minAlder;
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

        public override string ToString()
        {
            return $"id: {_id}   Aktivitet: {_navn}, Tidspunkt: {_tidspunkt}, Maks Deltagere: {_maksDeltagere}, Min Alder: {_minAlder}";
        }
    }
}
