$(document).ready(function () {
    $('.tree > ul').attr('role', 'tree').find('ul').attr('role', 'group');
    $('.tree').find('li:has(ul)').addClass('parent_li').attr('role', 'treeitem').find(' > span').attr('title', 'Collapse this branch').on('click', function (e) {
        var children = $(this).parent('li.parent_li').find(' > ul > li');
        if (children.is(':visible')) {
            children.hide('fast');
            $(this).attr('title', 'Expand this Attribute').find(' > i').removeClass().addClass('fa fa-lg fa-plus-circle');
        } else {
            children.show('fast');
            $(this).attr('title', 'Collapse this Attribute').find(' > i').removeClass().addClass('fa fa-lg fa-minus-circle');
        }
        e.stopPropagation();
    });
});
$(document).on('change', '.moduleCheckbox', function () {
    var check = $(this).is(':checked');
    if (check) {
        $(this).parents('.AttributesList').find('input[type=checkbox]').prop('checked', true);
    } else {
        $(this).parents('.AttributesList').find('input[type=checkbox]').prop('checked', false);
    }
});

$(document).on('change', '.actionCheckbox', function () {
    var check = $(this).is(':checked');
    if (check) {
        $(this).parents('.AttributesDetailList').find('input[type=checkbox]').prop('checked', true);
    } else {
        $(this).parents('.AttributesDetailList').find('input[type=checkbox]').prop('checked', false);
    }

    var unchecked = 0;
    $(this).parents('.AttributesList').find('.actionCheckbox').each(function () {
        if (!$(this).is(':checked')) {
            unchecked++;
        }
    });

    if (unchecked > 0) {
        $(this).parents('.AttributesList').find('.moduleCheckbox').prop('checked', false);
    } else {
        $(this).parents('.AttributesList').find('.moduleCheckbox').prop('checked', true);
    }
});

$('.selectAllBtn').click(function () {
    $('.AllModules').find('input[type=checkbox]').prop('checked', true);
});

$('.clearAllBtn').click(function () {
    $('.AllModules').find('input[type=checkbox]').prop('checked', false);
});

//$('#btn_Save').click(function () {
//    var AttributeDetailData = [];
//    var mainModuleId = 0;

//    $('.AttributesList').each(function () {
//        var attributeDetailId = $(this).find('.AttributeId').val();
//        var attributeId = $(this).find('.AttributeDetailId').val();
//        mainModuleId = attributeId;
//        var attributeDetailIdCheck = $(this).find('.moduleCheckbox').is(':checked');

//        if (attributeDetailIdCheck) {
//            var alldata = {
//                'AttributeId': attributeDetailId,
//                'AttributeDetailId': attributeId,
//            };
//            AttributeDetailData.push(alldata);
//        }
//    });

//    var allData2 = JSON.stringify(AttributeDetailData);

//    $.ajax({
//        type: "POST",
//        url: "/Admin/Product/InsertUpdateProduct/",
//        data: JSON.stringify({ 'attributeId': mainModuleId, 'AttributeDetailData': allData2 }),
//        dataType: 'json',
//        contentType: 'application/json; charset=utf-8',
//        success: function (data) {
//            if (data.status) {
//                toastr.success("Attributes Assigned Successfully", "Success", { timeOut: 3000, "closeButton": true });
//                $('.close').click();
//                oTable.ajax.reload(null, false);
//            } else {
//                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
//            }
//        }
//    });
//});
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
    if ($('#ProductId').val() > 0) {

        if ($('#CategoryId').find("option[value='" + $('#HiddenCategoryId').val() + "']").length) {
            $('#CategoryId').val($('#HiddenCategoryId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenCategoryName').val(), $('#HiddenCategoryId').val(), true, true);
            // Append it to the select
            $('#CategoryId').append(newOption).trigger('change');
        }
    }
    else {
        $('#CategoryId').val(null).trigger('change');

    }
    if ($('#ProductId').val() > 0) {

        if ($('#UnitId').find("option[value='" + $('#HiddenUnitId').val() + "']").length) {
            $('#UnitId').val($('#HiddenUnitId').val()).trigger('change');
        } else {
            // Create a DOM Option and pre-select by default
            var newOption = new Option($('#HiddenUnitName').val(), $('#HiddenUnitId').val(), true, true);
            // Append it to the select
            $('#UnitId').append(newOption).trigger('change');
        }
    }
    else {
        $('#UnitId').val(null).trigger('change');

    }
    var $checkoutForm = $('#popupForm').validate({
        rules: {
            ProductName: {
                required: true
            },
            CategoryId: {
                required: true
            },
            CostPrice: {
                required: true
            },
        },
        messages: {
            ProductName: {
                required: 'Product Name is required.'
            },
            CategoryId: {
                required: 'Category is required.'
            },
            CostPrice: {
                required: 'Cost Price is required.'
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

$('#UnitId').select2({
    ajax: {
        delay: 150,
        url: '/Admin/Combolist/GetUnitsOptionList/',
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
    placeholder: "-- Select Units--",
    minimumInputLength: 0,
    dropdownParent: $(".mySelect"),
    allowClear: true,
});
$('#popupForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupForm").valid()) {
        return false;
    }
    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var ProductId = $('#ProductId').val();
    var ProductName = $('#ProductName').val();
    var ProductNameUrdu = $('#ProductNameUrdu').val();
    var UnitId = $('#UnitId').val();
    if (UnitId == null || UnitId == undefined) {
        UnitId = 0;
    }
    var CostPrice = $('#CostPrice').val();
    var Barcode = $('#Barcode').val();
    var CategoryId = $('#CategoryId').val();
    if (CategoryId == null || CategoryId == undefined) {
        CategoryId = 0;
    }
    var SalePrice = $('#SalePrice').val();
    var StatusString = "No";
    if ($("#Status").is(":checked")) {
        StatusString = "Yes";
    }
    var ParentCategoryName = $('#ParentCategoryName').val();
    var ParentCategoryId = $('#ParentCategoryId').val();
    var AttributeDetailData = [];
    var mainAttributeId = 0;

    $('.AttributesList').each(function () {
        var attributeId = $(this).find('.AttributeId').val();
        var attributeDetailId = $(this).find('.AttributeDetailId').val();
        mainAttributeId = attributeId;
        var isChecked = $(this).find('.moduleCheckbox').is(':checked');

        if (isChecked) {
            var data = {
                'AttributeId': attributeId,
                'AttributeDetailId': attributeDetailId,
            };
            AttributeDetailData.push(data);
        }
    });

    var allData = JSON.stringify(AttributeDetailData);

    var st =
    {
        ProductId: ProductId,
        ProductName: ProductName,
        ProductNameUrdu: ProductNameUrdu,
        SalePrice: SalePrice,
        CostPrice: CostPrice,
        UnitId: UnitId,
        CategoryId: CategoryId,
        Barcode: Barcode,
        ParentCategoryName: ParentCategoryName,
        StatusString: StatusString,
        ParentCategoryId: ParentCategoryId,
        AttributeDetailData: allData
    }
   

    $.ajax({
        type: "POST",
        url: "/Admin/Product/InsertUpdateProduct/",
        data: JSON.stringify({ 'user': st, 'AttributeDetailData': allData }),

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
$("#CostPrice").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});