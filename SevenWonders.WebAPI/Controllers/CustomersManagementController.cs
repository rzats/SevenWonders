using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Account.Interfaces;
using SevenWonders.WebAPI.DTO.Account;

namespace SevenWonders.WebAPI.Controllers
{
    public class CustomersManagementController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IEnumerable<IAuthorizedPerson> GetCustomers()
        {
            WorkWithCustomer workWithCustomer = new WorkWithCustomer();
            return workWithCustomer.FindPersons(db, new SearchViewModel()).AsEnumerable().ToList();
        }

        [HttpPost]
        public IHttpActionResult GetContries()
        {
            WorkWithCustomer workWithCustomer = new WorkWithCustomer();
            var result= workWithCustomer.FindPersons(db, new SearchViewModel()).AsEnumerable().ToList();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IHttpActionResult ChangeCustomerStatus(int id)
        {
            var workWithCustomer = new WorkWithCustomer();
            workWithCustomer.ChangePersonStatus(db, id);
            return Ok();
        }
    }
}