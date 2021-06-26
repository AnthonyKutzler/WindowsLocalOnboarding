using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OnboardLocal.Model;

namespace OnboardLocal.Controller
{
    public class CsvService<T>
    {
        

        public void ImportPeople(string pathToFile)
        {
            var people = new List<Person>();
            if (File.Exists(pathToFile))
            {
                var reader = new StreamReader(File.OpenRead(pathToFile));
                while (!reader.EndOfStream)
                {
                    var val = reader.ReadLine().Split(',');
                    people.Add(new Person(val[0],val[1],val[2],val[3],val[4],val[5]));
                }
                new PeopleProvider().InsertPeople(people);
            }
            else
            {
                throw new FileNotFoundException("File Not Found!");
            }
        }

        public void ExportPeople(string pathToFile, IEnumerable<T> people)
        {
            const string header = "First, Last, Phone, Email, Background, Drug";
            CreateCsvFile(header, people, pathToFile);
        }

        public void CreateCsvFile(string header, IEnumerable<T> list, string path)
        {
            var csv = new StringBuilder(header);
            foreach (var item in list)
            {
                csv.AppendLine(item.ToString());
            }
            File.WriteAllText(path, csv.ToString());
        }
    }
}