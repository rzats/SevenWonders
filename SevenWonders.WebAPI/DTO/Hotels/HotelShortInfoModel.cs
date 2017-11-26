using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Hotels
{
    public class HotelShortInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string FoodType { get; set; }
        public decimal FoodPrice { get; set; }
        public List<string> Facilities { get; set; }
        public List<PhotoModel> HotelPhotos { get; set; }
    }
}