

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
       if(key == 13)  // the enter key code
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



