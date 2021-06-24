using System;
using System.Text.RegularExpressions;

namespace OnboardLocal.Model
{
    public class DrugTest
    {
        public string PrimaryId;
        public Person Person;
        public string Account;
        public string Blank = "";
        public string OrderCode;
        public string Split = "Split";
        public string TestReason = "Pre-Employment";
        public string ExpirationDate;
        public string ExpirationTime = "19:00";
        public string CollectionSite;
        public string Observed = "No";

        

        public DrugTest(Person person)
        {
            PrimaryId = Regex.Replace(person.Phone, "[^a-zA-Z]+", "");
            Person = person;
            Account = Properties.Settings.Default.AccountId;
            Blank = "";
            OrderCode = Properties.Settings.Default.OrderCode;
            Split = "Split";
            TestReason = "Pre-Employment";
            ExpirationDate = DateTime.Now.AddDays(14).ToString();
            ExpirationTime = "19:00";
            CollectionSite = Properties.Settings.Default.CollectionSite;
            Observed = "No";
        }

        public override string ToString()
        {
            return $"${PrimaryId}, ${Regex.Replace(Person.FirstName, "[^a-zA-Z]+", "")}, ${Regex.Replace(Person.Lastname, "[^a-zA-Z]+", "")}, ${Regex.Replace(Person.Phone, "[^a-zA-Z]+", "")}, ${Blank}, ${Account}, ${Blank}, ${OrderCode}, ${Split}, ${TestReason}, ${ExpirationDate}, ${ExpirationTime}, ${CollectionSite}, ${Observed}, ${Person.Email}";
        }
    }
}