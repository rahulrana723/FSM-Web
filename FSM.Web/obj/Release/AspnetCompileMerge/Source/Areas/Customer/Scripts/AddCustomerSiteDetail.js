$(document).ready(function () {
    if (FSM.Success == "ok") {
        $("#SiteDetailErrorDiv").empty();
        $("#SiteDetailErrorDiv").append("Record Saved Successfully!");
        $('#SiteDetailErrorDiv').css('color', 'green');
        $("#SiteDetailErrorDiv").css("display", "block");
        FSM.Success = null;
    }
    $('#PhoneNo1,#PhoneNo2,#PhoneNo3').mask('(999) 999-9999');
    $("#PostalCode").attr('maxlength', '9');
    $("#PostalCode").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");
    
});
$("#btnSaveSiteDetail").on("click", function (e) {
    var $form = $("#formSiteDetail"); // form id
    $(window).scrollTop(0);
    $.post($form.attr("action"), $form.serialize(), function (result) {

        var id = result.id;
        var activetab = result.activetab;

        if (result.length > 0) {
            $("#SitedetailErrorDiv").empty(); // empty div

            var ErrorList = "<ul style='list-style:none;'>"
            $(result).each(function (i) {
                ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
            });
            ErrorList = ErrorList + "</ul>"
            $(window).scrollTop(0);
            $("#SitedetailErrorDiv").append(ErrorList);
            $('#SitedetailErrorDiv').css('color', 'red');
            $("#SitedetailErrorDiv").css("display", "block");
        }
        else {
            $('#SitedetailErrorDiv').empty();
            window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
        }
    });
});

$("#btnCancelSiteDetail").on("click", function (e) {
    var id = $("#CustomerGeneralInfoId").val(); // form id
    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=Site Details'
});