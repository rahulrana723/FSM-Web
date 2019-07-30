
$(document).ready(function () {
    if (FSM.Success == "ok") {
        $(".jobalert").empty();
        $(".jobalert").append("<strong>Record Saved Successfully!</strong>");
        $('.jobalert').css('color', 'green');
        $('.jobalert').show();
        window.setTimeout(function () {
            $('.jobalert').hide();
        }, 4000)
        FSM.Success = null;
    }
});

$(".btnaddRem").click(function () {
    var customerGeneralinfoid = $(this).attr("customergeneralinfoid");
    $.ajax({
        url: common.SitePath + "/customer/Customer/_CustomerReminderCreate",
        data: { CustomerGeneralinfoid: customerGeneralinfoid },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divRemPopup").html(data);
            $("#modalReminder").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).on("change", "#JobId", function () {
    var jobId = $(this).val();
    $.ajax({
        url: common.SitePath + "/customer/customer/GetCustomerSiteByJobid",
        data: { jobid: jobId },
        type: 'POST',
        async: false,
        success: function (data) {
            $("#SiteId").val(data.siteid);
            $("#SiteId").prop("disabled", true);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});



$(document).on('click', '.grid-pager .pagination a', function (event) {
    event.preventDefault();
    var querystring = $(this).prop('href').split("?")[1];
    var values = querystring.split("&");
    var pagenumparam = '';

    $.each(values, function (i, item) {
        if (item.indexOf("grid-page") > -1) {
            pagenumparam = item;
        }
    })

$.get(common.SitePath + "/Customer/Customer/ViewCustomerContactLogPartial?customerGeneralinfoid=" + FSM.CustomerGeneralInfoId + "&" + pagenumparam, function (data) {
        $('#divContactPartial').empty();
        $('#divContactPartial').append(data);
    });

});


$(".btnaddlog").click(function () {
    var customerGeneralinfoid = $(this).attr("customerGeneralinfoid");
    $.ajax({
        url: common.SitePath + "/customer/Customer/_CustomercontactAddEdit",
        data: { CustomerGeneralinfoid: customerGeneralinfoid },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowPopup").html(data);
            $("#modalContactlog").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});



