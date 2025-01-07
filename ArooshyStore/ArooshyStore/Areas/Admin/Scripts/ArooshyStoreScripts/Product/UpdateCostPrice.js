$(document).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();
    }
});

$(function () {
    var $checkoutForm = $('#popupFormCostPrice').validate({
        rules: {
            CostPrice: {
                required: true,
                min: 1
            },
        },
        messages: {
            CostPrice: {
                required: 'Cost Price is required.',
                min : 'Cost Price should be greater than zero.'
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
$('#popupFormCostPrice').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupFormCostPrice").valid()) {
        return false;
    }

    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var ProductId = $('#ProductId').val();
    var CostPrice = $('#CostPrice').val();
    
    var st =
    {
        ProductId: ProductId,
        CostPrice: CostPrice,
    }
   
    $.ajax({
        type: "POST",
        url: "/Admin/Product/UpdateCostPrice/",
        data: JSON.stringify({ 'user': st }),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Product Cost Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })

})
$("#CostPrice").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});