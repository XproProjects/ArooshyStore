$(function () {
    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,

        "ajax": {
            "url": "/Admin/Expense/GetAllExpenses",
            "type": "POST",
            "datatype": "json",
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (xhr.status === 410) {
                    window.location.href = customErrorMessage;
                }
            }
        },
        "columns": [
            { "data": "ExpenseName", "name": "ExpenseName", "autoWidth": true },
            {
                "data": "ExpenseTypeName", "name": "ExpenseTypeName", "class": "Acenter", "orderable": true, "autoWidth": true, 
            },
            { "data": "ExpenseAmount", "name": "ExpenseAmount", "autoWidth": true },

            {
                "data": "ExpenseDate", "name": "ExpenseDate", "class": "Acenter", "orderable": true, "autoWidth": true, 'render': function (date) {
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
                "data": "ExpenseId", "width": "130px", "class": "Acenter", "orderable": false, "render": function (data) {
                    var edit = '<a title="Edit Expense" data-toggle="modal" data-target="#MyModal" class="AddEditRecord btn btn-primary btnOpenModal" href="javascript:void(0)" data-value="' + data + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-edit"></i></a>&nbsp';
                    var del = '<a title="Delete Expense" class="DeleteRecord btn btn-danger" data-toggle="modal" data-target="#DeleteModal" href="javascript:void(0)" data-value="' + data + '" style="border-radius:20px;padding:5px;color:#fff;padding-left:15px;padding-right:15px"><i class="fas fa-fw fa-times-circle"></i></a>&nbsp';
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
    $('#ExpenseName').focus();
});
$(document).on('click', '.AddEditRecord', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");

    var id = $(this).attr("data-value");
    if (id > 0) {
        $('#ModelHeaderSpan').html('Edit Expense');
    }
    else {
        $('#ModelHeaderSpan').html('Add Expense');
    }
    $('#modalDiv').html('');
    // $('#modalDiv').load('@Url.Action("InsertUpdateExpense", "Expense")?id=' + id + '');
    $.ajax({
        type: "GET",
        url: "/Admin/Expense/InsertUpdateExpense/",
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
    if (BElement.html() == 'Expense Name') {
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
    $('#DeleteModalTitle').html("Delete Expense");
    $('#DeleteModalBody').html("Are you sure you want to delete this Expense?");
    $('#DeleteModalFooter').html(
        '<button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>' +
        '<button type="button" class="btn btn-primary" onclick="DeleteRecord(' + id + ')">Yes</button>'
    );
});
function DeleteRecord(id) {
    $.ajax({
        url: '/Admin/Expense/DeleteExpense/' + id,
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