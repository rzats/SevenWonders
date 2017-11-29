function login() {
    var email = $('#email').val();
    var password = $('#password').val();
    var rememberMe = $('rememberMe').val();
    if (isValid()) {
        $.ajax({
            url: "../api/Account/Login",
            type: "POST",
            data:
                {
                    Email: email,
                    Password: password,
                    RememberMe: rememberMe
                },
            success: function () {
                window.location.href = "#/home";
                headerViewModel.loadHeader();
            },
            error: function (err) {
                console.log(err);
                $('#badPassword').show();
            }
        })
    }
    else {
        $('#emptyEntrance').show();
    }
}

function isValid() {
    return !(isEmpty($('#email').val()) ||
        isEmpty($('#password').val()));
}

function isEmpty(str) {
    return (!str || /^\s*$/.test(str));
}
