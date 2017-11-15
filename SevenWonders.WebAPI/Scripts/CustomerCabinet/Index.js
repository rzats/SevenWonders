function getErrors(responce) {
    var errors = [];
    var errorsString = "";
    if (responce != null) {
        var modelState = responce.ModelState;
        for (var key in modelState) {
            if (modelState.hasOwnProperty(key)) {
                for (var i = 0; i < modelState[key].length; i++) {
                    errorsString = (errorsString == "" ? "" : errorsString + "<br/>") + modelState[key][i];
                    errors.push(modelState[key][i]);
                }
            }
        }

    }
    return errorsString;
}


ko.validation.init({
    errorElementClass: "wrong-field",
    decorateElement: true,
    errorClass: 'wrong-field'
}, true);

function CustomerViewModel() {
    var self = this;
    self.validateNow = ko.observable(false);

    self.firstName = ko.observable("").extend({
        required: {
            message: "First name cannot be empty!",
            onlyIf: function () {
                return self.validateNow();
            }
        }
    });

    self.lastName = ko.observable("").extend({
        required: {
            message: "Last name cannot be empty!",
            onlyIf: function () {
                return self.validateNow();
            }
        }
    });

    self.dateOfBirth = ko.observable("").extend({
        required: {
            message: "Date of birth cannot be empty!",
            onlyIf: function () {
                return self.validateNow();
            }
        }
    });

    self.phoneNumber = ko.observable("").extend({
        required: {
            message: "Phone number cannot be empty!",
            onlyIf: function () {
                return self.validateNow();
            }
        }
    }).extend({
        pattern: {
            message: 'Phone number should contain only digits',
            params: '^[0-9]+$',
            onlyIf: function () {
                return self.validateNow();
            }
        }
    }).extend(
    {
        minLength: {
            params: 9,
            message: "Phone number should consist of 9 to 14 digits!",
            onlyIf: function () {
                return self.validateNow();
            }
        }
    }).extend(
    {
        maxLength: {
            params: 14,
            message: "Phone number should consist of 9 to 14 digits!",
            onlyIf: function () {
                return self.validateNow();
            }
        }
    });

    self.email = ko.observable("").extend({
        required: {
            message: "Email cannot be empty!",
            onlyIf: function () {
                return self.validateNow();
            }
        }
    }).extend({
        email: true
    });

    self.errors = ko.observable();


    self.errors = ko.validation.group(self);

    $.ajax({
        type: "GET",
        url: "../api/Customer/GetCurrentCustomer",
        success: function (data) {
            self.firstName(data.FirstName);
            self.lastName(data.LastName);
            self.dateOfBirth(data.DateOfBirth);
            self.phoneNumber(data.PhoneNumber);
            self.email(data.Email);
        },
        error: function (err) {
            console.log(err);
        }
    });

    self.changeCustomer = function () {
        self.validateNow(true);
        if (self.errors().length === 0) {
            $.ajax({
                type: "POST",
                url: "../api/Customer/EditCustomer",
                data:
                   {
                       FirstName: self.firstName,
                       LastName: self.lastName,
                       DateOfBirth: self.dateOfBirth,
                       PhoneNumber: self.phoneNumber,
                       Email: self.email
                   },
                success: function () {
                    window.location.href = "#/home";
                },
                error: function (err) {
                    console.log(getErrors(err.responseJSON));
                    $('#editServerErrors').show();
                    $('#editServerErrorsString').html(getErrors(err.responseJSON));
                }
            });
        }
        else {
            alert('errors');
        }
    }
}

ko.applyBindings(new CustomerViewModel());