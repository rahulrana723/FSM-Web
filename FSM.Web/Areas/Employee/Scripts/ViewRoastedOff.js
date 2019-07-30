

$(document).ready(function () {
    $('#ddPageSize').val(FSM.PageSize);
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
        FSM.Message = "";
        $(".jobalert").delay(2000).fadeOut();
    }
});

$(".btnRoasted").click(function () {
    $.ajax({
        url: common.SitePath + "/Employee/Employee/_AddRoastedOff",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#updateRoastedOffDv").html(data);
            $("#modalRoastedOff").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$('#searchRoasted').on('click', function () {
    var url = FSM.RoastedUrl + '?';
    var pagesize = $('#ddPageSize').val();

    var searchRoasted = $('#SearchKeyword').val();

    if (searchRoasted != "" && searchRoasted != undefined && url.indexOf("searchRoasted") < 0) {
        url = url + "EmployeeKeyword=" + searchRoasted + "&";
    }
    if (pagesize != "" && pagesize != undefined && url.indexOf("PageSize") < 0) {
        url = url + "PageSize=" + pagesize + "&";
    }

    $.get(url, function (data) {
        $('#dvShowEmpRoasted').empty();
        $('#dvShowEmpRoasted').append(data);
    });

});


$(document).on('click', '#dvShowEmpRoasted .grid-pager .pagination a', function (event) {

    event.preventDefault();

    var url = FSM.RoastedUrl + '?';
    var pagesize = $('#ddPageSize').val();
    var EmployeeKeyword = $('#SearchKeyword').val();

    var querystring = $(this).prop('href').split("?")[1];
    var values = querystring.split("&");
    var pagenumparam = '';
    var sortingorder = '';
    var sortingdir = '';

    $.each(values, function (i, item) {
        if (item.indexOf("grid-page") > -1) {
            pagenumparam = item;
        }
        else if (item.indexOf("grid-column") > -1) {
            sortingorder = item;
        }
        else if (item.indexOf("grid-dir") > -1) {
            sortingdir = item;
        }
    })

    if (pagenumparam != "" && url.indexOf("grid-page") < 0) {
        url = url + pagenumparam + "&";
    }

    if (sortingorder != "" && url.indexOf("grid-column") < 0) {
        url = url + sortingorder + "&";
    }

    if (sortingdir != "" && url.indexOf("grid-dir") < 0) {
        url = url + sortingdir + "&";
    }

    if (EmployeeKeyword != "" && EmployeeKeyword != undefined && url.indexOf("EmployeeKeyword") < 0) {
        url = url + "EmployeeKeyword=" + EmployeeKeyword + "&";
    }
   
    if (pagesize != "" && pagesize != undefined && url.indexOf("PageSize") < 0) {
        url = url + "PageSize=" + pagesize + "&";
    }

    $.get(url, function (data) {
        $('#dvShowEmpRoasted').empty();
        $('#dvShowEmpRoasted').append(data);
    });

});

$(document).on('click', '#dvShowEmpRoasted .grid-header-title a', function (event) {

    event.preventDefault();
    var url = FSM.RoastedUrl + '?';
    var pagesize = $('#ddPageSize').val();
    var EmployeeKeyword = $('#SearchKeyword').val();

    var querystring = $(this).prop('href').split("?")[1];
    var values = querystring.split("&");
    var pagenum = $('.active span').text();
    var pagenumparam = 'grid-page=' + pagenum;
    var sortingorder = '';
    var sortingdir = '';

    $.each(values, function (i, item) {
        if (item.indexOf("grid-column") > -1) {
            sortingorder = item;
            FSM.SortColumn = sortingorder;
        }
        else if (item.indexOf("grid-dir") > -1) {
            sortingdir = item;
            FSM.SortDir = sortingdir;
        }
    })

    if (pagenumparam != "" && url.indexOf("grid-page") < 0) {
        url = url + pagenumparam + "&";
    }

    if (sortingorder != "" && url.indexOf("grid-column") < 0) {
        url = url + sortingorder + "&";
    }

    if (sortingdir != "" && url.indexOf("grid-dir") < 0) {
        url = url + sortingdir + "&";
    }

    if (EmployeeKeyword != "" && EmployeeKeyword != undefined && url.indexOf("EmployeeKeyword") < 0) {
        url = url + "EmployeeKeyword=" + EmployeeKeyword + "&";
    }
   
    if (pagesize != "" && pagesize != undefined && url.indexOf("PageSize") < 0) {
        url = url + "PageSize=" + pagesize + "&";
    }

    $.get(url, function (data) {
        $('#dvShowEmpRoasted').empty();
        $('#dvShowEmpRoasted').append(data);
    });

});


$(document).off("click", ".updateEmpRoasted").on("click", ".updateEmpRoasted", function (event) {
    event.preventDefault();

    var id = $(this).attr("data-id");

    data = { id: id };

    $.get(FSM.EditRoastedUrl, data, function (result) {
        $('#updateRoastedOffDv').html(result);
        $('#modalRoastedOff').modal('show');
    });
});

//delete holiday
function DeleteRoastedOff(id) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete?");
    $(".btnconfirm").attr("id", id);
    $(".modal-title").html("Delete Rostered On/Off!");
}

$(document).on('click', ".btnconfirm", function () {
    var Id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Employee/DeleteRoastedOff",
        data: { id: Id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () { window.location.href = common.SitePath + "/Employee/Employee/ViewRoastedOff"; });
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})
$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.EmpRoastedUrl + "?PageSize=" + page_size;
});

//Enter button press then search record
$(document).on("keypress", "#SearchKeyword", function (e) {

    var key = e.which;
    if (key == 13)  // the enter key code
    {
        $('#searchRoasted').click();
        return false;
    }
});