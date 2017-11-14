﻿using Newtonsoft.Json.Linq;
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
                  flights.Add(ConvertToFlightModel(x));
              });
            return Ok(new { flights = flights, dataCount = dataCount });
        }

        public FlightModel ConvertToFlightModel(Flight flight)
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
        public IHttpActionResult Schedule(int id)
        {
            Flight flight = db.Flights.Find(id);

            List<Schedule> schedules = db.Schedule.Where(m => m.FlightId == flight.Id).Where(m => m.IsDeleted == false).ToList();

            List<ScheduleItemModel> schedulesNew = new List<ScheduleItemModel>();
            schedules.ToList().ForEach(x =>
            {
                schedulesNew.Add(ConvertToScheduleItemModel(x));
            });

            schedulesNew.Add(new ScheduleItemModel()
            {
                Id = 32,
                DayOfWeek = 2,
                DepartureTime = null,
                ArrivalTime = null
            });
            return Ok(schedulesNew);
        }

        public ScheduleItemModel ConvertToScheduleItemModel(Schedule item)
        {
            return new ScheduleItemModel()
            {
                Id=item.Id,
                DayOfWeek = (int) item.DayOfWeek,
                DepartureTime=item.DepartureTime,
                ArrivalTime = item.ArrivalTime
            };
        }
        //[HttpPost]
        //public ActionResult Schedule(List<ScheduleItem> schedulesForEveryDayNew)
        //{
        //    Flight flight = db.Flights.Find(schedulesForEveryDayNew[0].Item.FlightId);

        //    List<Schedule> schedulesOld = db.Schedule.Where(m => m.FlightId == flight.Id).Where(m => m.IsDeleted == false).ToList();
        //    List<DayOfWeek> days = new List<DayOfWeek>()
        //    { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
        //        DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday};

        //    List<ScheduleItem> schedulesForEveryDay = new List<ScheduleItem>();
        //    foreach (var day in days)
        //    {
        //        Schedule sch = new Schedule() { DayOfWeek = day, Flight = flight, FlightId = flight.Id };
        //        if (schedulesOld.FirstOrDefault(m => m.DayOfWeek == day) != null)
        //        {
        //            sch = schedulesOld.FirstOrDefault(m => m.DayOfWeek == day);
        //            schedulesForEveryDay.Add(new ScheduleItem(sch, true));
        //        }
        //        else
        //        {
        //            schedulesForEveryDay.Add(new ScheduleItem(sch, false));
        //        }
        //    }


        //    for (int i = 0; i < 7; i++)
        //    {
        //        if (schedulesForEveryDay[i].IsCreated && !schedulesForEveryDayNew[i].IsCreated)
        //        {
        //            schedulesForEveryDay[i].Item.IsDeleted = true;
        //            db.Entry(schedulesForEveryDay[i].Item).State = EntityState.Modified;

        //        }
        //        else if (schedulesForEveryDay[i].IsCreated && schedulesForEveryDayNew[i].IsCreated)
        //        {
        //            Schedule sch = db.Schedule.Find(schedulesForEveryDayNew[i].Item.Id);
        //            sch.DepartureTime = schedulesForEveryDayNew[i].Item.DepartureTime;
        //            sch.ArrivalTime = schedulesForEveryDayNew[i].Item.ArrivalTime;
        //            db.Entry(sch).State = EntityState.Modified;
        //        }
        //        else if (!schedulesForEveryDay[i].IsCreated && schedulesForEveryDayNew[i].IsCreated)
        //        {
        //            db.Schedule.Add(schedulesForEveryDayNew[i].Item);
        //        }
        //    }
        //    db.SaveChanges();

        //    var filters = Session["FlightFilters"] as SearchFlight;
        //    return RedirectToAction("Index", filters);
        //}
    }
}
