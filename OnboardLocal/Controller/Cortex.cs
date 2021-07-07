using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using OnboardLocal.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace OnboardLocal.Controller
{
    public class Cortex : IWebService
    {
        public IWebDriver Driver { get; set; }
        private WebDriverWait _wait;
        private string _user;
        private string _pass;
        private int _loginAttempts = 0;
        private const string EmailXpath = "//*[@id=\"email-input\"]";
        private const string NameXpath = "//*[@id=\"name-input\"]";
        private const string TBody = "//*[@id=\"dsp-onboarding\"]/div/main/div[3]/div[2]/div[1]/table/tbody";
        //*[@id="dsp-onboarding"]/div/main/div[3]/div[2]/div[1]/table/tbody
        private const string ExpanderXpath = "//*[text()='Onboarding']";
        
        //TODO: Test this
        private const string AssociateXpath = "//*[text()='Associate Settings']/../[text()='Edit']";
        private const string DrugTestXpath = "//*[text()='Drug Test']/../[text()='Edit']";
        private const string BackgroundXpath = "//*[text()='Background Check']/..";
        

        private const string Url =
            "https://logistics.amazon.com/account-management/delivery-associates?providerType=DA&email=&searchStart=0&searchSize=100";


        public Cortex(string user, string pass, IWebDriver driver)
        {
            Driver = driver;
            _wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
            _user = user;
            _pass = pass;

        }




        public void Setup()
        {
            Driver.Url = Url;
            _wait.Until(driver => driver.FindElement(By.TagName("tbody")));
        }

        public Person Search(Person person)
        {
            try
            {
                try
                {
                    Search(EmailXpath, person.Email);
                    _wait.Until(driver => driver.FindElement(By.TagName("tbody")));
                }
                catch (NoSuchElementException)
                {
                    Search(NameXpath, person.Lastname);
                    _wait.Until(driver => driver.FindElement(By.TagName("tbody")));
                }

                var rows = Driver.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                if (rows.Count < 1)
                {
                    try
                    {
                        return Search(FindEmailFromNames(person));
                    }
                    catch (ArgumentException)
                    {
                        NewOnboard(person);
                        person.Background = "";
                        return person;
                    }
                    //return person;
                }
                //else click the first name
                rows[0].FindElement(By.XPath("./td[1]/a")).Click();
                Driver.Close();
                Driver.SwitchTo().Window(Driver.WindowHandles[0]);
                _wait.Until(driver => driver.FindElement(By.XPath(ExpanderXpath)));
                return GatherInfo(person);
            }
            catch (NoSuchElementException)
            {
            }
            return person;
        }

        private void Search(string field, string value)
        {
            Driver.FindElement(By.XPath(field)).SendKeys(value);
            Driver.FindElement(By.XPath("//*[text()='Search']")).Click();
        }

        public bool Login()
        {
            Driver.Url = Url;
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(7));
                wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"ap_email\"]")));
                Driver.FindElement(By.XPath("//*[@id=\"ap_email\"]")).SendKeys(_user);
                Driver.FindElement(By.XPath("//*[@id=\"ap_password\"]")).SendKeys(_pass);
                Driver.FindElement(By.XPath("//*[@id=\"signInSubmit\"]")).Click();
                wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"ap_email\"]")));
            }
            catch (WebDriverTimeoutException)
            {
                return true;
            }
            catch (NoSuchElementException)
            {
                return true;
            }

            try
            {
                Driver.FindElement(By.XPath("//*[@id=\"auth-captcha-image\"]"));
                Driver.FindElement(By.XPath("//*[@id=\"ap_password\"]")).SendKeys(_pass);
                Thread.Sleep(30000);
                Driver.FindElement(By.XPath("//*[@id=\"ap_email\"]"));
            }
            catch (NoSuchElementException)
            {
                return true;
            }
            

            //Look for Captcha, if found notify event handler of captcha issues.
            /*try
            {
                Driver.FindElement(By.XPath(""));
            }
            catch (NoSuchElementException)
            {
                
            }
*/
            _loginAttempts++;
            return false;
            //throw new System.NotImplementedException();
        }
        

        private Person FindEmailFromNames(Person person)
        {
            try
            {
                Search(EmailXpath, person.Email);
                _wait.Until(driver => driver.FindElement(By.TagName("tbody")));
                //var rows = Driver.FindElements(By.XPath($"{TBody}/*"));
                string ele = null;
                var prog = 0;
                do
                {
                    var rows = Driver.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                    //var rows = Driver.FindElements(By.XPath($"{TBody}/*"));
                    foreach (var row in rows)
                    {
                        var name = row.FindElement(By.XPath("./td[1]/a")).Text;
                        var progress = int.Parse(row.FindElement(By.XPath("./td[6]")).Text.Split('/')[0]);
                        if (name.Contains(person.FirstName) && name.Contains(person.Lastname) && progress > prog)
                        {
                            prog = progress;
                            ele = row.FindElement(By.XPath("./td[4]")).Text;
                        }
                    }
                } while (Driver.FindElement(By.XPath("//*[@id=\"dsp-onboarding\"]/div/main/div[3]/div[2]/div[2]/div/div/button[2]")).Enabled);

                if (ele == null)
                    throw new ArgumentNullException();
                
                person.Email = ele;
                return person;
            }
            catch (NoSuchElementException)
            {
                throw new ArgumentNullException();
            }
        }

        private Person GatherInfo(Person person)
        {
            try
            {
                Driver.FindElement(By.XPath("//*[text()='OFFBOARDED']"));
                person.Background = "OFFBOARDED";
                return person;
            }
            catch (NoSuchElementException)
            {
                Driver.FindElement(By.XPath(ExpanderXpath)).Click();
                _wait.Until(driver => driver.FindElement(By.XPath(BackgroundXpath)));
                var phone = Driver.FindElement(By.XPath("//*[text()='Mobile:']/../span")).Text;
                person.Phone = phone.Contains("not") ? person.Phone : phone;
                var classes = Driver.FindElement(By.XPath(BackgroundXpath)).GetAttribute("class");
                if (classes.Contains("completed"))
                    person.Background = "Passed";
                else if (classes.Contains("in-progress"))
                    person.Background = "Pending";
                else if (classes.Contains("error"))
                    person.Background = "Failed";
                else
                {
                    if (Driver.FindElement(By.XPath("//*[text()='Associate Settings']/..")).GetAttribute("class")
                        .Contains("completed"))
                        person.Background = "Not Started";
                    else
                    {
                        UpdateAssociateSettings();
                        person.Background = "Badge Updated";
                    }
                }
                return person;
            }
        }

        public void NewOnboard(Person person)
        {
            try
            {
                Login();
                Driver.Url = Url;
                _wait.Until(driver => driver.FindElement(By.TagName("tbody")));
                Driver.FindElement(By.XPath("//*[text()='Add a Delivery Associate']")).Click();
                _wait.Until(driver => driver.FindElement(By.XPath("//*[text()='Send']")));
                Driver.FindElement(By.XPath("//*[@id=\"root_FIRST_NAME\"]")).SendKeys(person.FirstName);
                Driver.FindElement(By.XPath("//*[@id=\"root_LAST_NAME\"]")).SendKeys(person.Lastname);
                Driver.FindElement(By.XPath("//*[@id=\"root_EMAIL\"]")).SendKeys(person.Email);
                Driver.FindElement(By.XPath("//*[text()='Send']")).Click();
                _wait.Until(driver => driver.FindElement(By.XPath(ExpanderXpath)));
                Driver.FindElement(By.XPath(ExpanderXpath)).Click();
                UpdateAssociateSettings();
            }
            catch (Exception)
            {
                throw new NoSuchElementException("Email already Onboarded");
            }
        }

        private void UpdateAssociateSettings()
        {
            try
            {
                var inputSim = new InputSimulator();
                Driver.FindElement(By.XPath("//*[text()='Associate Settings']/../div/h3/a")).Click();
                _wait.Until(driver =>
                    driver.FindElement(
                        By.XPath(
                            "//*[@id=\"dsp-onboarding\"]/div/main/div/span[3]/div/div/div[2]/div[1]/div/div/span")));
                //Driver.FindElement(By.XPath(AssociateXpath)).Click();
                Driver.FindElement(
                        By.XPath("//*[@id=\"dsp-onboarding\"]/div/main/div/span[3]/div/div/div[2]/div[1]/div/div/span"))
                    .Click();
                inputSim.Keyboard.TextEntry(Properties.Settings.Default.StationCode);
                inputSim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                Driver.FindElement(
                    By.XPath("//*[@id=\"dsp-onboarding\"]/div/main/div/span[3]/div/div/div[2]/div[2]/div/div")).Click();
                inputSim.Keyboard.TextEntry("Amazon Logistics");
                inputSim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                Driver.FindElement(By.XPath("//*[@id=\"supervisor-alias\"]"))
                    .SendKeys(Properties.Settings.Default.BadgeId);
                Driver.FindElement(By.XPath("//*[text()='Confirm']"));
            }
            catch (NoSuchElementException ex)
            {
                Console.Write(ex.Message);
            }
        }

        public void UpdateDrugTests(IEnumerable<Person> people)
        {
            foreach (var person in people)
            {
                Search(person);
                Driver.FindElement(By.XPath("//*[text()='Drug Test']/../div/h3/a")).Click();
                _wait.Until(driver => driver.FindElement(By.XPath("//*[text()='Yes']")));
                Driver.FindElement(By.XPath("//*[text()='Yes']/../input")).Click();
                Driver.FindElement(By.XPath("//*[text()='Confirm']")).Click();
            }
            //TODO: Test
        }
    }

}