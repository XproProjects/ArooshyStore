function getCookie(name) {
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i].trim();
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
$(function () {
    var $checkoutForm = $('#checkoutForm').validate({
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
            HouseNo: {
                required: true
            },
            Contact1: {
                required: true
            }
        },
        messages: {
            CustomerSupplierName: {
                required: 'Customer Name is required.'
            },
            Email: {
                required: 'Email is required.',
                email: 'Please enter a valid email address.'
            },
            CityId: {
                required: 'City is required.'
            },
          
            HouseNo: {
                required: 'House Number is required.'
            },
            Contact1: {
                required: 'Contact 1 is required.'
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
            if (element.val() === '' || element.val() == null) {
                element.parents('td').siblings('td').find('.btn').css("margin-top", "-8.5px");
            }
        }
    });
});

$(document).on('change', 'select', function () {
    $(this).parents('td').siblings('td').find('.btn').css("margin-top", "14px");
    $(this).parents('label').siblings('em').remove();
});

$('#checkoutForm').on('submit', function (e) {
    e.preventDefault();

    // Check if the form is valid before proceeding
    if (!$('#checkoutForm').valid()) {
        return;
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
    var st = {
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
        StatusString: StatusString
    };

    $.ajax({
        type: "POST",
        url: "/User/CheckOut/CheckOutDetail/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Your Information Has Been Created Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                const cookieName = getCookie('CookieName');
                if (cookieName) {
                    console.log("Redirecting to: /User/CheckOut/CheckOutReview?cookieName=" + cookieName);
                    setTimeout(function () {
                        window.location.href = "/User/CheckOut/CheckOutReview?cookieName=" + cookieName;
                    }, 3000); 

                } else {
                    toastr.error('No cart cookie found!', 'Error', { timeOut: 3000, closeButton: true });
                }
            } else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    });
});
