function CustomerViewModel() {
	var self = this;

	self.firstName = ko.observable("");
	self.lastName = ko.observable("");
	self.dateOfBirth = ko.observable("");
	self.phoneNumber = ko.observable("");
	self.email = ko.observable("");
	self.discount = ko.observable("");

	self.loadCustomer = function () {
		$.ajax({
			type: "GET",
			url: "../api/CustomerCabinet/GetCurrentCustomer",
			success: function (data) {
				self.firstName(data.FirstName);
				self.lastName(data.LastName);

				var bits = (data.DateOfBirth).split(/\D/);
				var dateOfBirth = bits[0] + "-" + bits[1] + "-" + bits[2];
				self.dateOfBirth(dateOfBirth);

				self.phoneNumber(data.PhoneNumber);
				self.email(data.Email);
				self.discount(data.Discount);
			},
			error: function (err) {
				console.log(err);
			}
		});
	}
	self.loadCustomer();
	self.showEditCustomerModal = function () {
		$('#editCustomerModal').modal();
		editCustomerViewModel.passModel(self);
	}
}
var customerViewModel = new CustomerViewModel();
ko.applyBindings(customerViewModel, $("#customerDetails")[0]); 

ko.validation.rules['isEmailUnique'] = {
	validator: function (email) {
		var isValid = true;
		$.ajax({
			async: false,
			url: '../api/CustomerCabinet/IsEmailValid',
			type: 'Get',
			data: { email:email },
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
ko.validation.init({
	insertMessages: true,
	messagesOnModified: true,
	errorClass: 'validationMessage'
});
function EditCustomerViewModel() {
	var self = this;

	self.firstName = ko.observable("").extend({ required: true, minLength: 4, maxLength: 20 });
	self.lastName = ko.observable("").extend({ required: true, minLength: 4, maxLength: 20 });
	self.dateOfBirth = ko.observable("").extend({ required: true});
	self.phoneNumber = ko.observable("").extend({
		required: true,
		pattern: {
			message: 'Phone number should contain only digits',
			params: '^[0-9]+$'
		},
		minLength: 9,
		maxLength:  14			
	});
	self.email = ko.observable("").extend({
		required: true, email: true,
		isEmailUnique: {
			params: self.Id,
			message: 'Email should be unique!'
		} });

	self.errors = ko.observable();

	self.passModel = function (data) {
		self.firstName(data.firstName());
		self.lastName(data.lastName());
		self.dateOfBirth(data.dateOfBirth());

		self.phoneNumber(data.phoneNumber());
		self.email(data.email());

		self.errors = ko.validation.group(self, { deep: true });
		self.errors.showAllMessages(false);
	}
	self.changeCustomer = function () {
		self.errors = ko.validation.group(self, { deep: true });
		if (self.errors().length === 0) {
			$.ajax({
				type: "POST",
				url: "../api/CustomerCabinet/EditCustomer",
				data:
				{
					FirstName: self.firstName,
					LastName: self.lastName,
					DateOfBirth: self.dateOfBirth,
					PhoneNumber: self.phoneNumber,
					Email: self.email
				},
				success: function () {
					$('#editCustomerModal').modal('hide');
					customerViewModel.loadCustomer();
				},
				error: function (err) {
					console.log(getErrors(err.responseJSON));
					$('#editServerErrors').show();
					$('#editServerErrorsString').html(getErrors(err.responseJSON));
				}
			});
		}
		else {
			self.errors.showAllMessages(true);
		}
	}
}
var editCustomerViewModel = new EditCustomerViewModel();
ko.applyBindings(editCustomerViewModel, $("#editCustomerModal")[0]); 


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
		$.ajax("../api/ToursManagement/GetToursForCustomer", {
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
(function ($) {
	//var $j = jQuery.noConflict();
	//$j(".datepicker").datepicker();

	var app1 = $.sammy('#hotelInfo', function () {
		this.get('#/cabinet', function () {
			this.partial('/Views/Hotels/HotelShortInfo.html');
		});
	});
	var app2 = $.sammy('#flightInfo', function () {
		this.get('#/cabinet', function () {
			this.partial('/Views/Flights/FlightShortInfo.html');
		});
	});
	app1.run('#/cabinet');
	app2.run('#/cabinet');
})(jQuery);