$(document).ready(function () {
    if ($(".pagination").length > 0) {
        $('#ddPageSize').parent().css('margin-top', '-72px');
    }
    else {
        $('#ddPageSize').parent().css('margin-top', '8px');
    }
});
$('#EditSiteDetail, #DeleteSiteDetail').click(function (event) {
    event.preventDefault();
    $('#CustomerSitesErrorDiv').empty();
    var type = $(this).text();
    var querystring = $(this).prop('href').split("/");
    var sitedetailid = querystring[querystring.length - 1];
    if (type == "Edit") {
        $.get(common.SitePath + "/Customer/Customer/ManageCustomerSitesPartial?SiteDetailId=" + sitedetailid, function (data) {
            $('#dvCustForm').css('display', 'block');
            $('#dvCustAddNew').css('display', 'none');
            $('#dvCustFormPartial').empty();
            $('#dvCustFormPartial').append(data);
        });
    }
    else if (type == "Delete") {
       
            $(".commonpopup").modal('show');
            $(".modal-title").html("Delete Site detail !");
            $(".alertmsg").text("Are you sure to delete?");
            $(".btnconfirm").attr("id", sitedetailid);
    }
});
$(document).on('click', ".btnconfirm", function () {
    var sitedetailid = $(this).attr("id");
    $.post(common.SitePath + "/Customer/Customer/DeleteCustomerSite", { SiteDetailId: sitedetailid }, function (data) {
        var id = data.id;
        var activetab = data.activetab;
        $(".commonpopup").modal('hide');
        $(".jobalert").css("display", "block");
        $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
        $(".jobalert").delay(2000).fadeOut(function () {
            window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
        });
    });
   
})



