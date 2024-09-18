$(function () {
    $("#Contact1").inputmask();
    $("#Contact2").inputmask();
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            WarehouseName: {
                required: true
            },
            Email: {
                email: true
            },
            Contact1: {
                required: true
            },
        },
        messages: {
            WarehouseName: {
                required: 'Warehouse Name is required.'
            },
            Email: {
                email: 'Please enter a valid email address.',
            },
            Contact1: {
                required: 'Contact 1 is required.'
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
$('#popupForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupForm").valid()) {
        return false;
    }
    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var WarehouseId = $('#WarehouseId').val();
    var WarehouseName = $('#WarehouseName').val();
    var Email = $('#Email').val();
    var Contact1 = $('#Contact1').val();
    var Contact2 = $('#Contact2').val();
    var Address = $('#Address').val();
    var StatusString = "No";
    if ($("#Status").is(":checked")) {
        StatusString = "Yes";
    }
    var st =
    {
        WarehouseId: WarehouseId,
        WarehouseName: WarehouseName,
        Email: Email,
        Contact1: Contact1,
        Contact2: Contact2,
        Address: Address,
        StatusString: StatusString,
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/Warehouse/InsertUpdateWarehouse/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Warehouse Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})