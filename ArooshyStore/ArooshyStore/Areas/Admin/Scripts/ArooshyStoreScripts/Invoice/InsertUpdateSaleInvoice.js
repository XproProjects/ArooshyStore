﻿$('input[type=radio][name=SelectItemType]').change(function () {
    var radioValue = $("input[name='SelectItemType']:checked").val();
    if (radioValue == 'Existing Item') {
        $('.BarcodeDiv').removeAttr("hidden");
        $('.NewItemDiv').attr("hidden", "hidden");
    }
    else if (radioValue == 'New Item') {
        $('.BarcodeDiv').attr("hidden", "hidden");
        $('.NewItemDiv').removeAttr("hidden");
    }
    else {
        $('.BarcodeDiv').removeAttr("hidden");
        $('.NewItemDiv').attr("hidden", "hidden");
    }
    $('#BarcodeId').val(null).trigger('change.select2');

    $('#Rate').val('0');
    $('#LineDiscType').val('%');
    $('#LineDiscRate').val('0');
    $('#LineDiscAmount').val('0');
    $('#LineNetAmount').html('0.00');
    $('#Description').val('');


    $('#btnCancelDetail').hide();
    $('#btnAddDetail').find('.btnLineSpan').html('Add');

    $('#BarcodeIdDiv').closest('#BarcodeIdDiv').find('.select2-choice').css('border-color', '#CCCCCC');
    $('#BarcodeIdDiv').closest('#BarcodeIdDiv').find('.select2-choice').css('color', 'black');

    $('#Rate').css('border-color', '#bdbdbd');
    $('#Rate').css('background-color', '#fff');
});



$('#Rate').keyup(function () {
    getLineTotals();
})
$('#LineDiscType').change(function () {
    getLineTotals();
})
$('#LineDiscRate').keyup(function () {
    getLineTotals();
})
$('#Qty').keyup(function () {
    getLineTotals();
});
function getLineTotals() {
    var rate = $('#Rate').val();
    var quantity = $('#Qty').val();
    var discountType = $('#LineDiscType').val();
    var discountAmount = $('#LineDiscRate').val();
    var deliveryCharges = $('#DeliveryCharges').val()
    if (rate == '' || rate == null || rate == undefined || isNaN(rate)) {
        rate = 0;
    }
    if (discountType == '' || discountType == null) {
        discountType = "%";
    }
    if (discountAmount == '' || discountAmount == null || discountAmount == undefined || isNaN(discountAmount)) {
        discountAmount = 0;
    }
    if (deliveryCharges == '' || deliveryCharges == null || isNaN(deliveryCharges)) {
        deliveryCharges = 0; 
    }
    var multiplier = 1;
    if (quantity != '' && quantity != null && !isNaN(quantity)) {
        multiplier = parseFloat(quantity);
    }
    var netDiscount = 0;
    if (discountType.trim() == '%') {
        netDiscount = parseFloat(discountAmount) / 100 * parseFloat(rate);
        netDiscount = parseFloat(netDiscount).toFixed(2);
    }
    else if (discountType.trim() == '€') {
        netDiscount = parseFloat(discountAmount).toFixed(2);
    }
    else {
        netDiscount = 0;
    }
    $('#LineDiscAmount').val(netDiscount);

    var netPrice = parseFloat(rate) - parseFloat(netDiscount);
    netPrice = parseFloat(netPrice).toFixed(2);
    if (multiplier > 1) {
        netDiscount = (netDiscount * multiplier).toFixed(2);
        netPrice = (parseFloat(rate) * multiplier - parseFloat(netDiscount)).toFixed(2);
    }

    // Set the values
    $('#LineDiscAmount').val(netDiscount);
    $('#LineNetAmount').html(ReplaceNumberWithCommas(netPrice));

}
function GetInvoiceNo() {
    $.ajax({
        type: "POST",
        url: "/Invoice/GetMaxCodeForInvoice/",
        dataType: 'json',
        data: { 'type': 'Sale Invoice' },
        success: function (response) {
            $('#InvoiceNumber').val(response);
            $('#InvoiceNumberLbl').text(response);
        }
    })
}
function updateTotalAmount() {
    var vtable = $('#detailTable').DataTable();
    var totalAmount = 0;

    vtable.rows().every(function () {
        var data = this.data();
        var netPrice = parseFloat($(data[9]).text().trim());
        if (!isNaN(netPrice)) {
            totalAmount += netPrice;
        }
    });

    $('#TotalAmount').text(ReplaceNumberWithCommas(totalAmount.toFixed(2)));

    // Call updateNetAmount after updating the total amount
    updateNetAmount();
}
$('#btnAddDetail').click(function () {
    updateTotalAmount();
});
$(document).ready(function () {
    updateNetAmount();
    $('#DeliveryCharges').on('change keyup', function () {
        updateNetAmount();
    });
    $('#DiscType, #DiscRate').on('change keyup', function () {
        updateNetAmount();
    });

});
//netamount after adding deliver and gross amount
function updateNetAmount() {
    var grossAmount = parseFloat($('#TotalAmount').text().replace(/,/g, '')) || 0;
    var discType = $('#DiscType').val();
    var discRate = parseFloat($('#DiscRate').val()) || 0;

    var discountAmount = 0;
    if (discType === '%') {
        discountAmount = (grossAmount * discRate / 100).toFixed(2);
    } else if (discType === '€') {
        discountAmount = discRate.toFixed(2);
    }

    // Populate the DiscAmount field
    $('#DiscAmount').val(discountAmount);

    var amountAfterDiscount = (parseFloat(grossAmount) - parseFloat(discountAmount)).toFixed(2);
    var deliveryCharges = parseFloat($('#DeliveryCharges').val()) || 0;
    var netAmount = (parseFloat(amountAfterDiscount) + parseFloat(deliveryCharges)).toFixed(2);

    // Update NetAmount field
    $('#NetAmount').text(ReplaceNumberWithCommas(netAmount));
}


$(function () {
    $('#btnCancelDetail').hide();

    $('#detailTable').dataTable({
        "sDom": '<"top">rt<"bottom"lp i><"clear">',
        "autoWidth": true,
        "bPaginate": false,
        "serverSide": false,
        "bLengthChange": false,
        "processing": false,
        "searching": false,
        "bInfo": false,
        "ordering": false,
        "scrollX": true,
        //"scrollY": 200
    });
    if ($('#IsNewOrEdit').val() == 'Update') {

    }
    else {

        GetInvoiceNo();

    }
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
    $('#CustomerSupplierId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetCustomerSupplierOptionList/',
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
        placeholder: "-- Select--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#CustomerSupplierId').change(function () {
        var customerSupplierId = $(this).val();

        if (customerSupplierId) {
            $.ajax({
                "url": "/Admin/CustomerSupplier/GetDeliveryChargesForCustomer",
                type: 'POST',
                data: { customerSupplierId: customerSupplierId },
                success: function (response) {
                    if (response.deliveryCharges) {
                        $('#DeliveryCharges').val(response.deliveryCharges);
                        console.log("Delivery Charges: " + response.deliveryCharges);
                    } else {
                        console.log("No deliveryCharges available");
                    }
                },
                error: function () {
                    alert('Error fetching product details.');
                }
            });
        } else {
            $('#DeliveryCharges').val('0');
        }
    });
    $('#AttributeDetailId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetAttributesDetailOptionList/',
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
        placeholder: "-- Select Attribute Detail--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#AttributeId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetAttributesOptionList/',
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
        placeholder: "-- Select Attribute--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#DiscountOfferId').select2({
        ajax: {
            delay: 150,
            url: '/Admin/Combolist/GetDiscountOffersOptionList/',
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
        placeholder: "-- Select Discount Offer--",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });
    $('#ProductId').change(function () {
        var productId = $(this).val();

        if (productId) {
            $.ajax({
                "url": "/Admin/Product/GetProductSalePrice",
                type: 'POST',
                data: { productId: productId },
                success: function (response) {
                    if (response.salePrice) {
                        $('#Rate').val(response.salePrice);
                        $('#SelectedProductName').val(response.productName);
                    } else {
                       console.log("No sale price available");
                    }
                },
                error: function () {
                    alert('Error fetching product details.');
                }
            });
        } else {
            $('#Rate').val('0');
        }
    });


    $('#btnAddDetail').click(function () {

        var check = 0;

        // Retrieve values
        var salesPrice = $('#Rate').val();
        var discountType = $('#LineDiscType').val();
        var discountRate = $('#LineDiscRate').val();
        var discountAmount = $('#LineDiscAmount').val();
        var productName = $('#SelectedProductName').val();
        var ProductId = $('#ProductId').val();
        var DiscountOfferId = $('#DiscountOfferId').val();
        var AttributeId = $('#AttributeId').val();
        var AttributeDetailId = $('#AttributeDetailId').val();
        var MasterCategoryId = $('#MasterCategoryId').val();
        var ChildCategoryId = $('#ChildCategoryId').val();
        $('#DeliveryCharges').val(response.deliveryCharges);

        var CustomerSupplierId = $('#CustomerSupplierId').val();
        var quantity = $('#Qty').val();
        var multiplier = 1;
        if (quantity != '' && quantity != null && !isNaN(quantity) && parseFloat(quantity) > 0) {
            multiplier = parseFloat(quantity);
        }
        if (salesPrice == '' || salesPrice == null || salesPrice == 0) {
            $('#Rate').css('border-color', '#A90329');
            $('#Rate').css('background-color', '#FFF0F0');
            check = 1;
        } else {
            $('#Rate').css('border-color', '');
            $('#Rate').css('background-color', '');
        }

        if (check > 0) {
            return false;
        }

        if (discountType == '' || discountType == null) {
            discountType = '%';
        }

        if (discountRate == '' || discountRate == null) {
            discountRate = 0;
        }

        if (discountAmount == '' || discountAmount == null) {
            discountAmount = 0;
        }

        var AttributeName = "";
        if (AttributeId == '' || AttributeId == null || AttributeId == 0) {
            AttributeId = 0;
            AttributeName = "";
        } else {
            AttributeName = $('#AttributeId option:selected').text();
        }
        var AttributeDetailName = "";
        if (AttributeDetailId == '' || AttributeDetailId == null || AttributeDetailId == 0) {
            AttributeDetailId = 0;
            AttributeDetailName = "";
        } else {
            AttributeDetailName = $('#AttributeDetailId option:selected').text();
        }
        var MasterCategoryName = "";
        if (MasterCategoryId == '' || MasterCategoryId == null || MasterCategoryId == 0) {
            MasterCategoryId = 0;
            MasterCategoryName = "";
        } else {
            MasterCategoryName = $('#MasterCategoryId option:selected').text();
        }
        var ChildCategoryName = "";
        if (ChildCategoryId == '' || ChildCategoryId == null || ChildCategoryId == 0) {
            ChildCategoryId = 0;
            ChildCategoryName = "";
        } else {
            ChildCategoryName = $('#ChildCategoryId option:selected').text();
        }
        var ProductName = "";
        if (ProductId == '' || ProductId == null || ProductId == 0) {
            ProductId = 0;
            ProductName = "";
        } else {
            ProductName = $('#ProductId option:selected').text();
        }

        var DiscountName = "";
        if (DiscountOfferId == '' || DiscountOfferId == null || DiscountOfferId == 0) {
            DiscountOfferId = 0;
            DiscountName = "";
        } else {
            DiscountName = $('#DiscountOfferId option:selected').text();
        }
        var CustomerName = "";
        if (CustomerSupplierId == '' || CustomerSupplierId == null || CustomerSupplierId == 0) {
            CustomerSupplierId = 0;
            CustomerName = "";
        } else {
            CustomerName = $('#CustomerSupplierId option:selected').text();
        }
        var netDiscount = 0;
        if (discountType.trim() == '%') {
            netDiscount = (parseFloat(discountRate) / 100) * parseFloat(salesPrice);
        } else if (discountType.trim() == '€') {
            netDiscount = parseFloat(discountAmount);
        } else {
            netDiscount = 0;
        }

        netDiscount = parseFloat(netDiscount).toFixed(2);

        var netPrice = parseFloat(salesPrice) - parseFloat(netDiscount);
        netPrice = parseFloat(netPrice).toFixed(2);

        if (multiplier > 1) {
            netDiscount = (parseFloat(netDiscount) * multiplier).toFixed(2);
            netPrice = (parseFloat(salesPrice) * multiplier - parseFloat(netDiscount)).toFixed(2);
        }

        var vtable = $('#detailTable').DataTable();
        var checkKey = 0;
        var exists = vtable.rows().data().toArray().some(function (row) {
            return $(row[3]).find('input').val() == ProductId;
        });

        if (exists) {
            toastr.error("This product is already added in the list below.", "Error", { timeOut: 3000, "closeButton": true });
            return false;
        }
        if ($(this).find('.btnLineSpan').html().trim() == 'Add') {
            // Check if record already exists
            var rowsCount = vtable.data().length;
            for (irow = 0; irow < rowsCount; irow++) {
                var data = vtable.row(irow).data();
                var prevId = data[3].split('value=')[1].split('/>')[0].trim();
                if (prevId == ProductId) {
                    checkKey += 1;
                }
            }

            // If record already exists, show an error
            if (checkKey > 0) {
                toastr.error("This product is already added in the list below.", "Error", { timeOut: 3000, "closeButton": true });
            } else {
                // Add new row to DataTable
                vtable.row.add([
                    "<a href='javascript:void(0);' class='btn btn-primary editRowBtn' title='Edit' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-edit'></i></a>",
                    "<a href='javascript:void(0);' class='btn btn-danger deleteRowBtn' title='Delete' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-times-circle'></i></a>",
                    "<center><span class='SrNo'><input type='hidden' class='SR01' value='' /></span></center>",
                    "<span><input type='hidden' class='ProductId' value=" + ProductId + " />" + ProductName + "</span>",
                    "<span class='pull-right'><input type='hidden' class='SalesPrice' value=" + salesPrice + " />" + ReplaceNumberWithCommas(parseFloat(salesPrice).toFixed(2)) + "</span>",
                    "<span class='pull-right'>" + multiplier + "</span>",
                    "<span><input type='hidden' class='DiscountType' value=" + discountType + " />" + discountType + "</span>",
                    "<span class='pull-right'><input type='hidden' class='DiscountRate' value=" + discountRate + " />" + ReplaceNumberWithCommas(parseFloat(discountRate).toFixed(2)) + "</span>",
                    "<span class='pull-right'><input type='hidden' class='DiscountAmount' value=" + discountAmount + " />" + ReplaceNumberWithCommas(parseFloat(discountAmount).toFixed(2)) + "</span>",
                    "<span class='pull-right'><input type='hidden' class='NetAmount' value='" + netPrice + "' />" + netPrice + "</span>", // Add this line
                    "<span><input type='hidden' class='MasterCategoryId' value=" + MasterCategoryId + " />" + MasterCategoryName + "</span>",
                    "<span><input type='hidden' class='ChildCategoryId' value=" + ChildCategoryId + " />" + ChildCategoryName + "</span>",
                    "<span><input type='hidden' class='AttributeName' value=" + AttributeName + " />" + AttributeName + "</span>",
                    "<span><input type='hidden' class='AttributeDetailId' value=" + AttributeDetailId + " />" + AttributeDetailName + "</span>",
                ]).draw(false);
                updateTotalAmount();

            }
        } else {
            // Edit existing row
            var tr = $('#HiddenTr').val();

            $('#btnAddDetail').find('.btnLineSpan').html('Add');
            $('#btnCancelDetail').hide();

            vtable.row(tr).data([
                "<a href='javascript:void(0);' class='btn btn-primary editRowBtn' title='Edit' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-edit'></i></a>",
                "<a href='javascript:void(0);' class='btn btn-danger deleteRowBtn' title='Delete' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-times-circle'></i></a>",
                "<center><span class='SrNo'><input type='hidden' class='SR01' value='' /></span></center>",
                "<span><input type='hidden' class='ProductId' value=" + ProductId + " />" + ProductName + "</span>",
                "<span class='pull-right'><input type='hidden' class='SalesPrice' value=" + salesPrice + " />" + ReplaceNumberWithCommas(parseFloat(salesPrice).toFixed(2)) + "</span>",
                "<span class='pull-right'>" + multiplier + "</span>",
                "<span><input type='hidden' class='DiscountType' value=" + discountType + " />" + discountType + "</span>",
                "<span class='pull-right'><input type='hidden' class='DiscountRate' value=" + discountRate + " />" + ReplaceNumberWithCommas(parseFloat(discountRate).toFixed(2)) + "</span>",
                "<span class='pull-right'><input type='hidden' class='DiscountAmount' value=" + discountAmount + " />" + ReplaceNumberWithCommas(parseFloat(discountAmount).toFixed(2)) + "</span>",
                "<span class='pull-right'><input type='hidden' class='NetAmount' value='" + netPrice + "' />" + netPrice + "</span>", 
                "<span><input type='hidden' class='MasterCategoryId' value=" + MasterCategoryId + " />" + MasterCategoryName + "</span>",
                "<span><input type='hidden' class='ChildCategoryId' value=" + ChildCategoryId + " />" + ChildCategoryName + "</span>",
                "<span><input type='hidden' class='AttributeId' value=" + AttributeId + " />" + AttributeName + "</span>",
                "<span><input type='hidden' class='AttributeDetailId' value=" + AttributeDetailId + " />" + AttributeDetailName + "</span>",
            ]).draw(false);
            updateTotalAmount();

        }

        if (checkKey == 0) {
            getSerialNo();
            $('#Rate').val('0');
            $('#LineDiscType').val('%');
            $('#Qty').val('');
            $('#LineDiscRate').val('0');
            $('#LineDiscAmount').val('0');
            $('#SelectedProductName').val('');
            $('#ProductId').val(null).trigger('change.select2');
            $('#DiscountOfferId').val(null).trigger('change.select2');
            $('#AttributeId').val(null).trigger('change.select2');
            $('#AttributeDetailId').val(null).trigger('change.select2');
            $('#MasterCategoryId').val(null).trigger('change.select2');
            $('#ChildCategoryId').val(null).trigger('change.select2');
            $('#AttributeName').val('');
            $('#AttributeDetailName').val('');
            $('#MasterCategoryName').val('');
            $('#ChildCategoryName').val('');

            $('#CustomerSupplierId').val(null).trigger('change.select2');
            GetMainTotals();
        }
    });


    // Function to update serial numbers
    function getSerialNo() {
        var i = 0;
        $('.SrNo').each(function () {
            i++;
            $(this).html(i);
        });
    }

    // Cancel button functionality
    $('#btnCancelDetail').click(function () {
        $('#btnCancelDetail').hide();
        $('#Rate').val('0');
        $('#LineDiscType').val('%');
        $('#LineDiscRate').val('0');
        $('#LineDiscAmount').val('0');
        $('#SelectedProductName').val('');
        $('#ProductId').val(null).trigger('change.select2');
        $('#DiscountOfferId').val(null).trigger('change.select2');
        $('#CustomerSupplierId').val(null).trigger('change.select2');
        $('#AttributeId').val(null).trigger('change.select2');
        $('#btnAddDetail').find('.btnLineSpan').html('Add');
        $('#Rate').css('border-color', '#bdbdbd');
        $('#Rate').css('background-color', '#fff');
    });


    // Edit button functionality
    $(document).on('click', '.editRowBtn', function () {
        var vtable = $('#detailTable').DataTable();
        var rowIndex = vtable.row($(this).parents('tr')).index();
        $('#HiddenTr').val(rowIndex);

        // Fetch row data
        var data = vtable.row($(this).parents('tr')).data();

        // Populate the form fields with data for editing
        var productId = $(data[3]).find('input.ProductId').val();
        var salesPrice = $(data[4]).find('input.SalesPrice').val();
        var discountType = $(data[6]).find('input.DiscountType').val();
        var discountRate = $(data[7]).find('input.DiscountRate').val();
        var discountAmount = $(data[8]).find('input.DiscountAmount').val();
        var masterCategoryId = $(data[10]).find('input.MasterCategoryId').val();
        var childCategoryId = $(data[11]).find('input.ChildCategoryId').val();
        var attributeId = $(data[12]).find('input.AttributeId').val();
        var attributeDetailId = $(data[13]).find('input.AttributeDetailId').val();
        var quantity = $(data[5]).text();
        var netAmount = $(data[9]).find('input.NetAmount').val(); 

        //var barcode = $(data[12]).find('input').val();
        $('#ProductId').val(productId).trigger('change.select2');
        $('#Rate').val(salesPrice);
        $('#LineDiscType').val(discountType);
        $('#LineDiscRate').val(discountRate);
        $('#LineDiscAmount').val(discountAmount);
        $('#MasterCategoryId').val(masterCategoryId).trigger('change.select2');
        $('#ChildCategoryId').val(childCategoryId).trigger('change.select2');
        $('#AttributeId').val(attributeId).trigger('change.select2');
        $('#AttributeDetailId').val(attributeDetailId).trigger('change.select2');
        $('#Qty').val(quantity);
        $('#NetAmount').val(netAmount); 

        $('#btnAddDetail').find('.btnLineSpan').html('Update');
        $('#btnCancelDetail').show();
    });


    // Delete button functionality
    $(document).on('click', '.deleteRowBtn', function () {
        var vtable = $('#detailTable').DataTable();
        vtable.row($(this).parents('tr')).remove().draw(false);
        getSerialNo();
    });


    var $checkoutForm = $('#popupForm').validate({
        rules: {
            CustomerSupplierId: {
                required: true
            },
            InvoiceDate: {
                required: true
            },


        },
        messages: {
            CustomerSupplierId: {
                required: 'Customer Name is required.'
            },
            InvoiceDate: {
                required: 'Invoice Date is required.'
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
function enterEvent(event) {
    if (event.keyCode === 13) {
        console.log('Enter key pressed');
        var Barcode = $('#txtBarcode').val();
        if (Barcode !== '') {
            BarcodeScan(Barcode);
        }

        $("input[id=txtBarcode]").click();
        getLineTotals();
        $('#LineDiscount').val('');
    }
}

$(document).ready(function () {
    $('#txtBarcode').on('keydown', enterEvent);
});
function getSerialNos() {
    var i = 0;
    $('.SrNo').each(function () {
        i++;
        $(this).html(i);
    });
}

function BarcodeScan(barcode) {
    $.ajax({
        type: 'POST',
        url: '/Admin/Product/GetProductDetailsByBarcode',
        data: { barcode: barcode },
        success: function (response) {
            console.log('AJAX success:', response);
            if (response.productId > 0) {
                addToDataTable(response);

            } else {
                var errorMessage = "Product not found.";
                toastr.error(errorMessage, "Error", { timeOut: 3000, closeButton: true });
            }
        },
        error: function () {
            alert('Error fetching product details.');
        }
    });
}
$('#txtBarcode').on('change', function () {
    var barcode = $(this).val();

    if (barcode !== '') {
        BarcodeScan(barcode);
    }
});

$(document).ready(function () {
    //Add on enter press
    $('#txtBarcode').on('keydown', function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            var barcode = $('#txtBarcode').val();
            if (barcode !== '') {
                BarcodeScan(barcode);
            }
        }
    });
    // Click event for "Add" button
    $('#btnAddDetail').on('click', function () {
        var barcode = $('#txtBarcode').val();
        if (barcode !== '') {
            BarcodeScan(barcode);
        } else {
            toastr.error("Please enter a barcode.", "Error", { timeOut: 3000, closeButton: true });
        }
    });
    function BarcodeScan(barcode) {
        $.ajax({
            type: 'POST',
            url: '/Admin/Product/GetProductDetailsByBarcode',
            data: { barcode: barcode },
            success: function (response) {
                console.log('AJAX success:', response);
                if (response.productId > 0) {
                    // Check if the product exists in the DataTable
                    var vtable = $('#detailTable').DataTable();
                    var exists = vtable.rows().data().toArray().some(function (row) {
                        return $(row[3]).find('input').val() == response.productId;
                    });

                    if (exists) {
                        toastr.error("This product is already added in the list below.", "Error", { timeOut: 3000, closeButton: true });
                    } else {
                        // Add to DataTable if the product does not exist
                        addToDataTable(response);
                    }
                } else {
                    var errorMessage = "Product not found.";
                    toastr.error(errorMessage, "Error", { timeOut: 3000, closeButton: true });
                }
            },
            error: function () {
                alert('Error fetching product details.');
            }
        });
    }

    function addToDataTable(product) {
        var vtable = $('#detailTable').DataTable();

        // Check if the product already exists in the table
        var exists = vtable.rows().data().toArray().some(function (row) {
            return $(row[3]).find('input').val() == product.productId;
        });

        if (exists) {
            toastr.error("This product is already added in the list below.", "Error", { timeOut: 3000, closeButton: true });
        } else {
            // Calculate the net price
            var netPrice = (parseFloat(product.salePrice) - parseFloat(product.discountAmount || 0)).toFixed(2);

            // Add new row to DataTable by enter key by bar code
            vtable.row.add([
                "<a href='javascript:void(0);' class='btn btn-primary editRowBtn' title='Edit' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-edit'></i></a>",
                "<a href='javascript:void(0);' class='btn btn-danger deleteRowBtn' title='Delete' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-times-circle'></i></a>",
                "<center><span class='SrNo'><input type='hidden' class='SR01' value='' /></span></center>",
                "<span><input type='hidden' class='ProductId' value='" + product.productId + "' />" + product.productName + "</span>",
                "<span class='pull-right'><input type='hidden' class='SalesPrice' value='" + product.salePrice + "' />" + ReplaceNumberWithCommas(parseFloat(product.salePrice).toFixed(2)) + "</span>",
                "<span class='Quantity' style='text-align: center;'>1</span>",
                "<span><input type='hidden' class='DiscountType' value='" + (product.discountType || '%') + "' />" + (product.discountType || '%') + "</span>",
                "<span class='pull-right'><input type='hidden' class='DiscountRate' value='" + (product.discountRate || 0) + "' />" + ReplaceNumberWithCommas(parseFloat(product.discountRate || 0).toFixed(2)) + "</span>",
                "<span class='pull-right'><input type='hidden' class='DiscountAmount' value='" + (product.discountAmount || 0) + "' />" + ReplaceNumberWithCommas(parseFloat(product.discountAmount || 0).toFixed(2)) + "</span>",
                "<span class='pull-right'>" + netPrice + "</span>",
                "<span>" + (product.masterCategoryName || '') + "</span>",
                "<span>" + (product.childCategoryName || '') + "</span>",
                "<span><input type='hidden' class='Attribute' value='" + (product.attribute || '') + "' />" + (product.attribute || '') + "</span>",
                "<span><input type='hidden' class='AttributeDetail' value='" + (product.attributeDetail || '') + "' />" + (product.attributeDetail || '') + "</span>"
            ]).draw(false);

            updateTotalAmount();
            getSerialNos();
            $('#txtBarcode').val('');
        }
    }


    $('#txtBarcode').focus();
});

function getAllData() {
    var arrayData = [];
    var vtable = $('#detailTable').DataTable();

    // Iterate through each row in the DataTable
    vtable.rows().every(function () {
        var data = this.data();

        var getInt = (value) => {
            let intVal = parseInt(value, 10);
            return isNaN(intVal) ? 0 : intVal;
        };

        var getFloat = (value) => {
            let floatVal = parseFloat(value);
            return isNaN(floatVal) ? 0 : floatVal;
        };

        // Extract the relevant data from each row
        var warehouseId = getInt($(data[0]).find('input.WarehouseId').val());
        var productId = getInt($(data[3]).find('input.ProductId').val());
        var salesPrice = getFloat($(data[4]).find('input.SalesPrice').val());
        var qty = getFloat(data[5].split('value=')[1]?.split('/>')[0]?.trim());
        var discountType = data[6].split('value=')[1]?.split('/>')[0]?.trim() || ''; // String for DiscType
        var discountRate = getFloat(data[7].split('value=')[1]?.split('/>')[0]?.trim());
        var discountAmount = getFloat(data[8].split('value=')[1]?.split('/>')[0]?.trim());
        var netAmount = getFloat($(data[9]).find('input.NetAmount').val());
        var masterCategoryId = getInt($(data[10]).find('input.MasterCategoryId').val());
        var childCategoryId = getInt($(data[11]).find('input.ChildCategoryId').val());
        var attributeId = getInt($(data[12]).find('input.AttributeId').val());
        var attributeDetailId = getInt($(data[13]).find('input.AttributeDetailId').val());
        var discountOfferId = getInt($(data[7]).find('input.DiscountOfferId').val());

        var rowData = {
            'WarehouseId': warehouseId,
            'MasterCategoryId': masterCategoryId,
            'ChildCategoryId': childCategoryId,
            'ProductId': productId,
            'AttributeId': attributeId,
            'AttributeDetailId': attributeDetailId,
            'DiscountOfferId': discountOfferId,
            'UnitId': 0,
            'TotalAmount': netAmount,
            'Qty': qty,
            'Rate': salesPrice,
            'DiscType': discountType,
            'DiscRate': discountRate,
            'DiscAmount': discountAmount,
            'NetAmount': netAmount
        };

        arrayData.push(rowData);
    });

    console.log(arrayData);
    return arrayData;
}

function extractValueFromHtml(htmlString, type) {
    var value = '';
    try {
        if (type === 'value') {
            value = htmlString.split('value=')[1].split('/>')[0].trim();
        } else if (type === 'text') {
            value = htmlString.split('/>')[1].split('</span>')[0].trim();
        }
    } catch (e) {
        // Handle any potential errors gracefully
        console.error('Error extracting value:', e);
    }
    return value;
}

$('#popupForm').on('submit', function (e) {
    e.preventDefault();
    if (!$("#popupForm").valid()) {
        return false;
    }
    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var IsNewOrEdit = $('#IsNewOrEdit').val();
    var InvoiceNumber = $('#InvoiceNumber').val();
    var InvoiceDate = $('#InvoiceDate').val();
    var DeliveryCharges = $('#DeliveryCharges').val();
    var CustomerSupplierId = $('#CustomerSupplierId').val();


    var TotalAmount = parseFloat(ReplaceCommas($('#TotalAmount').html()));
    var DiscType = $('#DiscType').val();
    var DiscRate = $('#DiscRate').val();
    var DiscAmount = $('#DiscAmount').val();
    var NetAmount = parseFloat(ReplaceCommas($('#NetAmount').html()));
    var st =
    {
        IsNewOrEdit: IsNewOrEdit,
        InvoiceNumber: InvoiceNumber,
        InvoiceDate: InvoiceDate,
        CustomerSupplierId: CustomerSupplierId,
        InvoiceType: 'Sale Invoice',
        TotalAmount: TotalAmount,
        DeliveryCharges: DeliveryCharges,
        DiscType: DiscType,
        DiscRate: DiscRate,
        DiscAmount: DiscAmount,
        NetAmount: NetAmount,

    }
    var detailData = JSON.stringify(getAllData());
    $.ajax({
        type: "POST",
        url: "/Admin/Invoice/InsertUpdateSaleInvoice/",
        data: { 'user': st, 'detail': detailData },

        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save");
            $('#btn_Save').prop('disabled', false);
            if (data.status) {
                toastr.success("Invoice Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                window.location.href = "/admin/invoice/saleinvoiceindex";

            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })
})

$("#Rate").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 9, 2);
});
$("#DiscRate").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 9, 2);
});
$("#Qty").on('input keypress', function (event) {
    NumberPostive(event, this, 5);
});