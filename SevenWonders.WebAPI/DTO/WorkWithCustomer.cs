using SevenWonders.Models;
using SevenWonders.Interfaces;
using SevenWonders.ViewModels;
using System.Collections.Generic;
using SevenWonders.DAL.Context;
using SevenWonders.WebAPI;

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