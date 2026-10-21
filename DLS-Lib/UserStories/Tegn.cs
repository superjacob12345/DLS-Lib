using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class Tegn
    {
        private int _id;
        private string _navn = "";

        public Tegn() { }

        public Tegn(string navn)
        {
            _navn = navn;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        // string to string metode
        public override string ToString()
        {
            return _navn;
        }
    }
}
