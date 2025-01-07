$(document).ready(function () {
    $('.tree > ul').attr('role', 'tree').find('ul').attr('role', 'group');
    $('.tree').find('li:has(ul)').addClass('parent_li').attr('role', 'treeitem').find(' > span').attr('title', 'Collapse this branch').on('click', function (e) {
        var children = $(this).parent('li.parent_li').find(' > ul > li');
        if (children.is(':visible')) {
            children.hide('fast');
            $(this).attr('title', 'Expand this branch').find(' > i').removeClass().addClass('fa fa-lg fa-plus-circle');
        } else {
            children.show('fast');
            $(this).attr('title', 'Collapse this branch').find(' > i').removeClass().addClass('fa fa-lg fa-minus-circle');
        }
        e.stopPropagation();
    });
});
$(document).on('change', '.moduleCheckbox', function () {
    var check = $(this).is(':checked');
    if (check == true) {
        $(this).parents('.ModuleList').find('input[type=checkbox]').prop('checked', true);
    }
    else {
        $(this).parents('.ModuleList').find('input[type=checkbox]').prop('checked', false);
    }
})
$(document).on('change', '.actionCheckbox', function () {
    var checkCampus = $(this).is(':checked');
    if (checkCampus == true) {
        $(this).parents('.ActionList').find('input[type=checkbox]').prop('checked', true);
    }
    else {
        $(this).parents('.ActionList').find('input[type=checkbox]').prop('checked', false);
    }
    var checked = 0;
    $(this).parents('.ModuleList').find('.actionCheckbox').each(function () {
        var check = $(this).is(':checked');
        if (check == false) {
            checked += 1;
        }
    })
    if (checked > 0) {
        $(this).parents('.ModuleList').find('.moduleCheckbox').prop('checked', false);
    }
    else {
        $(this).parents('.ModuleList').find('.moduleCheckbox').prop('checked', true);
    }
})
$('.selectAllBtn').click(function () {
    $('.AllModules').find('input[type=checkbox]').prop('checked', true);
})
$('.clearAllBtn').click(function () {
    $('.AllModules').find('input[type=checkbox]').prop('checked', false);
})

$('.refreshBtn').click(function () {
    LoadAttributesDiv();
})

$(document).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();
    }
});