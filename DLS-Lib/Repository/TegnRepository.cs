using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class TegnRepository
    {
        private DLSopretForbindelse db = new DLSopretForbindelse();

        /// <summary>
        /// Gemmer et nyt tegn i databasen.
        /// </summary>
        /// <param name="tegn">Det tegn der skal gemmes. Dets Id sættes til det id databasen tildeler.</param>
        public void Opret(Tegn tegn)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"INSERT INTO Tegn (Navn)
                  OUTPUT INSERTED.Id
                  VALUES (@navn)",
                db.forbind);

            command.Parameters.AddWithValue("@navn", tegn.Navn);

            // Databasen giver tegnet et Id
            tegn.Id = (int)command.ExecuteScalar();
            db.LukForbindelse();
        }

        /// <summary>
        /// Henter alle tegn fra databasen.
        /// </summary>
        /// <returns>En liste med alle tegn. Listen er tom hvis der ingen tegn er.</returns>
        public List<Tegn> HentAlle()
        {
            List<Tegn> tegnListe = new List<Tegn>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT Id, Navn FROM Tegn",
                db.forbind);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Tegn tegn = new Tegn
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1)
                };

                tegnListe.Add(tegn);
            }

            reader.Close();
            db.LukForbindelse();

            return tegnListe;
        }

        /// <summary>
        /// Henter ét tegn fra databasen.
        /// </summary>
        /// <param name="id">Id på det tegn der skal hentes.</param>
        /// <returns>Tegnet, eller null hvis det ikke findes.</returns>
        public Tegn? HentEn(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT Id, Navn FROM Tegn WHERE Id = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            var reader = command.ExecuteReader();

            Tegn? tegn = null;

            if (reader.Read())
            {
                tegn = new Tegn
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1)
                };
            }

            reader.Close();
            db.LukForbindelse();

            return tegn;
        }

        /// <summary>
        /// Opdaterer et tegn i databasen.
        /// </summary>
        /// <param name="tegn">Tegnet med de nye oplysninger. Dets Id afgør hvilken række der opdateres.</param>
        public void Opdater(Tegn tegn)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"UPDATE Tegn
                  SET Navn = @navn
                  WHERE Id = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", tegn.Id);
            command.Parameters.AddWithValue("@navn", tegn.Navn);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        /// <summary>
        /// Sletter et tegn fra databasen. Tegnet fjernes automatisk fra spejdere og aktiviteter med ON DELETE CASCADE.
        /// </summary>
        /// <param name="id">Id på det tegn der skal slettes.</param>
        public void Slet(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "DELETE FROM Tegn WHERE Id = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }
    }
}

