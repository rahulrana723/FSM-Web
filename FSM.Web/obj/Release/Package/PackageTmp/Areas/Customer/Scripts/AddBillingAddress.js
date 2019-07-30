$(document).ready(function () {
    $("#PostalCode").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");
    $("#customerBillingAddressViewModel_PostalCode").attr("maxlength", "4");
    $("#poBoxAddress").attr("maxlength", "5");
    var pOBox = $('#POBox').is(':checked');
    
    if (pOBox) {
       
        $('div.poBoxList').show();
        //$(".Address_details_outer").hide();
        $("#BillingAddressErrorDiv").hide();
        $("div.showHideBillingAddress").hide();
        $('div.divsubrbstate').show()

    }
    else {
        $('div.poBoxList').hide();
        $(".Address_details_outer").show();
        $("div.showHideBillingAddress").show();
    }
});

//$(document).on('click', "#btnSaveBillingAddress", function (e) {
$("#btnSaveBillingAddress").click(function () {
        var $form = $("#formBillingAddress"); // form id
        var id = $('#BillingAddressId').val();
        var guidval = '00000000-0000-0000-0000-000000000000';

        if (id == "" || id == undefined || id == guidval) {
            $(window).scrollTop(0);
            $.post($form.attr("action"), $form.serialize(), function (result) {

                var id = result.id;
                var activetab = result.activetab;

                if (result.length > 0) {

                    $("#BillingAddressErrorDiv").empty(); // empty div

                    var ErrorList = "<ul style='list-style:none;'>"
                    $(result).each(function (i) {
                        ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
                    });
                    ErrorList = ErrorList + "</ul>"
                    $(window).scrollTop(0);
                    $("#BillingAddressErrorDiv").append(ErrorList);
                    $('#BillingAddressErrorDiv').css('color', 'red');
                    $("#BillingAddressErrorDiv").css("display", "block");
                }
                else {
                    $("#BillingAddressErrorDiv").empty();
                    $(window).scrollTop(0);
                    $('.jobalert').empty();
                    $('.jobalert').css('color', 'green');
                    $('.jobalert').html('<Strong>Record saved successfully!</Strong>');
                    $('.jobalert').show();
                    $("#btnSaveBillingAddress").attr("disabled", "disabled");
                    window.setTimeout(function () {
                        $('.jobalert').hide();
                        window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
                    }, 4000)
                   
                }
            });
        }
        else {
            $.post(common.SitePath + "/Customer/Customer/EditBillingAddress", $form.serialize(), function (result) {

                var id = result.id;
                var activetab = result.activetab;

                if (result.length > 0) {
                    $("#BillingAddressErrorDiv").empty(); // empty div
                    var ErrorList = "<ul style='list-style:none;'>"
                    $(result).each(function (i) {
                        ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
                    });
                    ErrorList = ErrorList + "</ul>"
                    $(window).scrollTop(0);
                    $("#BillingAddressErrorDiv").append(ErrorList);
                    $('#BillingAddressErrorDiv').css('color', 'red');
                    $("#BillingAddressErrorDiv").css("display", "block");
                }
                else {
                    $("#BillingAddressErrorDiv").empty();
                    $(window).scrollTop(0);
                    $('.jobalert').empty();
                    $('.jobalert').css('color', 'green');
                    $('.jobalert').html('<Strong>Record updated successfully!</Strong>');
                    $('.jobalert').show();
                    window.setTimeout(function () {
                        $('.jobalert').hide();
                        window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
                    }, 4000)
                  
                }
            });
        }
});

$("#btnCancelBillingAddress").on("click", function (e) {
    $('#dvCustBillAddNew').show();
    $('#dvCustBillForm').hide();
});

$(document).on("change", "#POBox", function () {
    var a = $("#poBoxAddress").val();
    var pOBox = $(this).prop('checked');
    if (pOBox) {
        $("div.poBoxList").css("display", "block");
        $(".Address_details_outer").show();
        $("#BillingAddressErrorDiv").hide();
        $("div.showHideBillingAddress").hide();
        $('div.divsubrbstate').show();

        $("#StreetNo").val("");
        $("#StreetName").val("");
        $("#StreetType").val("");
        $("#Unit").val("");
        $("#customerBillingAddressViewModel_Unit").val("");
        $("#customerBillingAddressViewModel_StreetNo").val("");
        $("#customerBillingAddressViewModel_StreetName").val("");
        $("#customerBillingAddressViewModel_StreetType").val("");

    }
    else {
        $("div.poBoxList").css("display", "none");
        $("#poBoxAddress").val("");
        $(".Address_details_outer").show();
        $("div.showHideBillingAddress").show();
        $('div.divsubrbstate').show()
    }
});