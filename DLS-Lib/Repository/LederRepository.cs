using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class LederRepository
    {
        private DLSopretForbindelse db = new DLSopretForbindelse();

        // CREATE
        public void Opret(Leder leder)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"INSERT INTO Leder (Navn, BrugerNavn, Adgangskode)
                  VALUES (@navn, @brugerNavn, @adgangskode)",
                db.forbind);

            command.Parameters.AddWithValue("@navn", leder.Navn);
            command.Parameters.AddWithValue("@brugerNavn", leder.BrugerNavn);
            command.Parameters.AddWithValue("@adgangskode", leder.Adgangskode);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // READ - hent alle ledere
        public List<Leder> HentAlle()
        {
            List<Leder> ledere = new List<Leder>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT * FROM Leder",
                db.forbind);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Leder leder = new Leder
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    BrugerNavn = reader.GetString(2),
                    Adgangskode = reader.GetString(3)
                };

                ledere.Add(leder);
            }

            reader.Close();
            db.LukForbindelse();

            return ledere;
        }

        // READ - hent én leder
        public Leder? HentEn(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT * FROM Leder WHERE Id = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            var reader = command.ExecuteReader();

            Leder? leder = null;

            if (reader.Read())
            {
                leder = new Leder
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    BrugerNavn = reader.GetString(2),
                    Adgangskode = reader.GetString(3)
                };
            }

            reader.Close();
            db.LukForbindelse();

            return leder;
        }

        // UPDATE
        public void Opdater(Leder leder)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"UPDATE Leder
                  SET Navn = @navn,
                      BrugerNavn = @brugerNavn,
                      Adgangskode = @adgangskode
                  WHERE Id = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", leder.Id);
            command.Parameters.AddWithValue("@navn", leder.Navn);
            command.Parameters.AddWithValue("@brugerNavn", leder.BrugerNavn);
            command.Parameters.AddWithValue("@adgangskode", leder.Adgangskode);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // DELETE
        public void Slet(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "DELETE FROM Leder WHERE Id = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }
    }
}
