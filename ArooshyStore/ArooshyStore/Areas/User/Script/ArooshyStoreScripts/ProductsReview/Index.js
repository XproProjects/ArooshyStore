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
            error.insertAfter(element.parent());
            if (element.val() == '' || element.val() == null) {
                element.parents('td').siblings('td').find('.btn').css("margin-top", "-8.5px");
            }
        }
    });
})
$(document).on('change', 'select', function () {
    $(this).parents('td').siblings('td').find('.btn').css("margin-top", "14px");
    $(this).parents('label').siblings('em').remove();
})
$('#reviewForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#reviewForm").valid()) {
        return false;
    }
    $('#btnSendMessage').attr('disabled', 'disabled');
    $('#btnSendMessage').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    var ReviewId = $('#ReviewId').val();
    var Rating = $('#Rating').val();
    var ReviewByCustomerId = $('#ReviewByCustomerId').val();
    var ProductId = $('#ProductId').val();
    var ReviewByEmail = $('#ReviewByEmail').val();
    var ReviewByName = $('#ReviewByName').val();
    var ReviewDetail = $('#ReviewDetail').val();

    var st =
    {
        ReviewId: ReviewId,
        Rating: Rating,
        ReviewByCustomerId: ReviewByCustomerId,
        ProductId: ProductId,
        ReviewByEmail: ReviewByEmail,
        ReviewByName: ReviewByName,
        ReviewDetail: ReviewDetail,

    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/User/ProductsReview/Index",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btnSendMessage').html("Save");
            $('#btnSendMessage').prop('disabled', false);
            if (data.status) {
                toastr.success("Review Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
                resetForm('#reviewForm');
                window.location.href = '/User/Home/GetProductDetails?productId=' + productId;
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }

        }
    })
})









