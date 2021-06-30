using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OnboardLocal.Model;

namespace OnboardLocal.Controller
{
    public class PeopleService
    {

        public IEnumerable<Person> GetPeopleSorted()
        {
            var people = new PeopleProvider().GetPeople().ToList();
            var a = people.Where(x => x.Background == "Passed").ToList();
            var b = people.Where(x => x.Background != "Passed").ToList();
            var c = a.Where(x => x.Drug == "Expired" || x.Drug == "Positive").ToList();
            var d = b.Where(x => x.Drug == "Expired" || x.Drug == "Positive").ToList();
            b = b.Where(x => x.Drug != "Expired" && x.Drug != "Positive").ToList();
            a = a.Where(x => x.Drug != "Expired" && x.Drug != "Positive").ToList();
            a = a.OrderBy(x => x.Drug).ToList();
            c = c.OrderBy(x => x.Drug).ToList();
            b = b.OrderByDescending(x => x.Background).ToList();
            d = d.OrderBy(x => x.Drug).ToList();
            people.Clear();
            people.AddRange(a);
            people.AddRange(c);
            people.AddRange(b);
            people.AddRange(d);
            return people;
        }
    }
}