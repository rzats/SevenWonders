using SevenWonders.Models;
using SevenWonders.Interfaces;
using SevenWonders.ViewModels;
using System.Collections.Generic;
using SevenWonders.WebAPI.Models;
using SevenWonders.DAL.Context;

namespace SevenWonders.Models
{
    public class WorkWithCustomer : WorkWithAutorizedPerson<Customer>
    {
        public override IEnumerable<IAuthorizedPerson> FindPersons(SevenWondersContext db, SearchViewModel search)
        {
            return base.FindPersons(db, search);
        }
    }
}