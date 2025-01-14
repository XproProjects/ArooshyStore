$("#txtSearchModal").on("keyup", function () {
    SearchTextModal();
});
$("#btnSearchModal").on("click", function () {
    SearchTextModal();
});
$('#txtSearchModal').on('keypress', function (event) {
    if (event.keyCode == 13) {
        SearchTextModal();
    }
});
$("#btnRefreshModal").on("click", function () {
    $("#txtSearchModal").val('');
    $(".roleRows").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf('') > -1)
    });
});

function SearchTextModal() {
    var value = $("#txtSearchModal").val().toLowerCase();
    $(".DataTr").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
}
$('.selectAllBtn').click(function () {
    $('.roleRows').each(function () {
        if ($(this).is(":visible")) {
            $(this).find('input[type=checkbox]').prop('checked', true);
        }
    })
})
$('.clearAllBtn').click(function () {
    $('.roleRows').find('input[type=checkbox]').prop('checked', false);
})
$('.saveRoleBtn').click(function () {
    var data = [];
    var mainModuleId = 0;
    $('.roleRows').each(function () {
        var actionId = $(this).find('.ActionId').val();
        var moduleId = $(this).find('.ModuleId').val();
        mainModuleId = moduleId;
        var actionIdCheck = $(this).find('.ActionIdCheckbox').is(':checked');
        var alldata = {
            'ActionId': actionId,
            'ModuleId': moduleId,
        }
        if (actionIdCheck == true) {
            data.push(alldata);
        }
    });
    var allData = JSON.stringify(data);
    $.ajax({
        type: "POST",
        url: "/Admin/Action/InsertUpdateAssign/",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ 'moduleId': mainModuleId, 'data': allData }),
        success: function (data) {
            if (data.status == true) {
                $('#MyModal').animate({ scrollTop: 0 }, 'fast');
                toastr.success("Action Assigned  Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                oTable.ajax.reload(null, false);
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})