function ShowSpinner() {
    $("#divLoading").show();
    $.post(null, null,
        function (data) {
            $("#PID")[0].innerHTML = data;
            $("#divLoading").hide();
        });
}