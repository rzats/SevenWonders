using SevenWonders.Models;
using SevenWonders.Interfaces;
using SevenWonders.ViewModels;
using System.Collections.Generic;
using SevenWonders.WebAPI.Models;

namespace SevenWonders.Models
{
    public class WorkWithCustomer : WorkWithAutorizedPerson<Customer>
    {
        public override IEnumerable<IAuthorizedPerson> FindPersons(SevenWondersEntities db, SearchViewModel search)
        {
            return base.FindPersons(db, search);
        }
    }
}