$(document).ready(function () {
    $('.tree > ul').attr('role', 'tree').find('ul').attr('role', 'group');
    $('.tree').find('li:has(ul)').addClass('parent_li').attr('role', 'treeitem').find(' > span').attr('title', 'Collapse this branch').on('click', function (e) {
        var children = $(this).parent('li.parent_li').find(' > ul > li');
        if (children.is(':visible')) {
            children.hide('fast');
            $(this).attr('title', 'Expand this Attribute').find(' > i').removeClass().addClass('fa fa-lg fa-plus-circle');
        } else {
            children.show('fast');
            $(this).attr('title', 'Collapse this Attribute').find(' > i').removeClass().addClass('fa fa-lg fa-minus-circle');
        }
        e.stopPropagation();
    });

})
$(document).on('change', '.moduleCheckbox', function () {
    var check = $(this).is(':checked');
    if (check == true) {
        $(this).parents('.AttributesList').find('input[type=checkbox]').prop('checked', true);
    }
    else {
        $(this).parents('.AttributesList').find('input[type=checkbox]').prop('checked', false);
    }
})
$(document).on('change', '.actionCheckbox', function () {
    var checkCampus = $(this).is(':checked');
    if (checkCampus == true) {
        $(this).parents('.AttributesDetailList').find('input[type=checkbox]').prop('checked', true);
    }
    else {
        $(this).parents('.AttributesDetailList').find('input[type=checkbox]').prop('checked', false);
    }
    var checked = 0;
    $(this).parents('.AttributesList').find('.actionCheckbox').each(function () {
        var check = $(this).is(':checked');
        if (check == false) {
            checked += 1;
        }
    })
    if (checked > 0) {
        $(this).parents('.AttributesList').find('.moduleCheckbox').prop('checked', false);
    }
    else {
        $(this).parents('.AttributesList').find('.moduleCheckbox').prop('checked', true);
    }
})
$('.selectAllBtn').click(function () {
    $('.AllModules').find('input[type=checkbox]').prop('checked', true);
})
$('.clearAllBtn').click(function () {
    $('.AllModules').find('input[type=checkbox]').prop('checked', false);
})
$('#btn_Save').click(function () {
    
    var AttributeDetailData = [];
    var ProductAttributeDetailId = 0;
    $('.AttributesList').each(function () {
        var attributeDetailId = $(this).find('.AttributeId').val();
        var attributeId = $(this).find('.AttributeDetailId').val();
        mainModuleId = attributeId;
        var attributeDetailIdCheck = $(this).find('.ActionIdCheckbox').is(':checked');
        var alldata = {
            'AttributeId': attributeDetailId,
            'AttributeDetailId': attributeId,
        }
        if (attributeDetailIdCheck == true) {
            data.push(alldata);
        }
    });
       
    var allData = JSON.stringify(AttributeDetailData);
    $.ajax({
        type: "POST",
        url: "/Admin/Action/InsertUpdateProduct/",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ 'attributeId': ProductAttributeDetailId, 'AttributeDetailData': allData }),
        success: function (data) {
            if (data.status == true) {

                toastr.success("Module Assigned  Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                location.reload();
            }
            else {
                alert(data.message);
            }
        }
    })
})
$(document).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();
    }
});