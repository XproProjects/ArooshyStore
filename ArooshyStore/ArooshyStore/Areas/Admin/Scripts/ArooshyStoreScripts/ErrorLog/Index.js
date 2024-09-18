$(document).on('click', '.btnMainOrder', function () {
    //Remove active class from all tab buttons
    $('.tabMainLi').removeClass("active");
    //Make this clicked button active
    $(this).parent("li").addClass("active");
    $(this).css("color", "#6B9635");
    $("html, body").animate({ scrollTop: 0 }, "slow");
    $('#btnFind').click();
});

$(function () {
    // Initialize Select2 dropdowns with AJAX
    $('#ErrorSourceIdList').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetErrorSourceOptionList/',
            dataType: 'json',
            data: function (params) {
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page || 1
                };
            },
            processResults: function (data, params) {
                return {
                    results: data.Results,
                    pagination: {
                        more: (params.page * 20) < data.Total
                    }
                };
            }
        },
        minimumInputLength: 0,
        dropdownParent: $(".mySelectList"),
        allowClear: true,
       // multiple: true
    });

    $('#ErrorClassIdList').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetErrorClassOptionList/',
            dataType: 'json',
            data: function (params) {
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page || 1
                };
            },
            processResults: function (data, params) {
                return {
                    results: data.Results,
                    pagination: {
                        more: (params.page * 20) < data.Total
                    }
                };
            }
        },
        minimumInputLength: 0,
        dropdownParent: $(".mySelectList"),
        allowClear: true,
        //multiple: true
    });

    $('#ErrorActionIdList').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetErrorActionOptionList/',
            dataType: 'json',
            data: function (params) {
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page || 1
                };
            },
            processResults: function (data, params) {
                return {
                    results: data.Results,
                    pagination: {
                        more: (params.page * 20) < data.Total
                    }
                };
            }
        },
        minimumInputLength: 0,
        dropdownParent: $(".mySelectList"),
        allowClear: true,
        //multiple: true
    });

    $('#ErrorLineNumberIdList').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetErrorLineNumberOptionList/',
            dataType: 'json',
            data: function (params) {
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page || 1
                };
            },
            processResults: function (data, params) {
                return {
                    results: data.Results,
                    pagination: {
                        more: (params.page * 20) < data.Total
                    }
                };
            }
        },
        minimumInputLength: 0,
        dropdownParent: $(".mySelectList"),
        allowClear: true,
        //multiple: true
    });

    // Initial no search to load all data
    NoSearch();
});

function getErrorLogs(SearchString, errorClass) {
        $('#myTable').dataTable({
            "autoWidth": false,
            "bLengthChange": false,
            //"searching": false,
            "processing": true,
            "serverSide": true,
            "responsive": true,
            "bDestroy": true,
            "order": [[0, "desc"]],
        "ajax": {
            "url": "/Admin/ErrorLog/GetAllErrorLogs",
            "type": "POST",
            "datatype": "json",
            "data": function (d) {
                d.SearchString = SearchString;
                d.ErrorClass = errorClass; // Pass the ErrorClass parameter
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                if (xhr.status === 410) {
                    window.location.href = customErrorMessage;
                } else {
                    console.error('AJAX error:', xhr.statusText);
                }
            }
        },
        "columns": [
            { "data": "ErrorClass", "name": "ErrorClass", "autoWidth": true },
            { "data": "ErrorAction", "name": "ErrorAction", "autoWidth": true },
            { "data": "ErrorSource", "name": "ErrorSource", "autoWidth": true },
            { "data": "ErrorLineNumber", "name": "ErrorLineNumber", "autoWidth": true },
            { "data": "ErrorDescription", "name": "ErrorDescription", "autoWidth": true },
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
            { "data": "UpdatedByString", "name": "UpdatedBy", "autoWidth": true }
        ]
    });

    $('#btnFind').html("<i class='fas fa-search'></i>").prop("disabled", false);
    $('#btnReset').html("<i class='fas fa-sync'></i>").prop("disabled", false);
}

$('#btnFind').click(function () {
    var errorClassCheckbox = $('#ErrorClassCheckbox').is(":checked");

    var errorClass = $('#ErrorClassIdList').val();
   
    var errorActionCheckbox = $('#ErrorActionCheckbox').is(":checked");

    var errorAction = $('#ErrorActionIdList').val();

    var errorSourceCheckbox = $('#ErrorSourceCheckbox').is(":checked");

    var errorSource = $('#ErrorSourceIdList').val();
    var errorLineNumberCheckbox = $('#ErrorLineNumberCheckbox').is(":checked");

    var errorLineNumber = $('#ErrorLineNumberIdList').val();
    if (errorClassCheckbox == true) {
        if (errorClass == '' || errorClass == null || errorClass == undefined) {
            toastr.error("Error Class is required.", "Error", { timeOut: 3000, "closeButton": true });
            return false;
        }
    }
    if (errorActionCheckbox == true) {
        if (errorAction == '' || errorAction == null || errorAction == undefined) {
            toastr.error("Error Action is required.", "Error", { timeOut: 3000, "closeButton": true });
            return false;
        }
    }
    if (errorSourceCheckbox == true) {
        if (errorSource == '' || errorSource == null || errorSource == undefined) {
            toastr.error("Error Source is required.", "Error", { timeOut: 3000, "closeButton": true });
            return false;
        }
    }
    if (errorLineNumberCheckbox == true) {
        if (errorLineNumber == '' || errorLineNumber == null || errorLineNumber == undefined) {
            toastr.error("Error Line Number is required.", "Error", { timeOut: 3000, "closeButton": true });
            return false;
        }
    }

    $('#btnFind').html("<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span>").prop("disabled", true);
    $('#btnReset').prop("disabled", true);

    $.ajax({
        type: "POST",
        url: "/Admin/ErrorLog/SetSearchString",
        dataType: 'json',
        data: {
            'classCheckbox': errorClassCheckbox, 'errorClass': errorClass,
            'errorSourceCheckbox': errorSourceCheckbox, 'errorSourceId': errorSource,
            'errorLineNumbersCheckbox': errorLineNumberCheckbox, 'errorLineNumberId': errorLineNumber,
            'errorActionCheckbox': errorActionCheckbox, 'errorActionId': errorAction
        },
        success: function (response) {
            getErrorLogs(response.searchString, errorClass); 
        },
        error: function (xhr, status, error) {
            console.error('AJAX error:', error);
        }
    });
});

function NoSearch() {
    $.ajax({
        type: "POST",
        url: "/Admin/ErrorLog/SetSearchString",
        dataType: 'json',
        data: {
            'classCheckbox': false, 'errorClass': null,
            'errorSourceCheckbox': false, 'errorSourceId': null,
            'errorLineNumbersCheckbox': false, 'errorLineNumberId': null,
            'errorActionCheckbox': false, 'errorActionId': null
          
        },
        success: function (response) {
            getErrorLogs(response.searchString, null); 
        },
        error: function (xhr, status, error) {
            console.error('AJAX error:', error);
        }
    });
}

$('#btnReset').click(function () {
    $('#ErrorLineNumberCheckbox').prop("checked", false);
    $('#ErrorLineNumberIdList').prop("disabled", true).val(null).trigger('change');

    $('#ErrorSourceCheckbox').prop("checked", false);
    $('#ErrorSourceIdList').prop("disabled", true).val(null).trigger('change');

    $('#ErrorClassCheckbox').prop("checked", false);
    $('#ErrorClassIdList').prop("disabled", true).val(null).trigger('change');

    $('#ErrorActionCheckbox').prop("checked", false);
    $('#ErrorActionIdList').prop("disabled", true).val(null).trigger('change');

    NoSearch();
});

$('#ErrorLineNumberCheckbox').change(function () {
    $('#ErrorLineNumberIdList').prop("disabled", !$(this).is(":checked")).val($(this).is(":checked") ? null : '').trigger('change');
});

$('#ErrorSourceCheckbox').change(function () {
    $('#ErrorSourceIdList').prop("disabled", !$(this).is(":checked")).val($(this).is(":checked") ? null : '').trigger('change');
});

$('#ErrorClassCheckbox').change(function () {
    $('#ErrorClassIdList').prop("disabled", !$(this).is(":checked")).val($(this).is(":checked") ? null : '').trigger('change');
});

$('#ErrorActionCheckbox').change(function () {
    $('#ErrorActionIdList').prop("disabled", !$(this).is(":checked")).val($(this).is(":checked") ? null : '').trigger('change');
});

$(document).on('keydown', function (event) {
    if (event.altKey && event.keyCode === 78) { // Alt+N
        $('#btnFind').click();
    }
    if (event.altKey && event.keyCode === 82) { // Alt+R
        $('#btnReset').click();
    }
});
