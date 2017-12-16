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
    });

    $(function () {
        //  $("#fromdatepicker").datepicker();
       // $("#todatepicker").datepicker();
    });
});