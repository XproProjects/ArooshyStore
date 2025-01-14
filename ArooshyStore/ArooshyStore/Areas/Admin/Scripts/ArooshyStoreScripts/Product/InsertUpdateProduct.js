$(document).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();
    }
});
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
    var checkstr = confirm('Are you sure you want to delete this Picture?');
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
function LoadAttributesDiv() {
    $(".AllModules").html('<center>' +
        '<div class="demo">' +
        '<div class="spinner-grow text-primary" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-secondary" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-success" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-danger" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-warning" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-info" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-light" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '</div>' +
        '</center>');
    $.ajax({
        type: "GET",
        url: "/Admin/Product/AttributesList/?id=" + $("#ProductId").val(),
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $(".AllModules").html(response);
        }
    })
}
$(function () {
    LoadAttributesDiv();
    $('#TagId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetProductTagsList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page
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
        //placeholder: "-- Select City--",
        minimumInputLength: 0,
        allowClear: true,
        dropdownParent: $(".mySelect"),
        multiple: true,
    });
    $('#CategoryId').select2({
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
        placeholder: "-- Select Category--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#DeliveryInfoId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetDeliveryInfoList/',
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
        placeholder: "-- Select Delivery Info--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
   
    if ($('#ProductId').val() > 0) {

        if ($('#CategoryId').find("option[value='" + $('#HiddenCategoryId').val() + "']").length) {
            $('#CategoryId').val($('#HiddenCategoryId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenCategoryName').val(), $('#HiddenCategoryId').val(), true, true);
            // Append it to the select
            $('#CategoryId').append(newOption).trigger('change');
        }

        if ($('#HiddenDeliveryInfoId').val() > 0) {
            if ($('#DeliveryInfoId').find("option[value='" + $('#HiddenDeliveryInfoId').val() + "']").length) {
                $('#DeliveryInfoId').val($('#HiddenDeliveryInfoId').val()).trigger('change');
            } else {
                // Create a DOM Option and pre-select by default
                var newOption = new Option($('#HiddenDeliveryInfoName').val(), $('#HiddenDeliveryInfoId').val(), true, true);
                // Append it to the select
                $('#DeliveryInfoId').append(newOption).trigger('change');
            }
        }
        

        //if ($('#TagId').find("option[value='" + $('#HiddenTagId').val() + "']").length) {
        //    $('#TagId').val($('#HiddenTagId').val()).trigger('change');
        //} else {
        //    // Create a DOM Option and pre-select by default
        //    var newOption = new Option($('#HiddenTagName').val(), $('#HiddenTagId').val(), true, true);
        //    // Append it to the select
        //    $('#TagId').append(newOption).trigger('change');
        //}
    }
    else {
        $('#CategoryId').val(null).trigger('change');
        $('#DeliveryInfoId').val(null).trigger('change');
        $('#TagId').val(null).trigger('change');
    }
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            ProductName: {
                required: true
            },
            CategoryId: {
                required: true
            },
            ArticleNumber: {
                required: true
            },
            Barcode: {
                required: true
            },
            SalePrice: {
                required: true,
                min: 1
            },
            SalePriceForWebsite: {
                required: true,
                min: 1
            },
            SalePriceAfterExpired: {
                required: "#IsExpired:checked",
                min: 1
            },

        },
        messages: {
            ProductName: {
                required: 'Product Name is required.'
            },
            CategoryId: {
                required: 'Category is required.'
            },
            ArticleNumber: {
                required: 'Article Number is required.'
            },
            Barcode: {
                required: 'Barcode is required.'
            },
            SalePrice: {
                required: 'Sale Price (Admin) is required.',
                min: 'Sale Price (Admin) should be greater than zero.'
            },
            SalePriceForWebsite: {
                required: 'Sale Price (Website) is required.',
                min: 'Sale Price (Website) should be greater than zero.'
            },
            SalePriceAfterExpired: {
                required: 'Sale Price (Exired) is required.',
                min: 'Sale Price (Expired) should be greater than zero.'
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
function getTagsData() {
    var arrayData = [];
    $('#TagId  > option:selected').each(function () {
        var id = $(this).val();
        var alldata = {
            'TagId': id
        }
        arrayData.push(alldata);
    });
    return arrayData;
}
function getSectionsData() {
    var tabsData = [];
    $('.ModuleList').each(function () {
        var AttributeId = $(this).find('.AttributeId').val();
        var tabCheckbox = $(this).find('.moduleCheckbox').is(':checked');

        var sectionsData = new Array();
        var check = 0;
        var checkSectionList = 0;
        //Loop on all actions of module
        $(this).find('.ActionList').each(function () {
            //this will be incremented if module has any action. Otherwise it will remain zero.
            checkSectionList++;
            ///
            var ProductAttributeDetailId = $(this).find('.ProductAttributeDetailId').val();
            var AttributeDetailId = $(this).find('.AttributeDetailId').val();
            var sectionCheckbox = $(this).find('.actionCheckbox').is(':checked');
            var isChecked = '';
            if (sectionCheckbox == true) {
                isChecked = 'yes';
            }
            else {
                isChecked = 'no';
            }
            //if action checkbox is checked then add it into array
            /* if (sectionCheckbox == true) {*/
            check++;
            var action =
            {
                ProductAttributeDetailId: ProductAttributeDetailId,
                AttributeDetailId: AttributeDetailId,
                IsChecked: isChecked,
            }
            sectionsData.push(action);
            /*}*/
        });
        //This condition will check if module has any action or not.
        if (checkSectionList > 0) {
            //If any action of module is checked
            /*if (check > 0) {*/
            var alldata = {
                AttributeId: AttributeId,
                AttributeDetails: sectionsData
            }
            tabsData.push(alldata);
            /* }*/
        }
        else {
            if (tabCheckbox == true) {
                var emptyArray = new Array();
                var action =
                {
                    AttriProductAttributeDetailIdbuteDetailId: 0,
                    AttributeDetailId: 0,
                    IsChecked: "no",
                }
                emptyArray.push(action);
                var alldata = {
                    AttributeId: AttributeId,
                    AttributeDetails: emptyArray
                }
                tabsData.push(alldata);
            }
        }
    });
    return tabsData;
}
function CheckSectionsData() {
    var check = "";
    var checkModule = 0;
    var checkedActionsInAllModules = 0;
    $('.ModuleList').each(function () {
        debugger;
        checkModule++;
        var count = 0;
        //Loop on all actions of module
        $(this).find('.ActionList').each(function () {
            debugger;
            var sectionCheckbox = $(this).find('.actionCheckbox').is(':checked');
            if (sectionCheckbox == true) {
                check++;
                count++;
                if (count == 1) {
                    checkedActionsInAllModules++;
                }
            }
        });
    });
    debugger;
    if (parseInt(checkModule) == parseInt(checkedActionsInAllModules)) {
        check = "Pass";
    }
    else {
        check = "Fail";
    }
    return check;
}
$("#IsExpired").change(function (e) {
    $("#ExpiredTextboxDiv").find("label.error").remove();
    $("#SalePriceAfterExpired").removeClass("error");
    if ($(this).is(":checked")) {
        $("#SalePriceAfterExpired").prop("disabled", false);
    }
    else {
        $("#SalePriceAfterExpired").val(0);
        $("#SalePriceAfterExpired").prop("disabled", true);
    }
});
$('#popupForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupForm").valid()) {
        return false;
    }

    if (CheckSectionsData() == "Fail") {
        toastr.error("Please select atleast one Attribute Detail in each Attribute.", "Error", { timeOut: 3000, "closeButton": true });
        return false;
    }
    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var ProductId = $('#ProductId').val();
    var ProductName = $('#ProductName').val();
    var ProductDescription = $('#ProductDescription').val();
    var ProductNameUrdu = '';
    var DeliveryInfoId = $('#DeliveryInfoId').val();
    var UnitId = 0;
    var CategoryId = $('#CategoryId').val();
    var ArticleNumber = $('#ArticleNumber').val();
    var Barcode = $('#Barcode').val();
    var CostPrice = 0;
    var SalePrice = $('#SalePrice').val();
    var SalePriceAfterExpired = $('#SalePriceAfterExpired').val();
    var SalePriceForWebsite = $('#SalePriceForWebsite').val();
    if (CategoryId == null || CategoryId == undefined) {
        CategoryId = 0;
    }
    var StatusString = "No";
    if ($("#Status").is(":checked")) {
        StatusString = "Yes";
    }
    var IsFeaturedString = "No";
    if ($("#Featured").is(":checked")) {
        IsFeaturedString = "Yes";
    }
    var IsExpiredString = "No";
    if ($("#IsExpired").is(":checked")) {
        IsExpiredString = "Yes";
    }

    var ShowOnWebsiteString = "No";
    if ($("#ShowOnWebsite").is(":checked")) {
        ShowOnWebsiteString = "Yes";
    }

    var detail = JSON.stringify(getSectionsData());
    var tagsData = JSON.stringify(getTagsData());

    var st =
    {
        ProductId: ProductId,
        ProductName: ProductName,
        ProductNameUrdu: ProductNameUrdu,
        ProductDescription: ProductDescription,
        DeliveryInfoId: DeliveryInfoId,
        SalePrice: SalePrice,
        CostPrice: CostPrice,
        SalePriceForWebsite: SalePriceForWebsite,
        SalePriceAfterExpired: SalePriceAfterExpired,
        UnitId: UnitId,
        CategoryId: CategoryId,
        ArticleNumber: ArticleNumber,
        Barcode: Barcode,
        StatusString: StatusString,
        IsFeaturedString: IsFeaturedString,
        IsExpiredString: IsExpiredString,
        ShowOnWebsiteString: ShowOnWebsiteString
    }
   
    $.ajax({
        type: "POST",
        url: "/Admin/Product/InsertUpdateProduct/",
        data: JSON.stringify({ 'user': st, 'data': detail, 'tags': tagsData }),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                var extension = $('#ProfileImage').val().split('.').pop().toLowerCase();
                if (extension != '') {
                    var typeId = data.Id;
                    var documentType = "Product";
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
                                    toastr.success("Product Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
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
                    toastr.success("Product Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
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
$("#SalePrice").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});
$("#SalePriceForWebsite").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});
$("#SalePriceAfterExpired").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});