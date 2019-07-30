
function DeleteDocuments(siteDetailId) {
    $(".commonpopupforJobDocs").modal('show');
    $(".alertmsg").text("Are you sure to delete document?");
    $(".btndeleteconfirm").attr("id", siteDetailId);
    $(".modal-title").html("Delete Site documents !");
}

$(document).on('click', ".btndeleteconfirm", function () {
    var siteDetailId = $(this).attr("id");
    var customerGeneralinfoid = FSM.CustomerGeneralInfoId;
    var pagenum = $('.active span').text();
    $.ajax({
        url: common.SitePath + "/customer/customer/DeleteSiteDocuments",
        data: { SiteDetailId: siteDetailId, PageNum: pagenum },
        type: 'Get',
        async: false,
        success: function (data) {
            $(".commonpopupforJobDocs").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Documents Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () {
                window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + customerGeneralinfoid + '&activetab=Documents&success=ok&pagenum=' + pagenum;
            });
        },
        error: function () {
            alert("something seems wrong");
        }
    });
})

$(document).on("click", ".viewdocuments", function () {
    var customerGeneralinfoid = FSM.CustomerGeneralInfoId;
    var sitedetailid = $(this).attr("sitedetailid");
    var pagenum = $('.active span').text();
    $.ajax({
        url: common.SitePath + "/customer/customer/_ViewSitesDocuments",
        data: { CustomerGeneralinfoid: customerGeneralinfoid, Sitedetailid: sitedetailid, PageNum: pagenum },
        type: 'POST',
        async: false,
        success: function (data) {
            $(".modaldoc").empty();
            var response = jQuery.parseJSON(data.list);
            var doc = "";
            for (var i = 0; i < data.length; i++) {
                doc = doc + "<p class='docName',style=float:left;width:80% ;padding: 0 0 10px 0;>" + response[i]["DocumentName"] + "</p><button type='button' class='btndownload btn-success' data-attr='" + response[i]["DocumentId"] + "' >Download</button></br>";
            }
            $(".modaldoc").append(doc);
            $("#modalcustdoc").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    })
})

$(document).on("click", ".btndownload ", function () {
    var documentId = $(this).attr("data-attr");
    var customerGeneralinfoid = FSM.CustomerGeneralInfoId;
    var pagenum = $('.active span').text();
    $.ajax({
        url: common.SitePath + "/customer/customer/DownloadFile",
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
    var Uploader = $("input:file")[0].files.length;
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
        return true;
    }
}
$(document).off("click", '.Customerquickview').on('click', '.Customerquickview', function (event) {
    event.preventDefault();
    var sitedetailid = $(this).attr("sitedetailid");
    var data = { siteId: sitedetailid };
    $.ajax({
        url: common.SitePath + "/customer/customer/CustomerDocQuickView",
        type: 'GET',
        data: data,
        async: false,
        success: function (result) {
            $(window).scrollTop(0);
            $('.quickviewdiv').empty();
            $('.quickviewdiv').html(result);
            $('#modalQuickCustSiteView').modal("show");
        },
        error: function () {
            alert('something went wrong !');
        }
    });
});



