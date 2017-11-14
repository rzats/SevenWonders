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

ko.validation.rules['isNumberUnique'] = {
	validator: function (val, param) {
		var isValid = true;
		$.ajax({
			async: false,
			url: '../api/Flights/IsNumberValid',
			type: 'Get',
			data: { number: val },
			success: function (response) {
				isValid = response === true;
			},
			error: function () {
				isValid = false; //however you would like to handle this              
			}
		});
		return isValid;
	},
	message: 'The Email is not unique'
}; 
ko.validation.rules['isDifference'] = {
	validator: function (val, otherVal) {
		debugger;
		return !(val === otherVal);
	},
	message: 'Departure and Arrival airports should be different!'
}; 
ko.validation.init({
	insertMessages: true,
	messagesOnModified: true,
	errorClass: 'validationMessage'
});

function CreateViewModel(reservationsViewModel) {
	var self = this;

	self.Number = ko.observable().extend({
		required: true, minLength: 4, maxLength: 4, pattern: {
			message: 'This field should contain only digits.',
			params: /\b\d{4}\b/g
		}, isNumberUnique: {
			message: 'Flight number should be unique!'
		} 
	});
	self.Price = ko.observable().extend({ required: true, numeric: 2 , max:1000 });
	self.AirplaneCompany = ko.observable().extend({ required: true, minLength: 4, maxLength: 20 })
	self.AirplaneModel = ko.observable().extend({ required: true, minLength: 4, maxLength: 20 })
	self.SeatsAmount = ko.observable().extend({ required: true, numeric: 0, min:1, max: 1000 });;

	self.DepartureAirports = ko.observableArray([]);
	self.selectedChoiceDeparture = ko.observable()
		.extend({ required: true});
	self.ArrivalAirports = ko.observableArray([]);
	self.selectedChoiceArrival = ko.observable()
		.extend({ required: true, isDifference: self.selectedChoiceDeparture });
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

	self.errors = ko.observable();

	self.updateViewModel = function () {
		self.Number(undefined);
		self.Price(undefined);
		self.AirplaneModel(undefined);
		self.AirplaneCompany(undefined);
		self.SeatsAmount(undefined);
		self.selectedChoiceDeparture(undefined);
		self.selectedChoiceArrival(undefined);

		self.errors = ko.validation.group(self, { deep: true });
		self.errors.showAllMessages(false);
	}
	self.addFlight = function () {
		self.updateViewModel();
		$('#editFlightModal').modal();
	}

	self.saveFlight = function () {
		debugger;
		self.errors= ko.validation.group(self, { deep: true });
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
		else {
			self.errors.showAllMessages(true);
		}
	}
}

var reservationsViewModel = new ReservationsViewModel();
var MainViewModel = {
	ReservationsViewModel: reservationsViewModel,
	CreateViewModel: new CreateViewModel(reservationsViewModel)
};


ko.applyBindings(MainViewModel);
$(document).ready(function () {

});
