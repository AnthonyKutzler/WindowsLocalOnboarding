using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OnboardLocal.Model;
using CsvHelper;

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
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var val = reader.ReadLine().Split(',');
                    var background = "";
                    var drug = "";
                    if (val.Length > 4)
                        background = val[4];
                    if (val.Length > 5)
                        drug = val[5];
                    people.Add(new Person(val[0],val[1],val[2],val[3], background, drug));
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
            CreateCsvFile(people, pathToFile);
            //CreateCsvFile(header, people, pathToFile);
        }

        public void CreateCsvFile(IEnumerable<T> list, string path)
        {
            using(var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
                csv.WriteRecords(list);
            }
            
            
            
            //var csv = new StringBuilder(header);
            /*foreach (var item in list)
            {
                csv.AppendLine(item.ToString());
            }
            File.WriteAllText(path, csv.ToString());
        */}
    }
}