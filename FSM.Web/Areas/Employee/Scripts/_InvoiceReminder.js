$("#btnSaveSend").click(function (e) {
    var isValid = true;
    var reminderId = $("#reminderId").val();
    var reminderDate = $('#ReminderDate').val();
    var jobId = $("#GetJobId").val();
    var contactListIds = $('#sitecontacts').val();
    var messageTypeId = $("#MessageTypeId").val();
    var templateMessageId = $("#TemplateMessageId").val();
    var notes = $(".mynotebook").val();
    var hasSms = $("#HasSMS").is(":checked");
    var hasEmail = $("#HasEmail").is(":checked");
    var schedule = $("#Schedule").is(':checked');
    var fromEmail = $("#FromEmail option:selected").text();
    var fromEmailVal = $("#FromEmail option:selected").val();
    if (fromEmailVal == "" || fromEmail == "(From Email)") {
        fromEmail = "invoicing@sydneyroofandgutter.com.au";
        fromEmailVal = "0";
    }
    var jobids = [];
    $.each($("#JobId option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            jobids.push(id);
        }
    });

    if (ReminderDate == "" && schedule == true) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css("color", "red");
        $("#ContactLogErrorDv").html("<strong>Please select reminder date</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }

    if (contactListIds.length == 0) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css("color", "red");
        $("#ContactLogErrorDv").html("<strong>Please select atleast one contact</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (TemplateMessageId == 0 || TemplateMessageId == "" || TemplateMessageId == undefined) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css("color", "red");
        $("#ContactLogErrorDv").html("<strong>Please select atleast one template</strong>");
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
            data: {
                ReminderDate: reminderDate, ReminderId: reminderId, JobId: jobId, ContactListIds: contactListIds, TempMsgId: templateMessageId, Note: notes, HasSMS: hasSms, HasEmail: hasEmail, hasSchedule: schedule, fromEmail: fromEmail, fromEmailVal: fromEmailVal
            },
            url: common.SitePath + "/Employee/Invoice/InvoiceSendReminder",
            type: "POST",
            dataType: "json",
            async: true,
            success: function (response) {
                if (response != null && response.success) {
                    $(".saved-rem-msg").css("color", "green");
                    $(".saved-rem-msg").html("Reminder Saved & Sent Successfully!!");
                    $(".saved-rem-msg").show();
                    window.setTimeout(function() {
                            $(".closebtncontact").trigger("click");
                            window.location.href = common.SitePath + "/Employee/Invoice/SaveInvoiceInfo?id=" + response.success + "&activetab=Contact Log"}, 2000);
                } else {
                    $(".closebtncontact").trigger("click");
                }
            },
            error: function (response) {
                alert("error!"); 
            }
        });
    }

});

//schedule check box true then show reminder date field
$(":checkbox").change(function () {
    var schedule = $("#Schedule").is(':checked');
    if (schedule == true) {
        $(".reminderDateDv").show();
        $("#btnSaveSend").text("Schedule Reminder");
    } else {
        $(".reminderDateDv").hide();
        $("#btnSaveSend").text("Send Reminder");
    }
});

$("#TemplateMessageId").change(function () {
    var tempmessageId = $("#TemplateMessageId option:selected").val();
    var templateName = getTemplateName(tempmessageId);

    // read template
    if (templateName != "") {
        $(".mynotebook").html("");
        $.ajax({
            url: common.SitePath + "/Customer/Customer/GetMessageTemplate",
            data: { templateName: templateName },
            type: "GET",
            dataType: "json",
            async: false,
            success: function (response) {
                var mydata = response.msgTemplate;
                mydata = mydata.replace(/<p>/g, '&#13;');
                mydata = mydata.replace(/<\/p>/g, '&#10;');
                //mydata = mydata.replace(/<p>/g, "\n");
                //mydata = mydata.replace(/<\/p>/g, "\n");

                $('.mynotebook').html(mydata)
              //  $(".mynotebook").get(0).value = mydata;
            },
            error: function (response) {
                alert("error!");  //
            }
        });
    }
    else {
        $(".mynotebook").html("");
    }
});

function getTemplateName(tempmessageId) {
    var templateName = "";
    switch (tempmessageId) {
        case "1":
            templateName = "ConfirmationAppointmentStrataRealestate";
            break;
        case "2":
            templateName = "ConfirmationAppointmentDomesticCustomer";
            break;
        case "3":
            templateName = "DueToRainJobPostponed";
            break;
        case "4":
            templateName = "UnavailableOTRWDueToBadHealth";
            break;
        case "5":
            templateName = "ContractedGutterCleanPriceIncrease";
            break;
        case "6":
            templateName = "ContractedGutterCleanSRASNeeded";
            break;
        case "7":
            templateName = "CustomerReminder";
            break;
        case "8":
            templateName = "ReminderForRain";
            break;
        case "9":
            templateName = "ReminderForSick";
            break;
        default:
    }
    return templateName;
}

$(document).ready(function () {
    //schedule is click then reminder date column show 
    var schedule = $("#Schedule").is(':checked');
    if (schedule == true) {
        $(".reminderDateDv").show();
        $("#btnSaveSend").text("Schedule Reminder");
    }

    var tempmessage = $("#TemplateMessageId option:selected").val();
    $(".ddlmultiselectsitecontacts")
        .multiselect({
            maxHeight: 200
            //includeSelectAllOption: true,
            //nonSelectedText: '(select)',
            //numberDisplayed: 1,
        })
        .multiselect("updateButtonText");

    $(".ReminderDatePicker").datepicker({
        minDate: 0,
        dateFormat: "dd/mm/yy"
    });
})