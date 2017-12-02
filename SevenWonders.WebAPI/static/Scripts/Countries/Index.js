$(document).ready(function () {
	loadCountriesTable();
});
function loadCountriesTable() {
	$("#countriesTable")
		.on('init.dt', function () {
			$('#countriesTable').on('click', '.edit', EditCountryHandler);
		})
		.on('init.dt', function () {
			$('#countriesTable').on('click', '.delete', DeleteCountryHandler);
		})
		.dataTable({
			"processing": false,
			"destroy": true,
			"serverSide": false,
			"ajax": {
				"url": "../api/Countries/GetCountries",
				"method": "GET",
				"dataSrc": "",
			},
			"createdRow": function (row, item, dataIndex) {
				var deleteCss = '<a data-countryid= "' + item.Id + '" class="btn btn-warning middle-button delete" role="button">Delete</a>';
				var editCss = '<a data-countryid= "' + item.Id + '" class="btn btn-warning middle-button edit" role="button">Edit</a>';
				$('td', row).eq(1).html(deleteCss);
				$('td', row).eq(2).html(editCss);
			},
			"columns": [
				{ "data": "Name" }, { "data": null }, { "data": null }
			],
			"language": {
				"emptyTable": "There are no countries at present.",
				"zeroRecords": "There were no matching countries found."
			},
			"searching": true,
			"ordering": true,
			"paging": true
		});
}

function EditCountryHandler(event) {
	event.preventDefault();
	idCountry = $(this).data("countryid");

	$.get('../api/Countries/GetCountry', { id: idCountry },
		function (html) {
			countryModifyViewModel.editCountry(html);
		});
}
function DeleteCountryHandler(event) {
	idCountry = $(this).data("countryid");
	countryModifyViewModel.deleteCountry(idCountry);
}
function addCountry(event) {
	countryModifyViewModel.addCountry();
}

ko.validation.rules['isNameUnique'] = {
	validator: function (name, id) {
		var isValid = true;
		$.ajax({
			async: false,
			url: '../api/Countries/IsNameValid',
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
function CountryModifyViewModel() {
	var self = this;
	self.Id = ko.observable(0);
	self.Name = ko.observable().extend({
		required: true, minLength: 4, maxLength: 20,
		isNameUnique: {
			params: self.Id,
			message: 'Country name should be unique!'
		}
	})	
	self.errors = ko.observable();

	self.updateViewModel = function (country) {
		if (country != undefined) {
			self.Id(country.Id);
			self.Name(country.Name);			
		}
		else {
			self.Id(0);
			self.Name(undefined);
		}
		self.errors = ko.validation.group(self, { deep: true });
		self.errors.showAllMessages(false);
	}
	self.addCountry = function () {
		self.updateViewModel();
		$('#editCountryModal').modal();
	}
	self.editCountry = function (country) {
		self.updateViewModel(country);
		$('#editCountryModal').modal();
	}
	self.saveEditing = function () {
		self.errors = ko.validation.group(self, { deep: true });
		if (self.errors().length === 0) {
			var model = {
				id: self.Id(),
				name: self.Name(),
			};
			$.ajax("../api/Countries/AddCountry", {
				type: "post",
				data: JSON.stringify(model),
				contentType: "application/json",
				success: function (result) {
					$('#countriesTable').DataTable().ajax.reload();
					$('#editCountryModal').modal('hide');
				}
			});
		}
		else {
			self.errors.showAllMessages(true);
		}
	}
	self.deleteCountry = function (id) {
		self.Id(id);
		$('#deleteCountryModal').modal();
	}
	self.saveDeleting = function () {
		var id = self.Id();
		$.ajax({
			type: "POST",
			url: '../api/Countries/DeleteCountry',
			data: JSON.stringify(id),
			contentType: "application/json",
			success: function (result) {
				$('#countriesTable').DataTable().ajax.reload();
				$('#deleteCountryModal').modal('hide');
			}
		});
	}
}

var countryModifyViewModel = new CountryModifyViewModel();
ko.applyBindings(countryModifyViewModel, document.getElementById("countryViewModel"))