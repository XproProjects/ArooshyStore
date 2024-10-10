$('#btnFind').click(function () {
    var categoryArray = [];
    $('input[name="category"]:checked').each(function () {
        var id = parseInt($(this).val(), 10); // Parse to int
        categoryArray.push(id);
    });

    var attributeArray = [];
    $('input[name="attribute"]:checked').each(function () {
        var id = parseInt($(this).val(), 10); // Parse to int
        attributeArray.push(id);
    });

    var discountArray = [];
    $('input[name="discountOffer"]:checked').each(function () {
        var id = parseInt($(this).val(), 10); // Parse to int
        discountArray.push(id);
    });

    var minPrice = $('#MinPriceCheckBox').val();
    var maxPrice = $('#maxPriceCheckBox').val();

    if (categoryArray.length === 0 && attributeArray.length === 0 && discountArray.length === 0 && (!minPrice && !maxPrice)) {
        toastr.error("Please select at least one filter.", "Error", { timeOut: 3000, "closeButton": true });
        return false;
    }

    $('#btnFind').html("<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span>").prop("disabled", true);

    $.ajax({
        type: "POST",
        url: "/User/Shop/SetSearchString",
        dataType: 'json',
        data: {
            'category': categoryArray,
            'attribute': attributeArray,
            'discount': discountArray,
            'minPrice': minPrice,
            'maxPrice': maxPrice
        },
        success: function (response) {
            $('#productsGrid').html(response);
        },
        error: function (xhr, status, error) {
            toastr.error("An error occurred while filtering products.", "Error", { timeOut: 3000, "closeButton": true });
            console.log(xhr.responseText);
        },
        complete: function () {
            $('#btnFind').html("<i class='fas fa-search'></i>").prop("disabled", false);
        }
    });
});
