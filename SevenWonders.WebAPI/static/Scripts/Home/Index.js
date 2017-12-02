$(document).ready(function () {
    var app = $.sammy('#mainDiv', function () {
        this.get('#/', function () {
            this.app.swap('Click form!');
        });
        this.get('#/home', function () {
            this.partial('/Views/Search/Index.html');
        });
        this.get('#/register', function () {
            this.partial('/Views/Account/Register.html');
        });
        this.get('#/login', function () {
            this.partial('/Views/Account/Login.html');
        });
        this.get('#/contact', function () {
            this.partial('/Views/Home/Contact.html');
        });
        this.get('#/cabinet', function () {
            this.partial('/Views/CustomerCabinet/Index.html');
        });
        this.get('#/customers', function () {
            this.partial('/Views/CustomersManagement/Index.html');
        });
        this.get('#/managers', function () {
            this.partial('/Views/ManagersManagement/Index.html');
        });
        this.get('#/flights', function () {
            this.partial('/Views/Flights/Index.html');
        });
        this.get('#/countries', function () {
            this.partial('/Views/Countries/Index.html');
        });
        this.get('#/cities', function () {
            this.partial('/Views/Cities/Index.html');
        });
        this.get('#/airports', function () {
            this.partial('/Views/Airports/Index.html');
        });
        this.get('#/tours', function () {
            this.partial('/Views/ToursManagement/Index.html');
        });
        this.get('#/booking', function () {
            this.partial('/Views/Search/Booking.html');
        });
    });
    $(function () {
        app.run('#/home');
    });
});

function HeaderViewModel() {
    var self = this;
    self.Role = ko.observable("");
    self.isCustomer = ko.computed(function () {
        return (self.Role() == "customer");
    }, self);
    self.isManager = ko.computed(function () {
        return (self.Role() == "manager");
    }, self);
    self.isAdmin = ko.computed(function () {
        return (self.Role() == "admin");
    }, self);
    self.ifAdminOrManager = ko.computed(function () {
        return (self.Role() == "admin" || self.Role() == "manager");
    }, self);
    self.isAuthorized = ko.computed(function () {
        return self.Role() != "";
    }, self);
    self.isUnAuthorized = ko.computed(function () {
        return self.Role() == "";
    }, self);
    self.loadHeader = function () {
        $.ajax("../api/Account/GetUserRole", {
            type: "get",
            contentType: "application/json",
            success: function (result) {
                self.Role(result);
            }
        });
    }
    self.loadHeader();

    self.logOut = function () {
        $.ajax("../api/Account/LogOut", {
            type: "post",
            contentType: "application/json",
            success: function () {
                window.location.href = "#/home";
                headerViewModel.loadHeader();
            },
        });
    }
}
var headerViewModel = new HeaderViewModel(); 
ko.applyBindings(headerViewModel, document.getElementById("headerViewModel"));