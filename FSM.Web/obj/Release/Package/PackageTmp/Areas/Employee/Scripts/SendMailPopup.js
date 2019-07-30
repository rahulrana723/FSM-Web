
var InvoiceEmail = {
    SendMailWithAttachment: function () {

        debugger;
        //var to = $("#CustomerEmailsendviewModel_To").val();
        var to = [];
        $('#CustomerIDs').each(function () {
            $('.fstChoiceItem').each(function () {
                to.push($(this).attr('data-text'))
            });
        });
      
        var cc = [];
        cc = $("#CC").val().split(',');
      
        var from = $("#CustomerEmailsendviewModel_From").val();
        var FromEmail = $("#FromEmail option:selected").text();
        var fromEmailVal = $("#FromEmail option:selected").val();
        if (fromEmailVal == "" || FromEmail=="(From Email)") {
            FromEmail = "invoicing@sydneyroofandgutter.com.au";
        }
        
        var subject = $("#Subject").val();
        var files = $("#fUpload").get(0).files;
       // var cc = $("#CC").val();
        var template = $('#EmailTemplates').val();
        var invoiceID = FSM.invoiceID;
        var employeeJobId = FSM.EmployeeJobId;
        var docs;
        var a = [];
        var templateData = $('.richText-editor').html();
        var templateType = $("#EmailTemplates").val();
        if (templateType != '' && templateType != null && templateType != 'undefined') {
            $("#TemplateError").text("");
        }
        else {
            $("#TemplateError").text("*Please select Template");
            return false;
        }

        if (to == "") {
            $("#ToError").text("*To email is required");
            return false;
        }
        else {
            $("#ToErrorr").text("");
        }

        debugger;
        if (to != "") {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            for (var i = 0, l = to.length; i < l; i++) {
                var value = to[i];
                if (reg.test(value) == true) {
                    $("#ToError").text("");
                }
                else {
                    $("#ToError").text("*Please enter valid email address");
                    return false;
                }
            }
        }
        debugger;
        if (cc != "") {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            for (var i = 0, l = cc.length; i < l; i++) {
                var value = cc[i];
             
                if (reg.test(value) == true) {
                    $("#CCError").text("");
                }
                else {
                    $("#CCError").text("*Please enter valid email address");
                    return false;
                }
            }
        }

        var Chkdoc = [];
        $('#jobDocId').each(function () {
            $('input:checked').each(function () {
                Chkdoc.push($(this).attr("docid"));
            });
        });

        var ChkSitedoc = [];
        $('#siteDocsId').each(function () {
            $('input:checked').each(function () {
                ChkSitedoc.push($(this).attr("Sitedocid"));
            });

        });

        var Chkimpdoc = [];
        $('#impDocId').each(function () {
            $('input:checked').each(function () {
                Chkimpdoc.push($(this).attr("Impdocid"));
            });

        });
        $("#SendEmail").val("Sending...");
        //Invoice Attahed

        var data = new FormData();
        for (var i = 0; i < files.length; i++)
            data.append("files[" + i + "]", files[i]);
        data.append("to", to);
        data.append("from", from);
        data.append("fromEmail", FromEmail);
        data.append("subject", subject);
        data.append("docid", Chkdoc)
        data.append("Sitedocid", ChkSitedoc)
        data.append("Impdocid", Chkimpdoc)
        data.append("Cc", cc);
        data.append("INVOICEID", invoiceID)
        data.append("Template", templateType)
        data.append("EmployeeJobId", employeeJobId)
        data.append("TemplateData", templateData);
        $(".btnsendemail").text("Sending....");
        $.ajax({
            type: 'POST',
            url: common.SitePath + "/Employee/Invoice/SendCustomerEmail",
            processData: false,
            contentType: false,
            async: true,
            dataType: "json",
            data: data,
            success: function (data) {
                if (data.responseText != "") {
                    $("#SendEmail").val("Send");
                    $(".btnsendemail").text("Send");
                    $("#EmailTemplates option").prop("selected", false);
                    $("#FromEmail option").prop("selected", false);
                    $("#myModal").modal('hide');
                    $('.jobalert').css('color', 'green');
                    $('.jobalert').show();
                    window.setTimeout(function () {
                        $('.jobalert').hide();
                        //location.reload();
                        debugger;
                        if (data.bookNextContractJob == true) {
                            if (data.invoiceAlreadySent == false) {
                                $(".commonpopup").modal('show');
                                $(".alertmsg").text("Would you like to book the next contracted job?");
                                $(".btnconfirm").attr("id", data.jobId);
                                $(".btncancel").attr("id", data.jobId);
                                $(".modal-title").html("Book next contract job!");
                            }
                        }
                    }, 1000)
                    $("#To").val("");
                    $('.richText-editor').html("");
                    $("#Subject").val("");
                    $("#list_files").empty();
                    $("#FromEmail")
                    var count = $("#list_files tr").length;
                    if (parseInt(count) <= 2) {
                        $("#list_files").append("  <thead<tr><th>Filename</th>  <th>Size</th>  </tr> </thead> <tbody>  <tr>  </tr> </tbody><tr class='emptytr'> <td colspan='2'>No Record Available</td></tr>");
                    }
                    $("#CC").val("");
                    $("#Bcc").val("");
                    $('#jobDocId').each(function () {

                        $(this).attr('checked', false);
                    });
                    $('#siteDocsId').each(function () {

                        $(this).attr('checked', false);
                    });
                    $('#impDocId').each(function () {

                        $(this).attr('checked', false);
                    });

                }
            },
            error: function () {
                $("#myModal").modal('hide');
                $('.jobalert').css('color', 'red');
                $('.jobalert').html("Some Error occured please try later");
                $("#SendEmail").val("Send");
                $(".btnsendemail").text("Send");
            }
        })

    },
}

$(document).ready(function () {
    $("#myModal").hide();
});
///Send Email Click button on Main form
$("#SendEmail").click(function () {
    $("#myModal").show();
})
//send button popup
$(".btnsendemail").click(function () {
    InvoiceEmail.SendMailWithAttachment();
})


