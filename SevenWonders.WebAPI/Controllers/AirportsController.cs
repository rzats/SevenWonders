using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Airports;
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
    public class AirportsController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetAirports()
        {
            var airports = db.Airports
                .Where(x => !x.IsDeleted && !x.City.IsDeleted && !x.City.Country.IsDeleted).OrderBy(x => x.Name).ToList();

            List<AirportModel> result = new List<AirportModel>();
            airports.ToList().ForEach(x =>
            {
                result.Add(convertToAirportModel(x));
            });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public void AddAirport([FromBody]AirportModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    Airport airport = new Airport()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Code=model.Code,
                        CityId = model.CityId,
                        IsDeleted = false
                    };
                    db.Airports.Add(airport);
                }
                if (model.Id != 0)
                {
                    Airport airport = db.Airports.FirstOrDefault(x => x.Id == model.Id);
                    airport.Name = model.Name;
                    airport.Code = model.Code;
                    airport.CityId = model.CityId;
                    db.Entry(airport).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        [HttpGet]
        public IHttpActionResult GetAirport(int id)
        {
            Airport airport = db.Airports.FirstOrDefault(x => x.Id == id);
            return Ok(airport);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public IHttpActionResult DeleteAirport([FromBody]int id)
        {
            Airport airport = db.Airports.Find(id);
            airport.IsDeleted = true;

            db.Entry(airport).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult IsCodeValid(int id, string code)
        {
            bool contain = db.Airports.Where(x => !x.IsDeleted)
                .Any(x => x.Id != id && x.Code == code);

            return Ok(!contain);
        }

        private AirportModel convertToAirportModel(Airport airport)
        {
            return new AirportModel()
            {
                Id = airport.Id,
                Name = airport.Name,
                Code=airport.Code,
                CityId = airport.CityId.Value,
                CityName = airport.City.Name
            };
        }
    }
}
