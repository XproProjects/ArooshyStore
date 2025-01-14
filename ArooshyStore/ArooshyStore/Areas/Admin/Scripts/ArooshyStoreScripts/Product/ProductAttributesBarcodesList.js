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
function getRowsDataForUpdateStock() {
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
    var titleVal = $(this).find("button:focus").attr("data-value");
    $('.btnSaveForm').attr('disabled', 'disabled');
    $('.btnSaveForm').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    if (titleVal == 0) {
        PrintBarcodes();
    }
    else if (titleVal == 1) {
        UpdateStock("");
    }
    else if (titleVal == 2) {
        UpdateStock("print");
    }
})

function PrintBarcodes() {
    if (CheckSectionsData() == 0) {
        toastr.error("Please print atleast one barcode.", "Error", { timeOut: 3000, "closeButton": true });
        $('.btnSaveForm').prop('disabled', false);
        $('#btnSavePrint').html("Print Barcodes");
        $('#btnUpdateStock').html("Update Stock");
        $('#btnUpdateStockPrintBarcode').html("Update Stock and Print Barcodes");
        return false;
    }
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
}

function UpdateStock(print) {
    var detail = JSON.stringify(getRowsDataForUpdateStock());

    $.ajax({
        type: "POST",
        url: "/Admin/Product/InsertUpdateProductStock/",
        data: JSON.stringify({ 'data': detail }),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.status) {
                if (print == "print") {
                    toastr.success("Product Stock Updated Successfully", "Success", { timeOut: 3000, "closeButton": true });
                    setTimeout(function () {
                        PrintBarcodes();
                    }, 2000);
                }
                else {
                    $('.btnSaveForm').prop('disabled', false);
                    $('#btnSavePrint').html("Print Barcodes");
                    $('#btnUpdateStock').html("Update Stock");
                    $('#btnUpdateStockPrintBarcode').html("Update Stock and Print Barcodes");
                    toastr.success("Product Stock Updated Successfully", "Success", { timeOut: 3000, "closeButton": true });
                }
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
}