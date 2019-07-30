$(document).ready(function myfunction() {
    if (FSM.Message != "" && FSM.Message != 0) {
        $(".jobalert").css("display", "block");
        if (FSM.Message == "1") {
            $(".jobalert").html("<strong>Record added Successfully!</strong>");
        }
        else if (FSM.Message == "2") {
            $(".jobalert").html("<strong>Record Updated Successfully!</strong>");
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
});
$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?page_size=" + page_size;
});

function DeleteEmployee(Empioyeeid) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete employee?");
    $(".btnconfirm").attr("id", Empioyeeid);
    $(".modal-title").html("Delete Employee Record!");
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Employee/DeleteEmployee",
        data: { EmployeeId: id },
        async: false,
        success: function (result) {
            if (result != false) {
                $("#GeneralInfoMsgDiv").empty();
                $(".commonpopup").modal('hide');
                $(".jobalert").css("display", "block");
                $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
                $(".jobalert").delay(2000).fadeOut(function () {
                    window.location.href = common.SitePath + "/Employee/Employee/EmployeeDetails";
                });
            }
            else {
                $(window).scrollTop(0);
                $("#GeneralInfoMsgDiv").empty();
                $(".commonpopup").modal('hide');
                $('#GeneralInfoMsgDiv').css('color', 'red');
                $("#GeneralInfoMsgDiv").css("display", "block");
                $("#GeneralInfoMsgDiv").html("<strong>something went wrong!</strong>");
            }
        },
        error: function () {
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Something went wrong!</strong>");
            $(".jobalert").delay(5000).fadeOut();
            $(".commonpopup").modal('hide');
        }
    });
})

$(document).on('dblclick', '.cssEditEmployee', function () {
    // getting customer id
    var EmployeeId = $(this).find('td:eq(0)').text();
    window.location = FSM.EditEmployee + "?EmployeeId=" + EmployeeId;
});