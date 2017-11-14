using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Flights
{
    public class EditFlightModel
    {
        public int id { get; set; }
        public string number { get; set; }
        public decimal price { get; set; }
        public int departureAirportId { get; set; }
        public int arrivalAirportId { get; set; }
        public string airplaneModel { get; set; }
        public string airplaneCompany { get; set; }
        public int seatsAmount { get; set; }
    }
}