using Newtonsoft.Json.Linq;
using SevenWonders.Models;
using SevenWonders.ViewModels;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SevenWonders.Controllers
{
    public class ManagersManagementController : ApiController
    {
        SevenWondersEntities db = new SevenWondersEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/<controller>
        [HttpGet]
        public IEnumerable<Interfaces.IAuthorizedPerson> GetManagers()
        {
            try
            {
                WorkWithManager workWithManager = new WorkWithManager();
                var result = workWithManager.FindPersons(db, new SearchViewModel()).AsEnumerable().ToList();
                return result;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return null;
            }
        }

        [HttpPost]
        public void AddManager([FromBody]JObject model)
        {
            var manager = model["manager"].ToObject<FullManagerViewModel>();
            var countries = model["countries"].ToObject<int[]>();
            if (ModelState.IsValid)
            {
                var utils = new Utils();
                var workWithManager = new WorkWithManager();
                if (manager.Id == 0)
                {
                    if (db.Users.Any(x => x.Email == manager.Email))
                    {
                        ModelState.AddModelError("", "Manager with this email already exists");
                        var _countries = db.Countries.Where(x => (x.ManagerId == null || x.Manager.IsDeleted) && !x.IsDeleted).ToList();
                        var ctr = db.Countries.ToList();
                    }
                    else
                    {
                        workWithManager.AddFullManager(db, manager, countries);
                    }
                }
                if (manager.Id != 0)
                {
                    workWithManager.EditFullManager(db, manager, countries);
                }
            }
        }

        [HttpPost]
        public IEnumerable<Interfaces.IAuthorizedPerson> SearchManagers(SearchViewModel search)
        {
            try
            {
                WorkWithManager workWithManager = new WorkWithManager();
                return workWithManager.FindPersons(db, search).AsEnumerable().ToList();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return null;
            }
        }
    }
}
