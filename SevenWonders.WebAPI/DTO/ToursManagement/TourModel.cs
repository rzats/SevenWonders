using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.ToursManagement
{
    public class TourModel
    {
        public int Id { get; set; }
        public string TourState { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal Price { get; set; }

        public DateTime LeaveDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public string DepartureAirportCode { get; set; }
        public string DepartureAirportCity { get; set; }
        public string DepartureAirportCountry { get; set; }

        public string ArrivalAirportCode { get; set; }
        public string ArrivalAirportCity { get; set; }
        public string ArrivalAirportCountry { get; set; }

        public int HotelId { get; set; }
        public string HotelName { get; set; }
    }
}