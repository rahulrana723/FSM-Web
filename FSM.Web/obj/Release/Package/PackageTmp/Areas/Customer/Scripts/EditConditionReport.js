$(document).ready(function () {
    if (FSM.Success == "ok") {
        $("#ConditionReportErrorDiv").empty();
        $("#ConditionReportErrorDiv").append("Record Saved Successfully!");
        $('#ConditionReportErrorDiv').css('color', 'green');
        $("#ConditionReportErrorDiv").css("display", "block");
        FSM.Success = null;
    }

});

$(document).on('click', "#btnEditConditionReport", function (e) {
    var $form = $("#formConditionReport"); // form id
    $(window).scrollTop(0);
    $.post($form.attr("action"), $form.serialize(), function (result) {

        var id = result.id;
        var activetab = result.activetab;

        if (result.length > 0) {
            $("#ConditionReportErrorDiv").empty(); // empty div

            var ErrorList = "<ul style='list-style:none;'>"
            $(result).each(function (i) {
                ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
            });
            ErrorList = ErrorList + "</ul>"
            $(window).scrollTop(0);
            $("#ConditionReportErrorDiv").append(ErrorList);
            $('#ConditionReportErrorDiv').css('color', 'red');
            $("#ConditionReportErrorDiv").css("display", "block");
        }
        else {
            $("#ConditionReportErrorDiv").empty();
            window.location.href = '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
        }
    });
});

$("#btnCancelConditionReport").on("click", function (e) {
    var id = $("#CustomerGeneralInfoId").val(); // form id
    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=Condition Report'
});