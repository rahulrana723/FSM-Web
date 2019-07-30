$(document).ready(function () {
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
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?JobId=" + FSM.JobId + "&InvoiceId=" + FSM.InvoiceId + "&page_size=" + page_size;
});

function DeleteJobpurchaseOrder(id) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete?");
    $(".btnconfirm").attr("id", id);
    $(".modal-title").html("Delete job purchase Order!");
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    var Invoiceid = FSM.InvoiceId;
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Invoice/DeleteJobPurchaseorder",
        data: { jobid: id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () { window.location.href = common.SitePath + "/Employee/Invoice/ViewJobsInvoicePurchaseOrder?JobId='" + jobid + "'&InvoiceId='" + Invoiceid + "'"; });
            location.reload();
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})
$("#btnBackForInvoice").on("click", function (e) {
    var id = FSM.InvoiceId; // form id
    window.location.href = common.SitePath + '/Employee/Invoice/SaveInvoiceInfo?id=' + id + '&activetab=Job Detail'
});