
$('#DisplayJobStocksViewModel_Price').keyup(function (event) {
    if (this.value != this.value.replace(/[^0-9\.]/g, '')) {
        this.value = this.value.replace(/[^0-9\.]/g, '');
    }

});
$('#DisplayJobStocksViewModel_Quantity').keyup(function (e) {
    if (this.value != this.value.replace(/[^0-9\.]/g, '')) {
        this.value = this.value.replace(/[^0-9\.]/g, '');
    }
});

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
    var Jobid = $('#DisplayJobStocksViewModel_JobId').val();
    if (Jobid != "" && Jobid != undefined) {
        elementparam = elementparam + '&Jobid=' + Jobid;
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
        var Jobid = $('#DisplayJobStocksViewModel_JobId').val();
        if (Jobid != "" && Jobid != undefined) {
            elementparam = elementparam + 'Jobid=' + Jobid;
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
    var Jobid = $('#DisplayJobStocksViewModel_JobId').val();

    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?page_size=" + page_size + '&Jobid=' + Jobid;

});

$(document).on("click", ".editJobStock", function () {
    var Id = $(this).attr("Id");
    var stockid = $(this).attr("stockId");
    var pagenum = $('.active span').text();
    var invoiceid = $(".invoice").val();
    $.ajax({
        url: common.SitePath + "/Employee/Invoice/EditManageStockInfo",
        data: { ID: Id, stockID: stockid, PageNum: pagenum ,invoiceid:invoiceid},
        type: 'Post',
        async: false,
        success: function (data) {
            var json = JSON.parse(data.json);
            $('#DisplayJobStocksViewModel_ID').val(json.ID);
            $('#DisplayJobStocksViewModel_StockID').val(json.StockID);
            $('#DisplayJobStocksViewModel_UnitMeasure').val(json.UnitMeasure);
            $('#DisplayJobStocksViewModel_Price').val(json.Price);
            $('#DisplayJobStocksViewModel_Quantity').val(json.Quantity);
            $('#DisplayJobStocksViewModel_JobId').val(json.JobId);
            $('#hdnAvailableQuantity').val(json.AvailableQuantity);
            $('#DisplayJobStocksViewModel_AvailableQuantity').val(json.AvailableQuantity);
            $('#DisplayJobStocksViewModel_InvoiceId').val(invoiceid);
        },
        error: function () {
            alert("something seems wrong");
        }
    })

})


$(document).on("click", ".deletaJobStock", function () {
    var confirmdelete = confirm('Are you sure to delete stock?');
    if (confirmdelete) {
        var Id = $(this).attr("Id");
        var stockid = $(this).attr("stockId");
        var invoiceid = $(".invoice").val();
        var pagenum = $('.active span').text();
        $.ajax({
            url: common.SitePath + "/Employee/Invoice/DeleteManageStockInfo",
            data: { ID: Id, stockID: stockid, PageNum: pagenum, invoiceid: invoiceid },
            type: 'Get',
            async: false,
            success: function (data) {
                alert("Record deleted successfully.");
                location.reload();
            },
            error: function () {
                alert("something seems wrong");
            }
        });
        function SuccessLog(data) {

        }
    }

})



$(document).on("change", ".getjobunit", function () {
    var stockId = $(this).val();
    $.ajax({
        url: common.SitePath + "/Employee/Job/GetStockData",
        data: { StockId: stockId },
        type: 'Post',
        async: false,
        success: function (data) {
            var json = JSON.parse(data.json);
            $('#DisplayJobStocksViewModel_UnitMeasure').val(json.UnitMeasure);
            $('#DisplayJobStocksViewModel_Price').val(json.Price);
            $('#hdnAvailableQuantity').val(json.Available);
            $('#DisplayJobStocksViewModel_AvailableQuantity').val(json.Available);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
    function SuccessLog(data) {

    }
})
function ValidateQuantity() {
    var available = $('#DisplayJobStocksViewModel_Quantity').val();
    var quantity = $("#hdnAvailableQuantity").val();
    var ReqQuantity = $("#DisplayJobStocksViewModel_Quantity").val();
    var ReqItem = $("#DisplayJobStocksViewModel_StockID").val();

    if (ReqItem == "") {
        $("#validate_stockitem").text("Please select item");
        return false;
    }
    else {
        $("#validate_stockitem").text("");
    }
    if (ReqQuantity == "") {
        $("#quantity-vaidate").text("Please enter quantity");
        return false;
    }
    else {
        $("#quantity-vaidate").text("");
    }

    if (parseInt(available) <= parseInt(quantity)) {
        return true;
    }
    else {
        if (quantity != "" && quantity != null) {
            $("#quantity-vaidate").text("Maximum available quantity=" + quantity);
        }
        else
        {
            $("#quantity-vaidate").text("Quantity is not available");
        }
        return false;
    }
}

$("#btnBackForInfoInvoice").on("click", function (e) {
    var id = FSM.InvoiceId; // form id
    window.location.href = common.SitePath + '/Employee/Invoice/SaveInvoiceInfo?id=' + id + '&activetab=Invoice Detail'
});

