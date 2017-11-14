function ReservationsViewModel() {
	var self = this;

	self.flights = ko.observableArray([]);
	self.pageIndex = ko.observable(0);
	self.pageSize = ko.observable(10);
	self.dataCount = ko.observable(0);

	self.pageCount = ko.computed(function () {
		return Math.ceil(self.dataCount() / this.pageSize());
	}, self);
	self.indexOfFirst = ko.computed(function () {
		return self.pageIndex() * self.pageSize() + 1;
	}, self);
	self.indexOfLast = ko.computed(function () {
		if (self.pageIndex()+1 != self.pageCount()) {
			return self.indexOfFirst() + self.pageSize() - 1;
		}
		else {
			return self.dataCount();
		}
		return Math.ceil(self.dataCount() / this.pageSize());
	}, self);

	self.loadTable = function () {
		$.ajax("../api/Flights/GetFlights", {
			type: "get",
			data: {
				pageIndex: self.pageIndex(),
				pageSize: self.pageSize()
			},
			contentType: "application/json",
			success: function (result) {
				self.flights(result.flights);
				self.dataCount(result.dataCount);
			}
		});
	}
	self.loadTable();

	self.previousPage = function () {
		if (self.pageIndex() > 0) {
			self.pageIndex(self.pageIndex() - 1);
			self.loadTable();
		}
	};
	self.nextPage = function () {
		if (self.pageIndex() < self.pageCount() - 1) {
			self.pageIndex(self.pageIndex() + 1);
			self.loadTable();
		}
	};
	self.allPages = ko.dependentObservable(function () {
		var pages = [];
		for (i = 0; i < self.pageCount(); i++) {
			pages.push({ pageNumber: (i + 1) });
		}
		return pages;
	});
	self.moveToPage = function (index) {
		self.pageIndex(index);
		self.loadTable();
	};
}

function CreateViewModel(reservationsViewModel) {
	var self = this;
	self.validateNow = ko.observable(false);

	self.Number = ko.observable()
		.extend({
			number: {
				message: "Number should contain only numbers!",
				onlyIf: function () {
					return self.validateNow();
				}
			},
			minLength: {
				params: 4,
				message: "Number should contain 4 numbers!",
				onlyIf: function () {
					return self.validateNow();
				}
			},
			maxLength: {
				params: 4,
				message: "Number should contain 4 numbers!",
				onlyIf: function () {
					return self.validateNow();
				}
			}
		});
	self.Price = ko.observable();
	self.AirplaneCompany = ko.observable()
		.extend({
			minLength: {
				params: 4,
				message: "Number should contain 4 numbers!",
				onlyIf: function () {
					return self.validateNow();
				}
			},
			maxLength: {
				params: 20,
				message: "Number should contain 4 numbers!",
				onlyIf: function () {
					return self.validateNow();
				}
			}
		});
	self.AirplaneModel = ko.observable()
		.extend({
			minLength: {
				params: 1,
				message: "Airplane model cannot be empty!",
				onlyIf: function () {
					return self.validateNow();
				}
			},
		});
	self.SeatsAmount = ko.observable()
		.extend({
			required: {
				message: "First name cannot be empty!",
				onlyIf: function () {
					return self.validateNow();
				}
			}
		});

	self.errors = ko.observable();
	self.errors = ko.validation.group(self);

	self.DepartureAirports = ko.observableArray([]);
	self.selectedChoiceDeparture = ko.observable()
		.extend({ required: true });
	self.ArrivalAirports = ko.observableArray([]);
	self.selectedChoiceArrival = ko.observable()
		.extend({ required: true });
	self.loadArrivalAirports = function () {
		$.ajax("../api/Flights/GetAirports", {
			type: "get",
			contentType: "application/json",
			success: function (result) {
				self.DepartureAirports(result);
				self.ArrivalAirports(result);
			}
		});
	}
	self.loadArrivalAirports();

	self.updateViewModel = function () {
		self.Number(undefined);
		self.Price(undefined);
		self.AirplaneModel(undefined);
		self.AirplaneCompany(undefined);
		self.SeatsAmount(undefined);
		self.selectedChoiceDeparture(undefined);
		self.selectedChoiceArrival(undefined);
	}
	self.addFlight = function () {
		self.updateViewModel();
		$('#editFlightModal').modal();
	}

	self.saveFlight = function () {
		self.validateNow(true);
		if (self.errors().length === 0) {
			var model = {
				number: self.Number(),
				price: self.Price(),
				departureAirportId: self.selectedChoiceDeparture(),
				arrivalAirportId: self.selectedChoiceArrival(),
				airplaneModel: self.AirplaneModel(),
				airplaneCompany: self.AirplaneCompany(),
				seatsAmount: self.SeatsAmount()
			};
			$.ajax("../api/Flights/AddFlight", {
				type: "post",
				data: JSON.stringify(model),
				contentType: "application/json",
				success: function (result) {
					reservationsViewModel.loadTable();
					$('#editFlightModal').modal('hide');
				}
			});
		}
	}
}

var reservationsViewModel = new ReservationsViewModel();
var MainViewModel = {
	ReservationsViewModel: reservationsViewModel,
	CreateViewModel: new CreateViewModel(reservationsViewModel)
};

ko.validation.init({
	errorElementClass: "wrong-field",
	decorateElement: true,
	errorClass: 'wrong-field'
}, true);
ko.applyBindings(MainViewModel);
$(document).ready(function () {

});
