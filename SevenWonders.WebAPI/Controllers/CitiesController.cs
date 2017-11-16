using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Cities;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SevenWonders.WebAPI.Controllers
{
    public class CitiesController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetCities()
        {
            var cities = db.Cities.Where(x => !x.IsDeleted && !x.Country.IsDeleted).ToList();
            return Ok(cities);
        }

        [HttpPost]
        public void AddCity([FromBody]CityModel model)
        {
            if (ModelState.IsValid)
            {
                //if (model.Id == 0)
                //{
                //    Country country = new Country()
                //    {
                //        Id = model.Id,
                //        Name = model.Name,
                //        IsDeleted = false
                //    };
                //    db.Coutries.Add(country);
                //}
                //if (model.Id != 0)
                //{
                //    Country country = db.Coutries.FirstOrDefault(x => x.Id == model.Id);
                //    country.Name = model.Name;
                //    db.Entry(country).State = EntityState.Modified;
                //}
                db.SaveChanges();
            }
        }

        [HttpGet]
        public IHttpActionResult GetCity(int id)
        {
            City city = db.Cities.FirstOrDefault(x => x.Id == id);
            return Ok(city);
        }

        [HttpPost]
        public IHttpActionResult DeleteCity([FromBody]int id)
        {
            //Country country = db.Coutries.Find(id);
            //country.IsDeleted = true;

            //db.Entry(country).State = EntityState.Modified;
            //db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult IsNameValid(int id, string name)
        {
            bool contain = db.Cities.Where(x => !x.IsDeleted)
                .Any(x => x.Id != id && x.Name == name);

            return Ok(!contain);
        }
    }
}
