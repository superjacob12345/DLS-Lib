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

        public Spejder() { }

        public Spejder(string navn, string brugerNavn, string adgangskode, DateOnly foeselsdag)
        {
            _navn = navn;
            _brugerNavn = brugerNavn;
            _adgangskode = adgangskode;
            _foeselsdag = foeselsdag;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        [Required(ErrorMessage = "Skriv spejderens navn")]
        [StringLength(100, ErrorMessage = "Navnet må højst være 100 tegn")]
        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }
        [Required(ErrorMessage = "Skriv et brugernavn")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Brugernavnet skal være mellem 3 og 50 tegn")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Brugernavnet må ikke indeholde mellemrum")]
        public string BrugerNavn
        {
            get { return _brugerNavn; }
            set { _brugerNavn = value; }
        }
        // Ikke [Required], fordi feltet må stå tomt når man retter (så beholdes den gamle).
        // Ved oprettelse tjekkes det på siden.
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Adgangskoden skal være mellem 4 og 100 tegn")]
        public string Adgangskode
        {
            get { return _adgangskode; }
            set { _adgangskode = value; }
        }
        [Display(Name = "Fødselsdato")]
        [Foedselsdato]
        public DateOnly Foeselsdag
        {
            get { return _foeselsdag; }
            set { _foeselsdag = value; }
        }

        // De tegn (færdigheder) spejderen har taget
        public List<int> TegnIds
        {
            get { return _tegnIds; }
            set { _tegnIds = value; }
        }

        // Alder regnes ud fra fødselsdatoen, så den altid passer
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

        public override string ToString()
        {
            return $"Id: {_id}, Navn: {_navn}, Alder: {Alder}, BrugerNavn: {_brugerNavn}, Fødselsdato: {_foeselsdag}";
        }
    }
}
