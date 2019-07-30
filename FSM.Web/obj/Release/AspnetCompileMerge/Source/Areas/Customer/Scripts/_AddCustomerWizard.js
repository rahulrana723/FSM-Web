var CustomerWizard = {
    CustomerGeneralInfoId: "",
    CustomerSiteFilename: "",
    WizardSiteId: "",

    openCustomerContactWizard: function () {
        $.ajax({
            url: common.SitePath + "/customer/customer/_CustomercontactsAddWizard",
            data: { CustomerGeneralinfoid: CustomerWizard.CustomerGeneralInfoId },
            dataType: "html",
            success: function (data) {
                $('#divCustomercontactwizard').html(data);
                $("#AddSiteWizard").modal("hide");
                $("#AddContactWizard").modal("show");
                $(".wizardSiteid").val(CustomerWizard.WizardSiteId);
            }
        });
    }
}

$(document).ready(function () {
    $("#AddCustomerWizard").click(function (i, e) {
        $("#addCustomerWizard").modal("show");
        //$("#AddSiteWizard").modal("show");
        //$("#AddContactWizard").modal("show");
        return false;
    });

    $("#btnSaveGeneralInfo").click(function (i, e) {

        var $form = $("#formGeneralInfo"); // form id
        var formdata = new FormData($('#formGeneralInfo').get(0));
        $.ajax({
            url: $('#formGeneralInfo').attr("action"),
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            success: function (result) {
                var id = result.id;
                var siteid = result.siteid;
                CustomerWizard.CustomerGeneralInfoId = result.id;
                CustomerWizard.SiteDetailId = result.siteid;



                if (result.length > 0) {
                    $("#GeneralInfoMsgDiv").empty(); // empty div
                    var ErrorList = "<ul style='list-style:none;'>"
                    $(result).each(function (i) {
                        ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
                    });
                    ErrorList = ErrorList + "</ul>"
                    $("#GeneralInfoMsgDiv").append(ErrorList);
                    $('#GeneralInfoMsgDiv').css('color', 'red');
                    $("#GeneralInfoMsgDiv").css("display", "block");
                }
                else {
                    $("#GeneralInfoMsgDiv").empty();
                    $("#CustomerGeneralInfoId").val(id);
                    $(".SiteDetailId").val(siteid);
                    $("#addCustomerWizard").modal("hide");
                    $("#AddContactWizard").modal("show");
                    $(".Customerinfoid").val(CustomerWizard.CustomerGeneralInfoId);
                    $(".wizardSiteid").val(siteid);
                    CustomerWizard.openCustomerContactWizard();
                }
            },
            error: function () {
                alert('something went wrong !');
            }
        });
        return false;
    });


    $('#savecustomersite').click(function () {
        $('#savecustomersite').prop('disabled', true);
        var $form = $("#formSiteDetail"); // form id

        //    var id = $('#SiteDetailId').val();
        //    var guidval = '00000000-0000-0000-0000-000000000000';


        //    $.post($form.attr("action"), $form.serialize(), function (result) {

        //        var id = result.id;

        var activetab = result.activetab;
        if (result.length > 0) {
            $("#CustomerSitesErrorDiv").empty(); // empty div
            var ErrorList = "<ul style='list-style:none;'>"
            $(result).each(function (i) {
                ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
            });
            ErrorList = ErrorList + "</ul>"
            $("#CustomerSitesErrorDiv").append(ErrorList);
            $('#CustomerSitesErrorDiv').css('color', 'red');
            $("#CustomerSitesErrorDiv").css("display", "block");
            $('#savecustomersite').prop('disabled', false);
        }
        else {
            $("#CustomerSitesErrorDiv").empty();
            $("#SiteDetailId").val(result.SiteId);
            CustomerWizard.WizardSiteId = result.SiteId;
            CustomerWizard.openCustomerContactWizard();
        }

    });

});

$("#backSaveCustomerSite").click(function (i, e) {

    $("#addCustomerWizard").modal("show");
    $("#AddSiteWizard").modal("hide");
    return false;
});




var umbrellachecked = $('#UmbrellaGroup').is(':checked');
if (umbrellachecked) {
    $('.note').css('display', 'block');
}
else {
    $('.note').css('display', 'none');
}

$('#IsActive').prop('checked', true);
CustomerWizard.CustomerSiteFilename = "";
CustomerWizard.CustomerSiteFilename = $("#Street").val() + " " + $("#StreetName").val() + " " + $("#Suburb").val() + " " + $("#State").val();

$("#SiteFileName").val(CustomerWizard.CustomerSiteFilename);


$("#StrataPlan").change(function () {
    var startaplan = $("#StrataPlan").val();
    if (startaplan != null && startaplan != 'undefined' && startaplan != '') {
        $("#SiteFileName").val(startaplan);
    }
    else {
        CustomerWizard.CustomerSiteFilename = $("#Street").val() + " " + $("#StreetName").val() + " " + $("#Suburb").val() + " " + $("#State").val() + " " + $("#PostalCode").val();
        $("#SiteFileName").val(CustomerWizard.CustomerSiteFilename);
    }
});

$(document).on('change', '#UmbrellaGroup', function () {
    var umbrellachecked = $('#UmbrellaGroup').is(':checked');
    if (umbrellachecked) {
        $('.note').css('display', 'block');
    }
    else {
        $('.note').css('display', 'none');
        $("#txtNote").val("");
    }
});


$("#Street").change(function () {
    CustomerWizard.CustomerSiteFilename = CustomerWizard.CustomerSiteFilename + $(this).val()
})
$("#StreetName").change(function () {
    CustomerWizard.CustomerSiteFilename = CustomerWizard.CustomerSiteFilename + $(this).val();
})



$("#Suburb").change(function () {

    CustomerWizard.CustomerSiteFilename = CustomerWizard.CustomerSiteFilename + $(this).val();
})


$("#State").change(function () { CustomerWizard.CustomerSiteFilename = CustomerWizard.CustomerSiteFilename + $(this).val(); })

$("#SiteFileName").click(function () {
    CustomerWizard.CustomerSiteFilename = "";
    var startaplan = $("#StrataPlan").val();
    if (startaplan != null && startaplan != 'undefined' && startaplan != '') {
        $(this).val(startaplan);
    }
    else {
        CustomerWizard.CustomerSiteFilename = $("#Street").val() + " " + $("#StreetName").val() + " " + $("#Suburb").val() + " " + $("#State").val() + " " + $("#PostalCode").val();
        $(this).val(CustomerWizard.CustomerSiteFilename);
    }
})

$(".closebtn").click(function () {
    window.location.reload();
})