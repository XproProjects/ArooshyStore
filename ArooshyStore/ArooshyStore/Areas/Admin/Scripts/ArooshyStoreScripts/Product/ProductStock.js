$(function () {
    $("#txtSearch").focus();
    LoadProductAttributes();
});
function LoadProductAttributes() {
    $("#ProductAttributesDiv").html('<center>' +
        '<div class="demo" style="margin-top:60px">' +
        '<div class="spinner-grow text-primary" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-secondary" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-success" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-danger" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-warning" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-info" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-light" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '</div>' +
        '</center>');
    $.ajax({
        type: "GET",
        url: "/Admin/Product/GetProductAttributes/?barcode=" + $("#txtSearch").val(),
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $("#ProductAttributesDiv").html(response);
            $("#txtSearch").focus();
            $('#txtSearch').val('');
        }
    })
}
$('#btnSearch').click(function () {
    LoadProductAttributes();
});
$('#txtSearch').on('keypress', function (event) {
    if (event.keyCode == 13) {
        LoadProductAttributes();
    }
});
