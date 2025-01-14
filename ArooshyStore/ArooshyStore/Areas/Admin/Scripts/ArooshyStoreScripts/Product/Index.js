$(function () {
    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,

        "ajax": {
            "url": "/Admin/Product/GetAllProducts",
            "type": "POST",
            "datatype": "json",
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (xhr.status === 410) {
                    window.location.href = customErrorMessage;
                }
            }
        },
        "responsive": {
            "details": {
                "type": "column",
                "target": 1
            }
        },
        "columnDefs": [{
            "className": "control",
            "orderable": true,
            "targets": 1
        }],
        "columns": [
            {
                "data": "ImagePath", "width": "55px", "class": "Acenter", "autoWidth": false, "orderable": false, "render": function (data) {
                    return '<a href="' + data + '" target="_blank"><img src="' + data + '" style="height:55px;width:55px;" /></a>';
                }
            },
            { "data": "ArticleNumber", "name": "ArticleNumber", "class": "ArticleNumberClass control", "autoWidth": true },
            { "data": "ProductName", "name": "ProductName", "class": "ProductNameClass", "autoWidth": true },
            { "data": "CategoryName", "name": "CategoryName", "autoWidth": true },
            {
                "data": "StatusString", "name": "StatusString", "orderable": true, "autoWidth": false, 'render': function (data) {
                    if (data.toString() == "Active") {
                        return '<span class="badge badge-success badge-pill">Active</span>';
                    }
                    else {
                        return '<span class="badge badge-danger badge-pill">In-Active</span>';
                    }
                }
            },
            {
                "data": "IsFeaturedString", "name": "IsFeaturedString", "orderable": true, "autoWidth": false, 'render': function (data) {
                    if (data.toString() == "Yes") {
                        return '<span class="badge badge-success badge-pill">Yes</span>';
                    }
                    else {
                        return '<span class="badge badge-danger badge-pill">No</span>';
                    }
                }
            },
            {
                "data": "ShowOnWebsiteString", "name": "ShowOnWebsiteString", "orderable": true, "autoWidth": false, 'render': function (data) {
                    if (data.toString() == "Yes") {
                        return '<span class="badge badge-success badge-pill">Yes</span>';
                    }
                    else {
                        return '<span class="badge badge-danger badge-pill">No</span>';
                    }
                }
            },
            {
                "data": "SalePrice", "name": "SalePrice", "width": "130px", "class": "Aright", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;font-size:15px">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
                }
            },
            {
                "data": "SalePriceForWebsite", "name": "SalePriceForWebsite", "width": "130px", "class": "Aright", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;font-size:15px">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
                }
            },
            {
                "data": "IsExpiredString", "name": "IsExpiredString", "orderable": true, "autoWidth": false, 'render': function (data) {
                    if (data.toString() == "Yes") {
                        return '<span class="badge badge-success badge-pill">Yes</span>';
                    }
                    else {
                        return '<span class="badge badge-danger badge-pill">No</span>';
                    }
                }
            },
            {
                "data": "SalePriceAfterExpired", "name": "SalePriceAfterExpired", "width": "130px", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;font-size:15px">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
                }
            },
            { "data": "ProductDescription", "name": "ProductDescription", "autoWidth": true },
            {
                "data": "CreatedDate", "name": "CreatedDate", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (date) {
                    return getDateTimeForDatatable(date);
                }
            },
            { "data": "CreatedByString", "name": "CreatedBy", "autoWidth": true },
            {
                "data": "UpdatedDate", "name": "UpdatedDate", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (date) {
                    return getDateTimeForDatatable(date);
                }
            },
            { "data": "UpdatedByString", "name": "UpdatedBy", "autoWidth": true },
            {
                "data": "ProductId", "width": "70px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var detail = '<a title="View Product Detail" href ="/admin/product/productdetail/?id=' + data + '" target="_blank" class="btn btn-secondary" data-value="' + data + '" style="padding:5px;color:#fff;padding-left:15px;padding-right:15px">Detail</a>&nbsp';
                    return detail;
                }
            },
            {
                "data": "ProductId", "width": "160px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var print = '<a title="View and Update / Print Barcode Stickers" data-toggle="modal" data-target="#MyModal" class="btn btn-secondary btnPrint btnOpenModal" href="javascript:void(0)" data-value="' + data + '" style="padding:5px;color:#fff;padding-left:15px;padding-right:15px">Stock / Print Barcode</a>&nbsp';
                    return print;
                }
            },
            {
                "data": "ProductId", "width": "130px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var div = '';
                    div += '<div class="btn-group">' +
                        '<button class="btn btn-primary dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                        '<i class="fas fa-list mr-1"></i> Actions' +
                        '</button>' +
                        '<div class="dropdown-menu">';
                    div += '<a class="dropdown-item btnProductReview" href="javascript:void(0)" data-value="' + data + '" style="text-decoration:none !important;font-weight:normal !important" title="Product Reviews">Product Reviews</a>';
                    if ($('#DocumentActionRole').val() > 0) {
                        div += '<a class="dropdown-item btnAttachDocument btnOpenModal" href="javascript:void(0)" data-toggle="modal" data-target="#MyModal" data-value="' + data + '"style="text-decoration:none !important;font-weight:normal !important" title="Attach Document">Attach Documents</a>';
                    }
                    if ($('#ProductCostActionRole').val() > 0) {
                        div += '<a class="dropdown-item btnProductCost btnOpenModal" data-toggle="modal" data-target="#MyModal" href="javascript:void(0)" data-value="' + data + '" title="Show / Edit Product Cost" style="text-decoration:none !important;font-weight:normal !important">Product Cost</a>';
                    }
                    if ($('#EditActionRole').val() > 0) {
                        div += '<a class="dropdown-item AddEditRecord btnOpenModal" data-toggle="modal" data-target="#MyModal" href="javascript:void(0)" data-value="' + data + '" title="Edit Product" style="text-decoration:none !important;font-weight:normal !important">Edit</a>';
                    }
                    if ($('#DeleteActionRole').val() > 0) {
                        div += '<a class="dropdown-item DeleteRecord" href="javascript:void(0)" title="Delete Product" data-toggle="modal" data-target="#DeleteModal" data-value="' + data + '" style="text-decoration:none !important;font-weight:normal !important">Delete</a>';
                    }

                    div += '</div>' +
                        '</div>';

                    return div;


                }
            },
        ]
    });
    oTable = $('#myTable').DataTable();
});
$(document).on('click', '.btnProductReview', function () {

    var id = $(this).attr("data-value"); // Get the ProductId
    window.location.href = "/Admin/Product/ProductReviews?productId=" + id;
});

$(document).on('click', '.btnAttachDocument', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    $('#modalDiv').html('');
    var id = $(this).attr("data-value");
    $('#ModelHeaderSpan').html('Attach Document');
    $.ajax({
        type: "GET",
        url: "/Admin/Documents/Attachdocument/",
        data: {
            'type': 'Product',
            'id': id
        },
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
})
$(document).on('keydown', function (event) {
    if (event.altKey && event.keyCode === 78) {
        $('#AddEditRecord').click();
    }
})
$(document).on('shown.bs.modal', "#MyModal", function () {
    $('#ProductName').focus();
});

$(document).on('click', '.AddEditRecord', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    if (id > 0) {
        $('#ModelHeaderSpan').html('Edit Product');
    }
    else {
        $('#ModelHeaderSpan').html('Add Product');
    }
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateProduct", "Product")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/Product/InsertUpdateProduct/",
        data: {
            'id': id,
        },
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
});

$(document).on('click', '.btnProductCost', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg");
    var id = $(this).attr("data-value");
    $('#ModelHeaderSpan').html('Product Cost');
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateProduct", "Product")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/Product/UpdateCostPrice/",
        data: {
            'id': id,
        },
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
});
$(document).on('click', '.btnPrint', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    var articleNumber = $(this).parents("tr").find(".ArticleNumberClass").text();
    var productName = $(this).parents("tr").find(".ProductNameClass").text();
    $('#ModelHeaderSpan').html('Stock / Print Barcode Stickers (<span style="font-weight:bold;color:yellow">' + articleNumber + ' - ' + productName + '</span>)');
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateProduct", "Product")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/Product/ProductAttributesBarcodesList/",
        data: {
            'id': id,
        },
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
});
$('#btnSearch').click(function () {
    SearchItem();
});
$('#txtSearch').on('keypress', function (event) {
    if (event.keyCode == 13) {
        SearchItem();
    }
});
function SearchItem() {
    var BElement = $("#btnSearchJobType");
    if (BElement.html() == 'Article #') {
        oTable.columns(0).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Product Name') {
        oTable.columns(1).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Category') {
        oTable.columns(2).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Barcode') {
        oTable.columns(3).search($('#txtSearch').val().trim()).draw();
    }
    else {
        alert("Error! try again.");
    }
}
$("#ulSearchJobType").on("click", "a", function (e) {
    e.preventDefault();
    $("#btnSearchJobType").html($(this).html());
    $("#ulSearchJobType").removeClass("show");
})
$(document).on('click', '.DeleteRecord', function () {
    var id = $(this).attr("data-value");
    $('#DeleteModalTitle').html("Delete Product");
    $('#DeleteModalBody').html("Are you sure you want to delete this Product?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary" onclick="DeleteRecord(' + id + ')">Yes</button>'
    );
});
function DeleteRecord(id) {
    $.ajax({
        url: '/Admin/Product/DeleteProduct/' + id,
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data.status == true) {
                oTable.ajax.reload(null, false);
                $('#DeleteModal').modal('toggle');
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
}