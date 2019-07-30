
$(document).on('click', '.btndownloadsitedoc', function () {
    var documentid = $(this).attr("documentid");
    $.ajax({
        url: common.SitePath + "/Employee/Job/DownloadSiteDocuments",
        data: { SiteDocid: documentid },
        type: 'GET',
        async: false,
        success: function (data) {
            window.open(common.SitePath + data);
        },
        error: function () {
            alert("something seems wrong!")
        }
    })
})