function checkLogin() {
    var data = {
        Username: $.cookie("Username"),
        AuthCode: $.cookie("AuthCode")
    };
    $.ajax({
        url: 'models/login/Login.aspx/checkLogin',
        type: 'Post',
        data: JSON.stringify({ Data: data }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#darkModalForm").modal('hide');
        }
    });
}

function register() {
    if ($("#Form-username").val().length < 4 || $("#Form-username").val().length > 40) {
        alertBox("1", "Check Username length");
    }
    else if ($("#Form-pass5").val().length < 4 || $("#Form-pass5").val().length > 40) {
        alertBox("1", "Check Password length");
    }
    else
    {
        var data = {
            Username: $("#Form-username").val(),
            Password: $("#Form-pass5").val()
        };
        $.ajax({
            url: 'models/login/Login.aspx/register',
            type: 'Post',
            data: JSON.stringify({ Data: data }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var message = msg.d.split("~");
               alertBox(message[0], message[1]);
            }
        });
    }
}


function login() {
    if ($("#Form-username").val().length < 4 || $("#Form-username").val().length > 40) {
        alertBox("1", "Check Username length");
    }
    else if ($("#Form-pass5").val().length < 4 || $("#Form-pass5").val().length > 40) {
        alertBox("1", "Check Password length");
    }
    else {
        var data = {
            Username: $("#Form-username").val(),
            Password: $("#Form-pass5").val()
        };
        $.ajax({
            url: 'models/login/Login.aspx/login',
            type: 'Post',
            data: JSON.stringify({ Data: data }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var message = msg.d.split("~");
                if (message[0] === "1") { alertBox(message[0], message[1]); return; }
                else {
                    $("#darkModalForm").modal('hide');
                    $.cookie.set('AuthCode', message[1]);
                }
            }
        });
    }
}

function alertBox(alert, message) {
    $("#loginAlert").removeClass();
    if (alert === "1") {
         alert = "alert-danger";
    }
    else if (alert === "2") {
        alert = "alert-success";
    }
    $("#loginAlert").addClass('alert');
    $("#loginAlert").addClass(alert);
    $("#loginAlert").fadeIn(2000);
    document.getElementById("loginAlert").innerHTML = message;
    $("#loginAlert").fadeOut(4000);
}