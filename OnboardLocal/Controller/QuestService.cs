using System;
using OnboardLocal.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace OnboardLocal.Controller
{
    public class QuestService : IWebService
    {
        private IWebDriver _driver;
        private string _user;
        private string _pass;
        private WebDriverWait _wait;
        
        private const string NewTestUrl = "https://esp.employersolutions.com/ImportOrder/Index";
        private const string SearchUrl = "https://esp.employersolutions.com/Results/Summary";

        public QuestService(IWebDriver driver, string user, string pass)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _user = user;
            _pass = pass;
        }

        public void Setup()
        {
            throw new System.NotImplementedException();
        }

        public string Search(string searchTerm)
        {
            throw new System.NotImplementedException();
        }

        public bool Login()
        {
            throw new System.NotImplementedException();
        }
    }
}