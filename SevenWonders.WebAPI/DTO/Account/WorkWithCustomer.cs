using System.Collections.Generic;
using SevenWonders.WebAPI.Models;
using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Account.Interfaces;

namespace SevenWonders.WebAPI.DTO.Account
{
    public class WorkWithCustomer : WorkWithAutorizedPerson<Customer>
    {
        public override IEnumerable<IAuthorizedPerson> FindPersons(SevenWondersContext db, SearchViewModel search)
        {
            return base.FindPersons(db, search);
        }
    }
}