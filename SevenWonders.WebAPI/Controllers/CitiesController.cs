using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Cities;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public IHttpActionResult GetCities(int? countryId = null)
        {
            var cities = db.Cities.Where(x => !x.IsDeleted && !x.Country.IsDeleted).OrderBy(x=>x.Country.Name).ThenBy(x=>x.Name).ToList();
            if(countryId.HasValue)
            {
                cities = cities.Where(x => x.CountryId == countryId).ToList();
            }

            List<CityModel> result = new List<CityModel>();
            cities.ForEach(x =>
            {
                result.Add(convertToCityModel(x));
            });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public void AddCity([FromBody]CityModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    City city = new City()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        CountryId =model.CountryId,
                        IsDeleted = false
                    };
                    db.Cities.Add(city);
                }
                if (model.Id != 0)
                {
                    City city = db.Cities.FirstOrDefault(x => x.Id == model.Id);
                    city.Name = model.Name;
                    city.CountryId = model.CountryId;
                    db.Entry(city).State = EntityState.Modified;
                }
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
        [Authorize(Roles = "manager")]
        public IHttpActionResult DeleteCity([FromBody]int id)
        {
            City city = db.Cities.Find(id);
            city.IsDeleted = true;

            db.Entry(city).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult IsNameValid(int id, string name, int countryId)
        {
            bool contain = db.Cities.Where(x => !x.IsDeleted)
                .Any(x => x.Id != id && x.Name == name && x.CountryId==countryId);

            return Ok(!contain);
        }

        private CityModel convertToCityModel(City city)
        {
            return new CityModel()
            {
                Id = city.Id,
                Name = city.Name,
                CountryId=city.CountryId.Value,
                CountryName=city.Country.Name
            };
        }
    }
}
