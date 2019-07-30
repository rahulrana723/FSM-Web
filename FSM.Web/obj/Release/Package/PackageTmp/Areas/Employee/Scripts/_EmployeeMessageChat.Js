
$(document).on("click", "#messageSent", function () {
    var ToID = $('.ToId').val();
    var FromID = $('.FromId').val();
    var Messagebox = $('.MessageBox').val();
    Messagebox = $.emoticons.replace(Messagebox);
    if (ToID == "D5549f68-593a-4f34-99ca-24df2C9dbc16" || ToID == "4e439ec2-016d-473a-81f8-893a5c9faf7d" || ToID == "3a676860-7548-4c7a-9f60-fe70e24dfa11") {
        var ddlTextValue = $('select.ddlviewinbox option:selected').text();
        $.ajax({
            url: common.SitePath + "/Employee/Message/_AllEmployeeMessageSend",
            data: { EmployeeName: ddlTextValue, ToId: ToID, FromId: FromID, MessageBox: Messagebox },
            type: 'Post',
            async: false,
            success: function (data) {
                location.reload();
            },
            error: function () {
                alert("something seems wrong");
            }
        })
    }
    else {
        $.ajax({
            url: common.SitePath + "/Employee/Message/_EmployeeMessageChat",
            data: { ToId: ToID, FromId: FromID, MessageBox: Messagebox },
            type: 'Post',
            async: false,
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                $(".MessageView").html("");
                var msgdiv = "";
                for (i = 0; i < data.length; i++) {
                    msgdiv = msgdiv + "<div><b style='color:#93c749'> " + response[i]["UserName"] + ":</b> " + response[i]["Message"] + " </div>";
                }
                $(".MessageView").append(msgdiv);
                $(".MessageBox").val("");
                var $cont = $('.modal-body');
                $cont[0].scrollTop = $cont[0].scrollHeight;
                location.reload();
            },
            error: function () {
                alert("something seems wrong");
            }
        })
    }

});
$(document).on("click", "#DeleteMessage", function () {
    var FromID = $('.FromId').val();
    var ToID = $('.ToId').val();

    $.ajax({
        url: common.SitePath + "/Employee/Message/DeleteMessageChat",
        data: { FromId: FromID, ToId: ToID },
        type: 'Post',
        async: false,
        success: function (data) {
            var response = jQuery.parseJSON(data.list);
            $(".MessageView").html("");
            var msgdiv = "";
            for (i = 0; i < data.length; i++) {
                msgdiv = msgdiv + "<div><b style='color:#93c749'> " + response[i]["UserName"] + ":</b> " + response[i]["Message"] + " </div>";
            }
            $(".MessageView").append(msgdiv);
            alert("Messages deleted successfully!");
        },
        error: function () {
            alert("something seems wrong");
        }
    })
});

$(document).on("keypress", "#UserMessageViewModel_Message", function (e) {

    var key = e.which;
    if (key == 13)  // the enter key code
    {
        $('#messageSent').click();
        return false;
    }
});

$(document).off("click", "#thumbsnd").on("click", "#thumbsnd", function () {
    var ToID = $('.ToId').val();
    var FromID = $('.FromId').val();
    var Messagebox = "(Y)";
    Messagebox = $.emoticons.replace(Messagebox);
    if (ToID == "D5549f68-593a-4f34-99ca-24df2C9dbc16" || ToID == "4e439ec2-016d-473a-81f8-893a5c9faf7d" || ToID == "3a676860-7548-4c7a-9f60-fe70e24dfa11") {
        var ddlTextValue = $('select.ddlviewinbox option:selected').text();
        $.ajax({
            url: common.SitePath + "/Employee/Message/_AllEmployeeMessageSend",
            data: { EmployeeName: ddlTextValue, ToId: ToID, FromId: FromID, MessageBox: Messagebox },
            type: 'Post',
            async: false,
            success: function (data) {
                location.reload();
            },
            error: function () {
                alert("something seems wrong");
            }
        })
    }
    else {
        $.ajax({
            url: common.SitePath + "/Employee/Message/_EmployeeMessageChat",
            data: { ToId: ToID, FromId: FromID, MessageBox: Messagebox },
            type: 'Post',
            async: false,
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                $(".MessageView").html("");
                var msgdiv = "";
                for (i = 0; i < data.length; i++) {
                    msgdiv = msgdiv + "<div><b style='color:#93c749'> " + response[i]["UserName"] + ":</b> " + response[i]["Message"] + " </div>";
                }
                $(".MessageView").append(msgdiv);
                $(".MessageBox").val("");
                location.reload();
            },
            error: function () {
                alert("something seems wrong");
            }
        })
    }
});

$(document).off("click", "#thumbsnddown").on("click", "#thumbsnddown", function () {
    var ToID = $('.ToId').val();
    var FromID = $('.FromId').val();
    var Messagebox = "(n)";
    Messagebox = $.emoticons.replace(Messagebox);
    if (ToID == "D5549f68-593a-4f34-99ca-24df2C9dbc16" || ToID == "4e439ec2-016d-473a-81f8-893a5c9faf7d" || ToID == "3a676860-7548-4c7a-9f60-fe70e24dfa11") {
        var ddlTextValue = $('select.ddlviewinbox option:selected').text();
        $.ajax({
            url: common.SitePath + "/Employee/Message/_AllEmployeeMessageSend",
            data: { EmployeeName: ddlTextValue, ToId: ToID, FromId: FromID, MessageBox: Messagebox },
            type: 'Post',
            async: false,
            success: function (data) {
                location.reload();
            },
            error: function () {
                alert("something seems wrong");
            }
        })
    }
    else {
        $.ajax({
            url: common.SitePath + "/Employee/Message/_EmployeeMessageChat",
            data: { ToId: ToID, FromId: FromID, MessageBox: Messagebox },
            type: 'Post',
            async: false,
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                $(".MessageView").html("");
                var msgdiv = "";
                for (i = 0; i < data.length; i++) {
                    msgdiv = msgdiv + "<div><b style='color:#93c749'> " + response[i]["UserName"] + ":</b> " + response[i]["Message"] + " </div>";
                }
                $(".MessageView").append(msgdiv);
                $(".MessageBox").val("");
                location.reload();
            },
            error: function () {
                alert("something seems wrong");
            }
        })
    }
});
//$(document).on("click", "#thumbsnd", function () {
//    var Message = $("#UserMessageViewModel_Message").val();
//    $("#UserMessageViewModel_Message").val(Message+"👍");
//});
