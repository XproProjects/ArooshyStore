$('#ProfileImage').change(function (e) {
    var ext = $('#ProfileImage').val().split('.').pop().toLowerCase();
    var size = document.getElementById('ProfileImage').files[0].size;
    var filename = $('input[type=file]').val().replace(/C:\\fakepath\\/i, '');
    if ($.inArray(ext, ['jpeg', 'jpg', 'png']) == -1) {
        $("#DocumentImage").prop("src", "/Areas/Admin/Content/dummy.png");
        alert('Error! File type allowed : jpeg, jpg, png');
        $('#ProfileImage').val('');
        $('#AttachDocument').text('Choose Picture...');
    }
    else if (size > (20000 * 1024)) {
        $("#DocumentImage").prop("src", "/Areas/Admin/Content/dummy.png");
        alert('Error! Maximum File size must be 20 mb');
        $('#ProfileImage').val('');
        $('#AttachDocument').text('Choose Picture...');
    }
    else {

        //Get count of selected files
        var countFiles = $(this)[0].files.length;
        var imgPath = $(this)[0].value;
        $('#AttachDocument').text(filename);
        var extn = imgPath.substring(imgPath.lastIndexOf('.') + 1).toLowerCase();
        $('#AttachDocument').val(filename);
        if (typeof (FileReader) != "undefined") {

            //loop for each file selected for uploaded.
            for (var i = 0; i < countFiles; i++) {

                var reader = new FileReader();
                reader.onload = function (e) {
                    $("#DocumentImage").prop("height", "150");
                    $("#DocumentImage").css("width", "100%");
                    $("#DocumentImage").prop("src", e.target.result);
                }
                //image_holder.show();
                reader.readAsDataURL($(this)[0].files[i]);
            }

        }
        else {
            alert("This browser does not support FileReader.");
        }
    }
});
$(document).on("mouseenter", ".myDiv", function () {
    var attr = $('#DocumentImage').attr("src");
    if (attr.trim() == "/Areas/Admin/Content/dummy.png") {

    }
    else {
        $('.deleteImage').css("display", "block");
    }
}).on("mouseleave", ".myDiv", function () {
    $('.deleteImage').css("display", "none");
})
$(document).off('click', '.deleteImage').on('click', '.deleteImage', function () {
    var id = $('#HiddenDocumentId').val();
    DeleteImage(id);
});
function DeleteImage(id) {
    var checkstr = confirm('Are you sure you want to delete Profile Picture?');
    if (checkstr == true) {
        $.ajax({
            url: '/Admin/Documents/DeleteDocument/' + id,
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data.status == true) {
                    $('#ProfileImage').val('');
                    $('#AttachDocument').text('Choose Picture...');
                    $('#HiddenDocumentId').val(0);
                    $("#DocumentImage").prop("src", "/Areas/Admin/Content/dummy.png");
                }
                else {
                    alert(data.message);
                }
            }
        })
    }
    else {
        return false;
    }
}
$(function () {
    $('#CityId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetCitiesOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                return {
                    results: data.Results,
                    pagination: {
                        more: (params.page * 20) < data.Total
                    }
                };
            }
        },
        placeholder: "-- Select City--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#DesignationId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetDesignationsOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                return {
                    results: data.Results,
                    pagination: {
                        more: (params.page * 20) < data.Total
                    }
                };
            }
        },
        placeholder: "-- Select Designation--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    if ($('#EmployeeId').val() > 0) {
        if ($('#CityId').find("option[value='" + $('#HiddenCityId').val() + "']").length) {
            $('#CityId').val($('#HiddenCityId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenCityName').val(), $('#HiddenCityId').val(), true, true);
            // Append it to the select
            $('#CityId').append(newOption).trigger('change');
        }
    }
    else {
        $('#CityId').val(null).trigger('change');
    }
    if ($('#EmployeeId').val() > 0) {
        if ($('#DesignationId').find("option[value='" + $('#HiddenDesignationId').val() + "']").length) {
            $('#DesignationId').val($('#HiddenDesignationId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenDesignationName').val(), $('#HiddenDesignationId').val(), true, true);
            // Append it to the select
            $('#DesignationId').append(newOption).trigger('change');
        }
    }
    else {
        $('#DesignationId').val(null).trigger('change');
    }

    $("#Contact1").inputmask();
    $("#Contact2").inputmask();
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            EmployeeName: {
                required: true
            },
            Contact1: {
                required: true
            },
            //Email: {
            //    required: true,
            //    email: true,
            //},
            Password: {
                required: true
            },
            CityId: {
                required: true
            },
            CompleteAddress: {
                required: true
            },
           
        },
        messages: {
            EmployeeName: {
                required: 'Employee Name is required.'
            },
            Contact1: {
                required: 'Contact 1 is required.'
            },
            //Email: {
            //    required: 'Email is required.',
            //    email: 'Please enter a valid email address.',
            //},
            Password: {
                required: 'Password is required.'
            },
            CityId: {
                required: 'City is required.'
            },
            CompleteAddress: {
                required: 'Complete Address is required.'
            },
           
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
            if (element.val() == '' || element.val() == null) {
                element.parents('td').siblings('td').find('.btn').css("margin-top", "-8.5px");
            }
        }
    });
})
$(document).on('change', 'select', function () {
    $(this).parents('td').siblings('td').find('.btn').css("margin-top", "14px");
    $(this).parents('label').siblings('em').remove();
})
$('#popupForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupForm").valid()) {
        return false;
    }
    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var EmployeeId = $('#EmployeeId').val();
    var EmployeeName = $('#EmployeeName').val();
    var Contact1 = $('#Contact1').val();
    var Contact2 = $('#Contact2').val();
    var Email = $('#Email').val();
    var HouseNo = $('#HouseNo').val();
    var Street = $('#Street').val();
    var ColonyOrVillageName = $('#ColonyOrVillageName').val();
    var PostalCode = $('#PostalCode').val();
    var CityId = $('#CityId').val();
    if (CityId == null || CityId == undefined) {
        CityId = 0;
    }
    var DesignationId = $('#DesignationId').val();
    if (DesignationId == null || DesignationId == undefined) {
        DesignationId = 0;
    }
    var DateOfJoining = $('#DateOfJoining').val();
    var Gender = $('#Gender').val();
    var MaritalStatus = $('#MaritalStatus').val();
    var Salary = $('#Salary').val();
    var SalaryType = $('#SalaryType').val();
    var StatusString = "No";
    if ($("#Status").is(":checked")) {
        StatusString = "Yes";
    }
    var st =
    {
        EmployeeId: EmployeeId,
        EmployeeName: EmployeeName,
        Contact1: Contact1,
        Contact2: Contact2,
        Email: Email,
        HouseNo: HouseNo,
        Street: Street,
        ColonyOrVillageName: ColonyOrVillageName,
        PostalCode: PostalCode,
        CityId: CityId,
        DesignationId: DesignationId,
        Gender: Gender,
        MaritalStatus: MaritalStatus,
        Salary: Salary,
        SalaryType: SalaryType,
        StatusString: StatusString,
        DateOfJoining: DateOfJoining,
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/Employee/InsertUpdateEmployee/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                var extension = $('#ProfileImage').val().split('.').pop().toLowerCase();

                if (extension != '') {
                    var typeId = data.Id;
                    var documentType = "Employee";
                    var documentId = 0;
                    var extension = $('#ProfileImage').val().split('.').pop().toLowerCase();
                    var document = {
                        DocumentId: documentId,
                        DocumentType: documentType,
                        TypeId: typeId,
                        DocumentExtension: extension,
                        Remarks: 'ProfilePicture'
                    }
                    $.ajax({
                        type: "POST",
                        url: '/Admin/documents/AttachDocumentsForPost/',
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ 'doc': document }),
                        success: function (data) {
                            if (data.status == true) {
                                var bannerImage = $("#ProfileImage").val();
                                var file = $('#ProfileImage').get(0).files;
                                var documentFile = file[0];
                                var extension = $('#ProfileImage').val().split('.').pop().toLowerCase();
                                var documentIdWithDocumentTypeWithExtension = data.documentId + "_" + data.documentType + "_" + data.typeId + "_" + extension;
                                var formData = new FormData();
                                formData.append(file.name, documentFile);
                                formData.append("DocumentName", $("#ProfileImage").text());
                                var xhr = new XMLHttpRequest();
                                var url = '/Admin/documents/UploadDocument/' + documentIdWithDocumentTypeWithExtension;
                                xhr.open('POST', url, true);

                                xhr.onload = function (e) {
                                    var response = $.parseJSON(e.target.response);
                                    toastr.success( "Employee Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                                    $('.close').click();
                                    oTable.ajax.reload(null, false);
                                    //location.reload();
                                };
                                xhr.send(formData);
                            }
                            else {
                                alert("Error! Please try again.");
                            }
                        }
                    })
                } else {
                    toastr.success("Employee Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                    $('.close').click();
                    oTable.ajax.reload(null, false);
                    // location.reload();
                }
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })

})