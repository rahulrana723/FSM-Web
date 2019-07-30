$(document).on("click", ".custsaveBtns", function () {
    var CID = $('#CustCid').val();
    var CustName = $('#CustLName').val();
    var Custtype = $('#CustType').val();
    if (CustName == "") {
        $('.text-validation').text("*Please enter file name");
        return false;
    }
    if (Custtype == "0") {
        $('.text-validation').text("*Please select customer type");
        return false;
    }

    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/_AddNewCustomer",
        data: { Cid: CID, CustLName: CustName, CustType: Custtype },
        type: 'Post',
        async: false,
        success: function (data) {
            $('.text-validation').show();
            $('.text-validation').css('color', 'Green')
            $('.text-validation').text("Record saved!");
            setTimeout(function () {
                location.reload();
            }, 3000);
        },
        error: function () {
            alert("something seems wrong");
        }
    })
});
$(document).on("click", "#custcancel", function () {
    $("#modalAddCustomer").modal("hide");
});