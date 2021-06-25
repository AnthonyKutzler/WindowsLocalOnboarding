using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using OnboardLocal.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace OnboardLocal.Controller
{
    public class Quest : IWebService
    {
        public IWebDriver Driver { get; set; }
        private readonly string _user;
        private readonly string _pass;
        private readonly WebDriverWait _wait;
        private int _loginAttempts;
        private int _nameIndex;
        private int _resultIndex;
        private List<DrugTest> _newTests = new List<DrugTest>();

        private const string NewTestUrl = "https://esp.employersolutions.com/ImportOrder/Index";
        private const string SearchUrl = "https://esp.employersolutions.com/Results/Summary";
        private const string BodyXpath = "//*[@id=\"table-items\"]";
        private const string Csv = "";

        public Quest(string user, string pass, IWebDriver driver)
        {
            Driver = driver;
            _wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
            _user = user;
            _pass = pass;
        }

        public void Setup()
        {
            Driver.Url = SearchUrl;
            _wait.Until(driver => driver.FindElement(By.XPath(BodyXpath)));
        }

        public Person Search(Person person)
        {
            Driver.FindElement(By.XPath("//*[@id=\"search-boxes\"]/div/input[1]")).SendKeys(person.Phone.Replace("[\\D+]", ""));
            Driver.FindElement(By.XPath("//*[text()='Search']"));
            _wait.Until(driver => driver.FindElement(By.XPath(BodyXpath)));
            DefineColumnIndex();

            var oldValue = 0;
            try
            {
                foreach (var ele in Driver.FindElements(By.XPath($"${BodyXpath}/*")))
                {
                    var name = FormatName(ele.FindElement(By.XPath($"./td[${_nameIndex}]")).Text);
                    //If not person in question, continue.
                    if (!name.Contains(FormatName(person.FirstName)) ||
                        !name.Contains(FormatName(person.Lastname))) continue;
                    var result = ele.FindElement(By.XPath($"./td[${_resultIndex}]")).Text;
                    var newValue = ValueFromDrugResult(result);
                    if (newValue <= oldValue) continue;
                    person.Drug = result;
                    oldValue = newValue;
                }
            }
            catch (NoSuchElementException)
            {
                person.Drug = "";
                _newTests.Add(new DrugTest(person));
            }

            return person;
        }

        public bool Login()
        {
            try
            {
                if (_loginAttempts > 3)
                    throw new LoginException("Quest Failed to login, Please Check Password!");
                Driver.Url = SearchUrl;
                Driver.FindElement(By.XPath("//*[@id=\"UserName\"]")).SendKeys(_user);
                Driver.FindElement(By.XPath("//*[@id=\"Password\"]")).SendKeys(_pass);
                Driver.FindElement(By.XPath("//*[text()='Secure sign in']")).Click();
                _wait.Until(driver => driver.FindElement(By.XPath("//*[text()='DASHBOARD'")));
                return true;
            }
            catch (NoSuchElementException)
            {
                _loginAttempts++;
                return false;
            }
        }

        public void SetupNewTests()
        {
            if (!Login()) return;
            try
            {
                const string line =
                    "Primary ID, First Name, Last Name, Primary Phone, Date of Birth, Account Number, Modality, " 
                    + "Client Site Location, Order Code(s), Collection Type, Reason for Test, Order Expiration Date, " 
                    + "Order Expiration Time, Collection Site Code, Observed, Email(s)";
                new CsvService<DrugTest>().CreateCsvFile(line, _newTests, Csv);

                    //TODO Finish Implementing Logic
                
            }
            catch (Exception) { /* ignored*/ }
        }

        private static string FormatName(string name)
        {
            return Regex.Replace(name, "[^a-zA-Z]+", "");
        }

        private static int ValueFromDrugResult(string result)
        {
            switch (result)
            {
                case "No Show/Expired":
                    return 1;
                case "Positive":
                    return 2;
                case "Scheduled":
                    return 3;
                case "Collected":
                    return 4;
                case "At Lab":
                    return 5;
                case "Pending MRO Review":
                    return 6;
                case "Negative":
                    return 7;
                default:
                    return 0; 
            }
        }

        private void DefineColumnIndex()
        {
            if (_nameIndex != 0 && _resultIndex != 0) return;
            foreach (var ele in Driver.FindElements(By.XPath("//*[@id=\"results-table\"]/thead/tr[2]/*")))
            {
                if (ele.GetAttribute("class").Contains("doner-name"))
                    _nameIndex = _resultIndex;
                else if (ele.GetAttribute("class").Contains("result-status"))
                    break;
                _resultIndex++;
            }
        }
    }
}