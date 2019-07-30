

//******************** global variables**************************************************************************
var globalvar = {
    maxquantity: 0,
    Currentstockprice: "",
    totalcost: 0,
    RowNotoEdit: 0,
    preaddedstockquantity: 0,
    availabledit: 0,
    isExisted: false,
    deletedPrice: 0,
}
var Rowtodelete;

///**************************************************Btn Add Stock for Purchase functionality***********************************
$(document).on('click', '#btnaddstockitems', function () {
    var flag = 0;
    var Quantity, Price, unitMeasure, Label, ID, mode, available;
    var stock = $(".ddlstockname").val();
    Quantity = $("#Quantity").val();
    if (Quantity == '0' || Quantity == 'undefined' || Quantity == "") {
        var flag = 1;
    }
    if (stock == '0' || stock == '' || stock == 'undefined') {
        flag = 1;
    }
    if (flag == 1) {
        $(".errordiv").css("display", "block");
        $(".errordiv").html("Please enter purchase item and provide quantity between 1 to 10000").css("color", "Red");
        //$(".errormsgdiv").delay(2000).fadeOut();
        return false;

    }


    Label = $(".ddlstockname option:selected").text();
    Stockid = $(".ddlstockname").val();
    unitMeasure = $("#UnitOfMeasure").val();
    Price = $(".stockprice").val();
    available = $("#available").val();

    ///***************************Add  New Row with Data in purchase order************************************************************
    if (globalvar.RowNotoEdit == 0) {

        var searchrow = SearchTableForExistingStcokLabel(Stockid, Quantity);
        if (!globalvar.isExisted) {
            $("#btnaddstockitems").val("Add Item");
            var row = "<tr>";
            row = row + "<td style='display:none'>" + Stockid + "</td> \
                    <td>" + Label + "</td> \
                    <td>" + unitMeasure + "</td> \
                    <td>" + Quantity + "</td> \
                    <td>" + parseFloat(Price).toFixed(2) + "</td> \
                    <td style='display:none'>" + parseInt(available).toFixed(2) + "</td> \
                    <td><a data-attr='" + Stockid + "' price='" + Price + "' class='btn btn-default btnEditstock'>Edit </a> | <a price='" + Price + "' data-attr='" + Stockid + "' class='btn btn-default btnDeletestock'>Delete </a></td> ";
            $(".tblstock tbody").append(row);
            globalvar.totalcost = parseFloat(globalvar.totalcost) + parseFloat(Price);
            $("#Cost").val(globalvar.totalcost.toFixed(2));
            $(".ddlstockname ").val("");
            $("#UnitOfMeasure").val("");
            $(".stockprice").val("");
            $("#Quantity").val("")
            globalvar.preaddedstockquantity = 0;
        }

    }

        ///***************************Add  New Row with Data in purchase order************************************************************

    else {
        var trtoEdit = $('.tblstock tr').eq(globalvar.RowNotoEdit);
        trtoEdit.children('td')[0].textContent = Stockid;
        trtoEdit.children('td')[1].textContent = Label;
        trtoEdit.children('td')[2].textContent = unitMeasure;
        trtoEdit.children('td')[3].textContent = Quantity;
        trtoEdit.children('td')[4].textContent = Price;
        globalvar.RowNotoEdit = 0;
        globalvar.isExisted = false;

    }
    var tablecost = 0;
    $(".tblstock tr").each(function (index) {
        if (index != 0) {
            var cost = $(this).find("td")[4].textContent
            tablecost = tablecost + parseFloat(cost);
            globalvar.totalcost = tablecost;
        }
    });
    $("#Cost").val(globalvar.totalcost);
    $(".ddlstockname ").val("");
    $("#UnitOfMeasure").val("");
    $(".stockprice").val("");
    $("#Quantity").val("");
    $("#btnaddstockitems").val("Add Item");
    globalvar.maxquantity = 0;
    globalvar.Currentstockprice = "";
    globalvar.totalcost = 0;
    globalvar.RowNotoEdit = 0;
    globalvar.preaddedstockquantity = 0;
    globalvar.availabledit = 0;
    globalvar.isExisted = false;
})

//****************************Get Stock info by Stockid on Dropdown change*********************************
//$(document).on('change', ".ddlstockname", function () {
//    var stockid = $(this).val();
//    $("#btnaddstockitems").val("Add Item");
//    $(".errordiv").html("");
//    if (stockid != "") {
//        SearchTableForStockQuantity(stockid);
//        $.ajax({
//            type: 'GET',
//            url: common.SitePath + "/Employee/Stock/GetStockByStockid",
//            data: { id: stockid },
//            async: false,
//            success: function (data) {
//                var res = JSON.parse(data);
//                $("#UnitOfMeasure").val(res.UnitMeasure);
//                $("#maxunits").val(res.Quantity);
//                $("#Price").val(res.Price);
//                $("#available").val(res.Available);
//                $("#Quantity").val("");
//                globalvar.Currentstockprice = res.Price;
//            },
//            error: function () {
//                alert("something seems wrong");
//            }
//        });
//    }
//});

//*********************************************** Quantity Change functionality ****************************************************
$(document).on('change', "#Quantity", function () {
    var quatity = $(this).val();
    if (parseInt(quatity) >= parseInt(1) && parseInt(quatity) <= parseInt(10000)) {
        var newprice = parseInt(quatity) * globalvar.Currentstockprice;;
        $("#Price").val(newprice);
        $(".errordiv").html("");
    }
    else {
        $(this).val("");
        $(".errordiv").html("Quantity must be between 1 and 10000").css("color", "Red");
    }

})
//  ****************** Quantity (Only Integer allowed functionality) ************************************************
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
    //if a decimal has been added, disable the "."-button

});


//********************************************Saving Purchase Data functionality *****************************

$(document).on('click', '.btnaddpurchaseorder', function () {
    var rowCount = $('.tblstock tr').length;
    var flag = false;
    var OrderId = FSM.OrderId;
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
                    Description: $(".description").val(),
                    purchaseid: OrderId,
                };
            }).get();
            $.ajax({
                type: 'POST',
                url: common.SitePath + "/Employee/Purchase/SavePurchaseData",
                data: JSON.stringify({ values: tabledata }),
                contentType: 'application/json; charset=utf-8',
                traditional: true,
                async: false,
                success: function (data) {
                    window.location.href = common.SitePath + '/Employee/Purchase/ViewPurchaseOrder';
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

//***************************Edit stock data Row from the table ******************************************

$(document).on('click', '.btnEditstock', function () {
    $("#btnaddstockitems").val("Update");
    globalvar.RowNotoEdit = $(this).closest("tr")[0].rowIndex;
    var Stockid = $(this).closest("tr").children('td')[0].textContent;
    var UnitMeasure = $(this).closest("tr").children('td')[2].textContent;
    var Quantity = $(this).closest("tr").children('td')[3].textContent;
    var price = $(this).closest("tr").children('td')[4].textContent;
    var available = $(this).closest("tr").children('td')[5].textContent;
    $(".ddlstockname").val(Stockid.toString().toLowerCase());
    $("#UnitOfMeasure").val(UnitMeasure);
    $("#Quantity").val(Quantity);
    $("#Price").val(price);
    globalvar.preaddedstockquantity = 0;
    globalvar.Currentstockprice = parseFloat(price) / parseInt(Quantity);
});


function SearchTableForExistingStcokLabel(stockid, Quantity) {
    $(".tblstock tr").each(function (index) {
        if (index != 0) {
            $row = $(this);
            var id = $row.find("td:first").text();
            if (id == stockid) {
                var quantity = $row.find("td")[3].textContent
                var price = $row.find("td")[4].textContent
                var oneprice = price / quantity;
                globalvar.totalcost = parseFloat(globalvar.totalcost) - (parseFloat(oneprice) * parseInt(quantity));
                var totalquantity = parseInt(quantity) + parseInt(Quantity);
                $row.children('td')[3].textContent = totalquantity;
                $row.children('td')[4].textContent = parseFloat(oneprice) * parseInt(totalquantity);
                globalvar.totalcost = parseFloat(globalvar.totalcost) + parseFloat(oneprice) * parseInt(totalquantity);
                $("#Cost").val(globalvar.totalcost.toFixed(2));
                globalvar.isExisted = true;
                $(".ddlstockname ").val("");
                $("#UnitOfMeasure").val("");
                $(".stockprice").val("");
                $("#Quantity").val("")
            }
        }
    })
}

function SearchTableForStockQuantity(stockid) {
    $(".tblstock tr").each(function (index) {
        if (index != 0) {
            $row = $(this);
            var id = $row.find("td:first").text();
            if (id == stockid) {
                globalvar.preaddedstockquantity = globalvar.preaddedstockquantity + parseInt($row.find("td")[3].textContent);
            }
        }
    })
}




//***************************Delete stock data Row from the table ******************************************
$(document).on('click', '.btnDeletestock', function () {
    $(".commonpopup").modal('show');
    $(".alertmsg").text("Are you sure to delete?");
    $(".modal-title").html("Delete purchase order detail !");
    globalvar.deletedPrice = $(this).parent().parent().find('td').eq(4).html();
    Rowtodelete = $(this).closest('tr');
});

$(document).on('click', ".btnconfirm", function () {
    var price = globalvar.deletedPrice;
    var Cost = $("#Cost").val();
    var newcost = (Cost -= price).toFixed(2);
    $("#Cost").val(newcost);
    Rowtodelete.remove();
    $(".ddlstockname ").val("");
    $("#UnitOfMeasure").val("");
    $(".stockprice").val("");
    $("#Quantity").val("");
    globalvar.maxquantity = 0;
    globalvar.Currentstockprice = "";
    globalvar.totalcost = 0;
    globalvar.RowNotoEdit = 0;
    globalvar.preaddedstockquantity = 0;
    globalvar.availabledit = 0;
    globalvar.isExisted = false;
    $(".commonpopup").modal('hide');
    $(".errormsgdiv").css("display", "block");
    $(".errormsgdiv").html("<strong>Stock item deleted successfully!</strong>");
    $(".errormsgdiv").delay(2000).fadeOut();
})
$(document).ready(function () {
    if (FSM.OrderId != null && FSM.OrderId != '0' && FSM.OrderId != "") {
        $(".btnaddpurchaseorder").val("Update Order");

        var _cost, _Supplier = 0, description;
        $.ajax({
            type: 'GET',
            url: common.SitePath + "/Employee/Purchase/GetPurchaseItemByOrderId",
            data: { id: FSM.OrderId },
            async: false,
            success: function (data) {
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
                $(".ddlsupplier").val(_Supplier.toString().toLowerCase())
                $(".description").val(description);
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    }
})
$(".btnclear").click(function () {

    $(".ddlstockname ").val("");
    $("#UnitOfMeasure").val("");
    $(".stockprice").val("");
    $("#Quantity").val("")
    $(".errordiv").html("");

})