$(function () {
    if ($('#ExpenseId').val() > 0) {

        if ($('#ExpenseTypeId').find("option[value='" + $('#HiddenExpenseTypeId').val() + "']").length) {
            $('#ExpenseTypeId').val($('#HiddenExpenseTypeId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenTypeName').val(), $('#HiddenExpenseTypeId').val(), true, true);
            // Append it to the select
            $('#ExpenseTypeId').append(newOption).trigger('change');
        }
    }
    else {
        $('#UnitId').val(null).trigger('change');

    }
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            ExpenseName: {
                required: true
            },
            ExpenseTypeId: {
                required: true
            },
            ExpenseDate: {
                required: true
            },
            ExpenseAmount: {
                required: true,
                min:1
            },

        },
        messages: {
            ExpenseName: {
                required: 'Expense Name is required.'
            },
            ExpenseTypeId: {
                required: 'Expense Type is required.'
            },
            ExpenseDate: {
                required: 'Expense Date is required.'
            },
            ExpenseAmount: {
                required: 'Expense Amount is required.',
                min:'Expense Amount should be greater than zero.'
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
$('#ExpenseTypeId').select2({
    ajax: {
        delay: 150,
        url: '/Admin/Combolist/GetExpenseTypesOptionList/',
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
    placeholder: "-- Select Expense Type--",
    minimumInputLength: 0,
    dropdownParent: $(".mySelect"),
    allowClear: true,
});
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

    var ExpenseId = $('#ExpenseId').val();
    var ExpenseName = $('#ExpenseName').val();
    var PaidTo = $('#PaidTo').val();
    var PaidFrom = $('#PaidFrom').val();
    var ExpenseAmount = $('#ExpenseAmount').val();
    var ExpenseDate = $('#ExpenseDate').val();
    var ExpenseTypeId = $('#ExpenseTypeId').val();

    var st =
    {
        ExpenseId: ExpenseId,
        ExpenseName: ExpenseName,
        PaidTo: PaidTo,
        PaidFrom: PaidFrom,
        ExpenseAmount: ExpenseAmount,
        ExpenseDate: ExpenseDate,
        ExpenseTypeId: ExpenseTypeId,
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/Expense/InsertUpdateExpense/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Expense Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})
$("#ExpenseAmount").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});