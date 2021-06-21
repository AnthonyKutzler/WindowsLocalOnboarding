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
        
        public void RunApplication(MainWindow window)
        {
            //TODO Try/ Catch on View to display error, move away from excel and use application to view results, with sqlite
            var excel = new ExcelService(window.FilePath.Text);
            _all = excel.GetPeople();
            FilterBackgrounds();
            var driver = StartDriver();
            //Check for backgrounds
            _all = _all.Concat(UpdatePeople(_filter, new Cortex(window.AmzEmail.Text, window.AmzPassword.Password, driver)));
            FilterDrugScreens(window.DrugScreen);
            //check for drug tests
            _all = _all.Concat(UpdatePeople(_filter, new Quest(window.QuestUsername.Text, window.QuestPassword.Password, driver)));
            FilterNewDrugTests(window.DrugScreen);
            //setup new drug tests
            _all = _all.Concat(new Quest(window.QuestUsername.Text, window.QuestPassword.Password, driver).SetupNewTests(_filter));
            excel.Save(_all);
            
        }

        private static IEnumerable<Person> UpdatePeople(IEnumerable<Person> people, IWebService service)
        {
            while (service.Login())
            {
                var people1 = (Person[]) people;
                for (var i = 0; i < people1.Length; i++)
                {
                    service.Setup();
                    people1[i] = service.Search(people1[i]);
                }
            }
            return people;
        }

        private static IWebDriver StartDriver()
        {
            var options = new ChromeOptions();
            options.AddArguments("start-maximized");
            options.AddArguments("enable-automation");
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-infobars");
            options.AddArguments("--disable-dev-shm-usage");
            options.AddArguments("--disable-browser-side-navigation");
            options.AddArguments("--disable-gpu");
            options.AddArgument($@"user-data-dir=${Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}AppData\Local\Google\Chrome\User Data\Default");
            return new ChromeDriver($@"${System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)}chromedriver.exe",options);
        }

        private void FilterBackgrounds()
        {
            _filter = _all.Where(person => person.Background == "Pending" || person.Background == "");
            _all = _all.Where(person => person.Background != "Pending" && person.Background != "");
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
            _filter = _all.Where(person => person.Background == backgroundText && (person.Drug != "Positive" || person.Drug != "Negative" || person.Drug != "Expired"));
            _all = _all.Where(person => !(person.Background == backgroundText && (person.Drug != "Positive" || person.Drug != "Negative" || person.Drug != "Expired")));
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