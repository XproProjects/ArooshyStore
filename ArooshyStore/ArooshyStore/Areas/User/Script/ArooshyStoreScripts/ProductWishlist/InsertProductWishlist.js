$(document).ready(function () {
    $(document).off('click', '.add-to-wishlist').on('click', '.add-to-wishlist', function () {
        var WishlistId = $('#WishlistId').val();
        var UserId = $('#UserId').val();
        var ProductId = $(this).data('product-id');

        if (UserId === "") {
            toastr.error("You need to log in to add products to your wishlist.", "Error", { timeOut: 3000, "closeButton": true });
            return;
        }

        var wishlistData = {
            WishlistId: WishlistId,
            UserId: UserId,
            ProductId: ProductId
        };

        $.ajax({
            type: "POST",
            url: "/User/CustomerAccount/InsertUpdateWishlist/",
            data: { 'user': wishlistData },
            dataType: 'json',
            success: function (data) {
                $('#btn_Save').html("Save");
                $('#btn_Save').prop('disabled', false);
                if (data.status) {
                    toastr.success("Product successfully added to wishlist", "Success", { timeOut: 3000, "closeButton": true });
                    oTable.ajax.reload(null, false);
                } else {
                    toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
                }
            }
        });
    });

    // Remove from wishlist functionality
    $(document).off('click', '.remove-from-wishlist').on('click', '.remove-from-wishlist', function () {
        var wishlistId = $(this).data('id');
        if (confirm('Are you sure you want to remove this item from your wishlist?')) {
            $.ajax({
                url: "/User/CustomerAccount/DeleteWishlistProduct/",
                type: 'POST',
                data: { id: wishlistId },
                dataType: 'json',
                success: function (response) {
                    if (response.status) {
                        toastr.success("Product successfully removed from wishlist", "Success", { timeOut: 3000, "closeButton": true });
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
