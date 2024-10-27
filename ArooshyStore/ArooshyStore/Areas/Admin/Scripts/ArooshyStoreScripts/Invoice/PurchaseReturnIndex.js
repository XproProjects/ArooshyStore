
﻿$(function () {
    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,

        "ajax": {
            "url": "/Admin/Invoice/GetAllInvoices",
            "type": "POST",
            "datatype": "json",
            "data": function (d) {
                d.Type = 'Purchase Return';
                d.From = '';
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (xhr.status === 410) {
                    window.location.href = customErrorMessage;
                }
            }
        },
        "columns": [
            { "data": "InvoiceNumber", "name": "InvoiceNumber", "width": "120px", "autoWidth": false },
            {
                "data": "InvoiceDate", "name": "Date", "width": "120px", "class": "Acenter", "orderable": true, "autoWidth": false, 'render': function (date) {
                    return getDateForDatatable(date);
                }
            },
            {
                "data": "NetAmount", "name": "NetAmount", "width": "130px", "class": "Aright", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;font-size:15px">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
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
                "data": "InvoiceNumber", "width": "180px", "class": "Acenter", "orderable": false, "autoWidth": false, "render": function (data) {
                    var div = '';
                    div += '<div class="btn-group">' +
                        '<button class="btn btn-primary dropdown-toggle waves-effect waves-themed" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                        '<i class="fas fa-list mr-1"></i> Actions' +
                        '</button>' +
                        '<div class="dropdown-menu">';

                    if ($('#EditActionRole').val() > 0) {
                        div += '<a class="dropdown-item AddEditRecord btnOpenModal btnAddEdit" href="javascript:void(0)" data-value="' + data + '" title="Edit Sale Invoice"  style="text-decoration:none !important;font-weight:normal !important">Edit</a>';
                    }
                    if ($('#DeleteActionRole').val() > 0) {
                        div += '<a class="dropdown-item DeleteRecord" href="javascript:void(0)" title="Delete Sale Invoice" data-toggle="modal" data-target="#DeleteModal"  data-value="' + data + '" style="text-decoration:none !important;font-weight:normal !important">Delete</a>';
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
    $('#UnitName').focus();
});
$(document).on('click', '.btnAddEdit', function () {
    //$("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    window.location.href = "/Admin/invoice/insertupdatepurchasereturn/?id=" + id + "&type=addedit";


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
    if (BElement.html() == 'Invoice Number') {
        oTable.columns(0).search($('#txtSearch').val().trim()).draw();
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
    $('#DeleteModalTitle').html("Delete Sale Invoice");
    $('#DeleteModalBody').html("Are you sure you want to delete this Purchase Return Invoice?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary btnYesDelete" data-value="' + id + '">Yes</button>'
    );
});
$(document).off('click', '.btnYesDelete').on('click', '.btnYesDelete', function () {
    var id = $(this).attr("data-value");
    DeleteRecord(id);
});
function DeleteRecord(id) {
    $.ajax({
        url: '/Admin/Invoice/DeleteInvoice/' + id,
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
