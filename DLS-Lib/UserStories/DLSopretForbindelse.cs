using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection;
using System.Text;

namespace DLS_Lib.UserStories 
{
    public class DLSopretForbindelse
    {
 


        public void OpretForbindelse()
        {
            var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(),
                            optional: true)
                .Build();
            var connectionString = config["PCstring"];
            _forbind = new SqlConnection(connectionString);
            _forbind.Open();
        }

        public void LukForbindelse()
        {
            _forbind.Close();
        }
        SqlConnection _forbind;
        public SqlConnection forbind
        {
            get { return _forbind; }
            set { _forbind = value; }

        }
        public DLSopretForbindelse()
        {

        }

    }
}