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
    $('#UserTypeId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetUserTypesOptionList/',
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
        placeholder: "-- Select User Type--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#Gender').select2({
        dropdownParent: $(".mySelect"),
        placeholder: "-- Select Gender--",
    });
   
    if ($('#UserId').val() > 0) {
        $('#Gender').val($('#HiddenGender').val()).trigger('change');

        if ($('#UserTypeId').find("option[value='" + $('#HiddenUserTypeId').val() + "']").length) {
            $('#UserTypeId').val($('#HiddenUserTypeId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenTypeName').val(), $('#HiddenUserTypeId').val(), true, true);
            // Append it to the select
            $('#UserTypeId').append(newOption).trigger('change');
        }
    }
    else {
        $('#Gender').val(null).trigger('change');
        $('#UserTypeId').val(null).trigger('change');
        $('#Email').val("");
        $('#Password').val("");
    }

    var $checkoutForm = $('#popupForm').validate({
        rules: {
            FullName: {
                required: true
            },
            UserTypeId: {
                required: true
            },
            Password: {
                required: true
            },
            Email: {
                required: true,
                email: true,
                //required: true
            },
            //Password: {
            //    required: true
            //},
            //AddressId: {
            //    required: true
            //},
        },
        messages: {
            FullName: {
                required: 'Full Name is required.'
            },
            UserTypeId: {
                required: 'User Type is required.'
            },
            Password: {
                required: 'Password is required.'
            },
            Email: {
                required: 'Email is required.',
                email: 'Please enter a valid email address.',
                //required: 'Email is required.'
            },
            //Password: {
            //    required: 'Password is required.'
            //},
            //AddressId: {
            //    required: 'Current Address is required.'
            //},
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
    var UserId = $('#UserId').val();
    var FullName = $('#FullName').val();
    var UserTypeId = $('#UserTypeId').val();
    var Gender = $('#Gender').val();
    var Cnic = $('#Cnic').val();
    var Contact1 = $('#Contact1').val();
    var Contact2 = $('#Contact2').val();
    var DOB = $('#DOB').val();
    var Email = $('#Email').val();
    var Password = $('#Password').val();
    var Address1 = $('#Address1').val();
    var Address2 = $('#Address2').val();
    var isChangePassword = $('#IsChangePassword').val();
    var InfoId = $('#InfoId').val();
    var Password = $('#Password').val();
    var StatusString = 'No';
    if ($('#IsActive').is(":checked")) {
        StatusString = "Yes";
    }
    var st =
    {
        UserId: UserId,
        FullName: FullName,
        UserTypeId: UserTypeId,
        Gender: Gender,
        Cnic: Cnic,
        Contact1: Contact1,
        Contact2: Contact2,
        DOB: DOB,
        Email: Email,
        Password: Password,
        Address1: Address1,
        Address2: Address2,
        StatusString: StatusString,
        IsChangePassword: isChangePassword,
        InfoId: InfoId,
        Password: Password
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/User/InsertUpdateUser/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                var extension = $('#ProfileImage').val().split('.').pop().toLowerCase();
                if (extension != '') {
                    var typeId = data.Id;
                    var documentType = "User";
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
                                    toastr.success("User Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                                    $('.close').click();
                                    oTable.ajax.reload(null, false);
                                };
                                xhr.send(formData);  // multipart/form-data
                            }
                            else {
                                alert("Error! Please try again.");
                            }
                        }
                    })
                }
                else {
                    toastr.success("User Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                    $('.close').click();
                    oTable.ajax.reload(null, false);
                }
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})
$("#ZipCode").on('input keypress', function (event) {
    NumberPostive(event, this, 10);
});
$(document).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();
    }
});
$(document).on('click', '.btnChangeUserPassword', function () {
    $(this).css('color', '#3276B1');
    $('#IsChangePassword').val(1);
    $('#Password').val('');
    $('#Password').prop("disabled", false);
    $('#eyeIcon').css("display", "block");

});
$(document).on('click', '.btnCancelUserPassword', function () {
    $(this).css('color', '#3276B1');
    $('#IsChangePassword').val(0);
    $('#Password').val('*************************');
    $('#Password').prop("disabled", true);
    $('#eyeIcon').css("display", "none");
});
$('#eyeIcon').on('click', function () {
    if ($(this).hasClass('fa-eye-slash')) {
        $(this).removeClass('fa-eye-slash');
    }
    else {
        $(this).addClass('fa-eye-slash');
    }
    var input = $("#Password");
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});