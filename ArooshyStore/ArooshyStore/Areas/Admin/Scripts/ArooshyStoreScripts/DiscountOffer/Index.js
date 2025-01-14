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
                "data": "ImagePath", "width": "45px", "class": "Acenter", "autoWidth": false, "orderable": false, "render": function (data) {
                    return '<a href="' + data + '" target="_blank"><img src="' + data + '" style="height:55px;width:55px;" /></a>';
                }
            },
            { "data": "DiscountName", "name": "DiscountName", "class": "control", "autoWidth": true },
            {
                "data": "ExpiredOn", "name": "ExpiredOn", "width": "120px", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (date) {
                    return getDateForDatatable(date);
                }
            },
            {
                "data": "StatusString", "name": "StatusString", "width": "100px", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (data) {
                    if (data === "Active") {
                        return '<span class="badge badge-success badge-pill">Active</span>';
                    } else {
                        return '<span class="badge badge-danger badge-pill">In-Active</span>';
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
                "data": "OfferId", "width": "130px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var div = '';
                    div += '<div class="btn-group">' +
                        '<button class="btn btn-primary dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                        '<i class="fas fa-list mr-1"></i> Actions' +
                        '</button>' +
                        '<div class="dropdown-menu">';
                    if ($('#EditActionRole').val() > 0) {
                        div += '<a class="dropdown-item AddEditRecord" href="/admin/discountoffer/insertupdatediscountoffer/' + data + '" data-value="' + data + '" title="Edit Offer" style="text-decoration:none !important;font-weight:normal !important">Edit</a>';
                    }
                    if ($('#DeleteActionRole').val() > 0) {
                        div += '<a class="dropdown-item DeleteRecord" href="javascript:void(0)" title="Delete Offer" data-toggle="modal" data-target="#DeleteModal" data-value="' + data + '" style="text-decoration:none !important;font-weight:normal !important">Delete</a>';
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


$(document).on('keydown', function (event) {
    if (event.altKey && event.keyCode === 78) {
        $('#AddEditRecord').click();
    }
})
$(document).on('shown.bs.modal', "#MyModal", function () {
    $('#DiscountName').focus();
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
    if (BElement.html() == 'Product Name / Article #') {
        oTable.columns(0).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Offer Name') {
        oTable.columns(1).search($('#txtSearch').val().trim()).draw();
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
    $('#DeleteModalBody').html("Are you sure you want to delete this Offer?");
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