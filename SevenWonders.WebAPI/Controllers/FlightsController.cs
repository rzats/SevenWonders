using Newtonsoft.Json.Linq;
using SevenWonders.DAL.Context;
using SevenWonders.Models;
using SevenWonders.WebAPI.DTO;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
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
                DepartureAirportCode = flight.DepartureAirport.Code,
                DepartureAirportName = flight.DepartureAirport.Name,
                DepartureAirportCityName = flight.DepartureAirport.City.Name,
                DepartureAirportCountryName = flight.DepartureAirport.City.Country.Name,
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
        public void AddFlight([FromBody]JObject model)
        {
            int i = 5 + 3;
            //var manager = model["manager"].ToObject<FullManagerViewModel>();
            //var countries = model["countries"].ToObject<int[]>();

            //if (ModelState.IsValid)
            //{
            //    db.Flights.Add(flight);
            //    db.Airplanes.Add(flight.Airplane);
            //    db.SaveChanges();
            //}
        }

        [HttpGet]
        public IHttpActionResult GetAirports()
        {
            var data = db.Airports.Where(x => !x.IsDeleted
            && !x.City.IsDeleted
            && !x.City.Country.IsDeleted);

            List<DropDownListItem> countries = new List<DropDownListItem>();
            return Ok();
        }
        //public ActionResult Create()
        //{
        //    var aireplanes = db.Airplanes.Where(p2 => p2.IsDeleted == false).OrderBy(pq => pq.Model).ToList().Select(s => new
        //    {
        //        Id = s.Id,
        //        Model = string.Format("{0} (seats: {1}), {2}", s.Model, s.SeatsAmount, s.Company)
        //    });
        //    var airports = db.Airports.Where(p2 => p2.IsDeleted == false && p2.City.IsDeleted == false && p2.City.Country.IsDeleted == false)
        //        .OrderBy(pq => pq.Code).ToList().Select(s => new
        //        {
        //            Id = s.Id,
        //            Code = string.Format("{0} ({1}, {2})", s.Code, s.City.Name, s.City.Country.Name)
        //        });

        //    ViewBag.AirplaneId = new SelectList(aireplanes, "Id", "Model");
        //    ViewBag.ArrivalAirportId = new SelectList(airports, "Id", "Code");
        //    ViewBag.DepartureAirportId = new SelectList(airports, "Id", "Code");


        //    ViewBag.FlightNumbers = db.Flights.Where(p2 => p2.IsDeleted == false && p2.ArrivalAirport.IsDeleted == false
        //    && p2.ArrivalAirport.City.IsDeleted == false && p2.ArrivalAirport.City.Country.IsDeleted == false
        //    && p2.DepartureAirport.IsDeleted == false && p2.DepartureAirport.City.IsDeleted == false && p2.DepartureAirport.City.Country.IsDeleted == false)
        //        .Select(s => new { s.Id, s.Number }).ToArray();

        //    return PartialView(new Flight() { AirplaneId = 0 });
        //}

        //[HttpPost]
        //public ActionResult Create(Flight flight)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Flights.Add(flight);
        //        db.Airplanes.Add(flight.Airplane);
        //        db.SaveChanges();
        //    }
        //    var filters = Session["FlightFilters"] as SearchFlight;
        //    return RedirectToAction("Index", filters);

        //}

        //[HttpGet]
        //public ActionResult Edit(int? id)
        //{
        //    Flight flight = db.Flights.Find(id);

        //    var aireplanes = db.Airplanes.Where(p2 => p2.IsDeleted == false).OrderBy(pq => pq.Model).ToList().Select(s => new
        //    {
        //        Id = s.Id,
        //        Model = string.Format("{0} (seats: {1}), {2}", s.Model, s.SeatsAmount, s.Company)
        //    });
        //    var airports = db.Airports.Where(p2 => p2.IsDeleted == false && p2.City.IsDeleted == false && p2.City.Country.IsDeleted == false)
        //        .OrderBy(pq => pq.Code).ToList().Select(s => new
        //        {
        //            Id = s.Id,
        //            Code = string.Format("{0} ({1}, {2})", s.Code, s.City.Name, s.City.Country.Name)
        //        });

        //    ViewBag.AirplaneId = new SelectList(aireplanes, "Id", "Model", flight.AirplaneId);
        //    ViewBag.ArrivalAirportId = new SelectList(airports, "Id", "Code", flight.ArrivalAirportId);
        //    ViewBag.DepartureAirportId = new SelectList(airports, "Id", "Code", flight.DepartureAirportId);


        //    ViewBag.FlightNumbers = db.Flights.Where(p2 => p2.IsDeleted == false && p2.ArrivalAirport.IsDeleted == false
        //    && p2.ArrivalAirport.City.IsDeleted == false && p2.ArrivalAirport.City.Country.IsDeleted == false
        //    && p2.DepartureAirport.IsDeleted == false && p2.DepartureAirport.City.IsDeleted == false && p2.DepartureAirport.City.Country.IsDeleted == false)
        //        .Select(s => new { s.Id, s.Number }).ToArray();

        //    return PartialView(flight);
        //}

        //[HttpPost]
        //public ActionResult Edit(Flight flight)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(flight).State = EntityState.Modified;
        //        db.Entry(flight.Airplane).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    var filters = Session["FlightFilters"] as SearchFlight;
        //    return RedirectToAction("Index", filters);
        //}

        //[HttpPost]
        //public ActionResult Delete(int id)
        //{
        //    Flight flight = db.Flights.Find(id);
        //    flight.IsDeleted = true;

        //    db.Entry(flight).State = EntityState.Modified;
        //    db.SaveChanges();

        //    var filters = Session["FlightFilters"] as SearchFlight;
        //    return RedirectToAction("Index", filters);

        //}

        //[HttpGet]
        //public ActionResult Schedule(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Flight flight = db.Flights.Find(id);
        //    if (flight == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    List<Schedule> schedules = db.Schedule.Where(m => m.FlightId == flight.Id).Where(m => m.IsDeleted == false).ToList();
        //    List<DayOfWeek> days = new List<DayOfWeek>()
        //    { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
        //        DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday};

        //    List<ScheduleItem> schedulesNew = new List<ScheduleItem>();
        //    foreach (var day in days)
        //    {
        //        Schedule sch = new Schedule() { DayOfWeek = day, Flight = flight, FlightId = flight.Id };
        //        if (schedules.FirstOrDefault(m => m.DayOfWeek == day) != null)
        //        {
        //            sch = schedules.FirstOrDefault(m => m.DayOfWeek == day);
        //            schedulesNew.Add(new ScheduleItem(sch, true));
        //        }
        //        else
        //        {
        //            schedulesNew.Add(new ScheduleItem(sch, false));
        //        }
        //    }
        //    return PartialView(schedulesNew);
        //}

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

        //public bool IsInPlace(Airport airport, string word)
        //{
        //    if (airport.Name.ToLower().Contains(word.ToLower())
        //        || airport.Code.ToLower().Contains(word.ToLower())
        //        || airport.City.Name.ToLower().Contains(word.ToLower())
        //        || airport.City.Country.Name.ToLower().Contains(word.ToLower()))
        //    {
        //        return true;
        //    }
        //    else return false;
        //}
    }
}
