using System.Collections;
using OpenQA.Selenium;

namespace OnboardLocal.Controller
{
    public interface IWebService
    {
        void Setup();
        string Search(string searchTerm);
        bool Login();
    }
}