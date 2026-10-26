using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class SpejderRepository
    {
        private DLSopretForbindelse db = new DLSopretForbindelse();

        // CREATE
        public void Opret(Spejder spejder)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"INSERT INTO Spejder
                  (Navn, BrugerNavn, Adgangskode, Foeselsdag)
                  VALUES
                  (@navn, @brugerNavn, @adgangskode, @foeselsdag)",
                db.forbind);

            command.Parameters.AddWithValue("@navn", spejder.Navn);
            command.Parameters.AddWithValue("@brugerNavn", spejder.BrugerNavn);
            command.Parameters.AddWithValue("@adgangskode", spejder.Adgangskode);
            command.Parameters.AddWithValue("@foeselsdag", spejder.Foeselsdag);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // READ - hent alle spejdere
        public List<Spejder> HentAlle()
        {
            List<Spejder> spejdere = new List<Spejder>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT * FROM Spejder",
                db.forbind);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Spejder spejder = new Spejder
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    BrugerNavn = reader.GetString(2),
                    Adgangskode = reader.GetString(3),
                    Foeselsdag = DateOnly.FromDateTime(reader.GetDateTime(4))
                };

                spejdere.Add(spejder);
            }

            reader.Close();
            db.LukForbindelse();

            return spejdere;
        }

        // READ - hent én spejder
        public Spejder? HentEn(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT * FROM Spejder WHERE SpejderId = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            var reader = command.ExecuteReader();

            Spejder? spejder = null;

            if (reader.Read())
            {
                spejder = new Spejder
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    BrugerNavn = reader.GetString(2),
                    Adgangskode = reader.GetString(3),
                    Foeselsdag = DateOnly.FromDateTime(reader.GetDateTime(4))
                };
            }

            reader.Close();
            db.LukForbindelse();

            return spejder;
        }

        // UPDATE
        public void Opdater(Spejder spejder)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"UPDATE Spejder
                  SET Navn = @navn,
                      BrugerNavn = @brugerNavn,
                      Adgangskode = @adgangskode,
                      Foedselsdato = @foedselsdato
                  WHERE Id = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", spejder.Id);
            command.Parameters.AddWithValue("@navn", spejder.Navn);
            command.Parameters.AddWithValue("@brugerNavn", spejder.BrugerNavn);
            command.Parameters.AddWithValue("@adgangskode", spejder.Adgangskode);
            command.Parameters.AddWithValue("@foeselsdag", spejder.Foeselsdag);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // DELETE
        public void Slet(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "DELETE FROM Spejder WHERE Id = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }
    }
}

