function FlightsTableViewModel() {
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

	self.idOfDeletedFlight=ko.observable();
	self.showModalDeleteFlight = function (flight) {
		self.idOfDeletedFlight(flight.Id);
		$('#deleteFlightModal').modal();
	};
    self.deleteFlight = function () {	
		var id = self.idOfDeletedFlight();
		$.ajax({
			type: "POST",
			url: '../api/Flights/DeleteFlight',
			data: JSON.stringify(id),
			contentType: "application/json",
            success: function (result) {
				//check if page is not empty in feature
				self.dataCount(self.dataCount()-1);
				if (self.pageIndex() + 1 > self.pageCount()) {
					self.pageIndex(self.pageCount() - 1);
				}
				self.loadTable();
				$('#deleteFlightModal').modal('hide');

			}
		});
	}

	self.showModalEditFlight = function (flight) {
		flightModifyViewModel.editFlight(flight);
	}
	self.showModalSchedule = function (flight) {
		scheduleViewModel.initScheduleModel(flight);
	}
}

ko.validation.rules['isNumberUnique'] = {
	validator: function (number,id) {
		var isValid = true;
		$.ajax({
			async: false,
			url: '../api/Flights/IsNumberValid',
			type: 'Get',
			data: { id: id, number: number },
			success: function (response) {
				isValid = response === true;
			},
			error: function () {
				isValid = false;              
			}
		});
		return isValid;
	},
	message: 'The Number is not unique.'
}; 
ko.validation.rules['isDifference'] = {
	validator: function (val, otherVal) {
		return !(val === otherVal);
	},
	message: 'Departure and Arrival airports should be different!'
}; 
ko.validation.init({
	insertMessages: true,
	messagesOnModified: true,
	errorClass: 'validationMessage'
});

function FlightModifyViewModel(FlightsTableViewModel) {
	var self = this;
	self.Id = ko.observable(-1);
	self.Number = ko.observable().extend({
		required: true, minLength: 4, maxLength: 4, pattern: {
			message: 'This field should contain only digits.',
			params: /\b\d{4}\b/g
		}, isNumberUnique: {
			params: self.Id,
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
		self.errors= ko.validation.group(self, { deep: true });
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
					flightsTableViewModel.loadTable();
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

function ScheduleItemViewModel(id, day, departureTime, arrivalTime) {
	var self = this;
	self.Id = id;
	self.DayOfWeek = day;
	self.DepartureTime = departureTime;
	self.ArrivalTime = arrivalTime;

	self.isOld = ko.computed(function () {
		return self.Id !== -1;
		}, self);
}
function ScheduleViewModel() {
	var self = this;
	self.Schedule = ko.observableArray([]);
	self.FlightId = ko.observable();
	self.DropdownDays = ko.observableArray([
		{ Text: "Monday", Id: 1 },
		{ Text: "Tuesday", Id: 2 },
		{ Text: "Wednesday", Id: 3 },
		{ Text: "Thursday", Id: 4 },
		{ Text: "Friday", Id: 5 },
		{ Text: "Saturday", Id: 6 },
		{ Text: "Sunday", Id: 0 }
	]);

	self.initScheduleModel = function (flight) {
		self.FlightId(flight.Id);
		self.loadSchedule()
		$('#scheduleModal').modal();
	};
	self.loadSchedule = function () {
		$.ajax("../api/Flights/GetSchedule", {
			type: "get",
			data: {
				id: self.FlightId()
			},
			contentType: "application/json",
			success: function (result) {
				self.Schedule([]);
				result.forEach(function (item, i, result) {
					var bits = (item.DepartureTime).split(/\D/);
					var departureTime = bits[3] + ":" + bits[4] ;
					bits = (item.ArrivalTime).split(/\D/);
					var arrivalTime = bits[3] + ":" + bits[4];
					self.Schedule.push(new ScheduleItemViewModel(item.Id, item.DayOfWeek, departureTime, arrivalTime));
				});
			}
		});
	}
	self.addScheduleItem = function () {
		self.Schedule.push(new ScheduleItemViewModel(-1, 1, "00:00","00:00"));
	}
	self.removeSchedule = function (item) {
		self.Schedule.remove(item)
	};
	self.saveSchedule = function () {
		var postObject =
			{
				schedule: self.Schedule(),
				flightId: self.FlightId()
			};
		$.ajax("../api/Flights/EditSchedule", {
			type: "POST",
			data: JSON.stringify(postObject),
			contentType: "application/json",
			success: function (result) {
				$('#scheduleModal').modal('hide');
			}
		});
	};
}

var flightsTableViewModel = new FlightsTableViewModel();
var flightModifyViewModel = new FlightModifyViewModel();
var scheduleViewModel = new ScheduleViewModel();

ko.applyBindings(flightsTableViewModel, document.getElementById("flightsTable"));
ko.applyBindings(flightModifyViewModel, document.getElementById("flightModifyViewModel"));
ko.applyBindings(scheduleViewModel, document.getElementById("scheduleModal"));
