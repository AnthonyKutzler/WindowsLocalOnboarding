namespace OnboardLocal.Model
{
    public class Person
    {
        public int Line { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Background { get; set; } = "";
        public string Drug { get; set; } = "";
        public bool Change { get; set; } = false;
        
        public void ExcelLineToPerson(string[] values)
        {
            FirstName = values[0];
            Lastname = values[1];
            Phone = values[2];
            Email = values[3];
            Background = values[4];
            Drug = values[5];
        }
        
    }
    
    
}