$(document).ready(function () {
    var departmentAttendancePercentages = departmentAttendancePercentagess;
    departmentAttendancePercentages.sort(function (a, b) {
        return b.MemberCount - a.MemberCount;
    });
    var top10Data = departmentAttendancePercentages.slice(0, 10);

    var data = top10Data.map(function (item) {
        return [item.DepartmentName, item.AttendancePercentage];
    });

    var doughnutColors = [
        myapp_get_color.success_200,
        myapp_get_color.success_400,
        myapp_get_color.primary_50,
        myapp_get_color.primary_300,
        myapp_get_color.primary_500,
        myapp_get_color.success_200,
        myapp_get_color.success_400,
        myapp_get_color.primary_50,
        myapp_get_color.primary_300,
        myapp_get_color.primary_500
    ];

    $.plot("#flot-bar-fill", [
        {
            data: data,
            bars: {
                show: true,
                barWidth: 0.5,
                fill: function (series, i) {
                    var attendancePercentage = series.data[i][1];
                    if (attendancePercentage >= 90) {
                        return myapp_get_color.success_200;
                    } else if (attendancePercentage >= 75) {
                        return myapp_get_color.success_400;
                    }
                    else if (attendancePercentage >= 5) {
                        return myapp_get_color.success_400;
                    }
                    else {
                        return myapp_get_color.primary_200;
                    }
                }
            }
        }
    ], {
        xaxis: {
            mode: "categories",
            tickLength: 0,
        },
        yaxis: {
            tickFormatter: function (val, axis) {
                return val.toFixed(2) + "%";
            },
            min: 0,
            max: 100,
        },
        grid: {
            borderWidth: 0,
        },
    });
    var departmentChartData = departmentChartDatas;
    departmentChartData.sort(function (a, b) {
        return b.MemberCount - a.MemberCount;
    });
    var top10ChartData = departmentChartData.slice(0, 10);

    var doughnutData = top10ChartData.map(function (item) {
        return item.MemberCount;
    });

    var departmentNames = top10ChartData.map(function (item) {
        return "Department " + item.DepartmentName;
    });

    var doughnutConfig = {
        type: 'doughnut',
        data: {
            datasets: [{
                data: doughnutData,
                backgroundColor: doughnutColors,
                label: 'My dataset'
            }],
            labels: departmentNames
        },
        options: {
            responsive: true,
            legend: {
                display: true,
                position: 'bottom',
            }
        }
    };

    new Chart($("#doughnutChart > canvas").get(0).getContext("2d"), doughnutConfig);
});
