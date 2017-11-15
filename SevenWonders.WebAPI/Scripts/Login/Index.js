function login() {
    var email = $('#email').val();
    var password = $('#password').val();
    var rememberMe = $('rememberMe').val();
    debugger;
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
                window.location.reload(true);
                window.location.href = "#/home";
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
