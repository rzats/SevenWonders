using SevenWonders.DAL.Context;
using SevenWonders.WebAPI.DTO.Tours;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SevenWonders.WebAPI.Controllers
{
    public class ToursManagementController : ApiController
    {
        private SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetTours(int pageIndex, int pageSize)
        {
            //List<Tour> data = db.Tours.Where(t => t.Reservation.Room.Hotel.City.Country.Manager.Id == manager.Id && !t.IsDeleted).ToList();
            var data = db.Tours.Where(t => !t.IsDeleted);

            int dataCount = data.Count();
            data = data.OrderByDescending(x => x.CreationDate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

            List<TourModel> tours = new List<TourModel>();
            data.ToList().ForEach(x =>
            {
                tours.Add(convertToTourModel(x));
            });
            return Ok(new { tours = tours, dataCount = dataCount });
        }

        private TourModel convertToTourModel(Tour tour)
        {
            return new TourModel()
            {
                Id = tour.Id,
                TourState=tour.TourState.Name,
                CustomerFirstName = tour.Customer.FirstName,
                CustomerLastName = tour.Customer.LastName,
                CustomerEmail = tour.Customer.Email,
                CustomerPhoneNumber = tour.Customer.PhoneNumber,
                OrderDate = tour.CreationDate,
                Price = tour.TotalPrice.Value,
                LeaveDate = tour.Reservation.LeaveDate,
                ReturnDate = tour.Reservation.ReturnDate,
                DepartureAirportCode = tour.Reservation.LeaveFlight.DepartureAirport.Code,
                DepartureAirportCity = tour.Reservation.LeaveFlight.DepartureAirport.City.Name,
                DepartureAirportCountry = tour.Reservation.LeaveFlight.DepartureAirport.City.Country.Name,
                ArrivalAirportCode = tour.Reservation.ReturnFlight.DepartureAirport.Code,
                ArrivalAirportCity = tour.Reservation.ReturnFlight.DepartureAirport.City.Name,
                ArrivalAirportCountry = tour.Reservation.ReturnFlight.DepartureAirport.City.Country.Name
            };
        }

    }
}
