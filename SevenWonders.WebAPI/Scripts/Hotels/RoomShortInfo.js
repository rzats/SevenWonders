function RoomShortInfoViewModel() {
    var self = this;
    self.MaxPeople = ko.observable();
    self.RoomType = ko.observable();
    self.WindowView = ko.observable();
    self.Equipments = ko.observableArray([]);
    self.RoomPhotos = ko.observableArray([]);

    self.updateViewModel = function (room) {
        self.MaxPeople(room.MaxPeople());
        self.RoomType(room.RoomType());
        self.WindowView(room.WindowView());
        self.Equipments(room.Equipments());
        self.RoomPhotos(room.RoomPhotos());
    };
}

var roomShortInfoViewModel = new RoomShortInfoViewModel();
(function ($) {
    RoomBind();
})(jQuery);