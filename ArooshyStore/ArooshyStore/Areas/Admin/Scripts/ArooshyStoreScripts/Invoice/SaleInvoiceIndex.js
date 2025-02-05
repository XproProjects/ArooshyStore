$('input[type=radio][name=MoreFilterRadio]').change(function () {
    var radioValue = $("input[name='MoreFilterRadio']:checked").val();
    if (radioValue.toString().toLowerCase() == 'date') {
        $('.DateDiv').removeAttr("hidden");
        $('.MonthDiv').attr("hidden", "hidden");
        $('.BetweenDatesDiv').attr("hidden", "hidden");
    }
    else if (radioValue.toString().toLowerCase() == 'month') {
        $('.MonthDiv').removeAttr("hidden");
        $('.DateDiv').attr("hidden", "hidden");
        $('.BetweenDatesDiv').attr("hidden", "hidden");
    }
    else if (radioValue.toString().toLowerCase() == 'between dates') {
        $('.BetweenDatesDiv').removeAttr("hidden");
        $('.DateDiv').attr("hidden", "hidden");
        $('.MonthDiv').attr("hidden", "hidden");
    }
    else {
        $('.BetweenDatesDiv').attr("hidden", "hidden");
        $('.DateDiv').attr("hidden", "hidden");
        $('.MonthDiv').attr("hidden", "hidden");
    }
    LoadDataTable();
});
$('#DateFilter,#MonthFilter,#FromDateFilter,#ToDateFilter').change(function () {
    LoadDataTable();
})

$(function () {
    $('.nav-menu li a[href="/admin/invoice/saleinvoiceindex/?from=' + $("#From").val() + '"]').parent("li").addClass('active');
    $('.nav-menu li a[href="/admin/invoice/saleinvoiceindex/?from=' + $("#From").val() + '"]').parent("li").parent("ul").css('display', "block");
    $('.nav-menu li a[href="/admin/invoice/saleinvoiceindex/?from=' + $("#From").val() + '"]').parent("li").parent("ul").parent("li").removeClass("open").addClass("open");
    $('.nav-menu li a[href="/admin/invoice/saleinvoiceindex/?from=' + $("#From").val() + '"]').parent("li").parent("ul").parent("li").find("a").find("b").find("em").removeClass("fa-angle-down").removeClass("fa-angle-up").addClass("fa-angle-up");
    LoadDataTable();

});
function LoadDataTable() {
    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,
        "bDestroy": true,
        "fnInitComplete": function (oSettings, json) {
            getInvoiceItemsList();
        },
        "ajax": {
            "url": "/Admin/Invoice/GetAllInvoices",
            "type": "POST",
            "datatype": "json",
            "data": function (d) {
                d.Type = 'Sale Invoice';
                d.From = $("#From").val();
                d.FilterType = $("input[name='MoreFilterRadio']:checked").val();
                d.DateFilter = $("#DateFilter").val();
                d.MonthFilter = $("#MonthFilter").val();
                d.FromDateFilter = $("#FromDateFilter").val();
                d.ToDateFilter = $("#ToDateFilter").val();
                d.TextboxFilter = $('#txtSearch').val();
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (xhr.status === 410) {
                    window.location.href = customErrorMessage;
                }
            }
        },
        "columns": [
            { "data": "InvoiceNumber", "name": "InvoiceNumber", "class": "InvoiceNumberClass Acenter", "width": "120px", "autoWidth": false },
            {
                "data": "InvoiceDate", "name": "InvoiceDate", "width": "130px", "class": "Acenter", "orderable": true, "autoWidth": false, 'render': function (date) {
                    return getDateTimeForDatatable(date);
                }
            },
            { "data": "CustomerName", "name": "CustomerName", "autoWidth": false },
            {
                "data": "Status", "name": "Status", "class": "Acenter", "width": "130px", "orderable": true, "autoWidth": false, 'render': function (data) {
                    if (data.toString().toLowerCase() == "ordered") {
                        return '<span class="badge badge-info badge-pill" style="color:#fff;font-weight:bold;font-size:14px">Ordered</span>';
                    }
                    else if (data.toString().toLowerCase() == "confirmed") {
                        return '<span class="badge badge-warning badge-pill" style="color:#fff;font-weight:bold;font-size:14px">Confirmed</span>';
                    }
                    else if (data.toString().toLowerCase() == "on hold") {
                        return '<span class="badge badge-dark badge-pill" style="color:#fff;font-weight:bold;font-size:14px">On Hold</span>';
                    }
                    else if (data.toString().toLowerCase() == "dispatched") {
                        return '<span class="badge badge-success badge-pill" style="color:#fff;font-weight:bold;font-size:14px">Dispatched</span>';
                    }
                    else if (data.toString().toLowerCase() == "delivered") {
                        return '<span class="badge badge-success badge-pill" style="color:#fff;font-weight:bold;font-size:14px">Delivered</span>';
                    }
                    else if (data.toString().toLowerCase() == "exchanged") {
                        return '<span class="badge badge-success badge-pill" style="color:#fff;font-weight:bold;font-size:14px">Exchanged</span>';
                    }
                    else if (data.toString().toLowerCase() == "returned") {
                        return '<span class="badge badge-danger badge-pill" style="color:#fff;font-weight:bold;font-size:14px">Returned</span>';
                    }
                    else if (data.toString().toLowerCase() == "rejected") {
                        return '<span class="badge badge-danger badge-pill" style="color:#fff;font-weight:bold;font-size:14px">Rejected</span>';
                    }
                    else if (data.toString().toLowerCase() == "cancelled") {
                        return '<span class="badge badge-danger badge-pill" style="color:#fff;font-weight:bold;font-size:14px">Cancelled</span>';
                    }
                    else {
                        return '<span class="badge badge-danger badge-pill" style="color:#fff;font-weight:bold;font-size:14px">No Status</span>';
                    }
                }
            },
            {
                "data": "InvoiceNumber", "width": "70px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var detail = '';
                    detail = '<span style="color:green;font-weight:bold;font-size:15px" class="TotalItemsSpanInList" data-value="' + data + '">....</span><br /><a style="font-weight:bold" href="javascript:void(0)" data-value="' + data + '" data-toggle="modal" data-target="#MyModal" class="btnViewItemsDetail btnOpenModal">View Detail</a> ';
                    return detail;
                }
            },
            {
                "data": "NetAmount", "name": "NetAmount", "width": "130px", "class": "Aright", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;font-size:15px">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
                }
            },
            {
                "data": "TotalAmount", "name": "TotalAmount", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;font-size:15px">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
                }
            },
            { "data": "DiscType", "name": "DiscType", "autoWidth": false },
            {
                "data": "DiscRate", "name": "DiscRate", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
                }
            },
            {
                "data": "DiscAmount", "name": "DiscAmount", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
                }
            },
            {
                "data": "DeliveryCharges", "name": "DeliveryCharges", "orderable": true, "autoWidth": false, 'render': function (data) {
                    return '<span style="color:green;font-weight:bold;">' + ReplaceNumberWithCommas(parseFloat(data).toFixed(2)) + '</span>';
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
                "data": "InvoiceNumberWithStatus", "width": "150px", "class": "Acenter", "orderable": false, "autoWidth": false, "render": function (data) {
                    var invoiceNumber = data.split('|')[0];
                    var status = data.split('|')[1];
                    var div = '';
                    div += '<div class="btn-group">' +
                        '<button class="btn btn-primary dropdown-toggle waves-effect waves-themed" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                        '<i class="fas fa-list mr-1"></i> Actions' +
                        '</button>' +
                        '<div class="dropdown-menu">';
                    div += '<a class="dropdown-item" target="_blank" href="/admin/invoice/printinvoice/?id=' + invoiceNumber + '" title="Print Invoice" style="text-decoration:none !important">Print</a>';
                    if (status != 'Returned' && status != 'returned') {
                        if ($('#EditActionRole').val() > 0) {
                            div += '<a class="dropdown-item btnUpdateStatus btnOpenModal" href="javascript:void(0)" data-toggle="modal" data-target="#MyModal" data-value="' + invoiceNumber + '" style="text-decoration:none !important;font-weight:normal !important" title="Update Status">Update Status</a>';
                        }
                        if ($('#ExchangeActionRole').val() > 0) {
                            div += '<a class="dropdown-item AddEditRecord btnOpenModal btnExchange" href="javascript:void(0)" data-value="' + invoiceNumber + '" title="Exchange Sale Invoice"  style="text-decoration:none !important;font-weight:normal !important">Exchange</a>';
                        }
                        if ($('#ReturnActionRole').val() > 0) {
                            div += '<a class="dropdown-item ReturnRecord" href="javascript:void(0)" title="Return Sale Invoice" data-toggle="modal" data-target="#DeleteModal"  data-value="' + invoiceNumber + '" style="text-decoration:none !important;font-weight:normal !important">Return</a>';
                        }
                        if ($('#EditActionRole').val() > 0) {
                            div += '<a class="dropdown-item AddEditRecord btnOpenModal btnAddEdit" href="javascript:void(0)" data-value="' + invoiceNumber + '" title="Edit Sale Invoice"  style="text-decoration:none !important;font-weight:normal !important">Edit</a>';
                        }
                    }
                    if ($('#DeleteActionRole').val() > 0) {
                        div += '<a class="dropdown-item DeleteRecord" href="javascript:void(0)" title="Delete Sale Invoice" data-toggle="modal" data-target="#DeleteModal"  data-value="' + invoiceNumber + '" style="text-decoration:none !important;font-weight:normal !important">Delete</a>';
                    }
                    div += '</div>' +
                        '</div>';

                    return div;
                }
            },
        ]
    });
    oTable = $('#myTable').DataTable();
}
function getInvoiceItemsList() {
    $(".TotalItemsSpanInList").each(function () {
        var id = $(this).attr("data-value");
        var ref = $(this);
        $.ajax({
            type: "POST",
            url: "/Admin/Invoice/GetTotalInvoiceItems/",
            dataType: 'json',
            data: { 'id': id },
            success: function (response) {
                ref.html(response);
            }
        })
    })
}

$(document).on('click', '.btnViewItemsDetail', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    var invoiceNumber = $(this).parents("tr").find(".InvoiceNumberClass").text();
    $('#ModelHeaderSpan').html('Invoice Items List (<span style="font-weight:bold;color:yellow">' + invoiceNumber + '</span>)');
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateProduct", "Product")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/Invoice/InvoiceDetailsList/",
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
    var type = '';
    if (id == "0") {
        type = 'new';
    }
    else {
        type = 'edit';
    }
    window.location.href = "/admin/invoice/insertupdatesaleinvoice/?id=" + id + "&type=" + type + "";
});
$(document).on('click', '.btnExchange', function () {
    //$("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    window.location.href = "/admin/invoice/insertupdatesaleinvoice/?id=" + id + "&type=exchange";
});
$('#btnSearch').click(function () {
    LoadDataTable();
});
$('#txtSearch').on('keypress', function (event) {
    if (event.keyCode == 13) {
        LoadDataTable();
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
    $('#DeleteModalBody').html("Are you sure you want to delete this Sale Invoice?");
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

$(document).on('click', '.ReturnRecord', function () {
    var id = $(this).attr("data-value");
    $('#DeleteModalTitle').html("Return Sale Invoice");
    $('#DeleteModalBody').html("Are you sure you want to return this Sale Invoice?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary btnYesReturn" data-value="' + id + '">Yes</button>'
    );
});
$(document).off('click', '.btnYesReturn').on('click', '.btnYesReturn', function () {
    var id = $(this).attr("data-value");
    ReturnRecord(id);
});
function ReturnRecord(id) {
    $.ajax({
        url: '/Admin/Invoice/ReturnInvoice/' + id,
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

$(document).on('click', '.btnUpdateStatus', function () {
    var id = $(this).attr("data-value");
    $('#ModelHeaderSpan').html('Update Invoice Status');
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateCity", "City")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/Invoice/InsertUpdateInvoiceStatus/",
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
