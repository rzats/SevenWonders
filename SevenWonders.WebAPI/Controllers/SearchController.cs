//using SevenWonders.DAL.Context;
//using SevenWonders.WebAPI.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace SevenWonders.WebAPI.Controllers
//{
//    public class SearchController : ApiController
//    {
//        SevenWondersContext db = new SevenWondersContext();
//        public class ReservationModelJson
//        {
//            public int PeopleCount;
//            public String LeaveFlight;
//            public String LeaveDate;
//            public String ReturnFlight;
//            public String ReturnDate;
//            public String FoodType;
//            public int AvailableRoom;
//            public String HotelLocation;
//        }

//        [Authorize]
//        private int FindCustomerId()
//        {
//            int userId = User.Identity.GetUserId<int>();
//            string userEmail = db.Users.Where(p => p.Id == userId).Select(p => p.Email).FirstOrDefault();
//            return db.Customers.Where(p => p.Email == userEmail).Select(p => p.Id).FirstOrDefault();

//        }

//        public List<City> FillCity(int countryId)
//        {
//            var cities = db.Cities.Where(c => (c.IsDeleted == false && c.Country.Id == countryId))
//                .Select(p => new SelectListItem
//                {
//                    Text = p.Name,
//                    Value = p.Id.ToString()
//                }).OrderBy(p => p.Text);

//            return Json(cities, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult FillHotel(int cityId)
//        {
//            var hotels = db.Hotels.Where(c => (c.IsDeleted == false && c.City.Id == cityId))
//                           .Select(p => new SelectListItem
//                           {
//                               Text = p.Name,
//                               Value = p.Id.ToString()
//                           }).OrderBy(p => p.Text);

//            return Json(hotels, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public ActionResult SearchTours(SearchModel model)
//        {
//            string error = "Sorry, but there are no tours, which match to your requirements. Try to change some information.";
//            List<Flight> leaveFlights = FlightsSearch(model.CityFrom, model.CityTo, model.DapartureDay, model.PeopleNumber, true, ref error);
//            List<Flight> returnFlights = FlightsSearch(model.CityTo, model.CityFrom, model.DapartureDay.AddDays(model.Duration), model.PeopleNumber, false, ref error);
//            List<Room> rooms = HotelsSearch(model.CityTo, model.DapartureDay, model.DapartureDay.AddDays(model.Duration), model.PeopleNumber, ref error, model.Hotel, model.FoodType);
//            ViewBag.ErrorMessage = error;

//            List<Reservation> reservations = new List<Reservation>();
//            for (int i = 0; i < leaveFlights.Count; ++i)
//                for (int j = 0; j < returnFlights.Count; ++j)
//                    for (int k = 0; k < rooms.Count; ++k)
//                    {
//                        Reservation tempReservation = new Reservation();
//                        tempReservation.LeaveDate = model.DapartureDay;
//                        tempReservation.ReturnDate = model.DapartureDay.AddDays(model.Duration);
//                        tempReservation.LeaveFlight = leaveFlights[i];
//                        tempReservation.PersonAmount = model.PeopleNumber;
//                        tempReservation.ReturnFlight = returnFlights[j];
//                        tempReservation.Room = rooms[k];
//                        reservations.Add(tempReservation);
//                    }

//            List<ReservationModel> resModels = new List<ReservationModel>();
//            for (int i = 0; i < reservations.Count; ++i)
//            {
//                var appropriateReservation = resModels.Where(p => (p.Hotel.Id == reservations[i].Room.HotelId
//                && p.Reservation.LeaveFlight.Id == reservations[i].LeaveFlight.Id
//                && p.Reservation.ReturnFlight.Id == reservations[i].ReturnFlight.Id)).FirstOrDefault();

//                if (appropriateReservation != null)
//                {
//                    bool flag = false;
//                    foreach (var item in appropriateReservation.Rooms)
//                    {
//                        //Check if there are such room in list
//                        //it means that these reservations have different flights 
//                        if (item.Id == reservations[i].RoomId)
//                        {
//                            ReservationModel tempReservModel = new ReservationModel();
//                            tempReservModel.Duration = model.Duration;
//                            tempReservModel.Reservation = reservations[i];
//                            tempReservModel.Rooms.Add(reservations[i].Room);
//                            tempReservModel.Hotel = reservations[i].Room.Hotel;
//                            tempReservModel.TotalPrices.Add(0);
//                            tempReservModel.PricesWithoutFood.Add(0);
//                            tempReservModel.FoodInclude.Add(true);
//                            tempReservModel.Food = true;
//                            resModels.Add(tempReservModel);
//                            flag = true;
//                            break;
//                        }
//                    }
//                    //If there  are not such room in list, then we will add it
//                    if (flag == false)
//                    {
//                        appropriateReservation.Rooms.Add(reservations[i].Room);
//                        appropriateReservation.TotalPrices.Add(0);
//                        appropriateReservation.PricesWithoutFood.Add(0);
//                        appropriateReservation.FoodInclude.Add(true);
//                        appropriateReservation.Food = true;

//                    }
//                }
//                //if list doesn't contain item with such hotel
//                else
//                {
//                    ReservationModel tempReservModel = new ReservationModel();
//                    tempReservModel.Duration = model.Duration;
//                    tempReservModel.Reservation = reservations[i];
//                    tempReservModel.Rooms.Add(reservations[i].Room);
//                    tempReservModel.Hotel = reservations[i].Room.Hotel;
//                    tempReservModel.TotalPrices.Add(0);
//                    tempReservModel.PricesWithoutFood.Add(0);
//                    tempReservModel.FoodInclude.Add(true);
//                    tempReservModel.Food = true;
//                    resModels.Add(tempReservModel);
//                }
//            }
//            resModels = PricesCalculating(resModels);

//            if (model.PriceFrom != 0 || model.PriceTo != 0)
//                resModels = PriceRangeReservations(resModels, model.PriceFrom, model.PriceTo);

//            return View(resModels);
//        }

//        private List<ReservationModel> PricesCalculating(List<ReservationModel> list)
//        {
//            for (int i = 0; i < list.Count; ++i)
//            {
//                int hotelId = list[i].Hotel.Id;
//                var extraPrices = db.ExtraPrices.Where(p => p.Hotel.Id == hotelId);
//                for (var j = list[i].Reservation.LeaveDate; j < list[i].Reservation.ReturnDate; j = j.AddDays(1))
//                {
//                    var extraPricesInDay = extraPrices.Where(p => (j >= p.StartDate && j <= p.EndDate)).ToList();
//                    int percent = 0;
//                    if (extraPricesInDay != null && extraPricesInDay.Count != 0)
//                    {
//                        foreach (var item in extraPricesInDay)
//                        {
//                            percent += item.AdditionalPercent;
//                        }
//                    }
//                    for (int k = 0; k < list[i].Rooms.Count; ++k)
//                    {
//                        list[i].TotalPrices[k] += list[i].Rooms[k].Price + list[i].Rooms[k].Price * percent / 100 + list[i].Reservation.PersonAmount * (list[i].Hotel.FoodPrice
//                            + list[i].Reservation.LeaveFlight.Price + list[i].Reservation.ReturnFlight.Price);
//                        list[i].PricesWithoutFood[k] += list[i].Rooms[k].Price + list[i].Rooms[k].Price * percent / 100 + list[i].Reservation.PersonAmount *
//                            (list[i].Reservation.LeaveFlight.Price + list[i].Reservation.ReturnFlight.Price);
//                    }
//                    list[i].MinPrice = list[i].PricesWithoutFood.Min();
//                }
//            }
//            return list;
//        }

//        private List<ReservationModel> PriceRangeReservations(List<ReservationModel> list, int minPrice = 0, int maxPrice = 0)
//        {
//            if (list != null && list.Count != 0)
//            {
//                if (maxPrice != 0 && minPrice > maxPrice)
//                {
//                    var temp = minPrice;
//                    minPrice = maxPrice;
//                    maxPrice = temp;
//                }
//                for (int j = 0; j < list.Count; ++j)
//                {
//                    var item = list[j];
//                    for (int i = 0; i < item.PricesWithoutFood.Count; ++i)
//                    {
//                        if (maxPrice != 0)
//                        {
//                            if (item.PricesWithoutFood[i] < minPrice || item.PricesWithoutFood[i] > maxPrice)
//                            {
//                                item.PricesWithoutFood.RemoveAt(i);
//                                item.TotalPrices.RemoveAt(i);
//                                item.Rooms.RemoveAt(i);
//                                i--;
//                            }
//                        }
//                        else
//                        {
//                            if (item.PricesWithoutFood[i] < minPrice)
//                            {
//                                item.PricesWithoutFood.RemoveAt(i);
//                                item.TotalPrices.RemoveAt(i);
//                                item.Rooms.RemoveAt(i);
//                                i--;
//                            }
//                        }
//                    }

//                    if (item.PricesWithoutFood.Count == 0)
//                    {
//                        list.Remove(item);
//                        j--;
//                    }
//                    else
//                    {
//                        list[j].MinPrice = list[j].PricesWithoutFood.Min();
//                    }
//                }

//            }
//            return list;
//        }

//        private List<Flight> FlightsSearch(int cityFromId, int cityToId, DateTime departureDay, int peopleNumber, bool isLeaveFlight, ref string error)
//        {
//            var flightsQuery = db.Flights.Where(p => (p.DepartureAirport.CityId == cityFromId && p.ArrivalAirport.CityId == cityToId));
//            List<Flight> flights = flightsQuery.ToList<Flight>(); //all flights that are between inputed cities

//            //Check if these flights are on date, which customer selected
//            //If some flight has no departure on selected date, then this flight will be removed from list
//            for (int i = 0; i < flights.Count; ++i)
//            {
//                var temp = flights[i].Id;
//                IEnumerable<Schedule> schedule = db.Schedule.Where(p => (p.IsDeleted == false && p.FlightId == temp)); ;
//                bool check = false;
//                foreach (var item in schedule.ToList())
//                {
//                    if (item.DayOfWeek == departureDay.DayOfWeek)
//                    {
//                        check = true;
//                        break;
//                    }
//                }

//                if (check == false)
//                {
//                    flights.RemoveAt(i);
//                }
//            }

//            flights = CheckingFlightsSeats(flights, peopleNumber, departureDay, isLeaveFlight);

//            if (flights == null || flights.Count == 0)
//            {
//                error = String.Format("Sorry, but we cant find flight from {0} to {1}! Try to change some information.", db.Cities.Find(cityFromId).Name, db.Cities.Find(cityToId).Name);
//                return flights;
//            }
//            return flights;
//        }

//        private List<Flight> CheckingFlightsSeats(List<Flight> flights, int peopleNumber, DateTime departureDay, bool isLeaveFlights)
//        {
//            for (int i = 0; i < flights.Count; ++i)
//            {
//                int flightId = flights[i].Id;

//                IEnumerable<int> temp = db.Reservations.Where(r => (r.IsDeleted == false &&
//                ((r.LeaveFlightId == flightId && r.LeaveDate == departureDay)
//                || (r.ReturnFlightId == flightId && r.ReturnDate == departureDay))))
//                .Select(r => r.PersonAmount).ToList();

//                int reservedSeats = 0;
//                foreach (var item in temp)
//                {
//                    reservedSeats += item;
//                }
//                if (flights[i].Airplane.SeatsAmount - reservedSeats < peopleNumber)
//                {
//                    flights.RemoveAt(i);
//                }
//            }
//            return flights;
//        }

//        private List<Room> HotelsSearch(int cityId, DateTime departureDate, DateTime returnDate, int peopleNumber, ref string error, int hotelId = 0, int foodTypeId = 0,
//            List<int> facilities = null, List<int> stars = null)
//        {
//            List<Room> rooms = new List<Room>();
//            var hotelsQuery = db.Hotels.Where(h => (h.IsDeleted == false && h.City.Id == cityId));
//            if (hotelsQuery == null || hotelsQuery.Count() == 0)
//            {
//                error = String.Format("Sorry, but we cant find hotel in {0}! Try to change some information.", db.Cities.Find(cityId).Name);
//                return rooms;
//            }
//            if (hotelId != 0)
//            {
//                hotelsQuery = hotelsQuery.Where(p => p.Id == hotelId);
//            }
//            if (foodTypeId != 0)
//            {
//                hotelsQuery = hotelsQuery.Where(p => p.FoodTypeId == foodTypeId);
//                if (hotelsQuery == null || hotelsQuery.Count() == 0)
//                {
//                    error = String.Format("Sorry, but we cant find hotel in {0} with {1} food type! Try to change some information.", db.Cities.Find(cityId).Name, db.FoodTypes.Find(foodTypeId).Name);
//                    return rooms;
//                }
//            }
//            List<Hotel> hotels = hotelsQuery.ToList<Hotel>();

//            for (int i = 0; i < hotels.Count; ++i)
//            {
//                var index = hotels[i].Id;
//                IEnumerable<Room> roomsQuery = db.Rooms.Where(p => ((p.HotelId == index) && (p.IsDeleted == false))); ;
//                foreach (Room item in roomsQuery.ToList())
//                {
//                    var t = db.Reservations.Where(r => r.RoomId == item.Id && r.LeaveDate >= departureDate && r.ReturnDate <= returnDate && r.IsDeleted == false);
//                    if (t.ToList().Count == 0)
//                    {
//                        //Check if room with the same room type and hotelId is already in list 
//                        bool checkRoomType = false;
//                        foreach (var room in rooms)
//                        {
//                            if (room.HotelId == item.HotelId && room.RoomTypeId == item.RoomTypeId)
//                            {
//                                checkRoomType = true;
//                                break;
//                            }
//                        }
//                        if (checkRoomType == false)
//                        {
//                            rooms.Add(item);
//                        }
//                    }
//                }
//            }
//            if (rooms == null || rooms.Count == 0)
//            {
//                error = String.Format("Sorry, but we cant find available rooms in all hotels in {0}! Try to change some information.", db.Cities.Find(cityId).Name);
//                return rooms;
//            }

//            int maxPerson = rooms.OrderByDescending(r => r.MaxPeople).FirstOrDefault().MaxPeople;
//            rooms = rooms.Where(r => r.MaxPeople >= peopleNumber).ToList();
//            if (rooms.Count == 0)
//            {
//                error = String.Format("Sorry, but we can book only one tour in one booking. We cant find available room for {0} persons. We can find room for maximum {1} persons!", peopleNumber, maxPerson);
//                return rooms;
//            }

//            return rooms;
//        }

//        public ActionResult HotelInfo(int id)
//        {
//            return PartialView(db.Hotels.Where(p => p.Id == id).FirstOrDefault());
//        }

//        public ActionResult RoomInfo(int id)
//        {
//            return PartialView(db.Rooms.Where(p => p.Id == id).FirstOrDefault());
//        }

//        public ActionResult FlightsInfo(int leaveFlightId, int returnFlightId, DateTime leaveDate, DateTime returnDate)
//        {
//            FlightsInfoModel model = new FlightsInfoModel();
//            model.LeaveDate = leaveDate;
//            model.ReturnDate = returnDate;
//            model.LeaveFlight = db.Flights.Where(p => p.Id == leaveFlightId).FirstOrDefault();
//            model.ReturnFlight = db.Flights.Where(p => p.Id == returnFlightId).FirstOrDefault();
//            model.LeaveSchedule = db.Schedule.Where(s => (s.Flight.Id == model.LeaveFlight.Id) && (s.DayOfWeek == leaveDate.DayOfWeek)).FirstOrDefault();
//            model.ReturnSchedule = db.Schedule.Where(s => (s.Flight.Id == model.ReturnFlight.Id) && (s.DayOfWeek == returnDate.DayOfWeek)).FirstOrDefault();
//            return PartialView(model);
//        }

//        public ActionResult HotelsReviews(int id)
//        {
//            return PartialView(db.Feedbacks.Where(h => h.Hotel.Id == id).ToList().OrderBy(p => p.CreationDate).Reverse().ToList());
//        }

//        [Authorize(Roles = "customer")]
//        public ActionResult ConfirmTour(int leaveFlightId, int returnFlightId, DateTime leaveDate, DateTime returnDate, int personsAmount, int roomId, bool foodInclude, decimal totalPrice, int duration)
//        {
//            return View(CreateBookInfo(leaveFlightId, returnFlightId, leaveDate, returnDate, personsAmount, roomId, foodInclude, totalPrice));
//        }

//        public ActionResult GeneratePDF(int leaveFlightId, int returnFlightId, DateTime leaveDate, DateTime returnDate, int personsAmount, int roomId, bool foodInclude, decimal totalPrice)
//        {
//            ConfirmTour data = new ConfirmTour();

//            int userId = User.Identity.GetUserId<int>();
//            string userEmail = db.Users.Where(p => p.Id == userId).Select(p => p.Email).FirstOrDefault();
//            data.Customer = db.Customers.Where(p => p.Email == userEmail).FirstOrDefault();
//            data.DateTime = DateTime.Now;
//            data.BookInfo = CreateBookInfo(leaveFlightId, returnFlightId, leaveDate, returnDate, personsAmount, roomId, foodInclude, totalPrice);
//            data.BookInfo.TourId = db.Tours.OrderByDescending(u => u.Id).FirstOrDefault().Id;

//            string fileName = "BookingConfirmation" + data.BookInfo.TourId;
//            Response.ContentType = "application/pdf";
//            Response.AppendHeader(
//              "Content-Disposition",
//              "attachment; filename=BookingConfirmation.pdf"
//            );
//            string toPdf = RenderViewToString("DocumentTourInfo", data);
//            using (Document document = new Document())
//            {
//                using (var writer = PdfWriter.GetInstance(document, Response.OutputStream))
//                {
//                    document.Open();
//                    string example_css = System.IO.File.ReadAllText(Server.MapPath(@"~/Content/bootstrap.css"));
//                    using (var msCss = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(example_css)))
//                    {
//                        using (var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(toPdf)))
//                        {
//                            //Parse the HTML
//                            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, msHtml, msCss);
//                        }
//                    }
//                }
//            }

//            return View("ConfirmTour", CreateBookInfo(leaveFlightId, returnFlightId, leaveDate, returnDate, personsAmount, roomId, foodInclude, totalPrice));
//        }

//        public string RenderViewToString(string viewName, object model)
//        {
//            ViewData.Model = model;
//            using (var sw = new StringWriter())
//            {
//                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
//                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
//                viewResult.View.Render(viewContext, sw);
//                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
//                return sw.GetStringBuilder().ToString();
//            }
//        }

//        public BookInfo CreateBookInfo(int leaveFlightId, int returnFlightId, DateTime leaveDate, DateTime returnDate, int personsAmount, int roomId, bool foodInclude, decimal totalPrice)
//        {
//            int duration = Convert.ToInt32(Math.Ceiling((double)(returnDate - leaveDate).Hours / 24));

//            int userId = User.Identity.GetUserId<int>();
//            string userEmail = db.Users.Where(p => p.Id == userId).Select(p => p.Email).FirstOrDefault();
//            Customer customer = db.Customers.Where(p => p.Email == userEmail).FirstOrDefault();
//            Reservation reservation = new Reservation();
//            reservation.LeaveDate = leaveDate;
//            reservation.ReturnDate = returnDate;
//            reservation.LeaveFlight = db.Flights.Where(p => p.Id == leaveFlightId).FirstOrDefault();
//            reservation.ReturnFlight = db.Flights.Where(p => p.Id == returnFlightId).FirstOrDefault();
//            reservation.PersonAmount = personsAmount;
//            reservation.Room = db.Rooms.Where(r => r.Id == roomId).FirstOrDefault();
//            reservation.WithoutFood = !foodInclude;

//            BookInfo model = new BookInfo();
//            model.Discount = customer.Discount;
//            if (foodInclude == true)
//            {
//                model.TotalPrice = totalPrice;
//            }
//            else
//            {
//                model.TotalPrice = totalPrice - personsAmount * reservation.Room.Hotel.FoodPrice * duration;
//            }
//            model.Flights.LeaveFlight = reservation.LeaveFlight;
//            model.Flights.ReturnFlight = reservation.ReturnFlight;
//            model.Flights.LeaveSchedule = db.Schedule.Where(p => (p.Flight.Id == leaveFlightId && p.DayOfWeek == leaveDate.DayOfWeek)).FirstOrDefault();
//            model.Flights.ReturnSchedule = db.Schedule.Where(p => (p.Flight.Id == returnFlightId && p.DayOfWeek == returnDate.DayOfWeek)).FirstOrDefault();
//            model.Reservation = reservation;
//            model.CustomerId = customer.Id;

//            return model;
//        }

//        [Authorize(Roles = "customer")]
//        public ActionResult BookTour(int leaveFlightId, int returnFlightId, DateTime leaveDate, DateTime returnDate, int personsAmount, int roomId, bool foodInclude, decimal totalPrice, int customerId)
//        {
//            Reservation reservation = new Reservation();
//            reservation.LeaveFlight = db.Flights.Where(p => p.Id == leaveFlightId).FirstOrDefault();
//            reservation.ReturnFlight = db.Flights.Where(p => p.Id == returnFlightId).FirstOrDefault();
//            Schedule schedule = db.Schedule.Where(p => (p.Flight.Id == leaveFlightId && p.DayOfWeek == leaveDate.DayOfWeek)).FirstOrDefault();
//            DateTime date = new DateTime(leaveDate.Year, leaveDate.Month, leaveDate.Day, schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
//            reservation.LeaveDate = date;
//            schedule = db.Schedule.Where(p => (p.Flight.Id == returnFlightId && p.DayOfWeek == returnDate.DayOfWeek)).FirstOrDefault();
//            date = new DateTime(returnDate.Year, returnDate.Month, returnDate.Day, schedule.DepartureTime.Hour, schedule.DepartureTime.Minute, schedule.DepartureTime.Second);
//            reservation.ReturnDate = date;
//            reservation.PersonAmount = personsAmount;
//            reservation.Room = db.Rooms.Where(r => r.Id == roomId).FirstOrDefault();
//            reservation.WithoutFood = !foodInclude;
//            db.Reservations.Add(reservation);
//            db.SaveChanges();
//            Tour tour = new Tour();
//            tour.CreationDate = DateTime.Now.ToUniversalTime();
//            tour.Customer = db.Customers.Find(customerId);
//            tour.IsDeleted = false;
//            tour.TourState = db.TourStates.Find(1);
//            tour.TotalPrice = totalPrice;
//            tour.Reservation = reservation;

//            Manager manager = tour.Reservation.Room.Hotel.City.Country.Manager;
//            db.Tours.Add(tour);
//            db.SaveChanges();

//            Utils utils = new Utils();
//            string body = string.Format("Dear " + tour.Customer.FirstName + " " + tour.Customer.LastName + "," +
//                   "<br/>thank you for your booking. You have successfully booked tour in <b>{0}</b>. <br/> Leave date: <b>{1}</b>. <br/>Return date: <b>{2}</b>.<br/>Total price: <b>${3}</b>.<br/> Please make payment for this tour in 48 hours, otherwise we will cancel this booking.<br/>You can find all details about this tour in your Cabinet on our site.<br/>Have a nice trip!<br/><br/>Best regards, <br/>7wonders.com",
//                   tour.Reservation.Room.Hotel.Name, tour.Reservation.LeaveDate.ToLongDateString(), tour.Reservation.ReturnDate.ToLongDateString(), tour.TotalPrice);
//            var message = utils.GenerateMessage(tour.Customer.Email, "7wonders", "Tours booking", body, true);
//            utils.SendEmail(message);

//            if (manager != null)
//            {
//                manager.Tour.Add(tour);
//                //Send email to manager
//                body = string.Format("<h2>New booking</h2><br/><p>Customer: <b>{0}</b> <br/> Country: <b>{1}</b><br/>City: <b>{2}</b><br/> Hotel: <b>{3}</b><br/> Leave date: <b>{4}</b> <br/>Return date: <b>{5}</b><br/>Total price: <b>${6}</b> <br/>Customers email: <b>{7}</b> <br/>Customers phone number: <b>{8}</b>",
//                       tour.Customer.FirstName + tour.Customer.LastName, tour.Reservation.Room.Hotel.City.Country.Name, tour.Reservation.Room.Hotel.City.Name, tour.Reservation.Room.Hotel.Name, tour.Reservation.LeaveDate.ToLongDateString(), tour.Reservation.ReturnDate.ToLongDateString(), tour.TotalPrice, tour.Customer.Email, tour.Customer.PhoneNumber);
//                message = utils.GenerateMessage(manager.Email, "7wonders", "New booking", body, true);
//                utils.SendEmail(message);
//            }

//            return View();
//        }

//        public ActionResult SuccessMessage()
//        {
//            return RedirectToAction("Index");
//        }


//        [HttpGet]
//        public ActionResult GetHotelDetailModal(int hotelId)
//        {
//            Hotel model = db.Hotels.FirstOrDefault(Hotel => Hotel.Id == hotelId);
//            return PartialView("~/Views/Home/Partials/HotelDetailsModal.cshtml", model);
//        }

//        [HttpGet]
//        public ActionResult GetRoomDetailModal(int roomId)
//        {
//            Room model = db.Rooms.FirstOrDefault(room => room.Id == roomId);
//            return PartialView("~/Views/Home/Partials/RoomDetailsModal.cshtml", model);
//        }

//        [HttpGet]
//        public ActionResult GetFlightDetailModal(int leaveFlightId, int returnFlightId, string leaveDate, string returnDate)
//        {
//            var leaveD = DateTime.Parse(leaveDate);
//            var returnD = DateTime.Parse(returnDate);

//            FlightsInfoModel model = new FlightsInfoModel();
//            model.LeaveDate = leaveD;
//            model.ReturnDate = returnD;
//            model.LeaveFlight = db.Flights.Where(p => p.Id == leaveFlightId).FirstOrDefault();
//            model.ReturnFlight = db.Flights.Where(p => p.Id == returnFlightId).FirstOrDefault();
//            model.LeaveSchedule = db.Schedule.Where(s => (s.Flight.Id == model.LeaveFlight.Id) && (s.DayOfWeek == leaveD.DayOfWeek)).FirstOrDefault();
//            model.ReturnSchedule = db.Schedule.Where(s => (s.Flight.Id == model.ReturnFlight.Id) && (s.DayOfWeek == returnD.DayOfWeek)).FirstOrDefault();
//            return PartialView("~/Views/Home/Partials/FlightDetailsModal.cshtml", model);
//        }

//        [HttpGet]
//        public ActionResult GetHotelReviewsModal(int hotelId)
//        {
//            Hotel model = db.Hotels.FirstOrDefault(Hotel => Hotel.Id == hotelId);
//            return PartialView("~/Views/Home/Partials/HotelReviewsModal.cshtml", model);
//        }
//    }
//}
