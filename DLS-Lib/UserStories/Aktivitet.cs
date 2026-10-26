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
        private string _sted = "";
        private string _beskrivelse = "";
        private List<int> _lederIds = new List<int>();
        private List<int> _kraevedeTegnIds = new List<int>();

        public Aktivitet() { }

        public Aktivitet(string navn, DateTime tidspunkt, int maksDeltagere, int minAlder, string sted,string beskrivelse )
        {
            _navn = navn;
            _tidspunkt = tidspunkt;
            _maksDeltagere = maksDeltagere;
            _minAlder = minAlder;
            _sted = sted;
            _beskrivelse = beskrivelse;
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

        public string Sted
        {
            get { return _sted; }
            set { _sted = value; }
        }

        public string Beskrivelse
        {
            get { return _beskrivelse; }
            set { _beskrivelse = value; }
        }

        // De ledere der varetager aktiviteten
        public List<int> LederIds
        {
            get { return _lederIds; }
            set { _lederIds = value; }
        }

        // De tegn en spejder skal have for at måtte deltage
        public List<int> KraevedeTegnIds
        {
            get { return _kraevedeTegnIds; }
            set { _kraevedeTegnIds = value; }
        }
        public override string ToString()
        {
            return $"Aktivitet: {Navn}, Tidspunkt: {Tidspunkt}, MaksDeltagere: {MaksDeltagere}, MinAlder: {MinAlder}, Sted: {Sted}, Beskrivelse: {Beskrivelse}";

        }
        
    }
}
