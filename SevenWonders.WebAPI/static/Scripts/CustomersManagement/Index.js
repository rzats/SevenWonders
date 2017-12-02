$(document).ready(function () {
	$("#customersTable")
			.dataTable({
				"processing": false,
				"serverSide": false,
				"ajax": {
					"url": "../api/CustomersManagement/GetCustomers",
					"method": "GET",
					"dataSrc": "",
				},
				"createdRow": function (row, item, dataIndex) {
					if (item.IsDeleted == true) {
						$('td', row).eq(5).html('<span style="color: rgb(255, 0, 0);" data-customer-id="'+item.Id+'">Deleted</span>');
					} 
					else {
						$('td', row).eq(5).html('<span style="color: rgb(0, 128, 0);" data-customer-id="' + item.Id + '">Active</span>');
					}

					var bits = (item.DateOfBirth).split(/\D/);
					var date = bits[0] + "-" + bits[1] + "-" + bits[2];
					$('td', row).eq(4).text(date);

					var buttonCss = "<button class='btn btn-warning middle-button' onclick='changeStatus(" + item.Id + ")\'> Change status </button>"
					$('td', row).eq(6).html(buttonCss);
				},
				"columns": [
					{ "data": "FirstName" }, { "data": "LastName" }, { "data": "PhoneNumber" },
					{ "data": "Email" }, { "data": "DateOfBirth" }, { "data": "IsDeleted" } , { "data":null }
				],
				"language": {
					"emptyTable": "There are no customers at present.",
					"zeroRecords": "There were no matching customers found."
				},
				"searching": true,
				"ordering": true,
				"paging": true
			});
});

function changeStatus(id) {
	$.ajax({
		type: "POST",
		url: "../api/CustomersManagement/ChangeCustomerStatus/" + id,
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