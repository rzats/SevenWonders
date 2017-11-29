using Microsoft.Owin.Security;
using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Account;
using SevenWonders.WebAPI.Models;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SevenWonders.WebAPI.Controllers
{
    public class CustomerCabinetController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetCurrentCustomer()
        {
            return Ok(GetCustomer(User.Identity.Name));
        }

        [HttpPost]
        [Authorize(Roles = "customer")]
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

        [HttpGet]
        public IHttpActionResult IsEmailValid(string email)
        {
            var currentCustomer = GetCustomer(User.Identity.Name);
            bool contain = db.Customers.Any(x => x.Email == email && email != currentCustomer.Email);
            return Ok(!contain);
        }

        private Customer GetCustomer(string email)
        {
            return db.Customers.FirstOrDefault(x => x.Email == email && x.IsDeleted == false);
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
