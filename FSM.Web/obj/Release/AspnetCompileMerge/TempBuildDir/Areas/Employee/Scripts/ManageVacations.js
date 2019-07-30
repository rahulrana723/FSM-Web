
$(document).ready(function () {

    
    $('#SearchStartDate').datepicker({
        dateFormat: "dd/mm/yy"
    });
    $('#SearchEndDate').datepicker({
        dateFormat: "dd/mm/yy"
    });

    $('#ddPageSize').val(FSM.PageSize);

});

$(document).on("click", ".addEmpVacation", function (event) {
    
    event.preventDefault();
   
    $.get(FSM.AddVacationUrl, function (result) {
        $('#addVacationDv').html(result);
        $('#modalEmpVacation').modal('show');
    });

});

$(document).on("click", ".savevacation", function (event) {
   
    event.preventDefault();

    var form = $('#vacationForm');
    var data = form.serialize();
    var action = form.attr("action") + "?PageSize=" + FSM.PageSize;

    $.post(action, data, function (result) {
        if (result.errorList != null) {
            $("#AddVacationErrorDv").empty(); // empty div

            var ErrorList = "<ul style='list-style:none;'>"
            $(result.errorList).each(function (i) {
                ErrorList = ErrorList + "<li>" + result.errorList[i].ErrorMessage + "</li>";
            });
            ErrorList = ErrorList + "</ul>"
            $(window).scrollTop(0);
            $("#AddVacationErrorDv").append(ErrorList);
            $('#AddVacationErrorDv').css('color', 'red');
            $("#AddVacationErrorDv").css("display", "block");
        }

        else {
            $("#AddVacationErrorDv").empty();
            $('#manageVacationsDV').empty();
            $('#manageVacationsDV').append(result);
            $('#modalEmpVacation').modal('hide');

            $(window).scrollTop(0);
            $('#vacationMsgDiv').css('color', 'green');
            $('#vacationMsgDiv').html('Vacation added successfully !');
            $('#vacationMsgDiv').show();
            window.setTimeout(function () {
                $('#vacationMsgDiv').hide();
            }, 4000)

        }
    });

});

$(document).on("click", ".updateEmpVacation", function (event) {
    event.preventDefault();

    var id = $(this).attr("data-id");

    data = { id: id };

    $.get(FSM.EditVacationUrl, data, function (result) {
        $('#updateVacationDv').html(result);

        //var startVal = $(".editstartdate").val();
        //var aaaa = $.datepicker.formatDate('mm/dd/yy', new Date(startVal));
        //if (startVal != "" && startVal != undefined) {
        //    $(".editstartdate").val($.datepicker.formatDate('mm/dd/yy', new Date(startVal)));
        //}
        //var endVal = $(".editenddate").val();
        //if (endVal != "" && endVal != undefined) {
        //    $(".editenddate").val($.datepicker.formatDate('mm/dd/yy', new Date(endVal)));
        //}

        $('#modalEmpEditVacation').modal('show');
    });



});

$(document).on("click", ".updatevacation", function (event) {
    event.preventDefault();
    var form = $('#editvacationForm');
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
            $("#EditVacationErrorDv").empty();
            $('#manageVacationsDV').empty();
            $('#manageVacationsDV').append(result);
            $('#modalEmpEditVacation').modal('hide');

            $(window).scrollTop(0);
            $('#vacationMsgDiv').css('color', 'green');
            $('#vacationMsgDiv').html('Record updated successfully !');
            $('#vacationMsgDiv').show();
            window.setTimeout(function () {
                $('#vacationMsgDiv').hide();
            }, 4000)
        }
    });

});

function deleteEmpVacation(data_id) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete holiday ?");
    $(".btnconfirm").attr("id", data_id);
    $(".modal-title").html("Delete Holiday!");
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    data = { id: id, PageSize: FSM.PageSize };

    $.get(FSM.DeleteVacationUrl, data, function (result) {
        $(".commonpopup").modal('hide');
        $('#manageVacationsDV').empty();
        $('#manageVacationsDV').append(result);

        $(window).scrollTop(0);
        $('#vacationMsgDiv').css('color', 'green');
        $('#vacationMsgDiv').html('Holiday deleted successfully !');
        $('#vacationMsgDiv').show();
        window.setTimeout(function () {
            $('#vacationMsgDiv').hide();
        }, 4000)
    });
})

$(document).on("click", ".searchvacation", function (event) {
    event.preventDefault();
    var startdate = $('#SearchStartDate').val();
    var enddate = $('#SearchEndDate').val();

    var regex = new RegExp("^((0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)?[0-9]{2})*$");
    if (!regex.test(startdate)) {
        startdate = '';
    }
    if (!regex.test(enddate)) {
        enddate = '';
    }

    var querystring = '?StartDate=' + startdate + '&EndDate=' + enddate + "&PageSize=" + FSM.PageSize;

    $.get(FSM.DefaultUrl + querystring, function (result) {
        $('#manageVacationsDV').empty();
        $('#manageVacationsDV').append(result);
    });
});

$(document).on("change", "#ddPageSize", function () {
    var pageSize = $('#ddPageSize').val();
    var data = { PageSize: pageSize };

    FSM.PageSize = pageSize;

    $.get(FSM.DefaultUrl, data, function (result) {
        $('#manageVacationsDV').empty();
        $('#manageVacationsDV').append(result);
    });
});

$('.grid-header-title a').on('click', function () {
    var elementparam = $(this).attr('href');

    var pagenum = $('.active span').text();
    if (pagenum != "" && pagenum != undefined) {
        elementparam = elementparam + '&grid-page=' + pagenum;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    var aaa = elementparam.indexOf("PageSize");
    if (pagesize != "" && pagesize != undefined && pagesize > 0 && !(elementparam.indexOf("PageSize") > 0)) {
        elementparam = elementparam + '&PageSize=' + pagesize;
    }

    var startdate = $('#SearchStartDate').val();
    var enddate = $('#SearchEndDate').val();

    var regex = new RegExp("^((0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)?[0-9]{2})*$");
    if (!regex.test(startdate)) {
        startdate = '';
    }
    if (!regex.test(enddate)) {
        enddate = '';
    }

    if (!(elementparam.indexOf("&StartDate") > 0) && !(elementparam.indexOf("&EndDate") > 0)) {
        elementparam = elementparam + '&StartDate=' + startdate + '&EndDate=' + enddate;
    }

    $(this).attr('href', elementparam);

});

$('.grid-footer a').on('click', function myfunction() {
    var elementparam = $(this).attr('href');

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0 && !(elementparam.indexOf("PageSize") > 0)) {
        elementparam = elementparam + '&PageSize=' + pagesize;
    }

    var regex = new RegExp("^((0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)?[0-9]{2})*$");
    if (!regex.test(startdate)) {
        startdate = '';
    }
    if (!regex.test(enddate)) {
        enddate = '';
    }

    if (!(elementparam.indexOf("&StartDate") > 0) && !(elementparam.indexOf("&EndDate") > 0)) {
        elementparam = elementparam + '&StartDate=' + startdate + '&EndDate=' + enddate;
    }

    $(this).attr('href', elementparam);

});
