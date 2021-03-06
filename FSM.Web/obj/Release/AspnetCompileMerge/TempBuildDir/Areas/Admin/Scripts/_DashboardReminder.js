﻿$("#btnPreview").on("click", function (event) {
    event.preventDefault();
    var href = common.SitePath + "/Admin/Dashboard/DashboardReminderExportPreview?";

    var CustomerID = $("#CustomerId").val();
    var JobId = $("#_JobId").val();
    var SiteID = $("#SiteId").val();
    var LogDate = $("#LogDate").val();
    var ReContactDate = $("#ReContactDate").val();

    if (LogDate > ReContactDate && ReContactDate != "") {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Log date can't be greater than ReContact date !</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    var Note = $("#note").val();
    if (Note == "") {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Note field is required !</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }


    if (CustomerID != undefined && CustomerID != "" && href.indexOf("CustomerId") < 0) {
        href = href + 'CustomerId=' + CustomerID + "&";
    }
    if (JobId != undefined && JobId != "" && href.indexOf("JobId") < 0) {
        href = href + 'JobId=' + JobId + "&";
    }
    if (SiteID != undefined && SiteID != "" && href.indexOf("SiteId") < 0) {
        href = href + 'SiteId=' + SiteID + "&";
    }
    if (LogDate != undefined && LogDate != "" && href.indexOf("LogDate") < 0) {
        href = href + 'LogDate=' + LogDate + "&";
    }
    if (ReContactDate != undefined && ReContactDate != "" && href.indexOf("ReContactDate") < 0) {
        href = href + 'ReContactDate=' + ReContactDate + "&";
    }

    if (Note != undefined && Note != "" && href.indexOf("Note") < 0) {
        href = href + 'Note=' + Note + "&";
    }

    $(this).attr("href", href);
    window.location.href = href;

});


$("#btnSaveSend").click(function (e) {
    var isValid = true;
    var ReminderDate = $("#ReminderDate").val();
    var JobId = $("#_JobId").val();
    var MessageTypeId = $("#MessageTypeId").val();
    var TemplateMessageId = $("#TemplateMessageId").val();
    var Notes = $("#Note").val();
    var HasSMS = $('#HasSMS').is(':checked');
    var HasEmail = $("#HasEmail").is(':checked');

    var jobids = [];
    $.each($("#dashboardmsg option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            jobids.push(id);
        }
    });


    //if ($("#_JobId").val() == "") {
    //    $("#ContactLogErrorDv").empty();
    //    $("#ContactLogErrorDv").css("display", "block");
    //    $('#ContactLogErrorDv').css('color', 'red');
    //    $("#ContactLogErrorDv").html("<strong>Please select atleast one job</strong>");
    //    $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
    //    });
    //    return false;
    //}


    //if ($("#_JobId").val() == "") {
    //    $("#ContactLogErrorDv").empty();
    //    $("#ContactLogErrorDv").css("display", "block");
    //    $('#ContactLogErrorDv').css('color', 'red');
    //    $("#ContactLogErrorDv").html("<strong>Please select atleast one job</strong>");
    //    $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
    //    });
    //    return false;
    //}
    if (jobids.length == 0) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please select atleast one job</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (MessageTypeId == false) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please Select Message Type</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (HasSMS == false && HasEmail == false) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please Check atleast One option from SMS or Email</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (isValid == false) {
        e.preventDefault();
    }
    else {
        $(this).text("Sending...");
        $.ajax({
            url: common.SitePath + '/Admin/Dashboard/SaveSendReminder',
            data: { ReminderDate: ReminderDate, JobId: jobids, MessageTypeId: MessageTypeId, TemplateMessageId: TemplateMessageId, Note: Notes, HasSMS: HasSMS, HasEmail: HasEmail },
            type: 'POST',
            dataType: 'json',
            async: true,
            success: function (response) {
                if (response != null && response.success) {
                    $('.saved-rem-msg').css('color', 'green');
                    $('.saved-rem-msg').html("Reminder Saved & Sent Successfully!!");
                    $('.saved-rem-msg').show();

                    window.setTimeout(function () {
                        $(".closebtncontact").trigger('click');
                        $('.btnaddRem').text("Send Messages");
                    }, 2000)
                } else {
                    $(".closebtncontact").trigger('click');
                    $('.btnaddRem').text("Send Messages");
                }
            },
            error: function (response) {
                alert("error!");  //
            }
        });
    }

});

$(".closebtncontact").click(function (e) {
    $('.btnaddRem').text("Send Messages");
});


$("#btnSave").click(function (e) {
    var isValid = true;
    var Custid = $("#CustomerId").val();
    var JobId = $("#Job_Id").val();
    var LogDate = $("#LogDate").val();
    var ReContactdate = $("#ReContactDate").val();
    var Notes = $("#note").val();
    if ($("#CustomerId").val() == "") {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please select Customer Id</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (isValid == false) {
        e.preventDefault();
    }
    else {
        $.ajax({
            url: common.SitePath + '/Admin/Dashboard/SaveReminder',
            data: { CustomerId: Custid, JobId: JobId, Logdate: LogDate, ReContactdate: ReContactdate, Note: Notes },
            type: 'POST',
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response != null && response.success) {
                    $('.saved-rem-msg').css('color', 'green');
                    $('.saved-rem-msg').html("Reminder Saved Successfully!!!");
                    $('.saved-rem-msg').show();
                    window.setTimeout(function () {
                        $(".closebtncontact").trigger('click');
                    }, 2000)
                } else {
                    $(".closebtncontact ").trigger('click');
                }
            },
            error: function (response) {
                alert("error!");  //
            }
        });
    }
});

$("#TemplateMessageId").change(function () {
    var tempmessage = $("#TemplateMessageId option:selected").val();
    if (tempmessage == "") {
       $('#Note').removeAttr('readonly');
    }
    else {
         $('#Note').attr('readonly', 'true');
    }

    //$("#Note").val(tempmessage);
});


$(document).ready(function () {

    $('.ddlmultiselectmsgdashboard').multiselect({

        maxHeight: 200,
        nonSelectedText: '(Select)',
        includeSelectAllOption: true,
    });

})
