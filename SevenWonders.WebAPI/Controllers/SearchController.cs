using SevenWonders.DAL.Context;
using SevenWonders.Models;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebPages.Html;

namespace SevenWonders.WebAPI.Controllers
{
    public class SearchController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetTours(
            int countryFrom,
            int? cityFrom,
            int countryTo,
            int? cityTo, 
            int people,
            DateTime departureDate,
            int duration)
        {
            var citiesFrom = db.Cities.Where(x => x.CountryId == countryFrom);
            if (cityFrom.HasValue)
                citiesFrom = citiesFrom.Where(x => x.Id == cityFrom);

            var citiesTo = db.Cities.Where(x => x.CountryId == countryTo);
            if (cityTo.HasValue)
                citiesTo = citiesTo.Where(x => x.Id == cityTo);


        }

        public List<Room> AvailableRooms(int cityId, int people, DateTime departureDate, int duration)
        {
            var reservations = db.Reservations;
            var allRooms = db.Rooms.Where(x => x.Hotel.CityId == cityId
                                            && x.MaxPeople >= people);
            var availableRooms = allRooms.Where(x=> 
        }
    }
}
