using Newtonsoft.Json.Linq;
using SevenWonders.DAL.Context;
using SevenWonders.Models;
using SevenWonders.WebAPI.DTO;
using SevenWonders.WebAPI.DTO.Flights;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
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
