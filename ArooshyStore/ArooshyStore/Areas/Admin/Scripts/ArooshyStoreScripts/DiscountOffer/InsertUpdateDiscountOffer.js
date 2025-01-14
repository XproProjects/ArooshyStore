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
    $('.nav-menu li a[href="/admin/home/setting"]').parent("li").addClass('active');
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
    LoadProductsList();
    $('#MasterCategoryId').val(null).trigger('change');
    $('#ChildCategoryId').val(null).trigger('change');
    $('#ProductId').val(null).trigger('change');
    var $checkoutForm = $('#popupForm').validate({
        ignore: ":not(:visible)",
        rules: {
            DiscountName: {
                required: true
            },
            ExpiredOn: {
                required: true
            },
        },
        messages: {
            DiscountName: {
                required: 'Offer Name is required.'
            },
            ExpiredOn: {
                required: 'Expiry Date is required.'
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
function LoadProductsList() {
    $("#ProductsListDiv").html('<center>' +
        '<div class="demo" style="margin-top:60px">' +
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
        url: "/Admin/DiscountOffer/ProductsList/?id=" + $('#OfferId').val(),
        //contentType: 'application/html; charset=utf-8', type: 'GET', dataType: 'html',
        success: function (response) {
            //$('#ProductsData').html('');
            $("#ProductsListDiv").html(response);
        }
    })
}
$(document).off("click", "#btnFilter").on("click", "#btnFilter", function () {
    if ($(this).attr("data-value") == "0") {
        $(this).attr("data-value", "1");
        $(this).html("Hide Filters");
        $("#FiltersDiv").removeAttr("hidden");
    }
    else {
        $(this).attr("data-value", "0");
        $(this).html("Show Filters");
        $("#FiltersDiv").attr("hidden", "hidden");
        $(".DataTr").filter(function () {
            $(this).toggle($(this).find(".ParentCategoryId").val().indexOf("") > -1)
        });
    }
    $('#MasterCategoryId').val(null).trigger('change.select2');
    $('#ChildCategoryId').val(null).trigger('change.select2');
    $('#ProductId').val(null).trigger('change.select2');
})
$("#MasterCategoryId").change(function () {
    var value = $(this).val();
    if (value != null && value != 0 && value != "" && value != undefined) {
        $(".DataTr").filter(function () {
            $(this).toggle($(this).find(".ParentCategoryId").val().indexOf(value) > -1)
        });
    }
    else {
        $(".DataTr").filter(function () {
            $(this).toggle($(this).find(".ParentCategoryId").val().indexOf("") > -1)
        });
    }
})
$("#ChildCategoryId").change(function () {
    var value = $(this).val();
    if (value != null && value != 0 && value != "" && value != undefined) {
        $(".DataTr").filter(function () {
            $(this).toggle($(this).find(".CategoryId").val().indexOf(value) > -1)
        });
    }
    else {
        $(".DataTr").filter(function () {
            $(this).toggle($(this).find(".CategoryId").val().indexOf("") > -1)
        });
    }
})
$("#ProductId").change(function () {
    var value = $(this).val();
    if (value != null && value != 0 && value != "" && value != undefined) {
        $(".DataTr").filter(function () {
            $(this).toggle($(this).find(".ProductId").val().indexOf(value) > -1)
        });
    }
    else {
        $(".DataTr").filter(function () {
            $(this).toggle($(this).find(".ProductId").val().indexOf("") > -1)
        });
    }
})
function getRowsDataList() {
    var arrayData = [];
    $('.DataTr').each(function () {
        var ProductId = $(this).find('.ProductId').val();
        var DiscountType = $(this).find('.DiscountType').val();
        var DiscountRate = $(this).find('.DiscountRate').val();
        if (DiscountRate > 0) {
            var alldata = {
                'ProductId': ProductId,
                'DiscountType': DiscountType,
                'DiscountRate': DiscountRate,
            }
            arrayData.push(alldata);
        }
    });
    return arrayData;
}
$('#popupForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupForm").valid()) {
        return false;
    }
    var OfferId = $('#OfferId').val();
    var DiscountName = $('#DiscountName').val();
    var ExpiredOn = $('#ExpiredOn').val();
    var StatusString = "No";
    if ($("#Status").is(":checked")) {
        StatusString = "Yes";
    }
    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var st =
    {
        OfferId: OfferId,
        DiscountName: DiscountName,
        ExpiredOn: ExpiredOn,
        StatusString: StatusString,
    }
    var detail = JSON.stringify(getRowsDataList());
    $.ajax({
        type: "POST",
        url: "/Admin/DiscountOffer/InsertUpdateDiscountOffer/",
        data: JSON.stringify({ 'user': st, 'data': detail }),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
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
                                    window.location.href = "/admin/discountoffer/index";
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
                    window.location.href = "/admin/discountoffer/index";
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
    NumberPostiveNegativeWithDecimal(event, this, 2, 2);
});

$(document).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();
    }
});