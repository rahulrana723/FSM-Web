
var InvoiceEmail = {

}
$(document).ready(function () {
    //Invoice Pdf Download
    var invoiceID = FSM.invoiceID;

    $.ajax({
        type: 'POST',
        url: common.SitePath + "/Employee/Invoice/GetExportFile",
        data: { InvoiceId: invoiceID },
        type: 'Get',
        async: true,
        success: function (data) {

        },
        error: function () {
        }
    })



    var count = $("#list_files tr").length;
    if (parseInt(count) <= 2) {
        $("#list_files").append("<tr class='emptytr'> <td colspan='2'>No Record Available</td></tr>");
    }
    var Emaillength = $('#CustomerEmailsendviewModel_BillingContact> option').length;
    var email = $("#hdnemail").val();
    $("#CustomerEmailsendviewModel_To").val(email)

    if (Emaillength > 1) {
        $("#CustomerEmailsendviewModel_To").val("");
    }
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
    var count = $("#list_files tr").length;

    if (parseInt(count) <= 2) {
        $("#list_files").append("<tr> <td colspan='2'>No Record Available</td></tr>");
    }
    else {
        $(".emptytr").remove();

        var file_name = $("#fUpload");
        var ReqItem = $("#fUpload").val();
        var lg = file_name[0].files.length;
        var items = file_name[0].files;
        if (ReqItem != "") {
            var size = $('#fUpload')[0].files[0].size;
        }
        if (lg > 5) {
            alert("Maximum 5 documents uploaded.");
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
    }
};



$('.allChkimpdoc').change(function () {
    // this will contain a reference to the checkbox 
        var checked = $(".allChkimpdoc").prop('checked');
    if (checked) {
        // the checkbox is now checked 
        $('#impDocId').each(function () {
            $('input').each(function () {
                $('.checkedItemimp').prop('checked', true);
               
            });
        });

    } else {
        $('#impDocId').each(function () {
            $('input').each(function () {
                $('.checkedItemimp').prop('checked', false);
            });
        });
    }
})


$('.allChkjobdoc').change(function () {
    // this will contain a reference to the checkbox 
    var checked = $(".allChkjobdoc").prop('checked');
    if (checked) {
        // the checkbox is now checked 
        $('#jobDocId').each(function () {
            $('input').each(function () {
                $('.checkedItemjob').prop('checked', true);
            });
        });
    } else {
        $('#jobDocId').each(function () {
            $('input').each(function () {
                $('.checkedItemjob').prop('checked', false);
            });
        });
    }
})


$('.allChksitedoc').change(function () {
    // this will contain a reference to the checkbox 
    var checked = $(".allChksitedoc").prop('checked');
    if (checked) {
        // the checkbox is now checked 
        $('#siteDocsId').each(function () {
            $('input:checked').each(function () {
                $('.checkedItemsite').prop('checked', true);

            });
        });

    } else {
        $('#siteDocsId').each(function () {
            $('input:checked').each(function () {
                $('.checkedItemsite').prop('checked', false);
            });
        });
    }
})





$(document).on('click', ".btnconfirm", function () {
    $(".commonpopup").modal('hide');
    var id = $(this).attr("id");
    $.ajax({
        url: common.SitePath + "/Employee/Invoice/_SaveNextContractJob",
        data: { JobId: id },
        type: 'Get',
        async: false,
        success: function (data) {
            $(".commonpopup").modal('hide');
            $("#divContractJobPopUp").empty();
            $("#divContractJobPopUp").html(data);
            $("#modalContractJobPopUp").modal("show");
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
});

$(document).on('click', ".btncancel", function () {
    $(".commonpopup").modal('hide');
    var id = $(this).attr("id");
    $.ajax({
        url: common.SitePath + "/Employee/Invoice/_CancelNextContractJob",
        data: { JobId: id },
        type: 'Get',
        async: false,
        success: function (data) {
            $(".commonpopup").modal('hide');
            $("#divCancelContractJobPopUp").empty();
            $("#divCancelContractJobPopUp").html(data);
            $("#modalCancelContractJobPopUp").modal("show");
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})

