function HotelShortInfoViewModel() {
	var self = this;
	self.Id = ko.observable(-1);
	self.Name = ko.observable();
	self.Description = ko.observable();
	self.Rating = ko.observable();
	self.Country = ko.observable();
	self.City = ko.observable();
	self.Address = ko.observable();
	self.Facilities = ko.observableArray([]);
	self.HotelPhotos = ko.observableArray([]);

	self.updateViewModel = function (hotel) {
		self.Id(hotel.Id);
		self.Name(hotel.Name);
		self.Description(hotel.Description);
		self.Rating(hotel.Rating);
		self.Country(hotel.Country);
		self.City(hotel.City);
		self.Address(hotel.Address);
		self.Facilities(hotel.Facilities);
		self.HotelPhotos(hotel.HotelPhotos);
	};
}

var hotelShortInfoViewModel = new HotelShortInfoViewModel();

(function ($) {
	HotelBind();
})(jQuery);