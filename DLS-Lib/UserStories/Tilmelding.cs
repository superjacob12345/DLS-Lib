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


        public Tilmelding opretTilmelding(int spejderId, int aktivitetId, DateOnly dato)
        {
            return new Tilmelding(spejderId, aktivitetId, dato);
        }

        public Tilmelding hentTilmelding(int spejderId, int aktivitetId)
        {
            // This method would typically retrieve a Tilmelding from a database or collection
            // For demonstration purposes, we will return a new Tilmelding with the provided IDs and current date
            return new Tilmelding(spejderId, aktivitetId, DateOnly.FromDateTime(DateTime.Now));
        }
        public Tilmelding opdaterTilmelding(int spejderId, int aktivitetId, DateOnly nyDato)
        {
            // This method would typically update a Tilmelding in a database or collection
            // For demonstration purposes, we will return a new Tilmelding with the updated date
            return new Tilmelding(spejderId, aktivitetId, nyDato);
        }

        public void sletTilmelding(int spejderId, int aktivitetId)
        {
            // This method would typically delete a Tilmelding from a database or collection
            // For demonstration purposes, we will just print a message
            Console.WriteLine($"Tilmelding for SpejderId: {spejderId} and AktivitetId: {aktivitetId} has been deleted.");
        }

        public override string ToString()
        {
            return $"SpejderId: {_spejderId}, AktivitetId: {_aktivitetId}, Dato: {_dato}";
        }
    }
}
