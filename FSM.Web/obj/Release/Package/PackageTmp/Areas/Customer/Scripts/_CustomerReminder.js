$("#btnReminderSaveSend").click(function (e) {
    var isValid = true;
    var ReminderID = $("#reminderId").val();
    var JobNo = $("#RemJobId").val();
    var MessageTypeId = $("#MessageTypeId").val();
    var TemplateMessageId = $("#TemplateMessageId").val();
    var Notes = $("#Note").val();
    var HasSMS = $('#HasSMS').is(':checked');
    var HasEmail = $("#HasEmail").is(':checked');
    var jobids = [];
    $.each($("#RemJobId option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            jobids.push(id);
        }
    });
    //if (JobNo == "") {
    //    $("#ContactLogErrorDv").empty();
    //    $("#ContactLogErrorDv").css("display", "block");
    //    $('#ContactLogErrorDv').css('color', 'red');
    //    $("#ContactLogErrorDv").html("<strong>Please select atleast 1 job</strong>");
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
    if (HasSMS == false && HasEmail == false) {
        $("#ContactLogErrorDv").empty();
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Please Check atleast One option from SMS or Email</strong>");
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
            data: { ReminderId: ReminderID, JobId: JobNo, MsgTypeId: MessageTypeId, MessageTypeId: MessageTypeId, TempMsgId: TemplateMessageId, TemplateMessageId: TemplateMessageId, Note: Notes, HasSMS: HasSMS, HasEmail: HasEmail },
            url: common.SitePath + '/Customer/Customer/CustomerSendReminder',
            type: 'POST',
            dataType: 'json',
            async: true,
            success: function (response) {
                debugger
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
                debugger
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
$(document).ready(function () {
    var tempmessage = $("#TemplateMessageId option:selected").val();
     if (tempmessage == "") {
         $('#Note').removeAttr('readonly');
     }
     else {
          $('#Note').attr('readonly', 'true');
     }

    $('.ddlmultiselectmsgcustomer')
   .multiselect({
       maxHeight: 200,
       includeSelectAllOption: true,
       nonSelectedText: '(select)',
   })
   .multiselect('updateButtonText');
})