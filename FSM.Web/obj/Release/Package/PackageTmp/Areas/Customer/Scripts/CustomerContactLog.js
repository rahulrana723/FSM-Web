
$(document).ready(function () {
    var tempmessage = $("#TemplateMessageId option:selected").val();
    if (tempmessage == "") {
        $('#Note').removeAttr('readonly');
    }
    else {
        $('#Note').attr('readonly', 'true');
    }

    $('#LogDate,#ReContactDate').datepicker({
        dateFormat: 'dd/mm/yy',
        minDate: 0
    });
    $("#JobId").attr('maxlength', '10');
    $("#Note").attr('maxlength', '500');
});

//checking validation before  partial form submitted
//$('#form0').submit(function () {
//    var jobid = $("#JobId").val();
//    // Check if empty of not
//    if (jobid == "") {
//        $(".errorjob").text("*Please provide Job Id");
//        return false;
//    }
//    else {
//        $(".errorjob").text("");
//    }
//});

//validation for maximum length
$('#JobId').keyup(function (e) {
    var limitNum = $(this).attr("maxlength");
    if ($(this).val().length > limitNum) {
        $(".errorjob").text("*Maximum length (1o char) exceed");
        return false;
    }
    else {
        $(".errorjob").text("");
    }
});

//valiudation for special character for job id
$('#JobId').keypress(function (e) {
    var regex = new RegExp("^[a-zA-Z0-9]+$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (regex.test(str)) {
        return true;
    }
    e.preventDefault();
    return false;
});
$(document).on("change", ".ddljob", function () {
    var job_id = $(this).val();
    if (job_id != "" && job_id != 'undefined' && job_id != '0') {

        $.ajax({
            url: common.SitePath + "/customer/customer/GetCustomerSiteByJobid",
            data: {
                jobid: job_id
            },
            type: 'POST',
            async: false,
            success: function (data) {
                $(".custsitename").val(data.sitename);
                $(".custsiteid").val(data.siteid);
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    }
});

$("#btnPreview").click(function () {
    var CustomerID = $("#CustomerId").val();
    var JobID = $("#JobId").val();
    var SiteID = $("#SiteId").val();
    var LogDate = $("#LogDate").val();
    var ReContactDate = $("#ReContactDate").val();
    var Note = $("#note").val();
    $.ajax({
        url: common.SitePath + "/Customer/Customer/Export",
        data: {
            CustomerID: CustomerID, JobID: JobID, SiteID: SiteID, LogDate: LogDate, ReContactDate: ReContactDate, Note: Note
        },
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (result) {
            return true;
        },
        error: function () {
            return false;
        }
    });
});


$("#btnSaveSend").click(function (e) {
    var isValid = true;
    var ReminderID = $("#reminderId").val();
    var JobNo = $("#JobNo").val();
    var MessageTypeId = $("#MessageTypeId").val();
    var TemplateMessageId = $("#TemplateMessageId").val();
    var Notes = $("#Note").val();
    var HasSMS = $('#HasSMS').is(':checked');
    var HasEmail = $("#HasEmail").is(':checked');
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
            url: common.SitePath + '/Employee/CustomerJob/SaveSendReminder',
            data: {
                ReminderId: ReminderID, JobNo: JobNo, MsgTypeId: MessageTypeId, MessageTypeId: MessageTypeId, TempMsgId: TemplateMessageId, TemplateMessageId: TemplateMessageId, Note: Notes, HasSMS: HasSMS, HasEmail: HasEmail
            },
            type: 'POST',
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response != null && response.success) {
                    $('.saved-rem-msg').css('color', 'green');
                    $('.saved-rem-msg').html("Reminder Saved & Sent Successfully!!");
                    $('.saved-rem-msg').show();
                    window.setTimeout(function () {
                        $(".closebtncontact").trigger('click');
                        window.location.href = common.SitePath + '/Employee/CustomerJob/SaveJobInfo?id=' + response.success + '&activetab=Contact Log'
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

$("#TemplateMessageId").change(function () {
    var tempmessage = $("#TemplateMessageId option:selected").val();
    if (tempmessage == "") {
        $('#Note').removeAttr('readonly');
    }
    else {
        $('#Note').attr('readonly', 'true');
    }
});

$(document).off("click", ".savejobLog").on("click", ".savejobLog", function () {
    var note = $("#note").val();
    var lDate = $("#LogDate").val().split(' ')[0].split('/')[2] + '-' + $("#LogDate").val().split(' ')[0].split('/')[1] + '-' + $("#LogDate").val().split(' ')[0].split('/')[0];
    var logDate = new Date(lDate);
    var rDate = $("#ReContactDate").val().split(' ')[0].split('/')[2] + '-' + $("#ReContactDate").val().split(' ')[0].split('/')[1] + '-' + $("#ReContactDate").val().split(' ')[0].split('/')[0];
    var RecontactDate = new Date(rDate);
    if (logDate > RecontactDate && RecontactDate != "") {
        $("#ContactLogErrorDv").empty();
        $(window).scrollTop(0);
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Log date can't be greater than ReContact date !</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (note == "") {
        $("#ContactLogErrorDv").empty();
        $(window).scrollTop(0);
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Note field is required !</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }

    var formdata = new FormData($('#contactLogForm').get(0));

    $.ajax({
        url: $('#contactLogForm').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            $('#divContactPartial').empty();
            $('#divContactPartial').html(result);
            $("#modalContactlog").modal("hide");
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Saved Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () {
            });
        },
        error: function () {
            alert('something went wrong !');
        }
    });

});

$(document).off("click", ".logSubmit").on("click", ".logSubmit", function () {
    var note = $("#note").val();
    var lDate = $("#LogDate").val().split(' ')[0].split('/')[2] + '-' + $("#LogDate").val().split(' ')[0].split('/')[1] + '-' + $("#LogDate").val().split(' ')[0].split('/')[0];
    var logDate = new Date(lDate);
    var rDate = $("#ReContactDate").val().split(' ')[0].split('/')[2] + '-' + $("#ReContactDate").val().split(' ')[0].split('/')[1] + '-' + $("#ReContactDate").val().split(' ')[0].split('/')[0];
    var RecontactDate = new Date(rDate);
    if (logDate > RecontactDate && RecontactDate != "") {
        $("#ContactLogErrorDv").empty();
        $(window).scrollTop(0);
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Log date can't be greater than ReContact date !</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (note == "") {
        $("#ContactLogErrorDv").empty();
        $(window).scrollTop(0);
        $("#ContactLogErrorDv").css("display", "block");
        $('#ContactLogErrorDv').css('color', 'red');
        $("#ContactLogErrorDv").html("<strong>Note field is required !</strong>");
        $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    });

    $(document).on("click", ".reminderSubmit", function () {
        var note = $("#note").val();
        if (note == "") {
            $("#ContactLogErrorDv").empty();
            $(window).scrollTop(0);
            $("#ContactLogErrorDv").css("display", "block");
            $('#ContactLogErrorDv').css('color', 'red');
            $("#ContactLogErrorDv").html("<strong>Note field is required !</strong>");
            $("#ContactLogErrorDv").delay(4000).fadeOut(function () {
            });
            return false;
        }
    });
