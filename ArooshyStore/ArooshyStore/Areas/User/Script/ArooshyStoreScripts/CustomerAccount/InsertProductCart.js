
$(document).ready(function () {
    $(document).on('click', '.add-to-cart', function () {
        var $card = $(this).closest('.card');
        var CartId = $('#CartId').val();
        var Quantity = $('#Quantity').val();
        var DiscountId = $('#DiscountId').val();
        var ProductId = $card.data('product-id');
        var GivenSalePrice = $card.data('given-sale-price');
        var ActualSalePrice = $card.data('actual-sale-price');
        var DiscountAmount = $('#DiscountAmount').val();
        var UserId = $('#UserId').val();
        if (!UserId || UserId === "0") {
            UserId = localStorage.getItem("localUserId");
            if (!UserId) {
                UserId = generateNewUserId();
                localStorage.setItem("localUserId", UserId);
                setCookie(cookieName, UserId, 7);
            }
        }
        $(document).on('click', '#cart', function (e) {
            e.preventDefault();
            var UserId = $('#UserId').val();
            if (!UserId || UserId === "0") {
                UserId = localStorage.getItem("localUserId");
            }
            var redirectUrl = "/user/customeraccount/CartItems?userId=" + UserId;
            window.location.href = redirectUrl;
        });
        var cookieName = "cart_" + UserId;
        var wishlistData = {
            CartId: CartId,
            CookieName: cookieName,
            Quantity: 1,
            DiscountId: DiscountId,
            UserId: UserId,
            ActualSalePrice: ActualSalePrice,
            DiscountAmount: DiscountAmount,
            GivenSalePrice: GivenSalePrice,
            ProductId: ProductId
        };
        function getSectionsData() {
            var arrayData = [];
            var productId = $('#product-container').data('product-id');

            $('.attribute-select').each(function () {
                var selectedOption = $(this).find("option:selected");
                var attributeDetailId = selectedOption.val();
                if (attributeDetailId) {
                    var alldata = {
                        'AttributeDetailId': attributeDetailId,
                        'AttributeId': $(this).data('attribute-detail-id'),
                        'ProductId': productId,
                        'CartId': CartId
                    };
                    arrayData.push(alldata);
                }
            });
            return arrayData;
        }

        // Capture selected attributes

        var detail = JSON.stringify(getSectionsData());
        $.ajax({
            type: "POST",
            url: "/User/CustomerAccount/InsertUpdateCart/",
            data: { 'user': wishlistData, 'data': detail, },
            dataType: 'json',
            success: function (data) {
                $('#btn_Save').html("Save");
                $('#btn_Save').prop('disabled', false);
                if (data.status) {
                    toastr.success("Product successfully added to cart", "Success", { timeOut: 3000, "closeButton": true });
                    oTable.ajax.reload(null, false);
                } else {
                    toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
                }
            }
        });
    });


});
$(document).ready(function () {
    $(document).on('click', '.remove-from-cart', function () {
        var cartId = $(this).data('id');
        if (confirm('Are you sure you want to remove this item from your wishlist?')) {
            $.ajax({
                url: "/User/CustomerAccount/DeleteCartProduct/",
                type: 'POST',
                data: { id: cartId },
                dataType: 'json',
                success: function (response) {
                    if (response.status) {
                        toastr.success("Product successfully removed from cart", "Success", { timeOut: 3000, "closeButton": true });
                        location.reload();
                    } else {
                        toastr.error(response.message, "Error", { timeOut: 3000, "closeButton": true });
                    }
                },
                error: function () {
                    toastr.error('An error occurred while deleting the item. Please try again.', "Error", { timeOut: 3000, "closeButton": true });
                }
            });
        }
    });
});