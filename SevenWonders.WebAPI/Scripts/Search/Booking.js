(function ($) {
    var app1 = $.sammy('#hotelInfo', function () {
        this.get('#/booking', function () {
            this.partial('/Views/Hotels/HotelShortInfo.html');
        });
    });
    var app2 = $.sammy('#flightInfo', function () {
        this.get('#/booking', function () {
            this.partial('/Views/Flights/FlightShortInfo.html');
        });
    });
    var app3 = $.sammy('#roomInfo', function () {
        this.get('#/booking', function () {
            this.partial('/Views/Hotels/RoomShortInfo.html');
        });
    });
    app1.run('#/booking');
    app2.run('#/booking');
    app3.run('#/booking');
})(jQuery);

function parseUrl() {
    var query = location.href;
    var pos = query.indexOf("?");
    if (pos == -1) return [];
    query = location.href.substr(pos + 1);

    var result = {};
    query.split("&").forEach(function (part) {
        var item = part.split("=");
        result[item[0]] = decodeURIComponent(item[1]);
    });
    return result;
}

function RoomViewModel(tour) {
    var self = this;
    var tour = tour;
    self.Id = ko.observable();
    self.RoomType = ko.observable();
    self.MaxPeople = ko.observable();
    self.WindowView = ko.observable();
    self.Price = ko.observable();
    self.Equipments = ko.observableArray([]);
    self.RoomPhotos = ko.observableArray([]);

    self.includeFood = ko.observable();
    self.priceForRoom = ko.computed(function () {
        if (self.includeFood())
            return tour.People() * (tour.Flights().LeaveFlightPrice + tour.Flights().ReturnFlightPrice) + tour.Duration() * (self.Price() + tour.Hotel().FoodPrice * tour.People());           
        else return tour.People() * (tour.Flights().LeaveFlightPrice + tour.Flights().ReturnFlightPrice) + tour.Duration() * self.Price();
    }, self);

    self.updateModel = function (room) {
        self.Id(room.Id);
        self.RoomType(room.RoomType);
        self.MaxPeople(room.MaxPeople);
        self.WindowView(room.WindowView);
        self.Price(room.Price);
        self.Equipments(room.Equipments);
        self.RoomPhotos(room.RoomsPhotos);
        self.includeFood(true);
    }
    self.showRoomDetail = function () {
        roomShortInfoViewModel.updateViewModel(self)
        $('#roomInfoModal').modal();
    }
    self.showBookModal = function () {
        if (ToursListViewModel.isCustomer()) {
            $('#bookingConfirmationModal').modal();
        }
    }
    self.bookTour = function () {
        var model = {
            PersonAmount: tour.People(),
            LeaveDate: tour.DepartureDate(),
            Duration: tour.Duration(),
            RoomId: self.Id(),
            LeaveScheduleId: tour.DepartureScheduleId(),
            ReturnScheduleId: tour.ArrivalScheduleId(),
            WithoutFood: self.includeFood()
        };
        $.ajax("../api/Search/BookTour", {
            type: "post",
            data: JSON.stringify(model),
            contentType: "application/json",
            success: function (result) {
                $('#bookingConfirmationModal').modal('hide');
                window.location.href = "#/cabinet";
            }
        });
    }
}
function TourViewModel() {
    var self = this;
    self.People = ko.observable();
    self.Duration = ko.observable();
    self.DepartureDate = ko.observable();
    self.ArrivaleDate = ko.observable();
    self.DepartureScheduleId = ko.observable();
    self.ArrivalScheduleId = ko.observable();
    self.Hotel = ko.observable();
    self.Flights = ko.observable();
    self.Rooms = ko.observableArray([]);

    self.minPrice = ko.observable();
    self.updateModel = function (tour) {
        self.People(tour.People);
        self.Duration(tour.Duration);
        self.DepartureDate(tour.DepartureDate);
        self.ArrivaleDate(tour.ArrivaleDate);
        self.DepartureScheduleId(tour.DepartureScheduleId);
        self.ArrivalScheduleId(tour.ArrivalScheduleId);
        self.Hotel(tour.Hotel);
        self.Flights(tour.Flights);

        var minRoomPrice = tour.Rooms[0].Price;
        tour.Rooms.forEach(function (item, i, result) {
            var room = new RoomViewModel(self);
            room.updateModel(item);
            self.Rooms.push(room);

            if (room.Price < minRoomPrice)
                minRoomPrice = room.Price;
        });

        self.minPrice(self.People() * (self.Flights().LeaveFlightPrice + self.Flights().ReturnFlightPrice) + self.Duration() * minRoomPrice);
    }

    self.showHotelDetail = function () {
        hotelShortInfoViewModel.updateViewModel(self.Hotel())
        $('#hotelInfoModal').modal();
    }
    self.showFlightDetail = function () {       
        flightShortInfoViewModel.updateViewModel(self.Flights())
        $('#flightInfoModal').modal();
    };
}
function ToursListViewModel() {
    var self = this;
    self.tours = ko.observableArray([]);
    self.isCustomer = ko.observable();
    self.notFound = ko.observable();

    self.loadTours = function () {
        var parameters = parseUrl();
        $.ajax("../api/Search/GetTours", {
            type: "get",
            data: parameters,
            contentType: "application/json",
            success: function (result) 
            {
                if (result === "") {
                    self.notFound(true);
                }
                else {
                    self.notFound(false);
                    self.tours([]);
                    self.isCustomer(result.isCustomer);
                    result.tours.forEach(function (item, i, result) {
                        var bits = (item.DepartureDate).split(/\D/);
                        item.DepartureDate = bits[0] + "-" + bits[1] + "-" + bits[2];
                        bits = (item.ArrivaleDate).split(/\D/);
                        item.ArrivaleDate = bits[0] + "-" + bits[1] + "-" + bits[2];

                        var tour = new TourViewModel();
                        tour.updateModel(item);
                        self.tours.push(tour);
                    });
                }
            }
        });
    }
    self.loadTours();
}

var ToursListViewModel = new ToursListViewModel();
ko.applyBindings(ToursListViewModel, document.getElementById("toursForSearchTable"));

function HotelBind() {
    ko.applyBindings(hotelShortInfoViewModel, document.getElementById("hotelInfoModal"));
}
function FlightBind() {
    ko.applyBindings(flightShortInfoViewModel, document.getElementById("flightInfoModal"));
}
function RoomBind() {
    ko.applyBindings(roomShortInfoViewModel, document.getElementById("roomInfoModal"));
}