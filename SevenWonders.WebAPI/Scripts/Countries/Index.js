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
				"emptyTable": "There are no customers at present.",
				"zeroRecords": "There were no matching customers found."
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
			$('#id').val(html.Id);
			$('#name').val(html.Name);

			$('#saveEditing').click(function () {
				saveEditing(idCountry);
			});
			$('#editCountryModal').modal('show', { backdrop: 'static' });
		});
}

function DeleteCountryHandler(event) {
}

function saveEditing(idCountry) {
	var id = idCountry;
	var name = $('#name').val();

	var postObject =
		{
			Id: id,
			Name: name
		};

	$.ajax({
		type: "POST",
		url: "../api/Countries/AddCountry",
		data: postObject,
		success: function () {
			$('#countriesTable').DataTable().ajax.reload();
			$('#editCountryModal').modal('hide');
		},
		error: function () {
			alert('failed');
		}
	});
}

function addCountry(event) {
	$('#id').val(0);
	$('#name').val("");

	$('#saveEditing').click(function () {
		saveEditing(0);
	});
	$('#editCountryModal').modal('show', { backdrop: 'static' });
}