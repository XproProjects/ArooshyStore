// Login Form Submission
$(document).ready(function () {
    $('#forgotPasswordForm').hide();

    $('#showForgotPassword').click(function () {
        $('#loginForm').hide();
        $('#forgotPasswordForm').show();
    });

    $('#backToLogin').click(function () {
        $('#forgotPasswordForm').hide();
        $('#loginForm').show();
    });
});
$(document).ready(function () {
    // Initialize validation for login form
    $('#loginForm').validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true
            }
        },
        messages: {
            Email: {
                required: 'Email is required.',
                email: 'Please enter a valid email address.'
            },
            Password: {
                required: 'Password is required.'
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        }
    });

    // Initialize validation for forgot password form
    $('#forgotPasswordForm').validate({
        rules: {
            ForgotEmail: {
                required: true,
                email: true
            }
        },
        messages: {
            ForgotEmail: {
                required: 'Email is required.',
                email: 'Please enter a valid email address.'
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        }
    });
    // Initialize validation for forgot password form
    $('#userResetForm').validate({
        rules: {
            userResetPassword: {
                required: true,
            },
            userConfirmPassword: {
                required: true,
                equalTo: '#userResetPassword' 
            }
        },
        messages: {
            userResetPassword: {
                required: 'Password is required.',
            },
            userConfirmPassword: {
                required: 'Confirm Password is required.',
                equalTo: 'Passwords do not match.' // Message for when passwords don't match
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        }
    });

    // Toggle between login and forgot password forms
    $('#showForgotPassword').click(function () {
        $('#loginForm').hide();
        $('#forgotPasswordForm').show();
    });

    $('#backToLogin').click(function () {
        $('#forgotPasswordForm').hide();
        $('#loginForm').show();
    });
$("#btnLogin").click(function (event) {
    var form = $("#userLoginForm");

    if (form[0].checkValidity() === false) {
        event.preventDefault();
        event.stopPropagation();
    }

    form.addClass('was-validated');
    submitLoginForm(); 
});

function submitLoginForm() {
    $('.important').html(''); 
    $('#btnLogin').attr('disabled', 'disabled').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var loginData = {
        Email: $('#userEmail').val(),
        Password: $('#userPassword').val()
    };

    $.ajax({
        type: "POST",
        url: "/user/customeraccount/login/",
        data: loginData,
        dataType: 'json',
        success: function (data) {
            $('#btnLogin').prop('disabled', false).html("Sign in");

            if (data.status) {
                toastr.success("Logged in successfully", "Success", { timeOut: 3000, closeButton: true });
            } else {
                $("#userLoginForm").removeClass('was-validated');
                $('.important').html('<span style="font-size:16px;">' + data.message + '</span>').css('margin-bottom', '10px');
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });

            }
        },
        error: function () {
            $('#btnLogin').prop('disabled', false).html("Sign in");
            $('.important').html('<span style="font-size:16px;">An error occurred. Please try again.</span>').css('margin-bottom', '10px');
        }
    });
}

// Forgot Password Form Submission
(function () {
    var $checkoutForm = $('#forgotPasswordForm').validate({
        rules: {
            ForgotEmail: {
                required: true
            },

        },
        messages: {
            ForgotEmail: {
                required: 'Email is required.'
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
$('#userForgotPasswordForm').on('submit', function (e) {
    e.preventDefault();
    $('.importantForgot').html('');
    $('#btnSubmitForgotPassword').attr('disabled', 'disabled').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var email = $('#userForgotEmail').val();

    $.ajax({
        type: "POST",
        url: "/user/customeraccount/forgotpassword/",
        data: { 'email': email },
        dataType: 'json',
        success: function (data) {
            $('#btnSubmitForgotPassword').prop('disabled', false).html("Submit");

            if (data.status) {
                $('.importantForgot').html('<span style="font-size:14px;color:green !important">' + data.message + '</span>').css('margin-bottom', '10px');
            } else {
                $("#userForgotPasswordForm").removeClass('was-validated');
                $('.importantForgot').html('<span style="font-size:14px;">' + data.message + '</span>').css('margin-bottom', '10px');
            }
        },
        error: function () {
            $('#btnSubmitForgotPassword').prop('disabled', false).html("Submit");
            $('.importantForgot').html('<span style="font-size:14px;">An error occurred. Please try again.</span>').css('margin-bottom', '10px');
        }
    });
});
// Reset Password Form Submission
$('#userResetForm').on('submit', function (e) {
    e.preventDefault();
    $('.important').html('');

    var password = $('#userResetPassword').val();
    var confirmPassword = $('#userConfirmPassword').val();
    var userId = $('#HiddenUserIdForResetPassword').val();

    if (password !== confirmPassword) {
        $('.important').html('<span style="font-size:16px;">Passwords do not match.</span>').css('margin-bottom', '10px');
        return false;
    }

    $('#btnReset').attr('disabled', 'disabled').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    $.ajax({
        type: "POST",
        url: "/user/customeraccount/resetpassword/",
        data: { 'userId': userId, 'password': password },
        dataType: 'json',
        success: function (data) {
            $('#btnReset').prop('disabled', false).html("Reset Now");

            if (data.status) {
                toastr.success("Password Saved Successfully", "Success", { timeOut: 3000, closeButton: true });
            } else {
                $('.important').html('<span style="font-size:14px;">' + data.message + '</span>').css('margin-bottom', '10px');
            }
        },
        error: function () {
            $('#btnReset').prop('disabled', false).html("Reset Now");
            $('.important').html('<span style="font-size:14px;">An error occurred. Please try again.</span>').css('margin-bottom', '10px');
        }
    });
});
$("#userResetPassword, #userConfirmPassword").on("focus", function () {
    $('.important').html('');
});
$("#userPassword, #userEmail").on("mousedown keydown", function () {
    $('.important').html('');
    $('.important').css('margin-bottom', '70px');
});
});
