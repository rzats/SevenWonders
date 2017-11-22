function SearchViewModel() {
    debugger;
    var self = this;
    self.Countries = ko.observableArray([]);
    self.CitiesFrom = ko.observableArray([]);
    self.CitiesTo = ko.observableArray([]);

	self.CountryFrom = ko.observable();
	self.CityFrom = ko.observable();
	self.CountryTo = ko.observable();
	self.CityTo = ko.observable();
	self.People = ko.observable();
	self.DepartureDay = ko.observable();
	self.Duration = ko.observable();

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
        debugger;
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
        debugger;
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

	self.submitSearch = function () {
		self.updateViewModel();
		$('#editFlightModal').modal();
    }

    self.CountryFrom.subscribe(function () {
        self.loadCityFrom();
    });
    self.CountryTo.subscribe(function (s) {
        self.loadCityTo();
    });
}
var searchViewModel = new SearchViewModel();

ko.applyBindings(searchViewModel);