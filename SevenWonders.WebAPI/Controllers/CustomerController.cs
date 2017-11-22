using Microsoft.Owin.Security;
using SevenWonders.DAL.Context;
using SevenWonders.Interfaces;
using SevenWonders.Models;
using SevenWonders.WebAPI.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System;
using Microsoft.Owin.Host.SystemWeb;
using System.Net;

namespace SevenWonders.WebAPI.Controllers
{
    public class CustomerController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetCurrentCustomer()
        {
            return Ok(GetCustomer(User.Identity.Name));
        }

        [HttpPost]
        public IHttpActionResult EditCustomer(Customer editCustomer)
        {
            if (ModelState.IsValid)
            {
                var customer = db.Customers.Find(GetCustomer(User.Identity.Name).Id);
                User user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == customer.Email);

                if (user != null && User.Identity.Name != editCustomer.Email)
                {
                    ModelState.AddModelError("", "user with such email already exists");
                    return BadRequest(ModelState);
                }
                customer.FirstName = editCustomer.FirstName;
                customer.LastName = editCustomer.LastName;
                customer.PhoneNumber = editCustomer.PhoneNumber;
                customer.DateOfBirth = editCustomer.DateOfBirth;
                customer.Email = editCustomer.Email;
                db.Entry(customer).State = EntityState.Modified;
                user.Email = editCustomer.Email;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                Utils utils = new Utils();
                utils.AddUserInCookies(user, AuthenticationManager);
                return Ok();
            }

            return BadRequest(ModelState);
        }

        private Customer GetCustomer(string email)
        {
            return db.Customers.FirstOrDefault(x => x.Email == email && x.IsDeleted == false);
        }

        private bool IsCurrentUser(string email)
        {
            bool IsCurrentUser = false;
            if (User.Identity.Name == email)
            {
                IsCurrentUser = true;
            }
            return IsCurrentUser;
        }

        private T GetPersonByEmail<T>(string email) where T : class, IPerson
        {
            DbSet<T> dbSet = db.Set<T>();
            return dbSet.FirstOrDefault(x => x.Email == email);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
    }
}
