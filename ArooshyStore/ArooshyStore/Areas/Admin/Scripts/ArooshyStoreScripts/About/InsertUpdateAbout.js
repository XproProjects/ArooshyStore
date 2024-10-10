$(function () {
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            Description: {
                required: true
            },
            Service1Name: {
                required: true
            },
            Service2Name: {
                required: true
            },
        },
        messages: {
            Description: {
                required: 'Description is required.'
            },
            Service1Name: {
                required: 'Service 1 Name is required.',
            },
            Service2Name: {
                required: 'Service 2 Name is required.'
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

    var AboutId = $('#AboutId').val();
    var Description = $('#Description').val();
    var Service1Name = $('#Service1Name').val();
    var Service1Icon = $('#Service1Icon').val();
    var Service1Description = $('#Service1Description').val();
    var Service2Name = $('#Service2Name').val();
    var Service2Icon = $('#Service2Icon').val();
    var Service2Description = $('#Service2Description').val();
    var Service3Name = $('#Service3Name').val();
    var Service3Icon = $('#Service3Icon').val();
    var Service3Description = $('#Service3Description').val();
    var st =
    {
        AboutId: AboutId,
        Description: Description,
        Service1Name: Service1Name,
        Service1Icon: 'fa ' + Service1Icon,
        Service1Description: Service1Description,
        Service2Name: Service2Name,
        Service2Icon: 'fa ' + Service2Icon,
        Service2Description: Service2Description,
        Service3Name: Service3Name,
        Service3Icon: 'fa ' + Service3Icon,
        Service3Description: Service3Description,

    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/About/InsertUpdateAbout/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("About Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})