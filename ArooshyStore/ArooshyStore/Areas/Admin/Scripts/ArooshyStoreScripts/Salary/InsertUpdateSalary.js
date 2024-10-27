function calculateNetAmount() {
    let basicSalary = parseFloat($('#BasicSalary').val()) || 0;
    let bonusAmount = parseFloat($('#BonusAmount').val()) || 0;
    let advanceSalary = parseFloat($('#AdvanceSalary').val()) || 0;
    let deductionAmount = parseFloat($('#DeductionAmount').val()) || 0;

    let loan = parseFloat($('#Loan').val()) || 0;
    let netAmount = basicSalary + bonusAmount - advanceSalary - loan - deductionAmount;
    $('#NetSalary').val(netAmount.toFixed(2));
}

// Event listeners for real-time calculation
$('#BasicSalary, #BonusAmount, #AdvanceSalary, #Loan,#DeductionAmount').on('input', function () {
    calculateNetAmount();
});

$(function () {
    $('#EmployeeId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetEmployeesOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                return {
                    results: data.Results,
                    pagination: {
                        more: (params.page * 20) < data.Total
                    }
                };
            }
        },
        placeholder: "-- Select Employee--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    if ($('#SalaryId').val() > 0) {
        if ($('#EmployeeId').find("option[value='" + $('#HiddenEmployeeId').val() + "']").length) {
            $('#EmployeeId').val($('#HiddenEmployeeId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenEmployeeName').val(), $('#HiddenEmployeeId').val(), true, true);
            // Append it to the select
            $('#EmployeeId').append(newOption).trigger('change');
        }
    }
    else {
        $('#EmployeeId').val(null).trigger('change');
    }
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            EmployeeId: {
                required: true
            },
            BasicSalary: {
                required: true
            }
        },
        messages: {
            EmployeeId: {
                required: 'Employee Name is required.'
            },
            BasicSalary: {
                required: 'Salary  is required.'

            }
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

    var SalaryId = $('#SalaryId').val();
    var EmployeeId = $('#EmployeeId').val();
    var BasicSalary = $('#BasicSalary').val();
    var GrossAmount = $('#BasicSalary').val();
    var AdvanceSalary = $('#AdvanceSalary').val();
    var Loan = $('#Loan').val();
    var BonusAmount = $('#BonusAmount').val();
    var DeductionAmount = $('#DeductionAmount').val();
    var EmployeeId = $('#EmployeeId').val();
    var ForMonth = $('#ForMonth').val();
    var NetSalary = $('#NetSalary').val();
    var st =
    {
        SalaryId: SalaryId,
        EmployeeId: EmployeeId,
        BasicSalary: BasicSalary,
        AdvanceSalary: AdvanceSalary,
        Loan: Loan,
        BonusAmount: BonusAmount,
        DeductionAmount: DeductionAmount,
        ForMonth: ForMonth,
        GrossAmount: GrossAmount,
        NetSalary: NetSalary,
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/Salary/InsertUpdateSalary/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Salary Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})