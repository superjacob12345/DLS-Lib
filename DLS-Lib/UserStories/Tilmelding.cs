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

        public Tilmelding() { }

        public Tilmelding(int spejderId, int aktivitetId, DateOnly dato)
        {
            _spejderId = spejderId;
            _aktivitetId = aktivitetId;
            _dato = dato;
        }

        public int SpejderId
        {
            get { return _spejderId; }
            set { _spejderId = value; }
        }
        public int AktivitetId
        {
            get { return _aktivitetId; }
            set { _aktivitetId = value; }
        }
        public DateOnly Dato
        {
            get { return _dato; }
            set { _dato = value; }
        }

        public override string ToString()
        {
            return $"SpejderId: {_spejderId}, AktivitetId: {_aktivitetId}, Dato: {_dato}";
        }
    }
}
