$(document).ready(function myfunction() {
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
    $('th').removeClass('customer-sites');
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

    var CustomerLastName = $('#CustomerLastName').val();
    var CustomerType = $('#CustomerType').val();
    var Contracted = $('#Contracted').is(':checked');
    var BlackListed = $('#BlackListed').is(':checked');

    if (CustomerLastName != "" && CustomerLastName != undefined && elementparam.indexOf('CustomerLastName=') < 0) {
        elementparam = elementparam + '&CustomerLastName=' + CustomerLastName;
    }

    if (CustomerType != "" && CustomerType != undefined && parseInt(CustomerType) > 0 && elementparam.indexOf('CustomerType=') < 0) {
        elementparam = elementparam + '&CustomerType=' + CustomerType;
    }

    if (Contracted != "" && Contracted != undefined && elementparam.indexOf('Contracted=') < 0) {
        elementparam = elementparam + '&Contracted=' + Contracted;
    }

    if (BlackListed != "" && BlackListed != undefined && elementparam.indexOf('BlackListed=') < 0) {
        elementparam = elementparam + '&BlackListed=' + BlackListed;
    }

    var pagenum = $('.active span').text();
    if (pagenum != "" && pagenum != undefined && elementparam.indexOf('grid-page=') < 0) {
        elementparam = elementparam + '&grid-page=' + pagenum;
    }

    var pagesize = parseInt($('#ddPageSize').val() && elementparam.indexOf('page_size=') < 0);
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

    var CustomerLastName = $('#CustomerLastName').val();
    var CustomerType = $('#CustomerType').val();
    var Contracted = $('#Contracted').is(':checked');
    var BlackListed = $('#BlackListed').is(':checked');

    if (CustomerLastName != "" && CustomerLastName != undefined) {
        elementparam = elementparam + '&CustomerLastName=' + CustomerLastName;
    }

    if (CustomerType != "" && CustomerType != undefined && parseInt(CustomerType) > 0) {
        elementparam = elementparam + '&CustomerType=' + CustomerType;
    }

    if (Contracted != "" && Contracted != undefined) {
        elementparam = elementparam + '&Contracted=' + Contracted;
    }

    if (BlackListed != "" && BlackListed != undefined) {
        elementparam = elementparam + '&BlackListed=' + BlackListed;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }
    $(this).attr('href', elementparam);
});

$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?page_size=" + page_size;
});

function DeleteCustomer(CustomergeneralInfoId) {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure to delete customer?");
    $(".btnconfirm").attr("id", CustomergeneralInfoId);
    $(".modal-title").html("Delete Customer!");

    //$.ajax({
    //    url: common.SitePath + "/Customer/Customer/CheckCustomerJob",
    //    data: { CustomerGeneralInfoId: CustomergeneralInfoId },
    //    type: 'Get',
    //    async: false,
    //    success: function (data) {
    //        if (data === 0) {
    //            $(".commonpopupformsg").modal('show');
    //            $(".alertmsg").text("Please delete job of this customer.");
    //            return false;
    //        }
    //        else
    //        {
    //           $(".commonpopup").modal('show');
    //           $(".alertmsg").text("Are you sure to delete customer?");
    //           $(".btnconfirm").attr("id", CustomergeneralInfoId);
    //           $(".modal-title").html("Delete Customer!");
    //        }
    //    },
    //    error: function () {
    //        alert("something seems wrong");
    //    }
    // });
}

$(document).on('click', ".btnconfirm", function () {
    
    var id = $(this).attr("id");
    $.ajax({
        type: 'POST',
        url: common.SitePath + "/Customer/Customer/DeleteCustomer",
        data: { CustomerGeneralInfoId: id },
        async: false,
        success: function (result) {
            $(".commonpopup").modal('hide');
            $(".jobalert").css("display", "block");
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
            setTimeout(function () {
                location.reload();
            }, 3000);
        },
        error: function () {
            $(".commonpopup").modal('hide');
        }
    });
})

$(document).on('dblclick', '.cssEditCustomer', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();

    window.location = FSM.EditCustomer + "/" + id;
});

$(document).on('click', '.customer-sites', function () {
    // getting customer id
    var id = $(this).siblings(":first").text();
    window.location = FSM.EditCustomer + "?id=" + id + "&activetab=Customer Sites&hassitecount=yes";
});

$(document).on('click', '.getsiteaddress', function (event) {
    {
        event.preventDefault();
        var id = $(this).attr('data-id');
        var data = { id: id };

        $.get(FSM.SiteAddressURL, data, function (result) {
            $('.address-popup').html(result);
            $('#modalSiteAddress').modal('show');
        });
    }
});