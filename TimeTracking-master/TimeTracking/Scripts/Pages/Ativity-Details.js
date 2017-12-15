$(function () {
    $.post("/Activities/ListComments/" + $("#ActivityId").val(), function (data) {

        $("#comments").html(data);
    });

});