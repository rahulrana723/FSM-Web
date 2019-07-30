$(document).on("click", ".sitesaveBtns", function () {
    $('.text-validation').html("");
        var customerGeneralInfoId=$("#CustGeneralId").val();
        var Unit=$("#siteUnit").val();
        var Street=$("#siteStreet").val();
        var Name=$("#siteName").val();
        var Type=$("#siteType").val();
        var Suburb=$("#siteSuburb").val();
        var State=$("#siteState").val();
        var PostalCode=$("#siteCode").val();
        var tabledata = {"CustomerGeneralInfoId": customerGeneralInfoId, "Unit": Unit, "Street": Street, "StreetName": Name,
            "StreetType": Type, "Suburb": Suburb, "State": State, "PostalCode": PostalCode }

        $.ajax({
            url: common.SitePath + "/Employee/CustomerJob/_AddNewCustomerSite",
            data: { model: tabledata },
            type: 'Post',
            async: false,
            success: function (data) {
                if (data.errors != undefined) {
                    var i = 0;
                    var errorList = '';
                    for (var key in data.errors) {
                        errorList = data.errors[i];
                        i = i + 1;
                        $('.text-validation').append(errorList + ',');
                    }
                }
                else
                {
                    $('.text-validation').show();
                    $('.text-validation').css('color', 'Green')
                    $('.text-validation').append("Record saved");
                    setTimeout(function () {
                        location.reload();
                    }, 3000);
                }
                
            },
            error: function () {
                alert("something seems wrong");
            }
        })
    });
$(document).on("click", "#sitecancel", function () {
    $("#modalAddSite").modal("hide");
});