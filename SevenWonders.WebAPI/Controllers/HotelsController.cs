using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Hotels;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SevenWonders.WebAPI.Controllers
{
    public class HotelsController : ApiController
    {
        private SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetHotelShortInfo(int id)
        {
            var data = db.Hotels.FirstOrDefault(x => x.Id == id);
            var hotel = convertToTourModel(data);

            return Ok(hotel);
        }

        private HotelShortInfoModel convertToTourModel(Hotel hotel)
        {
            var ll = new List<HotelPhotoModel>();
            ll.Add(new HotelPhotoModel()
            {
                Id = 1,
                PhotoLink = "../../Content/img/Hotels/Hotel10/2.jpg"
            });
            ll.Add(new HotelPhotoModel()
            {
                Id = 1,
                PhotoLink = "../../Content/img/Hotels/Hotel10/1.jpg"
            });
            return new HotelShortInfoModel()
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Facilities = hotel.Facilities.Select(x => {
                    return x.Name;
                }).ToList(),
                Description = hotel.Description,
                Rating = hotel.Rating.Id,
                Country = hotel.City.Country.Name,
                City = hotel.City.Name,
                Address = hotel.Adress,
                HotelPhotos = ll
            };
        }
    }
}