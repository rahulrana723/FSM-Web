$(document).on('click', '#addcustomerbilling', function () {
    var hidebillingbtn = $(this).attr("data-hidebillingbtn");
    $('#CustomerBillingErrorDiv').empty();
    $.get(common.SitePath + "/Customer/Customer/ManageCustomerBillingsAddPartial?CustomerGeneralInfoId=" + FSM.GeneralInfoId + "&HideBillingBtn=" + hidebillingbtn, function (data) {

        $('#dvCustBillForm').css('display', 'block');
        $('#dvCustBillAddNew').css('display', 'none');

        $('#dvCustBillFormPartial').empty();
        $('#dvCustBillFormPartial').append(data);
    });
});

$(document).ready(function () {
    $('.form-control#ddPageSize').val(FSM.SelectedVal);
    if (FSM.BillingCount==1) {
        $.get(common.SitePath + "/Customer/Customer/ManageCustomerBillingPartial?BillingAddressId=" + FSM.BillingAddressId, function (data) {
            $('#dvCustBillForm').css('display', 'block');
            $('#dvCustBillAddNew').css('display', 'none');
            $('#dvCustBillFormPartial').empty();
            $('#dvCustBillFormPartial').append(data);
        });
    }
    else {
        $('#dvCustBillAddNew').show();
        $('#dvCustBillForm').hide();
    }
});

$('#btnbillingsearch').click(function () {
    var name = $('#Name').val();
    var page_size = $('#ddPageSize').val();

    if (page_size==undefined) {
        page_size = FSM.SelectedVal;
    }


    $.get(common.SitePath + "/Customer/Customer/CustomerBillingList?Name=" + name + "&page_size=" + page_size + "&CustomerGeneralInfoId=" + FSM.GeneralInfoId, function (data) {
        $('#dvShowCustBillList').empty();
        $('#dvShowCustBillList').append(data);
    });

});
$(document).on("keypress", ".BillingSrchBox", function (e) {
    var key = e.which;
    if (key == 13)  // the enter key code
    {
        $('#btnbillingsearch').click();
        return false;
    }
});

$(document).off('click', '.grid-pager .pagination a').on('click', '.grid-pager .pagination a', function (event) {
        event.preventDefault();
        var querystring = $(this).prop('href').split("?")[1];
        var values = querystring.split("&");
        var pagenumparam = '';
        var page_size = $('#ddPageSize').val();
        if (page_size==undefined) {
            page_size = FSM.SelectedVal;
        }

        var name = $('#Name').val();

        $.each(values, function (i, item) {
            if (item.indexOf("grid-page") > -1) {
                pagenumparam = item;
            }
        })

        $.get(common.SitePath + "/Customer/Customer/CustomerBillingList?" + pagenumparam + "&page_size=" + page_size + "&Name=" + name + "&CustomerGeneralInfoId=" + FSM.GeneralInfoId, function (data) {
            $('#dvShowCustBillList').empty();
            $('#dvShowCustBillList').append(data);
            $('#ddPageSize').val(page_size);
        });

    });


$(document).off('click', '.grid-header-title a').on('click', '.grid-header-title a', function (event) {
    event.preventDefault();

    var pagenum = $('#dvShowCustBillList li.active span').text();
    var page_size = $('#ddPageSize').val();
    if (page_size==undefined) {
        page_size=FSM.SelectedVal;
    }

    var elementparam = $(this).attr('href');

    if (pagenum != undefined && pagenum != "") {
        elementparam = elementparam + "&grid-page=" + pagenum;
    }

    if (page_size != undefined && page_size != "" && elementparam.indexOf("page_size") < 0) {
        elementparam = elementparam + "&page_size=" + page_size;
    }

    if (elementparam.indexOf("CustomerGeneralInfoId")<0) {
        //elementparam = elementparam + "&CustomerGeneralInfoId=" + FSM.GeneralInfoId;
        }
    var url = common.SitePath + "/Customer/Customer/CustomerBillingList" + elementparam;
    $.get(url, function (data) {
        $('#dvShowCustBillList').empty();
        $('#dvShowCustBillList').append(data);
        $('#ddPageSize').val(page_size);
    });
});