using Microsoft.Owin.Security;
using SevenWonders.DAL.Context;
using SevenWonders.Interfaces;
using SevenWonders.Models;
using SevenWonders.WebAPI.Models;
using SevenWonders.WebAPI.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System;
using Microsoft.Owin.Host.SystemWeb;


namespace SevenWonders.WebAPI.Controllers
{
    public class AccountController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpPost]
        public IHttpActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Utils utils = new Utils();
                var user = GetPersonByEmail<User>(model.Email);

                if (user != null)
                {
                    return BadRequest("user with such email already exists");
                }

                user = new User() { Email = model.Email, Password = utils.GetEncodedHash(model.Password, Security.solt), Role = db.Roles.Find(1) };
                var customer = new Customer() { Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, DateOfBirth = model.DateOfBirth, PhoneNumber = model.PhoneNumber };
                db.Users.Add(user);
                db.Customers.Add(customer);
                db.SaveChanges();
                utils.AddUserInCookies(user, AuthenticationManager);
                return Ok();
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public IHttpActionResult Login(LoginModel model)
        {
                if (ModelState.IsValid)
                {
                    Utils utils = new Utils();
                    var hashPassword = utils.GetEncodedHash(model.Password, Security.solt);
                    User user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == model.Email && u.Password == hashPassword);
                    if (user != null)
                    {
                        if (user.Role.Name == Enum.GetName(typeof(Security.RoleType), 3))
                        {
                            Manager manager = GetPersonByEmail<Manager>(user.Email);
                            if (manager.IsDeleted)
                            {
                                return Ok();
                            }
                        }
                        if (user.Role.Name == Enum.GetName(typeof(Security.RoleType), 1))
                        {
                            Customer customer = GetPersonByEmail<Customer>(user.Email);
                            if (customer.IsDeleted)
                            {
                                return Ok();
                            }
                        }
                        if (user.Role.Name == Enum.GetName(typeof(Security.RoleType), 2))
                        {
                            Administrator customer = GetPersonByEmail<Administrator>(user.Email);
                            if (customer.IsDeleted)
                            {
                                return Ok();
                            }
                        }
                    }
                    if (user == null)
                    {
                        return BadRequest("Wrong login or password.");
                    }
                    utils.AddUserInCookies(user, AuthenticationManager);
                }
                return Ok();
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
