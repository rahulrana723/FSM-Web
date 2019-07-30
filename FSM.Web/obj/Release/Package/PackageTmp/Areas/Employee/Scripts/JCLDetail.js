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

function DeleteJCL(JCLId) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete Sale Item?");
    $(".btnconfirm").attr("id", JCLId);
    $(".modal-title").html("Delete Sale Item Record!");
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/JCL/DeleteJCL",
        data: { JCLId: id },
        async: false,
        success: function (result) {
            if (result != false) {
                $("#GeneralInfoMsgDiv").empty();
                $(".commonpopup").modal('hide');
                $(".jobalert").css("display", "block");
                $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
                $(".jobalert").delay(2000).fadeOut(function () {
                    window.location.href = common.SitePath + "/Employee/JCL/GETJCLList";
                });
            }
            else
            {
                $("#GeneralInfoMsgDiv").empty();
                $(".commonpopup").modal('hide');
                $(".commonpopupformsg").modal('show');
                $(".delete_stock").text("Delete Related Data");
                $(".alertmsg").text("Please Delete JCL Regarding Data First");
                return false;
            }
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})

$(document).on('dblclick', '.cssEditJCL', function () {
    // getting customer id
    var JCLId = $(this).find('td:eq(0)').text();
    window.location = FSM.EditEmployee + "?JCLId=" + JCLId;
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

    
    if (keyword != "" && keyword != undefined && elementparam.indexOf('keyword=') < 0) {
        elementparam = elementparam + '&keyword=' + keyword;
    }


    var pagenum = $('.active span').text();
    if (pagenum != "" && pagenum != undefined && elementparam.indexOf('grid-page=') < 0) {
        elementparam = elementparam + '&grid-page=' + pagenum;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0 && elementparam.indexOf('page_size=') < 0) {
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
        elementparam = elementparam + '&keyword=' + keyword;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);

});