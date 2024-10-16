﻿$(function () {
    var $checkoutForm = $('#userResetForm').validate({
        rules: {
            password: {
                required: true
            },
            confirmPassword: {
                required: true,
                equalTo: '#password' 

            },
        },
        messages: {
            password: {
                required: 'password is required.'
            },
            confirmPassword: {
                required: 'Confirm Password is required.'
            },
          
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
            if (element.val() == '' || element.val() == null) {
                element.parents('td').siblings('td').find('.btn').css("margin-top", "-8.5px");
            }
        }
    });
})
$('#btnSubmit').click(function (event) {
    event.preventDefault();
    SubmitForm();
});
$(function () {
    $('#password').focus();
})
$('#btnSubmit').click(function () {
    SubmitForm();
})
function SubmitForm() {

    if ($('#password').val() == "" || $('#confirmPassword').val() == "") {
        if ($('#password').val() == "" && $('#confirmPassword').val() == "") {
            $('#password').css('border-color', '#cf564a');
            $('#confirmPassword').css('border-color', '#cf564a');
            $('.invalid-newpassword').remove();
            $('.new').addClass('state-error');
            $('#password').css('border-color', '#A90329');
            $('#password').css('background-color', '#FFF0F0');
            $('#confirmPassword').css('border-color', '#A90329');
            $('#confirmPassword').css('background-color', '#FFF0F0');
            $('.new').append('<p style="color:#cf564a;" for="oldPassword" class="invalid-newpassword pull-right">This field is required.</p>');
            $('.invalid-confirmpassword').remove();
            $('.confirm').addClass('state-error');
            $('.confirm').append('<p style="color:#cf564a;" for="oldPassword" class="invalid-confirmpassword pull-right">This field is required.</p>');
            $(".buttons").css('margin-top', '7px');
        }
        else if ($('#password').val() == "") {
            $('#password').css('border-color', '#cf564a');
            $('#confirmPassword').css('border-color', '');
            $('.invalid-confirmpassword').remove();
            $('.invalid-newpassword').remove();
            $('.new').addClass('state-error');
            $('#password').css('border-color', '#A90329');
            $('#password').css('background-color', '#FFF0F0');
            $('.new').append('<p style="color:#cf564a;" for="oldPassword" class="invalid-newpassword pull-right">This field is required.</p>');
        }
        else if ($('#confirmPassword').val() == "") {
            $('#confirmPassword').css('border-color', '#cf564a');
            $('#password').css('border-color', '');
            $('.invalid-newpassword').remove();
            $('.invalid-confirmpassword').remove();
            $('.confirm').addClass('state-error');
            $('#confirmPassword').css('border-color', '#A90329');
            $('#confirmPassword').css('background-color', '#FFF0F0');
            $('.confirm').append('<p style="color:#cf564a;" for="oldPassword" class="invalid-confirmpassword pull-right">This field is required.</p>');
        }
        return false;
    }
    else {
        if ($('#password').val() == $('#confirmPassword').val()) {
            $('#password').css('border-color', '');
            $('.invalid-newpassword').remove();
            $('#confirmPassword').css('border-color', '');
            $('.invalid-confirmpassword').remove();
            var v = {
                Password: $("#password").val()
            }
            $.ajax({
                type: "POST",
                url: "/User/CustomerAccount/ChangePassword",
                data:  v ,
                dataType: 'json',
                success: function (data) {
                    if (data.status) {
                        toastr.success("Password Changed successfully", "Success", { timeOut: 3000, closeButton: true });
                    } else {
                        $("#userResetForm").removeClass('was-validated');
                        $('.important').html('<span style="font-size:16px;">' + data.message + '</span>').css('margin-bottom', '10px');
                        toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });

                    }
                }
            })
        }
        else {
            $('#confirmPassword').css('border-color', '#cf564a');
            $('#password').css('border-color', '#cf564a');
            $('.invalid-confirmpassword').remove();
            $('.confirm').addClass('state-error');
            $('#password').css('border-color', '#A90329');
            $('#password').css('background-color', '#FFF0F0');
            $('#confirmPassword').css('border-color', '#A90329');
            $('#confirmPassword').css('background-color', '#FFF0F0');
            $('.confirm').append('<p style="color:#cf564a;" for="oldPassword" class="invalid-confirmpassword pull-right">Passwords do not match.</p>');
        }

    }
}
$("#password").mousedown(function () {
    $('#password').css('border-color', '#BDBDBD');
    $('#password').css('background-color', '#fff');
}).keydown(function () {
    $('#password').css('border-color', '#BDBDBD');
    $('#password').css('background-color', '#fff');
    $('.invalid-newpassword').remove();
    $('#password').css('border-color', '');
});
$("#confirmPassword").mousedown(function () {
    $('#confirmPassword').css('border-color', '#BDBDBD');
    $('#confirmPassword').css('background-color', '#fff');
}).keydown(function () {
    $('#confirmPassword').css('border-color', '#BDBDBD');
    $('#confirmPassword').css('background-color', '#fff');
    $('.invalid-confirmpassword').remove();
    $('#confirmPassword').css('border-color', '');
});
$(document).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        SubmitForm();
    }
});
$('#eyeIcon').on('click', function () {
    if ($(this).hasClass('fa-eye-slash')) {
        $(this).removeClass('fa-eye-slash');
    }
    else {
        $(this).addClass('fa-eye-slash');
    }
    var input = $("#password");
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});
$('#eyeIcons').on('click', function () {
    if ($(this).hasClass('fa-eye-slash')) {
        $(this).removeClass('fa-eye-slash');
    }
    else {
        $(this).addClass('fa-eye-slash');
    }
    var input = $("#confirmPassword");
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});