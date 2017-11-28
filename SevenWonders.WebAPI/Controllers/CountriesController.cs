using SevenWonders.WebAPI.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Countries;

namespace SevenWonders.WebAPI.Controllers
{
    public class CountriesController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetCountries()
        {
            var countries = db.Coutries.Where(x => !x.IsDeleted).OrderBy(x=>x.Name).ToList();
            return Ok(countries);
        }

        [HttpPost]
        public void AddCountry([FromBody]CountryModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    Country country = new Country()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        IsDeleted = false
                    };
                    db.Coutries.Add(country);                  
                }
                if (model.Id != 0)
                {
                    Country country = db.Coutries.FirstOrDefault(x => x.Id == model.Id);
                    country.Name = model.Name;
                    db.Entry(country).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        [HttpGet]
        public IHttpActionResult GetCountry(int id)
        {
            Country country = db.Coutries.FirstOrDefault(x => x.Id == id);
            return Ok(country);
        }

        [HttpPost]
        public IHttpActionResult DeleteCountry([FromBody]int id)
        {
            Country country = db.Coutries.Find(id);
            country.IsDeleted = true;

            db.Entry(country).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult IsNameValid(int id, string name)
        {
            bool contain = db.Coutries.Where(x => !x.IsDeleted)
                .Any(x => x.Id != id && x.Name == name);

            return Ok(!contain);
        }
    }
}