using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class SpejderRepository
    {
        private DLSopretForbindelse db = new DLSopretForbindelse();

        /// <summary>
        /// Gemmer en ny spejder i databasen sammen med spejderens tegn.
        /// </summary>
        /// <param name="spejder">Den spejder der skal gemmes. Dens Id sættes til det id databasen tildeler.</param>
        public void Opret(Spejder spejder)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"INSERT INTO Spejder
                  (Navn, BrugerNavn, Adgangskode, Foeselsdag)
                  OUTPUT INSERTED.SpejderId
                  VALUES
                  (@navn, @brugerNavn, @adgangskode, @foeselsdag)",
                db.forbind);

            command.Parameters.AddWithValue("@navn", spejder.Navn);
            command.Parameters.AddWithValue("@brugerNavn", spejder.BrugerNavn);
            command.Parameters.AddWithValue("@adgangskode", spejder.Adgangskode);
            command.Parameters.AddWithValue("@foeselsdag", spejder.Foeselsdag);

            // Databasen giver spejderen et Id
            spejder.Id = (int)command.ExecuteScalar();

            GemTegn(spejder);
            db.LukForbindelse();
        }

        /// <summary>
        /// Henter alle spejdere fra databasen med deres tegn.
        /// </summary>
        /// <returns>En liste med alle spejdere. Listen er tom hvis der ingen spejdere er.</returns>
        public List<Spejder> HentAlle()
        {
            List<Spejder> spejdere = new List<Spejder>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT SpejderId, Navn, BrugerNavn, Adgangskode, Foeselsdag FROM Spejder",
                db.forbind);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                spejdere.Add(LavSpejder(reader));
            }

            reader.Close();

            foreach (Spejder spejder in spejdere)
            {
                HentTegn(spejder);
            }

            db.LukForbindelse();

            return spejdere;
        }

        /// <summary>
        /// Henter én spejder fra databasen med spejderens tegn.
        /// </summary>
        /// <param name="id">Id på den spejder der skal hentes.</param>
        /// <returns>Spejderen, eller null hvis spejderen ikke findes.</returns>
        public Spejder? HentEn(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"SELECT SpejderId, Navn, BrugerNavn, Adgangskode, Foeselsdag
                  FROM Spejder
                  WHERE SpejderId = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            var reader = command.ExecuteReader();

            Spejder? spejder = null;

            if (reader.Read())
            {
                spejder = LavSpejder(reader);
            }

            reader.Close();

            if (spejder != null)
            {
                HentTegn(spejder);
            }

            db.LukForbindelse();

            return spejder;
        }

        /// <summary>
        /// Opdaterer en spejder i databasen. Spejderens tegn slettes og gemmes igen ud fra objektet.
        /// </summary>
        /// <param name="spejder">Spejderen med de nye oplysninger. Dens Id afgør hvilken række der opdateres.</param>
        public void Opdater(Spejder spejder)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"UPDATE Spejder
                  SET Navn = @navn,
                      BrugerNavn = @brugerNavn,
                      Adgangskode = @adgangskode,
                      Foeselsdag = @foeselsdag
                  WHERE SpejderId = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", spejder.Id);
            command.Parameters.AddWithValue("@navn", spejder.Navn);
            command.Parameters.AddWithValue("@brugerNavn", spejder.BrugerNavn);
            command.Parameters.AddWithValue("@adgangskode", spejder.Adgangskode);
            command.Parameters.AddWithValue("@foeselsdag", spejder.Foeselsdag);

            command.ExecuteNonQuery();

            // Slet de gamle tegn og gem de nye
            var sletCommand = new SqlCommand(
                "DELETE FROM SpejderTegn WHERE SpejderId = @id",
                db.forbind);
            sletCommand.Parameters.AddWithValue("@id", spejder.Id);
            sletCommand.ExecuteNonQuery();

            GemTegn(spejder);
            db.LukForbindelse();
        }

        /// <summary>
        /// Sletter en spejder fra databasen. Spejderens tegn og tilmeldinger slettes automatisk med ON DELETE CASCADE.
        /// </summary>
        /// <param name="id">Id på den spejder der skal slettes.</param>
        public void Slet(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "DELETE FROM Spejder WHERE SpejderId = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // Laver en spejder ud fra den række readeren står på
        private Spejder LavSpejder(SqlDataReader reader)
        {
            Spejder spejder = new Spejder();
            spejder.Id = reader.GetInt32(0);
            spejder.Navn = reader.GetString(1);
            spejder.BrugerNavn = reader.GetString(2);
            spejder.Adgangskode = reader.GetString(3);
            spejder.Foeselsdag = DateOnly.FromDateTime(reader.GetDateTime(4));
            return spejder;
        }

        // Henter spejderens tegn. Forbindelsen skal være åben.
        private void HentTegn(Spejder spejder)
        {
            var command = new SqlCommand(
                "SELECT TegnId FROM SpejderTegn WHERE SpejderId = @id",
                db.forbind);
            command.Parameters.AddWithValue("@id", spejder.Id);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                spejder.TegnIds.Add(reader.GetInt32(0));
            }
            reader.Close();
        }

        // Gemmer spejderens tegn. Forbindelsen skal være åben.
        private void GemTegn(Spejder spejder)
        {
            foreach (int tegnId in spejder.TegnIds)
            {
                var command = new SqlCommand(
                    "INSERT INTO SpejderTegn (SpejderId, TegnId) VALUES (@spejderId, @tegnId)",
                    db.forbind);
                command.Parameters.AddWithValue("@spejderId", spejder.Id);
                command.Parameters.AddWithValue("@tegnId", tegnId);
                command.ExecuteNonQuery();
            }
        }
    }
}
