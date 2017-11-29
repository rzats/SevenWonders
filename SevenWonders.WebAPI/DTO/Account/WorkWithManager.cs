using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Account.Interfaces;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SevenWonders.WebAPI.DTO.Account
{
    public class WorkWithManager : WorkWithAutorizedPerson<Manager>
    {
        public override IEnumerable<IAuthorizedPerson> FindPersons(SevenWondersContext db, SearchViewModel search)
        {
            return base.FindPersons(db, search);
        }

        private void AddCountriesForManager(SevenWondersContext db, Manager manager, int[] countries)
        {

            if (manager.Countries != null)
            {
                foreach (var country in manager.Countries.ToList())
                {
                    country.ManagerId = null;
                    db.Entry(country).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            if (countries != null)
            {
                var length = countries.Length;
                for (int i = 0; i < length; i++)
                {
                    var country = db.Coutries.Find(countries[i]);
                    country.ManagerId = manager.Id;
                    db.Entry(country).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        public void AddFullManager(SevenWondersContext db, FullManagerViewModel user, int[] countries)
        {
            if (db.Users.Any(x => x.Email == user.Email))
            {
                throw new OverflowException("user with this email is already exist");
            }
            Utils utils = new Utils();
            string password = utils.GetEncodedHash(user.Password, Security.solt);
            db.Users.Add(new User() { Email = user.Email, Password = password, RoleId = 3 });
            Manager manager = new Manager() { DateOfBirth = user.DateOfBirth.ToUniversalTime(), FirstName = user.FirstName, LastName = user.LastName, PhoneNumber = user.PhoneNumber, Email = user.Email, IsDeleted = false };
            db.Managers.Add(manager);
            db.SaveChanges();
            AddCountriesForManager(db, manager, countries);
        }

        public void EditFullManager(SevenWondersContext db, FullManagerViewModel user, int[] countries)
        {

            Manager manager = db.Managers.Find(user.Id);
            var userFromDb = db.Users.FirstOrDefault(x => x.Email == manager.Email);
            manager.LastName = user.LastName;
            manager.FirstName = user.FirstName;
            manager.Email = user.Email;
            manager.PhoneNumber = user.PhoneNumber;
            manager.DateOfBirth = user.DateOfBirth;
            db.Entry(manager).State = EntityState.Modified;
            db.SaveChanges();

            Utils utils = new Utils();
            if (user.Password != userFromDb.Password)
            {
                userFromDb.Password = utils.GetEncodedHash(user.Password, Security.solt);
            }
            userFromDb.Email = user.Email;
            AddCountriesForManager(db, manager, countries);
            db.Entry(userFromDb).State = EntityState.Modified;
            db.SaveChanges();
        }

        public FullManagerViewModel GetFullManager(SevenWondersContext db, int Id)
        {
            var manager = db.Managers.Find(Id);
            var usermanager = db.Users.FirstOrDefault(x => x.Email == manager.Email);
            var fullmanager = new FullManagerViewModel(manager.Id, manager.Email, usermanager.Password, manager.FirstName, manager.LastName, manager.PhoneNumber, manager.DateOfBirth);
            return fullmanager;
        }

    }
}