﻿$(function () {
    $('.nav-menu li a[href="/admin/customersupplier/index/?type=' + $("#Type").val()+'"]').parent("li").addClass('active');
    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,

        "ajax": {
            "url": "/Admin/CustomerSupplier/GetAllCustomerSupplier",
            "type": "POST",
            "datatype": "json",
            "data": function (d) {
                d.Type = $("#Type").val()
            },

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
            { "data": "CustomerSupplierName", "name": "CustomerSupplierName", "class": "control", "autoWidth": true },
            { "data": "Email", "name": "Email", "autoWidth": true },
            { "data": "CityName", "name": "CityName", "autoWidth": true },
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
            { "data": "Contact1", "name": "Contact1", "autoWidth": true },
            { "data": "Contact2", "name": "Contact2", "autoWidth": true },
            { "data": "HouseNo", "name": "HouseNo", "autoWidth": true },
            { "data": "Street", "name": "Street", "autoWidth": true },
            { "data": "ColonyOrVillageName", "name": "ColonyOrVillageName", "autoWidth": true },
            { "data": "PostalCode", "name": "PostalCode", "autoWidth": true },
            { "data": "CompleteAddress", "name": "CompleteAddress", "autoWidth": true },
            { "data": "CreditDays", "name": "CreditDays", "autoWidth": true },
            { "data": "CreditLimit", "name": "CreditLimit", "autoWidth": true },
            { "data": "Remarks", "name": "Remarks", "autoWidth": true },
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
                "data": "CustomerSupplierId", "width": "120px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var edit = '<a title="Edit ' + $("#Type2").val() + '" data-toggle="modal" data-target="#MyModal" class="AddEditRecord btn btn-primary btnOpenModal" href="javascript:void(0)" data-value="' + data + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-edit"></i></a>&nbsp';
                    var del = '<a title="Delete ' + $("#Type2").val() + '" class="DeleteRecord btn btn-danger" data-toggle="modal" data-target="#DeleteModal" href="javascript:void(0)" data-value="' + data + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-times-circle"></i></a>&nbsp';
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
    $('#CustomerSupplierName').focus();
});
$(document).on('click', '.AddEditRecord', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    if (id > 0) {
        $('#ModelHeaderSpan').html('Edit ' + $("#Type2").val() + '');
    } else {
        $('#ModelHeaderSpan').html('Add ' + $("#Type2").val() + '');
    }

    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateCustomerSupplier", "CustomerSupplier")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/CustomerSupplier/InsertUpdateCustomerSupplier/",
        data: {
            'id': id,
            'type': $("#Type").val()
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
    if (BElement.html() == '' + $("#Type2").val() +' Name') {
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
    $('#DeleteModalTitle').html("Delete " + $("#Type2").val() +"");
    $('#DeleteModalBody').html("Are you sure you want to delete this " + $("#Type2").val() +"?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary" onclick="DeleteRecord(' + id + ')">Yes</button>'
    );
});
function DeleteRecord(id) {
    $.ajax({
        url: '/Admin/CustomerSupplier/DeleteCustomerSupplier/' + id,
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