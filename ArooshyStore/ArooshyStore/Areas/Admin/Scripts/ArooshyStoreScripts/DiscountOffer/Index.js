$(function () {
    $('.nav-menu li a[href="/admin/home/setting"]').parent("li").addClass('active');
    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,

        "ajax": {
            "url": "/Admin/DiscountOffer/GetAllDiscountOffers",
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
            { "data": "DiscountName", "name": "DiscountName", "autoWidth": true },
            { "data": "DiscPercent", "name": "DiscPercent", "class": "Acenter", "autoWidth": true  },
            { "data": "SelectType", "name": "SelectType", "class": "Acenter", "autoWidth": true },
            {
                "data": "StatusString", "name": "StatusString", "width": "130px", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (data) {
                    if (data === "Active") {
                        return '<span class="badge badge-success badge-pill">Active</span>';
                    } else {
                        return '<span class="badge badge-danger badge-pill">In-Active</span>';
                    }
                }
            },
            { "data": "CategoryName", "name": "CategoryName", "class": "Acenter", "autoWidth": true },
            { "data": "ProductName", "name": "ProductName", "class": "Acenter", "autoWidth": true },
            {
                "data": "ExpiredOn", "name": "ExpiredOn", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (date) {
                    return getDateForDatatable(date);
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
                "data": "OfferId", "width": "130px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var edit = '<a title="Edit Discount Offer" data-toggle="modal" data-target="#MyModal" class="AddEditRecord btn btn-primary btnOpenModal" href="javascript:void(0)" data-value="' + data + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-edit"></i></a>&nbsp';
                    var del = '<a title="Delete Discount Offer" class="DeleteRecord btn btn-danger" data-toggle="modal" data-target="#DeleteModal" href="javascript:void(0)" data-value="' + data + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-times-circle"></i></a>&nbsp';
                    var final = '';
                    if ($('#EditActionRole').val() > 0) {
                        final = final + edit;
                    }
                    if ($('#DeleteActionRole').val() > 0) {
                        final = final + del;
                    }
                    return final;
                }
            },
        ]
    });
    oTable = $('#myTable').DataTable();
});


$(document).on('keydown', function (event) {
    if (event.altKey && event.keyCode === 78) {
        $('#AddEditRecord').click();
    }
})
$(document).on('shown.bs.modal', "#MyModal", function () {
    $('#DiscountName').focus();
});
$(document).on('click', '.AddEditRecord', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");

    var id = $(this).attr("data-value");
    if (id > 0) {
        $('#ModelHeaderSpan').html('Edit Discount Offer');
    }
    else {
        $('#ModelHeaderSpan').html('Add Discount Offer');
    }
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateOffer", "Offer")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/DiscountOffer/InsertUpdateDiscountOffer/",
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
    if (BElement.html() == 'Offer Name') {
        oTable.columns(0).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Discount on Category Name') {
        oTable.columns(1).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Discount on Product Name') {
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
    $('#DeleteModalTitle').html("Delete Discount Offer");
    $('#DeleteModalBody').html("Are you sure you want to delete this Discount Offer?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary" onclick="DeleteRecord(' + id + ')">Yes</button>'
    );
});
function DeleteRecord(id) {
    $.ajax({
        url: '/Admin/DiscountOffer/DeleteDiscountOffer/' + id,
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