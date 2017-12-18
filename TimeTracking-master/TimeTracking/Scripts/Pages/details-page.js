$(function () {
    $.post("/Activities/Index/" + $("#ProjectID").val(), function (data) {
        $("#activities").html(data);
    });

    var options = [{ "Id": 1, "Period": "Day" }, { "Id": 2, "Period": "Week" }];
    /* options.forEach(function (option) {
         var option = $('<option>').attr('value', this.key).html(this.value);
         $('#period').append(option);
     });*/
    $.each(options, function () {
        var option = $('<option>').attr('value', this.Id).html(this.Period);

        $('#period').append(option);

        var today = getCurrentDate();
        $("#fromdatepicker").val(today);
        //$("#fromdatepicker").text(today);
        $("#todatepicker").val(today);
        //$("#todatepicker").text(today);
    });

    $("#todatepicker").on("change", function () {
        var todate = new Date(this.value);
        var fromdate = new Date($("#fromdatepicker")[0].value);
    });
    // Returns an array of dates between the two dates
    var getDates = function (startDate, endDate) {
        var dates = [],
            currentDate = startDate,
            addDays = function (days) {
                var date = new Date(this.valueOf());
                date.setDate(date.getDate() + days);
                return date;
            };
        while (currentDate <= endDate) {
            dates.push(currentDate);
            currentDate = addDays.call(currentDate, 1);
        }
        return dates;
    };

    function getCurrentDate() {
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
        var today = dd + '/' + mm + '/' + yyyy;
        return today;
    }

    function generateTable() {

    }
    $(function () {
        //  $("#fromdatepicker").datepicker();
        // $("#todatepicker").datepicker();
    });
});