$(function () {
    //layouts.fixedNavigation('on');
    // $("body").addClass("nav-function-fixed");
    $("body").removeClass("nav-function-fixed").addClass("nav-function-fixed");
    if (location.pathname.toLowerCase() == '/' || location.pathname.toLowerCase() == '/admin/home/index') {
        $('.nav-menu li a[href="/admin/home/index"]').parent("li").addClass('active');
    }
    else if (location.pathname.toLowerCase() == '/' || location.pathname.toLowerCase() == '/admin/home/underdevelopment') {
        $('.nav-menu li a[href="/admin/home/index"]').removeClass('active');
    }
    else {
        $('.nav-menu li a[href="' + location.pathname.toLowerCase() + '"]').parent("li").addClass('active');
        $('.nav-menu li a[href="' + location.pathname.toLowerCase() + '"]').parent("li").parent("ul").parent("li").addClass('active').addClass('open');
        $('.nav-menu li a[href="' + location.pathname.toLowerCase() + '"]').parent("li").parent("ul").parent("li").find('.collapse-sign').find('em').removeClass('fa-angle-down').addClass('fa-angle-up');
    }
    //Jquery UI Datepicker default options for all datepickers
    $.datepicker.setDefaults({
        dateFormat: "dd-mm-yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-30:+20", //Min year is current year minus 30 and max is current year plus 20
        clickInput: true,
        //minDate: new Date(2021, 0 , 1),
        autoclose: true,
        todayHighlight: true,
        autoSize: true,
        //maxDate: "+1m", // Sets max date to plus 1 month
        //minDate: "-1m", // Sets max date to minus 1 month
        firstDay: 1 //Sets first day of month as Monday. Defaultt is Sunday 0.
    });
});
function CreatedDatePicker(elementsArray, parent) {
    for (i = 0; i < elementsArray.length; ++i) {
        $('.' + elementsArray[i]).attr("placeholder", "dd-mm-yyyy");
        // $('.' + elementsArray[i]).attr("readonly", "readonly");
        $('.' + elementsArray[i]).datepicker({
            container: parent,
        });
    }
}
function ActivateSideMenu(url) {
    $('.nav-menu li a[href="' + url + '"]').parent("li").addClass('active');
    $('.nav-menu li a[href="' + url + '"]').parent("li").parent("ul").parent("li").addClass('active').addClass('open');
    $('.nav-menu li a[href="' + url + '"]').parent("li").parent("ul").parent("li").find('.collapse-sign').find('em').removeClass('fa-angle-down').addClass('fa-angle-up');
}
$(document).on("keyup", "#txtSearch,.txtSearch", function () {
    var color = $('.TopSettingIcon').find("i").css("color");
    if ($(this).val() == '' || $(this).val() == null) {
        $(this).css("border-color", "#E5E5E5");
        $(this).parents('.input-group').find('.btnSearchGo').css("background-color", "#FAF8FB");
        $(this).parents('.input-group').find('.btnSearchGo').css("color", "#000");
    }
    else {
        $(this).css("border-color", color);
        $(this).parents('.input-group').find('.btnSearchGo').css("background-color", color);
        $(this).parents('.input-group').find('.btnSearchGo').css("color", "#fff");
    }

});
$(document).on('mouseenter', '.btnSearchGo', function () {
    var color = $('.TopSettingIcon').find("i").css("color");
    $(this).parents('.input-group').find('.btnSearchGo').css("background-color", color);
    $(this).parents('.input-group').find('.btnSearchGo').css("color", "#fff");
}).on('mouseleave', '.btnSearchGo', function () {
    if ($("#txtSearch").val() == '' || $("#txtSearch").val() == null) {
        $(this).parents('.input-group').find('.btnSearchGo').css("background-color", "#FAF8FB");
        $(this).parents('.input-group').find('.btnSearchGo').css("color", "#000");
    }
    else {
        var color = $('.TopSettingIcon').find("i").css("color");
        $(this).parents('.input-group').find('.btnSearchGo').css("background-color", color);
        $(this).parents('.input-group').find('.btnSearchGo').css("color", "#fff");
    }
});
$(document).on('click', '.btnOpenModal', function () {
    var color = $('.TopSettingIcon').find("i").css("color");
    $('#AddEditModalHeader').css("background-color", color);
});
$(document).on('click', '.btnOpenTopModal', function () {
    var color = $('.TopSettingIcon').find("i").css("color");
    $('#TopModalHeader').css("background-color", color);
});
$("#ulSearchCategory").on("click", "a", function (e) {
    e.preventDefault();
    $("#btnSearchCategory").html($(this).html());
    $("#ulSearchCategory").removeClass("show");
})
$(document).on("click", '.select2-selection__clear', function () {
    $(this).parents("span").prev('select').val(null).trigger("change");
})
$(document).on('change', 'select', function () {
    $(this).parents('section').find('label.error').remove();
    //      $(this).parents('td').siblings('td').find('.btn').css("margin-top", "10px");
    //$(this).parents('label').siblings('em').remove();
})

$(function () {
    var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    var now = new Date();
    var date = days[now.getDay()] + ', ' +
        months[now.getMonth()] + ' ' +
        now.getDate() + ', ' +
        now.getFullYear();
    $('.js-get-date').html(date);
})
//#region DateTime Converter
function getDateTimeForDatatable(date) {
    if (date == null) {
        return '';
    }
    else {
        var newdate = new Date(parseInt(date.substr(6)));
        var day = newdate.getDate();
        var month = newdate.getMonth() + 1;
        if (parseFloat(day) < 10) {
            day = "0" + day;
        }
        if (parseFloat(month) < 10) {
            month = "0" + month;
        }
        var year = newdate.getFullYear();

        var hours = newdate.getHours();
        var minutes = newdate.getMinutes();
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        var today = day + '-' + month + '-' + year + '&nbsp;&nbsp;&nbsp;' + strTime;
        return today;
    }
}
function getYearForDatatable(date) {
    if (date == null) {
        return '';
    }
    else {
        var newdate = new Date(parseInt(date.substr(6)));
        var day = newdate.getDate();
        var month = newdate.getMonth() + 1;
        if (parseFloat(day) < 10) {
            day = "0" + day;
        }
        if (parseFloat(month) < 10) {
            month = "0" + month;
        }
        var year = newdate.getFullYear();

        var today = year;
        return today;
    }
}
function getDateForDatatable(date) {
    if (date == null) {
        return '';
    }
    else {
        var newdate = new Date(parseInt(date.substr(6)));
        var day = newdate.getDate();
        var month = newdate.getMonth() + 1;
        if (parseFloat(day) < 10) {
            day = "0" + day;
        }
        if (parseFloat(month) < 10) {
            month = "0" + month;
        }
        var year = newdate.getFullYear();

        var today = day + '-' + month + '-' + year;
        return today;
    }
}
function getMonthForDatatable(date) {
    if (date == null) {
        return '';
    }
    else {
        var newdate = new Date(parseInt(date.substr(6)));
        var day = newdate.getDate();
        var month = newdate.getMonth() + 1;
        if (parseFloat(day) < 10) {
            day = "0" + day;
        }
        if (parseFloat(month) < 10) {
            month = "0" + month;
        }
        var year = newdate.getFullYear();

        var today = month + '-' + year;
        return today;
    }
}
function getMonthForDatatableWithDateString(date) {
    if (date == null) {
        return '';
    }
    else {
        var newdate = new Date(date);
        var day = newdate.getDate();
        var month = newdate.getMonth() + 1;
        if (parseFloat(day) < 10) {
            day = "0" + day;
        }
        if (parseFloat(month) < 10) {
            month = "0" + month;
        }
        var year = newdate.getFullYear();

        var today = month + '-' + year;
        return today;
    }
}
function getMonthStringForDatatable(date) {
    if (date == null) {
        return '';
    }
    else {
        const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        ];
        var newdate = new Date(parseInt(date.substr(6)));
        var monthString = monthNames[newdate.getMonth()];
        var year = newdate.getFullYear();
        var today = monthString + ' ' + year;
        return today;
    }
}
function getFullMonthStringForDatatable(date) {
    if (date == null) {
        return '';
    }
    else {
        const monthNames = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        ];
        var newdate = new Date(parseInt(date.substr(6)));
        var monthString = monthNames[newdate.getMonth()];
        var year = newdate.getFullYear();
        var today = monthString + ', ' + year;
        return today;
    }
}
function getDateTimeForAppendedJquery(date) {
    if (date == null) {
        return '';
    }
    else {

        var today = new Date(parseInt(date.substr(6)));
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!

        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        var h = today.getHours();
        var m = today.getMinutes();
        if (h < 10) h = '0' + h;
        if (m < 10) m = '0' + m;
        var today = yyyy + '-' + mm + '-' + dd + 'T' + h + ':' + m;
        return today;
    }
}
function getDateAndTimeFromInputTypeDate(myDate) {
    var today = new Date(myDate);
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!

    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var hours = today.getHours();
    var minutes = today.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    var today = dd + '-' + mm + '-' + yyyy + '&nbsp;&nbsp;&nbsp;' + strTime;
    return today;

}
function getTodayDateForTypeMonth() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!

    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var today = yyyy + '-' + mm;
    return today;

}

//#endregion
//#region Set Current Date
function SetCurrentDate(id) {
    var newdate = new Date();
    newdate.setDate(newdate.getDate());
    var day = newdate.getDate();
    var month = newdate.getMonth() + 1;
    if (digits_count(day) == 1) {
        day = "0" + day;
    }
    if (digits_count(month) == 1) {
        month = "0" + month;
    }
    var year = newdate.getFullYear();
    var today = year + '-' + month + '-' + (day);
    $(id).val(today);
}
function digits_count(n) {
    var count = 0;
    if (n > 1)
        ++count;
    while (n / 10 >= 1) {
        n /= 10;
        ++count;
    }
    return count;
}
function getNowTime() {
    var d = new Date(),
        h = d.getHours(),
        m = d.getMinutes();
    if (h < 10) h = '0' + h;
    if (m < 10) m = '0' + m;
    var today = h + ':' + m;
    return today;
}
function getGivenTime(givenTime) {
    var d = new Date(parseInt(givenTime.substr(6))),
        h = d.getHours(),
        m = d.getMinutes();
    if (h < 10) h = '0' + h;
    if (m < 10) m = '0' + m;
    var today = h + ':' + m;
    return today;
}
//#endregion
//#region Convert hours into hours and minutes string
function ConvertHoursIntoHoursAndMinutesString(totalHours) {
    var hourAndMinute = '';
    var hour = parseInt(totalHours.toString().split(".")[0]);
    var minutes = parseFloat(parseFloat('0.' + totalHours.toString().split(".")[1]) * 60).toFixed(0);
    if (minutes > 0) {
        if (hour == 1 && minutes == 1) {
            hourAndMinute = hour + ' hour ' + minutes + ' minute';
        }
        else if (hour > 1 && minutes == 1) {
            hourAndMinute = hour + ' hours ' + minutes + ' minute';
        }
        else if (hour == 1 && minutes > 1) {
            hourAndMinute = hour + ' hour ' + minutes + ' minutes';
        }
        else if (hour > 1 && minutes > 1) {
            hourAndMinute = hour + ' hours ' + minutes + ' minutes';
        }
        else if (hour == 0 && minutes > 1) {
            hourAndMinute = minutes + ' minutes';
        }
        else if (hour == 0 && minutes == 1) {
            hourAndMinute = minutes + ' minute';
        }
        else if (hour == 0 && minutes < 1) {
            hourAndMinute = minutes + ' minute';
        }
        else {
            hourAndMinute = hour + ' hour(s) ' + minutes + ' minute(s)';
        }
    }
    else {
        if (hour > 1) {
            hourAndMinute = hour + ' hours';
        }
        else {
            hourAndMinute = hour + ' hour';
        }
    }
    return hourAndMinute;
}
//#endregion
//#region Restrict Alphabets in textboxes and only allow numbers or decimals
function NumberPostiveNegativeWithDecimal(evt, element, totalDigits, decimalPoints) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if ((charCode != 127) &&
        (charCode != 45 || $(element).val().indexOf('-') != -1) &&
        (charCode != 46 || $(element).val().indexOf('.') != -1) &&
        (charCode < 48 || charCode > 57)) {
        evt.preventDefault();
    }
    //#region Restrict Number of digits and decimals
    if (totalDigits == 1 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{1})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 1 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{1})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 1 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{1})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 1 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{1})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 1 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{1})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 2 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{2})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 2 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{2})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 2 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{2})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 2 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{2})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 2 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{2})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 3 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 3 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 3 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 3 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 3 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 4 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{4})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 4 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{4})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 4 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{4})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 4 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{4})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 4 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{4})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 5 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{5})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 5 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{5})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 5 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{5})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 5 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{5})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 5 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{5})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 6 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{6})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 6 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{6})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 6 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{6})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 6 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{6})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 6 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{6})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 7 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{7})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 7 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{7})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 7 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{7})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 7 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{7})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 7 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{7})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 8 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{8})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 8 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{8})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 8 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{8})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 8 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{8})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 8 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{8})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 9 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{9})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 9 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{9})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 9 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{9})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 9 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{9})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 9 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{9})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 10 && decimalPoints == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{10})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{1})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 10 && decimalPoints == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{10})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 10 && decimalPoints == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{10})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{3})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 10 && decimalPoints == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{10})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{4})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 10 && decimalPoints == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{10})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{5})./g, '$1');    // not more than 2 digits after decimal
    }
    //#endregion
    return true;
}
function NumberPostive(evt, element, totalDigits) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if ((charCode != 8) && (charCode != 127) && (charCode < 48 || charCode > 57)) {
        evt.preventDefault();
    }
    //#region Restrict Number of digits
    if (totalDigits == 1) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{1})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 2) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{2})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 3) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 4) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{4})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 5) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{5})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 6) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{6})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 7) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{7})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 8) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{8})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 9) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\9]{6})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    else if (totalDigits == 10) {
        element.value = element.value
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{10})[\d]/g, '$1')   // not more than 6 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{0})./g, '$1');    // not more than 2 digits after decimal
    }
    //#endregion
    return true;
}
//#endregion
//#region Get Amount in Commas, Without Commas, Words etc
function ReplaceNumberWithCommas(yourNumber) {
    if (yourNumber != "0") {
        //Seperates the components of the number
        var n = yourNumber.toString().split(".");
        //Comma-fies the first part
        n[0] = n[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");

        if (n.length == 1) {
            n[0] = n[0] + ".00";
        }
        else {
            if (n[1].toString().length == 1) {
                n[1] = n[1] + "0";
            }
        }
        //Combines the two sections
        n = n.join(".");
    }
    else {
        n = yourNumber;
    }
    return n;
}
function ReplaceCommas(yourNumber) {
    var noCommas = yourNumber.replace(/,/g, ''),
        asANumber = +noCommas;
    return noCommas;
}
function convertNumberToWords(amount) {
    var words = new Array();
    words[0] = 'Zero';
    words[1] = 'One';
    words[2] = 'Two';
    words[3] = 'Three';
    words[4] = 'Four';
    words[5] = 'Five';
    words[6] = 'Six';
    words[7] = 'Seven';
    words[8] = 'Eight';
    words[9] = 'Nine';
    words[10] = 'Ten';
    words[11] = 'Eleven';
    words[12] = 'Twelve';
    words[13] = 'Thirteen';
    words[14] = 'Fourteen';
    words[15] = 'Fifteen';
    words[16] = 'Sixteen';
    words[17] = 'Seventeen';
    words[18] = 'Eighteen';
    words[19] = 'Nineteen';
    words[20] = 'Twenty';
    words[21] = 'Twenty One';
    words[22] = 'Twenty Two';
    words[23] = 'Twenty Three';
    words[24] = 'Twenty Four';
    words[25] = 'Twenty Five';
    words[26] = 'Twenty Six';
    words[27] = 'Twenty Seven';
    words[28] = 'Twenty Eight';
    words[29] = 'Twenty Nine';
    words[30] = 'Thirty';
    words[31] = 'Thirty One';
    words[32] = 'Thirty Two';
    words[33] = 'Thirty Three';
    words[34] = 'Thirty Four';
    words[35] = 'Thirty Five';
    words[36] = 'Thirty Six';
    words[37] = 'Thirty Seven';
    words[38] = 'Thirty Eight';
    words[39] = 'Thirty Nine';
    words[40] = 'Forty';
    words[41] = 'Forty One';
    words[42] = 'Forty Two';
    words[43] = 'Forty Three';
    words[44] = 'Forty Four';
    words[45] = 'Forty Five';
    words[46] = 'Forty Six';
    words[47] = 'Forty Seven';
    words[48] = 'Forty Eight';
    words[49] = 'Forty Nine';
    words[50] = 'Fifty';
    words[51] = 'Fifty One';
    words[52] = 'Fifty Two';
    words[53] = 'Fifty Three';
    words[54] = 'Fifty Four';
    words[55] = 'Fifty Five';
    words[56] = 'Fifty Six';
    words[57] = 'Fifty Seven';
    words[58] = 'Fifty Eight';
    words[59] = 'Fifty Nine';
    words[60] = 'Sixty';
    words[61] = 'Sixty One';
    words[62] = 'Sixty Two';
    words[63] = 'Sixty Three';
    words[64] = 'Sixty Four';
    words[65] = 'Sixty Five';
    words[66] = 'Sixty Six';
    words[67] = 'Sixty Seven';
    words[68] = 'Sixty Eight';
    words[69] = 'Sixty Nine';
    words[70] = 'Seventy';
    words[71] = 'Seventy One';
    words[72] = 'Seventy Two';
    words[73] = 'Seventy Three';
    words[74] = 'Seventy Four';
    words[75] = 'Seventy Five';
    words[76] = 'Seventy Six';
    words[77] = 'Seventy Seven';
    words[78] = 'Seventy Eight';
    words[79] = 'Seventy Nine';
    words[80] = 'Eighty';
    words[81] = 'Eighty One';
    words[82] = 'Eighty Two';
    words[83] = 'Eighty Three';
    words[84] = 'Eighty Four';
    words[85] = 'Eighty Five';
    words[86] = 'Eighty Six';
    words[87] = 'Eighty Seven';
    words[88] = 'Eighty Eight';
    words[89] = 'Eighty Nine';
    words[90] = 'Ninety';
    words[91] = 'Ninety One';
    words[92] = 'Ninety Two';
    words[93] = 'Ninety Three';
    words[94] = 'Ninety Four';
    words[95] = 'Ninety Five';
    words[96] = 'Ninety Six';
    words[97] = 'Ninety Seven';
    words[98] = 'Ninety Eight';
    words[99] = 'Ninety Nine';

    amount = amount.toString();
    var atemp = amount.split(".");
    var number = atemp[0].split(",").join("");
    var n_length = number.length;
    var words_string = "";
    if (n_length <= 9) {
        var n_array = new Array(0, 0, 0, 0, 0, 0, 0, 0, 0);
        var received_n_array = new Array();
        for (var i = 0; i < n_length; i++) {
            received_n_array[i] = number.substr(i, 1);
        }
        for (var i = 9 - n_length, j = 0; i < 9; i++, j++) {
            n_array[i] = received_n_array[j];
        }
        for (var i = 0, j = 1; i < 9; i++, j++) {
            if (i == 0 || i == 2 || i == 4 || i == 7) {
                if (n_array[i] == 1) {
                    n_array[j] = 10 + parseInt(n_array[j]);
                    n_array[i] = 0;
                }
            }
        }
        value = "";
        for (var i = 0; i < 9; i++) {
            if (i == 0 || i == 2 || i == 4 || i == 7) {
                value = n_array[i] * 10;
            } else {
                value = n_array[i];
            }
            if (value != 0) {
                words_string += words[value] + " ";
            }
            if ((i == 1 && value != 0) || (i == 0 && value != 0 && n_array[i + 1] == 0)) {
                words_string += "Crores ";
            }
            if ((i == 3 && value != 0) || (i == 2 && value != 0 && n_array[i + 1] == 0)) {
                words_string += "Lakhs ";
            }
            if ((i == 5 && value != 0) || (i == 4 && value != 0 && n_array[i + 1] == 0)) {
                words_string += "Thousand ";
            }
            if (i == 6 && value != 0 && (n_array[i + 1] != 0 && n_array[i + 2] != 0)) {
                words_string += "Hundred ";
            } else if (i == 6 && value != 0) {
                words_string += "Hundred ";
            }
        }
        words_string = words_string.split("  ").join(" ");
    }
    if (parseFloat(amount) == 0) {
        words_string = "Zero";
    }
    else {
        if (n_length == 10) {
            var num1 = words[parseFloat(number.substr(0, 1))];
            var num2 = words[parseFloat(number.toString().slice(1, 3))];
            var num3 = words[parseFloat(number.toString().slice(3, 5))];
            var num4 = words[parseFloat(number.toString().slice(5, 7))];
            var num5 = words[parseFloat(number.toString().slice(7, 8))];
            var num6 = words[parseFloat(number.toString().slice(8, 10))];

            words_string = ((parseFloat(number.substr(0, 1)) != 0) ? (num1 + " Arab ") : '') +
                ((parseFloat(number.toString().slice(1, 3)) != 0) ? (num2 + " Crores ") : '') +
                ((parseFloat(number.toString().slice(3, 5)) != 0) ? (num3 + " Lakhs ") : '') +
                ((parseFloat(number.toString().slice(5, 7)) != 0) ? (num4 + " Thousand ") : '') +
                ((parseFloat(number.toString().slice(7, 8)) != 0) ? (num5 + " Hundred ") : '') +
                ((parseFloat(number.toString().slice(8, 10)) != 0) ? (num6) : '');
        }
        else if (n_length == 11) {
            var num1 = words[parseFloat(number.substr(0, 2))];
            var num2 = words[parseFloat(number.toString().slice(2, 4))];
            var num3 = words[parseFloat(number.toString().slice(4, 6))];
            var num4 = words[parseFloat(number.toString().slice(6, 8))];
            var num5 = words[parseFloat(number.toString().slice(8, 9))];
            var num6 = words[parseFloat(number.toString().slice(9, 11))];

            words_string = ((parseFloat(number.substr(0, 2)) != 0) ? (num1 + " Arab ") : '') +
                ((parseFloat(number.toString().slice(2, 4)) != 0) ? (num2 + " Crores ") : '') +
                ((parseFloat(number.toString().slice(4, 6)) != 0) ? (num3 + " Lakhs ") : '') +
                ((parseFloat(number.toString().slice(6, 8)) != 0) ? (num4 + " Thousand ") : '') +
                ((parseFloat(number.toString().slice(8, 9)) != 0) ? (num5 + " Hundred ") : '') +
                ((parseFloat(number.toString().slice(9, 11)) != 0) ? (num6) : '');
        }
    }
    return words_string;
}
function MyNumberToWords(amount) {
    amount = amount.toString();
    var atemp = amount.split(".");
    var number = atemp[0].split(",").join("");
    var n_length = number.length;
    var returnNum = '';
    if (n_length == 0 || n_length == 1 || n_length == 2) {
        returnNum = amount.toString();
    }
    else if (n_length == 3) {
        returnNum = amount.toString().slice(0, 1) + "." + amount.toString().slice(1, 3) + " Hundred";
    }
    else if (n_length == 4) {
        returnNum = amount.toString().slice(0, 1) + "." + amount.toString().slice(1, 3) + " Thousand";
    }
    else if (n_length == 5) {
        returnNum = amount.toString().slice(0, 2) + "." + amount.toString().slice(2, 4) + " Thousand";
    }
    else if (n_length == 6) {
        returnNum = amount.toString().slice(0, 1) + "." + amount.toString().slice(1, 3) + " Lac";
    }
    else if (n_length == 7) {
        returnNum = amount.toString().slice(0, 2) + "." + amount.toString().slice(2, 4) + " Lac";
    }
    else if (n_length == 8) {
        returnNum = amount.toString().slice(0, 1) + "." + amount.toString().slice(1, 3) + " Crore";
    }
    else if (n_length == 9) {
        returnNum = amount.toString().slice(0, 2) + "." + amount.toString().slice(2, 4) + " Crore";
    }
    else if (n_length == 10) {
        returnNum = amount.toString().slice(0, 1) + "." + amount.toString().slice(1, 3) + " Arab";
    }
    else if (n_length == 11) {
        returnNum = amount.toString().slice(0, 2) + "." + amount.toString().slice(2, 4) + " Arab";
    }
    else {
        returnNum = '';
    }
    return returnNum;
}
//#endregion
//#region Add Custom Validate Method
$.validator.addMethod("GreaterThanDecimal", function (value, element, param) {
    if (this.optional(element)) return true;
    var i = parseFloat(value, 10);
    var j = parseFloat($(param).val(), 10);
    return i >= j;
});
$.validator.addMethod("LessThanDecimal", function (value, element, param) {
    if (this.optional(element)) return true;
    var i = parseFloat(value, 10);
    var j = parseFloat($(param).val(), 10);
    return i <= j;
});
$.validator.addMethod("GreaterThanInt", function (value, element, param) {
    if (this.optional(element)) return true;
    var i = parseInt(value, 10);
    var j = parseInt($(param).val(), 10);
    return i >= j;
});
$.validator.addMethod("LessThanInt", function (value, element, param) {
    if (this.optional(element)) return true;
    var i = parseInt(value, 10);
    var j = parseInt($(param).val(), 10);
    return i <= j;
});
$.validator.addMethod("GreaterThanDateTime", function (value, element, params) {
    if (!/Invalid|NaN/.test(new Date(value))) {
        return new Date(value) >= new Date($(params).val());
    }
    return isNaN(value) && isNaN($(params).val())
        || (Number(value) >= Number($(params).val()));
});
$.validator.addMethod("LessThanDateTime", function (value, element, params) {
    if (!/Invalid|NaN/.test(new Date(value))) {
        return new Date(value) <= new Date($(params).val());
    }
    return isNaN(value) && isNaN($(params).val())
        || (Number(value) <= Number($(params).val()));
});
$.validator.addMethod("GreaterThanOnlyDate", function (value, element, params) {
    return value >= $(params).val();
});
$.validator.addMethod("LessThanOnlyDate", function (value, element, params) {
    return value <= $(params).val();
});
$.validator.addMethod("GreaterThanOnlyTime", function (value, element, params) {
    var time1Hours = $(params).val().split(':')[0];
    var time2Hours = value.split(':')[0];
    var time1Minutes = $(params).val().split(':')[1];
    var time2Minutes = value.split(':')[1];
    var date1 = new Date();
    date1.setHours(time1Hours);
    date1.setMinutes(time1Minutes);

    var date2 = new Date();
    date2.setHours(time2Hours);
    date2.setMinutes(time2Minutes);
    return date2 >= date1;
});
$.validator.addMethod("LessThanOnlyTime", function (value, element, params) {
    var time1Hours = $(params).val().split(':')[0];
    var time2Hours = value.split(':')[0];
    var time1Minutes = $(params).val().split(':')[1];
    var time2Minutes = value.split(':')[1];
    var date1 = new Date();
    date1.setHours(time1Hours);
    date1.setMinutes(time1Minutes);

    var date2 = new Date();
    date2.setHours(time2Hours);
    date2.setMinutes(time2Minutes);
    return date2 <= date1;
});

$.validator.addMethod("EqualPasswords", function (value, element, params) {
    return value == $(params).val();
});
//#endregion
