﻿$(function () {
    $('.nav-menu li a[href="/admin/home/setting"]').parent("li").addClass('active');
    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,

        "ajax": {
            "url": "/Admin/Company/GetAllCompanys",
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
            { "data": "CompanyName", "name": "CompanyName", "autoWidth": true },
            { "data": "Email", "name": "Email", "autoWidth": true },
            { "data": "Contact1", "name": "Contact1", "autoWidth": true },          
            { "data": "Contact2", "name": "Contact2", "autoWidth": true },
            { "data": "Address", "name": "Address", "autoWidth": true }, 
            { "data": "FacebookId", "name": "FacebookId", "autoWidth": true },
            { "data": "InstagramId", "name": "InstagramId", "autoWidth": true },
            { "data": "LinkedInId", "name": "LinkedInId", "autoWidth": true },

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
                "data": "CompanyId", "width": "130px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var edit = '<a title="Edit Company" data-toggle="modal" data-target="#MyModal" class="AddEditRecord btn btn-primary btnOpenModal" href="javascript:void(0)" data-value="' + data + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-edit"></i></a>&nbsp';
                    var del = '<a title="Delete Company" class="DeleteRecord btn btn-danger" data-toggle="modal" data-target="#DeleteModal" href="javascript:void(0)" data-value="' + data + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-times-circle"></i></a>&nbsp';
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
    $('#CompanyName').focus();
});
$(document).on('click', '.AddEditRecord', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    if (id > 0) {
        $('#ModelHeaderSpan').html('Edit Company');
    }
    else {
        $('#ModelHeaderSpan').html('Add Company');
    }
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateCompany", "Company")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/Company/InsertUpdateCompany/",
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
    if (BElement.html() == 'Company Name') {
        oTable.columns(0).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Email') {
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
    $('#DeleteModalTitle').html("Delete Company");
    $('#DeleteModalBody').html("Are you sure you want to delete this Company?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary" onclick="DeleteRecord(' + id + ')">Yes</button>'
    );
});
function DeleteRecord(id) {
    $.ajax({
        url: '/Admin/Company/DeleteCompany/' + id,
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