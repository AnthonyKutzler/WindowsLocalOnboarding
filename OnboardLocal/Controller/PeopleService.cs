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
            var people = new PeopleProvider().GetPeople();
            //x = people filter BG="Passed"
            //y = opposite of ^^
            //z = x filter DT = "Expired" || "Pos"
            //x = Opposite of ^^
            //x.Sort
            //z.Sort
            //y.SortReverse
            
            //people = x+z+y
            //return people;
            
            return people.OrderBy(x => x.Background).ThenBy(x => x.Drug).ThenBy(x => x.Lastname);
        }
    }
}