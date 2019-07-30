$(document).ready(function () {
    $('.DatePicker').datepicker({ minDate: 0, dateFormat: 'dd/mm/yy' });


    $('#StartDate').datepicker({
        dateFormat: 'dd/mm/yy'
    }).attr('readonly', 'readonly');

    $('#EndDate').datepicker({
        dateFormat: 'dd/mm/yy'
    }).attr('readonly', 'readonly');

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
        //else if (FSM.Message == "3") {
        //    $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
        //}
        FSM.Message = "";
        $(".jobalert").delay(2000).fadeOut();
    }

});

$('#searchVacation').on('click', function () {
    
    var pattern =/^([0-9]{2})\/([0-9]{2})\/([0-9]{4})$/;
    var url = FSM.VacationUrl + '?';
    var pagesize = $('#ddPageSize').val();

    var EmployeeKeyword = $('#EmployeeKeyword').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();

    var KeywordLength = (EmployeeKeyword != null && EmployeeKeyword != undefined) ? EmployeeKeyword.length : 0;
    if (KeywordLength > 50) {
        alert('Employee Id or Name length should not be more than 50 characters !');
        $('#EmployeeKeyword').val('');
        $('#EmployeeKeyword').focus();
        return false;
    }

    var validStartDate = true;
    if (StartDate != "" && StartDate != undefined) {
        validStartDate = pattern.test(StartDate);
    }

    var validEndDate = true;
    if (EndDate != "" && EndDate != undefined) {
        validEndDate = pattern.test(EndDate);
    }

    if (!validStartDate || !validEndDate) {
        alert('Start or End dates are not valid');
        return false;
    }

    if (EmployeeKeyword != "" && EmployeeKeyword != undefined && url.indexOf("EmployeeKeyword") < 0) {
        url = url + "EmployeeKeyword=" + EmployeeKeyword + "&";
    }
    if (StartDate != "" && StartDate != undefined && url.indexOf("StartDate") < 0) {
        url = url + "StartDate=" + StartDate + "&";
    }
    if (EndDate != "" && EndDate != undefined && url.indexOf("EndDate") < 0) {
        url = url + "EndDate=" + EndDate + "&";
    }
    if (pagesize != "" && pagesize != undefined && url.indexOf("PageSize") < 0) {
        url = url + "PageSize=" + pagesize + "&";
    }

    $.get(url, function (data) {
        $('#dvShowEmpVacation').empty();
        $('#dvShowEmpVacation').append(data);
    });

});

$(document).on('click', '#dvShowEmpVacation .grid-pager .pagination a', function (event) {

    event.preventDefault();

    var url = FSM.VacationUrl + '?';
    var pagesize = $('#ddPageSize').val();
    var EmployeeKeyword = $('#EmployeeKeyword').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();

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
    if (StartDate != "" && StartDate != undefined) {
        url = url + "StartDate=" + StartDate + "&";
    }
    if (EndDate != "" && EndDate != undefined) {
        url = url + "EndDate=" + EndDate + "&";
    }

    if (pagesize != "" && pagesize != undefined && url.indexOf("PageSize") < 0) {
        url = url + "PageSize=" + pagesize + "&";
    }

    $.get(url, function (data) {
        $('#dvShowEmpVacation').empty();
        $('#dvShowEmpVacation').append(data);
    });

});

$(document).on('click', '#dvShowEmpVacation .grid-header-title a', function (event) {
    
    event.preventDefault();
    var url = FSM.VacationUrl + '?';
    var pagesize = $('#ddPageSize').val();
    var EmployeeKeyword = $('#EmployeeKeyword').val();
    var StartDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();

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
    if (StartDate != "" && StartDate != undefined) {
        url = url + "StartDate=" + StartDate + "&";
    }
    if (EndDate != "" && EndDate != undefined) {
        url = url + "EndDate=" + EndDate + "&";
    }

    if (pagesize != "" && pagesize != undefined && url.indexOf("PageSize") < 0) {
        url = url + "PageSize=" + pagesize + "&";
    }

    $.get(url, function (data) {
        $('#dvShowEmpVacation').empty();
        $('#dvShowEmpVacation').append(data);
    });

});

function ApproveVacation(event,id) {

    event.preventDefault();
    var pagenum = $('.active span').text();
    var pagenumparam = '&grid-page=' + pagenum;
    var sortingorder = '&' + FSM.SortColumn;
    var sortingdir = '&' + FSM.SortDir;
    var pagesize = "&PageSize=" + $('#ddPageSize').val();

    var EmployeeKeyword = "&EmployeeKeyword=" + $('#EmployeeKeyword').val();
    var StartDate = "&StartDate=" + $('#StartDate').val();
    var EndDate = "&EndDate=" + $('#EndDate').val();

    var url = FSM.ApproveVacationUrl + '?id=' + id + pagenumparam + sortingorder + sortingdir + pagesize + EmployeeKeyword + StartDate + EndDate;
    $.get(url, function (data) {

        if (data.alreadyBooked == true) {
            $(window).scrollTop(0);
            $('#vacationDv').html('Leave not approved as employee already has job assigned on given dates !');
            $('#vacationDv').css('color', 'red');
            $('#vacationDv').show();
            window.setTimeout(function () {
                $('#vacationDv').hide();
            }, 4000)
        }
        else {
            $('#dvShowEmpVacation').empty();
            $('#dvShowEmpVacation').append(data);

            $(window).scrollTop(0);
            $('.jobalert').html('<strong>Holiday has been approved !</strong>');
            $('.jobalert').css('color', 'green');
            $('.jobalert').show();
            window.setTimeout(function () {
                $('.jobalert').hide();
            }, 4000)
        }

        

    });
}

function RejectVacation(event, id) {
    
    event.preventDefault();
    var pagenum = $('.active span').text();
    var pagenumparam = '&grid-page=' + pagenum;
    var sortingorder = '&' + FSM.SortColumn;
    var sortingdir = '&' + FSM.SortDir;
    var pagesize = "&PageSize=" + $('#ddPageSize').val();

    var EmployeeKeyword = "&EmployeeKeyword=" + $('#EmployeeKeyword').val();
    var StartDate = "&StartDate=" + $('#StartDate').val();
    var EndDate = "&EndDate=" + $('#EndDate').val();

    var url = FSM.RejectVacationUrl + '?id=' + id + pagenumparam + sortingorder + sortingdir + pagesize + EmployeeKeyword + StartDate + EndDate;

    $.get(url, function (data) {
        $('#dvShowEmpVacation').empty();
        $('#dvShowEmpVacation').append(data);

        $(window).scrollTop(0);
        $('.jobalert').html('<strong>Holiday has been rejected !</strong>');
        $('.jobalert').css('color', 'green');
        $('.jobalert').show();
        window.setTimeout(function () {
            $('.jobalert').hide();
        }, 4000)
    });
}

$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.EmpVacationUrl + "?PageSize=" + page_size;
});

$(document).on("keypress", ".srch_vacations", function (e) {

    var key = e.which;
    if (key == 13)  // the enter key code
    {
        $('#searchVacation').click();
        return false;
    }
});

$(document).off("click",".updateEmpVacationAdmin").on("click", ".updateEmpVacationAdmin", function (event) {
    event.preventDefault();

    var id = $(this).attr("data-id");

    data = { id: id };

    $.get(FSM.EditVacationUrl, data, function (result) {
        $('#updateVacationAdminDv').html(result);
        $('#modalEmpEditVacationAdmin').modal('show');
    });
});

$(document).on('dblclick', '.cssEditEmpVacation', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();
    data = { id: id };

    $.get(FSM.EditVacationUrl, data, function (result) {
        $('#updateVacationAdminDv').html(result);
        $('#modalEmpEditVacationAdmin').modal('show');
    });
});

$(document).off("click", ".updateAdminvacation").on("click", ".updateAdminvacation", function (event) {
    event.preventDefault();
    var form = $('#editadminvacationForm');
    var data = form.serialize();
    var action = form.attr("action") + "?PageSize=" + FSM.PageSize;

    $.post(action, data, function (result) {
        if (result.errorList != null) {
            $("#EditVacationErrorDv").empty(); // empty div
            var ErrorList = "<ul style='list-style:none;'>"
            $(result.errorList).each(function (i) {
                ErrorList = ErrorList + "<li>" + result.errorList[i].ErrorMessage + "</li>";
            });
            ErrorList = ErrorList + "</ul>"
            $(window).scrollTop(0);
            $("#EditVacationErrorDv").append(ErrorList);
            $('#EditVacationErrorDv').css('color', 'red');
            $("#EditVacationErrorDv").css("display", "block");
        }

        else {
            $('#manageVacationsDV').empty();
            $('#manageVacationsDV').append(result);
            $('#modalEmpEditVacation').modal('hide');
            $(window).scrollTop(0);
            $('#modalEmpEditVacationAdmin').modal('hide');
            $('.jobalert').css('color', 'green');
            $('.jobalert').html('<strong>Record updated successfully !</strong>');
            $('.jobalert').show();
            window.setTimeout(function () {
                $('.jobalert').hide();
                location.reload();
            }, 3000)
        }
    });
});

$(".btnEmployeeholiday").click(function () {
    $.ajax({
        url: common.SitePath + "/Employee/Employee/_AddEmployeeHolidays",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#updateVacationAdminDv").empty();
            $("#updateVacationAdminDv").html(data);
            $("#modalEmployeeHoliday").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

//delete holiday
function DeleteEmployeeHoliday(id) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete?");
    $(".btnconfirm").attr("id", id);
    $(".modal-title").html("Delete Leave!");
}

$(document).on('click', ".btnconfirm", function () {
    var Id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Employee/DeleteEmployeeHoliday",
        data: { id: Id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () { window.location.href = common.SitePath + "/Employee/Employee/GetVacations"; });
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})