$(document).ready(function () {


    $('.DatePicker').monthpicker({
        pattern: 'mmm yyyy',
        monthNames: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
    });

    $('.ddlmultiselect')
        .multiselect({
            maxHeight: 200,
            includeSelectAllOption: true,
            nonSelectedText: '(select)',
        });

    var daysValue = $("#Days").val();
    if (daysValue == "") {
        $('.multiselect').attr('disabled', true);
    } 


    $(document).on('change', '#Days', function (event) {
        var daysValue = $("#Days").val();
        if (daysValue == "") {
            $('.multiselect').attr('disabled', true);
            $(".multiselect").multiselect('destroy');
            $('.multiselect').append("<option value=''>(Select)</option>");
        } else {
            bindWeeksDays(daysValue);
            $('.multiselect').attr('disabled', false);

        }
    });

    function bindWeeksDays(daysValue) {
        var data = {
            dayValue: daysValue
        };

        $.get(FSM.GetWeeks, data, function (result) {
            $("#Weeks").multiselect('destroy');
            $("#Weeks").html("");
            for (var i = 0; i < result.length; i++) {
                var opt = new Option(result[i].Text, result[i].Value);
                $('#Weeks').append(opt).multiselect('rebuild');
            }
        });
    }
});

function ValidateRoasteddata() {
    var otrw = $("#tempAssignTo").val();
    var day = $("#Days").val();
    var week = $("#Weeks").val();
    var startDate = $("#StartDateBooked").val();
    var endDate = $("#EndDateBooked").val();

    $("#errrorOtrw").text("");
    $("#errrorDays").text("");
    $("#errrorWeeks").text("");

    if (otrw == "" || otrw == undefined) {
        $("#errrorOtrw").text("Please select OTRW").css("color", "red");
        return false;
    }
    if (day == "" || day == undefined) {
        $("#errrorDays").text("Please select Days").css("color", "red");
        return false;
    }
    if (week == "" || week == []) {
        $("#errrorWeeks").text("Please select Weeks").css("color", "red");
        return false;
    }
    if (startDate == "" || startDate == null) {
        $("#errrorStart").text("Please select start date").css("color", "red");
        return false;
    }
    if (endDate == "" || endDate == null) {
        $("#errrorEnd").text("Please select end date").css("color", "red");
        return false;
    }
}