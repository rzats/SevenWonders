using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace SevenWonders.WebAPI.DTO.Account
{
    public abstract class WorkWithAutorizedPerson<T> where T : class, IAuthorizedPerson
    {
        public virtual IEnumerable<IAuthorizedPerson> FindPersons(SevenWondersContext db, SearchViewModel search)
        {
            DbSet<T> dbSet = db.Set<T>();
            var query = (from person in dbSet
                         select person).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search.FirstName))
                query = query.Where(x => x.FirstName.Contains(search.FirstName));
            if (!string.IsNullOrWhiteSpace(search.LastName))
                query = query.Where(x => x.LastName.Contains(search.LastName));
            if (!string.IsNullOrWhiteSpace(search.PhoneNumber))
                query = query.Where(x => x.PhoneNumber.Contains(search.PhoneNumber));
            if (search.DateOfBirthFrom != null && search.DateOfBirthFrom != DateTime.MinValue)
                query = query.Where(x => x.DateOfBirth >= search.DateOfBirthFrom);
            if (search.DateOfBirthTo != null && search.DateOfBirthTo != DateTime.MinValue)
                query = query.Where(x => x.DateOfBirth <= search.DateOfBirthTo);
            return query;
        }

        public virtual bool ChangePersonStatus(SevenWondersContext db, int id)
        {
            DbSet<T> dbSet = db.Set<T>();
            var result = false;
            var customer = dbSet.Find(id);
            if (customer != null)
            {
                customer.IsDeleted = !customer.IsDeleted;
                db.Entry(customer).State = EntityState.Modified;
                result = true;
                db.SaveChanges();
            }
            return result;
        }

    }
}