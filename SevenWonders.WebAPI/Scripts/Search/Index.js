function SearchViewModel(FlightsTableViewModel) {
	var self = this;
	self.CountryFrom = ko.observable();
	self.CityFrom = ko.observable();
	self.CountryTo = ko.observable();
	self.CityTo = ko.observable();
	self.People = ko.observable();
	self.DepartureDay = ko.observable();
	self.Duration = ko.observable();

	self.DepartureAirports = ko.observableArray([]);
	self.selectedChoiceDeparture = ko.observable()
		.extend({ required: true });
	self.ArrivalAirports = ko.observableArray([]);
	self.selectedChoiceArrival = ko.observable()
		.extend({ required: true, isDifference: self.selectedChoiceDeparture });
	self.loadAirports = function () {
		$.ajax("../api/Flights/GetAirports", {
			type: "get",
			contentType: "application/json",
			success: function (result) {
				self.DepartureAirports(result);
				self.ArrivalAirports(result);
			}
		});
	}
	self.loadAirports();

	self.errors = ko.observable();

	self.updateViewModel = function (flight) {
		if (flight != undefined) {
			self.Id(flight.Id);
			self.Number(flight.Number);
			self.Price(flight.Price);
			self.AirplaneModel(flight.AirplaneModel);
			self.AirplaneCompany(flight.AirplaneCompany);
			self.SeatsAmount(flight.AirplaneSeatsAmount);
			self.selectedChoiceDeparture(flight.DepartureAirportId);
			self.selectedChoiceArrival(flight.ArrivalAirportId);

			self.urlAction("../api/Flights/EditFlight");
		}
		else {
			self.Number(undefined);
			self.Price(undefined);
			self.AirplaneModel(undefined);
			self.AirplaneCompany(undefined);
			self.SeatsAmount(undefined);
			self.selectedChoiceDeparture(undefined);
			self.selectedChoiceArrival(undefined);

			self.urlAction("../api/Flights/AddFlight");
		}
		self.errors = ko.validation.group(self, { deep: true });
		self.errors.showAllMessages(false);
	}

	self.addFlight = function () {
		self.updateViewModel();
		$('#editFlightModal').modal();
	}
	self.editFlight = function (flight) {
		self.updateViewModel(flight);
		$('#editFlightModal').modal();
	}

	self.saveChanges = function () {
		self.errors = ko.validation.group(self, { deep: true });
		if (self.errors().length === 0) {
			var model = {
				id: self.Id(),
				number: self.Number(),
				price: self.Price(),
				departureAirportId: self.selectedChoiceDeparture(),
				arrivalAirportId: self.selectedChoiceArrival(),
				airplaneModel: self.AirplaneModel(),
				airplaneCompany: self.AirplaneCompany(),
				seatsAmount: self.SeatsAmount()
			};
			$.ajax(self.urlAction(), {
				type: "post",
				data: JSON.stringify(model),
				contentType: "application/json",
				success: function (result) {
					FlightsTableViewModel.loadTable();
					$('#editFlightModal').modal('hide');
				}
			});
		}
		else {
			self.errors.showAllMessages(true);
		}
	}
	self.urlAction = ko.observable();
}