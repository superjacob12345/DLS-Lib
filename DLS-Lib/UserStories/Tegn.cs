using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Tegn
    {
        private int _id;
        private string _navn = "";

        /// <summary>
        /// Opretter et tomt tegn. Bruges fx når en formular eller en databaserække skal læses ind i objektet.
        /// </summary>
        public Tegn() { }

        /// <summary>
        /// Opretter et tegn med det angivne navn. Id sættes af databasen når tegnet gemmes.
        /// </summary>
        /// <param name="navn">Tegnets navn.</param>
        public Tegn(string navn)
        {
            _navn = navn;
        }

        /// <summary>
        /// Tegnets id, som databasen tildeler. Er 0 indtil tegnet er gemt.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Tegnets navn.
        /// </summary>
        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        /// <summary>
        /// Giver tegnet som tekst.
        /// </summary>
        /// <returns>Tegnets navn.</returns>
        public override string ToString()
        {
            return _navn;
        }
    }
}
