
$(document).ready(function () {

    $('#NextContactDate').datepicker({
        minDate: 0,
        dateFormat: 'dd/mm/yy'
    });

    if (FSM.Success == "ok") {
        $(".jobalert").empty();
        $(".jobalert").append("<strong>Record Saved Successfully!</strong>");
        $('.jobalert').css('color', 'green');
        $(".jobalert").css("display", "block");
        window.setTimeout(function () {
            $('.jobalert').hide();
        }, 4000)
        FSM.Success = null;
    }

    var umbrellachecked = $('#UmbrellaGroup').is(':checked');
    if (umbrellachecked) {
        $('.note').css('display', 'block');
    }
    else {
        $('.note').css('display', 'none');
    }
    //var status = $('#BlackListed').is(':checked');
    //if (status) {
    //    $('div.blacklist').css('display', 'block');
    //}
    //else {
    //    $('div.blacklist').css('display', 'none');
    //}
});

$(document).on('change', '#UmbrellaGroup', function () {
    var umbrellachecked = $('#UmbrellaGroup').is(':checked');
    if (umbrellachecked) {
        $('.note').css('display', 'block');
    }
    else {
        $('.note').css('display', 'none');
        $("#txtNote").val("");
    }
});

$("#btnSaveGeneralInfo").on("click", function (e) {
    var $form = $("#formGeneralInfo"); // form id
    $(window).scrollTop(0);
    $.post($form.attr("action"), $form.serialize(), function (result) {

        var id = result.id;
        var activetab = result.activetab;

        if (result.length > 0) {

            $("#GeneralInfoMsgDiv").empty(); // empty div

            var ErrorList = "<ul style='list-style:none;'>"
            $(result).each(function (i) {
                ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
            });
            ErrorList = ErrorList + "</ul>"
            $(window).scrollTop(0);
            $("#GeneralInfoMsgDiv").append(ErrorList);
            $('#GeneralInfoMsgDiv').css('color', 'red');
            $("#GeneralInfoMsgDiv").css("display", "block");
        }
        else {
            $("#GeneralInfoMsgDiv").empty();
            window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
        }
    });
});

$(document).on('click', "#btnEditGeneralInfo", function (e) {

    var status = $("#BlackListed").is(":checked");
    if (status) {
        $("div.blacklist").css("display", "block");
    }
    else {
        $("div.blacklist").css("display", "none");
    }

    var $form = $("#formGeneralInfo"); // form id
    $(window).scrollTop(0);
    $.post($form.attr("action"), $form.serialize(), function (result) {

        var id = result.id;
        var activetab = result.activetab;

        if (result.length > 0) {
            $("#GeneralInfoMsgDiv").empty(); // empty div

            var ErrorList = "<ul style='list-style:none;'>"
            $(result).each(function (i) {
                ErrorList = ErrorList + "<li>" + result[i].ErrorMessage + "</li>";
            });
            ErrorList = ErrorList + "</ul>"
            $(window).scrollTop(0);
            $("#GeneralInfoMsgDiv").append(ErrorList);
            $('#GeneralInfoMsgDiv').css('color', 'red');
            $("#GeneralInfoMsgDiv").css("display", "block");
        }
        else {
            $("#GeneralInfoMsgDiv").empty();
            window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=' + activetab + '&success=ok';
        }

    });
});


$(document).on('click', "#btnEditGeneralInfoForJob", function (e) {
    var status = $("#BlackListed").is(":checked");
    if (status) {
        $("div.blacklist").css("display", "block");
    }
    else {
        $("div.blacklist").css("display", "none");
    }

    $(".jobalert").empty(); // empty div
    var $form = $("#formGeneralInfo"); // form id
    var formdata = new FormData($('#formGeneralInfo').get(0));

    $.ajax({
        url: $('#formGeneralInfo').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            $('#GeneralInfoMsgDiv').empty();
            $(window).scrollTop(0);

            if (result.status == "saved") {
                $('.jobalert').css('color', 'green');
                $('.jobalert').html(result.msg);
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                }, 4000)
            }
            else {
                var ErrorList = "<ul style='list-style:none;'>"
                $(result.errors).each(function (i) {
                    ErrorList = ErrorList + "<li>" + result.errors[i].ErrorMessage + "</li>";
                });
                ErrorList = ErrorList + "</ul>"
                $(window).scrollTop(0);
                $('#GeneralInfoMsgDiv').css('color', 'red');
                $('#GeneralInfoMsgDiv').html(ErrorList);
                $('#GeneralInfoMsgDiv').show();
            }
        },
        error: function () {
            alert('something went wrong !');
        }
    });
});

//$(document).on("change", "#BlackListed", function () {
//    var status = $(this).prop('checked');
//    if (status) {
//        $("div.blacklist").css("display", "block");
//    }
//    else {
//        $("div.blacklist").css("display", "none");
//        $("#BlackListReason").val("");
//    }
//});

$("#btnCancelGeneralInfo").on("click", function (e) {
    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?activetab=General Info'
});

$("#btnEditCancelGeneralInfo").on("click", function (e) {
    var id = $("#CustomerGeneralInfoId").val(); // form id
    window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + id + '&activetab=General Info'
});

$("#btnEditCancelGeneralInfoForJob").on("click", function (e) {
    var id = FSM.Jid; // form id
    window.location.href = common.SitePath + '/Employee/CustomerJob/SaveJobInfo?id=' + id + '&activetab=General Info'
});