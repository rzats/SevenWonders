using SevenWonders.WebAPI.DTO.ToursManagement;
using SevenWonders.WebAPI.Models;
using SevenWonders.DAL.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public IHttpActionResult GetToursForManager(int pageIndex, int pageSize)
        {
            var email = User.Identity.Name;
            var manager = getManager(email);

            var data = db.Tours.Where(t => t.Reservation.Room.Hotel.City.Country.Manager.Id == manager.Id && !t.IsDeleted);

            int dataCount = data.Count();
            data = data.OrderByDescending(x => x.CreationDate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

            var KK = data.ToList();
            List<TourModel> tours = new List<TourModel>();
            data.ToList().ForEach(x =>
            {
                tours.Add(convertToTourModel(x));
            });
            return Ok(new { tours = tours, dataCount = dataCount });
        }

        [HttpGet]
        public IHttpActionResult GetToursForCustomer(int pageIndex, int pageSize)
        {
            var email = User.Identity.Name;
            var customer = getCustomer(email);
            List<TourModel> toursToReturn = new List<TourModel>();

            var data = customer.Tours.Where(x => !x.IsDeleted && !x.Reservation.IsDeleted);
            int dataCount = data.Count();
            data = data.OrderByDescending(x => x.CreationDate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

            var KK = data.ToList();
            List<TourModel> tours = new List<TourModel>();
            data.ToList().ForEach(x =>
            {
                tours.Add(convertToTourModel(x));
            });
            return Ok(new { tours = tours, dataCount = dataCount });
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public IHttpActionResult DeleteTour([FromBody]int id)
        {
            Tour tour = db.Tours.Find(id);
            tour.IsDeleted = true;

            db.Entry(tour).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public IHttpActionResult PayForTour([FromBody]int id)
        {
            Tour tour = db.Tours.Find(id);
            tour.TourStateId = db.TourStates.Where(ts => ts.Name == "Paid").FirstOrDefault().Id;

            tour.Customer.TotalPayment = tour.Customer.TotalPayment + tour.TotalPrice.Value;
            tour.Customer.Discount = calculateDiscount(tour.Customer.TotalPayment);

            db.Entry(tour).State = EntityState.Modified;
            db.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public IHttpActionResult UpdateTours()
        {
            Manager manager = getManager(User.Identity.Name);
            List<Tour> data = db.Tours.Where(x=>!x.IsDeleted && x.Reservation.Room.Hotel.City.Country.Manager.Id== manager.Id).ToList();
            foreach (var tour in data)
            {
                if (!tour.IsDeleted)
                {
                    if (tour.TourState.Name == "Registered" && tour.CreationDate.AddHours(48) < DateTime.Now)
                    {
                        tour.IsDeleted = true;
                        tour.Reservation.IsDeleted = true;

                        db.Entry(tour).State = EntityState.Modified;
                    }
                    if (tour.TourState.Name == "Paid" && tour.Reservation.ReturnDate < DateTime.Now)
                    {
                        tour.TourStateId = db.TourStates.Where(ts => ts.Name == "Done").FirstOrDefault().Id;

                        db.Entry(tour).State = EntityState.Modified;
                    }
                }
            }
            db.SaveChanges();
            return Ok();
        }

        private TourModel convertToTourModel(Tour tour)
        {
            return new TourModel()
            {
                Id = tour.Id,
                TourState = tour.TourState.Name,
                CustomerFirstName = tour.Customer.FirstName,
                CustomerLastName = tour.Customer.LastName,
                CustomerEmail = tour.Customer.Email,
                CustomerPhoneNumber = tour.Customer.PhoneNumber,
                OrderDate = tour.CreationDate,
                Price = tour.TotalPrice.Value,
                LeaveDate = tour.Reservation.LeaveDate,
                ReturnDate = tour.Reservation.ReturnDate,
                DepartureAirportCode = tour.Reservation.LeaveSchedule.Flight.DepartureAirport.Code,
                DepartureAirportCity = tour.Reservation.LeaveSchedule.Flight.DepartureAirport.City.Name,
                DepartureAirportCountry = tour.Reservation.LeaveSchedule.Flight.DepartureAirport.City.Country.Name,
                ArrivalAirportCode = tour.Reservation.ReturnSchedule.Flight.DepartureAirport.Code,
                ArrivalAirportCity = tour.Reservation.ReturnSchedule.Flight.DepartureAirport.City.Name,
                ArrivalAirportCountry = tour.Reservation.ReturnSchedule.Flight.DepartureAirport.City.Country.Name,
                HotelId = tour.Reservation.Room.HotelId.Value,
                HotelName= tour.Reservation.Room.Hotel.Name
            };
        }

        private int calculateDiscount(decimal totalPayment)
        {
            if (totalPayment >= 50000)
                return 10;
            else if (totalPayment >= 20000)
                return 5;
            else if (totalPayment >= 10000)
                return 3;
            else return 0;
        }

        private Customer getCustomer(string email)
        {
            return db.Customers.FirstOrDefault(x => x.Email == email && !x.IsDeleted);
        }
        private Manager getManager(string email)
        {
            return db.Managers.FirstOrDefault(x => x.Email == email && !x.IsDeleted);
        }
    }
}