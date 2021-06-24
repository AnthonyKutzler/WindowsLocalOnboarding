using System.Text.RegularExpressions;

namespace OnboardLocal.Model
{
    public class DrugTest
    {
        public string PrimaryId;
        public string First;
        public string Last;
        public string Phone;
        public string Account;
        public string Blank = "";
        public string OrderCode;
        public string Split = "Split";
        public string TestReason = "Pre-Employment";
        public string ExpirationDate;
        public string ExpirationTime = "19:00";
        public string CollectionSite;
        public string Observed = "No";
        public string Email;

        public DrugTest(string first, string last, string phone, string email)
        {
            PrimaryId = Regex.Replace(phone, "[^a-zA-Z]+", "");
            First = Regex.Replace(first, "[^a-zA-Z]+", "");
            Last = Regex.Replace(last, "[^a-zA-Z]+", "");;
            Phone = Regex.Replace(phone, "[^a-zA-Z]+", "");
            Account = Properties.Settings.Default.AccountId;
            Blank = "";
            OrderCode = Properties.Settings.Default.OrderCode;
            Split = "Split";
            TestReason = "Pre-Employment";
            ExpirationDate = expirationDate;
            ExpirationTime = expirationTime;
            CollectionSite = Properties.Settings.Default.CollectionSite;
            Observed = "No";
            Email = email;
        }

        public DrugTest(Person person)
        {
            PrimaryId = Regex.Replace(person.Phone, "[^a-zA-Z]+", "");
            First = Regex.Replace(person.FirstName, "[^a-zA-Z]+", "");
            Last = Regex.Replace(person.Lastname, "[^a-zA-Z]+", "");;
            Phone = Regex.Replace(person.Phone, "[^a-zA-Z]+", "");
            Account = Properties.Settings.Default.AccountId;
            Blank = "";
            OrderCode = Properties.Settings.Default.OrderCode;
            Split = "Split";
            TestReason = "Pre-Employment";
            ExpirationDate = expirationDate;
            ExpirationTime = expirationTime;
            CollectionSite = Properties.Settings.Default.CollectionSite;
            Observed = "No";
            Email = person.Email;
        }

        public override string ToString()
        {
            return $"${}, ${}, ${}, ${}, ${Blank}, ${}, ${Blank}, ${}, ${}, ${}, ${}, ${}, ${}, ${}, ${}";
        }
    }
}