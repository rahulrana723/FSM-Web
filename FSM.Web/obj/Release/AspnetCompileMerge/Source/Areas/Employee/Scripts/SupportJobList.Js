$(document).ready(function () {
    $('#ddPageSize').val(FSM.SelectedVal);
});

$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?page_size=" + page_size;
});


function checkSupportJobs() {
    
    var jobid = $("input[name='select']:checked").attr("jobid");
    var Supportid = $(".supportid").val();
   
    $.ajax({
        type: 'POST',
        url: common.SitePath + "/Employee/Job/SaveSupportJob",
        dataType: "json",
        data: { jobid: jobid, supportJobId: Supportid },
        success: function (data) {
            alert("Job Succesfully Link");
        },
        error: function () {
        }
    })
}

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

     var searchstring = $('#searchkeyword').val();

    if (searchstring != "" && searchstring != undefined) {
        elementparam = elementparam + '&searchkeyword=' + searchstring;
    }
    var jobtype = $('#JobType').val();

    var Jobtype = $('#JobType').val();
    if (Jobtype != "" && Jobtype != undefined) {
        elementparam = elementparam + '&Jobtype=' + Jobtype;
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
    var searchstring = $('#searchkeyword').val();

    if (searchstring != "" && searchstring != undefined) {
        elementparam = elementparam + '&searchkeyword=' + searchstring;
    }

    var jobtype = $('#JobType').val();

    var Jobtype = $('#JobType').val();
    if (Jobtype != "" && Jobtype != undefined) {
        elementparam = elementparam + '&JobType=' + Jobtype;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);


});
