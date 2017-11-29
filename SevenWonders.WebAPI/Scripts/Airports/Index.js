$(document).ready(function () {
	loadAirportsTable();
});
function loadAirportsTable() {
	$("#airportsTable")
		.on('init.dt', function () {
			$('#airportsTable').on('click', '.edit', EditAirportHandler);
		})
		.on('init.dt', function () {
			$('#airportsTable').on('click', '.delete', DeleteAirportHandler);
		})
		.dataTable({
			"processing": false,
			"destroy": true,
			"serverSide": false,
			"ajax": {
				"url": "../api/Airports/GetAirports",
				"method": "GET",
				"dataSrc": "",
			},
			"createdRow": function (row, item, dataIndex) {
				var deleteCss = '<a data-airportid= "' + item.Id + '" class="btn btn-warning middle-button delete" role="button">Delete</a>';
				var editCss = '<a data-airportid= "' + item.Id + '" class="btn btn-warning middle-button edit" role="button">Edit</a>';
				$('td', row).eq(3).html(deleteCss);
				$('td', row).eq(4).html(editCss);
			},
			"columns": [
				{ "data": "Name" }, { "data": "Code" }, { "data": "CityName" }, { "data": null }, { "data": null }
			],
			"language": {
				"emptyTable": "There are no airports at present.",
				"zeroRecords": "There were no matching airports found."
			},
			"searching": true,
			"ordering": true,
			"paging": true
		});
}

function EditAirportHandler(event) {
	event.preventDefault();
	idAirport = $(this).data("airportid");

	$.get('../api/Airports/GetAirport', { id: idAirport },
		function (html) {
			airportModifyViewModel.editAirport(html);
		});
}
function DeleteAirportHandler(event) {
	idAirport = $(this).data("airportid");
	airportModifyViewModel.deleteAirport(idAirport);
}
function addAirport(event) {
	airportModifyViewModel.addAirport();
}

ko.validation.rules['isCodeUnique'] = {
	validator: function (code, id) {
		var isValid = true;
		$.ajax({
			async: false,
			url: '../api/Airports/IsCodeValid',
			type: 'Get',
			data: { id: id, code: code},
			success: function (response) {
				isValid = response === true;
			},
			error: function () {
				isValid = false;
			}
		});
		return isValid;
	}
};
ko.validation.init({
	insertMessages: true,
	messagesOnModified: true,
	errorClass: 'validationMessage'
});
function AirportModifyViewModel() {
	var self = this;
	self.Id = ko.observable(0);
	self.Name = ko.observable().extend({
		required: true, minLength: 4, maxLength: 50
	})
	self.Cities = ko.observableArray([]);
	self.CityId = ko.observable(0)
		.extend({ required: true });
	self.Code = ko.observable().extend({
		required: true, minLength: 3, maxLength: 3,
		pattern: {
			message: 'This field should contain Uppercase letters.',
			params: /\b[A-Z]+\b/g
		},
		isCodeUnique: {
			params: self.Id,
			message: 'Airport code should be unique!'
		}
	})

	self.loadCities = function () {
		$.ajax("../api/Cities/GetCities", {
			type: "get",
			contentType: "application/json",
			success: function (result) {
				self.Cities(result);
			}
		});
	}
	self.loadCities();

	self.errors = ko.observable();

	self.updateViewModel = function (airport) {
		if (airport != undefined) {
			self.Id(airport.Id);
			self.Name(airport.Name);
			self.Code(airport.Code);
			self.CityId(airport.CityId);
		}
		else {
			self.Id(0);
			self.Name(undefined);
			self.Code(undefined);
			self.CityId(undefined);
		}
		self.errors = ko.validation.group(self, { deep: true });
		self.errors.showAllMessages(false);
	}
	self.addAirport = function () {
		self.updateViewModel();
		$('#editAirportModal').modal();
	}
	self.editAirport = function (airport) {
		self.updateViewModel(airport);
		$('#editAirportModal').modal();
	}
	self.saveEditing = function () {
		self.errors = ko.validation.group(self, { deep: true });
		if (self.errors().length === 0) {
			var model = {
				Id: self.Id(),
				Name: self.Name(),
				Code: self.Code(),
				CityId: self.CityId()
			};
			$.ajax("../api/Airports/AddAirport", {
				type: "post",
				data: JSON.stringify(model),
				contentType: "application/json",
				success: function (result) {
					$('#airportsTable').DataTable().ajax.reload();
					$('#editAirportModal').modal('hide');
				}
			});
		}
		else {
			self.errors.showAllMessages(true);
		}
	}
	self.deleteAirport = function (id) {
		self.Id(id);
		$('#deleteAirportModal').modal();
	}
	self.saveDeleting = function () {
		var id = self.Id();
		$.ajax({
			type: "POST",
			url: '../api/Airports/DeleteAirport',
			data: JSON.stringify(id),
			contentType: "application/json",
			success: function (result) {
				$('#airportsTable').DataTable().ajax.reload();
				$('#deleteAirportModal').modal('hide');
			}
		});
	}
}

var airportModifyViewModel = new AirportModifyViewModel();
ko.applyBindings(airportModifyViewModel, document.getElementById("airportViewModel"));