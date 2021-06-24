using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OnboardLocal.Controller
{
    public class CsvService<T>
    {
        

        public IEnumerable<T> GetFromCsv(string pathToFile)
        {
            throw new System.NotImplementedException();
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