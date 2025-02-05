$('input[type=radio][name=SelectItemType]').change(function () {
    var radioValue = $("input[name='SelectItemType']:checked").val();
    if (radioValue == 'Existing Item') {
        $('.BarcodeDiv').removeAttr("hidden");
        $('.NewItemDiv').attr("hidden", "hidden");
        $('#txtBarcode').focus();
    }
    else if (radioValue == 'New Item') {
        $('.BarcodeDiv').attr("hidden", "hidden");
        $('.NewItemDiv').removeAttr("hidden");
    }
    else {
        $('.BarcodeDiv').removeAttr("hidden");
        $('.NewItemDiv').attr("hidden", "hidden");
    }
    $('#btnCancelDetail').hide();
    $('#btnAddDetail').find('.btnLineSpan').html('Add');
    ResetLineValues();
});

function ResetLineValues() {
    $('#HiddenOfferDetailId').val('0');
    $('#txtBarcode').val('');
    $('#ProductId').val(null).trigger('change.select2');
    $('#ProductAttributeDetailBarcodeId').val(null).trigger('change.select2');
    $('#Rate').val('0');
    $('#Qty').val('0');
    $('#LineDiscType').val('Rs.');
    $('#LineDiscRate').val('0');
    $('#LineDiscAmount').val('0');
    $('#LineNetAmount').html('0.00');
    $('#ProductIdDiv').closest('#ProductIdDiv').find('.select2-choice').css('border-color', '#CCCCCC');
    $('#ProductIdDiv').closest('#ProductIdDiv').find('.select2-choice').css('color', 'black');

    $('#ProductAttributeDetailBarcodeIdDiv').closest('#ProductIdDiv').find('.select2-choice').css('border-color', '#CCCCCC');
    $('#ProductAttributeDetailBarcodeIdDiv').closest('#ProductIdDiv').find('.select2-choice').css('color', 'black');

    $('#Rate').css('border-color', '#bdbdbd');
    $('#Rate').css('background-color', '#fff');
    $('#Qty').css('border-color', '#bdbdbd');
    $('#Qty').css('background-color', '#fff');
    $('#txtBarcode').css('border-color', '#bdbdbd');
    $('#txtBarcode').css('background-color', '#fff');
}
$('input[type=radio][name=CashOrCredit]').change(function () {
    GetCustomer();
})
function GetCustomer() {
    var radioValue = $("input[name='CashOrCredit']:checked").val();
    if (radioValue == 'Cash') {
        $.ajax({
            type: "POST",
            url: '/Admin/Invoice/GetCashCustomer/',
            dataType: 'json',
            success: function (response) {
                if (response.data.CustomerSupplierId > 0) {
                    if ($('#CustomerSupplierId').find("option[value='" + response.data.CustomerSupplierId + "']").length) {
                        $('#CustomerSupplierId').val(response.data.CustomerSupplierId).trigger('change.select2');
                    } else {
                        // Create a DOM Option and pre-select by default
                        var newOption = new Option(response.data.CustomerName, response.data.CustomerSupplierId, true, true);
                        // Append it to the select
                        $('#CustomerSupplierId').append(newOption).trigger('change.select2');
                    }
                    $('#CustomerSupplierId').prop("disabled", true);
                    $(".btnAddNewCustomer").attr("hidden", "hidden");
                }
                else {
                    $('#CustomerSupplierId').val(null).trigger('change.select2');
                    $('#CustomerSupplierId').prop("disabled", false);
                    $(".btnAddNewCustomer").removeAttr("hidden");
                }
                $('#DeliveryCharges').val('0');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("Error fetching details: " + textStatus + " " + errorThrown);
                alert("Failed to fetch details: " + errorThrown);
            }
        });
    }
    else {
        $('#CustomerSupplierId').prop("disabled", false);
        $(".btnAddNewCustomer").removeAttr("hidden");
        if ($("#HiddenCustomerSupplierId").val() > 0) {
            if ($('#CustomerSupplierId').find("option[value='" + $("#HiddenCustomerSupplierId").val() + "']").length) {
                $('#CustomerSupplierId').val($("#HiddenCustomerSupplierId").val()).trigger('change.select2');
            } else {
                // Create a DOM Option and pre-select by default
                var newOption = new Option($("#HiddenCustomerSupplierName").val(), $("#HiddenCustomerSupplierId").val(), true, true);
                // Append it to the select
                $('#CustomerSupplierId').append(newOption).trigger('change.select2');
            }
        }
        else {
            $('#CustomerSupplierId').val(null).trigger('change.select2');
            $('#DeliveryCharges').val('0');
        }
    }
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
$(function () {
    $('.nav-menu li a[href="/admin/invoice/saleinvoiceindex/?from=all"]').parent("li").addClass('active');
    $('.nav-menu li a[href="/admin/invoice/saleinvoiceindex/?from=all"]').parent("li").parent("ul").css('display', "block");
    $('.nav-menu li a[href="/admin/invoice/saleinvoiceindex/?from=all"]').parent("li").parent("ul").parent("li").removeClass("open").addClass("open");
    $('.nav-menu li a[href="/admin/invoice/saleinvoiceindex/?from=all"]').parent("li").parent("ul").parent("li").find("a").find("b").find("em").removeClass("fa-angle-down").removeClass("fa-angle-up").addClass("fa-angle-up");
    $('#txtBarcode').focus();
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
            url: '/Admin/Combolist/GetProductAttributesFromBarcodeTableOptionList/',
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
                    type: "customer"
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
        placeholder: "-- Select Customer --",
        minimumInputLength: 0,
        dropdownParent: $(".mySelect"),
        allowClear: true,
    });


    GetCustomer();
    if ($('#IsNewOrEdit').val() === 'Update' || $('#IsNewOrEdit').val() === 'Exchange') {
        getInvoiceDetail($('#InvoiceNumber').val());
    } else {
        GetInvoiceNo();
    }

    //var $checkoutForm = $('#popupForm').validate({
    //    rules: {
    //        CustomerSupplierId: {
    //            required: true
    //        },
    //        InvoiceDate: {
    //            required: true
    //        },
    //    },
    //    messages: {
    //        CustomerSupplierId: {
    //            required: 'Customer is required.'
    //        },
    //        InvoiceDate: {
    //            required: 'Invoice Date is required.'
    //        },
    //    },
    //    errorPlacement: function (error, element) {
    //        error.insertAfter(element.parent());
    //        if (element.val() == '' || element.val() == null) {
    //            element.parents('td').siblings('td').find('.btn').css("margin-top", "-8.5px");
    //        }
    //    }
    //});
})

$('#ProductId').change(function () {
    $('#ProductIdDiv').closest('#ProductIdDiv').find('.select2-selection').css('border-color', '#ddd');
    $('#ProductIdDiv').closest('#ProductIdDiv').find('.select2-selection').css('background-color', '#fff');

    $('#Rate').css('border-color', '#bdbdbd');
    $('#Rate').css('background-color', '#fff');
    $('#Qty').css('border-color', '#bdbdbd');
    $('#Qty').css('background-color', '#fff');

    var productId = $(this).val();
    if (productId != 0 && productId != '' && productId != undefined && productId != null) {
        $.ajax({
            "url": "/Admin/Product/GetProductSalePrice",
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                $('#HiddenOfferDetailId').val(response.offerDetailId);
                $('#Rate').val(response.salePrice);
                $('#Qty').val('1');
                $('#LineDiscType').val(response.discountType);
                $('#LineDiscRate').val(response.discountRate);
                getLineTotals();
            },
            error: function () {
                alert('Error fetching product details.');
            }
        });
    }
    else {
        ResetLineValues();
    }
});
$('#ProductAttributeDetailBarcodeId').change(function () {
    $('#ProductAttributeDetailBarcodeIdDiv').closest('#ProductAttributeDetailBarcodeIdDiv').find('.select2-selection').css('border-color', '#ddd');
    $('#ProductAttributeDetailBarcodeIdDiv').closest('#ProductAttributeDetailBarcodeIdDiv').find('.select2-selection').css('background-color', '#fff');
});

$('#CustomerSupplierId').change(function () {
    var customerSupplierId = $(this).val();
    if (customerSupplierId != 0 && customerSupplierId != '' && customerSupplierId != undefined && customerSupplierId != null) {
        $.ajax({
            type: "POST",
            url: "/Admin/Invoice/GetDeliveryCharges/",
            dataType: 'json',
            data: { 'customerId': customerSupplierId },
            success: function (response) {
                $('#DeliveryCharges').val(response);
                GetMainTotals();
            }
        })
    } else {
        $('#DeliveryCharges').val('0');
    }
});

$('#btnAddDetail').click(function () {
    var check = 0;
    var radioValue = $("input[name='SelectItemType']:checked").val();
    if (radioValue == 'Existing Item') {
        var Barcode = $('#txtBarcode').val();
        if (Barcode == '' || Barcode == null) {
            $('#txtBarcode').css('border-color', '#A90329');
            $('#txtBarcode').css('background-color', '#FFF0F0');
            return false;
        }
        GetProductFromBarcode();
    }
    else if (radioValue == 'New Item') {
        var ProductId = $('#ProductId').val();
        var ProductAttributeDetailBarcodeId = $('#ProductAttributeDetailBarcodeId').val();
        var OfferDetailId = $('#HiddenOfferDetailId').val();
        var Rate = $('#Rate').val();
        var Qty = $('#Qty').val();
        var LineDiscType = $('#LineDiscType').val();
        var LineDiscRate = $('#LineDiscRate').val();
        var LineDiscAmount = $('#LineDiscAmount').val();

        if (ProductId == '' || ProductId == null || ProductId == 0) {
            $('#ProductIdDiv').closest('#ProductIdDiv').find('.select2-selection').css('border-color', '#A90329');
            $('#ProductIdDiv').closest('#ProductIdDiv').find('.select2-selection').css('background-color', '#FFF0F0');
            check = 1;
        }

        if (ProductAttributeDetailBarcodeId == '' || ProductAttributeDetailBarcodeId == null || ProductAttributeDetailBarcodeId == 0) {
            $('#ProductAttributeDetailBarcodeIdDiv').closest('#ProductAttributeDetailBarcodeIdDiv').find('.select2-selection').css('border-color', '#A90329');
            $('#ProductAttributeDetailBarcodeIdDiv').closest('#ProductAttributeDetailBarcodeIdDiv').find('.select2-selection').css('background-color', '#FFF0F0');
            check = 1;
        }

        if (Rate == '' || Rate == null || Rate == 0) {
            $('#Rate').css('border-color', '#A90329');
            $('#Rate').css('background-color', '#FFF0F0');
            check = 1;
        }

        if (Qty == '' || Qty == null || Qty == 0) {
            $('#Qty').css('border-color', '#A90329');
            $('#Qty').css('background-color', '#FFF0F0');
            check = 1;
        }

        if (check > 0) {
            return false;
        }

        if (LineDiscType == '' || LineDiscType == null) {
            LineDiscType = 'Rs.';
        }

        if (LineDiscRate == '' || LineDiscRate == null || isNaN(LineDiscRate)) {
            LineDiscRate = 0;
        }

        if (LineDiscAmount == '' || LineDiscAmount == null || isNaN(LineDiscAmount)) {
            LineDiscAmount = 0;
        }

        var LineNetAmount = parseFloat(ReplaceCommas($('#LineNetAmount').html()));
        if (LineNetAmount == '' || LineNetAmount == null || isNaN(LineNetAmount)) {
            LineNetAmount = 0;
        }
        var ProductName = $('#ProductId option:selected').text();
        var AttributeName = $('#ProductAttributeDetailBarcodeId option:selected').text();



        var vtable = $('#detailTable').DataTable();
        if ($(this).find('.btnLineSpan').html().trim() == 'Add') {
            AddRowToTable("Add", 0, vtable, ProductId, ProductName, ProductAttributeDetailBarcodeId, AttributeName,
                OfferDetailId, Rate, Qty, LineDiscType, LineDiscRate, LineDiscAmount, LineNetAmount);

        }
        else {
            // Edit existing row
            var tr = $('#HiddenTr').val();

            $('#btnAddDetail').find('.btnLineSpan').html('Add');
            $('#btnCancelDetail').hide();
            $('#btnCancelDetail').attr("hidden", "hidden");

            AddRowToTable("Edit", tr, vtable, ProductId, ProductName, ProductAttributeDetailBarcodeId, AttributeName,
                OfferDetailId, Rate, Qty, LineDiscType, LineDiscRate, LineDiscAmount, LineNetAmount);
        }
    }

    $('input[type=radio][name=SelectItemType]').prop("disabled", false);
    $('input[type=radio][value="Existing Item"]').prop("checked", true);
    $('.NewItemDiv').attr("hidden", "hidden");
    $('.BarcodeDiv').removeAttr("hidden");
    $('#txtBarcode').focus();

    getSerialNo();
    ResetLineValues();
    GetMainTotals();
    return false;
});

function AddRowToTable(insertType, tr, vtable, productId, productName, productAttributeDetailBarcodeId, productAttributeDetailBarcodeName,
    offerDetailId, rate, qty, discType, discRate, discAmount, netPrice) {

    if (insertType == "Add") {
        vtable.row.add([
            "<a href='javascript:void(0);' class='btn btn-primary editRowBtn' title='Edit' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-edit'></i></a>",
            "<a href='javascript:void(0);' class='btn btn-danger deleteRowBtn' title='Delete' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-times-circle'></i></a>",
            "<center><input type='hidden' class='SR01' value='' /><input type='hidden' class='OfferDetailId' value=" + offerDetailId + " /><span class='SrNo'></span></center>",
            "<span><input type='hidden' class='ProductId' value=" + productId + " />" + productName + "</span>",
            "<span><input type='hidden' class='ProductAttributeDetailBarcodeId' value=" + productAttributeDetailBarcodeId + " />" + productAttributeDetailBarcodeName + "</span>",
            "<span class='pull-right'><input type='hidden' class='SalesPrice' value=" + rate + " />" + ReplaceNumberWithCommas(parseFloat(rate).toFixed(2)) + "</span>",
            "<span><input type='hidden' class='Quantity' value=" + qty + " />" + qty + "</span>",
            "<span><input type='hidden' class='DiscountType' value=" + discType + " />" + discType + "</span>",
            "<span class='pull-right'><input type='hidden' class='DiscountRate' value=" + discRate + " />" + ReplaceNumberWithCommas(parseFloat(discRate).toFixed(2)) + "</span>",
            "<span class='pull-right'><input type='hidden' class='DiscountAmount' value=" + discAmount + " />" + ReplaceNumberWithCommas(parseFloat(discAmount).toFixed(2)) + "</span>",
            "<span class='pull-right'><input type='hidden' class='NetAmount' value='" + netPrice + "' />" + ReplaceNumberWithCommas(parseFloat(netPrice).toFixed(2)) + "</span>",
        ]).draw(false);
    }
    else if (insertType == "Edit") {
        vtable.row(tr).data([
            "<a href='javascript:void(0);' class='btn btn-primary editRowBtn' title='Edit' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-edit'></i></a>",
            "<a href='javascript:void(0);' class='btn btn-danger deleteRowBtn' title='Delete' style='font-size: 13.5px;padding: 6px;padding-left:10px;padding-right:10px'><i class='fas fa-times-circle'></i></a>",
            "<center><input type='hidden' class='SR01' value='' /><input type='hidden' class='OfferDetailId' value=" + offerDetailId + " /><span class='SrNo'></span></center>",
            "<span><input type='hidden' class='ProductId' value=" + productId + " />" + productName + "</span>",
            "<span><input type='hidden' class='ProductAttributeDetailBarcodeId' value=" + productAttributeDetailBarcodeId + " />" + productAttributeDetailBarcodeName + "</span>",
            "<span class='pull-right'><input type='hidden' class='SalesPrice' value=" + rate + " />" + ReplaceNumberWithCommas(parseFloat(rate).toFixed(2)) + "</span>",
            "<span><input type='hidden' class='Quantity' value=" + qty + " />" + qty + "</span>",
            "<span><input type='hidden' class='DiscountType' value=" + discType + " />" + discType + "</span>",
            "<span class='pull-right'><input type='hidden' class='DiscountRate' value=" + discRate + " />" + ReplaceNumberWithCommas(parseFloat(discRate).toFixed(2)) + "</span>",
            "<span class='pull-right'><input type='hidden' class='DiscountAmount' value=" + discAmount + " />" + ReplaceNumberWithCommas(parseFloat(discAmount).toFixed(2)) + "</span>",
            "<span class='pull-right'><input type='hidden' class='NetAmount' value='" + netPrice + "' />" + ReplaceNumberWithCommas(parseFloat(netPrice).toFixed(2)) + "</span>",
        ]).draw(false);
    }

    $('input[type=radio][name=SelectItemType]').prop("disabled", false);
    //$('input[type=radio][value="Existing Item"]').prop("checked", true);
}

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
    $('#btnCancelDetail').attr("hidden", "hidden");
    $('#btnAddDetail').find('.btnLineSpan').html('Add');

    $('input[type=radio][name=SelectItemType]').prop("disabled", false);
    $('input[type=radio][value="Existing Item"]').prop("checked", true);
    $('.NewItemDiv').attr("hidden", "hidden");
    $('.BarcodeDiv').removeAttr("hidden");
    $('#txtBarcode').focus();

    ResetLineValues();
});



// Edit button functionality
$(document).on('click', '.editRowBtn', function () {
    var vtable = $('#detailTable').DataTable();
    var rowIndex = vtable.row($(this).parents('tr')).index();
    $('#HiddenTr').val(rowIndex);

    $('input[type=radio][name=SelectItemType]').prop("disabled", true);
    $('input[type=radio][value="New Item"]').prop("checked", true);
    $('.BarcodeDiv').attr("hidden", "hidden");
    $('.NewItemDiv').removeAttr("hidden");

    var data = vtable.row($(this).parents('tr')).data();
    var OfferDetailId = $(data[2]).find('input.OfferDetailId').val();
    var ProductId = $(data[3]).find('input.ProductId').val();
    var ProductAttributeDetailBarcodeId = $(data[4]).find('input.ProductAttributeDetailBarcodeId').val();
    var SalesPrice = $(data[5]).find('input.SalesPrice').val();
    var Quantity = $(data[6]).find('input.Quantity').val();
    var DiscountType = $(data[7]).find('input.DiscountType').val();
    var DiscountRate = $(data[8]).find('input.DiscountRate').val();
    var DiscountAmount = $(data[9]).find('input.DiscountAmount').val();
    var NetAmount = $(data[10]).find('input.NetAmount').val();

    updateSelect2Option('#ProductId', ProductId, data[3]);
    updateSelect2Option('#ProductAttributeDetailBarcodeId', ProductAttributeDetailBarcodeId, data[4]);

    // Populate the fields
    $('#HiddenOfferDetailId').val(OfferDetailId);
    $('#Rate').val(SalesPrice);
    $('#Qty').val(Quantity);
    $('#LineDiscType').val(DiscountType);
    $('#LineDiscRate').val(DiscountRate);
    $('#LineDiscAmount').val(DiscountAmount);
    $('#LineNetAmount').html(ReplaceNumberWithCommas(NetAmount));
    $('#btnAddDetail').find('.btnLineSpan').html('Update');
    $('#btnCancelDetail').show();
    $('#btnCancelDetail').removeAttr("hidden");
});

function updateSelect2Option(selector, value, dataItem) {
    if ($(selector).find("option[value='" + value + "']").length) {
        $(selector).val(value).trigger('change.select2');
    } else {
        var displayText = dataItem.split('/>')[1].split('</span>')[0].trim();
        var newOption = new Option(displayText, value, true, true);
        $(selector).append(newOption).trigger('change.select2');
    }
}



// Delete button functionality
$(document).on('click', '.deleteRowBtn', function () {
    var vtable = $('#detailTable').DataTable();
    vtable.row($(this).parents('tr')).remove().draw(false);
    getSerialNo();
    $('#btnCancelDetail').hide();
    $('#btnCancelDetail').attr("hidden", "hidden");
    ResetLineValues();
    GetMainTotals();
});

$('#txtBarcode').on('keypress', function (event) {
    if (event.keyCode == 13) {
        var Barcode = $('#txtBarcode').val();
        if (Barcode != '') {
            GetProductFromBarcode();
        }
    }
});

function GetProductFromBarcode() {
    $('#txtBarcode').css('border-color', '#bdbdbd');
    $('#txtBarcode').css('background-color', '#fff');
    $.ajax({
        type: 'POST',
        url: '/Admin/Product/GetProductDetailsByBarcode',
        data: { 'barcode': $('#txtBarcode').val() },
        success: function (response) {
            if (response.productId > 0) {
                var vtable = $('#detailTable').DataTable();
                AddRowToTable("Add", 0, vtable, response.productId, response.productName, response.productAttributeDetailBarcodeId, response.attributeName,
                    response.offerDetailId, response.salePrice, response.quantity, response.discountType, response.discountRate, response.discountAmount, response.netAmount);
                $('#txtBarcode').val('');
                $('#txtBarcode').focus();
                getSerialNo();
                ResetLineValues();
                GetMainTotals();

            } else {
                $('#txtBarcode').val('');
                $('#txtBarcode').focus();
                var errorMessage = "Error! Product not found with this barcode.";
                toastr.error(errorMessage, "Error", { timeOut: 3000, closeButton: true });
            }
        },
        error: function () {
            alert('Error fetching product details.');
        }
    });

}

$('#txtBarcode').keyup(function () {
    $('#txtBarcode').css('border-color', '#bdbdbd');
    $('#txtBarcode').css('background-color', '#fff');
})
$('#Rate').keyup(function () {
    $('#Rate').css('border-color', '#bdbdbd');
    $('#Rate').css('background-color', '#fff');
    getLineTotals();
})
$('#Qty').keyup(function () {
    $('#Qty').css('border-color', '#bdbdbd');
    $('#Qty').css('background-color', '#fff');
    getLineTotals();
});
$('#LineDiscType').change(function () {
    getLineTotals();
})
$('#LineDiscRate').keyup(function () {
    getLineTotals();
})
function getLineTotals() {
    var rate = $('#Rate').val();
    var Quantity = $('#Qty').val();
    var discountType = $('#LineDiscType').val();
    var discountRate = $('#LineDiscRate').val();
    if (rate == '' || rate == null || rate == undefined || isNaN(rate)) {
        rate = 0;
    }
    if (Quantity == '' || Quantity == null || Quantity == undefined || isNaN(Quantity)) {
        Quantity = 0;
    }
    if (discountType == '' || discountType == null) {
        discountType = "Rs.";
    }
    if (discountRate == '' || discountRate == null || discountRate == undefined || isNaN(discountRate)) {
        discountRate = 0;
    }
    var netDiscount = 0;
    if (discountType.trim() == '%') {
        netDiscount = parseFloat(discountRate) / 100 * parseFloat(rate);
        netDiscount = parseFloat(netDiscount).toFixed(2);
    }
    else if (discountType.trim() == 'Rs.') {
        netDiscount = parseFloat(discountRate).toFixed(2);
    }
    else {
        netDiscount = 0;
    }
    $('#LineDiscAmount').val(netDiscount);

    var netPrice = parseFloat(rate) - parseFloat(netDiscount);
    netPrice = parseFloat(netPrice) * parseFloat(Quantity);
    netPrice = parseFloat(netPrice).toFixed(2);

    $('#LineNetAmount').html(ReplaceNumberWithCommas(netPrice));
}

$('#DeliveryCharges').on('change keyup', function () {
    GetMainTotals();
});
$('#DiscType, #DiscRate').on('change keyup', function () {
    GetMainTotals();
});
function GetMainTotals() {
    var vtable = $('#detailTable').DataTable();
    var totalAmount = 0;
    debugger;
    vtable.rows().every(function () {
        var data = this.data();
        var netPrice = parseFloat($(data[10]).find('input.NetAmount').val());
        if (netPrice != '' && netPrice != null && netPrice != undefined && !isNaN(netPrice)) {
            totalAmount += netPrice;
        }
    });

    $('#TotalAmount').text(ReplaceNumberWithCommas(parseFloat(totalAmount).toFixed(2)));

    var discType = $('#DiscType').val();
    var discRate = $('#DiscRate').val();

    if (discType == '' || discType == null) {
        discType = "Rs.";
    }
    if (discRate == '' || discRate == null || discRate == undefined || isNaN(discRate)) {
        discRate = 0;
    }
    var netDiscount = 0;
    if (discType.trim() == '%') {
        netDiscount = parseFloat(discRate) / 100 * parseFloat(totalAmount);
        netDiscount = parseFloat(netDiscount).toFixed(2);
    }
    else if (discType.trim() == 'Rs.') {
        netDiscount = parseFloat(discRate).toFixed(2);
    }
    else {
        netDiscount = 0;
    }

    // Populate the DiscAmount field
    $('#DiscAmount').val(netDiscount);

    var amountAfterDiscount = (parseFloat(totalAmount) - parseFloat(netDiscount)).toFixed(2);
    var deliveryCharges = $('#DeliveryCharges').val();
    if (deliveryCharges == '' || deliveryCharges == null || deliveryCharges == undefined || isNaN(deliveryCharges)) {
        deliveryCharges = 0;
    }
    var netAmount = (parseFloat(amountAfterDiscount) + parseFloat(deliveryCharges)).toFixed(2);

    // Update NetAmount field
    $('#NetAmount').text(ReplaceNumberWithCommas(netAmount));
}


$(document).on('change', 'select', function () {
    $(this).parents('td').siblings('td').find('.btn').css("margin-top", "14px");
    $(this).parents('label').siblings('em').remove();
})

function getInvoiceDetail(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/Invoice/GetInvoiceDetailsList/',
        data: { 'id': id },
        dataType: 'json',
        success: function (response) {
            var vtable = $('#detailTable').DataTable();
            vtable.clear().draw();
            if (response && response.data && response.data.length > 0) {
                for (var key in response.data) {
                    AddRowToTable("Add", 0, vtable, response.data[key].ProductId, response.data[key].ProductName, response.data[key].ProductAttributeDetailBarcodeId, response.data[key].AttributeName,
                        response.data[key].OfferDetailId, response.data[key].Rate, response.data[key].Qty, response.data[key].DiscType, response.data[key].DiscRate, response.data[key].DiscAmount, response.data[key].NetAmount);
                }
                getSerialNo();
                GetMainTotals();
            } else {
                console.error("No valid data found.");
                alert("No data received for the given invoice ID.");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error fetching invoice details: " + textStatus + " " + errorThrown);
            alert("Failed to fetch invoice details: " + errorThrown);
        }
    });

}

function CheckAllData() {
    var check = 0;
    var vtable = $('#detailTable').DataTable();
    vtable.rows().every(function () {
        check++;
    });

    return check;
}
function getAllData() {
    var arrayData = [];
    var vtable = $('#detailTable').DataTable();
    vtable.rows().every(function () {
        var data = this.data();

        //var getInt = (value) => {
        //    let intVal = parseInt(value, 10);
        //    return isNaN(intVal) ? 0 : intVal;
        //};

        //var getFloat = (value) => {
        //    let floatVal = parseFloat(value);
        //    return isNaN(floatVal) ? 0 : floatVal;
        //};

        //var warehouseId = getInt($(data[0]).find('input.WarehouseId').val());

        var OfferDetailId = $(data[2]).find('input.OfferDetailId').val();
        var ProductId = $(data[3]).find('input.ProductId').val();
        var ProductAttributeDetailBarcodeId = $(data[4]).find('input.ProductAttributeDetailBarcodeId').val();
        var SalesPrice = $(data[5]).find('input.SalesPrice').val();
        var Quantity = $(data[6]).find('input.Quantity').val();
        var DiscountType = $(data[7]).find('input.DiscountType').val();
        var DiscountRate = $(data[8]).find('input.DiscountRate').val();
        var DiscountAmount = $(data[9]).find('input.DiscountAmount').val();
        var NetAmount = $(data[10]).find('input.NetAmount').val();

        var TotalAmount = parseFloat(SalesPrice) * parseFloat(Quantity);
        var rowData = {
            'WarehouseId': 0,
            'ProductId': ProductId,
            'ProductAttributeDetailBarcodeId': ProductAttributeDetailBarcodeId,
            'Qty': Quantity,
            'TotalAmount': TotalAmount,
            'Rate': SalesPrice,
            'OfferDetailId': OfferDetailId,
            'DiscType': DiscountType,
            'DiscRate': DiscountRate,
            'DiscAmount': DiscountAmount,
            'NetAmount': NetAmount,
        };

        arrayData.push(rowData);
    });

    console.log(arrayData);
    return arrayData;
}

$('#CustomerSupplierId').change(function () {
    $('#CustomerSupplierIdDiv').closest('#CustomerSupplierIdDiv').find('.select2-selection').css('border-color', '#ddd');
    $('#CustomerSupplierIdDiv').closest('#CustomerSupplierIdDiv').find('.select2-selection').css('background-color', '#fff');
});

$(document).on('keyup change', '#InvoiceDate', function () {
    $('#InvoiceDate').css('border-color', '#bdbdbd');
    $('#InvoiceDate').css('background-color', '#fff');
})

$(document).off("click", "#btn_Save").on('click', "#btn_Save", function (e) {
    e.preventDefault();
    SaveForm("exit");
})
$(document).off("click", "#btn_SaveAndPrint").on('click', "#btn_SaveAndPrint", function (e) {
    e.preventDefault();
    SaveForm("print");
})

function SaveForm(type) {
    var InvoiceDate = $('#InvoiceDate').val();
    var CustomerSupplierId = $('#CustomerSupplierId').val();

    var check = 0;
    if (InvoiceDate == '' || InvoiceDate == null) {
        $('#InvoiceDate').css('border-color', '#A90329');
        $('#InvoiceDate').css('background-color', '#FFF0F0');
        check = 1;
    }

    if (CustomerSupplierId == '' || CustomerSupplierId == null || CustomerSupplierId == 0) {
        $('#CustomerSupplierIdDiv').closest('#CustomerSupplierIdDiv').find('.select2-selection').css('border-color', '#A90329');
        $('#CustomerSupplierIdDiv').closest('#CustomerSupplierIdDiv').find('.select2-selection').css('background-color', '#FFF0F0');
        check = 1;
    }
    if (check > 0) {
        $("html, body").animate({
            scrollTop: 0
        }, 300);
        return false;
    }

    if (CheckAllData() == 0) {
        toastr.error("Error! Please add Products Detail.", "Error", { timeOut: 3000, "closeButton": true });
        return false;
    }

    $('#btn_Save').attr('disabled', 'disabled');
    $('#btn_SaveAndPrint').attr('disabled', 'disabled');
    $('#btn_Save').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    $('#btn_SaveAndPrint').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");

    var CashOrCredit = $("input[name='CashOrCredit']:checked").val();
    var IsNewOrEdit = $('#IsNewOrEdit').val();
    var InvoiceNumber = $('#InvoiceNumber').val();
    var TotalAmount = parseFloat(ReplaceCommas($('#TotalAmount').html()));
    var DiscType = $('#DiscType').val();
    var DiscRate = $('#DiscRate').val();
    var DiscAmount = $('#DiscAmount').val();
    var DeliveryCharges = $('#DeliveryCharges').val();
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
        CashOrCredit: CashOrCredit,
        AdminOrClient: "Admin"
    }
    var detailData = JSON.stringify(getAllData());
    $.ajax({
        type: "POST",
        url: "/Admin/Invoice/InsertUpdateInvoice/",
        data: { 'user': st, 'detail': detailData },

        dataType: 'json',
        success: function (data) {
            $('#btn_Save').html("Save & Exit");
            $('#btn_SaveAndPrint').html("Save & Print");
            $('#btn_Save').prop('disabled', false);
            $('#btn_SaveAndPrint').prop('disabled', false);
            if (data.status) {
                if (type == "print") {
                    window.location.href = "/admin/invoice/PrintInvoice/?id="+data.Id+"";
                }
                else {
                    toastr.success("Sale Invoice Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                    setTimeout(function () {
                        window.location.href = "/admin/invoice/saleinvoiceindex/?from=all";
                    }, 2000);
                }
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
            }
        }
    })

}
$("#Rate").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 9, 2);
});
$("#LineDiscRate").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 9, 2);
});
$("#DiscRate").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 9, 2);
});
$("#DiscAmount").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 9, 2);
});
$("#DeliveryCharges").on('input keypress', function (event) {
    NumberPostiveNegativeWithDecimal(event, this, 5, 2);
});
$("#Qty").on('input keypress', function (event) {
    NumberPostive(event, this, 5);
});

//$(document).keypress(function (event) {
//    var keycode = (event.keyCode ? event.keyCode : event.which);
//    if (keycode == '13') {
//        event.preventDefault();
//    }
//});


//$(window).keypress(function (event) {
//    if (!(event.which == 115 && event.ctrlKey) && !(event.which == 19)) return true;
//    alert();
//    event.preventDefault();
//    return false;
//});

$(document).off("click", "#btn_New").on('click', "#btn_New", function (e) {
    window.open("/admin/invoice/insertupdatesaleinvoice/?id=0&type=new", '_blank');
})
$(document).keypress(function (event) {
    //Shift + S or Shift + s
    if (event.shiftKey && event.which == 68) {
        $("#btn_Save").click();
    }
});
$(document).keypress(function (event) {
    //Shift + S or Shift + s
    if (event.shiftKey && event.which == 83) {
        $("#btn_SaveAndPrint").click();
    }
});
$(document).keypress(function (event) {
    //Shift + N or Shift + n
    if (event.shiftKey && event.which == 78) {
        $("#btn_New").click();
    }
});