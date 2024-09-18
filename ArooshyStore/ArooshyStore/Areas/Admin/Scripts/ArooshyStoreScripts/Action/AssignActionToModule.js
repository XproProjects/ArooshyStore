$(function () {

    $('#myTable').dataTable({

        "autoWidth": true,
        "bLengthChange": false,
        //"searching": false,
        "processing": true,
        "serverSide": true,
        "responsive": true,

        "ajax": {
            "url": "/Admin/Module/GetAllModules",
            "type": "POST",
            "datatype": "json",
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (xhr.status === 410) {
                    window.location.href = customErrorMessage;
                }
            }
        },
        "columns": [
            { "data": "ModuleName", "name": "ModuleName", "autoWidth": true },
            //{ "data": "FullName", "name": "FullName", "autoWidth": true },

            {
                "data": "ModuleId", "width": "100px", "class": "Acenter", "orderable": false, "render": function (data) {
                    return '<a title="Assign" href="javascript:void(0)"  class="Assign btn btn-primary btnSaveForm btnOpenModal" data-toggle="modal" data-target="#MyModal" data-value="' + data + '" ><i class="fas fa-list"></i>&nbsp;Assign</a>';
                }
            }
        ]
    });
    oTable = $('#myTable').DataTable();

});
$(document).on('keydown', function (event) {
    if (event.altKey && event.keyCode === 78) {
        $('#AddEditRecord').click();
    }
})
$('#btnSearch').click(function () {
    SearchItem();
});
$('#txtSearch').on('keypress', function (event) {
    if (event.keyCode == 13) {
        SearchItem();
    }
});
function SearchItem() {
    var BElement = $("#btnSearchCategory");
    if (BElement.html() == 'Module Name') {
        oTable.columns(0).search($('#txtSearch').val().trim()).draw();
    }
    else {
        alert("Error! try again.");
    }
}
$(document).on('change', '.SelectAll', function () {
    var check = $(this).is(':checked');
    if (check == true) {
        $('#myTable > tbody').find('input[type=checkbox]').prop('checked', true);
    }
    else {
        $('#myTable > tbody').find('input[type=checkbox]').prop('checked', false);
    }
})

$(document).on('click', '.Assign', function () {
    $("#MyModal").find('.modal-dialog').removeClass("modal-lg").addClass("modal-lg");
    var id = $(this).attr("data-value")
    $('#ModelHeaderSpan').html('Assign Action To Module');
    $.ajax({
        type: "GET",
        url: "/Admin/Action/insertupdateassign/",
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