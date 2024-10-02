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
    if ($('#CustomerSupplierId').val() > 0) {
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
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            CustomerSupplierName: {
                required: true
            },
            Contact1: {
                required: true
            },
            Email: {
                required: true,
                email: true,
            },
            CityId: {
                required: true
            },
            CompleteAddress: {
                required: true
            },
        },
        messages: {
            CustomerSupplierName: {
                required: 'Customer Name is required.'
            },
            Contact1: {
                required: 'Contact 1 is required.'
            },
            Email: {
                required: 'Email is required.',
                email: 'Please enter a valid email address.',
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

    $('#popupForm').on('submit', function (e) {
        e.preventDefault();
        if (!$checkoutForm.valid()) {
            return false;
        }

        $('#btn_Save').attr('disabled', 'disabled').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

       
        var CustomerSupplierId = $('#CustomerSupplierId').val();
        var CustomerSupplierName = $('#CustomerSupplierName').val();
        var CustomerSupplierType = "Customer";
        var Contact1 = $('#Contact1').val();
        var Contact2 = $('#Contact2').val();
        var Email = $('#Email').val();
        var Password = $('#Password').val();
        var isChangePassword = $('#IsChangePassword').val();
        var HouseNo = $('#HouseNo').val();
        var Street = $('#Street').val();
        var ColonyOrVillageName = $('#ColonyOrVillageName').val();
        var PostalCode = $('#PostalCode').val();
        var CityId = $('#CityId').val();
        if (CityId == null || CityId == undefined) {
            CityId = 0;
        }
        var CompleteAddress = $('#CompleteAddress').val();
        var CreditDays = $('#CreditDays').val();
        var CreditLimit = $('#CreditLimit').val();
        var Remarks = $('#Remarks').val();
        var StatusString = "No";
        if ($("#Status").is(":checked")) {
            StatusString = "Yes";
        }
        var st =
        {
            CustomerSupplierId: CustomerSupplierId,
            CustomerSupplierName: CustomerSupplierName,
            CustomerSupplierType: CustomerSupplierType,
            Contact1: Contact1,
            Contact2: Contact2,
            Email: Email,
            Password: Password,
            IsChangePassword: isChangePassword,
            HouseNo: HouseNo,
            Street: Street,
            ColonyOrVillageName: ColonyOrVillageName,
            PostalCode: PostalCode,
            CityId: CityId,
            CreditDays: CreditDays,
            CreditLimit: CreditLimit,
            Remarks: Remarks,
            CompleteAddress: CompleteAddress,
            StatusString: StatusString,
        }
        $.ajax({
            type: "POST",
            url: "/User/CustomerAccount/Register/",
            data: { 'user': st },
            dataType: 'json',
            success: function (data) {
                $('#btn_Save').html("Save");
                $('#btn_Save').prop('disabled', false);
                if (data.status) {
                    var extension = $('#ProfileImage').val().split('.').pop().toLowerCase();

                    if (extension != '') {
                        var typeId = data.Id;
                        var documentType = $("Customer").val();
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
                                        toastr.success("Customer Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
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
                    }
                    else {
                        toastr.success("Customer Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                        $('.close').click();
                        oTable.ajax.reload(null, false);
                        // location.reload();
                    }
                }
                else {
                    toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
                }
            }
        });
    });
});
