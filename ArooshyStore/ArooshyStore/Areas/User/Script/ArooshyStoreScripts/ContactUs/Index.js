$(function () {
    var $checkoutForm = $('#contact_form').validate({
        rules: {
            txtName: {
                required: true
            },
            txtEmail: {
                required: true,
                email: true 
            },
            txtSubject: {
                required: true
            },
            txtPhone: {
                required: true
            },
            txtMessage: {
                required: true
            },
        },
        messages: {
            txtName: {
                required: 'Name is required.'
            },
            txtEmail: {
                required: 'Email is required.',
                email: 'Enter a valid email address.'
            },
            txtSubject: {
                required: 'Subject is required.'
            },
            txtPhone: {
                required: 'Phone Number is required.'
            },
            txtMessage: {
                required: 'Message is required.'
            },
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        }
    });

    $('#contact_form').on("submit", function (e) {
        e.preventDefault(); 

        if (!$(this).valid()) {
            return; 
        }

        var contact = {
            txtName: $('#txtName').val(),
            txtEmail: $('#txtEmail').val(),
            txtSubject: $('#txtSubject').val(),
            txtMessage: $('#txtMessage').val(),
            txtPhone: $('#txtPhone').val()
        };

        $('#btnSendMessage').prop('disabled', true);
        $('#btnSendMessage').html("<i class='bx bx-refresh bx-spin'></i> &nbsp; Processing...");

        $.ajax({
            url: "/User/ContactUs/SendMessage",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(contact),
            dataType: 'json',
            success: function (data) {
                if (data.status == true) {
                    toastr.success("Your message has been saved successfully. Our team will contact you soon", "Success", { timeOut: 3000, closeButton: true });
                    $('#contact_form')[0].reset();
                } else {
                    toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
                }
                $('#btnSendMessage').prop('disabled', false);
                $('#btnSendMessage').html("Send Message");
            },
            error: function () {
                alert("An error occurred. Please try again.");
                $('#btnSendMessage').prop('disabled', false);
                $('#btnSendMessage').html("Send Message");
            }
        });
    });
});
