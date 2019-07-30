$(document).ready(function () {
    $('#ddPageSize').val(FSM.PageSize);
    if ($(".pagination").length > 0) {
        $('#ddPageSize').parent().css('margin-top', '-72px');
    }
    else {
        $('#ddPageSize').parent().css('margin-top', '8px');
    }
    // select parent actions
    parentSelect();

});

$('input[type="checkbox"].rolemodule').change(function () {
    var $form = $("#formManageRolesActions");
    var arrIds = [];
    var arrDeactivateIds = [];
    var elmnt = $('.activeaction');
    var deactiveelmnt = $('.deactiveaction');
    var modulename = $(this).attr('data-modulename');

    // getting active checkbox
    $(elmnt).each(function (i) {
        var id = $(elmnt[i]).attr('data-id');
        arrIds.push(id);
    });

    // getting deactive checkbox
    $(deactiveelmnt).each(function (i) {
        var id = $(deactiveelmnt[i]).attr('data-id');
        arrDeactivateIds.push(id);
    });

    $.ajax({
        type: "POST",
        url: FSM.GetUserActionsUrl,
        data: $form.serialize(),
        cache: 'false',
        success: function (data) {
            $('#userActionsDv').html('');
            $('#userActionsDv').html(data);
        },
        complete: function () {
            $(arrIds).each(function (i) {
                var id = $("input[data-id='" + arrIds[i] + "']").attr('id');
                $('#' + id + '').prop('checked', true);
                $('#' + id + '').addClass('activeaction');
                $('#' + id + '').removeClass('deactiveaction');
            });
            $(arrDeactivateIds).each(function (i) {
                var id = $("input[data-id='" + arrDeactivateIds[i] + "']").attr('id');
                $('#' + id + '').prop('checked', false);
                $('#' + id + '').addClass('deactiveaction');
                $('#' + id + '').removeClass('activeaction');
            });
            // select parent action
            var module = $("input[data-name='" + modulename + "']").length;
            var activemodule = $("input[data-name='" + modulename + "'].activeaction").length;
            if (module == (activemodule + 1)) {
                $("input[data-name='" + modulename + "'].selectall").prop('checked', true);
                $("input[data-name='" + modulename + "'].selectall").removeClass('deactiveaction');
                $("input[data-name='" + modulename + "'].selectall").addClass('activeaction');
            }

        },
        error: function (data) {
            alert("Something went wrong !");
        }
    });
});

$(document).on("change", "input[type='checkbox'].roleactions", function () {
    var cbAttrVal = $(this).attr("data-name");

    if ($(this).is(":checked")) {
        $(this).removeClass('deactiveaction');
        $(this).addClass('activeaction');
    }
    else {
        $(this).removeClass('activeaction');
        $(this).addClass('deactiveaction');
    }
    var module = $("input[data-name='" + cbAttrVal + "'].roleactions").length;
    var activemodule = $("input[data-name='" + cbAttrVal + "'].roleactions.activeaction").length;
    if (module == activemodule) {
        $("input[data-name='" + cbAttrVal + "'].selectall").prop('checked', true);
        $("input[data-name='" + cbAttrVal + "'].selectall").removeClass('deactiveaction');
        $("input[data-name='" + cbAttrVal + "'].selectall").addClass('activeaction');
    }
    else {
        $("input[data-name='" + cbAttrVal + "'].selectall").prop('checked', false);
        $("input[data-name='" + cbAttrVal + "'].selectall").removeClass('activeaction');
        $("input[data-name='" + cbAttrVal + "'].selectall").addClass('deactiveaction');
    }

});

$(document).on("change", "input[type='checkbox'].selectall", function () {
    var cbAttrVal = $(this).attr("data-name");
    if ($(this).is(":checked")) {
        $("input[data-name='" + cbAttrVal + "']").prop('checked', true);
        $("input[data-name='" + cbAttrVal + "']").removeClass('deactiveaction');
        $("input[data-name='" + cbAttrVal + "']").addClass('activeaction');
    }
    else {
        $("input[data-name='" + cbAttrVal + "']").prop('checked', false);
        $("input[data-name='" + cbAttrVal + "']").removeClass('activeaction');
        $("input[data-name='" + cbAttrVal + "']").addClass('deactiveaction');
    }
});

$(document).on("click", "#editRole", function (event) {
    event.preventDefault();

    var queryString = window.location.href.indexOf("?") > 0 ? window.location.href.substring(window.location.href.indexOf("?")) : "";
    var roleid = $(this).attr('data-roleid');
    var data = { RoleId: roleid };

    $.get(FSM.EditUserRoleUrl + queryString, data, function (result) {
        $('#divRolePopup').html(result);
        $('#modalUserRole').modal('show');
    });
});

$(document).on('dblclick', '.cssEditRole', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();
    var queryString = window.location.href.indexOf("?") > 0 ? window.location.href.substring(window.location.href.indexOf("?")) : "";

    var data = { RoleId: id };

    $.get(FSM.EditUserRoleUrl + queryString, data, function (result) {
        $('#divRolePopup').html(result);
        $('#modalUserRole').modal('show');
    });
});

$(document).on("click", "#saveRole", function (event) {
    var $form = $("#formEditUserRole");

    $.ajax({
        type: "POST",
        url: FSM.EditUserRoleUrl,
        data: $form.serialize(),
        cache: 'false',
        success: function (data) {
            $('#rolesListDv').html('');
            $('#rolesListDv').html(data);
            $('#modalUserRole').modal('hide');

            $(window).scrollTop(0);
            $('.jobalert').css('color', 'green');
            $('.jobalert').html('<strong>Record updated successfully !</strong>');
            $('.jobalert').show();
            window.setTimeout(function () {
                $('.jobalert').hide();
            }, 4000)
        },
        error: function (data) {
            alert("Something went wrong !");
        }
    });
});

$(document).on("click", "#deleteRole", function (event) {
    event.preventDefault();

    var queryString = window.location.href.indexOf("?") > 0 ? window.location.href.substring(window.location.href.indexOf("?")) : "";
    var roleid = $(this).attr('data-roleid');
    var data = { RoleId: roleid };

    var confirmdelete = confirm('User having this role may not work further. Are you sure to delete it?');
    if (confirmdelete) {
        $.get(FSM.DeleteUserRoleUrl + queryString, data, function (result) {
            $('#rolesListDv').html('');
            $('#rolesListDv').html(result);

            $(window).scrollTop(0);
            $('.jobalert').css('color', 'green');
            $('.jobalert').html('<strong>Role deleted successfully !</strong>');
            $('.jobalert').show();
            window.setTimeout(function () {
                $('.jobalert').hide();
            }, 4000)

        });
    }
});

$(document).on("click", "#addUserRoles", function (event) {
    $('.jobalert').empty();
    var $form = $("#formAddUserRole");

    $.ajax({
        type: "POST",
        url: FSM.AddUserRoleUrl,
        data: $form.serialize(),
        cache: 'false',
        success: function (data) {
            if (data.error != "" && data.error != undefined) {
                $(window).scrollTop(0);
                $('#roleMsgDv').css('color', 'red');
                $('#modalAddUserRole').modal('hide');
                $('#roleMsgDv').html(data.error);
                $('#roleMsgDv').show();
            }
            else {
                $('#rolesListDv').html('');
                $('#rolesListDv').html(data);
                $('#modalAddUserRole').modal('hide');

                $(window).scrollTop(0);
                $('.jobalert').css('color', 'green');
                $('.jobalert').html('<strong>Record Saved successfully !</strong>');
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                }, 4000)
            }
        },
        error: function (data) {
            alert("Something went wrong !");
        }
    });
});

$(document).on("change", "#ddPageSize", function () {
    var pageSize = $('#ddPageSize').val();
    var Searrchkeyword = $('#searchkeyword').val();
    var data = { PageSize: pageSize, searchKeyword: Searrchkeyword };

    $.get(FSM.DefaultUrl, data, function (result) {
        $('#rolesListDv').html('');
        $('#rolesListDv').html(result);
    });
});

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

    var Searrchkeyword = $('#searchkeyword').val();

    if (Searrchkeyword != "" && Searrchkeyword != undefined) {
        elementparam = elementparam + '&searchkeyword=' + Searrchkeyword;
    }

    var pagenum = $('.active span').text();
    if (pagenum != "" && pagenum != undefined) {
        elementparam = elementparam + '&grid-page=' + pagenum;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&PageSize=' + pagesize;
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

    var Searrchkeyword = $('#searchkeyword').val();

    if (Searrchkeyword != "" && Searrchkeyword != undefined) {
        elementparam = elementparam + '&searchkeyword=' + Searrchkeyword;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&PageSize=' + pagesize;
    }

    $(this).attr('href', elementparam);

});

$('.support_role').click(function (event) {
    {
        event.preventDefault();
        var queryString = window.location.href.indexOf("?") > 0 ? window.location.href.substring(window.location.href.indexOf("?")) : "";
        var roleid = $(this).attr('data-roleid');
        var data = { RoleId: roleid };

        $.get(FSM.AddUserRoleUrl + queryString, data, function (result) {
            $('#divRolePopup').html(result);
            $('#modalAddUserRole').modal('show');
        });
    }
});

function parentSelect() {
    // for customer module
    var moduleCustomers = $("input[data-name='Customers']").length;
    var activemoduleCustomers = $("input[data-name='Customers'].activeaction").length;
    if (moduleCustomers == (activemoduleCustomers + 1)) {
        $("input[data-name='Customers'].selectall").prop('checked', true);
        $("input[data-name='Customers'].selectall").removeClass('deactiveaction');
        $("input[data-name='Customers'].selectall").addClass('activeaction');
    }

    // for employee holidays module
    //var moduleempholiday = $("input[data-name='Employee Leave']").length;
    //var activemoduleempholiday = $("input[data-name='Employee Leave'].activeaction").length;
    //if (moduleempholiday == (activemoduleempholiday + 1)) {
    //    $("input[data-name='Employee Leave'].selectall").prop('checked', true);
    //    $("input[data-name='Employee Leave'].selectall").removeClass('deactiveaction');
    //    $("input[data-name='Employee Leave'].selectall").addClass('activeaction');
    //}

    // for employee holidays module
    var moduleempholiday = $("input[data-name='Employee Holidays']").length;
    var activemoduleempholiday = $("input[data-name='Employee Holidays'].activeaction").length;
    if (moduleempholiday == (activemoduleempholiday + 1)) {
        $("input[data-name='Employee Holidays'].selectall").prop('checked', true);
        $("input[data-name='Employee Holidays'].selectall").removeClass('deactiveaction');
        $("input[data-name='Employee Holidays'].selectall").addClass('activeaction');
    }
    // for employee list module
    var moduleemplist = $("input[data-name='Employee List']").length;
    var activemoduleemplist = $("input[data-name='Employee List'].activeaction").length;
    if (moduleemplist == (activemoduleemplist + 1)) {
        $("input[data-name='Employee List'].selectall").prop('checked', true);
        $("input[data-name='Employee List'].selectall").removeClass('deactiveaction');
        $("input[data-name='Employee List'].selectall").addClass('activeaction');
    }

    // for invoices module
    var moduleInvoices = $("input[data-name='Invoices']").length;
    var activemoduleInvoices = $("input[data-name='Invoices'].activeaction").length;
    if (moduleInvoices == (activemoduleInvoices + 1)) {
        $("input[data-name='Invoices'].selectall").prop('checked', true);
        $("input[data-name='Invoices'].selectall").removeClass('deactiveaction');
        $("input[data-name='Invoices'].selectall").addClass('activeaction');
    }

    // for jobs module
    var moduleJobs = $("input[data-name='Jobs']").length;
    var activemoduleJobs = $("input[data-name='Jobs'].activeaction").length;
    if (moduleJobs == (activemoduleJobs + 1)) {
        $("input[data-name='Jobs'].selectall").prop('checked', true);
        $("input[data-name='Jobs'].selectall").removeClass('deactiveaction');
        $("input[data-name='Jobs'].selectall").addClass('activeaction');
    }

    // for manage holidays module
    var moduleholiday = $("input[data-name='Manage Holidays']").length;
    var activemoduleholiday = $("input[data-name='Manage Holidays'].activeaction").length;
    if (moduleholiday == (activemoduleholiday + 1)) {
        $("input[data-name='Manage Holidays'].selectall").prop('checked', true);
        $("input[data-name='Manage Holidays'].selectall").removeClass('deactiveaction');
        $("input[data-name='Manage Holidays'].selectall").addClass('activeaction');
    }

    // for message module
    var moduleMessages = $("input[data-name='Messages']").length;
    var activemoduleMessages = $("input[data-name='Messages'].activeaction").length;
    if (moduleMessages == (activemoduleMessages + 1)) {
        $("input[data-name='Messages'].selectall").prop('checked', true);
        $("input[data-name='Messages'].selectall").removeClass('deactiveaction');
        $("input[data-name='Messages'].selectall").addClass('activeaction');
    }

    // for public holiday module
    var modulepubholiday = $("input[data-name='Public Holidays']").length;
    var activemodulepubholiday = $("input[data-name='Public Holidays'].activeaction").length;
    if (modulepubholiday == (activemodulepubholiday + 1)) {
        $("input[data-name='Public Holidays'].selectall").prop('checked', true);
        $("input[data-name='Public Holidays'].selectall").removeClass('deactiveaction');
        $("input[data-name='Public Holidays'].selectall").addClass('activeaction');
    }

    // for purchase order module
    var modulepurchase = $("input[data-name='Purchase Order']").length;
    var activemodulepurchase = $("input[data-name='Purchase Order'].activeaction").length;
    if (modulepurchase == (activemodulepurchase + 1)) {
        $("input[data-name='Purchase Order'].selectall").prop('checked', true);
        $("input[data-name='Purchase Order'].selectall").removeClass('deactiveaction');
        $("input[data-name='Purchase Order'].selectall").addClass('activeaction');
    }

    // for stock module
    var moduleStock = $("input[data-name='Stock']").length;
    var activemoduleStock = $("input[data-name='Stock'].activeaction").length;
    if (moduleStock == (activemoduleStock + 1)) {
        $("input[data-name='Stock'].selectall").prop('checked', true);
        $("input[data-name='Stock'].selectall").removeClass('deactiveaction');
        $("input[data-name='Stock'].selectall").addClass('activeaction');
    }

    // for timesheet module
    var moduleTimeSheet = $("input[data-name='TimeSheet']").length;
    var activemoduleTimeSheet = $("input[data-name='TimeSheet'].activeaction").length;
    if (moduleTimeSheet == (activemoduleTimeSheet + 1)) {
        $("input[data-name='TimeSheet'].selectall").prop('checked', true);
        $("input[data-name='TimeSheet'].selectall").removeClass('deactiveaction');
        $("input[data-name='TimeSheet'].selectall").addClass('activeaction');
    }

    // for JCL module
    var moduleTimeSheet = $("input[data-name='JCL']").length;
    var activemoduleTimeSheet = $("input[data-name='JCL'].activeaction").length;
    if (moduleTimeSheet == (activemoduleTimeSheet + 1)) {
        $("input[data-name='JCL'].selectall").prop('checked', true);
        $("input[data-name='JCL'].selectall").removeClass('deactiveaction');
        $("input[data-name='JCL'].selectall").addClass('activeaction');
    }

    // for Settings module
    var moduleTimeSheet = $("input[data-name='Settings']").length;
    var activemoduleTimeSheet = $("input[data-name='Settings'].activeaction").length;
    if (moduleTimeSheet == (activemoduleTimeSheet + 1)) {
        $("input[data-name='Settings'].selectall").prop('checked', true);
        $("input[data-name='Settings'].selectall").removeClass('deactiveaction');
        $("input[data-name='Settings'].selectall").addClass('activeaction');

    }
    // for User Logs module
    var moduleTimeSheet = $("input[data-name='User Logs']").length;
    var activemoduleTimeSheet = $("input[data-name='User Logs'].activeaction").length;
    if (moduleTimeSheet == (activemoduleTimeSheet + 1)) {
        $("input[data-name='User Logs'].selectall").prop('checked', true);
        $("input[data-name='User Logs'].selectall").removeClass('deactiveaction');
        $("input[data-name='User Logs'].selectall").addClass('activeaction');

    }
    // for Asset module
    var moduleTimeSheet = $("input[data-name='Asset']").length;
    var activemoduleTimeSheet = $("input[data-name='Asset'].activeaction").length;
    if (moduleTimeSheet == (activemoduleTimeSheet + 1)) {
        $("input[data-name='Asset'].selectall").prop('checked', true);
        $("input[data-name='Asset'].selectall").removeClass('deactiveaction');
        $("input[data-name='Asset'].selectall").addClass('activeaction');

    }
}

