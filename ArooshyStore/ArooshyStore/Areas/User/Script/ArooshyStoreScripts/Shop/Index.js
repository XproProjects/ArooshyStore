$(document).ready(function () {
    $('#sortSelect').change(function () {
        filterProducts();
    });
    $('input[name="category"], input[name="attribute"], input[name="discountOffer"]').change(function () {
        filterProducts();
    });

    function filterProducts() {
        var categoryArray = [];
        $('input[name="category"]:checked').each(function () {
            categoryArray.push(parseInt($(this).val(), 10));
        });

        var attributeArray = [];
        $('input[name="attribute"]:checked').each(function () {
            attributeArray.push(parseInt($(this).val(), 10));
        });

        var discountArray = [];
        $('input[name="discountOffer"]:checked').each(function () {
            discountArray.push(parseInt($(this).val(), 10));
        });

        var minPrice = $('#MinPriceCheckBox').val();
        var maxPrice = $('#maxPriceCheckBox').val();
        var selectedSort = $('#sortSelect').val();
        if (!minPrice && !maxPrice && categoryArray.length === 0 && attributeArray.length === 0 && discountArray.length === 0) {
            toastr.error("Please select at least one filter.", "Error", { timeOut: 3000, closeButton: true });
            return false;
        }

        $('#btnFind').html("<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span>").prop("disabled", true);

        $.ajax({
            type: "POST",
            url: "/User/Shop/SetSearchString",
            dataType: 'html',
            data: {
                categoryCheckbox: categoryArray.length > 0,
                category: categoryArray,
                attributeCheckbox: attributeArray.length > 0,
                attribute: attributeArray,
                discountCheckbox: discountArray.length > 0,
                discount: discountArray,
                minPrice: minPrice,
                maxPrice: maxPrice,
                sortBy: selectedSort
            },
            success: function (response) {
                console.log("Response received:", response);
                $('#productsGrid').html(response);
                updatePaginationAfterFilter(response); 
            },
            error: function (xhr, status, error) {
                toastr.error("An error occurred while filtering products.", "Error", { timeOut: 3000, closeButton: true });
                console.log(xhr.responseText);
            },
            complete: function () {
                $('#btnFind').html("<i class='fas fa-search'></i>").prop("disabled", false);
            }
        });
    }
});
