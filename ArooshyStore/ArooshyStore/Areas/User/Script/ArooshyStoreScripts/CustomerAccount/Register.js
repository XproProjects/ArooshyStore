
$(function () {
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            CustomerSupplierName: {
                required: true
            },
            Email: {
                required: true,
                email: true,
            },
            CityId: {
                required: true
            },
            Password: {
                required: true
            },
           
        },
        messages: {
            CustomerSupplierName: {
                required: 'Customer Name is required.'
            },
          
            Email: {
                required: 'Email Is required.',
                email: 'Please enter a valid email address.',
            },
             CityId: {
                required: 'City is required.'
            },
            Password: {
                required: 'Password is required.'
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
    var captchaResponse = grecaptcha.getResponse();
    if (captchaResponse.length === 0) {
        $('#Captcha').text('Captcha is required.').css('color', 'red');
        return false; 
    } else {
        $('#Captcha').text(''); 
    }
    if (!$("#popupForm").valid()) {
        return false;
    }
    if (!$('#flexSwitchCheckChecked').is(':checked')) {
        $('#termsError').show(); 
        return; 
    } else {
        $('#termsError').hide(); 
    }
    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var CustomerSupplierId = $('#CustomerSupplierId').val();
    var CustomerSupplierName = $('#CustomerSupplierName').val();
    var CustomerSupplierType = "Customer";
    var Contact1 = $('#Contact1').val();
    var Contact2 = $('#Contact2').val();
    var Email = $('#Email').val();
    var Password = $('#Password').val();
    var isChangePassword = $('#IsChangePassword').val();
    var HouseNo = $('#HouseNo').val();
    var Street = $('#Street').val();
    var ColonyOrVillageName = $('#ColonyOrVillageName').val();
    var PostalCode = $('#PostalCode').val();
    var CityId = $('#CityId').val();
    var CompleteAddress = $('#CompleteAddress').val();
    var CreditDays = $('#CreditDays').val();
    var CreditLimit = $('#CreditLimit').val();
    var Remarks = $('#Remarks').val();
    var StatusString = "No";
    if ($("#Status").is(":checked")) {
        StatusString = "Yes";
    }
    var st =
    {
        CustomerSupplierId: CustomerSupplierId,
        CustomerSupplierName: CustomerSupplierName,
        CustomerSupplierType: CustomerSupplierType,
        Contact1: Contact1,
        Contact2: Contact2,
        Email: Email,
        Password: Password,
        IsChangePassword: isChangePassword,
        HouseNo: HouseNo,
        Street: Street,
        ColonyOrVillageName: ColonyOrVillageName,
        PostalCode: PostalCode,
        CityId: CityId,
        CreditDays: CreditDays,
        CreditLimit: CreditLimit,
        Remarks: Remarks,
        CompleteAddress: CompleteAddress,
        StatusString: StatusString,
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/User/CustomerAccount/Register/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Account Has Been Created Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                setTimeout(function () {
                    window.location.href = '/user/customeraccount/login';
                }, 3000); 

                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})