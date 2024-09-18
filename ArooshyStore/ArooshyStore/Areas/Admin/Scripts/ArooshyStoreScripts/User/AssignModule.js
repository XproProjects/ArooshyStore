$(function () {
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

})
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
$('.saveRoleBtn').click(function () {
    //$('.saveRoleBtn').attr('disabled', 'disabled');
    //$('.saveRoleBtn').html("<i class='fa fa-refresh fa-spin'></i> &nbsp; Processing...");
    var moduleData = [];
    var mainUserId = 0;
    $('.ModuleList').each(function () {
        var userId = $(this).find('.UserId').val();
        mainUserId = userId;
        var moduleId = $(this).find('.ModuleId').val();
        var moduleCheckbox = $(this).find('.moduleCheckbox').is(':checked');

        var actionData = new Array();
        var checkActionList = 0;
        //Loop on all actions of module
        $(this).find('.ActionList').each(function () {
            //this will be incremented if module has any action. Otherwise it will remain zero.
            checkActionList++;
            ///
            var actionId = $(this).find('.ActionId').val();
            var actionCheckbox = $(this).find('.actionCheckbox').is(':checked');
            var isChecked = '';
            if (actionCheckbox == true) {
                isChecked = 'yes';
            }
            else {
                isChecked = 'no';
            }
            //if action checkbox is checked then add it into array
            var action =
            {
                ActionId: actionId,
                IsChecked: isChecked
            }
            actionData.push(action);
        });
        //This condition will check if module has any action or not.
        if (checkActionList > 0) {
            //If any action of module is checked
            var alldata = {
                UserId: userId,
                ModuleId: moduleId,
                ActionList: actionData
            }
            moduleData.push(alldata);
        }
        else {
            var emptyArray = new Array();
            var action =
            {
                ActionId: 0,
                IsChecked: 'no'
            }
            emptyArray.push(action);
            var alldata = {
                UserId: userId,
                ModuleId: moduleId,
                ActionList: emptyArray
            }
            moduleData.push(alldata);
        }

    });
    var allData2 = JSON.stringify(moduleData);
    $.ajax({
        type: "POST",
        url: "/Admin/User/InsertUpdateAssignModule/",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ 'userId': mainUserId, 'assignType': 'Module', 'data': allData2 }),
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