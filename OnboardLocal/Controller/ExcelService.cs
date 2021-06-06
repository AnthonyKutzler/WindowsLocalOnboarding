using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using OnboardLocal.Model;
using Excel = Microsoft.Office.Interop.Excel;

namespace OnboardLocal.Controller
{
    public class ExcelService
    {

        private readonly string _path;
        private readonly Excel.Application _excelApplication;
        private Excel.Workbook _excelWorkbook;
        private Excel.Worksheet _excelWorksheet;
        private Excel.Range _excelRange;
        

        public ExcelService(string path)
        {
            _path = path;
            _excelApplication = new Excel.Application();
            
        }
        public IEnumerable<Person> GetPeople()
        {
            _excelWorkbook = _excelApplication.Workbooks.Open(_path);
            _excelWorksheet = (Excel.Worksheet) _excelWorkbook.Worksheets.get_Item(1);
            _excelRange = _excelWorksheet.UsedRange;
            var people = new Person[_excelRange.Rows.Count - 1];
            for (var r = 2; r <= _excelRange.Rows.Count; r++)
            {
                var person = new Person
                {
                    Line = r,
                    FirstName = _excelRange.Cells[r, 1].ToString(),
                    Lastname = _excelRange.Cells[r, 2].ToString(),
                    Phone = _excelRange.Cells[r, 3].ToString(),
                    Email = _excelRange.Cells[r, 4].ToString(),
                    Background = _excelRange.Cells[r, 5].ToString(),
                    Drug = _excelRange.Cells[r, 6].ToString()
                };
                people[r - 2] = person;
            }
            return people;
        }

        private bool Save(IEnumerable<Person> people)
        {
            foreach(var person in people)
            {
                
                _excelWorksheet.Cells[person.Line, 1] = person.FirstName;
                _excelWorksheet.Cells[person.Line, 2] = person.Lastname;
                _excelWorksheet.Cells[person.Line, 3] = person.Phone;
                _excelWorksheet.Cells[person.Line, 4] = person.Email;
                _excelWorksheet.Cells[person.Line, 5] = person.Background;
                _excelWorksheet.Cells[person.Line, 6] = person.Drug;
                _excelWorksheet.Cells[person.Line, 7] = person.Change;
            }
            
            Close(_path);
            return true;
        }
        
        public string CreateTemplate()
        {
            _excelWorkbook = _excelApplication.Workbooks.Add();
            _excelWorksheet = (Excel.Worksheet)  _excelWorkbook.Worksheets.get_Item(1);

            _excelWorksheet.Cells[1, 1] = "Firstname";
            _excelWorksheet.Cells[1, 2] = "Lastname";
            _excelWorksheet.Cells[1, 3] = "Phone";
            _excelWorksheet.Cells[1, 4] = "Email";
            _excelWorksheet.Cells[1, 5] = "Background";
            _excelWorksheet.Cells[1, 6] = "Drug";
            _excelWorksheet.Cells[1, 7] = "Change";
            _excelWorksheet.Cells[1, 8] = "Class";
            
            Close(Path.Combine(Environment.CurrentDirectory, "data") + @"/onboard.xlsx");
            return "";
        }

        private void Close(string path)
        {
            object misValue = System.Reflection.Missing.Value;
            _excelWorkbook.SaveAs(path, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            _excelWorkbook.Close(true, misValue, misValue);
            _excelApplication.Quit();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if(_excelRange != null)
                Marshal.ReleaseComObject(_excelRange);
            Marshal.ReleaseComObject(_excelWorksheet);
            Marshal.ReleaseComObject(_excelWorkbook);
            Marshal.ReleaseComObject(_excelApplication);
        }
    }
}

