$(document).off('click', '.grid-pager .pagination a').on('click', '.grid-pager .pagination a', function (event) {
    event.preventDefault();
    var querystring = $(this).prop('href').split("?")[1];
    var values = querystring.split("&");
    var pagenumparam = '';
    var page_size = $('#ddPageSize').val();
    if (page_size==undefined) {
        page_size=FSM.SelectedVal;
    }

    var name = $('#srchBox').val();

    $.each(values, function (i, item) {
        if (item.indexOf("grid-page") > -1) {
            pagenumparam = item;
        }
    })

    $.get(common.SitePath + "/Customer/Customer/JobHistoryPartial?" + pagenumparam + "&page_size=" + page_size + "&Keyword=" + name + "&CustomerGeneralInfoId="+FSM.GeneralInfoId, function (data) {
        $('#divjobList').empty();
        $('#divjobList').append(data);
        $('#ddPageSize').val(page_size);
    });
});

$(document).off('click', '.grid-header-title a').on('click', '.grid-header-title a', function (event) {
    event.preventDefault();
    var pagenum = $('li.active span').text();
    var Name = $("#srchBox").val();
    var page_size = $('#ddPageSize').val();
    if (page_size==undefined) {
        page_size = FSM.SelectedVal;
    }

    var elementparam = $(this).attr('href');

    if (pagenum != undefined && pagenum != "") {
        elementparam = elementparam + "&grid-page=" + pagenum;
    }

    if (page_size != undefined && page_size != "" && elementparam.indexOf("page_size") < 0) {
        elementparam = elementparam + "&page_size=" + page_size;
    }

    if (elementparam.indexOf("CustomerGeneralInfoId")<0) {
        elementparam = elementparam + "&CustomerGeneralInfoId=" + FSM.GeneralInfoId;
    }
    elementparam=elementparam+"&Keyword="+Name;
    var url = common.SitePath + "/Customer/Customer/JobHistoryPartial" + elementparam;
    $.get(url, function (data) {
        $('#divjobList').empty();
        $('#divjobList').append(data);
        $('#ddPageSize').val(page_size);
    });
     
});
$(document).on('dblclick', '.cssEditJob', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();

   window.location = FSM.EditJob + "/" + id;
    //window.open(FSM.EditJob + "/" + id, '_blank');
});