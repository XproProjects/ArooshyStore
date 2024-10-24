$(function () {
    $('footer.webfooter').remove();
})
$('#btnSearch').click(function () {
    var attendanceDate = $('#AttendanceDate').val();
    if (attendanceDate == '' || attendanceDate == null) {
        $('#AttendanceDate').css('border-color', '#A90329');
        $('#AttendanceDate').css('background-color', '#FFF0F0');
        return false;
    }
    $('.newTh').remove();
    var today = new Date(attendanceDate);
    var day = today.getDate();
    var mm = today.getMonth() + 1;
    var yyyy = today.getFullYear();
    if (mm < 10) {
        mm = '0' + mm;
    }
    var day2 = 0;
    var dayString = '';
    var td = '';
    //It will append previous days from current day from current month
    for (var d = 1; d < day; d++) {
        day2 = day - d;
        if (day2 < 10) {
            day2 = "0" + day2;
        }
        td += '<th class="newTh" style="text-align: center;width:110px"> ' + day2 + '</th>';
    }
    $('#HeadingTr').append(td);
    $('.NoRecordTd').attr("colspan", 4 + day - 1);
    var colSpan = 4 + parseFloat(day) - 1;
    $('#TableBody').html('<tr class="noRecordTr"><td colspan="' + colSpan + '" class="dataTables_empty" rowspan="4" style="text-align: left;font-size:17px;background-color:#fff">' +
        '<center>' +
        '<div class="demo">' +
        '<div class="spinner-grow text-primary" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-secondary" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-success" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-danger" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-warning" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-info" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '<div class="spinner-grow text-light" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>' +
        '</div>' +
        '</center>' +
        '</td ></tr>');
    $.ajax({
        type: "POST",
        url: "/admin/employeeattendance/GetEmployeesForAttendance/",
        dataType: 'json',
        data: { 'attendanceDate': attendanceDate },
        success: function (response) {
            if (response.data.length == 0) {
                $('#TableBody').html('<tr class="noRecordTr"><td class="NoRecordTd" colspan="' + colSpan + '" style="text-align: center; padding: 14px; background-color: #fff; font-size: 17px;">No Record Found.</td></tr>');
            }
            else {
                GetSelectOptions(response, day);
            }
        }
    })
})
function GetSelectOptions(response, day) {
    var leaveTypes = [
        { LeaveTypeId: 1, LeaveTypeName: 'Sick Leave' },
        { LeaveTypeId: 2, LeaveTypeName: 'Casual Leave' },
        { LeaveTypeId: 3, LeaveTypeName: 'Maternity Leave' },
        { LeaveTypeId: 4, LeaveTypeName: 'Paternity Leave' }
    ];

    var div = '';
    var i = 0;
    for (var key in response.data) {
        var select = '<select class="custom-select Attendance" data-attendance-value="' + response.data[key].Attendance + '">' +
            '<option value="P">P - Present</option>' +
            '<option value="A">A - Absent</option>' +
            '<option value="H">H - Holiday</option>';
            '<option value="L">L - Leave</option>';

        for (var leaveKey in leaveTypes) {
            select += '<option value="' + leaveTypes[leaveKey].LeaveTypeId + '">L - ' + leaveTypes[leaveKey].LeaveTypeName + '</option>';
        }
        select += '</select>';

        var inTime = '';
        var outTime = '';
        var inTimeGiven = response.data[key].InTimeDateTime ? getGivenTime(response.data[key].InTimeDateTime) : getNowTime();
        var outTimeGiven = response.data[key].OutTimeDateTime ? getGivenTime(response.data[key].OutTimeDateTime) : getNowTime();

        inTime = '<input type="time" class="form-control InTime" value="' + inTimeGiven + '" id="InTime' + i + '" />';
        outTime = '<input type="time" class="form-control OutTime" value="' + outTimeGiven + '" id="OutTime' + i + '" />';

        var employeeName = response.data[key].EmployeeName || 'N/A'; 

        div += '<tr class="newRows">' +
            '<td style="text-align: left;">' +
            '<input type="hidden" class="EmployeeId" value="' + response.data[key].EmployeeId + '" />' +
            '<input type="hidden" class="AttendanceId" value="' + response.data[key].AttendanceId + '" />' +
            employeeName + 
            '</td>' +
            '<td style="text-align: center;"><div class="input-group">' + select + '</div></td>' +
            '<td style="width: 110px;">' + inTime + '</td>' +
            '<td style="text-align: center;">' + outTime + '</td>';

        for (var key2 in response.data[key].DateList) {
            var bgColor = '';
            switch (response.data[key].DateList[key2].Attendance) {
                case 'P': bgColor = '#F0FFF0'; break; 
                case 'A': bgColor = '#F8A5A5'; break; 
                case 'L': bgColor = '#F8ED62'; break; 
                case 'H': bgColor = '#58CCED'; break; 
                default: bgColor = '#FFF'; break;
            }

            div += '<td style="text-align: center; background-color: ' + bgColor + ';">' +
                response.data[key].DateList[key2].Attendance + '</td>';
        }

        div += '</tr>';
        i++;
    }

    var tableWidth = day <= 4 ? 1000 : day <= 8 ? 1300 : day <= 12 ? 1600 : day <= 16 ? 1900 :
        day <= 20 ? 2200 : day <= 24 ? 2500 : day <= 28 ? 2800 : 3000;

    $('#myTableMinStockLevel').css("width", tableWidth + "px");
    $('#TableBody').html(div);
    SetSelectValue();
}

function SetSelectValue() {
    $('.Attendance').each(function () {
        if ($(this).attr("data-attendance-value") == "" || $(this).attr("data-attendance-value") == null || $(this).attr("data-attendance-value") == "null" || $(this).attr("data-attendance-value") == undefined) {
            $(this).val("P");
        }
        else {
            $(this).val($(this).attr("data-attendance-value"));
        }
    })
}
$('#btnSave').click(function () {
    $('#btnSave').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    $('#btnSave').prop("disabled", true);
    SaveAttendance("");
})
$('#btnSaveAndPrint').click(function () {
    $('#btnSaveAndPrint').html("<i class='fal fa-sync fa-spin'></i> &nbsp; Processing...");
    $('#btnSaveAndPrint').prop("disabled", true);
    SaveAttendance("Print");
})
function SaveAttendance(print) {
    var check = 0;
    var attendanceDate = $('#AttendanceDate').val();
    if (attendanceDate == '' || attendanceDate == null) {
        $('#AttendanceDate').css('border-color', '#A90329');
        $('#AttendanceDate').css('background-color', '#FFF0F0');
        return false;
    }
    $('#btnSave').html("<i class='fa fa-spin fa-sync'></i>&nbsp;&nbsp;Processing....");
    $('#btnSave').prop("disabled", true);
    $('#btnSaveAndPrint').html("<i class='fa fa-spin fa-sync'></i>&nbsp;&nbsp;Processing....");
    $('#btnSaveAndPrint').prop("disabled", true);
    var data = [];
    $('.newRows').each(function () {
        check++;
        var employeeId = $(this).find('.EmployeeId').val();
        var attendanceId = $(this).find('.AttendanceId').val();
        var attendance = $(this).find('.Attendance').val();
        var inTime = $(this).find('.InTime').val();
        var outTime = $(this).find('.OutTime').val();
        var alldata = {
            'AttendanceId': attendanceId,
            'AttendanceDate': attendanceDate,
            'EmployeeId': employeeId,
            'Attendance': attendance,
            'CheckInTime': inTime,
            'CheckOutTime': outTime
        }
        data.push(alldata);
    });
    if (check == 0) {
        toastr.error("There is no attendance record to be saved.", "Error", { timeOut: 3000, "closeButton": true });
        $('#btnSave').html("Save");
        $('#btnSave').prop("disabled", false);
        $('#btnSaveAndPrint').html("Save");
        $('#btnSaveAndPrint').prop("disabled", false);
        return false;
    }
    var allData = JSON.stringify(data);
    $.ajax({
        type: "POST",
        url: "/admin/employeeattendance/InsertUpdateEmployeeAttendance/",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ 'data': allData }),
        success: function (data) {
            if (data.status == true) {
                toastr.success("Attendance Saved Successfully", "Success", { timeOut: 3000, "closeButton": true });
                $('.close').click();
                if (print == "Print") {
                    window.location.href = "/admin/employeeattendance/printattendancereport/" + attendanceDate;
                }
                else {
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                }
            }
            else {
                toastr.error(data.message, "Error", { timeOut: 3000, "closeButton": true });
                $('#btnSave').html("Save");
                $('#btnSave').prop("disabled", false);
                $('#btnSaveAndPrint').html("Save");
                $('#btnSaveAndPrint').prop("disabled", false);
            }
        }
    })
}
$('#AttendanceDate').keydown(function () {
    $('#AttendanceDate').css('border-color', '#bdbdbd');
    $('#AttendanceDate').css('background-color', '#fff');
}).mousedown(function () {
    $('#AttendanceDate').css('border-color', '#bdbdbd');
    $('#AttendanceDate').css('background-color', '#fff');
})
$('#btnRefresh').click(function () {
    location.reload();
})