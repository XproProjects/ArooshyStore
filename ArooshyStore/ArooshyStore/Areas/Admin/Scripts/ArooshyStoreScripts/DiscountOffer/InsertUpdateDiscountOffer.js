$('#ProfileImage').change(function (e) {
    var ext = $('#ProfileImage').val().split('.').pop().toLowerCase();
    var size = document.getElementById('ProfileImage').files[0].size;
    var filename = $('input[type=file]').val().replace(/C:\\fakepath\\/i, '');
    if ($.inArray(ext, ['jpeg', 'jpg', 'png']) == -1) {
        $("#DocumentImage").prop("src", "/Areas/Admin/Content/noimage.png");
        alert('Error! File type allowed : jpeg, jpg, png');
        $('#ProfileImage').val('');
        $('#AttachDocument').text('Choose Picture...');
    }
    else if (size > (20000 * 1024)) {
        $("#DocumentImage").prop("src", "/Areas/Admin/Content/noimage.png");
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
    if (attr.trim() == "/Areas/Admin/Content/noimage.png") {

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
    var checkstr = confirm('Are you sure you want to delete Picture?');
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
                    $("#DocumentImage").prop("src", "/Areas/Admin/Content/noimage.png");
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
    $('#MasterCategoryId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetCategoryOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                    type: "master"
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
        placeholder: "-- Select Master Category--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#ChildCategoryId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetCategoryOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                    type: "child"
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
        placeholder: "-- Select Child Category--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#ProductId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetProductsOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                    employeeId: 0,
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
        placeholder: "-- Select Product--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    if ($('#OfferId').val() > 0) {
        if ($('#HiddenSelectType').val() == 'Master Category') {
            if ($('#HiddenCategoryId').val() > 0 && $('#HiddenCategoryName').val() != null && $('#HiddenCategoryName').val() != '') {
                if ($('#MasterCategoryId').find("option[value='" + $('#HiddenCategoryId').val() + "']").length) {
                    $('#MasterCategoryId').val($('#HiddenCategoryId').val()).trigger('change');
                } else {
                    var newOption = new Option($('#HiddenCategoryName').val(), $('#HiddenCategoryId').val(), true, true);
                    $('#MasterCategoryId').append(newOption).trigger('change');
                }
            }
            else {
                $('#MasterCategoryId').val(null).trigger('change');
            }
        }
        else if ($('#HiddenSelectType').val() == 'Child Category') {
            if ($('#HiddenCategoryId').val() > 0 && $('#HiddenCategoryName').val() != null && $('#HiddenCategoryName').val() != '') {
                if ($('#ChildCategoryId').find("option[value='" + $('#HiddenCategoryId').val() + "']").length) {
                    $('#ChildCategoryId').val($('#HiddenCategoryId').val()).trigger('change');
                } else {
                    var newOption = new Option($('#HiddenCategoryName').val(), $('#HiddenCategoryId').val(), true, true);
                    $('#ChildCategoryId').append(newOption).trigger('change');
                }
            }
            else {
                $('#ChildCategoryId').val(null).trigger('change');
            }
        }
        else if ($('#HiddenSelectType').val() == 'Product') {
            if ($('#HiddenProductId').val() > 0 && $('#HiddenProductName').val() != null && $('#HiddenProductName').val() != '') {
                if ($('#ProductId').find("option[value='" + $('#HiddenProductId').val() + "']").length) {
                    $('#ProductId').val($('#HiddenProductId').val()).trigger('change');
                } else {
                    var newOption = new Option($('#HiddenProductName').val(), $('#HiddenProductId').val(), true, true);
                    $('#ProductId').append(newOption).trigger('change');
                }
            }
            else {
                $('#ProductId').val(null).trigger('change');
            }
        }
    }
    else {
        $('#MasterCategoryId').val(null).trigger('change');
        $('#ChildCategoryId').val(null).trigger('change');
        $('#ProductId').val(null).trigger('change');
    }
    var $checkoutForm = $('#popupForm').validate({
        ignore: ":not(:visible)",
        rules: {
            DiscountName: {
                required: true
            },
            DiscPercent: {
                required: true,
                min: 1,
            },
            ExpiredOn: {
                required: true
            },
            MasterCategoryId: {
                required: true
            },
            ChildCategoryId: {
                required: true
            },
            ProductId: {
                required: true
            },
        },
        messages: {
            DiscountName: {
                required: 'Offer Name is required.'
            },
            DiscPercent: {
                required: 'Disc Percent is required.',
                min: 'Disc Percent should be greater than zero.'
            },
            ExpiredOn: {
                required: 'Expiry Date is required.'
            },
            MasterCategoryId: {
                required: 'Master Category is required.'
            },
            ChildCategoryId: {
                required: 'Child Category is required.'
            },
            ProductId: {
                required: 'Product is required.'
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
$('input[type=radio][name=AssignType]').change(function () {
    var radioValue = $("input[name='AssignType']:checked").val();
    if (radioValue == 'Master Category') {
        $('#MasterCategoryDiv').removeAttr("hidden");
        $('#ChildCategoryDiv').attr("hidden", "hidden");
        $('#ProductDiv').attr("hidden", "hidden");
    }
    else if (radioValue == 'Child Category') {
        $('#MasterCategoryDiv').attr("hidden", "hidden");
        $('#ChildCategoryDiv').removeAttr("hidden");
        $('#ProductDiv').attr("hidden", "hidden");
    }
    else {
        $('#MasterCategoryDiv').attr("hidden", "hidden");
        $('#ChildCategoryDiv').attr("hidden", "hidden");
        $('#ProductDiv').removeAttr("hidden");
    }
    $('#MasterCategoryId').val(null).trigger('change');
    $('#ChildCategoryId').val(null).trigger('change');
    $('#ProductId').val(null).trigger('change');
});
$(document).on('change', 'select', function () {
    $(this).parents('td').siblings('td').find('.btn').css("margin-top", "16px");
    $(this).parents('label').siblings('em').remove();
})
$('#popupForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupForm").valid()) {
        return false;
    }
    var OfferId = $('#OfferId').val();
    var DiscountName = $('#DiscountName').val();
    var DiscPercent = $('#DiscPercent').val();
    var ExpiredOn = $('#ExpiredOn').val();
    var MasterCategoryId = $('#MasterCategoryId').val();
    var ChildCategoryId = $('#ChildCategoryId').val();
    var ProductId = $('#ProductId').val();
    var radioValue = $("input[name='AssignType']:checked").val();
    var CategoryId = radioValue == 'Master Category' ? MasterCategoryId : ChildCategoryId;
    if (CategoryId == null) {
        CategoryId = 0;
    }
    var StatusString = "No";
    if ($("#Status").is(":checked")) {
        StatusString = "Yes";
    }
    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var st =
    {
        DiscPercent: DiscPercent,
        DiscountName: DiscountName,
        ExpiredOn: ExpiredOn,
        CategoryId: CategoryId,
        SelectType: radioValue,
        OfferId: OfferId,
        ProductId: ProductId,
        StatusString: StatusString,
    }
    //var queryData = JSON.stringify(st);
    $.ajax({
        type: "POST",
        url: "/Admin/DiscountOffer/InsertUpdateDiscountOffer/",
        data: { 'user': st },
        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                var extension = $('#ProfileImage').val().split('.').pop().toLowerCase();
                if (extension != '') {
                    var typeId = data.Id;
                    var documentType = "DiscountOffer";
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
                                    toastr.success("Discount Offer Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                                    $('.close').click();
                                    oTable.ajax.reload(null, false);
                                    //location.reload();
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
                    toastr.success("Discount Offer Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
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
$("#DiscPercent").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 2,2);
});