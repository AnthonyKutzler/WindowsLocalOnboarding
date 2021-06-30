using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
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
        

        public Quest(string user, string pass, IWebDriver driver)
        {
            Driver = driver;
            _wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(7));
            _user = user;
            _pass = pass;
        }

        public void Setup()
        {
            //Driver.Url = SearchUrl;
            //_wait.Until(driver => driver.FindElement(By.XPath(BodyXpath)));
        }
        public Person Search(Person person)
        {
            var e = Driver.FindElement(By.XPath("//*[@id=\"search-boxes\"]/div/input[1]"));
            e.Clear();    
            e.SendKeys(Regex.Replace(person.Phone, @"[\D+]", ""));
            Driver.FindElement(By.XPath("//*[@id=\"search-boxes\"]/div/input[14]")).Click();
            _wait.Until(driver => driver.FindElement(By.XPath(BodyXpath)));
            Thread.Sleep(3000);
            var oldValue = 0;
            try
            {
                var elements = Driver.FindElements(By.XPath($"{BodyXpath}/*"));
                if (elements.Count < 1) throw new NoSuchElementException();
                foreach (var ele in elements)
                {
                    var name = FormatName(ele.FindElement(By.XPath($"./td[{_nameIndex}]")).Text).ToLower();
                    //If not person in question, continue.
                    if (!name.Contains(FormatName(person.FirstName).ToLower()) ||
                        !name.Contains(FormatName(person.Lastname).ToLower())) throw new NoSuchElementException();
                    var result = ele.FindElement(By.XPath($"./td[{_resultIndex}]")).Text;
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
                Driver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/div/form/fieldset/button")).Click();
                _wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"UserName\"]")));
                return false;
            }
            catch (NoSuchElementException)
            {
                DefineColumnIndex();
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                DefineColumnIndex();
                return true;
            }
        }

        public void SetupNewTests()
        {
            var csv = Path.Combine(Environment.CurrentDirectory, "data") + @"\drug.csv";
            if (!Login()) return;
            try
            {
                string[] line = {"Primary ID", "First Name", "Last Name", "Primary Phone", "Date of Birth", "Account Number", "Modality",
                    "Client Site Location", "Order Code(s)", "Collection Type", "Reason for Test", "Order Expiration Date",
                    "Order Expiration Time", "Collection Site Code", "Observed", "Email(s)"} ;
                
                    
                    //"Primary ID,First Name,Last Name,Primary Phone,Date of Birth,Account Number,Modality,Client Site Location,Order Code(s),Collection Type,Reason for Test,Order Expiration Date,Order Expiration Time,Collection Site Code,Observed,Email(s)";}
                new CsvService<DrugTest>().CreateCsvFile(line, _newTests, csv);

                if (!Login() || _newTests.Count < 1) return;
                Driver.Url = NewTestUrl;
                Driver.FindElement(
                    By.XPath("/html/body/div[2]/div[2]/div[2]/div[1]/div[3]/form/div[2]/div[1]/div/input[2]")).Click();
                Driver.FindElement(By.XPath("//*[@id=\"ImportFileName\"]")).SendKeys(csv);
                Driver.FindElement(By.XPath("//*[@id=\"ui-id-4\"]/input[2]")).Click();
                Driver.FindElement(By.XPath("//*[@id=\"import-order-form\"]/div[2]/div[4]/div/input")).Click();
                Driver.FindElement(By.XPath("//*[@id=\"Import\"]")).Click();
                File.Delete(csv);
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                /* ignored*/
            }
        }

        private static string FormatName(string name)
        {
            return Regex.Replace(name, "[^a-zA-Z]+", " ");
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
                if (ele.GetAttribute("class").Contains("donor-name"))
                    _nameIndex = _resultIndex+1;
                else if (ele.GetAttribute("class").Contains("result-status"))
                    break;
                _resultIndex++;
            }
            _resultIndex++;
        }
    }
}