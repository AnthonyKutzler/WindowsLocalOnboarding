using System;
using OnboardLocal.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace OnboardLocal.Controller
{
    public class Quest : IWebService
    {
        public IWebDriver Driver { get; set; }
        private string _user;
        private string _pass;
        private WebDriverWait _wait;
        
        private const string NewTestUrl = "https://esp.employersolutions.com/ImportOrder/Index";
        private const string SearchUrl = "https://esp.employersolutions.com/Results/Summary";

        public Quest(string user, string pass, IWebDriver driver)
        {
            Driver = driver;
            _wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
            _user = user;
            _pass = pass;
        }

        public void Setup()
        {
            throw new System.NotImplementedException();
        }

        public Person Search(Person person)
        {
            throw new System.NotImplementedException();
        }

        public bool Login()
        {
            try
            {
                Driver.Url = SearchUrl;
                Driver.FindElement(By.XPath("//*[@id=\"UserName\"]")).SendKeys(_user);
                Driver.FindElement(By.XPath("//*[@id=\"Password\"]")).SendKeys(_pass);
                Driver.FindElement(By.XPath("//*[text()='Secure sign in']")).Click();
                _wait.Until(driver => driver.FindElement(By.XPath("//*[text()='DASHBOARD'")));
                return true;
            }
            catch (NoSuchElementException e)
            {
                return false;
            }
        }
    }
}