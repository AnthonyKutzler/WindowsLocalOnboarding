using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
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
        private const string ExpanderXpath = "//*[text()='Onboarding']";
        
        //TODO: Test this
        private const string BackgroundXpath = "//*[text()='Background Check']/../..";
        

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
            _wait.Until(driver => driver.FindElement(By.XPath(TBody)));
        }

        public Person Search(Person person)
        {
            try
            {
                try
                {
                    Search(EmailXpath, person.Email);
                    _wait.Until(driver => driver.FindElement(By.XPath(TBody)));
                }
                catch (NoSuchElementException e)
                {
                    Search(NameXpath, person.Lastname);
                    _wait.Until(driver => driver.FindElement(By.XPath(TBody)));
                }

                var rows = Driver.FindElements(By.XPath($"${TBody}/*"));
                if (rows.Count > 1)
                {
                    try
                    {
                        return Search(FindEmailFromNames(rows, person));
                    }
                    catch (ArgumentException e)
                    {
                        person.Background = "Not in Cortex";
                    }
                    return person;
                }
                //else click the first name
                rows[0].FindElement(By.XPath("./td[1]/a")).Click();
                Driver.Close();
                Driver.SwitchTo().Window(Driver.WindowHandles[0]);
                _wait.Until(driver => driver.FindElement(By.XPath(ExpanderXpath)));
                return GatherInfo(person);
            }
            catch (NoSuchElementException e)
            {
                
            }


            throw new System.NotImplementedException();
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
                wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"ap_email\"]")));
            }
            catch (NoSuchElementException e)
            {
                return true;
            }

            //Look for Captcha, if found notify event handler of captcha issues.
            try
            {
                Driver.FindElement(By.XPath(""));
            }
            catch (NoSuchElementException e)
            {
                
            }

            _loginAttempts++;
            return false;
            //throw new System.NotImplementedException();
        }
        

        private Person FindEmailFromNames(ReadOnlyCollection<IWebElement> rows, Person person)
        {
            IWebElement ele = null;
            var prog = 0;
            foreach (var row in rows)
            {
                var name = row.FindElement(By.XPath("./td[1]/a")).Text;
                var progress = int.Parse(row.FindElement(By.XPath("./td[6]")).Text.Split('/')[0]);
                if (name.Contains(person.FirstName) && name.Contains(person.Lastname) && progress > prog)
                {
                    prog = progress;
                    ele = row;
                }
                        
            }

            if (ele == null)
                throw new ArgumentNullException();

            person.Email = ele.FindElement(By.XPath("./td[4]")).Text;
            return person;
        }

        private Person GatherInfo(Person person)
        {
            try
            {
                Driver.FindElement(By.XPath("//*[text()='OFFBOARDED']"));
                person.Background = "OFFBOARDED";
                return person;
            }
            catch (NoSuchElementException e)
            {
                Driver.FindElement(By.XPath(ExpanderXpath)).Click();
                _wait.Until(driver => driver.FindElement(By.XPath(BackgroundXpath)));
                var classes = Driver.FindElement(By.XPath(BackgroundXpath)).GetAttribute("class");
                if (classes.Contains("completed"))
                    person.Background = "Passed";
                else if (classes.Contains("in-progress"))
                    person.Background = "Pending";
                else if (classes.Contains("error"))
                    person.Background = "Failed";
                else
                {
                    if (Driver.FindElement(By.XPath("//*[text()='Associate Settings']/../..")).GetAttribute("class")
                        .Contains("complete"))
                        person.Background = "Not Started";
                    else
                        person.Background = "Associate Settings Not Complete";
                }

                return person;
            }
            
        }
        

    }

}