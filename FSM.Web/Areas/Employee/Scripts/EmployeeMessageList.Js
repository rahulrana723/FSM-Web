$(document).ready(function myfunction() {
    $(".Chatbtngrid").children().css("cursor", "pointer");
    var definition = { smile: { title: "Smile", codes: [":)", ":=)", ":-)"] }, "sad-smile": { title: "Sad Smile", codes: [":(", ":=(", ":-("] }, "big-smile": { title: "Big Smile", codes: [":D", ":=D", ":-D", ":d", ":=d", ":-d"] }, cool: { title: "Cool", codes: ["8)", "8=)", "8-)", "B)", "B=)", "B-)", "(cool)"] }, wink: { title: "Wink", codes: [":o", ":=o", ":-o", ":O", ":=O", ":-O"] }, crying: { title: "Crying", codes: [";(", ";-(", ";=("] }, sweating: { title: "Sweating", codes: ["(sweat)", "(:|"] }, speechless: { title: "Speechless", codes: [":|", ":=|", ":-|"] }, kiss: { title: "Kiss", codes: [":*", ":=*", ":-*"] }, "tongue-out": { title: "Tongue Out", codes: [":P", ":=P", ":-P", ":p", ":=p", ":-p"] }, blush: { title: "Blush", codes: ["(blush)", ":$", ":-$", ":=$", ':">'] }, wondering: { title: "Wondering", codes: [":^)"] }, sleepy: { title: "Sleepy", codes: ["|-)", "I-)", "I=)", "(snooze)"] }, dull: { title: "Dull", codes: ["|(", "|-(", "|=("] }, "in-love": { title: "In love", codes: ["(inlove)"] }, "evil-grin": { title: "Evil grin", codes: ["]:)", ">:)", "(grin)"] }, talking: { title: "Talking", codes: ["(talk)"] }, yawn: { title: "Yawn", codes: ["(yawn)", "|-()"] }, puke: { title: "Puke", codes: ["(puke)", ":&", ":-&", ":=&"] }, "doh!": { title: "Doh!", codes: ["(doh)"] }, angry: { title: "Angry", codes: [":@", ":-@", ":=@", "x(", "x-(", "x=(", "X(", "X-(", "X=("] }, "it-wasnt-me": { title: "It wasn't me", codes: ["(wasntme)"] }, party: { title: "Party!!!", codes: ["(party)"] }, worried: { title: "Worried", codes: [":S", ":-S", ":=S", ":s", ":-s", ":=s"] }, mmm: { title: "Mmm...", codes: ["(mm)"] }, nerd: { title: "Nerd", codes: ["8-|", "B-|", "8|", "B|", "8=|", "B=|", "(nerd)"] }, "lips-sealed": { title: "Lips Sealed", codes: [":x", ":-x", ":X", ":-X", ":#", ":-#", ":=x", ":=X", ":=#"] }, hi: { title: "Hi", codes: ["(hi)"] }, call: { title: "Call", codes: ["(call)"] }, devil: { title: "Devil", codes: ["(devil)"] }, angel: { title: "Angel", codes: ["(angel)"] }, envy: { title: "Envy", codes: ["(envy)"] }, wait: { title: "Wait", codes: ["(wait)"] }, bear: { title: "Bear", codes: ["(bear)", "(hug)"] }, "make-up": { title: "Make-up", codes: ["(makeup)", "(kate)"] }, "covered-laugh": { title: "Covered Laugh", codes: ["(giggle)", "(chuckle)"] }, "clapping-hands": { title: "Clapping Hands", codes: ["(clap)"] }, thinking: { title: "Thinking", codes: ["(think)", ":?", ":-?", ":=?"] }, bow: { title: "Bow", codes: ["(bow)"] }, rofl: { title: "Rolling on the floor laughing", codes: ["(rofl)"] }, whew: { title: "Whew", codes: ["(whew)"] }, happy: { title: "Happy", codes: ["(happy)"] }, smirking: { title: "Smirking", codes: ["(smirk)"] }, nodding: { title: "Nodding", codes: ["(nod)"] }, shaking: { title: "Shaking", codes: ["(shake)"] }, punch: { title: "Punch", codes: ["(punch)"] }, emo: { title: "Emo", codes: ["(emo)"] }, yes: { title: "Yes", codes: ["(y)", "(Y)", "(ok)"] }, no: { title: "No", codes: ["(n)", "(N)"] }, handshake: { title: "Shaking Hands", codes: ["(handshake)"] }, skype: { title: "Skype", codes: ["(skype)", "(ss)"] }, heart: { title: "Heart", codes: ["(h)", "<3", "(H)", "(l)", "(L)"] }, "broken-heart": { title: "Broken heart", codes: ["(u)", "(U)"] }, mail: { title: "Mail", codes: ["(e)", "(m)"] }, flower: { title: "Flower", codes: ["(f)", "(F)"] }, rain: { title: "Rain", codes: ["(rain)", "(london)", "(st)"] }, sun: { title: "Sun", codes: ["(sun)"] }, time: { title: "Time", codes: ["(o)", "(O)", "(time)"] }, music: { title: "Music", codes: ["(music)"] }, movie: { title: "Movie", codes: ["(~)", "(film)", "(movie)"] }, phone: { title: "Phone", codes: ["(mp)", "(ph)"] }, coffee: { title: "Coffee", codes: ["(coffee)"] }, pizza: { title: "Pizza", codes: ["(pizza)", "(pi)"] }, cash: { title: "Cash", codes: ["(cash)", "(mo)", "($)"] }, muscle: { title: "Muscle", codes: ["(muscle)", "(flex)"] }, cake: { title: "Cake", codes: ["(^)", "(cake)"] }, beer: { title: "Beer", codes: ["(beer)"] }, drink: { title: "Drink", codes: ["(d)", "(D)"] }, dance: { title: "Dance", codes: ["(dance)", "\\o/", "\\:D/", "\\:d/"] }, ninja: { title: "Ninja", codes: ["(ninja)"] }, star: { title: "Star", codes: ["(*)"] }, mooning: { title: "Mooning", codes: ["(mooning)"] }, finger: { title: "Finger", codes: ["(finger)"] }, bandit: { title: "Bandit", codes: ["(bandit)"] }, drunk: { title: "Drunk", codes: ["(drunk)"] }, smoking: { title: "Smoking", codes: ["(smoking)", "(smoke)", "(ci)"] }, toivo: { title: "Toivo", codes: ["(toivo)"] }, rock: { title: "Rock", codes: ["(rock)"] }, headbang: { title: "Headbang", codes: ["(headbang)", "(banghead)"] }, bug: { title: "Bug", codes: ["(bug)"] }, fubar: { title: "Fubar", codes: ["(fubar)"] }, poolparty: { title: "Poolparty", codes: ["(poolparty)"] }, swearing: { title: "Swearing", codes: ["(swear)"] }, tmi: { title: "TMI", codes: ["(tmi)"] }, heidy: { title: "Heidy", codes: ["(heidy)"] }, myspace: { title: "MySpace", codes: ["(MySpace)"] }, malthe: { title: "Malthe", codes: ["(malthe)"] }, tauri: { title: "Tauri", codes: ["(tauri)"] }, priidu: { title: "Priidu", codes: ["(priidu)"] } };
    $.emoticons.define(definition);
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

$(".Chatbtngrid").click(function () {

    var employeeId = $(this).attr("EmployeeId");
    $.ajax({
        url: common.SitePath + "/Employee/Message/_EmployeeMessageChat",
        data: { EmployeeId: employeeId },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowPopup").empty();
            $("#divShowPopup").html(data);
            $("#modalContactlog").modal("show");
            setTimeout(function () {
                var $cont = $('.modal-body');
                $cont[0].scrollTop = $cont[0].scrollHeight;
            }, 300);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});
$("#ViewInboxMesg").click(function () {
    $.ajax({
        url: common.SitePath + "/Employee/Message/_ViewInboxMessageChat",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#ViewMessagePopUp").empty();
            $("#ViewMessagePopUp").html(data);
            $("#modalViewMessageLog").modal("show");
            setTimeout(function () {
                var $cont = $('.modal-body');
                $cont[0].scrollTop = $cont[0].scrollHeight;
            }, 300);
            $(".ddlviewinbox").prop("selectedIndex", 0);
        },
        error: function () {
            alert("something seems wrong");
        }
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

    var searchstring = $('#searchkeyword').val();
    var Role = $('#Role').val();

    if (searchstring != "" && searchstring != undefined) {
        elementparam = elementparam + '&searchkeyword=' + searchstring;
    }

    if (Role != "" && Role != undefined) {
        elementparam = elementparam + '&Role=' + Role;
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

    var searchstring = $('#searchkeyword').val();
    var Role = $('#Role').val();

    if (searchstring != "" && searchstring != undefined) {
        elementparam = elementparam + '&searchkeyword=' + searchstring;
    }
    if (Role != "" && Role != undefined) {
        elementparam = elementparam + '&Role=' + Role;
    }


    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);

});


$(document).on("click", "#messageInboxSent", function () {
    var ToID = $('select.ddlviewinbox option:selected').val();
    var FromID = $('.FromId').val();
    var Messagebox = $('.msgsendArea').val();
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

$(document).on("keypress", ".msgsendArea", function (e) {

    var key = e.which;
    if (key == 13)  // the enter key code
    {
        $('#messageInboxSent').click();
        return false;
    }
});

$(document).off("click", "#thumbsndinbox").on("click", "#thumbsndinbox", function () {
    var ToID = $('select.ddlviewinbox option:selected').val();
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

$(document).off("click", "#thumbsndinboxdown").on("click", "#thumbsndinboxdown", function () {
    var ToID = $('select.ddlviewinbox option:selected').val();
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

//$(document).on("click", "#thumbsndinbox", function () {
//    var Message = $(".msgsendArea").val();
//    $(".msgsendArea").val(Message+"👍");
//});
