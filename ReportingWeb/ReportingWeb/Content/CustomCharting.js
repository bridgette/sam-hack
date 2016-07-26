function exportActionTable() {
    if ($('#inputForm [name="EventId"]').val() == "") {
        alert('Event cannot be blank');
        return;
    }

    var eventId = $('#inputForm [name="EventId"]').val();
    var startDate = $('#inputForm [name="StartDate"]').val();
    var endDate = $('#inputForm [name="EndDate"]').val();

    window.location = '/Home/ExportActionTable?eventId=' + eventId + '&startDate=' + startDate + '&endDate=' + endDate;
}

function getDataDrawChart() {
    $('#downloadCSV').hide();

    if ($('#inputForm [name="EventId"]').val() == "") {
        alert('Event cannot be blank');
        return;
    }

    var postObj = {
        eventId: $('#inputForm [name="EventId"]').val(),
        startDate: $('#inputForm [name="StartDate"]').val(),
        endDate: $('#inputForm [name="EndDate"]').val()
    }

    $.ajax({
        type: "POST",
        url: "/Home/QueryData",
        data: postObj,
        success: function (data) {
            var countInData = [];
            var countOutData = [];

            var times = [];

            var currentAttendeeCount = 0;
            var attendeeCounts = []

            $(data).each(function (index) {
                time = new Date(data[index].RowKey)

                var countIn = data[index].CountIn;
                var countOut = data[index].CountOut;

                currentAttendeeCount += countIn;
                currentAttendeeCount -= countOut;

                attendeeCounts.push(currentAttendeeCount);

                times.push(time);
                countInData.push(countIn);
                countOutData.push(countOut);
            });

            var series = [{
                name: 'Entrances',
                data: countInData
            }, {
                name: 'Exits',
                data: countOutData
            }];

            renderChart('#chartContainer', 'Entrances and Exits Over Time', series, times);

            var series2 = [{
                type: 'line',
                name: 'Total',
                data: attendeeCounts
            }];

            renderChart('#chartContainer2', 'Total Attendees Over Time', series2, times);

            showCSVButton();
        }
    })
}

function showCSVButton() {
    $('#downloadCSV').show();
}

function renderChart(cssSelector, title, series, times) {
    $(cssSelector).highcharts({
        title: {
            text: title
        },
        xAxis: {
            categories: times,
            labels: {
                formatter: function () {
                    var date = new Date(this.value);
                    return date.format();
                }
            }
        },
        yAxis: {
            title: {
                text: 'Count'
            }
        },
        credits: {
            enabled: false
        },
        tooltip: {
            formatter: tooltipFormatter
        },
        series: series,
        dataType: "json"
    });
}

function tooltipFormatter() {
    var date = new Date(this.x);
    return '<b>' + date.format() + ':</b> ' + this.series.name + ' ' + this.y + '</b>';
}