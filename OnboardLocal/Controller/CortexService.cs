using System;
using System.Collections;
using System.Collections.Generic;
using OnboardLocal.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace OnboardLocal.Controller
{
    public class CortexService : IWebService
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private string _user;
        private string _pass;
        
        private const string Url = "https://logistics.amazon.com/account-management/delivery-associates?providerType=DA&email=&searchStart=0&searchSize=100";

        public CortexService(IWebDriver driver, string user, string pass)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _user = user;
            _pass = pass;
        }
    

        public void Setup()
        {
            _driver.Url = Url;
            _wait.Until(driver => driver.FindElement(By.XPath("")));
        }

        public string Search(string searchTerm)
        {
            throw new System.NotImplementedException();
        }

        public bool Login()
        {
            throw new System.NotImplementedException();
        }

        public string GetPhoneNumber()
        {
            throw new System.NotImplementedException();
        }

        public string GetEmail()
        {
            throw new System.NotImplementedException();
        }

        public string GetStatus()
        {
            throw new System.NotImplementedException();
        }
    }
}