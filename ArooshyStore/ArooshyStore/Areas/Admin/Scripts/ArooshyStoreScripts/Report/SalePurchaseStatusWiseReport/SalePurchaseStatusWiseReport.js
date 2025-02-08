$(function () {
    $('#ParentCategoryId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetAllCategoryOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                    type : "master"
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
        placeholder: "-- Select Master Category --",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });

    $('#ChildCategoryId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetAllCategoryOptionList/',
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
        placeholder: "-- Select Child Category --",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });

    $('#ProductId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetAllProductsOptionList/',
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
        placeholder: "-- Select Product --",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });

    $('#ProductAttributeDetailBarcodeId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetProductAllAttributesFromBarcodeTableOptionList/',
            dataType: 'json',

            data: function (params) {
                params.page = params.page || 1;
                return {
                    searchTerm: params.term,
                    pageSize: 20,
                    pageNumber: params.page,
                    productId: $("#ProductId").val()
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
        placeholder: "-- Select Size & Color --",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#Status').select2({
        placeholder: "-- Select Invoice Status --",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#Status').val(null).trigger("change");
});
