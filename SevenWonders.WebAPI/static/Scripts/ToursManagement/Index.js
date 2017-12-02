(function ($) {
	var app1 = $.sammy('#hotelInfo', function () {
		this.get('#/tours', function () {
			this.partial('/Views/Hotels/HotelShortInfo.html');
		});
	});
	var app2 = $.sammy('#flightInfo', function () {
		this.get('#/tours', function () {
			this.partial('/Views/Flights/FlightShortInfo.html');
		});
	});
	app1.run('#/tours');
	app2.run('#/tours');
})(jQuery);

function ToursTableViewModel() {
	var self = this;
	self.tours = ko.observableArray();

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
		if (self.pageIndex() + 1 != self.pageCount()) {
			return self.indexOfFirst() + self.pageSize() - 1;
		}
		else {
			return self.dataCount();
		}
		return Math.ceil(self.dataCount() / this.pageSize());
	}, self);

	self.loadTable = function () {
		$.ajax("../api/ToursManagement/GetToursForManager", {
			type: "get",
			data: {
				pageIndex: self.pageIndex(),
				pageSize: self.pageSize()
			},
			contentType: "application/json",
			success: function (result) {
				result.tours.forEach(function (item, i, result) {
					var bits = (item.OrderDate).split(/\D/);
					var orderDate = bits[0] + "-" + bits[1] + "-" + bits[2] + " " + bits[3] + ":" + bits[4] + ":" + bits[5];
					item.OrderDate = orderDate;

					var bits = (item.LeaveDate).split(/\D/);
					var leaveDate = bits[0] + "-" + bits[1] + "-" + bits[2];
					item.LeaveDate = leaveDate;

					var bits = (item.ReturnDate).split(/\D/);
					var returnDate = bits[0] + "-" + bits[1] + "-" + bits[2];
					item.ReturnDate = returnDate;
				});
				self.tours(result.tours);
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

	self.idOfSelectedTour = ko.observable();
	self.showModalDeleteTour = function (tour) {
		self.idOfSelectedTour(tour.Id);
		$('#deleteTourModal').modal();
	};
	self.deleteTour = function () {
		var id = self.idOfSelectedTour();
		$.ajax({
			type: "POST",
			url: '../api/ToursManagement/DeleteTour',
			data: JSON.stringify(id),
			contentType: "application/json",
			success: function (result) {
				//check if page is not empty in feature
				self.dataCount(self.dataCount() - 1);
				if (self.pageIndex() + 1 > self.pageCount()) {
					self.pageIndex(self.pageCount() - 1);
				}
				self.loadTable();
				$('#deleteTourModal').modal('hide');
			}
		});
	}

	self.showModalPayTour = function (tour) {
		self.idOfSelectedTour(tour.Id);
		$('#payTourModal').modal();
	};
	self.payTour = function () {
		var id = self.idOfSelectedTour();
		$.ajax({
			type: "POST",
			url: '../api/ToursManagement/PayForTour',
			data: JSON.stringify(id),
			contentType: "application/json",
			success: function (result) {
				self.loadTable();
				$('#payTourModal').modal('hide');
			}
		});
	}

	self.showHotelDetail = function (tour) {
		var id = tour.HotelId;
		$.ajax("../api/Hotels/GetHotelShortInfo", {
			type: "get",
			data: {
				id: id
			},
			contentType: "application/json",
			success: function (result) {
				hotelShortInfoViewModel.updateViewModel(result)
				$('#hotelInfoModal').modal();
			}
		});
	}
	self.showFlightDetail = function (tour) {
		var id = tour.Id;
		$.ajax("../api/Flights/GetFlightsShortInfo", {
			type: "get",
			data: {
				id: id
			},
			contentType: "application/json",
			success: function (result) {
				flightShortInfoViewModel.updateViewModel(result)
				$('#flightInfoModal').modal();
			}
		});
	};

	self.updateTours = function () {
		$.ajax({
			type: "POST",
			url: '../api/ToursManagement/UpdateTours',
			contentType: "application/json",
			success: function (result) {
				self.loadTable();
				$('#payTourModal').modal('hide');
			}
		});
	};
}

var mainTableViewModel = new ToursTableViewModel();
ko.applyBindings(mainTableViewModel, $("#mainTable")[0]);

function HotelBind() {
	ko.applyBindings(hotelShortInfoViewModel, document.getElementById("hotelInfoModal"));
}
function FlightBind() {
	ko.applyBindings(flightShortInfoViewModel, document.getElementById("flightInfoModal"));
}