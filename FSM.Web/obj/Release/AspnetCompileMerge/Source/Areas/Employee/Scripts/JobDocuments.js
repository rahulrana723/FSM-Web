
$(document).off("click", '#btnSaveJobDocs').on('click', '#btnSaveJobDocs', function () {
    if ($('#JobDocs')[0].files.length > 0) {
        var size = $('#JobDocs')[0].files[0].size;
        if (size > 11194304) {
            alert("Maximum file size 10 MB");
            return false;
        }
        var formdata = new FormData($('#frmJobDocs').get(0));
        $.ajax({
            url: $('#frmJobDocs').attr("action"),
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            async: false,
            success: function (result) {
                $(window).scrollTop(0);
                $('#JobDocs').val('');
                $('.jobdocs-list').empty();
                $('.jobdocs-list').html(result);
            },
            error: function () {
                alert('something went wrong !');
            }
        });
    }
    else {
        alert("No file selected !");
        return false;
    }
});

function DeleteJobDocs(Id, JobId) {
    $(".commonpopupforJobDocs").modal('show');
    $(".alertmsg").text("Are you sure to delete job document?");
    $(".btndeleteconfirm ").attr("data-id", Id);
    $(".btndeleteconfirm ").attr("data-jobid", JobId);
    $(".modal-title").html("Delete Job document !");
}


$(document).off("click", '.btndeleteconfirm ').on('click', '.btndeleteconfirm ', function (event) {
    event.preventDefault();
    //if (confirm("Are you sure to delete this doc ?")) {
        //var action = $(this).attr('href');
        var docid = $(this).attr('data-id');
        var jobid = $(this).attr('data-jobid');

        $.ajax({
            url: common.SitePath + "/Employee/CustomerJob/DeleteJobDocument",
            data: { id: docid, jobid: jobid },
            type: 'GET',
            async: false,
            success: function (result) {
                $(".commonpopupforJobDocs").modal('hide');
                $(window).scrollTop(0);
                $('.jobdocs-list').empty();
                $('.jobdocs-list').html(result);
            },
            error: function () {
                alert('something went wrong !');
            }
        });
    //}
});

$(document).off("click", '.quickview').on('click', '.quickview', function (event) {
    event.preventDefault();

    var action = $(this).attr('href');
    var jobid = $(this).attr("data-jobid");
    var data = { id: jobid };

    $.ajax({
        url: action,
        type: 'GET',
        data: data,
        async: false,
        success: function (result) {
            $(window).scrollTop(0);
            $('.quickviewdiv').empty();
            $('.quickviewdiv').html(result);
            $('#modalQuickView').modal("show");
        },
        error: function () {
            alert('something went wrong !');
        }
    });
});
$('#add_btnDoc').click(function () {
    {
        $('.Jobdocument_collapse').toggle();
    }
});