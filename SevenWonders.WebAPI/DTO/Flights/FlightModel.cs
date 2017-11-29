using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Flights
{
    public class FlightModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int DepartureAirportId { get; set; }
        public string DepartureAirportCode { get; set; }
        public string DepartureAirportName { get; set; }
        public string DepartureAirportCityName { get; set; }
        public string DepartureAirportCountryName { get; set; }
        public int ArrivalAirportId { get; set; }
        public string ArrivalAirportCode { get; set; }
        public string ArrivalAirportName { get; set; }
        public string ArrivalAirportCityName { get; set; }
        public string ArrivalAirportCountryName { get; set; }
        public decimal Price { get; set; }
        public int AirplaneSeatsAmount { get; set; }
        public string AirplaneCompany { get; set; }
        public string AirplaneModel { get; set; }
    }
}