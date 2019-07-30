
var invoicelisting = {
    invoiceid: "",
    UpdateInvoiceStatus: function (invoicestatus, invoiceid, IsActive) {
        $.ajax({
            type: 'POST',
            url: common.SitePath + "/Employee/Invoice/UpdateInvoiceStatus",
            data: { InvoiceId: invoiceid, StatusName: invoicestatus, StatusValue: IsActive },
            async: false,
            success: function (result) {
            },
            error: function () {
            }
        });
    }
}

$(document).ready(function () {
    if (FSM.Message != "" && FSM.Message != 0) {
        $(".jobalert").css("display", "block");
        if (FSM.Message == "1") {
            $(".jobalert").html("<strong>Record added Successfully!</strong>");
        }
        else if (FSM.Message == "2") {
            $(".jobalert").html("<strong>Record Updated Successfully!</strong>");
        }
        else if (FSM.Message == "3") {
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
        }
        else if (FSM.Message == "4") {
            $(".jobalert").html("<strong>Record Send Successfully!</strong>");
        }
        FSM.Message = "";
        $(".jobalert").delay(2000).fadeOut();
    }


    $('#StartDate').datepicker({
        dateFormat: 'dd/mm/yy',
    });
    $('#EndDate').datepicker({
        dateFormat: 'dd/mm/yy',
    });


    $('#ddPageSize').val(FSM.SelectedVal);
    if (parseInt(FSM.HasGridRecords) > 0) {
        if ($(".pagination").length > 0) {
            $('#ddPageSize').parent().css('margin-top', '-72px');
        }
        else {
            $('#ddPageSize').parent().css('margin-top', '8px');
        }
    }
    else {
        $('#ddPageSize').parent().css('margin-top', '0px');
    }
});

$('#ddPageSize').on('change', function () {

    var JobId = FSM.JobId;
    var page_size = $('#ddPageSize').val();
    var Searrchkeyword = $('#searchkeyword').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();
    var SentStatus = $('#SentStatus').val();
    var CustomerType = $('#CustomerType').val();
    var pagenum = $('.active span').text();


    if (JobId != "" && JobId != undefined && page_size != '00000000-0000-0000-0000-000000000000') {
        window.location.href = FSM.URL + "?page_size=" + page_size + "&searchkeyword=" + Searrchkeyword + "&SentStatus=" + SentStatus + "&EndDate=" + EndDate + "&StartDate=" + StartDate + "&CustomerType=" + CustomerType + "&grid-page=" + pagenum + "&jid=" + JobId;
    }
    else {

        window.location.href = FSM.URL + "?page_size=" + page_size + "&searchkeyword=" + Searrchkeyword + "&SentStatus=" + SentStatus + "&EndDate=" + EndDate + "&StartDate=" + StartDate + "&CustomerType=" + CustomerType + "&grid-page=" + pagenum;
    }
});

$('.grid-header-title a').on('click', function () {
    var elementparam = $(this).attr('href');

    var index = elementparam.indexOf("grid-column");

    if (index > 1) {
        var paramarray = elementparam.split("&");

        elementparam = '?';
        $.each(paramarray, function (i, item) {

            if (item.indexOf("grid-column") > -1 || item.indexOf("grid-dir") > -1) {
                elementparam = elementparam + item + '&';
            }
        });

        elementparam = elementparam.substring(0, (elementparam.length - 1));
    }

    var Searrchkeyword = $('#searchkeyword').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();
    var SentStatus = $('#SentStatus').val();
    var CustomerType = $('#CustomerType').val();

    if (Searrchkeyword != "" && Searrchkeyword != undefined) {
        elementparam = elementparam + '&searchkeyword=' + Searrchkeyword;
    }

    if (SentStatus != "" && SentStatus != undefined && parseInt(SentStatus) > 0) {
        elementparam = elementparam + '&SentStatus=' + SentStatus;
    }

    if (EndDate != "" && EndDate != undefined) {
        elementparam = elementparam + '&EndDate=' + EndDate;
    }

    if (StartDate != "" && StartDate != undefined) {
        elementparam = elementparam + '&StartDate=' + StartDate;
    }
    if (CustomerType != "" && CustomerType != undefined && parseInt(CustomerType) > 0) {
        elementparam = elementparam + '&CustomerType=' + CustomerType;
    }

    var pagenum = $('.active span').text();
    if (pagenum != "" && pagenum != undefined) {
        elementparam = elementparam + '&grid-page=' + pagenum;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }
    var JobId = FSM.JobId;
    if (JobId != "" && JobId != undefined && pagesize != '00000000-0000-0000-0000-000000000000') {
        elementparam = elementparam + '&jid=' + JobId;
    }
    $(this).attr('href', elementparam);

});

$('.grid-footer a').on('click', function myfunction() {

    var elementparam = $(this).attr('href');

    var index = elementparam.indexOf("grid-page");
    var paramarray = elementparam.split("&");

    if (index > 1) {

        elementparam = '?';
        $.each(paramarray, function (i, item) {

            if (item.indexOf("grid-page") > -1) {
                elementparam = elementparam + item + '&';
            }
        });

        elementparam = elementparam.substring(0, (elementparam.length - 1));
    }
    else {
        elementparam = paramarray[0];
    }

    var Searrchkeyword = $('#searchkeyword').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();
    var SentStatus = $('#SentStatus').val();
    var CustomerType = $('#CustomerType').val();

    if (Searrchkeyword != "" && Searrchkeyword != undefined) {
        elementparam = elementparam + '&searchkeyword=' + Searrchkeyword;
    }

    if (SentStatus != "" && SentStatus != undefined && parseInt(SentStatus) > 0) {
        elementparam = elementparam + '&SentStatus=' + SentStatus;
    }

    if (EndDate != "" && EndDate != undefined) {
        elementparam = elementparam + '&EndDate=' + EndDate;
    }

    if (StartDate != "" && StartDate != undefined) {
        elementparam = elementparam + '&StartDate=' + StartDate;
    }

    if (CustomerType != "" && CustomerType != undefined && parseInt(CustomerType) > 0) {
        elementparam = elementparam + '&CustomerType=' + CustomerType;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }
    var JobId = FSM.JobId;
    if (JobId != "" && JobId != undefined && pagesize != '00000000-0000-0000-0000-000000000000') {
        elementparam = elementparam + '&jid=' + JobId;
    }

    $(this).attr('href', elementparam);


});


function DeleteInvoice(id) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure to delete Invoice?");
    $(".btnconfirm").attr("id", id);
    $(".modal-title").html("Delete Invoice !");
}

$(document).on('click', ".btnconfirm", function () {
    var JobId = FSM.JobId;
    var page_size = $('#ddPageSize').val();
    var Searrchkeyword = $('#searchkeyword').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();
    var SentStatus = $('#SentStatus').val();
    var CustomerType = $('#CustomerType').val();
    var pagenum = $('.active span').text();


    var id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Invoice/DeleteInvoice",
        data: { Id: id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Invoice Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () {
                if (JobId != "" && JobId != undefined && page_size != '00000000-0000-0000-0000-000000000000') {
                    window.location.href = FSM.URL + "?page_size=" + page_size + "&searchkeyword=" + Searrchkeyword + "&SentStatus=" + SentStatus + "&EndDate=" + EndDate + "&StartDate=" + StartDate + "&CustomerType=" + CustomerType + "&grid-page=" + pagenum + "&jid=" + JobId;
                }
                else {

                    window.location.href = FSM.URL + "?page_size=" + page_size + "&searchkeyword=" + Searrchkeyword + "&SentStatus=" + SentStatus + "&EndDate=" + EndDate + "&StartDate=" + StartDate + "&CustomerType=" + CustomerType + "&grid-page=" + pagenum;
                }
                //window.location.href = common.SitePath + "/Employee/Invoice/InvoiceList";
            });
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})

$(document).on('dblclick', '.cssEditInvoice', function () {
    // getting customer id
    var Id = $(this).find('td:eq(0)').text();
    window.location = FSM.EditInvoice + "/" + Id;
});


//***********************************************Quick view Grid**********************************************************************
$(".Quickbtngrid").bind('click', function () {
    var quickbtn = $(this).children('.qucikbtn').text();
    if (quickbtn == "v") {
        $('.QuickDiv').remove();
        $(this).children('.qucikbtn').text(">");
        return false;
    }
    $('.qucikbtn').text(">");
    $(this).children('.qucikbtn').text("v");
    var invoice = $(this).attr("Invoiceid");
    var customerNote;
    var operationNote;
    var jobNote;
    var jobType;
    var price;
    var paid;
    var approved;
    var Billingcontact;
    var BillingNotes;
    var oTrwNotes;
    var time,
     IsPhotoupdloaded
    , IsDocsUploaded
    , IsPaid, Stock
    , Material
   , Isapproved
   , IsSend;
    invoicelisting.invoiceid = invoice;
    $.ajax({
        url: common.SitePath + "/Employee/Invoice/GetQuickViewInvoiceData",
        data: { InvoiceId: invoice },
        type: 'Get',
        async: false,
        success: function (data) {
            customerNote = data.CustomerNotes, operationNote = data.OperationNotes, jobNote = data.JobNotes,
            Billingcontact = data.BillingContact, BillingNotes = data.BillingNotes, oTrwNotes = data.OTrwNotes,
            jobType = data.DisplayType, price = data.InvoicePrice, paid = data.Paid, approved = data.ApprovedByName, time = data.TimeTaken
            IsPhotoupdloaded = data.Photos,
            IsDocsUploaded = data.RequiredDocs,
            IsPaid = data.IsPaid,
            Stock = data.Stock,
            Material = data.Material,
            Isapproved = data.IsApproved,
            IsSend = data.SentStatus
        },
        error: function () {
            alert("something seems wrong");
        }
    });
    $('.QuickDiv').remove();

    if (IsPhotoupdloaded == true) {
        IsPhotoupdloaded = '<input type=checkbox class="chkphotos" checked="checked"> </input>';
    }
    else {
        IsPhotoupdloaded = '<input type=checkbox class="chkphotos"> </input>';
    }

    if (IsDocsUploaded == true) {
        IsDocsUploaded = '<input type=checkbox  class="chkdocs" checked="checked"> </input>';
    }
    else {
        IsDocsUploaded = '<input type=checkbox  class="chkdocs"> </input>';
    }

    if (IsPaid == true) {
        IsPaid = '<input type=checkbox class="chkpaid" checked="checked"> </input>';
    }
    else {
        IsPaid = '<input type=checkbox class="chkpaid"> </input>';
    }
    if (Stock == true) {
        Stock = '<input type=checkbox class="chkstack" checked="checked"> </input>';
    }
    else {
        Stock = '<input type=checkbox class="chkstack"> </input>';
    }

    if (Material == true) {
        Material = '<input type=checkbox  class="chkmaterial" checked="checked"> </input>';
    }
    else {
        Material = '<input type=checkbox  class="chkmaterial"> </input>';
    }
    if (Isapproved == true && IsSend != 2 && IsSend != 0) {
        Isapproved = '<b>Approved By : </b>' + approved;
    }
    else {
        Isapproved = '<b>Approved & Sent :&nbsp;&nbsp;</b><input type=checkbox  class="chkapproved" > </input>';
    }

    $('<tr class="QuickDiv"><td colspan="12"><table><tbody>\
        <tr><td><b>Billling Contact: &nbsp;&nbsp;</b> ' + Billingcontact + '</td> <td><b>Status: </b> </td></tr>\
        <tr><td><b>Billing Notes: &nbsp;&nbsp;</b> ' + BillingNotes + '</td><td><b>PHOTOS &nbsp;&nbsp; </b> ' + IsPhotoupdloaded + '</td></tr>\
        <tr><td><b>Job Notes: &nbsp;&nbsp;</b> ' + jobNote + '</td><td><b>Required Documents: &nbsp;&nbsp; </b>' + IsDocsUploaded + '</td></tr>\
        <tr><td><b>Opeation Notes:&nbsp;&nbsp; </b> ' + operationNote + '</td> <td><b>Paid: &nbsp;&nbsp; </b>' + IsPaid + '</td></tr>\
        <tr><td><b>Otrw Notes:&nbsp;&nbsp; </b> ' + oTrwNotes + '</td> <td><b>Stock </b>&nbsp;&nbsp;' + Stock + '</td></tr>\
        <tr><td><b>Job Type: &nbsp;&nbsp;</b> ' + jobType + '</td> <td><b>Material </b>&nbsp;&nbsp;' + Material + '</td></tr>\
         <tr><td><b>Time Taken: &nbsp;&nbsp;</b> ' + time + '</td> <td>' + Isapproved + '</td></tr>\
        </tbody></table></td></tr>'
       ).insertAfter($(this).closest('tr'));
});


$(document).on("change", ".chkphotos", function () {
    var invoiceid = invoicelisting.invoiceid;
    var ISActive = $(this).is(":checked")
    invoicelisting.UpdateInvoiceStatus("Photos", invoiceid, ISActive);
})
$(document).on("change", ".chkdocs", function () {
    var invoiceid = invoicelisting.invoiceid;
    var ISActive = $(this).is(":checked")
    invoicelisting.UpdateInvoiceStatus("Docs", invoiceid, ISActive);
})
$(document).on("change", ".chkpaid", function () {
    var invoiceid = invoicelisting.invoiceid;
    var ISActive = $(this).is(":checked")
    invoicelisting.UpdateInvoiceStatus("Paid", invoiceid, ISActive);
})
$(document).on("change", ".chkstack", function () {
    var invoiceid = invoicelisting.invoiceid;
    var ISActive = $(this).is(":checked")
    invoicelisting.UpdateInvoiceStatus("Stock", invoiceid, ISActive);
})
$(document).on("change", ".chkmaterial", function () {
    var invoiceid = invoicelisting.invoiceid;
    var ISActive = $(this).is(":checked")
    invoicelisting.UpdateInvoiceStatus("Material", invoiceid, ISActive);
})

$(document).on("change", ".chkapproved", function () {
    var invoiceid = invoicelisting.invoiceid;
    var ISActive = $(this).is(":checked")
    invoicelisting.UpdateInvoiceStatus("approved", invoiceid, ISActive);
})
//*********************************************************End Quick view Grid*****************************************************************

//****************myob code******************************//


$(document).on('click', '.btnsyncmyobinvoicelist', function () {
    $(".btnsyncmyobinvoicelist").text("Syncing Invoices...");
    $.ajax({
        //  url: common.SitePath + "/Employee/Invoice/MyobSync",
        url: common.SitePath + "/Employee/Invoice/SynctoMyobInvoiceList",
        data: {},
        type: 'Get',
        async: true,
        success: function (data) {
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Syncing Invoices .It will update all invoices soon.....!</strong>");
            $(".jobalert").fadeOut(2000, function() {
                $(this).html("");
            });
            //USed for the live file 
            //window.location.href = "https://secure.myob.com/oauth2/account/authorize?client_id=enpstq27p2t9fq32aj4bkfx9&redirect_uri=http://localhost:54245/Employee/Invoice/SynctoMyobInvoiceList&response_type=code&scope=CompanyFile";
            window.location.href = "https://secure.myob.com/oauth2/account/authorize?client_id=enpstq27p2t9fq32aj4bkfx9&redirect_uri=http://www.srag-portal.com/Employee/Invoice/SynctoMyobInvoiceList&response_type=code&scope=CompanyFile";
        },
        error: function () {
           
            //USed for the live file 
           // window.location.href = "https://secure.myob.com/oauth2/account/authorize?client_id=enpstq27p2t9fq32aj4bkfx9&redirect_uri=http://localhost:54245/Employee/Invoice/SynctoMyobInvoiceList&response_type=code&scope=CompanyFile";
            window.location.href = "https://secure.myob.com/oauth2/account/authorize?client_id=enpstq27p2t9fq32aj4bkfx9&redirect_uri=http://www.srag-portal.com/Employee/Invoice/SynctoMyobInvoiceList&response_type=code&scope=CompanyFile";

        }
    });
})