using System;
using System.Collections;
using OnboardLocal.Model;
using OpenQA.Selenium;

namespace OnboardLocal.Controller
{
    public interface IWebService
    {
        IWebDriver Driver { get; set; }
        void Setup();
        Person Search(Person person);
        bool Login();
    }

    public class LoginException : Exception
    {
        public LoginException(string message) : base(message) { }

    }
}