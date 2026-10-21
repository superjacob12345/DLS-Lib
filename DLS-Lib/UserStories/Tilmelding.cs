using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Tilmelding
    {
        private DateOnly _dato;


        public Tilmelding(DateOnly dato)
        {
            _dato = dato;
            dato = DateOnly.MinValue;
        }
        public DateOnly Dato
        {
            get { return _dato; }
            set { _dato = value; }
        }

        public Tilmelding opretTilmelding(DateOnly dato)
        {
            return new Tilmelding(dato);
        }
        public Tilmelding hentTilmelding(DateOnly dato)
        {
            return new Tilmelding(dato);
        }
        public void opdaterTilmelding(DateOnly dato)
        {
            _dato = dato;
        }
        public void sletTilmelding()
        {
            _dato = DateOnly.MinValue;
        }
        public override string ToString()
        {
            return _dato.ToString();
        }
    }
}
