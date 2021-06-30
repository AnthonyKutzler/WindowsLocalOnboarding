using System;
using System.Text.RegularExpressions;
using CsvHelper.Configuration.Attributes;

namespace OnboardLocal.Model
{
    public class DrugTest
    {
        [Name("Primary ID")]
        public string PrimaryId{ get; set; }
        [Name("First Name")]
        public string FirstName{ get; set; }
        [Name("Last Name")]
        public string LastName{ get; set; }
        [Name("Phone Number")]
        public string PhoneNumber{ get; set; }
        [Name("Date of Birth")]
        public string DOB{ get; set; }
        [Name("Account Number")]
        public string Account{ get; set; }
        [Name("Modality")]
        public string Modality{ get; set; }
        [Name("Client Site Location")]
        public string ClientSiteLocation{ get; set; }
        [Name("Order Code(s)")]
        public string OrderCode{ get; set; }
        [Name("Collection Type")]
        public string Split { get; set; }
        [Name("Reason for Test")]
        public string TestReason { get; set; }
        [Name("Order Expiration Date")]
        public string ExpirationDate{ get; set; }
        [Name("Order Expiration Time")]
        public string ExpirationTime { get; set; }
        [Name("Collection Site Code")]
        public string CollectionSite{ get; set; }
        [Name("Observed")]
        public string Observed { get; set; }
        [Name("Email(s)")]
        public string Email{ get; set; }

        

        public DrugTest(Person person)
        {
            PrimaryId = Regex.Replace(person.Phone, @"[\D+]", "");
            FirstName = person.FirstName;
            LastName = person.Lastname;
            PhoneNumber = Regex.Replace(person.Phone, @"[\D+]", "");
            Account = Properties.Settings.Default.AccountId;
            DOB = "";
            Modality = "";
            ClientSiteLocation = "";
            OrderCode = Properties.Settings.Default.OrderCode;
            Split = "Split";
            TestReason = "Pre-Employment";
            ExpirationDate = DateTime.Now.AddDays(14).ToString("MM/dd/yyyy");
            ExpirationTime = "19:00";
            CollectionSite = Properties.Settings.Default.CollectionSite;
            Observed = "No";
            Email = person.Email;
        }

        
    }
}