var ManageDoc = {
    DocId: ''
}

$(document).on('click', '#btnUploadDocs', function () {
    if ($('#FilePosted')[0].files[0] != null) {
        var size = $('#FilePosted')[0].files[0].size;

        if (size > 11194304) {
            alert("Maximum file size 10 MB");
            return false;
        }
    }
    var formdata = new FormData($('#frmImportantDocs').get(0));
    $.ajax({
        url: $('#frmImportantDocs').attr("action"),

        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            $(window).scrollTop(0);
            if (result.errormsg != undefined && result.errormsg != '') {
                $('.importdoc-msg').html(result.errormsg);
                $('.importdoc-msg').css('color', 'red');
                $('.importdoc-msg').show();
            }
            else {
                ClearAllInput();
                $('.importdoc-msg').empty();
                $('.impdocs-list').empty();
                $('.impdocs-list').html(result);
                $('.jobalert').html('<strong>Record saved successfully !</strong>');
                $('.jobalert').css('color', 'green');
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                }, 4000)
            }
        },
        error: function () {
            alert('something went wrong !');
        }
    });
});

$(document).on('click', '.impdoc-download', function (event) {
    event.preventDefault();
    var filepath = $(this).attr('data-filepath');

    var aa = FSM.DownloadUrl;

    $.ajax({
        url: FSM.DownloadUrl,
        data: { Filepath: filepath },
        type: 'Get',
        async: false,
        success: function (data) {
            if (data.errormsg != undefined && data.errormsg != '') {
                alert(data.errormsg);
            }
            else {
                window.open(FSM.DocumentUrl + data);
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).on('click', '.impdoc-delete', function (event) {
    event.preventDefault();
    ManageDoc.DocId = $(this).attr('data-fileid');

    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure you want to delete document?");
    $(".btnconfirm").attr("id", ManageDoc.DocId);
    $(".modal-title").text("Delete document!");


});


$(document).on('click', ".btnconfirm", function () {
    $.ajax({
        url: FSM.DeleteUrl,
        data: { Fileid: ManageDoc.DocId },
        type: 'Get',
        async: false,
        success: function (data) {
            if (data.errormsg != undefined && data.errormsg != '') {
                alert(data.errormsg);
            }
            else {
                $('.impdocs-list').empty();
                $('.impdocs-list').html(data);
                $(".commonpopup").modal('hide');
                $('.jobalert').html('<strong>Document deleted successfully !</strong>');
                $('.jobalert').css('color', 'green');
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                }, 4000)
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
})
$(document).on('keypress', '#Description', function (event) {
    if (event.key == 'Enter') {
        event.preventDefault();
    }
});

function ClearAllInput() {
    $('#FilePosted').val('');
    $('#Description').val('');
}

$('#add_btnDoc').click(function () {
    {
        $('.document_collapse').toggle();
    }
});
$(document).off("click", '.Managequickview').on('click', '.Managequickview', function (event) {
    event.preventDefault();

    var action = $(this).attr('href');
    var data = {};
    $.ajax({
        url: action,
        type: 'GET',
        data: data,
        async: false,
        success: function (result) {
            $(window).scrollTop(0);
            $('.quickviewdiv').empty();
            $('.quickviewdiv').html(result);
            $('#modalManageQuickView').modal("show");
        },
        error: function () {
            alert('something went wrong !');
        }
    });
});