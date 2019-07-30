$(document).ready(function () {
    if ($(".pagination").length > 0) {
        $('#ddPageSize').parent().css('margin-top', '-72px');
    }
    else {
        $('#ddPageSize').parent().css('margin-top', '8px');
    }
});
$('#EditBillingDetail, #DeleteBillingDetail').click(function (event) {
    event.preventDefault();
    $('#BillingAddressErrorDiv').empty();
    var type = $(this).text();
    var querystring = $(this).prop('href').split("/");
    var billingdetailid = querystring[querystring.length - 1];
    if (type == "Edit") {
        $.get(common.SitePath + "/Customer/Customer/ManageCustomerBillingPartial?BillingAddressId=" + billingdetailid, function (data) {
            $('#dvCustBillForm').css('display', 'block');
            $('#dvCustBillAddNew').css('display', 'none');
            $('#dvCustBillFormPartial').empty();
            $('#dvCustBillFormPartial').append(data);
        });
    }
    else if (type == "Delete") {

        $(".commonpopup").modal('show');
        $(".modal-title").html("Delete Billing Address !");
        $(".alertmsg").text("Are you sure to delete?");
        $(".btnconfirm").attr("billid", billingdetailid);
    }
});
    $(document).off('click',  ".btnconfirm").on('click', ".btnconfirm", function () {
    var billingdetailid = $(this).attr("billid");
    $.post(common.SitePath + "/Customer/Customer/DeleteCustomerBilling", { BillingAddressId: billingdetailid }, function (data) {
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

$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    if (page_size==undefined) {
        page_size = FSM.SelectedVal;
    }
    $.get(common.SitePath + "/Customer/Customer/CustomerBillingList?page_size=" + page_size + "&CustomerGeneralInfoId=" + FSM.GeneralInfoId, function (data) {
        $('#dvShowCustBillList').empty();
        $('#dvShowCustBillList').append(data);
        $('#ddPageSize').val(page_size);
    });

});

$(document).on('dblclick', '.cssEditBilling', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();

    $.get(common.SitePath + "/Customer/Customer/ManageCustomerBillingPartial?BillingAddressId=" + id, function (data) {
        $('#dvCustBillForm').css('display', 'block');
        $('#dvCustBillAddNew').css('display', 'none');
        $('#dvCustBillFormPartial').empty();
        $('#dvCustBillFormPartial').append(data);
    });
});
