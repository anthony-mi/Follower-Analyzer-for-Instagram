function ShowSpinner() {
    $("#divLoading").show();
    $.post(null, null,
        function (data) {
            $("#PID")[0].innerHTML = data;
            $("#divLoading").hide();
        });
}

$(function () {
    $("#dialog").dialog({
        autoOpen: false,
        modal: true,
        title: "View Details"
    });
    $("#getMostPopularPosts").click(function () {
        $.ajax({
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $('#dialog').html(response);
                $('#dialog').dialog('open');
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });
});
