function parseUrl() {
    debugger;
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

function RoomViewModel(room) {
    var self = this;
    self.Id = room.Id;
    self.RoomType = room.RoomType;
    self.MaxPeople = room.MaxPeople;
    self.WindowView = room.WindowView;
    self.Price = room.Price;
    self.Equipments = room.Equipments;
    self.RoomsPhotos = room.RoomsPhotos;
}

function TourViewModel(tour) {
    var self = this;
    self.People = tour.People;
    self.DepartureDate = tour.DepartureDate;
    self.ArrivaleDate = tour.ArrivaleDate;
    self.DepartureScheduleId = tour.DepartureScheduleId;
    self.ArrivalScheduleId = tour.ArrivalScheduleId;
    self.Hotel = tour.Hotel;
    self.Flights = tour.Flights;
    self.Rooms = tour.Rooms;
}

function ToursListViewModel() {
    var self = this;
    self.tours = ko.observableArray([]);
    self.loadTours = function () {
        debugger;
        var parameters = parseUrl();

        $.ajax("../api/Search/GetTours", {
            type: "get",
            data: parameters,
            contentType: "application/json",
            success: function (result) 
            {

                self.tours([]);
                result.forEach(function (item, i, result) {
                    var tour = new TourViewModel(item);
                    self.tours.push(tour);
                });                          
            }
        });
    }
    self.loadTours();
}

var ToursListViewModel = new TourViewModel();
ko.applyBindings(ToursListViewModel);