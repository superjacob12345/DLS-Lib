using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DLS_Lib.UserStories
{
    public class DLSopretForbindelse
    {
        // Bruges hvis der ikke er sat en "PCstring" i User Secrets
        private const string StandardForbindelse =
            "Server=(localdb)\\MSSQLLocalDB;Database=DLS;Trusted_Connection=True;TrustServerCertificate=True;";

        SqlConnection _forbind = new SqlConnection();

        public SqlConnection forbind
        {
            get { return _forbind; }
            set { _forbind = value; }
        }

        public DLSopretForbindelse()
        {

        }

        public void OpretForbindelse()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .Build();

            string? connectionString = config["PCstring"];
            if (connectionString == null || connectionString == "")
            {
                connectionString = StandardForbindelse;
            }

            _forbind = new SqlConnection(connectionString);
            _forbind.Open();
        }

        public void LukForbindelse()
        {
            _forbind.Close();
        }
    }
}
