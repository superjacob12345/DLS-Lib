using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Aktiviteten skal have et navn")]
        [StringLength(100, ErrorMessage = "Navnet må højst være 100 tegn")]
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

        [Display(Name = "Maks deltagere")]
        [Range(1, 500, ErrorMessage = "Maks deltagere skal være mellem 1 og 500")]
        public int MaksDeltagere
        {
            get { return _maksDeltagere; }
            set { _maksDeltagere = value; }
        }

        [Display(Name = "Min. alder")]
        [Range(0, 99, ErrorMessage = "Min. alder skal være mellem 0 og 99 år")]
        public int MinAlder
        {
            get { return _minAlder; }
            set { _minAlder = value; }
        }

        [StringLength(100, ErrorMessage = "Stedet må højst være 100 tegn")]
        public string Sted
        {
            get { return _sted; }
            set { _sted = value; }
        }

        [StringLength(1000, ErrorMessage = "Beskrivelsen må højst være 1000 tegn")]
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
