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
	self.Number = ko.observable();
	self.Price = ko.observable();
	self.AirplaneModel = ko.observable();
	self.AirplaneCompany = ko.observable();
	self.SeatsAmount = ko.observable();
	self.ArrivalAirports = ko.observableArray([]);

	self.updateViewModel = function () {
		self.Number(undefined);
		self.Price(undefined);
		self.AirplaneModel(undefined);
		self.AirplaneCompany(undefined);
		self.SeatsAmount(undefined);
	}
	self.loadArrivalAirports =  = function () {
		$.ajax("../api/Flights/GetAirports", {
			type: "get",
			contentType: "application/json",
			success: function (result) {
				self.ArrivalAirports(result);
			}
		});
	}
	self.addFlight = function () {
		self.updateViewModel();
		$('#editFlightModal').modal();
	}
	self.saveFlight = function () {
		$.ajax("../api/Flights/AddFlight", {
			type: "post",
			data: {
				number: self.Number(),
				price: self.Price(),
				airplaneModel: self.AirplaneModel(),
				airplaneCompany: self.AirplaneCompany(),
				seatsAmount: self.SeatsAmount()
			},
			contentType: "application/json",
			success: function (result) {
				reservationsViewModel.loadTable();
				$('#editFlightModal').modal('hide');
			}
		});
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
