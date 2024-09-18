$(function () {
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            TypeName: {
                required: true
            },
           
        },
        messages: {
            TypeName: {
                required: 'Expense Type Name is required.'
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

    var ExpenseTypeId = $('#ExpenseTypeId').val();
    var TypeName = $('#TypeName').val();
    var StatusString = "No";
    if ($("#Status").is(":checked")) {
        StatusString = "Yes";
    }
    var st =
    {
        ExpenseTypeId: ExpenseTypeId,
        TypeName: TypeName,
        StatusString: StatusString,
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/ExpenseType/InsertUpdateExpenseType/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Expense Type Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})