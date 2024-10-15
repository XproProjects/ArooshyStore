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
        "columns": [
            {
                "data": "ImagePath", "width": "45px", "class": "Acenter", "autoWidth": false, "orderable": false, "render": function (data) {
                    return '<img src="' + data + '" style="height:45px;width:45px;" />';
                }
            },
            { "data": "Barcode", "name": "Barcode", "autoWidth": true },
            { "data": "ProductName", "name": "ProductName", "autoWidth": true },
            { "data": "CategoryName", "name": "CategoryName", "autoWidth": true },
            {
                "data": "StatusString", "name": "StatusString", "class": "Acenter", "orderable": true, "autoWidth": false, 'render': function (data) {
                    if (data.toString() == "Active") {
                        return '<span class="badge badge-success badge-pill">Active</span>';
                    }
                    else {
                        return '<span class="badge badge-danger badge-pill">In-Active</span>';
                    }
                }
            },
            //{ "data": "ProductNameUrdu", "name": "ProductNameUrdu", "autoWidth": true },
            { "data": "CostPrice", "name": "CostPrice", "autoWidth": true },
            { "data": "SalePrice", "name": "SalePrice", "autoWidth": true },
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
                "data": "ProductId", "width": "130px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var div = '';
                    div += '<div class="btn-group">' +
                        '<button class="btn btn-primary dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                        '<i class="fas fa-list mr-1"></i> Actions' +
                        '</button>' +
                        '<div class="dropdown-menu">';
                    div += '<a class="dropdown-item btnProductReview" href="javascript:void(0)" data-value="' + data + '" style="text-decoration:none !important;font-weight:normal !important" title="Product Reviews">Product Reviews</a>';
                    div += '<a class="dropdown-item btnAttachDocument btnOpenModal" href="javascript:void(0)" data-toggle="modal" data-target="#MyModal" data-value="' + data + '"style="text-decoration:none !important;font-weight:normal !important" title="Attach Document">Attach Documents</a>';
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
    alert(id);
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
    if (BElement.html() == 'Product Name') {
        oTable.columns(0).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Barcode') {
        oTable.columns(1).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Category') {
        oTable.columns(2).search($('#txtSearch').val().trim()).draw();
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