using SevenWonders.DAL.Context;
using SevenWonders.Models;
using SevenWonders.WebAPI.DTO.Flights;
using SevenWonders.WebAPI.DTO.Hotels;
using SevenWonders.WebAPI.DTO.Rooms;
using SevenWonders.WebAPI.DTO.Search;
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebPages.Html;

namespace SevenWonders.WebAPI.Controllers
{
    public class SearchController : ApiController
    {
        SevenWondersContext db = new SevenWondersContext();

        [HttpGet]
        public IHttpActionResult GetTours(
            int countryFrom,
            int? cityFrom,
            int countryTo,
            int? cityTo, 
            int people,
            DateTime departureDate,
            int duration)
        {
            try
            {
                var citiesFrom = db.Cities.Where(x => x.CountryId == countryFrom && !x.IsDeleted).ToList();
                if (cityFrom.HasValue)
                    citiesFrom = citiesFrom.Where(x => x.Id == cityFrom).ToList();

                var citiesTo = db.Cities.Where(x => x.CountryId == countryTo && !x.IsDeleted).ToList();
                if (cityTo.HasValue)
                    citiesTo = citiesTo.Where(x => x.Id == cityTo).ToList();

                var availableTours = new List<TourForSearchModel>();

                foreach (var tempCityFrom in citiesFrom)
                {
                    foreach (var tempCityTo in citiesTo)
                    {
                        if (tempCityFrom.Id != tempCityTo.Id)
                        {
                            var availableSchedulesDeparture = availableSchedules(tempCityFrom.Id, tempCityTo.Id, people, departureDate);
                            var availableSchedulesArrival = availableSchedules(tempCityTo.Id, tempCityFrom.Id, people, departureDate.AddDays(duration));
                            var availableRooms = this.availableRooms(tempCityTo.Id, people, departureDate, duration);


                            var bestScheduleDeparture = availableSchedulesDeparture.Aggregate((i1, i2) => i1.Flight.Price < i2.Flight.Price ? i1 : i2);
                            var bestScheduleArrival = availableSchedulesArrival.Aggregate((i1, i2) => i1.Flight.Price < i2.Flight.Price ? i1 : i2);

                            var hotelForTours = availableRooms.GroupBy(x => x.HotelId).ToDictionary(x => x.Key, x => x.ToList());
                            foreach (var hotelModel in hotelForTours)
                            {
                                var hotel = db.Hotels.Find(hotelModel.Key.Value);
                                var newTour = new TourForSearchModel()
                                {
                                    People = people,
                                    DepartureDate = departureDate,
                                    ArrivaleDate = departureDate.AddDays(duration),
                                    DepartureScheduleId = bestScheduleDeparture.Id,
                                    ArrivalScheduleId = bestScheduleArrival.Id,
                                    Hotel = convertToHotelShortInfoModel(hotel),
                                    Flights = convertToFlightShortInfoModel(bestScheduleDeparture, bestScheduleArrival, departureDate, departureDate.AddDays(duration)),
                                    Rooms = hotelModel.Value.Select(x => convertToRoomShortInfoModel(x)).ToList()
                                };
                                availableTours.Add(newTour);
                            }
                        }

                    }
                }
                return Ok(availableTours);
            }
            catch(Exception ex)
            {
                return Ok();
            }
        }

        private List<Room> availableRooms(int cityId, int people, DateTime departureDate, int duration)
        {
            DateTime arrivalDate = departureDate.AddDays(duration);
            var reservations = db.Tours.Where(x => !x.IsDeleted).Select(x => x.Reservation).Where(x => !x.IsDeleted).ToList();
            var allRooms = db.Rooms.Where(x => x.Hotel.CityId == cityId
                                            && x.MaxPeople >= people
                                            && !x.IsDeleted
                                            && !x.Hotel.IsDeleted).ToList();

            var availableRooms = allRooms.Where(x => !reservations.Any(y => y.RoomId == x.Id && (departureDate >= y.ReturnDate || arrivalDate <= y.LeaveDate))).ToList();
            return availableRooms.ToList();
        }
        private List<Schedule> availableSchedules(int cityDepartureId, int cityArrivalId, int people, DateTime date)
        {
            var reservations = db.Tours.Where(x => !x.IsDeleted).Select(x => x.Reservation).Where(x => !x.IsDeleted).ToList();
            var allSchedules = db.Schedule.Where(x => x.Flight.DepartureAirport.CityId == cityDepartureId 
                                            && x.Flight.ArrivalAirport.CityId == cityArrivalId
                                            && x.DayOfWeek == date.DayOfWeek
                                            && !x.IsDeleted
                                            && !x.Flight.IsDeleted
                                            && !x.Flight.ArrivalAirport.IsDeleted
                                            && !x.Flight.DepartureAirport.IsDeleted).ToList();
            var availableSchedules = allSchedules.Where(x => (x.Flight.Airplane.SeatsAmount
            - reservations.Where(y => y.LeaveDate.Date == date && y.LeaveScheduleId == x.Id).Sum(y => y.PersonAmount)
            - reservations.Where(y => y.ReturnDate.Date == date && y.ReturnScheduleId == x.Id).Sum(y => y.PersonAmount)) > 0).ToList() ;

            return availableSchedules.ToList();
        }

        private FlightShortInfoModel convertToFlightShortInfoModel(Schedule leaveSchedule, Schedule returnSchedule, DateTime leaveDate, DateTime returnDate)
        {
            var leaveFlightDepartureTime = leaveDate.ToShortDateString() + " "
                + leaveSchedule.DepartureTime.ToShortTimeString();

            var leaveFlightArrivalTime = "";
            if (leaveSchedule.ArrivalTime <= leaveSchedule.DepartureTime)
                leaveFlightArrivalTime = leaveDate.AddDays(1).ToShortDateString() + " "
                + leaveSchedule.ArrivalTime.ToShortTimeString();
            else leaveFlightArrivalTime = leaveDate.ToShortDateString() + " "
               + leaveSchedule.ArrivalTime.ToShortTimeString();

            var returnFlightDepartureTime =returnDate.ToShortDateString() + " "
                + returnSchedule.DepartureTime.ToShortTimeString();

            var returnFlightArrivalTime = "";
            if (returnSchedule.ArrivalTime <= returnSchedule.DepartureTime)
                returnFlightArrivalTime = returnDate.AddDays(1).ToShortDateString() + " "
                + returnSchedule.ArrivalTime.ToShortTimeString();
            else returnFlightArrivalTime = returnDate.ToShortDateString() + " "
               + returnSchedule.ArrivalTime.ToShortTimeString();
            return new FlightShortInfoModel()
            {
                LeaveFlightNumber = leaveSchedule.Flight.Number,
                LeaveFlightAirplaneModel = leaveSchedule.Flight.Airplane.Model,
                LeaveFlightAirplaneCompany = leaveSchedule.Flight.Airplane.Company,
                LeaveFlightDepartureAirport = leaveSchedule.Flight.DepartureAirport.Name,
                LeaveFlightDepartureCity = leaveSchedule.Flight.DepartureAirport.City.Name,
                LeaveFlightDepartureCountry = leaveSchedule.Flight.DepartureAirport.City.Country.Name,
                LeaveFlightDepartureTime = DateTime.Parse(leaveFlightDepartureTime),
                LeaveFlightArrivalAirport = leaveSchedule.Flight.ArrivalAirport.Name,
                LeaveFlightArrivalCity = leaveSchedule.Flight.ArrivalAirport.City.Name,
                LeaveFlightArrivalCountry = leaveSchedule.Flight.ArrivalAirport.City.Country.Name,
                LeaveFlightArrivalTime = DateTime.Parse(leaveFlightArrivalTime),

                ReturnFlightNumber = returnSchedule.Flight.Number,
                ReturnFlightAirplaneModel = returnSchedule.Flight.Airplane.Model,
                ReturnFlightAirplaneCompany = returnSchedule.Flight.Airplane.Company,
                ReturnFlightDepartureAirport = returnSchedule.Flight.DepartureAirport.Name,
                ReturnFlightDepartureCity = returnSchedule.Flight.DepartureAirport.City.Name,
                ReturnFlightDepartureCountry = returnSchedule.Flight.DepartureAirport.City.Country.Name,
                ReturnFlightDepartureTime = DateTime.Parse(returnFlightDepartureTime),
                ReturnFlightArrivalAirport =returnSchedule.Flight.ArrivalAirport.Name,
                ReturnFlightArrivalCity = returnSchedule.Flight.ArrivalAirport.City.Name,
                ReturnFlightArrivalCountry = returnSchedule.Flight.ArrivalAirport.City.Country.Name,
                ReturnFlightArrivalTime = DateTime.Parse(returnFlightArrivalTime),
            };
        }
        private HotelShortInfoModel convertToHotelShortInfoModel(Hotel hotel)
        {
            var ll = new List<PhotoModel>();
            ll.Add(new PhotoModel()
            {
                Id = 1,
                PhotoLink = "../../Content/img/Hotels/Hotel10/1.jpg"
            });
            return new HotelShortInfoModel()
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Facilities = hotel.Facilities.Select(x => {
                    return x.Name;
                }).ToList(),
                Description = hotel.Description,
                Rating = hotel.Rating.Id,
                Country = hotel.City.Country.Name,
                City = hotel.City.Name,
                Address = hotel.Adress,
                HotelPhotos = ll
            };
        }
        private RoomShortInfoModel convertToRoomShortInfoModel(Room room)
        {
            return new RoomShortInfoModel()
            {
                Id = room.Id,
                RoomType = room.RoomType.Name,
                MaxPeople = room.MaxPeople,
                Price = room.Price,
                WindowView = room.WindowView,
                Equipments = room.Equipments.Select(x => x.Name).ToList(),
                RoomsPhotos = room.RoomsPhotos.Select(x => new PhotoModel() { Id = x.Id, PhotoLink = x.photoLink }).ToList()
            };
        }
    }
}
