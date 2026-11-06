using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        /// <summary>
        /// Opretter en tom spejder. Bruges fx når en formular eller en databaserække skal læses ind i objektet.
        /// </summary>
        public Spejder() { }

        /// <summary>
        /// Opretter en spejder med de angivne oplysninger. Id sættes af databasen når spejderen gemmes.
        /// </summary>
        /// <param name="navn">Spejderens navn.</param>
        /// <param name="brugerNavn">Det brugernavn spejderen logger ind med.</param>
        /// <param name="adgangskode">Den adgangskode spejderen logger ind med.</param>
        /// <param name="foeselsdag">Spejderens fødselsdato.</param>
        public Spejder(string navn, string brugerNavn, string adgangskode, DateOnly foeselsdag)
        {
            _navn = navn;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
            _foeselsdag = foeselsdag;
        }

        /// <summary>
        /// Spejderens id, som databasen tildeler. Er 0 indtil spejderen er gemt.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Spejderens navn. Skal udfyldes og må højst være 100 tegn.
        /// </summary>
        [Required(ErrorMessage = "Skriv spejderens navn")]
        [StringLength(100, ErrorMessage = "Navnet må højst være 100 tegn")]
        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }
        /// <summary>
        /// Det brugernavn spejderen logger ind med. Skal være mellem 3 og 50 tegn uden mellemrum.
        /// </summary>
        [Required(ErrorMessage = "Skriv et brugernavn")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Brugernavnet skal være mellem 3 og 50 tegn")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Brugernavnet må ikke indeholde mellemrum")]
        public string BrugerNavn
        {
            get { return _brugerNavn; }
            set { _brugerNavn = value; }
        }
        /// <summary>
        /// Den adgangskode spejderen logger ind med. Skal være mellem 4 og 100 tegn.
        /// </summary>
        /// <remarks>
        /// Er ikke [Required], fordi feltet må stå tomt når man retter en spejder (så beholdes den gamle). Ved oprettelse tjekkes det på siden.
        /// </remarks>
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Adgangskoden skal være mellem 4 og 100 tegn")]
        public string Adgangskode
        {
            get { return _adgangskode; }
            set { _adgangskode = value; }
        }
        /// <summary>
        /// Spejderens fødselsdato. Må ikke ligge i fremtiden eller mere end 100 år tilbage.
        /// </summary>
        [Display(Name = "Fødselsdato")]
        [Foedselsdato]
        public DateOnly Foeselsdag
        {
            get { return _foeselsdag; }
            set { _foeselsdag = value; }
        }

        /// <summary>
        /// Id'erne på de tegn (færdigheder) spejderen har taget.
        /// </summary>
        public List<int> TegnIds
        {
            get { return _tegnIds; }
            set { _tegnIds = value; }
        }

        /// <summary>
        /// Spejderens alder i hele år. Regnes ud fra fødselsdatoen, så den altid passer.
        /// </summary>
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

        /// <summary>
        /// Laver en tekst med spejderens oplysninger. Adgangskode og tegn er ikke med.
        /// </summary>
        /// <returns>Spejderens id, navn, alder, brugernavn og fødselsdato.</returns>
        public override string ToString()
        {
            return $"Id: {_id}, Navn: {_navn}, Alder: {Alder}, BrugerNavn: {_brugerNavn}, Fødselsdato: {_foeselsdag}";
        }
    }
}
