$(document).ready(function () {
    if (FSM.TitleVal == 'Mr') {
        $('#Title').val(1);
    }
    else if (FSM.TitleVal == 'Mrs') {
        $('#Title').val(2);
    }
    if (FSM.ContactCount == 1) {
        $("#divContactsPartial").hide();
        $(".new_emp_tab").hide();
        $("#divContactsForm").show();
    }
    else {
        $("#divContactsPartial").show();
        $(".new_emp_tab").show();
        $("#divContactsForm").hide();
    }

    if (FSM.Success == "ok") {
        $(".jobalert").empty();
        $(".jobalert").append("<strong>Record Saved Successfully!</strong>");
        $('.jobalert').css('color', 'green');
        $(".jobalert").css("display", "block");
        $(".jobalert").hide(4000);

        FSM.Success = null;
    }
    else if (FSM.Success == "delete") {
        $(".jobalert").empty();
        $(".jobalert").append("<strong>Record deleted Successfully!</strong>");
        $('.jobalert').css('color', 'green');
        $(".jobalert").css("display", "block");
        $(".jobalert").hide(4000);

        FSM.Success = null;
    }

    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $("#contactsSerch").trigger("click");
            return false;
        }
    });

});

$(document).on('click', '.btnaddJobcontacts', function () {
    var customerGeneralinfoid = $(this).attr("customerGeneralinfoid");
    var siteID = $(this).attr("SiteId");
    var JobID = $(this).attr("JobId");
    var HideAddContacts = $(this).attr("data-HideAddContacts");
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/_UpdateJobContactsAddEdit",
        data: { CustomerGeneralinfoid: customerGeneralinfoid, SiteId: siteID, JobId: JobID, HideAddContacts: HideAddContacts },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divContactsForm").show();
            $("#divContactsForm").empty();
            $("#divContactsForm").html(data);
            $("#divContactsPartial").hide();
            $(".new_emp_tab").hide();
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).on('click', '#cancelcontact', function () {
    $("#divContactsPartial").show();
    $(".new_emp_tab").show();
    $("#divContactsForm").hide();
});
