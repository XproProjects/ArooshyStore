$(function () {
    $('#ModuleType').val($('#HiddenModuleType').val());
    $('#SystemType').val($('#HiddenSystemType').val());
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            ModuleName: {
                required: true
            },
        },
        messages: {
            ModuleName: {
                required: 'Module Name is required.'
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

    var ModuleId = $('#ModuleId').val();
    var ModuleName = $('#ModuleName').val();
    var st =
    {
        ModuleId: ModuleId,
        ModuleName: ModuleName
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/Module/InsertUpdateModule/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Module Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})