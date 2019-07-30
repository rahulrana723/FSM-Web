$(document).ready(function () {
    var categoryId = $("#CategoryId").val();
    if (categoryId == "") {
        $("#SubCategoryId").prop("disabled", true);
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
        else if (FSM.Message == "4") {
            $(".jobalert").html("<strong>Record Send Successfully!</strong>");
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

function DeleteRates(SubCategoryId) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure to delete employee rate?");
    $(".btnconfirm").attr("id", SubCategoryId);
    $(".modal-title").html("Delete Employee Rate !");
}

$(document).on('click', ".btnconfirm", function () {
    var SubCategoryId = $(this).attr("id");
    $.ajax({
        url: common.SitePath + "/Employee/Employee/DeleteEmployeeRate",
        data: { SubCategoryId: SubCategoryId },
        type: 'Get',
        async: false,
        success: function (data) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Emplotee Rate Deleted Successfully!</strong>");
            $(".jobalert").delay(2000).fadeOut(function () {
                location.reload();
            });
        },
        error: function () {
            alert("something seems wrong");
        }
    });
})

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
    var keyword = $('#Keyword').val();

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

$(document).on('dblclick', '.cssEditRates', function () {
    // getting customer id
    var subcategoryid = $(this).find('td:eq(0)').text();

    window.location = "/Employee/Employee/EditEmployeeRates?SubcategoryId=" + subcategoryid;
});