﻿$(function () {
    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,

        "ajax": {
            "url": "/Admin/User/GetAllUsers",
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
            "orderable": false,
            "targets": 1
        }],
        "columns": [
            {
                "data": "ImagePath", "width": "45px", "class": "Acenter", "orderable": false, "render": function (data) {
                    return '<img src="' + data + '" style="height:45px;width:45px;" />';
                }
            },
            { "data": "FullName", "name": "FullName", "orderable": true, "autoWidth": true },
            { "data": "TypeName", "name": "TypeName", "autoWidth": true },
            { "data": "Email", "name": "Email", "autoWidth": true },
            { "data": "Contact1", "name": "Contact1", "autoWidth": true },
            { "data": "Contact2", "name": "Contact2", "autoWidth": true },
            { "data": "Cnic", "name": "Cnic", "autoWidth": true },
            { "data": "Gender", "name": "Gender", "autoWidth": true },
            {
                "data": "DOB", "name": "DOB", "orderable": true, "autoWidth": true, 'render': function (date) {
                    return getDateForDatatable(date);
                }
            },
            { "data": "Address1", "name": "Address1", "autoWidth": true },
            { "data": "Address2", "name": "Address2", "autoWidth": true },
            {
                "data": "StatusString", "name": "StatusString", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (data) {
                    if (data.toString() == "Active") {
                        return '<span class="badge badge-success badge-pill">Active</span>';
                    }
                    else {
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
                "data": "UserIdWithTypeName", "width": "130px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var userId = data.split('|')[0];
                    var typeName = data.split('|')[1];
                    var div = '';
                    div += '<div class="btn-group">' +
                        '<button class="btn btn-primary dropdown-toggle waves-effect waves-themed" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                        '<i class="fas fa-list mr-1"></i> Actions' +
                        '</button>' +
                        '<div class="dropdown-menu">';
                    if ($('#AssignModuleRole').val() > 0) {
                        if (typeName != 'Super Admin') {
                            div += '<a class="dropdown-item btnOpenModal btnAssignModule" href="javascript:void(0)" data-value="' + userId + '" data-toggle="modal" data-target="#MyModal" title="Assign Permissions to this User">User Permissions</a>';
                        }
                    }
                    if ($('#EditActionRole').val() > 0) {
                        div += '<a class="dropdown-item AddEditRecord btnOpenModal" data-toggle="modal" data-target="#MyModal" href="javascript:void(0)" data-value="' + userId + '" title="Edit User"  style="text-decoration:none !important;font-weight:normal !important">Edit</a>';
                    }
                    if ($('#DeleteActionRole').val() > 0) {
                        if (typeName != 'Super Admin') {
                            div += '<a class="dropdown-item DeleteRecord" href="javascript:void(0)" title="Delete User" data-toggle="modal" data-target="#DeleteModal"  data-value="' + userId + '" style="text-decoration:none !important;font-weight:normal !important">Delete</a>';
                        }
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
    $('#UserName').focus();
});
$(document).on('click', '.btnAssignModule', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    $('#ModelHeaderSpan').html('Assign Module');
    $('#modalDiv').html('');
    $.ajax({
        type: "GET",
        url: "/Admin/User/AssignModule/",
        data: {
            'userId': id,
        },
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $('#modalDiv').html(response);
        }
    })
});
$(document).on('click', '.AddEditRecord', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value");
    if (id > 0) {
        $('#ModelHeaderSpan').html('Edit User');
    }
    else {
        $('#ModelHeaderSpan').html('Add User');
    }
    $('#modalDiv').html('');
    $.ajax({
        type: "GET",
        url: "/Admin/User/InsertUpdateUser/",
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
    if (BElement.html() == 'Full Name') {
        oTable.columns(0).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Email') {
        oTable.columns(1).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'Type') {
        oTable.columns(2).search($('#txtSearch').val().trim()).draw();
    }
    else if (BElement.html() == 'CNIC') {
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
    $('#DeleteModalTitle').html("Delete User");
    $('#DeleteModalBody').html("Are you sure you want to delete this User?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary" onclick="DeleteRecord(' + id + ')">Yes</button>'
    );
});
function DeleteRecord(id) {
    $.ajax({
        url: '/Admin/User/DeleteUser/' + id,
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