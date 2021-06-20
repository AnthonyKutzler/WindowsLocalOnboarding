using System;
using System.Data.SQLite;
using System.IO;

namespace OnboardLocal.Controller
{
    public class PeopleProvider
    {
        static string path = Path.Combine(Environment.CurrentDirectory, "data") + @"/onboard.db";
        
        static PeopleProvider(){}

        private static SQLiteConnection CreateConn()
        {
            var conn = new SQLiteConnection($"Data Source= ${path};Version=3;");
            try
            {
                conn.Open();
            }
            catch (SQLiteException e)
            {
                throw new Exception("Error opening database");
            }

            return conn;
        }

        private static void CreateTables(SQLiteConnection conn)
        {
            string createOnboarding =
                "CREATE TABLE onboarding (pk INTEGER PRIMARY KEY AUTOINCREMENT, first TEXT NOT NULL, last TEXT NOT NULL, phone TEXT NOT NULL UNIQUE, email TEXT NOT NULL UNIQUE)";
            
        }
    }
}