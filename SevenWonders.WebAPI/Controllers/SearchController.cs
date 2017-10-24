using SevenWonders.DAL.Context;
using SevenWonders.Models;
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
        public class ReservationModelJson
        {
            public int PeopleCount;
            public String LeaveFlight;
            public String LeaveDate;
            public String ReturnFlight;
            public String ReturnDate;
            public String FoodType;
            public int AvailableRoom;
            public String HotelLocation;
        }

        [Authorize]
        private int FindCustomerId()
        {
            int userId = User.Identity.GetUserId<int>();
            string userEmail = db.Users.Where(p => p.Id == userId).Select(p => p.Email).FirstOrDefault();
            return db.Customers.Where(p => p.Email == userEmail).Select(p => p.Id).FirstOrDefault();

        }



        [HttpPost]
        public List<ReservationModel> SearchTours(SearchModel model)
        {
            string error = "Sorry, but there are no tours, which match to your requirements. Try to change some information.";
            List<Flight> leaveFlights = FlightsSearch(model.CityFrom, model.CityTo, model.DapartureDay, model.PeopleNumber, true, ref error);
            List<Flight> returnFlights = FlightsSearch(model.CityTo, model.CityFrom, model.DapartureDay.AddDays(model.Duration), model.PeopleNumber, false, ref error);
            List<Room> rooms = HotelsSearch(model.CityTo, model.DapartureDay, model.DapartureDay.AddDays(model.Duration), model.PeopleNumber, ref error, model.Hotel, model.FoodType);


            List<Reservation> reservations = new List<Reservation>();
            for (int i = 0; i < leaveFlights.Count; ++i)
                for (int j = 0; j < returnFlights.Count; ++j)
                    for (int k = 0; k < rooms.Count; ++k)
                    {
                        Reservation tempReservation = new Reservation();
                        tempReservation.LeaveDate = model.DapartureDay;
                        tempReservation.ReturnDate = model.DapartureDay.AddDays(model.Duration);
                        tempReservation.LeaveFlight = leaveFlights[i];
                        tempReservation.PersonAmount = model.PeopleNumber;
                        tempReservation.ReturnFlight = returnFlights[j];
                        tempReservation.Room = rooms[k];
                        reservations.Add(tempReservation);
                    }

            List<ReservationModel> resModels = new List<ReservationModel>();
            for (int i = 0; i < reservations.Count; ++i)
            {
                var appropriateReservation = resModels.Where(p => (p.Hotel.Id == reservations[i].Room.HotelId
                && p.Reservation.LeaveFlight.Id == reservations[i].LeaveFlight.Id
                && p.Reservation.ReturnFlight.Id == reservations[i].ReturnFlight.Id)).FirstOrDefault();

                if (appropriateReservation != null)
                {
                    bool flag = false;
                    foreach (var item in appropriateReservation.Rooms)
                    {
                        //Check if there are such room in list
                        //it means that these reservations have different flights 
                        if (item.Id == reservations[i].RoomId)
                        {
                            ReservationModel tempReservModel = new ReservationModel();
                            tempReservModel.Duration = model.Duration;
                            tempReservModel.Reservation = reservations[i];
                            tempReservModel.Rooms.Add(reservations[i].Room);
                            tempReservModel.Hotel = reservations[i].Room.Hotel;
                            tempReservModel.TotalPrices.Add(0);
                            tempReservModel.PricesWithoutFood.Add(0);
                            tempReservModel.FoodInclude.Add(true);
                            tempReservModel.Food = true;
                            resModels.Add(tempReservModel);
                            flag = true;
                            break;
                        }
                    }
                    //If there  are not such room in list, then we will add it
                    if (flag == false)
                    {
                        appropriateReservation.Rooms.Add(reservations[i].Room);
                        appropriateReservation.TotalPrices.Add(0);
                        appropriateReservation.PricesWithoutFood.Add(0);
                        appropriateReservation.FoodInclude.Add(true);
                        appropriateReservation.Food = true;

                    }
                }
                //if list doesn't contain item with such hotel
                else
                {
                    ReservationModel tempReservModel = new ReservationModel();
                    tempReservModel.Duration = model.Duration;
                    tempReservModel.Reservation = reservations[i];
                    tempReservModel.Rooms.Add(reservations[i].Room);
                    tempReservModel.Hotel = reservations[i].Room.Hotel;
                    tempReservModel.TotalPrices.Add(0);
                    tempReservModel.PricesWithoutFood.Add(0);
                    tempReservModel.FoodInclude.Add(true);
                    tempReservModel.Food = true;
                    resModels.Add(tempReservModel);
                }
            }
            resModels = PricesCalculating(resModels);

            if (model.PriceFrom != 0 || model.PriceTo != 0)
                resModels = PriceRangeReservations(resModels, model.PriceFrom, model.PriceTo);

            return resModels;
        }

        [HttpGet]
        public List<City> GetCities(int countryId)
        {
            var cities = db.Cities.Where(c => (c.IsDeleted == false && c.Country.Id == countryId)).OrderBy(p => p.Name);

            var responce = cities.ToList();
            return responce;
        }

        [HttpGet]
        public List<Hotel> GetHotels(int cityId)
        {
            var hotels = db.Hotels.Where(c => (c.IsDeleted == false && c.City.Id == cityId))
                           .OrderBy(p => p.Name);

            return hotels.ToList();
        }

        private List<ReservationModel> PricesCalculating(List<ReservationModel> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int hotelId = list[i].Hotel.Id;
                var extraPrices = db.ExtraPrices.Where(p => p.Hotel.Id == hotelId);
                for (var j = list[i].Reservation.LeaveDate; j < list[i].Reservation.ReturnDate; j = j.AddDays(1))
                {
                    var extraPricesInDay = extraPrices.Where(p => (j >= p.StartDate && j <= p.EndDate)).ToList();
                    int percent = 0;
                    if (extraPricesInDay != null && extraPricesInDay.Count != 0)
                    {
                        foreach (var item in extraPricesInDay)
                        {
                            percent += item.AdditionalPercent;
                        }
                    }
                    for (int k = 0; k < list[i].Rooms.Count; ++k)
                    {
                        list[i].TotalPrices[k] += list[i].Rooms[k].Price + list[i].Rooms[k].Price * percent / 100 + list[i].Reservation.PersonAmount * (list[i].Hotel.FoodPrice
                            + list[i].Reservation.LeaveFlight.Price + list[i].Reservation.ReturnFlight.Price);
                        list[i].PricesWithoutFood[k] += list[i].Rooms[k].Price + list[i].Rooms[k].Price * percent / 100 + list[i].Reservation.PersonAmount *
                            (list[i].Reservation.LeaveFlight.Price + list[i].Reservation.ReturnFlight.Price);
                    }
                    list[i].MinPrice = list[i].PricesWithoutFood.Min();
                }
            }
            return list;
        }

        private List<ReservationModel> PriceRangeReservations(List<ReservationModel> list, int minPrice = 0, int maxPrice = 0)
        {
            if (list != null && list.Count != 0)
            {
                if (maxPrice != 0 && minPrice > maxPrice)
                {
                    var temp = minPrice;
                    minPrice = maxPrice;
                    maxPrice = temp;
                }
                for (int j = 0; j < list.Count; ++j)
                {
                    var item = list[j];
                    for (int i = 0; i < item.PricesWithoutFood.Count; ++i)
                    {
                        if (maxPrice != 0)
                        {
                            if (item.PricesWithoutFood[i] < minPrice || item.PricesWithoutFood[i] > maxPrice)
                            {
                                item.PricesWithoutFood.RemoveAt(i);
                                item.TotalPrices.RemoveAt(i);
                                item.Rooms.RemoveAt(i);
                                i--;
                            }
                        }
                        else
                        {
                            if (item.PricesWithoutFood[i] < minPrice)
                            {
                                item.PricesWithoutFood.RemoveAt(i);
                                item.TotalPrices.RemoveAt(i);
                                item.Rooms.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    if (item.PricesWithoutFood.Count == 0)
                    {
                        list.Remove(item);
                        j--;
                    }
                    else
                    {
                        list[j].MinPrice = list[j].PricesWithoutFood.Min();
                    }
                }

            }
            return list;
        }

        private List<Flight> FlightsSearch(int cityFromId, int cityToId, DateTime departureDay, int peopleNumber, bool isLeaveFlight, ref string error)
        {
            var flightsQuery = db.Flights.Where(p => (p.DepartureAirport.CityId == cityFromId && p.ArrivalAirport.CityId == cityToId));
            List<Flight> flights = flightsQuery.ToList<Flight>(); //all flights that are between inputed cities

            //Check if these flights are on date, which customer selected
            //If some flight has no departure on selected date, then this flight will be removed from list
            for (int i = 0; i < flights.Count; ++i)
            {
                var temp = flights[i].Id;
                IEnumerable<Schedule> schedule = db.Schedule.Where(p => (p.IsDeleted == false && p.FlightId == temp)); ;
                bool check = false;
                foreach (var item in schedule.ToList())
                {
                    if (item.DayOfWeek == departureDay.DayOfWeek)
                    {
                        check = true;
                        break;
                    }
                }

                if (check == false)
                {
                    flights.RemoveAt(i);
                }
            }

            flights = CheckingFlightsSeats(flights, peopleNumber, departureDay, isLeaveFlight);

            if (flights == null || flights.Count == 0)
            {
                error = String.Format("Sorry, but we cant find flight from {0} to {1}! Try to change some information.", db.Cities.Find(cityFromId).Name, db.Cities.Find(cityToId).Name);
                return flights;
            }
            return flights;
        }

        private List<Flight> CheckingFlightsSeats(List<Flight> flights, int peopleNumber, DateTime departureDay, bool isLeaveFlights)
        {
            for (int i = 0; i < flights.Count; ++i)
            {
                int flightId = flights[i].Id;

                IEnumerable<int> temp = db.Reservations.Where(r => (r.IsDeleted == false &&
                ((r.LeaveFlightId == flightId && r.LeaveDate == departureDay)
                || (r.ReturnFlightId == flightId && r.ReturnDate == departureDay))))
                .Select(r => r.PersonAmount).ToList();

                int reservedSeats = 0;
                foreach (var item in temp)
                {
                    reservedSeats += item;
                }
                if (flights[i].Airplane.SeatsAmount - reservedSeats < peopleNumber)
                {
                    flights.RemoveAt(i);
                }
            }
            return flights;
        }

        private List<Room> HotelsSearch(int cityId, DateTime departureDate, DateTime returnDate, int peopleNumber, ref string error, int hotelId = 0, int foodTypeId = 0,
            List<int> facilities = null, List<int> stars = null)
        {
            List<Room> rooms = new List<Room>();
            var hotelsQuery = db.Hotels.Where(h => (h.IsDeleted == false && h.City.Id == cityId));
            if (hotelsQuery == null || hotelsQuery.Count() == 0)
            {
                error = String.Format("Sorry, but we cant find hotel in {0}! Try to change some information.", db.Cities.Find(cityId).Name);
                return rooms;
            }
            if (hotelId != 0)
            {
                hotelsQuery = hotelsQuery.Where(p => p.Id == hotelId);
            }
            if (foodTypeId != 0)
            {
                hotelsQuery = hotelsQuery.Where(p => p.FoodTypeId == foodTypeId);
                if (hotelsQuery == null || hotelsQuery.Count() == 0)
                {
                    error = String.Format("Sorry, but we cant find hotel in {0} with {1} food type! Try to change some information.", db.Cities.Find(cityId).Name, db.FoodTypes.Find(foodTypeId).Name);
                    return rooms;
                }
            }
            List<Hotel> hotels = hotelsQuery.ToList<Hotel>();

            for (int i = 0; i < hotels.Count; ++i)
            {
                var index = hotels[i].Id;
                IEnumerable<Room> roomsQuery = db.Rooms.Where(p => ((p.HotelId == index) && (p.IsDeleted == false))); ;
                foreach (Room item in roomsQuery.ToList())
                {
                    var t = db.Reservations.Where(r => r.RoomId == item.Id && r.LeaveDate >= departureDate && r.ReturnDate <= returnDate && r.IsDeleted == false);
                    if (t.ToList().Count == 0)
                    {
                        //Check if room with the same room type and hotelId is already in list 
                        bool checkRoomType = false;
                        foreach (var room in rooms)
                        {
                            if (room.HotelId == item.HotelId && room.RoomTypeId == item.RoomTypeId)
                            {
                                checkRoomType = true;
                                break;
                            }
                        }
                        if (checkRoomType == false)
                        {
                            rooms.Add(item);
                        }
                    }
                }
            }
            if (rooms == null || rooms.Count == 0)
            {
                error = String.Format("Sorry, but we cant find available rooms in all hotels in {0}! Try to change some information.", db.Cities.Find(cityId).Name);
                return rooms;
            }

            int maxPerson = rooms.OrderByDescending(r => r.MaxPeople).FirstOrDefault().MaxPeople;
            rooms = rooms.Where(r => r.MaxPeople >= peopleNumber).ToList();
            if (rooms.Count == 0)
            {
                error = String.Format("Sorry, but we can book only one tour in one booking. We cant find available room for {0} persons. We can find room for maximum {1} persons!", peopleNumber, maxPerson);
                return rooms;
            }

            return rooms;
        }


    }
}
