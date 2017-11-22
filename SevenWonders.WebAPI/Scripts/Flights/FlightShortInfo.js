function FlightShortInfoViewModel() {
	var self = this;

	self.LeaveFlightNumber = ko.observable();
	self.LeaveFlightAirplaneModel = ko.observable();
	self.LeaveFlightAirplaneCompany = ko.observable();
	self.LeaveFlightDepartureAirport = ko.observable();
	self.LeaveFlightDepartureCity = ko.observable();
	self.LeaveFlightDepartureCountry = ko.observable();
	self.LeaveFlightDepartureTime = ko.observable();
	self.LeaveFlightArrivalAirport = ko.observable();
	self.LeaveFlightArrivalCity = ko.observable();
	self.LeaveFlightArrivalCountry = ko.observable();
	self.LeaveFlightArrivalTime = ko.observable();

	self.ReturnFlightNumber = ko.observable( );
	self.ReturnFlightAirplaneModel = ko.observable();
	self.ReturnFlightAirplaneCompany = ko.observable();
	self.ReturnFlightDepartureAirport = ko.observable();
	self.ReturnFlightDepartureCity = ko.observable();
	self.ReturnFlightDepartureCountry = ko.observable();
	self.ReturnFlightDepartureTime = ko.observable();
	self.ReturnFlightArrivalAirport = ko.observable();
	self.ReturnFlightArrivalCity = ko.observable();
	self.ReturnFlightArrivalCountry = ko.observable();
	self.ReturnFlightArrivalTime = ko.observable();

	self.updateViewModel = function (flight ) {
		self.LeaveFlightNumber(flight.LeaveFlightNumber);
		self.LeaveFlightAirplaneModel(flight.LeaveFlightAirplaneModel);
		self.LeaveFlightAirplaneCompany(flight.LeaveFlightAirplaneCompany);
		self.LeaveFlightDepartureAirport(flight.LeaveFlightDepartureAirport);
		self.LeaveFlightDepartureCity(flight.LeaveFlightDepartureCity);
		self.LeaveFlightDepartureCountry(flight.LeaveFlightDepartureCountry);

		var bits = (flight.LeaveFlightDepartureTime).split(/\D/);
		var date = bits[0] + "-" + bits[1] + "-" + bits[2] + " " + bits[3] + ":" + bits[4];
		self.LeaveFlightDepartureTime(date);
		self.LeaveFlightArrivalAirport(flight.LeaveFlightArrivalAirport);
		self.LeaveFlightArrivalCity(flight.LeaveFlightArrivalCity);
		self.LeaveFlightArrivalCountry(flight.LeaveFlightArrivalCountry);

		bits = (flight.LeaveFlightArrivalTime).split(/\D/);
		date = bits[0] + "-" + bits[1] + "-" + bits[2] + " " + bits[3] + ":" + bits[4];
		self.LeaveFlightArrivalTime(date);
		self.ReturnFlightNumber(flight.ReturnFlightNumber);
		self.ReturnFlightAirplaneModel(flight.ReturnFlightAirplaneModel);
		self.ReturnFlightAirplaneCompany(flight.ReturnFlightAirplaneCompany);
		self.ReturnFlightDepartureAirport(flight.ReturnFlightDepartureAirport);
		self.ReturnFlightDepartureCity(flight.ReturnFlightDepartureCity);
		self.ReturnFlightDepartureCountry(flight.ReturnFlightDepartureCountry);

		bits = (flight.ReturnFlightDepartureTime).split(/\D/);
		date = bits[0] + "-" + bits[1] + "-" + bits[2] + " " + bits[3] + ":" + bits[4];
		self.ReturnFlightDepartureTime(date);
		self.ReturnFlightArrivalAirport(flight.ReturnFlightArrivalAirport);
		self.ReturnFlightArrivalCity(flight.ReturnFlightArrivalCity);
		self.ReturnFlightArrivalCountry(flight.ReturnFlightArrivalCountry);

		bits = (flight.ReturnFlightArrivalTime).split(/\D/);
		date = bits[0] + "-" + bits[1] + "-" + bits[2] + " " + bits[3] + ":" + bits[4];
		self.ReturnFlightArrivalTime(date);
	};
}
var flightShortInfoViewModel = new FlightShortInfoViewModel();
(function ($) {
	FlightBind();
})(jQuery);