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
    var AssetStatus = $('#AssetAssignStatus').val();


    if (keyword != "" && keyword != undefined) {
        elementparam = elementparam + '&Searchkeyword=' + keyword;
    }
    if (AssetStatus != "" && AssetStatus != undefined && parseInt(AssetStatus) > 0 && elementparam.indexOf('AssetStatus=') < 0) {
        elementparam = elementparam + '&AssetAssignStatus=' + AssetStatus;
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
    var AssetStatus = $('#AssetAssignStatus').val();

    if (keyword != "" && keyword != undefined) {
        elementparam = elementparam + '&Searchkeyword=' + keyword ;
    }

    if (AssetStatus != "" && AssetStatus != undefined && parseInt(AssetStatus) > 0 && elementparam.indexOf('AssetStatus=') < 0) {
        elementparam = elementparam + '&AssetAssignStatus=' + AssetStatus;
    }
    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);


});


$(document).ready(function () {
})


function DeleteAssetManage(AssetId) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure to delete asset?");
    $(".btnconfirm").attr("id", AssetId);
    $(".modal-title").html("Delete Asset Management!");
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    $.ajax({
        type: 'POST',
        url: common.SitePath + "/Employee/AssetManagement/DeleteAssetManage",
        data: { AssetId: id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").delay(2000).fadeOut(function () { window.location.href = common.SitePath + "/Employee/AssetManagement/ViewAssetManagement"; });
        },
        error: function () {
            alert("something seems wrong");
            $(".commonpopup").modal('hide');
        }
    });
})


$(document).on('dblclick', '.cssEditAssetManage', function () {

    // getting customer id
    var AssetId = $(this).find('td:eq(0)').text();
    window.location = FSM.EditAsset + "/" + AssetId;
});