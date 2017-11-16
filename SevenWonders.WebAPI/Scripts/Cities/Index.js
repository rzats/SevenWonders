$(document).ready(function () {
	loadCitiesTable();
});
function loadCaitiesTable() {
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
				$('td', row).eq(1).html(deleteCss);
				$('td', row).eq(2).html(editCss);
			},
			"columns": [
				{ "data": "Name" }, { "data": "Country" }, { "data": null }, { "data": null }
			],
			"language": {
				"emptyTable": "There are no customers at present.",
				"zeroRecords": "There were no matching customers found."
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
	validator: function (name, id) {
		var isValid = true;
		$.ajax({
			async: false,
			url: '../api/Cities/IsNameValid',
			type: 'Get',
			data: { id: id, name: name },
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
function CityModifyViewModel() {
	var self = this;
	self.Id = ko.observable(0);
	self.Name = ko.observable().extend({
		required: true, minLength: 4, maxLength: 20,
		isNameUnique: {
			params: self.Id,
			message: 'City name should be unique!'
		}
	})
	self.errors = ko.observable();

	self.updateViewModel = function (city) {
		if (city != undefined) {
			self.Id(city.Id);
			self.Name(city.Name);
		}
		else {
			self.Id(0);
			self.Name(undefined);
		}
		self.errors = ko.validation.group(self, { deep: true });
		self.errors.showAllMessages(false);
	}
	self.addCity = function () {
		debugger;
		self.updateViewModel();
		$('#editCityModal').modal();
	}
	self.editCity = function (city) {
		debugger;
		self.updateViewModel(city);
		$('#editCityModal').modal();
	}
	self.saveEditing = function () {
		self.errors = ko.validation.group(self, { deep: true });
		if (self.errors().length === 0) {
			var model = {
				id: self.Id(),
				name: self.Name(),
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
		debugger;
		self.Id(id);
		$('#deleteCityModal').modal();
	}
	self.saveDeleting = function () {
		debugger;
		var id = self.Id();
		$.ajax({
			type: "POST",
			url: '../api/Cities/DeleteCity',
			data: JSON.stringify(id),
			contentType: "application/json",
			success: function (result) {
				debugger;
				$('#citiesTable').DataTable().ajax.reload();
				$('#deleteCityModal').modal('hide');
			}
		});
	}
}

var cityModifyViewModel = new CityModifyViewModel();
ko.applyBindings(cityModifyViewModel)