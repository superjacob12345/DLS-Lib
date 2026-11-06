using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class TilmeldingRepository
    {
        private DLSopretForbindelse db = new DLSopretForbindelse();

        /// <summary>
        /// Henter alle tilmeldinger fra databasen.
        /// </summary>
        /// <returns>En liste med alle tilmeldinger. Listen er tom hvis der ingen tilmeldinger er.</returns>
        public List<Tilmelding> HentAlle()
        {
            List<Tilmelding> tilmeldinger = new List<Tilmelding>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                @"SELECT SpejderId, AktivitetsId, Tilmeldingsdato
                  FROM Tilmelding",
                db.forbind);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Tilmelding tilmelding = new Tilmelding(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    DateOnly.FromDateTime(reader.GetDateTime(2))
                );

                tilmeldinger.Add(tilmelding);
            }

            reader.Close();
            db.LukForbindelse();

            return tilmeldinger;
        }

        /// <summary>
        /// Henter alle tilmeldinger for én spejder.
        /// </summary>
        /// <param name="spejderId">Id på spejderen.</param>
        /// <returns>En liste med spejderens tilmeldinger. Listen er tom hvis spejderen ikke er tilmeldt noget.</returns>
        public List<Tilmelding> HentForSpejder(int spejderId)
        {
            List<Tilmelding> resultat = new List<Tilmelding>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                @"SELECT SpejderId, AktivitetsId, Tilmeldingsdato
                  FROM Tilmelding
                  WHERE SpejderId = @spejderId",
                db.forbind);

            command.Parameters.AddWithValue("@spejderId", spejderId);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                resultat.Add(new Tilmelding(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    DateOnly.FromDateTime(reader.GetDateTime(2))
                ));
            }

            reader.Close();
            db.LukForbindelse();

            return resultat;
        }

        /// <summary>
        /// Henter alle tilmeldinger til én aktivitet.
        /// </summary>
        /// <param name="aktivitetId">Id på aktiviteten.</param>
        /// <returns>En liste med aktivitetens tilmeldinger. Listen er tom hvis ingen er tilmeldt.</returns>
        public List<Tilmelding> HentForAktivitet(int aktivitetId)
        {
            List<Tilmelding> resultat = new List<Tilmelding>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                @"SELECT SpejderId, AktivitetsId, Tilmeldingsdato
                  FROM Tilmelding
                  WHERE AktivitetsId = @aktivitetId",
                db.forbind);

            command.Parameters.AddWithValue("@aktivitetId", aktivitetId);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                resultat.Add(new Tilmelding(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    DateOnly.FromDateTime(reader.GetDateTime(2))
                ));
            }

            reader.Close();
            db.LukForbindelse();

            return resultat;
        }

        /// <summary>
        /// Tjekker om en spejder allerede er tilmeldt en aktivitet.
        /// </summary>
        /// <param name="spejderId">Id på spejderen.</param>
        /// <param name="aktivitetId">Id på aktiviteten.</param>
        /// <returns>true hvis spejderen er tilmeldt aktiviteten, ellers false.</returns>
        public bool ErTilmeldt(int spejderId, int aktivitetId)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"SELECT COUNT(*)
                  FROM Tilmelding
                  WHERE SpejderId = @spejderId
                  AND AktivitetsId = @aktivitetId",
                db.forbind);

            command.Parameters.AddWithValue("@spejderId", spejderId);
            command.Parameters.AddWithValue("@aktivitetId", aktivitetId);

            int antal = (int)command.ExecuteScalar();

            db.LukForbindelse();

            return antal > 0;
        }

        /// <summary>
        /// Tjekker reglerne for tilmelding: Spejderen må ikke allerede være tilmeldt, aktiviteten må ikke være fuld, spejderen skal være gammel nok og have alle krævede tegn, og aktiviteten må ikke være begyndt.
        /// </summary>
        /// <param name="spejder">Den spejder der vil tilmeldes.</param>
        /// <param name="aktivitet">Den aktivitet spejderen vil tilmeldes.</param>
        /// <returns>En tom tekst hvis spejderen kan tilmeldes, ellers en fejlbesked med den første regel der ikke er opfyldt.</returns>
        public string KanTilmelde(Spejder spejder, Aktivitet aktivitet)
        {
            if (ErTilmeldt(spejder.Id, aktivitet.Id))
            {
                return "Du er allerede tilmeldt denne aktivitet";
            }

            if (HentForAktivitet(aktivitet.Id).Count >= aktivitet.MaksDeltagere)
            {
                return "Aktiviteten er fuld";
            }

            if (spejder.Alder < aktivitet.MinAlder)
            {
                return "Du er for ung til denne aktivitet";
            }

            foreach (int tegnId in aktivitet.KraevedeTegnIds)
            {
                if (!spejder.TegnIds.Contains(tegnId))
                {
                    return "Du mangler et tegn for at deltage";
                }
            }

            if (aktivitet.Tidspunkt < DateTime.Now)
            {
                return "Aktiviteten er allerede begyndt";
            }

            return "";
        }

        /// <summary>
        /// Tilmelder en spejder til en aktivitet, hvis reglerne i KanTilmelde er opfyldt. Tilmeldingsdatoen sættes til dags dato.
        /// </summary>
        /// <param name="spejder">Den spejder der skal tilmeldes.</param>
        /// <param name="aktivitet">Den aktivitet spejderen skal tilmeldes.</param>
        /// <returns>En tom tekst hvis tilmeldingen lykkedes, ellers en fejlbesked.</returns>
        public string Tilmeld(Spejder spejder, Aktivitet aktivitet)
        {
            string fejl = KanTilmelde(spejder, aktivitet);

            if (fejl != "")
            {
                return fejl;
            }

            db.OpretForbindelse();

            var command = new SqlCommand(
                @"INSERT INTO Tilmelding
                  (SpejderId, AktivitetsId, Tilmeldingsdato)
                  VALUES
                  (@spejderId, @aktivitetId, @dato)",
                db.forbind);

            command.Parameters.AddWithValue("@spejderId", spejder.Id);
            command.Parameters.AddWithValue("@aktivitetId", aktivitet.Id);
            command.Parameters.AddWithValue("@dato", DateTime.Today);

            command.ExecuteNonQuery();

            db.LukForbindelse();

            return "";
        }

        /// <summary>
        /// Framelder en spejder fra en aktivitet.
        /// </summary>
        /// <param name="spejderId">Id på den spejder der skal frameldes.</param>
        /// <param name="aktivitetId">Id på den aktivitet spejderen skal frameldes.</param>
        public void Frameld(int spejderId, int aktivitetId)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"DELETE FROM Tilmelding
                  WHERE SpejderId = @spejderId
                  AND AktivitetsId = @aktivitetId",
                db.forbind);

            command.Parameters.AddWithValue("@spejderId", spejderId);
            command.Parameters.AddWithValue("@aktivitetId", aktivitetId);

            command.ExecuteNonQuery();

            db.LukForbindelse();
        }

        /// <summary>
        /// Sletter alle tilmeldinger til en aktivitet.
        /// </summary>
        /// <param name="aktivitetId">Id på aktiviteten.</param>
        public void SletForAktivitet(int aktivitetId)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "DELETE FROM Tilmelding WHERE AktivitetsId = @aktivitetId",
                db.forbind);

            command.Parameters.AddWithValue("@aktivitetId", aktivitetId);

            command.ExecuteNonQuery();

            db.LukForbindelse();
        }

        /// <summary>
        /// Sletter alle tilmeldinger for en spejder.
        /// </summary>
        /// <param name="spejderId">Id på spejderen.</param>
        public void SletForSpejder(int spejderId)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "DELETE FROM Tilmelding WHERE SpejderId = @spejderId",
                db.forbind);

            command.Parameters.AddWithValue("@spejderId", spejderId);

            command.ExecuteNonQuery();

            db.LukForbindelse();
        }
    }
}
