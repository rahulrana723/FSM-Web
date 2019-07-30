$(document).ready(function () {
    $('#StartDate').datepicker({ dateFormat: 'dd/mm/yy' }).attr('readonly', 'readonly');;
    $('#EndDate').datepicker({ dateFormat: 'dd/mm/yy' }).attr('readonly', 'readonly');;
    //Display message alert
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
})
//Add  holiday 
$(".btnaddeditholiday").click(function () {
    $.ajax({
        url: common.SitePath + "/Employee/Employee/_AddHolidays",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowPublicHolidayPopup").empty();
            $("#divShowPublicHolidayPopup").html(data);
            $("#modalHoliday").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).on('dblclick', '.cssEditPublic', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();

    $.get(common.SitePath + "/Employee/Employee/_EditHolidays?id=" + id, function (data) {
        $("#divShowPublicHolidayPopup").empty();
        $("#divShowPublicHolidayPopup").html(data);
        $(".modalholidaytitle").text("Edit Holiday");
        $("#modalHoliday").modal("show");
    });
});

//Edit holiday 
$(".btnedit").click(function () {
    var id = $(this).attr("id");
    $.ajax({
        url: common.SitePath + "/Employee/Employee/_EditHolidays",
        data: { id: id },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowPublicHolidayPopup").empty();
            $("#divShowPublicHolidayPopup").html(data);
            $(".modalholidaytitle").text("Edit Holiday");
            $("#modalHoliday").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});
//delete holiday
function DeletePublicHoliday(id) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete?");
    $(".btnconfirm").attr("id", id);
    $(".modal-title").html("Delete Leave!");
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Employee/DeleteHoliday",
        data: { Id: id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () { window.location.href = common.SitePath + "/Employee/Employee/GetPublicHoliday"; });
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})
//page size change 
$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?page_size=" + page_size;
});
//sorting 
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
    var Searrchkeyword = $('#SearchText').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();
    if (Searrchkeyword != "" && Searrchkeyword != undefined) {
        elementparam = elementparam + '&searchkeyword=' + Searrchkeyword;
    }
    if (EndDate != "" && EndDate != undefined) {
        elementparam = elementparam + '&EndDate=' + EndDate;
    }
    if (StartDate != "" && StartDate != undefined) {
        elementparam = elementparam + '&StartDate=' + StartDate;
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
//paging 
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
    var Searrchkeyword = $('#SearchText').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();
    if (Searrchkeyword != "" && Searrchkeyword != undefined) {
        elementparam = elementparam + '&searchkeyword=' + Searrchkeyword;
    }
    if (EndDate != "" && EndDate != undefined) {
        elementparam = elementparam + '&EndDate=' + EndDate;
    }
    if (StartDate != "" && StartDate != undefined) {
        elementparam = elementparam + '&StartDate=' + StartDate;
    }
    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }
    $(this).attr('href', elementparam);
});

