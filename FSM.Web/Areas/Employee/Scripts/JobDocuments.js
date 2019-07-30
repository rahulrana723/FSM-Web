
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

//$(document).ready(function () {
//    var Job_id = $('.jobhdnid').val();

//    alert(Job_id);
//    //var SiteId=$("#SiteCountviewModel_SiteDetailId").val();
//    //alert(SiteId);
//    $('#dropArea').filedrop({

//        url: '/Employee/CustomerJob/UploadFiles',
//        allowedfiletypes: ['image/jpeg', 'image/png', 'image/gif'],
//        allowedfileextensions: ['.jpg', '.jpeg', '.png', '.gif'],
//        paramname: 'files',
//        maxfiles: 20,
//        maxfilesize: 5, // in MB
//        data: {
//            JobId: Job_id,
//        },
//        dragOver: function () {
//            //$("#dropArea").css("display", "block");
//            $('#dropArea').addClass('active-drop');
//        },
//        dragLeave: function () {
//            $('#dropArea').removeClass('active-drop');

//        },
//        drop: function () {
//            if (siteid == "" || siteid == 'undefined' || siteid == null) {
//                alert("please select Site.")
//                return false;
//            }
//            else {

//                //  siteid = $('#SiteCountviewModel_SiteDetailId  option:selected').val();
//                $('#dropArea').removeClass('active-drop');
//            }
//        },
//        afterAll: function (e) {
//            $('#dropArea').html('file(s) uploaded successfully');
//            $(window).scrollTop(0);
//            $('.jobalert').empty();
//            $('.jobalert').css('color', 'green');
//            $('.jobalert').html('<Strong>Document(s) Uploaded successfully!</Strong>');
//            $('.jobalert').show();
//            window.setTimeout(function () {
//                $('.jobalert').hide();
//                window.location.href = common.SitePath + '/Customer/Customer/AddCustomerInfo?id=' + CustomerId + '&activetab=Documents&success=ok';
//            }, 4000)
//            //   return RedirectToAction("AddCustomerInfo", new { id = model.CustomerSiteDocuments.CustomerGeneralInfoId.ToString(), activetab = "Documents", success = "ok" });
//        },
//        uploadFinished: function (i, file, response, time) {


//            //$("#dropArea").css("display", "none");
//            //$('#uploadList').append('<li class="list-group-item">'+file.name+'</li>')
//        }
//    })
//})
