document.addEventListener("DOMContentLoaded", function () {
    const listViewButton = document.getElementById("listViewButton");
    const gridViewButton = document.getElementById("gridViewButton");
    const productList = document.getElementById("productList");
    const productsGrid = document.getElementById("productsGrid");

    productList.style.display = "none";
    productsGrid.style.display = "block";

    // List view button click
    listViewButton.addEventListener("click", function () {
        productList.style.display = "block";
        productsGrid.style.display = "none";
    });

    // Grid view button click
    gridViewButton.addEventListener("click", function () {
        productList.style.display = "none";
        productsGrid.style.display = "block";
    });
});
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

        $('#btnFind').html("<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span>").prop("disabled", true);

        // AJAX call
        $.ajax({
            type: "POST",
            url: "/User/Home/SetSearchString",
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
                //$('#productsGrid').html(response);
                if ($('#productList').is(':visible')) {
                    $('#productList').html(response);
                } else if ($('#productsGrid').is(':visible')) {
                    $('#productsGrid').html(response);
                }

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
