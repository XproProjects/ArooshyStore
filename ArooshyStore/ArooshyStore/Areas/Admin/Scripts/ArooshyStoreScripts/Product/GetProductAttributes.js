$(function () {
    $(".Stock").each(function () {
        if ($("#CreateActionRole").val() > 0) {
            $(this).prop("disabled", false);
        }
        else {
            $(this).prop("disabled", true);
        }
    })
    if ($('.DataTr').length > 0) {
        $("#btnSave").removeAttr("hidden");
    }
    else {
        $("#btnSave").attr("hidden", "hidden");
    }
})

function getRowsData() {
    var arrayData = [];
    $('.DataTr').each(function () {
        var id = $(this).val();
        var ProductAttributeDetailBarcodeId = $(this).find('.ProductAttributeDetailBarcodeId').val();
        var Stock = $(this).find('.Stock').val();
        var alldata = {
            'ProductAttributeDetailBarcodeId': ProductAttributeDetailBarcodeId,
            'StockType': 'Product Stock Page',
            'Stock': Stock,
            'ReferenceId': '',
            'WarehouseId': 0,
        }
        arrayData.push(alldata);
    });
    return arrayData;
}

$(document).off('click', '#btnSave').on('click', '#btnSave', function (e) {
    if ($('.DataTr').length == 0) {
        toastr.error("Error! There is nothing to be saved.", "Error", { timeOut: 3000, "closeButton": true });
        return false;
    }
    $(this).attr('disabled', 'disabled');
    $(this).html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var detail = JSON.stringify(getRowsData());

    $.ajax({
        type: "POST",
        url: "/Admin/Product/InsertUpdateProductStock/",
        data: JSON.stringify({ 'data': detail}),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('#btnSave').html("Save");
            $('#btnSave').prop('disabled', false);
            if (data.status) {
                toastr.success("Product Stock Updated Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $("#txtSearch").focus();
                $('#txtSearch').val('');
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})