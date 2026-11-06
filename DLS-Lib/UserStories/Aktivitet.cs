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

        /// <summary>
        /// Opretter en tom aktivitet. Bruges fx når en formular eller en databaserække skal læses ind i objektet.
        /// </summary>
        public Aktivitet() { }

        /// <summary>
        /// Opretter en aktivitet med de angivne oplysninger. Id, ledere og krævede tegn sættes bagefter.
        /// </summary>
        /// <param name="navn">Aktivitetens navn.</param>
        /// <param name="tidspunkt">Dato og klokkeslæt for hvornår aktiviteten begynder.</param>
        /// <param name="maksDeltagere">Det største antal spejdere der kan tilmeldes.</param>
        /// <param name="minAlder">Den alder en spejder mindst skal have for at deltage.</param>
        /// <param name="sted">Stedet hvor aktiviteten foregår.</param>
        /// <param name="beskrivelse">En beskrivelse af aktiviteten.</param>
        public Aktivitet(string navn, DateTime tidspunkt, int maksDeltagere, int minAlder, string sted,string beskrivelse )
        {
            _navn = navn;
            _tidspunkt = tidspunkt;
            _maksDeltagere = maksDeltagere;
            _minAlder = minAlder;
            _sted = sted;
            _beskrivelse = beskrivelse;
        }

        /// <summary>
        /// Aktivitetens id, som databasen tildeler. Er 0 indtil aktiviteten er gemt.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Aktivitetens navn. Skal udfyldes og må højst være 100 tegn.
        /// </summary>
        [Required(ErrorMessage = "Aktiviteten skal have et navn")]
        [StringLength(100, ErrorMessage = "Navnet må højst være 100 tegn")]
        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        /// <summary>
        /// Dato og klokkeslæt for hvornår aktiviteten begynder.
        /// </summary>
        public DateTime Tidspunkt
        {
            get { return _tidspunkt; }
            set { _tidspunkt = value; }
        }

        /// <summary>
        /// Det største antal spejdere der kan tilmeldes. Skal være mellem 1 og 500.
        /// </summary>
        [Display(Name = "Maks deltagere")]
        [Range(1, 500, ErrorMessage = "Maks deltagere skal være mellem 1 og 500")]
        public int MaksDeltagere
        {
            get { return _maksDeltagere; }
            set { _maksDeltagere = value; }
        }

        /// <summary>
        /// Den alder en spejder mindst skal have for at deltage. Skal være mellem 0 og 99 år.
        /// </summary>
        [Display(Name = "Min. alder")]
        [Range(0, 99, ErrorMessage = "Min. alder skal være mellem 0 og 99 år")]
        public int MinAlder
        {
            get { return _minAlder; }
            set { _minAlder = value; }
        }

        /// <summary>
        /// Stedet hvor aktiviteten foregår. Må højst være 100 tegn.
        /// </summary>
        [StringLength(100, ErrorMessage = "Stedet må højst være 100 tegn")]
        public string Sted
        {
            get { return _sted; }
            set { _sted = value; }
        }

        /// <summary>
        /// En beskrivelse af aktiviteten. Må højst være 1000 tegn.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Beskrivelsen må højst være 1000 tegn")]
        public string Beskrivelse
        {
            get { return _beskrivelse; }
            set { _beskrivelse = value; }
        }

        /// <summary>
        /// Id'erne på de ledere der varetager aktiviteten.
        /// </summary>
        public List<int> LederIds
        {
            get { return _lederIds; }
            set { _lederIds = value; }
        }

        /// <summary>
        /// Id'erne på de tegn en spejder skal have for at måtte deltage.
        /// </summary>
        public List<int> KraevedeTegnIds
        {
            get { return _kraevedeTegnIds; }
            set { _kraevedeTegnIds = value; }
        }
        /// <summary>
        /// Laver en tekst med aktivitetens oplysninger. Id, ledere og krævede tegn er ikke med.
        /// </summary>
        /// <returns>Aktivitetens navn, tidspunkt, maks. deltagere, min. alder, sted og beskrivelse.</returns>
        public override string ToString()
        {
            return $"Aktivitet: {Navn}, Tidspunkt: {Tidspunkt}, MaksDeltagere: {MaksDeltagere}, MinAlder: {MinAlder}, Sted: {Sted}, Beskrivelse: {Beskrivelse}";

        }
        
    }
}
