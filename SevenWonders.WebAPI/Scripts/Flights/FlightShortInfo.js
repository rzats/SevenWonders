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
		debugger;
		self.LeaveFlightNumber(flight.LeaveFlightNumber);
		self.LeaveFlightAirplaneModel(flight.LeaveFlightAirplaneModel);
		self.LeaveFlightAirplaneCompany(flight.LeaveFlightAirplaneCompany);
		self.LeaveFlightDepartureAirport(flight.LeaveFlightDepartureAirport);
		self.LeaveFlightDepartureCity(flight.LeaveFlightDepartureCity);
		self.LeaveFlightDepartureCountry(flight.LeaveFlightDepartureCountry);
		self.LeaveFlightDepartureTime(flight.LeaveFlightDepartureTime);
		self.LeaveFlightArrivalAirport(flight.LeaveFlightArrivalAirport);
		self.LeaveFlightArrivalCity(flight.LeaveFlightArrivalCity);
		self.LeaveFlightArrivalCountry(flight.LeaveFlightArrivalCountry);
		self.LeaveFlightArrivalTime(flight.LeaveFlightArrivalTime);

		self.ReturnFlightNumber(flight.ReturnFlightNumber);
		self.ReturnFlightAirplaneModel(flight.ReturnFlightAirplaneModel);
		self.ReturnFlightAirplaneCompany(flight.ReturnFlightAirplaneCompany);
		self.ReturnFlightDepartureAirport(flight.ReturnFlightDepartureAirport);
		self.ReturnFlightDepartureCity(flight.ReturnFlightDepartureCity);
		self.ReturnFlightDepartureCountry(flight.ReturnFlightDepartureCountry);
		self.ReturnFlightDepartureTime(flight.ReturnFlightDepartureTime);
		self.ReturnFlightArrivalAirport(flight.ReturnFlightArrivalAirport);
		self.ReturnFlightArrivalCity(flight.ReturnFlightArrivalCity);
		self.ReturnFlightArrivalCountry(flight.ReturnFlightArrivalCountry);
		self.ReturnFlightArrivalTime(flight.ReturnFlightArrivalTime);
	};
}
var flightShortInfoViewModel = new FlightShortInfoViewModel();
(function ($) {
	FlightBind();
})(jQuery);