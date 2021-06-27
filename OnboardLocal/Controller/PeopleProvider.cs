using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using OnboardLocal.Model;

namespace OnboardLocal.Controller
{
    public class PeopleProvider
    {
        static string path = Path.Combine(Environment.CurrentDirectory, "data") + @"\onboard.db";


        static PeopleProvider()
        {
        }

        private static SQLiteConnection CreateConn()
        {
            var conn = new SQLiteConnection($"Data Source= {path};Version=3;");
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "data")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "data"));
            if (!File.Exists(path))
                File.Create(path);
            try
            {
                conn.Open();
                CreateTables(conn);
            }
            catch (SQLiteException)
            {
                throw new Exception("Error opening database");
            }

            return conn;
        }

        private static void CreateTables(SQLiteConnection conn)
        {
            const string createOnboarding =
                "CREATE TABLE IF NOT EXISTS onboarding (pk INTEGER PRIMARY KEY AUTOINCREMENT, first TEXT NOT NULL, last TEXT NOT NULL, phone TEXT NOT NULL UNIQUE, email TEXT NOT NULL UNIQUE, background TEXT, drug TEXT, change INTEGER)";
            using (var sqliteCmd = new SQLiteCommand(createOnboarding, conn))
            {
                sqliteCmd.ExecuteNonQuery();
            }
        }

        public void InsertPeople(IEnumerable<Person> people)
        {
            foreach (var person in people)
            {
                InsertPerson(person);
            }
        }

        public void InsertPerson(Person person)
        {
            const string insert =
                "INSERT INTO onboarding(first, last, phone, email, background, drug, change) VALUES(@first, @last, @phone, @email, @background, @drug, 0)";
            
            
            var args = new Dictionary<string, object>()
            {
                {"@first", person.FirstName},
                {"@last", person.Lastname},
                {"@phone", person.Phone},
                {"@email", person.Email},
                {"@background", person.Background},
                {"@drug", person.Drug}
            };
            ExecuteWrite(insert, args);
        }

        public void UpdatePerson(Person person)
        {
            const string update =
                "UPDATE onboarding SET first = @first, last = @last, phone = @phone, email = @email, background = @background, drug = @drug, change = @change WHERE pk = @pk";
            var args = new Dictionary<string, object>()
            {
                {"@pk", person.Pk},
                {"@first", person.FirstName},
                {"@last", person.Lastname},
                {"@phone", person.Phone},
                {"@email", person.Email},
                {"@background", person.Background},
                {"@drug", person.Drug},
                {"@change", person.Change}
            };
            ExecuteWrite(update, args);
        }

        public void UpdatePeople(IEnumerable<Person> people)
        {
            foreach (var person in people)
            {
                UpdatePerson(person);
            }
        }

        public IEnumerable<Person> GetPeople()
        {
            const string select = "SELECT * FROM onboarding";
            var conn = CreateConn();
            var people = new List<Person>();
            using (var sqliteCmd = new SQLiteCommand(select, conn))
            {
                var reader = sqliteCmd.ExecuteReader();
                while (reader.Read())
                {
                    people.Add(new Person(reader.GetInt32(0), 
                        reader.GetString(1), 
                        reader.GetString(2), 
                        reader.GetString(3), 
                        reader.GetString(4), 
                        reader.GetString(5), 
                        reader.GetString(6), 
                        reader.GetInt32(7) > 0));        
                }
            }
            return people;
        }
        
        private static void ExecuteWrite(string query, Dictionary<string, object> args)
        {
            var conn = CreateConn();
            {
                using (var sqliteCmd = new SQLiteCommand(conn))
                {
                    sqliteCmd.CommandText = query;
                    foreach (var pair in args)
                    {
                        sqliteCmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                    sqliteCmd.Prepare();
                    sqliteCmd.ExecuteNonQuery();
                }
            }
        }
    }
}