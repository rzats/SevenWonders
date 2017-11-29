using Newtonsoft.Json.Linq;
using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Account;
using SevenWonders.WebAPI.DTO.Managers;
using SevenWonders.WebAPI.DTO.Shared;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SevenWonders.WebAPI.Controllers
{
    public class ManagersManagementController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IHttpActionResult GetManagers()
        {
            try
            {
                WorkWithManager workWithManager = new WorkWithManager();
                var result = workWithManager.FindPersons(db, new SearchViewModel()).AsEnumerable().ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public IHttpActionResult GetManager(int Id)
        {
            var countries = db.Coutries.Where(c => !c.IsDeleted && c.ManagerId==null).ToList();
            var managerCountriesIds = countries.Where(x => x.ManagerId == Id).Select(x => x.Id).ToList();
            WorkWithManager workWithManager = new WorkWithManager();
            var manager = workWithManager.GetFullManager(db, Id);

            var result = convertToManagerEditModel(manager, countries, managerCountriesIds);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
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
                        var _countries = db.Coutries.Where(x => (x.ManagerId == null || x.Manager.IsDeleted) && !x.IsDeleted).ToList();
                        var ctr = db.Coutries.ToList();
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
        [Authorize(Roles = "admin")]
        public IHttpActionResult ChangeManagerStatus(int id)
        {
            var workWithCustomer = new WorkWithManager();
            workWithCustomer.ChangePersonStatus(db, id);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetCountries()
        {
            var countries = db.Coutries.Where(c => !c.IsDeleted).ToList();
            var selectedCountries = new List<int>();
            var result = getCountries(countries, selectedCountries);
            return Ok(result);
        }

        [HttpGet]
        public List<Country> GetCountriesForSearch()
        {
            var countries = db.Coutries.Where(a => a.IsDeleted == false).ToList();
            return countries;
        }

        private List<DropDownListItem> getCountries(List<Country> allCountries, List<int> selectedCountries)
        {
            List<DropDownListItem> countries = new List<DropDownListItem>();
            allCountries.ForEach(c =>
            {
                if (selectedCountries.Contains(c.Id))
                {
                    countries.Add(new DropDownListItem()
                    {
                        Id = c.Id.ToString(),
                        Text = c.Name,
                        IsChecked = true
                    });
                }
                else
                {
                    countries.Add(new DropDownListItem()
                    {
                        Id = c.Id.ToString(),
                        Text = c.Name,
                        IsChecked = false
                    });
                }
            });

            return countries;
        }

        private ManagerEditModel convertToManagerEditModel(FullManagerViewModel manager, List<Country> allCountries, List<int> selectedCountries)
        {
            ManagerEditModel managerEditModel = new ManagerEditModel()
            {
                Id = manager.Id,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                DateOfBirth = manager.DateOfBirth.Date,
                PhoneNumber = manager.PhoneNumber,
                Email = manager.Email,
                Password = manager.Password
            };
            managerEditModel.Countries = getCountries(allCountries, selectedCountries);
            return managerEditModel;
        }
    }
}
