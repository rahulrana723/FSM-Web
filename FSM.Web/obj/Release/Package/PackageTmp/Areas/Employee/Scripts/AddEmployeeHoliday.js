$(document).ready(function () {
    $('#HolidayStartDate').datepicker({ minDate: 0, dateFormat: 'dd/mm/yy' });
    $('#HolidayEndDate').datepicker({ minDate: 0, dateFormat: 'dd/mm/yy' });
})


//Check inputfield validations
function checkformvalidation() {
   
    var empid = $("#tempAssignTo").val();
    var startdate = $('#HolidayStartDate').val();
    var Enddate = $('#HolidayEndDate').val();
    var Hours = $("#Hours").val();
    var Reason = $("#Reason").val();
    var status = true;
    isexisted = false;
    $("#errorEmployee,#errorstartdate,#errorEndDate,errorHour,#errorreason").html("");
    if (empid == '' || empid == 'undefined') {
        status = false;
        $("#errorEmployee").html("*Please select employee");
        return false;
    }
    else if (startdate == '' || startdate == 'undefined') {
        status = false;
        $("#errorstartdate").html("*Please provide start date");
        return false;
    }
    else if (Enddate == '' || Enddate == 'undefined') {
        status = false;
        $("#errorEndDate").html("*Please provide end date");
        return false;
    }
    else if (Hours == '' || Hours == 'undefined') {
        status = false;
        $("#errorHour").html("*Please provide hour");
        return false;
    }
    else if (Reason == '' || Reason == 'undefined') {
        status = false;
        $("#errorreason").html("*Please provide holiday description");
        return false;
    }
   
    if (status) {
        $(".formEmpholiday").submit();
        return true;
    }
    else {
        return false;
    }
}