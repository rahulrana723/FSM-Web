
var dropdown = 'nav li:has(ul)',
    dropdown_ul = 'nav li ul',
    nav_ul = 'nav > ul',
    nav_toggle = 'nav .nav-toggle',
    open_class = 'open',
    desktop_class = 'desktop',
    breakpoint = 992,
    anim_delay = 200;


function isDesktop() {
    return ($(window).width() > breakpoint);
}

$('#FSMDisplayUserStyle').on('click', function (event) {
    event.preventDefault();
    if ($('.main-dropdwn').is(':visible')) {
        $('.main-dropdwn').css('display', 'none');
    }
    else {
        $('.main-dropdwn').css('display', 'block');
    }

});

$(function () {
    $(document).click(function (e) {
        var target = $(e.target).parent();
        var target_ul = target.children('ul');

        if (!isDesktop()) {
            $(dropdown).not(target).removeClass(open_class);
            $(dropdown_ul).not(target_ul).slideUp(anim_delay);

            if (target.is(dropdown)) {
                target.toggleClass(open_class);
                target_ul.slideToggle(anim_delay);
            }

            if (target.is(nav_toggle)) {
                target.toggleClass(open_class);
                $(nav_ul).slideToggle(anim_delay);
            }
        }
    })

    $(window).resize(function () {
        $('body').toggleClass(desktop_class, isDesktop());

        if (isDesktop()) {
            $(dropdown).removeClass(open_class);
            $(dropdown_ul).hide();
            $(nav_toggle).addClass(open_class);
            $(nav_ul).show();
        }
    });

    $(window).resize();
});


$(document).ready(function () {
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/GetUnreadMessage",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            var response = jQuery.parseJSON(data);
            $(".msgcount").text(response);
        },
        error: function () {
            alert("something seems wrong");
        }
    })
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/GetProfilePics",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#my_image").attr("src", (common.SitePath + "/EmployeeImages/") + data);
        },
        error: function () {
            alert("something seems wrong");
        }
    })
});

$(document).on('click', '.fsm-notification', function () {
    
    $('.div-notification').empty();
    $('.lbl-notify').text('');
    var url = common.SitePath + "/Admin/Dashboard/GetNotifications"
    var UpdateUrl = common.SitePath + "/Employee/Employee/ApproveEmployee?EmployeeTempId=";
    var PurchaseOrderUrl = common.SitePath + "/Employee/Purchase/AddEditJobPurchaseOrder?Purchaseorderid=";
    var InvoiceUrl = common.SitePath + "/Employee/Invoice/SaveInvoiceInfo?id=";
    var notificationhtml = "<table class='table table-bordered table-striped table-condensed table-responsive'>";

    $.get(url, function (data) {
        var looplength = data.notificationList.length;
        var notifications = data.notificationList;

        for (var i = 0; i < looplength; i++) {
            if (notifications[i].NotificationType == "EmployeeProfile") {
                notificationhtml = notificationhtml + "<tr><td><a target='_blank' href='" + UpdateUrl
                    + notifications[i].NotificationTypeId + "&Module=Employee List'>" + notifications[i].NotificationMessage + "</td></a></tr>";
            }
            else if (notifications[i].NotificationType == "PurchaseOrder") {
                notificationhtml = notificationhtml + "<tr><td><a target='_blank' href='" + PurchaseOrderUrl
                    + notifications[i].NotificationTypeId + "'>" + notifications[i].NotificationMessage + "</td></a></tr>";
            }
            else if (notifications[i].NotificationType == "Invoice") {
                notificationhtml = notificationhtml + "<tr><td><a target='_blank' href='" + InvoiceUrl
                    + notifications[i].NotificationTypeId +"&Module=Employee List'>" + notifications[i].NotificationMessage + "</td></a></tr>";
            }
            else {
                notificationhtml = notificationhtml + "<tr><td>" + notifications[i].NotificationMessage + "</td></tr>";
            }

        }
        notificationhtml = notificationhtml + "</table>";
        $('.div-notification').html(notificationhtml);
    });

    $('#modalNotifications').modal("show");

});

$(document).on("click", ".btnPopCLoseOk", function () {
    $(".modalWarningMsg").modal('hide');
});

