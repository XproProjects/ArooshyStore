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
            "url": "/Admin/Attribute/GetAllAttributes",
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
                "data": "AttributeId", "name": "AttributeId", "width": "20px", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (data) {
                    return '<span  class="badge inbox-badge plusBtn" style="background-color: #838383;border-radius: 0px;height: 27px;width: 33px;cursor:pointer"><a href="#" title="View Attribute Details" style="" onclick="return false;" data-value="' + data + '"><i class="fal fa-angle-down txt-color-white" style="margin-left: 0px;margin-top: 1px;padding-top:3px;padding-bottom:3px;font-weight:bold;font-size:15px;color:#fff"></i></a></span>';
                }
            },
           
            { "data": "AttributeName", "name": "AttributeName", "autoWidth": true },
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
                "data": "AttributeId", "width": "130px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var div = '';
                    div += '<div class="btn-group">' +
                        '<button class="btn btn-primary dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                        '<i class="fas fa-list mr-1"></i> Actions' +
                        '</button>' +
                        '<div class="dropdown-menu">';
                    div += '<a class="dropdown-item btnManageRecord btnOpenModal" href="javascript:void(0)" data-toggle="modal" data-target="#MyModal" data-value="' + data + '"style="text-decoration:none !important;font-weight:normal !important" title="Manage Attribute Detail">Manage Attribute Detail</a>';
                    if ($('#EditActionRole').val() > 0) {
                        div += '<a class="dropdown-item AddEditRecord btnOpenModal" data-toggle="modal" data-target="#MyModal" href="javascript:void(0)" data-value="' + data + '" title="Edit Attribute" style="text-decoration:none !important;font-weight:normal !important">Edit</a>';
                    }

                    if ($('#DeleteActionRole').val() > 0) {
                        div += '<a class="dropdown-item DeleteRecord" href="javascript:void(0)" title="Delete Attribute" data-toggle="modal" data-target="#DeleteModal" data-value="' + data + '" style="text-decoration:none !important;font-weight:normal !important">Delete</a>';
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
    $('#AttributeName').focus();
});
$(document).on('click', '.AddEditRecord', function () {
    var id = $(this).attr("data-value");
    if (id > 0) {
        $('#ModelHeaderSpan').html('Edit Attribute');
    }
    else {
        $('#ModelHeaderSpan').html('Add Attribute');
    }
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateAttribute", "Attribute")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/Attribute/InsertUpdateAttribute/",
        data: {
            'id': id,
        },
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#AttributesData').html('');
            $('#modalDiv').html(response);
        }
    })
});
$(document).on('click', '.btnManageRecord', function () {
    var id = $(this).attr("data-value");
    $('#ModelHeaderSpan').html('Add Attribute Detail');
    $('#modalDiv').html('');

    $.ajax({
        type: "GET",
        url: "/Admin/Attribute/InsertUpdateAttributeDetail",
        data: { 'id': 0, 'attributeId': id },
        success: function (response) {
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
    if (BElement.html() == 'Attribute Name') {
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
    $('#DeleteModalTitle').html("Delete Attribute");
    $('#DeleteModalBody').html("Are you sure you want to delete this Attribute?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary" onclick="DeleteRecord(' + id + ')">Yes</button>'
    );
});
function DeleteRecord(id) {
    $.ajax({
        url: '/Admin/Attribute/DeleteAttribute/' + id,
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

$(document).on('click', '.plusBtn', function () {
    var i = $(this).find('a').find('i');
    var a = $(this).find('a');
    var attributeId = $(this).find('a').data("value");
    var tr = $(this).closest('tr');

    if (i.hasClass("fa-angle-down")) {
        $(i).removeClass('fa-angle-down');
        $(i).addClass('fa-spin fa-sync');
        getPropertyTypeDetailsList(attributeId, tr, i);
    } else if (i.hasClass("fa-sync")) {
        return false;
    } else {
        $(i).removeClass('fa-angle-up');
        $(i).addClass('fa-angle-down');
        $(".newDocumentsRow" + attributeId)
            .children('td, th')
            .animate({ padding: 0 })
            .wrapInner('<div />')
            .children()
            .slideUp(function () {
                $(".newDocumentsRow" + attributeId).remove();
            });
    }
    return false;
});


function getPropertyTypeDetailsList(attributeId, tr, i) {
    var totalColumns = $('#myTable thead th').length;

    var headerRow = '<tr style="border-bottom: 1px solid #D8D8D8" class="newDocumentsRow' + attributeId + '">' +
        '<th rowspan="1" colspan="1" style="width: 20.75px; vertical-align:middle; text-align: center; padding:5px; font-size:13px; background-color:#838383; color:#fff !important;" aria-sort="ascending" aria-label=": activate to sort column descending">' +
        'Sr#' +
        '</th>' +
        '<th rowspan="1"style="width: 1068.75px; vertical-align:middle; text-align: left; padding:5px; font-size:13px; background-color:#838383; color:#fff !important;" aria-label="Product Attribute Name: activate to sort column ascending">' +
        'Attriute Detail Name' +
        '</th>' +
        '<th rowspan="1" style="width: 1068.75px; vertical-align:middle; text-align: center; padding:5px; font-size:13px; background-color:#838383; color:#fff !important;" aria-label="Product Attribute Name: activate to sort column ascending">' +
        'Status' +
        '</th>' +
        '<th rowspan="1" colspan="1" style="width: 260px; vertical-align:middle; text-align: center; padding:5px; font-size:13px; background-color:#838383; color:#fff !important;" aria-label="Edit">' +
        'Action' +
        '</th>' +
        '</tr>';

    $.ajax({
        url: '/Admin/Attribute/GetAllAttributeDetail',
        type: 'GET',
        data: { 'attributeId': attributeId },
        success: function (data) {
            var rows = headerRow;

            if (data && data.data.length > 0) {
                var color = '';
                $.each(data.data, function (index, obj) {
                    color = obj.StatusString == 'Active' ? '#1DC9B7' : '#FD3995';
                    var edit = '<a title="Edit Attribute Detail" data-toggle="modal" data-target="#MyModal" class="btnManageRecordDetail btn btn-primary btnOpenModal" href="javascript:void(0)" data-value="' + obj.AttributeDetailId + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-edit"></i></a>&nbsp;';
                    var del = '<a title="Delete Attribute Detail" class="deleteAttributeDetail btn btn-danger" href="javascript:void(0)" data-toggle="modal" data-target="#DeleteModal" data-id="' + obj.AttributeDetailId + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-times-circle"></i></a>&nbsp;';

                    rows += '<tr class="newDocumentsRow' + attributeId + '">' +
                        '<td style="vertical-align: middle; text-align: center; padding: 10px; font-size: 15px; border-bottom: 1px solid #D8D8D8; width: 20.75px;">' +
                        '<span style="font-weight:bold;">' + (index + 1) + '.</span></td>' +
                        '<td style="vertical-align: middle; text-align: left; padding: 10px; font-size: 15px; border-bottom: 1px solid #D8D8D8; width: 1068.75px;">' +
                        obj.AttributeDetailName + '</td>' +
                        '<td style="vertical-align: middle;color:' + color +';font-weight:bold; text-align: center; padding: 10px; font-size: 15px; border-bottom: 1px solid #D8D8D8; width: 1068.75px;">' +
                        obj.StatusString + '</td>' +
                        '<td  style="vertical-align: middle; text-align: center; padding: 10px; font-size: 15px; border-bottom: 1px solid #D8D8D8; width: 260px;">' +
                        edit + del + '</td>' +
                        '</tr>';
                });
                $(tr).after(rows);
                $(i).removeClass('fa-spin fa-sync').addClass('fa-angle-up');
            } else {
                var noDataRow = '<tr style="border-bottom: 1px solid #D8D8D8" class="newDocumentsRow' + attributeId + '">' +
                    '<th colspan="' + totalColumns + '" style="text-align: center; padding:15px; font-size:14px; background-color:#fff; color:#838383 !important;">' +
                    'No detail found.' +
                    '</th>' +
                    '</tr>';
                $(tr).after(headerRow + noDataRow);
                $(i).removeClass('fa-spin fa-sync').addClass('fa-angle-up');
            }
        },
        error: function (xhr, status, error) {
            console.error("An error occurred: " + error);
            var noDataRow = '<tr style="border-bottom: 1px solid #D8D8D8" class="newDocumentsRow' + attributeId + '">' +
                '<th colspan="' + totalColumns + '" style="text-align: center; padding:15px; font-size:14px; background-color:#fff; color:#838383 !important;">' +
                'Error loading details.' +
                '</th>' +
                '</tr>';
            $(tr).after(headerRow + noDataRow);
            $(i).removeClass('fa-spin fa-sync').addClass('fa-angle-down');
        }
    });
}

$(document).on('click', '.deleteAttributeDetail', function () {
    var id = $(this).attr("data-id");
    $('#DeleteModalTitle').html("Delete Attribute Detail");
    $('#DeleteModalBody').html("Are you sure you want to delete this Attribute Detail?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary" onclick="DeleteAttributeDetail(' + id + ')">Yes</button>'
    );
});

function DeleteAttributeDetail(id) {
    $.ajax({
        url: '/Admin/Attribute/DeleteAttributeDetail/' + id,
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data.status == true) {
                oTable.ajax.reload(null, false);
                $('#DeleteModal').modal('toggle');
            } else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        },
        error: function (xhr, status, error) {
            console.error("An error occurred: " + error);
            toastr.error("An error occurred while deleting the item.", "Error", { timeOut: 3000, "closeButton": true });
        }
    });
}
$(document).on('click', '.btnManageRecordDetail', function () {
    var id = $(this).attr("data-value");
    $('#ModelHeaderSpan').html(id > 0 ? 'Edit Attribute Detail' : 'Add Attribute Detail');
    $('#modalDiv').html('');

    $.ajax({
        type: "GET",
        url: "/Admin/Attribute/InsertUpdateAttributeDetail/",
        data: { 'id': id },
        success: function (response) {
            $('#modalDiv').html(response);
        }
    })
});