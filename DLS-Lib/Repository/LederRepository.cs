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

        /// <summary>
        /// Gemmer en ny leder i databasen.
        /// </summary>
        /// <param name="leder">Den leder der skal gemmes. Dens Id sættes til det id databasen tildeler.</param>
        public void Opret(Leder leder)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"INSERT INTO Leder (Navn, BrugerNavn, Adgangskode)
                  OUTPUT INSERTED.Id
                  VALUES (@navn, @brugerNavn, @adgangskode)",
                db.forbind);

            command.Parameters.AddWithValue("@navn", leder.Navn);
            command.Parameters.AddWithValue("@brugerNavn", leder.BrugerNavn);
            command.Parameters.AddWithValue("@adgangskode", leder.Adgangskode);

            // Databasen giver lederen et Id
            leder.Id = (int)command.ExecuteScalar();
            db.LukForbindelse();
        }

        /// <summary>
        /// Henter alle ledere fra databasen.
        /// </summary>
        /// <returns>En liste med alle ledere. Listen er tom hvis der ingen ledere er.</returns>
        public List<Leder> HentAlle()
        {
            List<Leder> ledere = new List<Leder>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT Id, Navn, BrugerNavn, Adgangskode FROM Leder",
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

        /// <summary>
        /// Henter én leder fra databasen.
        /// </summary>
        /// <param name="id">Id på den leder der skal hentes.</param>
        /// <returns>Lederen, eller null hvis lederen ikke findes.</returns>
        public Leder? HentEn(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT Id, Navn, BrugerNavn, Adgangskode FROM Leder WHERE Id = @id",
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

        /// <summary>
        /// Opdaterer en leder i databasen.
        /// </summary>
        /// <param name="leder">Lederen med de nye oplysninger. Dens Id afgør hvilken række der opdateres.</param>
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

        /// <summary>
        /// Sletter en leder fra databasen. Lederens kobling til aktiviteter slettes automatisk med ON DELETE CASCADE.
        /// </summary>
        /// <param name="id">Id på den leder der skal slettes.</param>
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
