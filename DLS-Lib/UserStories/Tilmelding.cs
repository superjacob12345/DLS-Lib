using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Tilmelding
    {
        private int _spejderId;
        private int _aktivitetId;
        private DateOnly _dato;

        /// <summary>
        /// Opretter en tom tilmelding.
        /// </summary>
        public Tilmelding() { }

        /// <summary>
        /// Opretter en tilmelding med de angivne oplysninger.
        /// </summary>
        /// <param name="spejderId">Id på den spejder der er tilmeldt.</param>
        /// <param name="aktivitetId">Id på den aktivitet spejderen er tilmeldt.</param>
        /// <param name="dato">Den dato tilmeldingen er foretaget.</param>
        public Tilmelding(int spejderId, int aktivitetId, DateOnly dato)
        {
            _spejderId = spejderId;
            _aktivitetId = aktivitetId;
            _dato = dato;
        }

        /// <summary>
        /// Id på den spejder der er tilmeldt.
        /// </summary>
        public int SpejderId
        {
            get { return _spejderId; }
            set { _spejderId = value; }
        }
        /// <summary>
        /// Id på den aktivitet spejderen er tilmeldt.
        /// </summary>
        public int AktivitetId
        {
            get { return _aktivitetId; }
            set { _aktivitetId = value; }
        }
        /// <summary>
        /// Den dato tilmeldingen er foretaget.
        /// </summary>
        public DateOnly Dato
        {
            get { return _dato; }
            set { _dato = value; }
        }

        /// <summary>
        /// Laver en tekst med tilmeldingens oplysninger.
        /// </summary>
        /// <returns>Spejderens id, aktivitetens id og tilmeldingsdatoen.</returns>
        public override string ToString()
        {
            return $"SpejderId: {_spejderId}, AktivitetId: {_aktivitetId}, Dato: {_dato}";
        }
    }
}
