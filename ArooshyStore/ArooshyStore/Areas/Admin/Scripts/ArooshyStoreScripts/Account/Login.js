$("#btnLogin").click(function (event) {

    // Fetch form to apply custom Bootstrap validation
    var form = $("#loginForm")

    if (form[0].checkValidity() === false) {
        event.preventDefault()
        event.stopPropagation()
    }

    form.addClass('was-validated');
    // Perform ajax submit here...
});
$("#btnSubmitForgotPassword").click(function (event) {

    // Fetch form to apply custom Bootstrap validation
    var form = $("#forgotPasswordForm")

    if (form[0].checkValidity() === false) {
        event.preventDefault()
        event.stopPropagation()
    }

    form.addClass('was-validated');
    // Perform ajax submit here...
});
$("#btnReset").click(function (event) {

    // Fetch form to apply custom Bootstrap validation
    var form = $("#resetForm")

    if (form[0].checkValidity() === false) {
        event.preventDefault()
        event.stopPropagation()
    }

    form.addClass('was-validated');
    // Perform ajax submit here...
});
$('#loginForm').on('submit', function (e) {
    e.preventDefault();
    $('.important').html('');
    $('#btnLogin').attr('disabled', 'disabled');
    $('#btnLogin').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    var password = $('#Password').val();
    var username = $('#Email').val();
    var login =
    {
        Email: username,
        Password: password,
        //IPAddress: ip
    }
    $.ajax({
        type: "POST",
        url: "/admin/account/login/",
        data: login,
        dataType: 'json',
        success: function (data) {
            $('#btnLogin').prop('disabled', false);
            $('#btnLogin').html("Login Now");
            if (data.status) {
                window.location.href = "/admin/home/index";
            }
            else {
                $(".LoginForm").removeClass('was-validated');
                $('.important').html('<span style="font-size:16px;">' + data.message + '</span>');
                $('.important').css('margin-bottom', '10px');
            }
        }
    })
})
$('#forgotPasswordForm').on('submit', function (e) {
    e.preventDefault();
    $('.importantForgot').html('');
    $('#btnSubmitForgotPassword').attr('disabled', 'disabled');
    $('#btnSubmitForgotPassword').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    var email = $('#ForgotEmail').val();
    $.ajax({
        type: "POST",
        url: "/admin/account/forgotpassword/",
        data: { 'email': email },
        dataType: 'json',
        success: function (data) {
            $('#btnSubmitForgotPassword').prop('disabled', false);
            $('#btnSubmitForgotPassword').html("Submit");
            if (data.status) {
                $('.importantForgot').html('<span style="font-size:14px;color:green !important">' + data.message + '</span>');
                $('.importantForgot').css('margin-bottom', '10px');
            }
            else {
                $(".ForgotPasswordForm").removeClass('was-validated');
                $('.importantForgot').html('<span style="font-size:14px;">' + data.message + '</span>');
                $('.importantForgot').css('margin-bottom', '10px');
            }
        }
    })
})
$('#resetForm').on('submit', function (e) {
    e.preventDefault();
    $('.important').html('');
    var password = $('#Password').val();
    var confirmPassword = $('#ConfirmPassword').val();
    var userId = $('#HiddenUserIdForResetPassword').val();
    if (password != confirmPassword) {
        $(".ResetForm").removeClass('was-validated');
        $('.important').html('<span style="font-size:16px;">Passwords do not match.</span>');
        $('.important').css('margin-bottom', '10px');
        return false;
    }
    $('#btnReset').attr('disabled', 'disabled');
    $('#btnReset').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    $.ajax({
        type: "POST",
        url: "/admin/account/resetpassword/",
        data: { 'userId': userId, 'password': password },
        dataType: 'json',
        success: function (data) {
            $('#btnReset').prop('disabled', false);
            $('#btnReset').html("Reset Now");
            if (data.status) {
                $('.important').html('<span style="font-size:14px;color:green !important">' + data.message + '</span>');
                $('.important').css('margin-bottom', '10px');
            }
            else {
                $(".ResetForm").removeClass('was-validated');
                $('.important').html('<span style="font-size:14px;">' + data.message + '</span>');
                $('.important').css('margin-bottom', '10px');
            }
        }
    })
})
$(document).off("click", "#btnForgotPassword").on("click", "#btnForgotPassword", function () {
    if ($('#btnLogin').html().trim() == "Login Now") {
        $('.LoginForm').attr("hidden", "hidden");
        $('.ForgotPasswordForm').removeAttr("hidden");
        //$('#Email').val('');
        //$('#Password').val('');
        //$('#ForgotEmail').val('');
        $(".LoginForm").removeClass('was-validated');
        $('.important').html('');
        $('.importantForgot').html('');
        $('#LoginHeaderBusinessName').html('Forgot Password');
        $('.important').css('margin-bottom', '70px');
        $('.importantForgot').css('margin-bottom', '70px');
    }
})
$(document).off("click", "#btnGoBackToLogin").on("click", "#btnGoBackToLogin", function () {
    if ($('#btnSubmitForgotPassword').html().trim() == "Submit") {
        $('.LoginForm').removeAttr("hidden");
        $('.ForgotPasswordForm').attr("hidden", "hidden");
        //$('#Email').val('');
        //$('#Password').val('');
        //$('#ForgotEmail').val('');
        $(".LoginForm").removeClass('was-validated');
        $('.important').html('');
        $('.importantForgot').html('');
        $('#LoginHeaderBusinessName').html($('#HiddenLoginHeaderBusinessName').html());
        $('.important').css('margin-bottom', '70px');
        $('.importantForgot').css('margin-bottom', '70px');
    }
})
$(document).off("click", "#btnGoBackToLoginOnResetForm").on("click", "#btnGoBackToLoginOnResetForm", function () {
    if ($('#btnReset').html().trim() == "Reset Now") {
        window.location.href = "/admin/account/login";
    }
})
$('#eyeIcon').on('click', function () {
    if ($(this).hasClass('fa-eye-slash')) {
        $(this).removeClass('fa-eye-slash');
    }
    else {
        $(this).addClass('fa-eye-slash');
    }
    var input = $("#Password");
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});
$('#eyeIcon2').on('click', function () {
    if ($(this).hasClass('fa-eye-slash')) {
        $(this).removeClass('fa-eye-slash');
    }
    else {
        $(this).addClass('fa-eye-slash');
    }
    var input = $("#ConfirmPassword");
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});
$("#Password,#ConfirmPassword,#Email").on("mousedown keydown", function () {
    $('.important').html('');
    $('.importantForgot').html('');
    $('.important').css('margin-bottom', '70px');
    $('.importantForgot').css('margin-bottom', '70px');
})
