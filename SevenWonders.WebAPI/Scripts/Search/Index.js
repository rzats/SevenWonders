$(document).ready(function () {
    getCountries();
});
function validateForm() {
    var isValid = true;
    var countryFrom = document.getElementById('countryFrom');
    var cityFrom = document.getElementById('cityFrom');
    var countryTo = document.getElementById('countryTo');
    var cityTo = document.getElementById('cityTo');
    var departureDay = document.getElementById('departureDay');
    if (countryFrom.value == "") {
        countryFrom.style.borderColor = "red";
        isValid = false;
    }
    else {
        countryFrom.style.borderStyle = "none";
    }

    if (cityFrom.value == "" || cityFrom.value == 0) {
        cityFrom.style.borderColor = "red";
        isValid = false;
    }
    else {
        cityFrom.style.borderStyle = "none";
    }

    if (countryTo.value == "") {
        countryTo.style.borderColor = "red";
        isValid = false;
    }
    else {
        countryTo.style.borderStyle = "none";
    }

    if (cityTo.value == "" || cityTo.value == 0) {
        cityTo.style.borderColor = "red";
        isValid = false;
    }
    else {
        cityTo.style.borderStyle = "none";
    }

    if (departureDay.value == "" || departureDay.value == 0) {
        departureDay.style.borderColor = "red";
        isValid = false;
    }
    else {
        departureDay.style.borderStyle = "none";
    }

    if (isValid) {
        document.getElementById('btnSearch').click();
    }
}

function FillCity(countryId, cityField) {
    $.ajax({
        url: "../api/Search/GetCities",
        type: "GET",
        data: { countryId: countryId },
        dataType: "JSON",
        success: function (cities) {
            console.log(cities);
            $(cityField).html(""); // clear before appending new list
            $(cityField).append(
                    $('<option></option>').val(0).html("Select city"));
            $.each(cities, function (i, city) {
                $(cityField).append(
                    $('<option></option>').val(city.Id).html(city.Name));
            });
        },
        error: function (err) {
            alert(err);
        }
    });
}

function FillHotel(cityId) {
    //$('#replaceClientModal').modal('show', { backdrop: 'static' });
    $.ajax({
        url: "../api/Search/GetHotels",
        type: "GET",
        data: { cityId: cityId },
        dataType: "JSON",
        success: function (hotels) {
            $("#hotel").html(""); // clear before appending new list
            $("#hotel").append(
                    $('<option></option>').val(0).html("Select hotel"));
            $.each(hotels, function (i, hotel) {
                $("#hotel").append(
                    $('<option></option>').val(hotel.Id).html(hotel.Name));
            });
        }
    });
}

function getCountries() {
    $.ajax({
        type: "GET",
        url: "../api/ManagersManagement/GetCountriesForSearch",
        success: function (result) {
            $.each(result, function (key, value) {
                $("#countryFrom").append($("<option></option>").val(value.Id).html(value.Name));
                $("#countryTo").append($("<option></option>").val(value.Id).html(value.Name));
            });
        }
    })
}