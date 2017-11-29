$(document).ready(function () {
	loadCitiesTable();
});
function loadCitiesTable() {
	$("#citiesTable")
		.on('init.dt', function () {
			$('#citiesTable').on('click', '.edit', EditCityHandler);
		})
		.on('init.dt', function () {
			$('#citiesTable').on('click', '.delete', DeleteCityHandler);
		})
		.dataTable({
			"processing": false,
			"destroy": true,
			"serverSide": false,
			"ajax": {
				"url": "../api/Cities/GetCities",
				"method": "GET",
				"dataSrc": "",
			},
			"createdRow": function (row, item, dataIndex) {
				var deleteCss = '<a data-cityid= "' + item.Id + '" class="btn btn-warning middle-button delete" role="button">Delete</a>';
				var editCss = '<a data-cityid= "' + item.Id + '" class="btn btn-warning middle-button edit" role="button">Edit</a>';
				$('td', row).eq(2).html(deleteCss);
				$('td', row).eq(3).html(editCss);
			},
			"columns": [
				{ "data": "Name" }, { "data": "CountryName" }, { "data": null }, { "data": null }
			],
			"language": {
				"emptyTable": "There are no cities at present.",
				"zeroRecords": "There were no matching cities found."
			},
			"searching": true,
			"ordering": true,
			"paging": true
		});
}

function EditCityHandler(event) {
	event.preventDefault();
	idCity = $(this).data("cityid");

	$.get('../api/Cities/GetCity', { id: idCity },
		function (html) {
			cityModifyViewModel.editCity(html);
		});
}
function DeleteCityHandler(event) {
	idCity = $(this).data("cityid");
	cityModifyViewModel.deleteCity(idCity);
}
function addCity(event) {
	cityModifyViewModel.addCity();
}

ko.validation.rules['isNameUnique'] = {
	validator: function (name, params) {
		var isValid = true;
		if (params.countryId() != undefined) {
			$.ajax({
				async: false,
				url: '../api/Cities/IsNameValid',
				type: 'Get',
				data: { id: params.id, name: name, countryId: params.countryId },
				success: function (response) {
					isValid = response === true;
				},
				error: function () {
					isValid = false;
				}
			});
		}
		return isValid;
	}
};
ko.validation.init({
	insertMessages: true,
	messagesOnModified: true,
	errorClass: 'validationMessage'
});
function CityModifyViewModel() {
	var self = this;
	self.Id = ko.observable(0);
	self.Countries = ko.observableArray([]);
	self.CountryId = ko.observable(0)
		.extend({ required: true });
	self.Name = ko.observable().extend({
		required: true, minLength: 4, maxLength: 20,
		isNameUnique: {
			params: { id: self.Id, countryId: self.CountryId },
			message: 'City name should be unique!'
		}
	})

	self.loadCountries = function () {
		$.ajax("../api/Countries/GetCountries", {
			type: "get",
			contentType: "application/json",
			success: function (result) {
				self.Countries(result);
			}
		});
	}
	self.loadCountries();

	self.errors = ko.observable();

	self.updateViewModel = function (city) {
		if (city != undefined) {
			self.Id(city.Id);
			self.Name(city.Name);
			self.CountryId(city.CountryId);
		}
		else {
			self.Id(0);
			self.Name(undefined);
			self.CountryId(undefined);
		}
		self.errors = ko.validation.group(self, { deep: true });
		self.errors.showAllMessages(false);
	}
	self.addCity = function () {

		self.updateViewModel();
		$('#editCityModal').modal();
	}
	self.editCity = function (city) {
		self.updateViewModel(city);
		$('#editCityModal').modal();
	}
	self.saveEditing = function () {
		self.errors = ko.validation.group(self, { deep: true });
		if (self.errors().length === 0) {
			var model = {
				Id: self.Id(),
				Name: self.Name(),
				CountryId: self.CountryId()
			};
			$.ajax("../api/Cities/AddCity", {
				type: "post",
				data: JSON.stringify(model),
				contentType: "application/json",
				success: function (result) {
					$('#citiesTable').DataTable().ajax.reload();
					$('#editCityModal').modal('hide');
				}
			});
		}
		else {
			self.errors.showAllMessages(true);
		}
	}
	self.deleteCity = function (id) {
		self.Id(id);
		$('#deleteCityModal').modal();
	}
	self.saveDeleting = function () {
		var id = self.Id();
		$.ajax({
			type: "POST",
			url: '../api/Cities/DeleteCity',
			data: JSON.stringify(id),
			contentType: "application/json",
			success: function (result) {
				$('#citiesTable').DataTable().ajax.reload();
				$('#deleteCityModal').modal('hide');
			}
		});
	}
}

var cityModifyViewModel = new CityModifyViewModel();
ko.applyBindings(cityModifyViewModel, document.getElementById("cityViewModel"));