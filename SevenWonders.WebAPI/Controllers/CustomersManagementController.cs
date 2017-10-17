using Newtonsoft.Json.Linq;
using SevenWonders.WebAPI.Models;
using SevenWonders.Models;
using SevenWonders.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace SevenWonders.Controllers
{
    public class CustomersManagementController : ApiController
    {
        SevenWondersEntities db = new SevenWondersEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CustomersManagementController()
        {

        }

        public CustomersManagementController(SevenWondersEntities context)
        {
            db = context;
        }

        [HttpGet]
        public IEnumerable<Interfaces.IAuthorizedPerson> GetCustomers()
        {
            WorkWithCustomer workWithCustomer = new WorkWithCustomer();
            return workWithCustomer.FindPersons(db, new SearchViewModel()).AsEnumerable().ToList();
        }

        [HttpGet]
        public IEnumerable<Tour> GetCustomerTours(int id)
        {
            var result = db.Tours.Include(x => x.Customer).Include(x => x.Reservation).Where(x => x.CustomerId == id).AsEnumerable().ToList();
            return result;
        }

        [HttpPost]
        public IHttpActionResult ChangeCustomerStatus(int id)
        {
            var workWithCustomer = new WorkWithCustomer();
            workWithCustomer.ChangePersonStatus(db, id);
            return Ok();
        }

        [HttpPost]
        public IEnumerable<Interfaces.IAuthorizedPerson> Customers(SearchViewModel search)
        {
            WorkWithCustomer workWithCustomer = new WorkWithCustomer();
            return workWithCustomer.FindPersons(db, search).AsEnumerable().ToList();
        }
    }
}