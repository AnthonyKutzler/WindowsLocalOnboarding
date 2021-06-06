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
        public string RunApplication(string amzUsername, string amzPassword, string questUsername, string questPassword, string excelPath)
        {
            IEnumerable<Person> all = new ExcelService(excelPath).GetPeople();
            //Filter Backgrounds
            var filter = all.Where(person => person.Background == "Pending" || person.Background == "");
            all = all.Where(person => person.Background != "Pending" && person.Background != "");
            IWebDriver driver = StartDriver();
            //Combine lists
            all = all.Concat(UpdatePeople(filter, new CortexService(driver, amzUsername, amzPassword)));
            //Filter Drug Tests
            filter = all.Where(person => person.Background == "Passed" && (person.Drug != "Positive" || person.Drug != "Negative" || person.Drug != "Expired"));
            all = all.Where(person => !(person.Background == "Passed" && (person.Drug != "Positive" || person.Drug != "Negative" || person.Drug != "Expired")));
            //Combine Again
            all = all.Concat(UpdatePeople(filter, new QuestService(driver, questUsername, questPassword)));
            
            return "Success";
        }

        private IEnumerable<Person> UpdatePeople(IEnumerable<Person> people, IWebService service)
        {
            while (service.Login())
            {
                foreach (var person in (Person[])people)
                {
                    service.Setup();
                    var updateText = service.Search((service is CortexService ? person.Email : Regex.Replace(person.Phone, "[\\D+]", "")));
                    if ((service is CortexService cortexService))
                    {
                        person.Background = updateText;
                        person.Email = cortexService.GetEmail();
                        person.Phone = cortexService.GetPhoneNumber();
                    }
                    else
                    {
                        person.Drug = updateText;
                    }
                }
            }
            return people;
        }

        private IWebDriver StartDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("start-maximized");
            options.AddArguments("enable-automation");
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-infobars");
            options.AddArguments("--disable-dev-shm-usage");
            options.AddArguments("--disable-browser-side-navigation");
            options.AddArguments("--disable-gpu");
            options.AddArgument(@$"user-data-dir=${Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}AppData\Local\Google\Chrome\User Data\Default");
            return new ChromeDriver(@$"${System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)}chromedriver.exe",options);
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