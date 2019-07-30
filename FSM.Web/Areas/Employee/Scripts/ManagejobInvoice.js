
var Purchaseorderjob = {
    CurrentStockPrice: 0,
    RowNotoEdit: 0,
    totalcost: 0,
    isExisted: false,
    maxquantity: 0,
    preaddedstockquantity: 0,
    JobId: '',
    deletedPrice: 0,
    GetStockList: function () {
        $.ajax({
            type: 'POST',
            url: common.SitePath + "/Employee/Stock/GetStocklist",
            data: {},
            async: false,
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                $(".ddlstock").html("");
                var site = "<option val='0'>(Select)</option>";
                for (i = 0; i < data.length; i++) {
                    $(".ddlstock").append(
                            $('<option></option>').val(response[i]["ID"]).html(response[i]["Label"])
                            );
                }
                $('.ddlstock').prepend('<option value="0" selected="selected">(Select)</option>');
            }
        });
    },
    GetSupplierList: function () {
        $.ajax({
            type: 'POST',
            url: common.SitePath + "/Employee/Job/GetSuppliers",
            data: {},
            async: false,
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                $(".ddlsupplier").html("");
                var site = "<option val='0'>(Select)</option>";
                for (i = 0; i < data.length; i++) {
                    $(".ddlsupplier").append(
                            $('<option></option>').val(response[i]["ID"]).html(response[i]["Name"])
                            );
                }
                $('.ddlsupplier').prepend('<option value="0" selected="selected">(Select)</option>');
            }
        });
    },
    SearchTableForExistingStcokLabel: function (stockid, Quantity) {
        $(".tblstock tr").each(function (index) {
            if (index != 0) {
                $row = $(this);
                var id = $row.find("td:first").text();
                if (id == stockid) {
                    var quantity = $row.find("td")[3].textContent
                    var price = $row.find("td")[4].textContent
                    var oneprice = price / quantity;
                    var totalquantity = parseInt(quantity) + parseInt(Quantity);
                    $row.children('td')[3].textContent = totalquantity;
                    $row.children('td')[4].textContent = parseFloat(oneprice) * parseInt(totalquantity);
                    Purchaseorderjob.isExisted = true;

                }
            }
        })
    },
    GetInvoiceDetailToEdit: function () {
        if (Purchaseorderjob.JobId != null && Purchaseorderjob.JobId != '0' && Purchaseorderjob.JobId != "") {
            var _cost, _Supplier = 0, description;
            $.ajax({
                type: 'GET',
                url: common.SitePath + "/Employee/Invoice/GetInvoicePurchaseItemByJobId",
                data: { jobid: Purchaseorderjob.JobId },
                async: false,
                success: function (data) {
                    $(".tblstock tbody").empty();
                    var response = jQuery.parseJSON(data.list);
                    for (i = 0; i < data.length; i++) {
                        var row = "<tr>";
                        row = row + "<td style='display:none'>" + response[i]["StockId"].toString().toLowerCase() + "</td> \
                    <td>" + response[i]["StockLabel"] + "</td> \
                    <td>" + response[i]["UnitMeasure"] + "</td> \
                    <td>" + response[i]["Quantity"] + "</td> \
                    <td>" + parseFloat(response[i]["Price"]).toFixed(2) + "</td> \
                    <td style='display:none'>" + parseFloat(response[i]["AvailableQuatity"]) + "</td> \
                    <td><a data-attr='" + response[i]["StockId"] + "' price='" + response[i]["Price"] + "' class='btn btn-default btnEditstock'>Edit </a> | <a  price='" + response[i]["Price"] + "' data-attr='" + response[i]["StockId"] + "' class='btn btn-default btnDeletestock'>Delete </a></td> ";
                        $(".tblstock tbody").append(row);
                        _cost = parseFloat(response[0]["Cost"]).toFixed(2);
                        _Supplier = response[0]["Supplierid"];
                        description = response[0]["Description"];
                    }
                    $("#Cost").val(_cost);
                    $(".ddlsupplier").val(_Supplier.toString().toLowerCase());
                    $(".itemdescription").val(description);
                },
                error: function () {
                    alert("something seems wrong");
                }
            });
        }
    }
}
var Rowtodelete;
$(document).ready(function () {

 //  $(".ddlmultiselect").multiselect();
})
//***stock change***
$(document).on('change', ".ddlstock", function () {
    var stockid = $(this).val();
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Stock/GetStockByStockid",
        data: { id: stockid },
        async: false,
        success: function (data) {
            var res = JSON.parse(data);
            $("#UnitOfMeasure").val(res.UnitMeasure);
            $("#maxunits").val(res.Quantity);
            $("#itemPrice").val(res.Price);
            $("#available").val(res.Available);
            $("#Quantity").val("");
            Purchaseorderjob.CurrentStockPrice = res.Price;
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});
//change on quantity changes
$(document).on('change', "#Quantity", function () {
    var quatity = $(this).val();
    if (parseInt(quatity) >= parseInt(1) && parseInt(quatity) <= parseInt(10000)) {
        var newprice = parseInt(quatity) * Purchaseorderjob.CurrentStockPrice;
        $("#itemPrice").val(newprice);
        $(".errordiv").html("");
    }
    else {
        $(this).val("");
        $(".errordiv").html("Quantity must be between 1 and 10000").css("color", "Red");
    }
})

//Add item to purchaseTable
$(document).on('click', '.btnAddstockitem', function () {
    Label = $(".ddlstock option:selected").text();
    Stockid = $(".ddlstock").val();
    unitMeasure = $("#UnitOfMeasure").val();
    Quantity = $("#Quantity").val();
    if (Quantity == '0' || Quantity == 'undefined' || Quantity == "") {
        var flag = 1;
    }
    if (Stockid == '0' || Stockid == '' || Stockid == 'undefined') {
        flag = 1;
    }
    if (flag == 1) {
        $(".errordiv").html("Please enter purchase item and provide quantity between 1 to 10000").css("color", "Red");
        return false;
    }
    Price = $(".stockprice").val();
    available = $("#available").val();
    if (Purchaseorderjob.RowNotoEdit == 0) {
        var searchrow = Purchaseorderjob.SearchTableForExistingStcokLabel(Stockid, Quantity);
        if (!Purchaseorderjob.isExisted) {
            $(".btnAddstockitem").text("Add Item");
            var row = "<tr>";
            row = row + "<td style='display:none'>" + Stockid + "</td> \
                    <td>" + Label + "</td> \
                    <td>" + unitMeasure + "</td> \
                    <td>" + Quantity + "</td> \
                    <td>" + parseFloat(Price).toFixed(2) + "</td> \
                    <td style='display:none'>" + parseInt(available).toFixed(2) + "</td> \
                    <td><a data-attr='" + Stockid + "' price='" + Price + "' class='btn btn-default btnEditstock'>Edit </a> | <a price='" + Price + "' data-attr='" + Stockid + "' class='btn btn-default btnDeletestock'>Delete </a></td> ";
            $(".tblstock tbody").append(row);
        }
    }
    else {
        var trtoEdit = $('.tblstock tr').eq(Purchaseorderjob.RowNotoEdit);
        trtoEdit.children('td')[0].textContent = Stockid;
        trtoEdit.children('td')[1].textContent = Label;
        trtoEdit.children('td')[2].textContent = unitMeasure;
        trtoEdit.children('td')[3].textContent = Quantity;
        trtoEdit.children('td')[4].textContent = Price;
    }
    var tablecost = 0;
    $(".tblstock tr").each(function (index) {
        if (index != 0) {
            var cost = $(this).find("td")[4].textContent
            tablecost = tablecost + parseFloat(cost);
            Purchaseorderjob.totalcost = tablecost;
        }
    });
    $("#Cost").val(Purchaseorderjob.totalcost);
    $(".ddlstock ").val("");
    $("#UnitOfMeasure").val("");
    $(".stockprice").val("");
    $("#Quantity").val("")
    Purchaseorderjob.totalcost = 0;
    Purchaseorderjob.isExisted = false;
    Purchaseorderjob.preaddedstockquantity = 0;
    Purchaseorderjob.RowNotoEdit = 0;
    Purchaseorderjob.maxquantity = 0;
    Purchaseorderjob.CurrentStockPrice = 0;
    Purchaseorderjob.RowNotoEdit = 0;
    $(".btnAddstockitem").text("Add Item");
})
$(document).on('click', '.btnEditstock', function () {
    $(".btnAddstockitem").text("Update");
    Purchaseorderjob.RowNotoEdit = $(this).closest("tr")[0].rowIndex;
    var Stockid = $(this).closest("tr").children('td')[0].textContent;
    var UnitMeasure = $(this).closest("tr").children('td')[2].textContent;
    var Quantity = $(this).closest("tr").children('td')[3].textContent;
    var price = $(this).closest("tr").children('td')[4].textContent;
    var available = $(this).closest("tr").children('td')[5].textContent;
    $(".ddlstock").val(Stockid.toString().toLowerCase());
    $("#UnitOfMeasure").val(UnitMeasure);
    $("#Quantity").val(Quantity);
    $("#itemPrice").val(price);
    Purchaseorderjob.preaddedstockquantity = 0;
    Purchaseorderjob.CurrentStockPrice = parseFloat(price) / parseInt(Quantity);
});
//only integer allowed  check 
$(document).on('keydown', "#Quantity", function (event) {
    if (event.shiftKey == true) {
        event.preventDefault();
    }
    if ((event.keyCode >= 48 && event.keyCode <= 57) ||
        (event.keyCode >= 96 && event.keyCode <= 105) ||
        event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
        event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {
    } else {
        event.preventDefault();
    }
    if (event.keyCode == 190)
        event.preventDefault();
});

$(document).on('click', '.btnaddpurchaseorder', function () {
    var rowCount = $('.tblstock tr').length;
    var flag = false;
    var purchaseid = "";
    var jobid = Purchaseorderjob.JobId;
    var supplier = $(".ddlsupplier").val();
    if (supplier != '0' && supplier != 'undefined' && supplier != "" && supplier != null) {
        $(".errorsupplier").html("");
          if (rowCount > 1) {
            var tabledata = $('.tblstock tr:gt(0)').map(function () {
                return {
                    StockId: $(this.cells[0]).text(),
                    StockLabel: $(this.cells[1]).text(),
                    UnitMeasure: $(this.cells[2]).text(),
                    Quantity: $(this.cells[3]).text(),
                    Price: $(this.cells[4]).text(),
                    Cost: $("#Cost").val(),
                    Supplierid: $(".ddlsupplier").val(),
                    Description: $(".itemdescription").val(),
                    purchaseid: purchaseid,
                    JobId: Purchaseorderjob.JobId,
                };
            }).get();
            $.ajax({
                type: 'POST',
                url: common.SitePath + "/Employee/Invoice/AddPurchaseOrder",
                data: JSON.stringify({ values: tabledata }),
                contentType: 'application/json; charset=utf-8',
                traditional: true,
                async: false,
                success: function (data) {
                    $("#jobpurchaseorder").modal('hide');
                },
                error: function () {
                }
            });
        }
        else {
            alert("Please add purchase item first");
        }
    }

    else {
        $(".errorsupplier").html("Please select Supplier").css('color', 'Red');
    }
})


$(document).on('click', '.btnDeletestock', function () {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure to delete?");
    $(".modal-title").html("Delete purchase order detail !");
    Purchaseorderjob.deletedPrice = $(this).parent().parent().find('td').eq(4).html();
    Rowtodelete = $(this).closest('tr');
});

$(document).on('click', ".btnconfirm", function () {
    var price = Purchaseorderjob.deletedPrice;
    var Cost = $("#Cost").val();
    var newcost = (Cost -= price).toFixed(2);
    $("#Cost").val(newcost);
    //Rowtodelete.remove();
    $(".ddlstock ").val("");
    $("#UnitOfMeasure").val("");
    $(".stockprice").val("");
    $("#Quantity").val("")
    Purchaseorderjob.totalcost = 0;
    Purchaseorderjob.isExisted = false;
    Purchaseorderjob.preaddedstockquantity = 0;
    Purchaseorderjob.RowNotoEdit = 0;
    Purchaseorderjob.maxquantity = 0;
    Purchaseorderjob.CurrentStockPrice = 0;
    $(".commonpopup").modal('hide');
    $(".errormsgdiv").css("display", "block");
    $(".errormsgdiv").html("<strong>Stock item deleted successfully!</strong>");
    $(".errormsgdiv").delay(2000).fadeOut();
})

$(document).on('click', ".btnclear", function () {
    $(".ddlstock ").val("");
    $("#UnitOfMeasure").val("");
    $(".stockprice").val("");
    $("#Quantity").val("")
});

$(document).on('click', ".btnmanageSupportjobinvoice,.btnmanagejobinvoice", function () {
    Purchaseorderjob.JobId = $(this).attr("data-val");
    $('.divjobpurchaseorder').html("");
    $.get(common.SitePath + '/Employee/Invoice/_InvoiceJobPurchaseOrder', function (data) {
        $('.divjobpurchaseorder').html(data);
        $('#jobpurchaseorder').modal('show');
        Purchaseorderjob.GetStockList();
        Purchaseorderjob.GetSupplierList();
        Purchaseorderjob.GetInvoiceDetailToEdit();
    });
    if (FSM.PurchaseOrderID != "") {
        $(".btnaddpurchaseorder").text("Update Purchase Order");
    }
    else {
        $(".btnaddpurchaseorder").text("Add Purchase Order");
    }
})
