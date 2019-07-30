var buyerId = _vendorRegistry.CustomEmailTemplateBuyerId;
function updatePreview() {
    var introduction = "";
    var helpMessage = "";
    var signature = "";
    var useLogo = false;
    var useIntroduction = false;
    var useMessage = false;
    var useSignature = false;

    if ($('#useLogo').is(':checked')) {
        useLogo = true;
    }
    if ($('#useIntroduction').is(':checked')) {
        introduction = $("#introduction").val();
        useIntroduction = true;
    }
    if ($('#useHelpMessage').is(':checked')) {
        helpMessage = $("#helpMessage").val();
        useMessage = true;
    }
    if ($('#useSignature').is(':checked')) {
        signature = $("#signature").val();
        useSignature = true;
    }

    $.ajax({
        type: "POST",
        url: common.baseurl + '/General/GetBidNotificationEmailPreview',
        data: {
            useLogo: useLogo,
            useIntroduction: useIntroduction,
            useHelpMessage: useMessage,
            useSignature: useSignature,
            introduction: introduction,
            helpMessage: helpMessage,
            signature: signature,
            buyerId: buyerId,
            fromSendBid: false,
            addendumTitle: ""
        },
        async: true,
        success: function (msg) {
            $("#email-preview").empty();
            $("#email-preview").append(msg);
        }
    });
}

$(document).ready(function () {
    if (_vendorRegistry._vendorRegistry.CustomEmailTemplateIsVmsPlusUser == 'True' && _vendorRegistry._vendorRegistry.CustomEmailTemplateIsUsersRoleVmsPlus == 'False')
    {
        $("input").prop("disabled", true);
        $("textarea").prop("disabled", true);
    }

    if (_vendorRegistry._vendorRegistry.CustomEmailTemplateIsVmsPlusUser != true) {
        $("input").prop("disabled", true);
        $("textarea").prop("disabled", true);
        $(".vms-plus-feature").css('background-image', 'url("../../Content/images/vms-watermark-regular1.jpg")');
        $(".vms-plus-feature").css('background-position', 'right bottom');
        $(".vms-plus-feature").css('background-size', '100% 100%');
        $(".vms-plus-feature").css('background-repeat', 'no-repeat');

    } else {
        updatePreview();

        $("#useIntroduction").change(function () {
            updatePreview();
        });
        $("#useHelpMessage").change(function () {
            updatePreview();
        });
        $("#useSignature").change(function () {
            updatePreview();
        });
        $("#useLogo").change(function () {
            updatePreview();
        });
    }

    var BuyerSolicitationCustomizationIdValue = _vendorRegistry.CustomEmailTemplateBuyerSolicitationCustomizationId;
    $('#BuyerSolicitationCustomizationId').val(BuyerSolicitationCustomizationIdValue);

    $('#tvrMessageArea').fadeIn('slow').delay(3000).fadeOut('slow');

    $(".variousPopup").fancybox({
        type: 'ajax',
        dataType: 'html',
        headers: { 'X-fancyBox': true },
        autoSize: 'true',
        openEffect: 'fade',
        closeEffect: 'fade',
        closeBtn: 'true',
        closeClick: 'false',
        scrolling: 'auto',
        modal: 'false'
    });

    $(".deletePopup").fancybox({
        type: 'ajax',
        dataType: 'html',
        headers: { 'X-fancyBox': true },
        autoSize: 'true',
        openEffect: 'fade',
        closeEffect: 'fade',
        closeBtn: 'true',
        closeClick: 'false',
        scrolling: 'auto',
        modal: 'false'
    });

});