

$(document).on('click', '.grid-pager .pagination a', function (event) {
    event.preventDefault();
    var querystring = $(this).prop('href').split("?")[1];
    var values = querystring.split("&");
    var pagenumparam = '';
    var page_size = $('#ddPageSize').val();
    if (page_size == undefined) {
        page_size = FSM.PageSize;
    }

    var name = $('#FirstNameSearch').val();

    $.each(values, function (i, item) {
        if (item.indexOf("grid-page") > -1) {
            pagenumparam = item;
        }
    })

    $.get(common.SitePath + "/Employee/CustomerJob/ViewCustomerContactLogPartial?JobId=" + FSM.JobId + "&page_size=" + page_size + "&Keyword=" + name + "&" + pagenumparam, function (data) {
        $('#divContactPartial').empty();
        $('#divContactPartial').append(data);
        $('#ddPageSize').val(page_size);
    });
});

 

function DeleteJobcontactLog(id, customerid) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure to delete?");
    $(".btnconfirm").attr("id", id);
    $(".btnconfirm").attr("customerid", customerid);
    $(".modal-title").html("Delete contact log !");
}
$(document).on('click', ".btnconfirm", function () {
    var contactid = $(this).attr("id");
    var CustomerGeneralinfoid = $(this).attr("customerid");
    var pagenum = $('.active span').text();

    $.ajax({
        url: common.SitePath + "/Employee/Invoice/DeleteCustomerContactLog",
        data: { Customercontactid: contactid, PageNum: pagenum },
        type: 'Get',
        async: false,
        success: function (data) {
            $('#divContactPartial').empty();
            $('#divContactPartial').append(data);
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () {
            });

        },
        error: function () {
            alert("something seems wrong");
        }
    });
})

$(document).on('click', '.grid-header-title a', function (event) {
    event.preventDefault();
    var pagenum = $('li.active span').text();
    var Name = $("#FirstNameSearch").val();
    var page_size = $('#ddPageSize').val();
    if (page_size==undefined) {
        page_size = FSM.PageSize;
    }

    var elementparam = $(this).attr('href');

   
    if (page_size != undefined && page_size != "" && elementparam.indexOf("page_size") < 0) {
        elementparam = elementparam + "&page_size=" + page_size;
    }

    elementparam = elementparam + "&JobId=" + FSM.JobId + "&Keyword=" + Name + "&grid-page=" + pagenum;

    var url = common.SitePath + "/Employee/CustomerJob/ViewCustomerContactLogPartial" + elementparam;
    $.get(url, function (data) {
        $('#divContactPartial').empty();
        $('#divContactPartial').append(data);
        $('#ddPageSize').val(page_size);
    });
     
});


$(".btnaddlog").click(function () {
    var JobGeneralinfoid = $(this).attr("JobId");
    $.ajax({
        url: common.SitePath + "/Employee/Invoice/_CustomercontactAddEdit",
        data: { JobId: JobGeneralinfoid },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowPopup").html(data);
            $("#modalContactlog").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});