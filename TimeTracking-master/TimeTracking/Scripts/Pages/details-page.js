$(function () {
    $.post("/Activities/Index/" + $("#ProjectID").val(), function (data) {
        $("#activities").html(data);
    });

    $.ajax({
        method: "POST",
        url: "Activities/CreateTimesheetEntry/",
        data: { ActvityID: "", UserID: "", NoOfhours : "", }
    })
        .done(function () {
            alert("Data Saved:");
        });


});