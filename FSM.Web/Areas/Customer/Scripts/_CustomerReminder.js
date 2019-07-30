$("#btnReminderSaveSend").click(function (e) {
    
    var isValid = true;
    var ReminderID = $("#reminderId").val();
    var ReminderDate = $('#ReminderDate').val();
    var JobNo = $("#RemJobId").val();
    var ContactListIds = $('#sitecontacts').val();
    var MessageTypeId = $("#MessageTypeId").val();
    var TemplateMessageId = $("#TemplateMessageId").val();
    var Notes = $(".mynotebook").val();
    var HasSMS = $('#HasSMS').is(':checked');
    var HasEmail = $("#HasEmail").is(':checked');
    var schedule = $("#Schedule").is(':checked');
    var fromEmail = $("#FromEmail option:selected").text();
    var fromEmailVal = $("#FromEmail option:selected").val();
    if (fromEmailVal == "" || fromEmail == "(From Email)") {
        fromEmail = "invoicing@sydneyroofandgutter.com.au";
        fromEmailVal = "0";
    }
    var jobids = [];
    $.each($("#RemJobId option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            jobids.push(id);
        }
    });
  
    if (ReminderDate == "" && schedule == true) {
      
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please select reminder date</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }

    if (jobids.length == 0) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please select atleast one job</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (ContactListIds.length == 0) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please select atleast one contact</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (TemplateMessageId == '0' || TemplateMessageId == '' || TemplateMessageId == undefined) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please select atleast one template</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
        //if (HasSMS == false && HasEmail == false) {
        //    $("#ContactLogErrorDv").empty();
        //    $("#ContactLogErrorDv").css("display", "block");
        //    $('#ContactLogErrorDv').css('color', 'red');
        //    $("#ContactLogErrorDv").html("<strong>Please Check atleast One option from SMS or Email</strong>");
        //    $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        //    });
        //    return false;
        //}
        //if (MessageTypeId == false) {
        //    $("#ContactLogErrorDv").empty();
        //    $("#ContactLogErrorDv").css("display", "block");
        //    $('#ContactLogErrorDv').css('color', 'red');
        //    $("#ContactLogErrorDv").html("<strong>Please Select Message Type</strong>");
        //    $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        //    });
        //    return false;
        //}
        //if (HasSMS == false && HasEmail == false) {
        //    $("#ContactLogErrorDv").empty();
        //    $("#ContactLogErrorDv").css("display", "block");
        //    $('#ContactLogErrorDv').css('color', 'red');
        //    $("#ContactLogErrorDv").html("<strong>Please Check atleast One option from SMS or Email</strong>");
        //    $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        //    });
        //    return false;
        //}
    if (isValid == false) {
        e.preventDefault();
    }
    else {
        $(this).text("Sending...");
        $.ajax({
            data: {
                ReminderDate: ReminderDate, ReminderId: ReminderID, JobId: JobNo, ContactListIds: ContactListIds, TempMsgId: TemplateMessageId, Note: Notes, HasSMS: HasSMS, HasEmail: HasEmail, hasSchedule: schedule, fromEmail: fromEmail, fromEmailVal: fromEmailVal
        },
                url: common.SitePath + '/Customer/Customer/CustomerSendReminder',
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
                        window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + response.success + '&activetab=Contact'
                    }, 2000)
                } else {
                    $(".closebtncontact").trigger('click');
                }
        },
                error: function (response) {
                alert("error!");  //
        }
        });
    }

});



//$("#btnPreviewCustomer").on("click", function (event) {
//        event.preventDefault();
//        var href = common.SitePath + "/Customer/Customer/CustomerReminderExportPreview?";

//        var CustomerID = $("#CustomerId").val();
//        var JobId = $("#RemJobId").val();
//        var SiteID = $("#SiteId").val();
//        var LogDate = $("#LogDate").val();
//        var ReContactDate = $("#ReContactDate").val();

//        if (LogDate > ReContactDate && ReContactDate != "") {
//            $("#ContactLogErrorDv").empty();
//            $(window).scrollTop(0);
//            $("#ContactLogErrorDv").css("display", "block");
//            $('#ContactLogErrorDv').css('color', 'red');
//            $("#ContactLogErrorDv").html("<strong>Log date can't be greater than ReContact date !</strong>");
//            $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
//            });
//            return false;
//        }
//        var Note = $("#note").val();
//        if (Note == "") {
//            $("#ContactLogErrorDv").empty();
//            $("#ContactLogErrorDv").css("display", "block");
//            $('#ContactLogErrorDv').css('color', 'red');
//            $("#ContactLogErrorDv").html("<strong>Note field is required !</strong>");
//            $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
//            });
//            return false;
//        }


//        if (CustomerID != undefined && CustomerID != "" && href.indexOf("CustomerId") < 0) {
//            href = href + 'CustomerId=' + CustomerID + "&";
//        }
//        if (JobId != undefined && JobId != "" && href.indexOf("JobId") < 0) {
//            href = href + 'JobId=' + JobId + "&";
//        }
//        if (SiteID != undefined && SiteID != "" && href.indexOf("SiteId") < 0) {
//            href = href + 'SiteId=' + SiteID + "&";
//        }
//        if (LogDate != undefined && LogDate != "" && href.indexOf("LogDate") < 0) {
//            href = href + 'LogDate=' + LogDate + "&";
//        }
//        if (ReContactDate != undefined && ReContactDate != "" && href.indexOf("ReContactDate") < 0) {
//            href = href + 'ReContactDate=' + ReContactDate + "&";
//        }

//        if (Note != undefined && Note != "" && href.indexOf("Note") < 0) {
//            href = href + 'Note=' + Note + "&";
//        }

//        $(this).attr("href", href);
//        window.location.href = href;

//    });

//schedule check box true then show reminder date field
$(":checkbox").change(function () {
    var schedule = $("#Schedule").is(':checked');
    if (schedule == true) {
        $(".reminderDateDv").show();
        $("#btnReminderSaveSend").text("Schedule Reminder");
    } else {
        $(".reminderDateDv").hide();
        $("#btnReminderSaveSend").text("Send Reminder");
    }
});

$('#RemJobId').on('change', function () {
    var arrJobIds = $(this).val();
    $.ajax({
        url: common.SitePath + '/Customer/Customer/_GetJobContacts',
        data: {
            jobIds: arrJobIds
        },
        type: 'POST',
        async: false,
        success: function (response) {
            $('.jobContactList').empty();
            $('.jobContactList').html(response);
        },
        error: function (response) {
            alert("error!");  //
        }
    });

});

$("#TemplateMessageId").change(function () {
    var tempmessageId = $("#TemplateMessageId option:selected").val();
    var templateName = getTemplateName(tempmessageId);

    // read template
    if (templateName != '') {
        $('.mynotebook').html('')
        $.ajax({
            url: common.SitePath + '/Customer/Customer/GetMessageTemplate',
            data: { templateName: templateName },
            type: 'GET',
            dataType: 'json',
            async: false,
            success: function (response) {
               // var mydata = response.msgTemplate;
                //mydata = mydata.replace(/<p>/g, '&#13;');
                //mydata = mydata.replace(/<\/p>/g, '&#10;');
                //mydata = mydata.replace(/<p>/g, '\n');
                //mydata = mydata.replace(/<\/p>/g, '\n');
                var mydata = response.msgTemplate;
                debugger;
                mydata = mydata.replace(/<p>/g, '&#13;');
                mydata = mydata.replace(/<\/p>/g, '&#10;');
                $('.mynotebook').html(mydata)

                //$('#Note').html(mydata)
                
                //$('.mynotebook').html(mydata)
             //   $('.mynotebook').get(0).value = mydata;
               // $('#Note').html(mydata)
            },
            error: function (response) {
                alert("error!");  //
            }
        });
        }
else {
        $('.mynotebook').html('')
}
});

function getTemplateName(tempmessageId) {
    var templateName = '';
    switch (tempmessageId) {
        case '1' :
            templateName = 'ConfirmationAppointmentStrataRealestate';
            break;
        case '2':
            templateName = 'ConfirmationAppointmentDomesticCustomer';
            break;
        case '3':
            templateName = 'DueToRainJobPostponed';
            break;
        case '4':
            templateName = 'UnavailableOTRWDueToBadHealth';
            break;
        case '5':
            templateName = 'ContractedGutterCleanPriceIncrease';
            break;
        case '6':
            templateName = 'ContractedGutterCleanSRASNeeded';
            break;
        case '7':
            templateName = 'CustomerReminder';
            break;
        case '8':
            templateName = 'ReminderForRain';
            break;
        case '9':
            templateName = 'ReminderForSick';
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
        $("#btnReminderSaveSend").text("Schedule Reminder");
    }


    var tempmessage = $("#TemplateMessageId option:selected").val();

    $('.ddlmultiselectmsgcustomer')
   .multiselect({
       maxHeight: 200,
       includeSelectAllOption: true,
           nonSelectedText: '(select)',
       numberDisplayed: 1,
    })
   .multiselect('updateButtonText');
    $('#ReminderDate').datepicker({
        minDate: 0,
            dateFormat: 'dd/mm/yy'
    });
})