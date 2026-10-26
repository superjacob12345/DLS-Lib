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
       
        private int _næsteId = 1;

        


        // CREATE
        public void Opret(Aktivitet aktivitet)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                @"INSERT INTO Aktivitet
          (Navn, Tidspunkt, MaksDeltagere, MinAlder, Sted, Beskrivelse)
          VALUES
          (@navn, @tidspunkt, @maksDeltagere, @minAlder, @sted, @beskrivelse)",
                db.forbind);

            command.Parameters.AddWithValue("@navn", aktivitet.Navn);
            command.Parameters.AddWithValue("@tidspunkt", aktivitet.Tidspunkt);
            command.Parameters.AddWithValue("@maksDeltagere", aktivitet.MaksDeltagere);
            command.Parameters.AddWithValue("@minAlder", aktivitet.MinAlder);
            command.Parameters.AddWithValue("@sted", aktivitet.Sted);
            command.Parameters.AddWithValue("@beskrivelse", aktivitet.Beskrivelse);

            command.ExecuteNonQuery();
            db.LukForbindelse();
        }

        // READ - hent alle aktiviteter
        public List<Aktivitet> HentAlle()
        {
            var aktiviteter = new List<Aktivitet>();

            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT * FROM Aktivitet",
                db.forbind);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var aktivitet = new Aktivitet
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    Tidspunkt = reader.GetDateTime(2),
                    MaksDeltagere = reader.GetInt32(3),
                    MinAlder = reader.GetInt32(4),
                    Sted = reader.GetString(5),
                    Beskrivelse = reader.GetString(6)
                };

                aktiviteter.Add(aktivitet);
            }

            reader.Close();
            db.LukForbindelse();

            return aktiviteter;
        }

        // READ - hent én aktivitet
        public Aktivitet? HentEn(int id)
        {
            db.OpretForbindelse();

            var command = new SqlCommand(
                "SELECT * FROM Aktivitet WHERE AktivitetsId = @id",
                db.forbind);

            command.Parameters.AddWithValue("@id", id);

            var reader = command.ExecuteReader();

            Aktivitet? aktivitet = null;

            if (reader.Read())
            {
                aktivitet = new Aktivitet
                {
                    Id = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    Tidspunkt = reader.GetDateTime(2),
                    MaksDeltagere = reader.GetInt32(3),
                    MinAlder = reader.GetInt32(4),
                    Sted = reader.GetString(5),
                    Beskrivelse = reader.GetString(6)
                };
            }

            reader.Close();
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
            db.LukForbindelse();
        }

        // DELETE
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
    }
}