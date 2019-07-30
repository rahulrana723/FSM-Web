
$(document).ready(function () {
    $("#CustomerEmailsendviewModel_To").val("support@srag-portal.com");
});


$("#CustomerEmailsendviewModel_BillingContact").change(function () {
    var Email = $("#CustomerEmailsendviewModel_BillingContact option:selected").html();
    var Emailval = $("#CustomerEmailsendviewModel_BillingContact option:selected").val();
    if (Emailval != null && Emailval != '' && Emailval != 'undefined') {
        $("#CustomerEmailsendviewModel_To").val(Email);
    }
    else {
        $("#CustomerEmailsendviewModel_To").val("");
    }
})

function uploadFile_onChange(input) {
    
        var file_name = $("#fUpload");
        var ReqItem = $("#fUpload").val();
        var lg = file_name[0].files.length;
        var items = file_name[0].files;
        if (ReqItem != "") {
            var size = $('#fUpload')[0].files[0].size;
        }
        if (lg > 5) {
            alert("Maximum 5 documents can be attached.");
            return false;
        }
        else if (size > 11194304) {
            alert("Maximum file size 10 MB");
            return false;
        }
        else if (lg > 0) {
            for (var i = 0; i < lg; i++) {
                var fileName = items[i].name; // get file name
                var fileSize = items[i].size; // get file size
                var fileType = items[i].type; // get file type
                $("#list_files").append("<tr><td>" + fileName + "</td><td>" + fileSize + "</td></tr>");
            }
        }
    
};


function SendMailWithAttachment() {
    
    var to = $("#CustomerEmailsendviewModel_To").val();
    var from = $("#CustomerEmailsendviewModel_From").val();
   var   FromEmail = "invoicing@sydneyroofandgutter.com.au";
    var subject = $("#CustomerEmailsendviewModel_Subject").val();
    var files = $("#fUpload").get(0).files;
    var cc = $("#CustomerEmailsendviewModel_CC").val();
    var bcc = $("#CustomerEmailsendviewModel_Bcc").val();
    var message = $("#CustomerEmailsendviewModel_Message").val();
    var Severity = $(".Severity :selected").text();
    var severityval = $(".Severity :selected").val();
    var docs;
    var a = [];
   if (severityval == "") {
        $("#Severityerror").text("*Please select Severity");
        return false;
    }
    else {
        $("#Severityerror").text("");
    }
      
    if (message == "")
    {
        $("#MessageError").text("*Message body can't be Empty");
        return false;
    }
    else
    {
        $("#MessageError").text("");
    }

    $("#SendEmail").val("Sending...");
    //Invoice Attahed

    var data = new FormData();
    for (var i = 0; i < files.length; i++)
        data.append("files[" + i + "]", files[i]);
    data.append("to", to);
    data.append("from", from);
    data.append("fromEmail", FromEmail);
    data.append("subject", subject);
    data.append("Cc", cc);
    data.append("Bcc", bcc);
    data.append("Message", message);
    if (severityval != "")
        data.append("Severity", Severity);
    
    $.ajax({
        type: 'POST',
        url: common.SitePath + "/Customer/Customer/SendSupportMail",
        processData: false,
        contentType: false,
        dataType: "json",
        data: data,
        success: function (data) {
            if (data.responseText != "") {
                $("#SendEmail").val("Send");
                $('.jobalert').css('color', 'green');
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                    location.reload();
                }, 4000)
                $("#CustomerEmailsendviewModel_To").val("");
                $("#CustomerEmailsendviewModel_Subject").val("");
                $("#list_files").empty();
                $("#CustomerEmailsendviewModel_CC").val("");
                $("#CustomerEmailsendviewModel_Bcc").val("");
                $("#CustomerEmailsendviewModel_Message").val("");
            }
        },
        error: function () {
        }
    })
}
