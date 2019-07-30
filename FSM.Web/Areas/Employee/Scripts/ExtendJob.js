$(document).on("click", "#UpdateExtendJob", function () {
    var updateDate = $("#DateBookedCalender").val();
    //updateDate = new Date(updateDate).toLocaleDateString();

    var EstimateHour = $("#estimatedHours").val();

    var jobId = $('#jobIdCal').val();
    //var OTRWRequired = $("#OtrwReuired").val();
    var userid = [];
    $.each($("#extendAssignTo option:selected"), function () {
        var id = $(this).val();
        userid.push(id);
    });

    if (updateDate == "" || updateDate == "Invalid Date") {
        $("#ErrorDv").empty();
        $(window).scrollTop(0);
        $("#ErrorDv").css("display", "block");
        $('#ErrorDv').css('color', 'red');
        $("#ErrorDv").html("<strong>Date field is required !</strong>");
        $("#ErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }

    if (EstimateHour == "" || EstimateHour == null) {
        $("#ErrorDv").empty();
        $(window).scrollTop(0);
        $("#ErrorDv").css("display", "block");
        $('#ErrorDv').css('color', 'red');
        $("#ErrorDv").html("<strong>Estimated hours is required !</strong>");
        $("#ErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }

    //if (userid.length == 0) {
    //    $("#ErrorDv").empty();
    //    $(window).scrollTop(0);
    //    $("#ErrorDv").css("display", "block");
    //    $('#ErrorDv').css('color', 'red');
    //    $("#ErrorDv").html("<strong>Please select atleast 1 OTRW !</strong>");
    //    $("#ErrorDv").delay(4000).fadeOut(function () {
    //    });
    //    return false;
    //}

    $("#UpdateExtendJob").val("Updating...");
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/UpdateExtendJobPopUp" + '?' + '&AssignTo2=' + userid,
        data: { JobId: jobId, UpdateDateBooked: updateDate, EstimatedHours: EstimateHour },
        type: 'Get',
        async: false,
        success: function (data) {

            $(window).scrollTop(0);
            if (data.Status == "Success") {
                $("#modalExtendJobCalender").hide();
                $('.assign-job-msg').css('color', 'green');
                $('.assign-job-msg').html("Updated successfully");
                $('.assign-job-msg').show();
                window.setTimeout(function () {
                    $('.assign-job-msg').hide();
                    location.reload();
                }, 4000)
            }
            else {
                $("#UpdateExtendJob").val("Update");
                $("#ErrorDv").css("display", "block");
                $('#ErrorDv').css('color', 'red');
                $("#ErrorDv").html("<strong>" + data.Required + "</strong>");
                $("#ErrorDv").delay(4000).fadeOut(function () { });
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});