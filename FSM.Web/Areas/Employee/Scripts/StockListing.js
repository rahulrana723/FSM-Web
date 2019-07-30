$(document).ready(function () {
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

    if (FSM.Message != "" && FSM.Message != 0) {
        $(".jobalert").css("display", "block");
        if (FSM.Message == "1") {
            $(".jobalert").html("<strong>Record added Successfully!</strong>");
        }
        else if (FSM.Message == "2") {
            $(".jobalert").html("<strong>Record Updated Successfully!</strong>");
        }
        else if (FSM.Message == "3") {
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
        }
        $(".jobalert").delay(2000).fadeOut();
    }


})


$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?page_size=" + page_size;
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

    var keyword = $('#searchkeyword').val();


    if (keyword != "" && keyword != undefined) {
        elementparam = elementparam + '&Searchkeyword=' + keyword;
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
    var keyword = $('#searchkeyword').val();

    if (keyword != "" && keyword != undefined) {
        elementparam = elementparam + '&Searchkeyword=' + keyword;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);


});



$(".btnAssignotrw").click(function () {

    $.ajax({
        url: common.SitePath + "/Employee/Stock/_AssignOTRWStock",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowInventoryPopup").html(data);
            $("#modalAssigntootrw").modal("show");
            //$('.modal-backdrop').show();
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});
$(document).ready(function () {
    $("#Quantity").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");
})

var available = {
    maxavailable: 0
}
$(document).on('change', ".ddlstockname", function () {
    var stockid = $(this).val();
    if (stockid != "") {
        $.ajax({
            type: 'GET',
            url: common.SitePath + "/Employee/Stock/GetStockByStockid",
            data: { id: stockid },
            async: false,
            success: function (data) {
                var res = JSON.parse(data);
                $("#UnitMeasure").val(res.UnitMeasure);
                $("#maxquantity").val(res.Quantity);
                $(".divmaxquantity").css("display", "block");
                $(".maxquantity").text("Maximum Quantity:" + res.Available);
                available.maxavailable = res.Available;
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    }
});

function Validateformdata() {
    var stockid = $(".ddlstockname").val();
    var otrwid = $(".ddlOtrwname  ").val();
    if (stockid != "" && stockid != undefined) {
        $(".errorstockname").text("");
        if (otrwid != "" && otrwid != undefined) {
            $(".erroremployeename").text("");

            var val = $("#Quantity").val();
            if (val > 0 && val <= available.maxavailable) {
                $("#errrorquantity").text("");
                return true;
            }
            else {
                $("#errrorquantity").text("Please enter valid quantity").css("color", "red");
                return false;
            }
            return true;
        }
        else {
            $(".erroremployeename").text("Please select OTRW").css("color", "red");
            return false;
        }
    }
    else {
        $(".errorstockname").text("Please select Stock").css("color", "red");
        return false;
    }

}

function DeleteStock(stockid) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Deleting a stock will delete all its dependent data. Are you sure to delete stock?");
    $(".btnconfirm").attr("id", stockid);
    $(".modal-title").html("Delete Stock!");
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    $.ajax({
        type: 'POST',
        url: common.SitePath + "/Employee/Stock/DeleteStock",
        data: { id: id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").delay(2000).fadeOut(function () { window.location.href = common.SitePath + "/Employee/Stock/Index"; });
        },
        error: function () {
            alert("something seems wrong");
            $(".commonpopup").modal('hide');
        }
    });
})

$(".btnAssignJobs").click(function () {
    $.ajax({
        url: common.SitePath + "/Employee/Stock/_AssignJOBSStock",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowInventoryPopup").html(data);
            $("#modalAssignJOBS").modal("show");
            //$('.modal-backdrop').show();
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

function ValidateJobdata() {
    var stockid = $(".ddlstockname").val();
    var jobid = $(".ddlJobs  ").val();
    if (stockid != "" && stockid != undefined) {
        $(".errorstockname").text("");
        if (jobid != "" && jobid != undefined) {
            $(".erroremployeename").text("");

            var val = $("#Quantity").val();
            if (val > 0 && val <= available.maxavailable) {
                $("#errrorquantity").text("");
                return true;
            }
            else {
                $("#errrorquantity").text("Please enter valid quantity").css("color", "red");
                return false;
            }
            return true;
        }
        else {
            $(".erroremployeename").text("Please select JobNo").css("color", "red");
            return false;
        }
    }
    else {
        $(".errorstockname").text("Please select Stock").css("color", "red");
        return false;
    }

}

$(document).on('dblclick', '.cssEditStock', function () {
    
    // getting customer id
    var StockId = $(this).find('td:eq(0)').text();
    window.location = FSM.EditStock + "/" + StockId;
});