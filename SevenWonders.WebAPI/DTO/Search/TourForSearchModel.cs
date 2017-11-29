using SevenWonders.WebAPI.DTO.Flights;
using SevenWonders.WebAPI.DTO.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Search
{
    public class TourForSearchModel
    {
        public int People { get; set; }
        public int Duration { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivaleDate { get; set; }
        public int DepartureScheduleId { get; set; }
        public int ArrivalScheduleId { get; set; }

        public HotelShortInfoModel Hotel { get; set; }
        public FlightShortInfoModel Flights { get; set; }
        public List<RoomShortInfoModel> Rooms { get; set; }
        public bool CanBook { get; set; }
        public TourForSearchModel() { }
    }
}