$(document).ready(function () {
    var ddlSiteAddress = $("#ddlSite").text();
    $("#showSiteAddress").text(ddlSiteAddress);
})
function DeletejobDocuments(siteDetailId, DocumentId) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure to delete document?");
    $(".btnconfirm").attr("id", siteDetailId);
    $(".btnconfirm").attr("docid", DocumentId);
    $(".modal-title").html("Delete Site documents !");
}

$(document).on('click', ".btnconfirm", function () {
    var siteDetailId = $(this).attr("id");
    var documentId = $(this).attr("docid");
    var customerGeneralinfoid = FSM.CustomerGeneralInfoId;
    var pagenum = $('.active span').text();
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/DeleteJobSiteDocuments",
        data: { SiteDetailId: siteDetailId,DocumentId:documentId, PageNum: pagenum },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#dvShowCustList").empty();
            $("#dvShowCustList").append(data);
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Documents Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () {
            });
        },
        error: function () {
            alert("something seems wrong");
        }
    });
})

$(document).on("click", ".viewjobdocuments", function () {
    var sitedetailid = $(this).attr("sitedetailid");
    var pagenum = $('.active span').text();
    $.ajax({
        url: common.SitePath + "/Employee/customerJob/_ViewSitesDocuments",
        data: {  Sitedetailid: sitedetailid, PageNum: pagenum },
        type: 'POST',
        async: false,
        success: function (data) {
            $(".modaldoc").empty();
            var response = jQuery.parseJSON(data.list);
            var doc = "";
            for (var i = 0; i < data.length; i++) {
                doc = doc + "<p class='docName',style=float:left;width:80% ;padding: 0 0 10px 0;>" + response[i]["DocumentName"] + "</p><button type='button' class='btndownloadJobdoc btn-success' data-attr='" + response[i]["DocumentId"] + "' >Download</button></br>";
            }
            $(".modaldoc").append(doc);
            $("#modalcustdoc").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    })
})

$(document).on("click", ".btndownloadJobdoc", function () {
    var documentId = $(this).attr("data-attr");
    var pagenum = $('.active span').text();
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/DownloadFile",
        data: { DocumentId: documentId },
        type: 'Get',
        async: false,
        success: function (data) {
            window.open(common.SitePath + data);
        },
        error: function () {
            alert("something seems wrong");
        }
    });

})

function ValidateDocs() {
    var ReqItem = $("#docName").val();
    var Uploader = $("#docName")[0].files.length;
    var siteName = $("#SiteCountviewModel_SiteDetailId").val();
    if (ReqItem != "") {
        var size = $('#docName')[0].files[0].size;
    }
    if (siteName == "") {
        $("#validateSitename").text("please select site name");
        return false;
    }
    else if (ReqItem == "") {
        alert("Please select atleast one document");
        return false;
    }
    else if (Uploader > 5) {
        alert("Maximum 5 documents uploaded.");
        return false;
    }
    else if (size > 11194304) {
        alert("Maximum file size 10 MB");
        return false;
    }
    else {
        $("#validate_stockitem").text("");
    
    var formdata = new FormData($('#addDocumentForm').get(0));
    $.ajax({
        url: $('#addDocumentForm').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            $('#dvShowCustList').empty();
            $('#dvShowCustList').html(result);
            $('.document_collapse').toggle();
            $('#docName').val('');
        },
        error: function () {
            alert('something went wrong !');
        }
    });
    }
}

$(document).off("click", '.quicksiteview').on('click', '.quicksiteview', function (event) {
    event.preventDefault();

    var action = $(this).attr('href');
    var siteid = $(this).attr("data-siteid");
    var data = { id: siteid };

    $.ajax({
        url: action,
        type: 'GET',
        data: data,
        async: false,
        success: function (result) {
            $(window).scrollTop(0);
            $('.quicksiteviewdiv').empty();
            $('.quicksiteviewdiv').html(result);
            $('#modalQuickSiteView').modal("show");
        },
        error: function () {
            alert('something went wrong !');
        }
    });
});




