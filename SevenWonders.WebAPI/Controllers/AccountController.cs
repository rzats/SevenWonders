using Microsoft.Owin.Security;
using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.Models;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System;
using SevenWonders.WebAPI.DTO.Account;
using SevenWonders.WebAPI.DTO.Account.Interfaces;

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
                    ModelState.AddModelError("", "user with such email already exists");
                    return BadRequest(ModelState);
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

        [HttpPost]
        public IHttpActionResult LogOut()
        {
            var AuthenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            AuthenticationManager.SignOut();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetUserRole()
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == User.Identity.Name);
                var role = user.Role;
                return Ok(role.Name);
            }
            else return Ok();
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
