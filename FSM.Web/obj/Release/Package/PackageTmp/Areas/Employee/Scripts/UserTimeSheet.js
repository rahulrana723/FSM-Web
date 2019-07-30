$(document).ready(function () {
    // $(".ddlmultiselect").multiselect();

    $('.ddlmultiselect')
   .multiselect({
       //  allSelectedText: 'All',
       maxHeight: 200,
       includeSelectAllOption: true
   })
   //.multiselect('selectAll', ture)
   .multiselect('updateButtonText');


    $('#JobStartDate').datepicker({
        dateFormat: 'dd/mm/yy'
    });
    $('#JobEndDate').datepicker({
        dateFormat: 'dd/mm/yy'
    });
    $('#startDateSheet').datepicker({
        dateFormat: 'dd/mm/yy'
    });
    $('#endDateSheet').datepicker({
        dateFormat: 'dd/mm/yy'
    });

    $('#ddPageSize').val(10);
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

$(document).on('click', '#searchTimeSheet', function () {
    var pattern = /^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$/;
    var isStartValid = true;
    var isEndValid = true;
    var isKeywordValid = true;

    var keyword = $('#Keyword').val();
    var jobstartdate = $('#JobStartDate').val();
    var jobenddate = $('#JobEndDate').val();

    var userid = [];
    $.each($("#UserId option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            userid.push("''" + id + "''");
        }
        else {
            userid.push("" + id + "");
        }
    });

    var pagesize = $('#ddPageSize').val();

    if (keyword != '' && keyword != undefined) {
        if (keyword.length > 50) {
            $('#sheetKeywordDV').show();
            isKeywordValid = false;
        }
        else {
            $('#sheetKeywordDV').hide();
            isKeywordValid = true;
        }
    }
    else {
        $('#sheetKeywordDV').hide();
        isKeywordValid = true;
    }

    if (jobstartdate != '' && jobstartdate != undefined) {
        if (!pattern.test(jobstartdate)) {
            $('#sheetStartDateDV').show();
            isStartValid = false;
        }
        else {
            $('#sheetStartDateDV').hide();
            isStartValid = true;
        }
    }
    else {
        $('#sheetStartDateDV').hide();
        isStartValid = true;
    }

    if (jobenddate != '' && jobenddate != undefined) {
        if (!pattern.test(jobenddate)) {
            $('#sheetEndDateDV').show();
            isEndValid = false;
        }
        else {
            $('#sheetEndDateDV').hide();
            isEndValid = true;
        }
    }
    else {
        $('#sheetEndDateDV').hide();
        isEndValid = true;
    }

    if (isStartValid == false || isEndValid == false || isKeywordValid == false) {
        return false;
    }
    else {
        var url = FSM.TimeSheetUrl + '?Keyword=' + keyword + '&JobStartDate=' + jobstartdate + '&JobEndDate=' + jobenddate + '&UserId=' + userid + '&PageSize=' + pagesize;

        $.get(url, function (data) {
            $('#timesheetDv').empty();
            $(".clsTimesheetDv").remove();
            $('#timesheetDv').append(data);
        });
    }
});

$(document).on('click', '.grid-pager .pagination a', function (event) {

    event.preventDefault();
    var querystring = $(this).prop('href').split("?")[1];
    var values = querystring.split("&");
    var pagenumparam = '';
    var sortcolumn = '';
    var sortdir = '';
    var pagesize = $('#ddPageSize').val();

    var keyword = $('#Keyword').val();
    var jobstartdate = $('#JobStartDate').val();
    var jobenddate = $('#JobEndDate').val();
    var userid = [];
    $.each($("#UserId option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            userid.push("''" + id + "''");
        }
        else {
            userid.push("" + id + "");
        }
    });

    $.each(values, function (i, item) {
        if (item.indexOf("grid-page") > -1) {
            pagenumparam = item;
        }
        if (item.indexOf("grid-column") > -1) {
            sortcolumn = item;
        }
        if (item.indexOf("grid-dir") > -1) {
            sortdir = item;
        }
    })

    var url = FSM.TimeSheetUrl + '?' + pagenumparam + '&' + sortcolumn + '&' + sortdir + '&Keyword=' + keyword + '&JobStartDate=' + jobstartdate + '&JobEndDate=' + jobenddate + '&UserId=' + userid + '&PageSize=' + pagesize;

    $.get(url, function (data) {
        $('#timesheetDv').empty();
        $(".clsTimesheetDv").remove();
        $('#timesheetDv').append(data);
    });

});

$(document).on('click', '.grid-header-title a', function (event) {

    event.preventDefault();

    var pagenum = $('#timesheetDv li.active span').text();
    var pagesize = $('#ddPageSize').val();

    var jobstartdate = $('#JobStartDate').val();
    var jobenddate = $('#JobEndDate').val();
    var userid = [];
    $.each($("#UserId option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            userid.push("''" + id + "''");
        }
        else {
            userid.push("" + id + "");
        }
    });

    var keyword = $('#Keyword').val();

    var elementparam = $(this).attr('href');

    if (pagenum != undefined && pagenum != "") {
        elementparam = elementparam + "&grid-page=" + pagenum;
    }
    if (jobstartdate != undefined && jobstartdate != "" && elementparam.indexOf("JobStartDate") < 0) {
        elementparam = elementparam + '&JobStartDate=' + jobstartdate;
    }
    if (jobenddate != undefined && jobenddate != "" && elementparam.indexOf("JobEndDate") < 0) {
        elementparam = elementparam + '&JobEndDate=' + jobenddate;
    }
    if (userid != undefined && userid != "" && elementparam.indexOf("UserId") < 0) {
        elementparam = elementparam + '&UserId=' + userid;
    }
    if (pagesize != undefined && pagesize != "" && elementparam.indexOf("PageSize") < 0) {
        elementparam = elementparam + '&PageSize=' + pagesize;
    }
    if (keyword != undefined && keyword != "" && elementparam.indexOf("Keyword") < 0) {
        elementparam = elementparam + '&Keyword=' + keyword;
    }

    var url = FSM.TimeSheetUrl + elementparam;

    $.get(url, function (data) {
        $('#timesheetDv').empty();
        $(".clsTimesheetDv").remove();
        $('#timesheetDv').append(data);
    });

});

function EditUserSheet(event, id, OtrwuserId) {
    event.preventDefault();
    var pagenum = $('#timesheetDv li.active span').text();
    var jobstartdate = $('#JobStartDate').val();
    var jobenddate = $('#JobEndDate').val();
    var userid = [];
    $.each($("#UserId option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            userid.push("''" + id + "''");
        }
        else {
            userid.push("" + id + "");
        }
    });

    var keyword = $('#Keyword').val();
    var pagesize = $('#ddPageSize').val();

    var url = FSM.EditTimeSheetUrl + "?Keyword=" + keyword + "&";

    if (pagenum != undefined && pagenum != "") {
        url = url + "grid-page=" + pagenum + "&";
    }
    if (jobstartdate != undefined && jobstartdate != "" && url.indexOf("JobStartDate") < 0) {
        url = url + 'JobStartDate=' + jobstartdate + "&";
    }
    if (jobenddate != undefined && jobenddate != "" && url.indexOf("JobEndDate") < 0) {
        url = url + 'JobEndDate=' + jobenddate + "&";
    }
    if (userid != undefined && userid != "" && url.indexOf("UserId") < 0) {
        url = url + 'UserId=' + userid + "&";
    }
    if (pagesize != undefined && pagesize != "" && url.indexOf("PageSize") < 0) {
        url = url + 'PageSize=' + pagesize + "&";
    }

    $.ajax({
        url: url,
        data: { id: id, EmpUserId: OtrwuserId },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divTimeSheetPopup").html(data);
            $("#modalTimeSheet").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

$(document).on('dblclick', '.cssEditTimeSheet', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();
    var userid = $(this).find('td:eq(1)').text();
    var url = FSM.EditTimeSheetUrl;

    $.ajax({
        url: url,
        data: { id: id, EmpUserId: userid },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divTimeSheetPopup").html(data);
            $("#modalTimeSheet").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });

   });

function SaveTimeSheet() {
    var url = FSM.EditTimeSheetUrl;
    var $form = $("#formEditTimeSheet");

    $.ajax({
        url: url,
        data: $form.serialize(),
        type: 'Post',
        async: false,
        success: function (data) {
            if (data.constructor == Array) {

                $("#SheetErrorDv").empty(); // empty div

                var ErrorList = "<ul style='list-style:none;'>"
                $(data).each(function (i) {
                    ErrorList = ErrorList + "<li>" + data[i].ErrorMessage + "</li>";
                });
                ErrorList = ErrorList + "</ul>"
                $(window).scrollTop(0);
                $("#SheetErrorDv").append(ErrorList);
                $('#SheetErrorDv').css('color', 'red');
                $("#SheetErrorDv").css("display", "block");
            }
            else {
                $("#SheetErrorDv").empty();
                $("#modalTimeSheet").modal("hide");
                $('#timesheetDv').empty();
                $(".clsTimesheetDv").remove();
                $('#timesheetDv').append(data);
                $('#sheetSuccessMsg').show();
                $(window).scrollTop(0);
                window.setTimeout(function () {
                    $('#sheetSuccessMsg').hide();
                }, 2000)
            }


        },
        error: function () {
            alert("something seems wrong");
        }
    });

}

$(document).on("click", ".closebtn", function () {
    $("#modalTimeSheet").modal("hide");
});


$("#generatereport").on("click", function (event) {
    event.preventDefault();
    var href = common.SitePath + "/Employee/job/Export?";
    var pattern = /^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$/;
    var isStartValid = true;
    var isEndValid = true;
    var isKeywordValid = true;
    var isReportType = true;

    var reportType = $("#reportType").val();
    var jobstartdate = $('#startDateSheet').val();
    var jobenddate = $('#endDateSheet').val();
    var userid = [];
    $.each($("#multiUserId option:selected"), function () {
        var id = $(this).val();
        if (id != "") {
            userid.push("''" + id + "''");
        }
        else {
            userid.push("" + id + "");
        }
    });


    if (reportType != '' && reportType != undefined && reportType != 0) {
        $('#sheetReportTypeDVpopUp').hide();
        isReportType = true;
    }
    else {
        $('#sheetReportTypeDVpopUp').show();
        isReportType = false;
    }

    var keyword = $('#keysrch').val();

    if (keyword != '' && keyword != undefined) {
        if (keyword.length > 50) {
            $('#sheetKeywordDVpopUp').show();
            isKeywordValid = false;
        }
        else {
            $('#sheetKeywordDVpopUp').hide();
            isKeywordValid = true;
        }
    }
    else {
        $('#sheetKeywordDVpopUp').hide();
        isKeywordValid = true;
    }

    if (jobstartdate != '' && jobstartdate != undefined) {
        if (!pattern.test(jobstartdate)) {
            $('#sheetStartDateDVpopUp').show();
            isStartValid = false;
        }
        else {
            $('#sheetStartDateDVpopUp').hide();
            isStartValid = true;
        }
    }
    else {
        $('#sheetStartDateDVpopUp').hide();
        isStartValid = true;
    }

    if (jobenddate != '' && jobenddate != undefined) {
        if (!pattern.test(jobenddate)) {
            $('#sheetEndDateDVpopUp').show();
            isEndValid = false;
        }
        else {
            $('#sheetEndDateDVpopUp').hide();
            isEndValid = true;
        }
    }
    else {
        $('#sheetEndDateDVpopUp').hide();
        isEndValid = true;
    }

    if (isStartValid == false || isEndValid == false || isKeywordValid == false || isReportType == false) {
        return false;
    }

    if (jobstartdate != undefined && jobstartdate != "" && href.indexOf("JobStartDate") < 0) {
        href = href + 'JobStartDate=' + jobstartdate + "&";
    }
    if (reportType != undefined && reportType != "" && href.indexOf("ReportType") < 0) {
        href = href + 'ReportType=' + reportType + "&";
    }
    if (jobenddate != undefined && jobenddate != "" && href.indexOf("JobEndDate") < 0) {
        href = href + 'JobEndDate=' + jobenddate + "&";
    }
    if (keyword != undefined && keyword != "" && href.indexOf("Keyword") < 0) {
        href = href + 'Keyword=' + keyword + "&";
    }

    if (userid != undefined && userid != "" && href.indexOf("UserId") < 0) {
        href = href + 'UserId=' + userid + "&";
    }

    $("#modalReportTypePopUp").modal("hide");

    $(this).attr("href", href);
    window.location.href = href;

});


$(document).on("keypress", ".srchTimesheet", function (e) {
    var key = e.which;
    if (key == 13)  // the enter key code
    {
        $('#searchTimeSheet').click();
        return false;
    }
});

$("#ReportPopUp").click(function () {
    $.ajax({
        url: common.SitePath + "/Employee/Job/_ReportTypeGenerate",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divTimeSheetPopup").empty();
            $("#divTimeSheetPopup").html(data);
            $("#modalReportTypePopUp").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

