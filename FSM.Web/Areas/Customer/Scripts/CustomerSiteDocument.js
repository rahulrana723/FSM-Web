
var Global = {
    SiteID: "",
    IsfromJobInvoice: false,
    IsfromJob:false,
    JobId: "",
    InvoiceId: ""
}


$('#btnsitesearch').on('click', function () {
    var name = $('#AddDocName').val();
    var page_size = $('#ddPageSize').val();
    var customerGeneralInfoId = FSM.CustomerGeneralInfoId;
    $.get(common.SitePath + "/Customer/Customer/CustomerSiteDocumentsList?Name=" + name + "&customerGeneralInfoId=" + customerGeneralInfoId, function (data) {
        $('#dvShowCustList').empty();
        $('#dvShowCustList').append(data);
    });

});

$(document).on("keypress", "#AddDocName", function (e) {
    var key = e.which;
    if (key == 13)  // the enter key code
    {
        $('#btnsitesearch').click();
        return false;
    }
});

$('#add_btn').click(function () {
    {
        $('.document_collapse').toggle();
    }
});
$('#btnjobsitesearch').on('click', function () {
    var name = $('#AddDocName').val();
    var page_size = $('#ddPageSize').val();
    var SiteInfoId = FSM.SiteId;
    $.get(common.SitePath + "/Employee/CustomerJob/CustomerSiteDocumentsList?Name=" + name + "&siteInfoId=" + SiteInfoId, function (data) {
        $('#dvShowCustList').empty();
        $('#dvShowCustList').append(data);
    });
});


$(document).ready(function () {

    var CustomerId = $('#CustomerSiteDocuments_CustomerGeneralInfoId').val();
    var siteid = "";
    var jobid = "";
    $(document).on('change', '#SiteCountviewModel_SiteDetailId', function () {
        Global.SiteID = $('#SiteCountviewModel_SiteDetailId  option:selected').val();
        siteid = $('#SiteCountviewModel_SiteDetailId  option:selected').val();
    });

    //for job and invoice


    $('#dropArea').filedrop({

        url: '/Customer/Customer/UploadFiles',
        //allowedfiletypes: ['image/jpeg', 'image/png', 'image/gif'],
        //allowedfileextensions: ['.jpg', '.jpeg', '.png', '.gif'],
        paramname: 'files',
        maxfiles: 20,
        maxfilesize: 5, // in MB
        data: {
            CustomerGeneralinfoid: CustomerId,
            SiteId: function () { return siteid; }
        },
        dragOver: function () {
            //$("#dropArea").css("display", "block");
            $('#dropArea').addClass('active-drop');
        },
        dragLeave: function () {
            $('#dropArea').removeClass('active-drop');

        },
        drop: function () {
            if (Global.SiteID == '' || Global.SiteID == 'undefined' || Global.SiteID == 'null') {
                Global.SiteID = $('#CustomerSiteDocuments_SiteId').val();
                siteid = $('#CustomerSiteDocuments_SiteId').val();
                if (FSM.JobId!="")
                    Global.IsfromJob = true;
                else if (FSM.InvoiceId!="")
                    Global.IsfromJobInvoice = true;
            }
            if (siteid == "" || siteid == 'undefined' || siteid == null) {
                alert("please select Site.");
                return false;
            }
            else {

                //  siteid = $('#SiteCountviewModel_SiteDetailId  option:selected').val();
                $('#dropArea').removeClass('active-drop');
            }
        },
        afterAll: function (e) {
            $('#dropArea').html('file(s) uploaded successfully');
            $(window).scrollTop(0);
            $('.jobalert').empty();
            $('.jobalert').css('color', 'green');
            $('.jobalert').html('<Strong>Document(s) Uploaded successfully!</Strong>');
            $('.jobalert').show();
            if (Global.IsfromJobInvoice) {
                window.setTimeout(function() {
                        $('.jobalert').hide();
                        window.location.href =
                            common.SitePath +
                            '/Employee/Invoice/SaveInvoiceInfo?id=' +
                            FSM.InvoiceId +
                            '&activetab=Site Documents&success=ok';
                    },
                    4000);
            }
            else if (Global.IsfromJob) {
                window.setTimeout(function() {
                        $('.jobalert').hide();
                        window.location.href =
                            common.SitePath +
                            '/Employee/CustomerJob/SaveJobInfo?id=' +
                            FSM.JobId +
                            '&activetab=Site Documents&success=ok';
                    },
                    4000);
            }
            else {
                window.setTimeout(function () {
                    $('.jobalert').hide();
                    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + CustomerId + '&activetab=Documents&success=ok';
                }, 4000)

            }
            //   return RedirectToAction("AddCustomerInfo", new { id = model.CustomerSiteDocuments.CustomerGeneralInfoId.ToString(), activetab = "Documents", success = "ok" });
        },
        uploadFinished: function (i, file, response, time) {


            //$("#dropArea").css("display", "none");
            //$('#uploadList').append('<li class="list-group-item">'+file.name+'</li>')
        }
    })
})

