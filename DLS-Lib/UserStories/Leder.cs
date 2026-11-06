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

        /// <summary>
        /// Opretter en tom leder. Bruges fx når en formular eller en databaserække skal læses ind i objektet.
        /// </summary>
        public Leder() { }

        /// <summary>
        /// Opretter en leder med de angivne oplysninger. Id sættes af databasen når lederen gemmes.
        /// </summary>
        /// <param name="navn">Lederens navn.</param>
        /// <param name="brugerNavn">Det brugernavn lederen logger ind med.</param>
        /// <param name="adgangskode">Den adgangskode lederen logger ind med.</param>
        public Leder(string navn, string brugerNavn, string adgangskode)
        {
            _navn = navn;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
        }

        /// <summary>
        /// Lederens id, som databasen tildeler. Er 0 indtil lederen er gemt.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Lederens navn.
        /// </summary>
        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        /// <summary>
        /// Det brugernavn lederen logger ind med.
        /// </summary>
        public string BrugerNavn
        {
            get { return _brugerNavn; }
            set { _brugerNavn = value; }
        }

        /// <summary>
        /// Den adgangskode lederen logger ind med.
        /// </summary>
        public string Adgangskode
        {
            get { return _adgangskode; }
            set { _adgangskode = value; }
        }

        /// <summary>
        /// Laver en tekst med lederens oplysninger. Adgangskoden er ikke med.
        /// </summary>
        /// <returns>Lederens id, navn og brugernavn.</returns>
        public override string ToString()
        {
            return $"Id: {_id}, Navn: {_navn}, BrugerNavn: {_brugerNavn}";
        }
    }
}
