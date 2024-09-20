$(function () {
    $('#CityId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetCitiesOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                    type: "master"
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
        placeholder: "-- Select City--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    if ($('#DeliveryId').val() > 0) {
        if ($('#CityId').find("option[value='" + $('#HiddenCityId').val() + "']").length) {
            $('#CityId').val($('#HiddenCityId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenCityName').val(), $('#HiddenCityId').val(), true, true);
            // Append it to the select
            $('#CityId').append(newOption).trigger('change');
        }
    }
    else {
        $('#CityId').val(null).trigger('change');
    }

    var $checkoutForm = $('#popupForm').validate({
        rules: {
            DeliveryCharges: {
                required: true,
                min: 1
            },
            CityId: {
                required: true
            },

        },
        messages: {
            DeliveryCharges: {
                required: 'Delivery Charges is required.',
                min: 'Delivery Charges should be greater than zero.'
            },
            CityId: {
                required: 'City is required.'
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

    var DeliveryId = $('#DeliveryId').val();
    var DeliveryCharges = $('#DeliveryCharges').val();
    var CityId = $('#CityId').val();

    var st =
    {
        DeliveryId: DeliveryId,
        DeliveryCharges: DeliveryCharges,
        CityId: CityId,
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/DeliveryCharges/InsertUpdateDeliveryCharges/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Delivery Charges Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})
$("#DeliveryCharges").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 4, 2);
});