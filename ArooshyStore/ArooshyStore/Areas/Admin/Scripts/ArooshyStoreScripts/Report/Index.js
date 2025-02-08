$(function () {
    
});

$(document).on('click', '.btnSaleReport', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    $('#ModelHeaderSpan').html('Sale Report');
    $('#modalDiv').html('');
    $.ajax({
        type: "GET",
        url: "/Admin/Report/SaleReport/?type='Sale'",
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
});
$(document).on('click', '.btnPurchaseReport', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    $('#ModelHeaderSpan').html('Purchase Report');
    $('#modalDiv').html('');
    $.ajax({
        type: "GET",
        url: "/Admin/Report/SaleReport/?type='Purchase'",
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
});
$(document).on('click', '.btnProductSaleReport', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    $('#ModelHeaderSpan').html('Product Sale Report');
    $('#modalDiv').html('');
    $.ajax({
        type: "GET",
        url: "/Admin/Report/ProductSaleReport/",
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
});
$(document).on('click', '.btnStockReport', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    $('#ModelHeaderSpan').html('Product Stock Report');
    $('#modalDiv').html('');
    $.ajax({
        type: "GET",
        url: "/Admin/Report/ProductStockReport/",
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
});