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
    window.location.href = FSM.URL + "?page_size=" + page_size;
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

    var keyword = $('#SearchKeyword').val();


    if (keyword != "" && keyword != undefined) {
        elementparam = elementparam + '&Searchkeyword=' + keyword;
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
    var keyword = $('#SearchKeyword').val();

    if (keyword != "" && keyword != undefined) {
        elementparam = elementparam + '&Searchkeyword=' + keyword;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }
    $(this).attr('href', elementparam);

});

function DeleteJobpurchaseOrder(id) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete?");
    $(".btnconfirm").attr("id", id);
    $(".modal-title").html("Delete job purchase Order!");
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Purchase/DeleteJobPurchaseorder",
        data: { purchaserId: id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () { window.location.href = common.SitePath + "/Employee/Purchase/ViewJobsPurchaseOrder"; });
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})

$(document).on('dblclick', '.cssEditPurchase', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();

    window.location = FSM.EditPurchase + "?Purchaseorderid=" + id;
});