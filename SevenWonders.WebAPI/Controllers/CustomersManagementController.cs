using Newtonsoft.Json.Linq;
using SevenWonders.Models;
using SevenWonders.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SevenWonders.DAL.Context;

namespace SevenWonders.Controllers
{
    public class CustomersManagementController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IEnumerable<Interfaces.IAuthorizedPerson> GetCustomers()
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
        public IHttpActionResult ChangeCustomerStatus(int id)
        {
            var workWithCustomer = new WorkWithCustomer();
            workWithCustomer.ChangePersonStatus(db, id);
            return Ok();
        }
    }
}