$(document).ready(function () {
    if (FSM.Success == "ok") {
        $("#BillingAddressErrorDiv").empty();
        $("#BillingAddressErrorDiv").append("Record Saved Successfully!");
        $('#BillingAddressErrorDiv').css('color', 'green');
        $("#BillingAddressErrorDiv").css("display", "block");
        FSM.Success = null;
    }
    $("#PostalCode").attr('maxlength', '4');
    $("#PostalCode").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");

    var pOBox = $('#POBox').is(':checked');
    
    if (pOBox) {
        //$('div.poBoxList').show();
        //$(".Address_details_outer").hide();
        //$("#BillingAddressErrorDiv").hide();

        $('div.poBoxList').show();
        $("#BillingAddressErrorDiv").hide();
        $("div.showHideBillingAddress").hide();
        $('div.divsubrbstate').show()
        $('div.divpo').css("display", "block");
    }
    else {
        // $('div.poBoxList').hide();
        //$(".Address_details_outer").show();
        $('div.poBoxList').hide();
        $(".Address_details_outer").show();
        $("div.showHideBillingAddress").show();
        $('div.divpo').css("display", "none");
       
    }

});

$(document).on('click', "#btnEditBillingAddress", function (e) {
    var $form = $("#formBillingAddress"); // form id
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
            window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
        }
    });
});

$(document).on('click', "#btnEditBillingAddressForJob", function (e) {
    var $form = $("#formBillingAddress"); // form id
    var formdata = new FormData($('#formBillingAddress').get(0));
    $.ajax({
        url: $('#formBillingAddress').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            
            $(window).scrollTop(0);
            if (result.status == "saved") {
                $("#BillingAddressErrorDiv").empty();
                $('.jobalert').empty();
                $('.jobalert').css('color', 'green');
                $('.jobalert').html(result.msg);
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                }, 4000)
            }
            else if (result.status == "failure")
            {
                $('#BillingAddressErrorDiv').empty();
                var ErrorList = "<ul style='list-style:none;'>"
                $(result).each(function (i) {

                    ErrorList = ErrorList + "<li>" + result.errors[i].ErrorMessage + "</li>";

                });
                ErrorList = ErrorList + "</ul>"
                $(window).scrollTop(0);
                $('#BillingAddressErrorDiv').css('color', 'red');
                $('#BillingAddressErrorDiv').html(ErrorList);
                $('#BillingAddressErrorDiv').show();
            }
            else {
                $('#BillingAddressErrorDiv').empty();
                var ErrorList = "<ul style='list-style:none;'>"
                $(result).each(function (i) {
                   
                        ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
                    
                });
                ErrorList = ErrorList + "</ul>"
                $(window).scrollTop(0);
                $('#BillingAddressErrorDiv').css('color', 'red');
                $('#BillingAddressErrorDiv').html(ErrorList);
                $('#BillingAddressErrorDiv').show();
            }
        }
    });
});

$("#btnCancelBillingAddress").on("click", function (e) {
    var id = $("#CustomerGeneralInfoId").val(); // form id
    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=Billing Address'
});

$("#btnCancelBillingAddressForJob").on("click", function (e) {
    var id = FSM.JobId; // form id
    window.location.href = common.SitePath + '/Employee/CustomerJob/SaveJobInfo?id=' + id + '&activetab=Billing Address'
});

$(document).on("change", "#POBox", function () {
    
    var a = $("#poBoxAddress").val();
    var pOBox = $(this).prop('checked');
    if (pOBox) {
        //$("div.poBoxList").css("display", "block");
        //$(".Address_details_outer").hide();
        //$("#BillingAddressErrorDiv").hide();
        $("div.poBoxList").css("display", "block");
        $(".Address_details_outer").show();
        $("#BillingAddressErrorDiv").hide();
        $(".shohideaddressdiv").hide();
        $('.divsubrbstate').show();
        $('div.divpo').css("display", "block");
        $("#Unit").val("");
        $("#StreetNo").val("");
        $("#StreetName").val("");
        $("#StreetType").val("");
    }
    else {
        $("div.poBoxList").css("display", "none");
        $("#poBoxAddress").val("");
        $(".Address_details_outer").show();
        $(".shohideaddressdiv").show();
        $('.divsubrbstate').show()
        $('div.divpo').css("display", "none");
      
    }
});