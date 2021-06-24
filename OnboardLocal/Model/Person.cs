namespace OnboardLocal.Model
{
    public class Person
    {
        public int Pk { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Background { get; set; } = "";
        public string Drug { get; set; } = "";
        public bool Change { get; set; } = false;

        public override string ToString()
        {
            return $"${FirstName}, ${Lastname}, ${Phone}, ${Email}, ${Background}, ${Drug}";
        }

        public Person(int pk, string firstName, string lastname, string phone, string email, string background, string drug, bool change)
        {
            Pk = pk;
            FirstName = firstName;
            Lastname = lastname;
            Phone = phone;
            Email = email;
            Background = background;
            Drug = drug;
            Change = change;
        }
    }
    
    
}