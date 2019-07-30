$(document).ready(function myfunction() {

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

$('.grid-header-title a').on('click', function () {

    var elementparam = $(this).attr('href');

    var index = elementparam.indexOf("grid-column");
    if (index >= 1) {
        var paramarray = elementparam.split("&");
        elementparam = '?';
        $.each(paramarray, function (i, item) {

            if (item.indexOf("grid-column") > -1 || item.indexOf("grid-dir") > -1) {
                elementparam = elementparam + item + '&';
            }
        });

        elementparam = elementparam.substring(0, (elementparam.length - 1));
    }
    var invoiceId = $('#invoiceiD').val();
    if (invoiceId != "" && invoiceId != undefined) {
        elementparam = elementparam + '&InvoiceId=' + invoiceId;
    }
    var pagenum = $('.active span').text();
    if (pagenum != "" && pagenum != undefined) {
        elementparam = elementparam + '&grid-page=' + pagenum;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);

});


$('.grid-footer a').on('click', function myfunction() {

    var elementparam = $(this).attr('href');

    var index = elementparam.indexOf("grid-page");
    var paramarray = elementparam.split("&");
    if (index >= 1) {

        elementparam = '?';
        $.each(paramarray, function (i, item) {

            if (item.indexOf("grid-page") > -1) {
                elementparam = elementparam + item + '&';
            }
        });
        var invoiceId = $('#invoiceiD').val();
        if (invoiceId != "" && invoiceId != undefined) {
            elementparam = elementparam + 'InvoiceId=' + invoiceId;
        }
        elementparam = elementparam.substring(0, (elementparam.length));
    }
    else {
        elementparam = paramarray[0];
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);


});


$('#ddPageSize').on('change', function () {
    var invoiceId = $('#invoiceiD').val();

    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?page_size=" + page_size + '&InvoiceId=' + invoiceId;

});

$(document).on("click", ".editJclInvoice", function () {
    var Id = $(this).attr("Id");
    var jclId = $(this).attr("JCLId");
    var pagenum = $('.active span').text();
    var invoiceid = $("#invoiceiD").val();
    $.ajax({
        url: common.SitePath + "/Employee/Invoice/EditManageJCLItem",
        data: { ID: Id, JCLId: jclId, PageNum: pagenum, invoiceid: invoiceid },
        type: 'Post',
        async: false,
        success: function (data) {
            var json = JSON.parse(data.json);
            $('#InvoiceJCLId').val(Id);
            $('#invoiceiD').val(invoiceid);
            $('#itemName').val(json.JCLId);
            $('#displayJCLItemInvoiceViewModel_Price').val(json.Price);
            $('#displayJCLItemInvoiceViewModel_BonusPerItem').val(json.BonusPerItem);
            $('#displayJCLItemInvoiceViewModel_DefaultQty').val(json.DefaultQty);
        },
        error: function () {
            alert("something seems wrong");
        }
    })

})

function DeleteJclInvoiceitem(Id) {
  $(".commonpopup").modal("show");
  $(".alertmsg").text("Are you sure to delete Sale Item?");
  $(".btnconfirm").attr("id", Id);
  $(".modal-title").html("Delete Sale Item!");
}

$(document).on('click', ".btnconfirm", function () {
    var ID = $(this).attr("id");
    $.ajax({
        type: 'POST',
        url: common.SitePath + "/Employee/Invoice/DeleteJCLInvoice",
        data: { Id: ID },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
            setTimeout(function () {
                location.reload();
            }, 3000);
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})


$(document).on("change", "#itemName", function () {
    var JCLID = $(this).val();
    if (JCLID == "")
    {
        return false;
    }
    $.ajax({
        url: common.SitePath + "/Employee/Invoice/GetJCLData",
        data: { JCLId: JCLID },
        type: 'Post',
        async: false,
        success: function (data) {
            var json = JSON.parse(data.json);
            $('#displayJCLItemInvoiceViewModel_Price').val(json.Price);
            $('#displayJCLItemInvoiceViewModel_BonusPerItem').val(json.BonusPerItem);
            $('#displayJCLItemInvoiceViewModel_DefaultQty').val(json.DefaultQty);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
})
function ValidateJCLItem() {
    var ReqItem = $("#itemName").val();

    if (ReqItem == "") {
        $("#validate_jclitem").text("Please select item");
        return false;
    }
    else {
        $("#validate_jclitem").text("");
    }
}

$("#btnBackForInfoInvoice").on("click", function (e) {
    var id = FSM.InvoiceId; // form id
    window.location.href = common.SitePath + '/Employee/Invoice/SaveInvoiceInfo?id=' + id + '&activetab=Invoice Detail'
});

