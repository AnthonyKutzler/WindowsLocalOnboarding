using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OnboardLocal.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace OnboardLocal.Controller
{
    public class OnboardingService
    {
        private IEnumerable<Person> _all;
        private IEnumerable<Person> _filter;
        private IEnumerable<Person> _filter2;
        
        public void RunApplication(string amzPassword, string questPassword, int drugScreen)
        {
            
            var peopleProvider = new PeopleProvider();
            _all = peopleProvider.GetPeople();
            FilterBackgrounds();
            var driver = StartDriver(Properties.Settings.Default.ChromedriverPath);
            try
            {
                //Check for backgrounds
                var cortex = new Cortex(Properties.Settings.Default.AmzEmail, amzPassword, driver);
                _all = _all.Union(UpdatePeople(_filter, cortex));
                FilterDrugScreens(drugScreen);
                //check for drug tests
                var quest = new Quest(Properties.Settings.Default.QuestUsername, questPassword, driver);
                _all = _all.Union(UpdatePeople(_filter, quest));
                quest.SetupNewTests();
                //if (quest.NegDrug.Count > 0)
                    //cortex.UpdateDrugTests(quest.NegDrug);
            }
            catch (LoginException ex)
            {
                throw new LoginException(ex.Message);
            }

            //setup new drug tests
            peopleProvider.UpdatePeople(_all);
            driver.Close();
        }

        public void NewOnboard(MainWindow window)
        {
            var driver = StartDriver(Properties.Settings.Default.ChromedriverPath);
            try
            {
                var person = new Person(window.First.Text, window.Last.Text, window.Phone.Text, window.Email.Text, "", "");
                new Cortex(window.NewAmzEmail.Text, window.NewAmzPassword.Password, driver).NewOnboard(person);
                driver.Close();
                new PeopleProvider().InsertPerson(person);
            }
            catch (Exception)
            {
                driver.Close();
            }
        }

        private static IEnumerable<Person> UpdatePeople(IEnumerable<Person> people, IWebService service)
        {
            var people1 = people.ToArray();
            if (!service.Login()) return people1;
            for (var i = 0; i < people1.Length; i++)
            {
                service.Setup();
                people1[i] = service.Search(people1[i]);
            }

            return people1;
        }

        private static IWebDriver StartDriver(string path)
        {
            var options = new ChromeOptions();
            options.AddArguments("start-maximized");
            options.AddArguments("enable-automation");
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-infobars");
            options.AddArguments("--disable-dev-shm-usage");
            options.AddArguments("--disable-browser-side-navigation");
            options.AddArguments("--disable-gpu");
            //options.AddArgument($@"user-data-dir=${Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}AppData\Local\Google\Chrome\User Data\Default");
            return new ChromeDriver(path,options);
        }

        private void FilterBackgrounds()
        {
            _filter = _all.Where(person => person.Background == "Pending" || person.Background == "" || person.Background == "Not Started");
            _all = _all.Where(person => !(person.Background == "Pending" || person.Background == "" || person.Background == "Not Started"));
        }

        private void FilterNewDrugTests(int when)
        {
            var backgroundText = "Passed";
            switch (when)
            {
                case 1:
                    backgroundText = "Pending";
                    break;
                case 2:
                    backgroundText = "";
                    break;
                default:
                    backgroundText = "Passed";
                    break;
            }
            _filter = _all.Where(person => person.Background == backgroundText && person.Drug == "");
            _all = _all.Where(person => !(person.Background == backgroundText && person.Drug == ""));

        }

        private void FilterDrugScreens(int when)
        {
            string backgroundText;
            switch (when)
            {
                case 1:
                    backgroundText = "Pending";
                    break;
                case 2:
                    backgroundText = "";
                    break;
                default:
                    backgroundText = "Passed";
                    break;
            }
            _filter = _all.Where(person => person.Background == backgroundText && (person.Drug != "Positive" || person.Drug != "Negative" || person.Drug != "Expired"));
            _all = _all.Where(person => !(person.Background == backgroundText && (person.Drug != "Positive" || person.Drug != "Negative" || person.Drug != "Expired")));
        }

        public static void CheckChromeDriver(string path)
        {
            try
            {
                var driver = StartDriver(path);
                driver.Close();
            }
            catch (Exception)
            {
                throw new Exception("Chromedriver of wrong version");
            }
        }
        
    }
}


/*public void UpdateProperty<TModel,TProperty>(TModel model, Expression<Func<TModel, TProperty>> targetProperty, TProperty value)
{
    var memberSelector = targetProperty as MemberExpression;
    var destinationNotNullable = false;
    PropertyInfo targetAssignmentPropertyInfo = null;
    if(targetProperty.Body.NodeType == ExpressionType.Convert)
    {
        var memberExpr = ((UnaryExpression)destiationMember.Body).Operand as MemberExpression;
        if(memberExpr != null)
        {
            var name = memberExpr.Member.Name;
            targetAssignmentPropertyInfo = target.GetType().GetProperty(name);
            destinationNullable = true;
        }
    }
    
    if(targetAssignemntPropertyInfo != null)
    {
        var currentValue = (T)targetAssignmentPropertyInfo.GetValue(model);
        if(currentValue == null && value == null)
        {
            return;
        }

        if(currentValue == null) 
        {
            targetAssignmentPropertyInfo.SetValue(model, value, null); 
        }
        else if(value == null)
        {
            if(destinationNotNullable)
            {
                targetAssignmentPropertyInfo.SetValue(model, -1, null);
            }
            else
            {
                targetAssignmentPropertyInfo.SetValue(model, null, null);
            }
        }
        else if(!object.Equals(currentValue, value))
        {
            targetAssignmentPropertytInfo.SetValue(model, value, null)
        }
    }
}



UpdateProperty(employee, e => e.LastName, "Dole")
*/