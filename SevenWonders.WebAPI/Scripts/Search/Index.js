ko.validation.init({
    insertMessages: false,
    decorateInputElement: true,
    messagesOnModified: true,
    errorClass: 'validationMessage'
});

function SearchViewModel() {
    var self = this;
    self.Countries = ko.observableArray([]);
    self.CitiesFrom = ko.observableArray([]);
    self.CitiesTo = ko.observableArray([]);

    self.CountryFrom = ko.observable().extend({ required: true });;
	self.CityFrom = ko.observable();
    self.CountryTo = ko.observable().extend({ required: true });;
	self.CityTo = ko.observable();
    self.People = ko.observable().extend({ required: true });;
    self.DepartureDay = ko.observable().extend({ required: true });;
    self.Duration = ko.observable().extend({ required: true });;

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

    self.loadCityFrom = function () {
        $.ajax("../api/Cities/GetCities", {
            type: "get",
            data: {
                countryId: self.CountryFrom(),
            },
            contentType: "application/json",
            success: function (result) {
                self.CitiesFrom(result);
            }
        });
    }
    self.loadCityTo = function () {
        $.ajax("../api/Cities/GetCities", {
            type: "get",
            data: {
                countryId: self.CountryTo(),
            },
            contentType: "application/json",
            success: function (result) {
                self.CitiesTo(result);
            }
        });
    }

    self.errors = ko.observable();
    self.submitSearch = function () {
        self.errors = ko.validation.group(self, { deep: true });
        if (self.errors().length === 0) {
            var href= "#/booking?countryFrom=" + self.CountryFrom()
                + (self.CityFrom() != undefined ? "&cityFrom=" + self.CityFrom() : "") 
                + "&countryTo=" + self.CountryTo()
                + (self.CityTo() != undefined ? "&cityTo=" + self.CityTo() : "")
                + "&people=" + self.People()
                + "&departureDate=" + self.DepartureDay()
                + "&duration=" + self.Duration()
            window.location.href = href;
        }
        else {
            self.errors.showAllMessages(true);
        }
    }

    self.CountryFrom.subscribe(function () {
        self.loadCityFrom();
    });
    self.CountryTo.subscribe(function (s) {
        self.loadCityTo();
    });
}
var searchViewModel = new SearchViewModel();
ko.applyBindings(searchViewModel, document.getElementById("searchForm"));

$(document).ready(function () {
    var today = new Date().toISOString().split('T')[0];
    document.getElementsByName("DepartureDay")[0].setAttribute('min', today);
});