$(function () {
    $('#OldPassword').focus();
})

$('#btnSubmit').click(function (e) {
    e.preventDefault();
    SubmitForm();
})
function SubmitForm() {
    var oldPassword = $('#OldPassword').val();
    if (oldPassword == null || oldPassword == '') {
        $('.invalid').remove();
        $('#OldPassword').css('border-color', '#A90329');
        $('#OldPassword').css('background-color', '#FFF0F0');
        $('.PasswordTextbox').append('<p style="color:#cf564a;" for="OldPassword" class="invalid pull-right" style="margin-top:5px !important">Old password is Required.</p>');
        return false;
    }
    $.ajax({
        url: "/Admin/Account/CheckOldPassword",
        type: "POST",
        data: { 'password': oldPassword },
        dataType: 'json',
        success: function (data) {
            if (data) {
                window.location.href = "/admin/account/changepassword"
            }
            else {
                $('.invalid').remove();
                $('#OldPassword').css('border-color', '#cf564a');
                $('.PasswordTextbox').addClass('state-error');
                $('#OldPassword').css('border-color', '#A90329');
                $('#OldPassword').css('background-color', '#FFF0F0');
                $('.PasswordTextbox').append('<p style="color:#cf564a;" for="OldPassword" class="invalid pull-right" style="margin-top:5px !important">Old password is incorrect.</p>');
                $(".buttons").css('margin-top', '7px');
            }
        }
    })
}
$("#OldPassword").mousedown(function () {
    $('#OldPassword').css('border-color', '#BDBDBD');
    $('#OldPassword').css('background-color', '#fff');
    $('.invalid').remove();
}).keydown(function () {
    $('#OldPassword').css('border-color', '#BDBDBD');
    $('#OldPassword').css('background-color', '#fff');
    $('.invalid').remove();
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
    var input = $("#OldPassword");
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});