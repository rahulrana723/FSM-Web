
$(document).off('click', '.grid-pager .pagination a').on('click', '.grid-pager .pagination a', function (event) {
    event.preventDefault();
    var querystring = $(this).prop('href').split("?")[1];
    var values = querystring.split("&");
    var pagenumparam = '';
    var page_size = $('#ddPageSize1').val();
    if (page_size == undefined) {
        page_size = FSM.SelectedVal;
    }

    var name = $('#srchBoxInvoice').val();

    $.each(values, function (i, item) {
        if (item.indexOf("grid-page") > -1) {
            pagenumparam = item;
        }
    })

    $.get(common.SitePath + "/Customer/Customer/InvoiceHistoryPartial?" + pagenumparam + "&page_size=" + page_size + "&Keyword=" + name + "&CustomerGeneralInfoId=" + FSM.GeneralInfoId, function (data) {
        $('#divInvoiceList').empty();
        $('#divInvoiceList').append(data);
        $('#ddPageSize').val(page_size);
    });
});

$(document).off('click', '.grid-header-title a').on('click', '.grid-header-title a', function (event) {
    event.preventDefault();
    var pagenum = $('li.active span').text();
    var Name = $("#srchBoxInvoice").val();
    var page_size = $('#ddPageSize1').val();
    if (page_size == undefined) {
        page_size = FSM.SelectedVal;
    }

    var elementparam = $(this).attr('href');

    if (pagenum != undefined && pagenum != "") {
        elementparam = elementparam + "&grid-page=" + pagenum;
    }

    if (page_size != undefined && page_size != "" && elementparam.indexOf("page_size") < 0) {
        elementparam = elementparam + "&page_size=" + page_size;
    }

    if (elementparam.indexOf("CustomerGeneralInfoId") < 0) {
        elementparam = elementparam + "&CustomerGeneralInfoId=" + FSM.GeneralInfoId;
    }
    elementparam = elementparam + "&Keyword=" + Name;
    var url = common.SitePath + "/Customer/Customer/InvoiceHistoryPartial" + elementparam;
    $.get(url, function (data) {
        $('#divInvoiceList').empty();
        $('#divInvoiceList').append(data);
        $('#ddPageSize').val(page_size);
    });

});