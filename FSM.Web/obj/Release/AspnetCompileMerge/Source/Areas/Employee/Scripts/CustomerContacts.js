$(document).on("click", "#AddContacts", function () {
    var formdata = new FormData($('#contactform').get(0));

    $.ajax({
        url: $('#contactform').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.status == "saved") {
                $('#contactsmsgDv').empty();
                $(window).scrollTop(0);
                $('.jobalert').empty();
                $('.jobalert').css('color', 'green');
                $('.jobalert').html(result.msg);
                $('.jobalert').show();
                window.setTimeout(function () {
                    $('.jobalert').hide();
                }, 4000)
            }
            else {
                $(window).scrollTop(0);
                $('#contactsmsgDv').empty();

                var ErrorList = "<ul style='list-style:none;'>"
                $(result.errors).each(function (i) {
                    ErrorList = ErrorList + "<li>" + result.errors[i].ErrorMessage + "</li>";
                });
                ErrorList = ErrorList + "</ul>"
                $(window).scrollTop(0);
                $('#contactsmsgDv').css('color', 'red');
                $('#contactsmsgDv').html(ErrorList);
                $('#contactsmsgDv').show();
                window.setTimeout(function () {
                    $('#contactsmsgDv').hide();
                }, 4000)
            }
        },
        error: function () {
            alert('something went wrong !');
        }
    });

});