$(function () {
    var $checkoutForm = $('#reviewForm').validate({
        rules: {
            ReviewByName: {
                required: true
            },
            ReviewByEmail: {
                required: true,
                email: true
            },
            Rating: {
                required: true
            },
            ReviewDetail: {
                required: true
            },
        },
        messages: {
            ReviewByName: {
                required: 'Name is required.'
            },
            ReviewByEmail: {
                required: 'Email is required.',
                email: 'Enter a valid email address.'
            },
            Rating: {
                required: 'Rating is required.'
            },
            ReviewDetail: {
                required: 'Review Detail is required.'
            },
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        }
    });

    $('#reviewForm').on("submit", function (e) {
        e.preventDefault();

        if (!$(this).valid()) {
            return;
        }

        var contact = {
            ReviewId: $('#ReviewId').val(),
            Rating: $('#Rating').val(),
            ReviewByCustomerId: $('#ReviewByCustomerId').val(),  // Added Customer ID
            ProductId: $('#ProductId').val(),                    // Added Product ID
            ReviewByEmail: $('#ReviewByEmail').val(),
            ReviewByName: $('#ReviewByName').val(),
            ReviewDetail: $('#ReviewDetail').val(),
        };

        $('#btnSendMessage').prop('disabled', true);
        $('#btnSendMessage').html("<i class='bx bx-refresh bx-spin'></i> &nbsp; Processing...");

        $.ajax({
            url: "/User/ProductsReview/Index",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(contact),
            dataType: 'json',
            success: function (data) {
                if (data.status == true) {
                    toastr.success("Your Review has been saved successfully.", "Success", { timeOut: 3000, closeButton: true });
                   
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
