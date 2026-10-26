using DLS_Lib.UserStories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLS_Lib.Repository
{
    public class AktivitetRepository
    {
        private DLSopretForbindelse db = new DLSopretForbindelse();

        // CREATE
        public void Opret(Aktivitet aktivitet)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"INSERT INTO Aktivitet
                  (Navn, Tidspunkt, MaksDeltagere, MinAlder, Sted, Beskrivelse)
                  OUTPUT INSERTED.AktivitetsId
                  VALUES
                  (@navn, @tidspunkt, @maksDeltagere, @minAlder, @sted, @beskrivelse)",
                db.forbind);

            command.Parameters.AddWithValue("@navn", aktivitet.Navn);
            command.Parameters.AddWithValue("@tidspunkt", aktivitet.Tidspunkt);
            command.Parameters.AddWithValue("@maksDeltagere", aktivitet.MaksDeltagere);
            command.Parameters.AddWithValue("@minAlder", aktivitet.MinAlder);
            command.Parameters.AddWithValue("@sted", aktivitet.Sted);
            command.Parameters.AddWithValue("@beskrivelse", aktivitet.Beskrivelse);

            // Databasen giver aktiviteten et Id
            aktivitet.Id = (int)command.ExecuteScalar();

            GemLedereOgTegn(aktivitet);
            db.LukForbindelse();
        }

        // READ - hent alle aktiviteter
        public List<Aktivitet> HentAlle()
        {
            var aktiviteter = new List<Aktivitet>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                @"SELECT AktivitetsId, Navn, Tidspunkt, MaksDeltagere, MinAlder, Sted, Beskrivelse
                  FROM Aktivitet",
                db.forbind);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                aktiviteter.Add(LavAktivitet(reader));
            }

            reader.Close();

            foreach (Aktivitet aktivitet in aktiviteter)
            {
                HentLedereOgTegn(aktivitet);
            }

            db.LukForbindelse();

            return aktiviteter;
        }

        // READ - hent én aktivitet
        public Aktivitet? HentEn(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"SELECT AktivitetsId, Navn, Tidspunkt, MaksDeltagere, MinAlder, Sted, Beskrivelse
                  FROM Aktivitet
                  WHERE AktivitetsId = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            var reader = command.ExecuteReader();

            Aktivitet? aktivitet = null;

            if (reader.Read())
            {
                aktivitet = LavAktivitet(reader);
            }

            reader.Close();

            if (aktivitet != null)
            {
                HentLedereOgTegn(aktivitet);
            }

            db.LukForbindelse();

            return aktivitet;
        }

        // UPDATE
        public void Opdater(Aktivitet aktivitet)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"UPDATE Aktivitet
                  SET Navn = @navn,
                      Tidspunkt = @tidspunkt,
                      MaksDeltagere = @maksDeltagere,
                      MinAlder = @minAlder,
                      Sted = @sted,
                      Beskrivelse = @beskrivelse
                  WHERE AktivitetsId = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", aktivitet.Id);
            command.Parameters.AddWithValue("@navn", aktivitet.Navn);
            command.Parameters.AddWithValue("@tidspunkt", aktivitet.Tidspunkt);
            command.Parameters.AddWithValue("@maksDeltagere", aktivitet.MaksDeltagere);
            command.Parameters.AddWithValue("@minAlder", aktivitet.MinAlder);
            command.Parameters.AddWithValue("@sted", aktivitet.Sted);
            command.Parameters.AddWithValue("@beskrivelse", aktivitet.Beskrivelse);

            command.ExecuteNonQuery();

            // Slet de gamle ledere og tegn og gem de nye
            var sletCommand = new SqlCommand(
                @"DELETE FROM AktivitetLeder WHERE AktivitetsId = @id;
                  DELETE FROM AktivitetTegn WHERE AktivitetsId = @id",
                db.forbind);
            sletCommand.Parameters.AddWithValue("@id", aktivitet.Id);
            sletCommand.ExecuteNonQuery();

            GemLedereOgTegn(aktivitet);
            db.LukForbindelse();
        }

        // DELETE (ledere, tegn og tilmeldinger slettes automatisk med ON DELETE CASCADE)
        public void Slet(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "DELETE FROM Aktivitet WHERE AktivitetsId = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // Laver en aktivitet ud fra den række readeren står på
        private Aktivitet LavAktivitet(SqlDataReader reader)
        {
            Aktivitet aktivitet = new Aktivitet();
            aktivitet.Id = reader.GetInt32(0);
            aktivitet.Navn = reader.GetString(1);
            aktivitet.Tidspunkt = reader.GetDateTime(2);
            aktivitet.MaksDeltagere = reader.GetInt32(3);
            aktivitet.MinAlder = reader.GetInt32(4);
            aktivitet.Sted = reader.GetString(5);
            aktivitet.Beskrivelse = reader.GetString(6);
            return aktivitet;
        }

        // Henter ledere og krævede tegn. Forbindelsen skal være åben.
        private void HentLedereOgTegn(Aktivitet aktivitet)
        {
            var lederCommand = new SqlCommand(
                "SELECT LederId FROM AktivitetLeder WHERE AktivitetsId = @id",
                db.forbind);
            lederCommand.Parameters.AddWithValue("@id", aktivitet.Id);

            var reader = lederCommand.ExecuteReader();
            while (reader.Read())
            {
                aktivitet.LederIds.Add(reader.GetInt32(0));
            }
            reader.Close();

            var tegnCommand = new SqlCommand(
                "SELECT TegnId FROM AktivitetTegn WHERE AktivitetsId = @id",
                db.forbind);
            tegnCommand.Parameters.AddWithValue("@id", aktivitet.Id);

            reader = tegnCommand.ExecuteReader();
            while (reader.Read())
            {
                aktivitet.KraevedeTegnIds.Add(reader.GetInt32(0));
            }
            reader.Close();
        }

        // Gemmer ledere og krævede tegn. Forbindelsen skal være åben.
        private void GemLedereOgTegn(Aktivitet aktivitet)
        {
            foreach (int lederId in aktivitet.LederIds)
            {
                var command = new SqlCommand(
                    "INSERT INTO AktivitetLeder (AktivitetsId, LederId) VALUES (@aktivitetId, @lederId)",
                    db.forbind);
                command.Parameters.AddWithValue("@aktivitetId", aktivitet.Id);
                command.Parameters.AddWithValue("@lederId", lederId);
                command.ExecuteNonQuery();
            }

            foreach (int tegnId in aktivitet.KraevedeTegnIds)
            {
                var command = new SqlCommand(
                    "INSERT INTO AktivitetTegn (AktivitetsId, TegnId) VALUES (@aktivitetId, @tegnId)",
                    db.forbind);
                command.Parameters.AddWithValue("@aktivitetId", aktivitet.Id);
                command.Parameters.AddWithValue("@tegnId", tegnId);
                command.ExecuteNonQuery();
            }
        }
    }
}
