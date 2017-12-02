$(document).ready(function () {
	loadManagersTable();
});
function loadManagersTable() {
	 $("#managersTable")
		.on('init.dt', function () {
			$('#managersTable').on('click', '.edit', EditManagerHandler);
		})
		.dataTable({
			"processing": false,
			"destroy": true,
			"serverSide": false,
			"ajax": {
				"url": "../api/ManagersManagement/GetManagers",
				"method": "POST",
				"dataSrc": "",
			},
			"createdRow": function (row, item, dataIndex) {
				if (item.IsDeleted == true) {
					$('td', row).eq(5).html('<span style="color: rgb(255, 0, 0);" data-customer-id="' + item.Id + '">Deleted</span>');
				}
				else {
					$('td', row).eq(5).html('<span style="color: rgb(0, 128, 0);" data-customer-id="' + item.Id + '">Active</span>');
				}

				var bits = (item.DateOfBirth).split(/\D/);
				var date = bits[0] + "-" + bits[1] + "-" + bits[2];			
				$('td', row).eq(4).text(date);

				var changeStatusCss = "<button class='btn btn-warning middle-button' onclick='changeStatus(" + item.Id + ")\'> Change status </button>"
				var editCss = '<a data-managerid= "' + item.Id + '" class="btn btn-warning middle-button edit" role="button">Edit</a>';
				$('td', row).eq(6).html(changeStatusCss);
				$('td', row).eq(7).html(editCss);
			},
			"columns": [
				{ "data": "FirstName" }, { "data": "LastName" }, { "data": "PhoneNumber" },
				{ "data": "Email" }, { "data": "DateOfBirth" }, { "data": "IsDeleted" }, { "data": null }, { "data": null }
			],
			"language": {
				"emptyTable": "There are no managers at present.",
				"zeroRecords": "There were no matching managers found."
			},
			"searching": true,
			"ordering": true,
			"paging": true
		});
}

function changeStatus(id) {
	$.ajax({
		type: "POST",
		url: "../api/ManagersManagement/ChangeManagerStatus/" + id,
		success: function () {
			var css = '*[data-customer-id="' + id + '"]';
			if ($(css).text() == "Active") {
				$(css).text("Deleted");
				$(css).css("color", "red");
			}
			else {
				$(css).text("Active");
				$(css).css("color", "green");
			}
		},
		error: function () {
			alert('failed to change status');
		}
	});
}

function EditManagerHandler(event) {
	event.preventDefault();
	idManager = $(this).data("managerid");  

	$.get('../api/ManagersManagement/GetManager', { id: idManager },
		function (html) {
			$('#id').val(html.Id);
			$('#firstname').val(html.FirstName);
			$('#lastname').val(html.LastName);

			var bits = (html.DateOfBirth).split(/\D/);
			var date = bits[0] + "-" + bits[1] + "-" + bits[2];
			$('#dateOfBirth').val(date);

			$('#phonenumber').val(html.PhoneNumber);
			$('#email').val(html.Email);
			$('#password').val(html.Password);

			$("#countries").empty();
			$('#countries').
				select2({
					placeholder: 'Select countries',
					width: 280,
					escapeMarkup: function (m) { return m; },
					data: [(html.Countries).forEach(function (element) {
						$("#countries").append($('<option>', { value: element.Id, text: element.Text, selected: element.IsChecked }));
					})],
					language: {
						noResults: function () {
							return 'No result.';
						},
						searching: function (val) {
							saveMultiselectValueInLocalVariable(val.term);
							return "Searching...";
						}
					}
				});
			$('#saveEditing').unbind();
			$('#saveEditing').click(function () {
				saveEditing(idManager);
			});

			$('#editManagerModal').modal('show');
		});

}

var currentSelectTerm;
var saveMultiselectValueInLocalVariable = function (text) {
	currentSelectTerm = text;
};

function saveEditing(idManager) {
	var id = idManager;
	var firstname = $('#firstname').val();
	var lastname = $('#lastname').val();
	var birthdate = $('#dateOfBirth').val();
	var phonenumber = $('#phonenumber').val();
	var email = $('#email').val();
	var password = $('#password').val();
	var countries = $('#countries').val();

	var postObject =
		{
			manager:
			{
				Id: id,
				FirstName: firstname,
				LastName: lastname,
				DateOfBirth: birthdate,
				PhoneNumber: phonenumber,
				Email: email,
				Password: password
			},
			countries: countries
		};
	$.ajax({
		type: "POST",
		url: "../api/ManagersManagement/AddManager",
		data: postObject,
		success: function () {
			$('#managersTable').DataTable().ajax.reload();
			$('#editManagerModal').modal('hide');
		},
		error: function () {
			alert('failed');
		}
	});
}

function addManager(event) {
	$.get('../api/ManagersManagement/GetCountries',
		function (html) {
			$('#id').val(0);
			$('#firstname').val("");
			$('#lastname').val("");
			$('#dateOfBirth').val(new Date());

			$('#phonenumber').val("");
			$('#email').val("");
			$('#password').val("");

			$("#countries").empty();
			$('#countries').
				select2({
					width: 280,
					placeholder: 'Select countries',
					escapeMarkup: function (m) { return m; },
					data: [html.forEach(function (element) {
						$("#countries").append($('<option>', { value: element.Id, text: element.Text, selected: element.IsChecked }));
					})],
					language: {
						noResults: function () {
							return 'No result.';
						},
						searching: function (val) {
							saveMultiselectValueInLocalVariable(val.term);
							return "Searching...";
						}
					}
				});

			$('#saveEditing').unbind();
			$('#saveEditing').click(function () {
				saveEditing(0);
			});

			$('#editManagerModal').modal('show', { backdrop: 'static' });
		});
}