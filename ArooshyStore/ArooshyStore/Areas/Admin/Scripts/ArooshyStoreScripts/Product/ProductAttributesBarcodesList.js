$(function () {
    
})
function getRowsData() {
    var arrayData = [];
    $('.DataTr').each(function () {
        var id = $(this).val();
        var ProductAttributeDetailBarcodeId = $(this).find('.ProductAttributeDetailBarcodeId').val();
        var ProductId = $(this).find('.ProductId').val();
        var AttributeDetailId1 = $(this).find('.AttributeDetailId1').val();
        var AttributeDetailId2 = $(this).find('.AttributeDetailId2').val();
        var Barcode = $(this).find('.Barcode').val();
        var barcodePrintValue = $(this).find('.TotalRecords').val();
        if (barcodePrintValue > 0) {
            var alldata = {
                'ProductAttributeDetailBarcodeId': ProductAttributeDetailBarcodeId,
                'ProductId': ProductId,
                'AttributeDetailId1': AttributeDetailId1,
                'AttributeDetailId2': AttributeDetailId2,
                'Barcode': Barcode,
                'TotalRecords': barcodePrintValue,
            }
            arrayData.push(alldata);
        }
    });
    return arrayData;
}
function CheckSectionsData() {
    var check = 0;
    $('.DataTr').each(function () {
        var barcodePrintValue = $(this).find('.TotalRecords').val();
        if (barcodePrintValue > 0) {
            check++;
        }
    });
    return check;
}

$('#popupPrintForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupPrintForm").valid()) {
        return false;
    }

    if (CheckSectionsData() == 0) {
        toastr.error("Please print atleast one barcode.", "Error", { timeOut: 3000, "closeButton": true });
        return false;
    }
    $('#btnSavePrint').attr('disabled', 'disabled');
    $('#btnSavePrint').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var detail = JSON.stringify(getRowsData());

    $.ajax({
        type: "POST",
        url: "/Admin/Product/PrintBarcodeStickers/",
        data: JSON.stringify({ 'data': detail }),
        //dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $("body").html(response);
            //$('#btn_Save').html("Save");
            //$('#btn_Save').prop('disabled', false);
            //if (data.status) {
            //    var extension = $('#ProfileImage').val().split('.').pop().toLowerCase();
               
            //}
            //else {
            //    toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            //}
        }
    })

})
$("#CostPrice").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});
$("#SalePrice").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});
$("#SalePriceForWebsite").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});
$("#SalePriceAfterExpired").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});