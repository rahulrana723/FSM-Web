
$(document).ready(function () {
    if (FSM.Success == "ok") {
        $("#EditResidenceDetailErrorDiv").empty();
        $("#EditResidenceDetailErrorDiv").append("Record Saved Successfully!");
        $('#EditResidenceDetailErrorDiv').css('color', 'green');
        $("#EditResidenceDetailErrorDiv").css("display", "block");
        FSM.Success = null;
    }
});

$(document).on('click', "#btnEditResidenceDetail", function (e) {
        var $form = $("#formResidenceDetail"); // form id
        $(window).scrollTop(0);
    $.post($form.attr("action"), $form.serialize(), function (result) {

        var id = result.id;
        var activetab = result.activetab;

        if (result.length > 0) {
            $("#EditResidenceDetailErrorDiv").empty(); // empty div
            var ErrorList = "<ul style='list-style:none;'>"
            $(result).each(function (i) {
                ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
            });
            ErrorList = ErrorList + "</ul>"
            $(window).scrollTop(0);
            $("#EditResidenceDetailErrorDiv").append(ErrorList);
            $('#EditResidenceDetailErrorDiv').css('color', 'red');
            $("#EditResidenceDetailErrorDiv").css("display", "block");
        }
        else {
            $("#EditResidenceDetailErrorDiv").empty();
            window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
        }
    });
});

$("#btnCancelResidenceDetail").on("click", function (e) {
    var id = $("#CustomerGeneralInfoId").val(); // form id
    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=Residence Details'
});