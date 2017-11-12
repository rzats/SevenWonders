function register()
{
    var firstName = $('#firstName').val();
    var lastName = $('#lastName').val();
    var dateOfBirth = $('#dateOfBirth').val();
    var phoneNumber = $('#phoneNumber').val();
    var email = $('#email').val();
    var password = $('#password').val();
    var confirmedPassword = $('#confirmPassword').val();

    if (isValid()) {
        $.ajax({
            url: "../api/Account/Register",
            type: "POST",
            data:
                {
                    FirstName: firstName,
                    LastName: lastName,
                    DateOfBirth: dateOfBirth,
                    PhoneNumber: phoneNumber,
                    Email: email,
                    Password: password,
                    ConfirmPassword: confirmedPassword
                },
            success: function () {
                window.location.href = "#/login";
            },
            error: function (err) {
                console.log(getErrors(err.responseJSON));
                $('#serverErrors').show();
                $('#serverErrorsString').html(getErrors(err.responseJSON));
            }
        })
    }
}

function isValid()
{
    if(isEmpty($('#email').val()) || 
        isEmpty($('#firstName').val()) ||
        isEmpty($('#lastName').val()) ||
        isEmpty($('#phoneNumber').val()) ||
        isEmpty($('#dateOfBirth').val()) ||
        isEmpty($('#password').val()) ||
        isEmpty($('#confirmPassword').val()))
    {
        $('#emptyEntrance').show();
        return false;
    }

    if(($('#password').val()) != $('#confirmPassword').val())
    {
        $('#noMatch').show();
        return false;
    }

    return true;
}

function isEmpty(str) {
    return (!str || /^\s*$/.test(str));
}

function getErrors(responce)
{
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