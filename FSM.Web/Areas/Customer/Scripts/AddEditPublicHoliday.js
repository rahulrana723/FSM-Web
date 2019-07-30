//Check for Existing entries
var addeditholiday = {
    isexisted: false,
    checkExistanceHoliday: function (startdate, id) {
        var date = startdate;
        var existdate = new Date(date.split("/").reverse().join("-"));
        $.ajax({
            url: common.SitePath + "/Employee/Employee/CheckExistingHolidays",
            data: { Date: existdate.toDateString(), Id: id },
            type: 'Get',
            async: false,
            success: function (data) {
                isexisted = data;
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    }
}
$(document).ready(function () {
    $('.startingdate').datepicker({ minDate: 0, dateFormat: 'dd/mm/yy' });
})
//Check inputfield validations
function checkformvalidation() {
    var id = $("#Id").val();
    var startdate = $('.startingdate').val();
    var Enddate = $('.Endingdate').val();
    var Reason = $("#Reason").val();
    var status = true;
    isexisted = false;
    $("#errorstartdate,#errorEndDate,#errorreason").html("");
    if (startdate == '' || startdate == 'undefined') {
        status = false;
        $("#errorstartdate").html("*Please provide date");
        return false;
    }
    else if (Reason == '' || Reason == 'undefined' ) {
        status = false;
        $("#errorreason").html("*Please provide holiday description");
        return false;
    }
    addeditholiday.checkExistanceHoliday(startdate, id);
    if (isexisted) {
        status = false;
        $("#errorstartdate").html("*Record already existed for this time interval");
        return false;
    }
    if (Reason.length > 500)
    {
        status = false;
        $("#errorreason").html("*Max length (500 characters)");
        return false;
    }
    if (status) {
        $(".formholiday").submit();
        return true;
    }
    else {
        return false;
    }
}