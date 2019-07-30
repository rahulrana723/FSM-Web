

var CustomerSite = {
    CustomerSiteFilename: "",
    UpdateSiteFilename:function()
    {
       
        CustomerSite.CustomerSiteFilename = $("#CustomerSiteDetailViewModel_Street").val() + " " + $("#CustomerSiteDetailViewModel_StreetName").val() + " " + $("#CustomerSiteDetailViewModel.Suburb").val() + " " + $("#CustomerSiteDetailViewModel_State").val() + " " + $("#CustomerSiteDetailViewModel_PostalCode").val();
        $("#SiteFileName").val(CustomerSite.CustomerSiteFilename);
        $("#CustomerSiteDetailViewModel_SiteFileName").val(CustomerSite.CustomerSiteFilename);
    }
}
$('#cancelcustomersite').on('click', function () {
    $('#dvCustAddNew').css('display', 'block');
    $('#dvCustForm').css('display', 'none');
});

$('#savecustomersite').click(function () {
    $('#savecustomersite').prop('disabled', true);
    var $form = $("#formSiteDetail"); // form id

    var id = $('#SiteDetailId').val();
    var guidval = '00000000-0000-0000-0000-000000000000';
    var height = $("#CustomerResidenceDetailViewModel_Height").val();
    var sras = $("#CustomerResidenceDetailViewModel_SRASinstalled").val();
    if (parseInt(height) >= 4 && parseInt(sras) == 0) {
        alert("Please add note that SRAS needs to be quoted when onsite.");
    }
 
    if (id == "" || id == undefined || id == guidval) {
        $.post($form.attr("action"), $form.serialize(), function (result) {

            var id = result.id;
            var activetab = result.activetab;
            if (result.length > 0) {
                $("#CustomerSitesErrorDiv").empty(); // empty div
                var ErrorList = "<ul style='list-style:none;'>"
                $(result).each(function (i) {
                    ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
                });
                ErrorList = ErrorList + "</ul>"
                $('#savecustomersite').prop('disabled', false);
                $(window).scrollTop(0);
                $("#CustomerSitesErrorDiv").append(ErrorList);
                $('#CustomerSitesErrorDiv').css('color', 'red');
                $("#CustomerSitesErrorDiv").css("display", "block");
                $('#savecustomersite').prop('disabled', false);
            }
            else {
                $("#CustomerSitesErrorDiv").empty();
                $(window).scrollTop(0);
                $('.jobalert').empty();
                $('.jobalert').css('color', 'green');
                $('.jobalert').html('<Strong>Record saved successfully!</Strong>');
                $('.jobalert').show();
                $("#savecustomersite").attr("disabled", "disabled");
                window.setTimeout(function () {
                    $('.jobalert').hide();
                    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
                }, 4000)
            }
        });
    }
    else {
        $.post(common.SitePath + "/Customer/Customer/EditCustomerList", $form.serialize(), function (result) {

            var id = result.id;
            var activetab = result.activetab;
            if (result.length > 0) {
                $("#CustomerSitesErrorDiv").empty(); // empty div
                var ErrorList = "<ul style='list-style:none;'>"
                $(result).each(function (i) {
                    ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
                });
                ErrorList = ErrorList + "</ul>"
                $('#savecustomersite').prop('disabled', false);
                $(window).scrollTop(0);
                $("#CustomerSitesErrorDiv").append(ErrorList);
                $('#CustomerSitesErrorDiv').css('color', 'red');
                $("#CustomerSitesErrorDiv").css("display", "block");
            }
            else {
                $("#CustomerSitesErrorDiv").empty();
                $(window).scrollTop(0);
                $('.jobalert').empty();
                $('.jobalert').css('color', 'green');
                $('.jobalert').html('<Strong>Record saved successfully!</Strong>');
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
                }, 2000)

            }
        });
    }
});

$(document).ready(function () {
    var currentVal = $("#siteContracted").val();
    if (currentVal != 0) {
        $(".priceShow").show();
    }
    else {
        $(".priceShow").hide();
    }
    var status = $('#BlackListed').is(':checked');
    if (status) {
        $('div.blacklist').show();
    }
    else {
        $('div.blacklist').hide();
    }
    var startaplan = $("#CustomerSiteDetailViewModel_StrataPlan").val();
    if (startaplan != null && startaplan != 'undefined' && startaplan != '') {
        $("#SiteFileName").val(startaplan);
        $("#CustomerSiteDetailViewModel_SiteFileName").val(startaplan);
    }
   else{
        CustomerSite.CustomerSiteFilename = "";
        CustomerSite.CustomerSiteFilename = $("#CustomerSiteDetailViewModel_Street").val() + " " + $("#CustomerSiteDetailViewModel_StreetName").val() + " " + $("#CustomerSiteDetailViewModel_Suburb").val() + " " + $("#CustomerSiteDetailViewModel_State").val() + " " + $("#CustomerSiteDetailViewModel_PostalCode").val();
        $("#SiteFileName").val(CustomerSite.CustomerSiteFilename);
    }
});


$("#CustomerSiteDetailViewModel_StrataPlan").change(function () {
    
    var startaplan = $("#CustomerSiteDetailViewModel_StrataPlan").val();
    if (startaplan != null && startaplan != 'undefined' && startaplan != '') {
        $("#SiteFileName").val(startaplan);
        $("#CustomerSiteDetailViewModel_SiteFileName").val(startaplan);
    }
    else {
        CustomerSite.CustomerSiteFilename = $("#CustomerSiteDetailViewModel_Street").val() + " " + $("#CustomerSiteDetailViewModel_StreetName").val() + " " + $("#CustomerSiteDetailViewModel.Suburb").val() + " " + $("#CustomerSiteDetailViewModel_State").val() + " " + $("#CustomerSiteDetailViewModel_PostalCode").val();
        $("#SiteFileName").val(CustomerSite.CustomerSiteFilename);
        $("#CustomerSiteDetailViewModel_SiteFileName").val(CustomerSite.CustomerSiteFilename);
    }
});


$("#SiteFileName ,.SiteFileName").click(function () {
    var startaplan = $("#CustomerSiteDetailViewModel_StrataPlan").val();
    if (startaplan != null && startaplan != 'undefined' && startaplan != '') {
        $("#SiteFileName").val(startaplan);
        $("#CustomerSiteDetailViewModel_SiteFileName").val(startaplan);
    }
    else {
        CustomerSite.CustomerSiteFilename = "";
        CustomerSite.CustomerSiteFilename = $("#CustomerSiteDetailViewModel_Street").val() + " " + $("#CustomerSiteDetailViewModel_StreetName").val() + " " + $("#CustomerSiteDetailViewModel_Suburb").val() + " " + $("#CustomerSiteDetailViewModel_State").val() + " " + $("#CustomerSiteDetailViewModel_PostalCode").val();
        $(this).val(CustomerSite.CustomerSiteFilename);
        $("#CustomerSiteDetailViewModel_SiteFileName").val(CustomerSite.CustomerSiteFilename);
    }
})


$("#CustomerSiteDetailViewModel_Street , #CustomerSiteDetailViewModel_StreetName,#CustomerSiteDetailViewModel_Suburb,#CustomerSiteDetailViewModel_State,#CustomerSiteDetailViewModel_PostalCode").change(function () {

    CustomerSite.UpdateSiteFilename();
})


$('#savecustomersiteForJob').on('click', function () {
    var $form = $("#formSiteDetail"); // form id
    var height = $("#CustomerResidenceDetailViewModel_Height").val();
    var sras = $("#CustomerResidenceDetailViewModel_SRASinstalled").val();
    if (parseInt(height) >= 4 && parseInt(sras) == 0) {
        alert("Please add note that SRAS needs to be quoted when onsite.");

    }

    var formdata = new FormData($('#formSiteDetail').get(0));
    $.ajax({
        url: $('#formSiteDetail').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.status == "saved") {
                $("#CustomerSitesErrorDiv").empty();
                $(window).scrollTop(0);
                $('.jobalert').empty();
                $('.jobalert').css('color', 'green');
                $('.jobalert').html(result.msg);
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                }, 4000)
            }
            else {
                $(window).scrollTop(0);
                $('.jobalert').empty();

                var ErrorList = "<ul style='list-style:none;'>"
                $(result.errors).each(function (i) {
                    ErrorList = ErrorList + "<li>" + result.errors[i].ErrorMessage + "</li>";
                });
                ErrorList = ErrorList + "</ul>"
                $(window).scrollTop(0);
                $('#CustomerSitesErrorDiv').css('color', 'red');
                $('#CustomerSitesErrorDiv').html(ErrorList);
                $('#CustomerSitesErrorDiv').show();
            }
        }
    });
});
$('#siteContracted').change(function () {
    var currentVal = $("#siteContracted").val();
    if (currentVal != 0) {
        $(".priceShow").show();
    }
    else {
        $(".priceShow").hide();
        $("#schdulePrice").val("");
    }
});

$("#cancelcustomersiteForJob").on("click", function (e) {
    var id = FSM.JobId; // form id
    window.location.href = common.SitePath + '/Employee/CustomerJob/SaveJobInfo?id=' + id + '&activetab=Site Detail'
});
$(document).on("change", "#BlackListed", function () {
    var a = $("#BlackListReason").val();
    var status = $(this).prop('checked');
    if (status) {
        $("div.blacklist").css("display", "block");
    }
    else {
        $("div.blacklist").css("display", "none");
        $("#BlackListReason").val("");
    }
});