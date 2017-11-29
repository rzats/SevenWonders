using Newtonsoft.Json.Linq;
using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Flights;
using SevenWonders.WebAPI.DTO.Shared;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace SevenWonders.WebAPI.Controllers
{
    public class FlightsController : ApiController
    {
        private SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetFlights(int pageIndex, int pageSize)
        {
            var data = db.Flights.Where(p2 => ! p2.IsDeleted
            && !p2.ArrivalAirport.IsDeleted
            && !p2.ArrivalAirport.City.IsDeleted
            && !p2.ArrivalAirport.City.Country.IsDeleted
            && !p2.DepartureAirport.IsDeleted
            && !p2.DepartureAirport.City.IsDeleted
            && !p2.DepartureAirport.City.Country.IsDeleted);

            int dataCount = data.Count();

            data=data.OrderBy(x=>x.Number)
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

            List<FlightModel> flights = new List<FlightModel>();
            data.ToList().ForEach(x =>
              {
                  flights.Add(convertToFlightModel(x));
              });
            return Ok(new { flights = flights, dataCount = dataCount });
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public void AddFlight([FromBody]EditFlightModel model)
        {
            Airplane airplane = new Airplane()
            {
                Model = model.airplaneModel,
                Company = model.airplaneCompany,
                SeatsAmount = model.seatsAmount,
                IsDeleted = false
            };

            Flight flight = new Flight()
            {
                Number = model.number,
                Price = model.price,
                DepartureAirportId = model.departureAirportId,
                ArrivalAirportId = model.arrivalAirportId,
                Airplane = airplane,
                AirplaneId=airplane.Id
            };
            db.Airplanes.Add(flight.Airplane);
            db.Flights.Add(flight);
            db.SaveChanges();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public void EditFlight([FromBody]EditFlightModel model)
        {
            Flight flight = db.Flights.FirstOrDefault(x => x.Id == model.id);
            flight.Number = model.number;
            flight.Price = model.price;
            flight.DepartureAirportId = model.departureAirportId;
            flight.ArrivalAirportId = model.arrivalAirportId;

            flight.Airplane.Model = model.airplaneModel;
            flight.Airplane.Company = model.airplaneCompany;
            flight.Airplane.SeatsAmount = model.seatsAmount;

            db.Entry(flight).State = EntityState.Modified;
            db.Entry(flight.Airplane).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public IHttpActionResult DeleteFlight([FromBody]int id)
        {
            Flight flight = db.Flights.Find(id);
            flight.IsDeleted = true;

            db.Entry(flight).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetAirports()
        {
            var data = db.Airports.Where(x => !x.IsDeleted
            && !x.City.IsDeleted
            && !x.City.Country.IsDeleted).ToList();

            List<DropDownListItem> airports = new List<DropDownListItem>();
            data.ForEach(x =>
            {
                airports.Add(new DropDownListItem()
                {
                    Id = x.Id.ToString(),
                    Text = x.Name,
                    IsChecked = false
                });
            });
            return Ok(airports);
        }

        [HttpGet]
        public IHttpActionResult IsNumberValid(int id, string number)
        {
            bool contain = db.Flights.Where(p2 => !p2.IsDeleted
           && !p2.ArrivalAirport.IsDeleted
           && !p2.ArrivalAirport.City.IsDeleted
           && !p2.ArrivalAirport.City.Country.IsDeleted
           && !p2.DepartureAirport.IsDeleted
           && !p2.DepartureAirport.City.IsDeleted
           && !p2.DepartureAirport.City.Country.IsDeleted).Any(x=>x.Number==number && x.Id!=id);

            return Ok(!contain);
        }

        [HttpGet]
        public IHttpActionResult GetSchedule(int id)
        {
            Flight flight = db.Flights.Find(id);

            List<Schedule> schedules = db.Schedule.Where(m => m.FlightId == flight.Id).Where(m => !m.IsDeleted).OrderBy(x=>x.DayOfWeek).ToList();

            List<ScheduleItemModel> schedulesNew = new List<ScheduleItemModel>();
            schedules.ToList().ForEach(x =>
            {
                schedulesNew.Add(convertToScheduleItemModel(x));
            });

            return Ok(schedulesNew);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public IHttpActionResult EditSchedule([FromBody]JObject model)
        {
            var schedules = model["schedule"].ToObject<List<ScheduleItemModel>>();
            var flightId = model["flightId"].ToObject<int>();

            var olderSchedules = db.Schedule.Where(x => x.FlightId == flightId && !x.IsDeleted).ToList();
            
            //add new
            schedules.Where(x => x.Id == -1).ToList().ForEach(x =>
            {
                var item = new Schedule()
                {
                    FlightId = flightId,
                    DepartureTime = x.DepartureTime.Value,
                    ArrivalTime = x.ArrivalTime.Value,
                    DayOfWeek = (DayOfWeek)x.DayOfWeek
                };
                db.Schedule.Add(item);
            });
           
            if (olderSchedules != null && olderSchedules.Count() != 0)
            {
                //delete
                olderSchedules.Where(x => !schedules.Any(y => y.Id == x.Id))
                    .ToList().ForEach(x =>
                    {
                        x.IsDeleted = true;
                        db.Entry(x).State = EntityState.Modified;
                    });
                //change
                olderSchedules.Where(x => schedules.Any(y => y.Id == x.Id))
                    .ToList().ForEach(x =>
                    {
                        var item = schedules.FirstOrDefault(s => s.Id == x.Id);
                        x.ArrivalTime = item.ArrivalTime.Value;
                        x.DepartureTime = item.DepartureTime.Value;
                        x.DayOfWeek = (DayOfWeek)item.DayOfWeek;
                        db.Entry(x).State = EntityState.Modified;
                    });
            };
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetFlightsShortInfo(int id)
        {
            var data = db.Tours.FirstOrDefault(x => x.Id == id).Reservation;
            var flightsForRegistration = convertToFlightShortInfoModel(data);

            return Ok(flightsForRegistration);
        }
        
        private FlightShortInfoModel convertToFlightShortInfoModel(Reservation reservation)
        {
            var leaveFlightDepartureTime = reservation.LeaveDate.ToShortDateString() + " "
                + reservation.LeaveSchedule.DepartureTime.ToShortTimeString();

            var leaveFlightArrivalTime="";
            if (reservation.LeaveSchedule.ArrivalTime <= reservation.LeaveSchedule.DepartureTime)
                leaveFlightArrivalTime = reservation.LeaveDate.AddDays(1).ToShortDateString() + " "
                + reservation.LeaveSchedule.ArrivalTime.ToShortTimeString();
            else leaveFlightArrivalTime = reservation.LeaveDate.ToShortDateString() + " "
               + reservation.LeaveSchedule.ArrivalTime.ToShortTimeString();

            var returnFlightDepartureTime = reservation.ReturnDate.ToShortDateString() + " "
                + reservation.ReturnSchedule.DepartureTime.ToShortTimeString();

            var returnFlightArrivalTime = "";
            if (reservation.ReturnSchedule.ArrivalTime <= reservation.ReturnSchedule.DepartureTime)
                returnFlightArrivalTime = reservation.ReturnDate.AddDays(1).ToShortDateString() + " "
                + reservation.ReturnSchedule.ArrivalTime.ToShortTimeString();
            else returnFlightArrivalTime = reservation.ReturnDate.ToShortDateString() + " "
               + reservation.ReturnSchedule.ArrivalTime.ToShortTimeString();
            return new FlightShortInfoModel()
            {
                LeaveFlightNumber = reservation.LeaveSchedule.Flight.Number,
                LeaveFlightAirplaneModel = reservation.LeaveSchedule.Flight.Airplane.Model,
                LeaveFlightAirplaneCompany = reservation.LeaveSchedule.Flight.Airplane.Company,
                LeaveFlightDepartureAirport = reservation.LeaveSchedule.Flight.DepartureAirport.Name,
                LeaveFlightDepartureCity = reservation.LeaveSchedule.Flight.DepartureAirport.City.Name,
                LeaveFlightDepartureCountry = reservation.LeaveSchedule.Flight.DepartureAirport.City.Country.Name,
                LeaveFlightDepartureTime = DateTime.Parse(leaveFlightDepartureTime),
                LeaveFlightArrivalAirport = reservation.LeaveSchedule.Flight.ArrivalAirport.Name,
                LeaveFlightArrivalCity = reservation.LeaveSchedule.Flight.ArrivalAirport.City.Name,
                LeaveFlightArrivalCountry = reservation.LeaveSchedule.Flight.ArrivalAirport.City.Country.Name,
                LeaveFlightArrivalTime = DateTime.Parse(leaveFlightArrivalTime),

                ReturnFlightNumber = reservation.ReturnSchedule.Flight.Number,
                ReturnFlightAirplaneModel = reservation.ReturnSchedule.Flight.Airplane.Model,
                ReturnFlightAirplaneCompany = reservation.ReturnSchedule.Flight.Airplane.Company,
                ReturnFlightDepartureAirport = reservation.ReturnSchedule.Flight.DepartureAirport.Name,
                ReturnFlightDepartureCity = reservation.ReturnSchedule.Flight.DepartureAirport.City.Name,
                ReturnFlightDepartureCountry = reservation.ReturnSchedule.Flight.DepartureAirport.City.Country.Name,
                ReturnFlightDepartureTime = DateTime.Parse(returnFlightDepartureTime),
                ReturnFlightArrivalAirport = reservation.ReturnSchedule.Flight.ArrivalAirport.Name,
                ReturnFlightArrivalCity = reservation.ReturnSchedule.Flight.ArrivalAirport.City.Name,
                ReturnFlightArrivalCountry = reservation.ReturnSchedule.Flight.ArrivalAirport.City.Country.Name,
                ReturnFlightArrivalTime = DateTime.Parse(returnFlightArrivalTime),
            };
        }

        private FlightModel convertToFlightModel(Flight flight)
        {
            return new FlightModel()
            {
                Id = flight.Id,
                Number = flight.Number,
                DepartureAirportId = flight.DepartureAirport.Id,
                DepartureAirportCode = flight.DepartureAirport.Code,
                DepartureAirportName = flight.DepartureAirport.Name,
                DepartureAirportCityName = flight.DepartureAirport.City.Name,
                DepartureAirportCountryName = flight.DepartureAirport.City.Country.Name,
                ArrivalAirportId = flight.ArrivalAirport.Id,
                ArrivalAirportCode = flight.ArrivalAirport.Code,
                ArrivalAirportName = flight.ArrivalAirport.Name,
                ArrivalAirportCityName = flight.ArrivalAirport.City.Name,
                ArrivalAirportCountryName = flight.ArrivalAirport.City.Country.Name,
                Price = flight.Price,
                AirplaneSeatsAmount = flight.Airplane.SeatsAmount,
                AirplaneCompany = flight.Airplane.Company,
                AirplaneModel = flight.Airplane.Model
            };
        }

        private ScheduleItemModel convertToScheduleItemModel(Schedule item)
        {
            return new ScheduleItemModel()
            {
                Id=item.Id,
                DayOfWeek = (int) item.DayOfWeek,
                DepartureTime=item.DepartureTime,
                ArrivalTime = item.ArrivalTime
            };
        }
    }
}
