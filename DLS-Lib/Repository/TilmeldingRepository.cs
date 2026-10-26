using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DLS_Lib.UserStories;

namespace DLS_Lib.Repository
{
    public class TilmeldingRepository
    {
        private DLSopretForbindelse db = new DLSopretForbindelse();

        // READ - hent alle tilmeldinger
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

        // READ - hent alle tilmeldinger for én spejder
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

        // READ - hent alle tilmeldinger til én aktivitet
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

        // Tjek om spejderen allerede er tilmeldt
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

        // Tjek reglerne for tilmelding
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

        // CREATE - tilmeld en spejder
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

        // DELETE - frameld en spejder fra en aktivitet
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

        // DELETE - slet alle tilmeldinger til en aktivitet
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

        // DELETE - slet alle tilmeldinger for en spejder
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
