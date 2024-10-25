// Function to set a cookie
function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

// Function to get a cookie by name
function getCookie(name) {
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i].trim();
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function generateUniqueCookieId() {
    return 'cookie_' + Math.random().toString(36).substring(2, 9);
}


// Function to update cart count
function updateCartCount(cookieName) {
    $.ajax({
        type: "GET",
        url: "/User/CustomerAccount/GetCartItemCount",
        data: { cookieName: cookieName },
        success: function (response) {
            if (response.status) {
                $('.alert-count').text(response.count);
                $('.cart-header-title').text(response.count + " ITEMS");
            } else {
                toastr.error(response.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        },
        error: function () {
            toastr.error('An error occurred while retrieving cart count.', "Error", { timeOut: 3000, closeButton: true });
        }
    });
}

// Function to load cart items
function loadCartItems() {
    const cookieName = getCookie('CookieName');
    if (cookieName) {
        $.ajax({
            type: "GET",
            url: "/User/CustomerAccount/CartDropdown",
            data: { userIdOrCookieName: cookieName },
            success: function (data) {
                $('.cart-list').html(data);
            },
            error: function () {
                toastr.error('An error occurred while loading cart items.', "Error", { timeOut: 3000, closeButton: true });
            }
        });
    } else {
        toastr.error('No cart cookie found!', 'Error', { timeOut: 3000, closeButton: true });
    }
}

// On document ready
$(document).ready(function () {
    const cookieName = getCookie('CookieName') || generateUniqueCookieId();
    setCookie('CookieName', cookieName, 7);

    updateCartCount(cookieName); 
    loadCartItems(); 

    $(document).on('click', '.add-to-cart', function () {
        const $card = $(this).closest('.card');
        const CartId = $('#CartId').val();
        const Quantity = $('#Quantity').val();
        const DiscountId = $('#DiscountId').val();
        const ProductId = $card.data('product-id');
        const GivenSalePrice = $card.data('given-sale-price');
        const ActualSalePrice = $card.data('actual-sale-price');
        const DiscountAmount = $('#DiscountAmount').val();
        let UserId = $('#UserId').val() || (getCookie('UserId') || generateUniqueCookieId());
        if (!$('#UserId').val()) {
            setCookie('UserId', UserId, 7);
        }

        const wishlistData = {
            CartId: CartId,
            CookieName: cookieName,
            Quantity: Quantity ,
            DiscountId: DiscountId,
            UserId: UserId,
            ActualSalePrice: ActualSalePrice,
            DiscountAmount: DiscountAmount,
            GivenSalePrice: GivenSalePrice,
            ProductId: ProductId
        };
        function getSectionsData() {
            const arrayData = [];
            const productId = $('#product-container').data('product-id');

            $('.attribute-select').each(function () {
                const selectedOption = $(this).find("option:selected");
                const attributeDetailId = selectedOption.val();
                if (attributeDetailId) {
                    arrayData.push({
                        'AttributeDetailId': attributeDetailId,
                        'AttributeId': $(this).data('attribute-detail-id'),
                        'ProductId': productId,
                        'CartId': CartId
                    });
                }
            });
            return arrayData;
        }

        const detail = JSON.stringify(getSectionsData());
        $.ajax({
            type: "POST",
            url: "/User/CustomerAccount/InsertUpdateCart/",
            data: { 'user': wishlistData, 'data': detail },
            dataType: 'json',
            success: function (data) {
                $('#btn_Save').html("Save").prop('disabled', false);
                if (data.status) {
                    toastr.success("Product successfully added to cart", "Success", { timeOut: 3000, "closeButton": true });
                    updateCartCount(cookieName); // Update cart count after adding item
                    loadCartItems(); 
                } else {
                    toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
                }
            }
        });
    });

    // Redirect to Cart on '#cart' button click
    $(document).on('click', '#cart', function () {
        const cookieName = getCookie('CookieName');
        if (cookieName) {
            window.location.href = "/user/customeraccount/CartItems?cookieName=" + cookieName;
        } else {
            toastr.error('No cart cookie found!', 'Error', { timeOut: 3000, closeButton: true });
        }
    });

    // Remove item from cart
    $(document).off('click', '.remove-from-cart').on('click', '.remove-from-cart', function () {
        const cartId = $(this).data('id');
        if (confirm('Are you sure you want to remove this item from your cart?')) {
            $.ajax({
                url: "/User/CustomerAccount/DeleteCartProduct/",
                type: 'POST',
                data: { id: cartId },
                dataType: 'json',
                success: function (response) {
                    if (response.status) {
                        toastr.success("Product successfully removed from cart", "Success", { timeOut: 3000, closeButton: true });
                        $('.close').click(); 
                        loadCartItems(); 
                        updateCartCount(cookieName); 

                        setTimeout(function () {
                            location.reload();
                        }, 500);
                    } else {
                        toastr.error(response.message, "Error", { timeOut: 3000, closeButton: true });
                    }
                },
                error: function () {
                    toastr.error('An error occurred while deleting the item. Please try again.', "Error", { timeOut: 3000, closeButton: true });
                }
            });
        }
    });

    //review produts button
    $('#review-button').on('click', function (e) {
        e.preventDefault();
        if (!$('#checkoutForm').valid()) {
            toastr.error('Please fill in all required information before reviewing your cart.', 'Error', { timeOut: 3000, closeButton: true });
            return;
        }
        const cookieName = getCookie('CookieName');
        if (cookieName) {
            window.location.href = "/User/CheckOut/CheckOutReview?cookieName=" + cookieName;
        } else {
            toastr.error('No cart cookie found!', 'Error', { timeOut: 3000, closeButton: true });
        }
    });

});

$(document).ready(function () {
    function loadCartItems() {
        var cookieName = getCookie('CookieName'); 
        if (cookieName) {
            $.ajax({
                type: "GET",
                url: "/User/CustomerAccount/CartDropdown", 
                data: { userIdOrCookieName: cookieName }, 
                success: function (data) {
                    $('.cart-list').html(data);
                },
                error: function () {
                    toastr.error('An error occurred while loading cart items.', "Error", { timeOut: 3000, closeButton: true });
                }
            });
        } else {
            toastr.error('No cart cookie found!', 'Error', { timeOut: 3000, closeButton: true });
        }
    }
    loadCartItems(); 
});

$(document).ready(function () {
    $('.complete-order').on('click', function (e) {
        e.preventDefault();
        toastr.success("Your order has been saved successfully", "Success", {
            timeOut: 3000,  
            closeButton: true
        });
        setTimeout(function () {
            var reviewUrl = "/User/CheckOut/CheckOutComplete";
            window.location.href = reviewUrl;
        }, 3000); 
    });
});



