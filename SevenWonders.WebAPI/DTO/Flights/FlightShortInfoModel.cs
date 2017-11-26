using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Flights
{
    public class FlightShortInfoModel
    {
        public int LeaveFlightId { get; set; }
        public decimal LeaveFlightPrice { get; set; }
        public string LeaveFlightNumber { get; set; }
        public string LeaveFlightAirplaneModel { get; set; }
        public string LeaveFlightAirplaneCompany { get; set; }
        public string LeaveFlightDepartureAirport { get; set; }
        public string LeaveFlightDepartureCity { get; set; }
        public string LeaveFlightDepartureCountry { get; set; }
        public DateTime LeaveFlightDepartureTime { get; set; }
        public string LeaveFlightArrivalAirport { get; set; }
        public string LeaveFlightArrivalCity { get; set; }
        public string LeaveFlightArrivalCountry { get; set; }
        public DateTime LeaveFlightArrivalTime { get; set; }

        public int ReturnFlightId { get; set; }
        public decimal ReturnFlightPrice { get; set; }
        public string ReturnFlightNumber { get; set; }
        public string ReturnFlightAirplaneModel { get; set; }
        public string ReturnFlightAirplaneCompany { get; set; }
        public string ReturnFlightDepartureAirport { get; set; }
        public string ReturnFlightDepartureCity { get; set; }
        public string ReturnFlightDepartureCountry { get; set; }
        public DateTime ReturnFlightDepartureTime { get; set; }
        public string ReturnFlightArrivalAirport { get; set; }
        public string ReturnFlightArrivalCity { get; set; }
        public string ReturnFlightArrivalCountry { get; set; }
        public DateTime ReturnFlightArrivalTime { get; set; }
    }
}