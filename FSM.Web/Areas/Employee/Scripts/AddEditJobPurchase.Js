

//******************** global variables**************************************************************************
var globalvar = {
    maxquantity: 0,
    Currentstockprice: "",
    totalcost: 0,
    RowNotoEdit: 0,
    preaddedstockquantity: 0,
    availabledit: 0,
    isExisted: false,
    SourceExport: "",
    isvalidpurchase:"",
  
 CheckISPurchaseavailable :function() {
     //Maximum Purchase check
     
     var purchaseorderid = $("#PurchaseOrderByJobViewModel_ID").val();
  
    var CurrentPurchaseNo = $("#PurchaseOrderByJobViewModel_PurchaseOrderNo").val();
    $.ajax({
        url: common.SitePath + "/Employee/Purchase/CheckPurchaseNumber",
        type: 'GET',
        async:false,
        data: { PurchaseNo: CurrentPurchaseNo },
        success: function (dataSuccess) {
            if (dataSuccess.result != CurrentPurchaseNo && purchaseorderid== "00000000-0000-0000-0000-000000000000") {
                alert("Purchase No " + CurrentPurchaseNo + " is created by some other user. You have assigned a new Purchase No " + dataSuccess.result + ".");
                var purchaseno = dataSuccess.result;
                $("#PurchaseOrderByJobViewModel_PurchaseOrderNo").val(purchaseno);
                globalvar.isvalidpurchase= "false";
            }
            else {
                
                globalvar.isvalidpurchase = "true";
            }

        },
    });
}

}


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


    Label = $(".ddlstockname").val();
    Stockid = "";
    purchaseItem = $(".ddlstockname").val();
    unitMeasure = $("#UnitOfMeasure").val();
    Price = $(".stockprice").val();
    available = $("#available").val();

    ///***************************Add  New Row with Data in purchase order************************************************************
    if (globalvar.RowNotoEdit == 0) {

        var searchrow = SearchTableForExistingStcokLabel(Stockid, Quantity);
        if (!globalvar.isExisted) {
            $("#btnaddstockitems").val("Add Item");
            var row = "<tr>";
            row = row + "<td style='display:none'>" + purchaseItem + "</td> \
                    <td>" + Label + "</td> \
                    <td>" + unitMeasure + "</td> \
                    <td>" + Quantity + "</td> \
                    <td>" + parseFloat(Price).toFixed(2) + "</td> \
                    <td style='display:none'>" + parseInt(available).toFixed(2) + "</td> \
                    <td align='center'><a data-attr='" + purchaseItem + "' price='" + Price + "' class='btn btn-default btnEditstock'>Edit </a></td> \
                   <td align='center'><a price='" + Price + "' data-attr='" + purchaseItem + "' class='btn btn-default btnDeletestock'>Delete </a></td> ";
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
        trtoEdit.children('td')[0].textContent = purchaseItem;
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
    globalvar.maxquantity = 0,
   globalvar.Currentstockprice = "",
  globalvar.totalcost = 0,
  globalvar.RowNotoEdit = 0,
  globalvar.preaddedstockquantity = 0,
 globalvar.availabledit = 0,
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
    globalvar.CheckISPurchaseavailable();
    if (globalvar.isvalidpurchase = "true") {
        var rowCount = $('.tblstock tr').length;
        var flag = false;
        var OrderId = FSM.OrderId;
        jobId = $("#getjobviewmodel_EmployeeJobId").val();
        var supplier = $(".ddlsupplier").val();
        if (supplier != '0' && supplier != 'undefined' && supplier != "" && supplier != null) {
            $(".errorsupplier").html("");
            if (rowCount > 1) {
                var tabledata = $('.tblstock tr:gt(0)').map(function () {
                    return {
                        StockId: "",
                        PurchaseItem: $(this.cells[1]).text(),
                        UnitMeasure: $(this.cells[2]).text(),
                        Quantity: $(this.cells[3]).text(),
                        Price: $(this.cells[4]).text(),
                        ItemId: $(this.cells[6]).text(),
                        Cost: $("#Cost").val(),
                        JobId: jobId,
                        Supplierid: $(".ddlsupplier").val(),
                        Description: $(".description").val(),
                        purchaseid: OrderId,
                    };
                }).get();
                $.ajax({
                    type: 'POST',
                    url: common.SitePath + "/Employee/Purchase/SaveJobPurchaseData",
                    data: JSON.stringify({ values: tabledata }),
                    contentType: 'application/json; charset=utf-8',
                    traditional: true,
                    async: false,
                    success: function (data) {

                        window.location.href = common.SitePath + '/Employee/Purchase/ViewJobsPurchaseOrder';

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
    }
})


//***************************Edit stock data Row from the table ******************************************

$(document).on('click', '.btnEditstock', function () {
    $("#btnaddstockitems").val("Update");
    globalvar.RowNotoEdit = $(this).closest("tr")[0].rowIndex;
    var PurchaseItem = $(this).closest("tr").children('td')[1].textContent;
    var UnitMeasure = $(this).closest("tr").children('td')[2].textContent;
    var Quantity = $(this).closest("tr").children('td')[3].textContent;
    var price = $(this).closest("tr").children('td')[4].textContent;
    var available = $(this).closest("tr").children('td')[5].textContent;
    var ItemId = $(this).closest("tr").children('td')[6].textContent;
    $(".ddlstockname").val(PurchaseItem.toString().toLowerCase());
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
    var delconfirm = confirm("Do you really want to Delete?");
    if (delconfirm) {
        var price = $(this).attr("price");
        var Cost = $("#Cost").val();
        var newcost = (Cost -= price).toFixed(2);
        $("#Cost").val(newcost);
        $(this).closest('tr').remove();
        $(".errormsgdiv").css("display", "block");
        $(".errormsgdiv").html("Stock item deleted successfully").css("color", "Green");
        $(".errormsgdiv").delay(2000).fadeOut();
        $(".ddlstockname ").val("");
        $("#UnitOfMeasure").val("");
        $(".stockprice").val("");
        $("#Quantity").val("")
        globalvar.maxquantity = 0,
  globalvar.Currentstockprice = "",
 globalvar.totalcost = 0,
 globalvar.RowNotoEdit = 0,
 globalvar.preaddedstockquantity = 0,
globalvar.availabledit = 0,
globalvar.isExisted = false;
    }
});
$(document).ready(function () {
    if (FSM.OrderId != null && FSM.OrderId != '0' && FSM.OrderId != "") {
        $(".btnaddpurchaseorder").val("Update Order");

        var _cost, _Supplier = 0, description;
        $.ajax({
            type: 'GET',
            url: common.SitePath + "/Employee/Purchase/GetJobPurchaseItemByOrderId",
            data: { id: FSM.OrderId },
            async: false,
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                for (i = 0; i < data.length; i++) {
                    var row = "<tr>";
                    row = row + "<td style='display:none'>" + response[i]["StockId"] + "</td> \
                    <td>" + response[i]["PurchaseItem"] + "</td> \
                    <td>" + response[i]["UnitMeasure"] + "</td> \
                    <td>" + response[i]["Quantity"] + "</td> \
                    <td>" + parseFloat(response[i]["Price"]).toFixed(2) + "</td> \
                    <td style='display:none'>" + parseFloat(response[i]["AvailableQuatity"]) + "</td> \
                    <td style='display:none'>" + response[i]["ItemId"] + "</td> \
                    <td align='center'><a data-attr='" + response[i]["PurchaseItem"] + "' price='" + response[i]["Price"] + "' class='btn btn-default btnEditstock'>Edit </a></td> \
                    <td align='center'><a  price='" + response[i]["Price"] + "' data-attr='" + response[i]["StockId"] + "' class='btn btn-default btnDeletestock'>Delete </a></td> ";
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

    $.widget("custom.combobox", {
        _create: function () {
            this.wrapper = $("<span>")
              .addClass("custom-combobox")
              .insertAfter(this.element);

            this.element.hide();
            this._createAutocomplete();
            this._createShowAllButton();
        },

        _createAutocomplete: function () {
            var selected = this.element.children(":selected"),
              value = selected.val() ? selected.text() : "";

            this.input = $("<input>")
              .appendTo(this.wrapper)
              .val(value)
              .attr("title", "")
              .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
              .autocomplete({
                  delay: 0,
                  minLength: 0,
                  source: $.proxy(this, "_source")
              })
              .tooltip({
                  classes: {
                      "ui-tooltip": "ui-state-highlight"
                  }
              });

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                },

                autocompletechange: "_removeIfInvalid"
            });
        },

        _createShowAllButton: function () {
            var input = this.input,
              wasOpen = false;

            $("<a>")
              .attr("tabIndex", -1)
              .attr("title", "Show All Items")
              .tooltip()
              .appendTo(this.wrapper)
              .button({
                  icons: {
                      primary: "ui-icon-triangle-1-s"
                  },
                  text: false
              })
              .removeClass("ui-corner-all")
              .addClass("custom-combobox-toggle ui-corner-right")
              .on("mousedown", function () {
                  wasOpen = input.autocomplete("widget").is(":visible");
              })
              .on("click", function () {
                  input.trigger("focus");

                  // Close if already visible
                  if (wasOpen) {
                      return;
                  }

                  // Pass empty string as value to search for, displaying all results
                  input.autocomplete("search", "");
              });
        },

        _source: function (request, response) {
            $("#getjobviewmodel_EmployeeJobId").empty();
            var SearchTerm = request.term;
            var JobDetailsList = [];

            // converting json to array
            var JobDetailsList = jQuery.parseJSON(FSM.JobListVal);
            var selectelmnt = this.element;
            // filtering values based on some value
            //JobDetailsList = jQuery.grep(JobDetailsList, function (item) {
            //    var text = item.Text;
            //    if (text != '' && text != undefined) {
            //        var index = text.toLowerCase().indexOf(request.term.toLowerCase());
            //        return index > -1 ? true : false;
            //    }
            //});

            // taking first twenty values if greater than 10
            //JobDetailsList = JobDetailsList.length > 10 ? JobDetailsList.slice(0, 20) : JobDetailsList;

             // getting values
            $.ajax({
                url: FSM.GetJobList,
                type: 'GET',
                data: { SearchTerm: SearchTerm },
                async: false,
                success: function (result) {
                    JobDetailsList = result.customerData;
                },
                error: function (ex) {
                    alert("something seems wrong!")
                }
            })


            window.setTimeout(function () {
                
                for (var i = 0; i < JobDetailsList.length; i++) {
                    $("#getjobviewmodel_EmployeeJobId").append("<option value='" + JobDetailsList[i].Value + "'>" + JobDetailsList[i].Text + "</option>");
                }

                var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                response(selectelmnt.children("option").map(function () {
                    var text = $(this).text();
                    if (this.value && (!request.term || matcher.test(text)))
                        return {
                            label: text,
                            value: text,
                            option: this
                        };
                }));
            }, 50)

            // binding values again to combo
            //for (var i = 0; i < JobDetailsList.length; i++) {
            //    $("#getjobviewmodel_EmployeeJobId").append("<option value='" + JobDetailsList[i].Value + "'>" + JobDetailsList[i].Text + "</option>");
            //}




            //var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            //response(this.element.children("option").map(function () {
            //    var text = $(this).text();
            //    if (this.value && (!request.term || matcher.test(text)))
            //        return {
            //            label: text,
            //            value: text,
            //            option: this
            //        };
            //}));
        },

        _removeIfInvalid: function (event, ui) {

            // Selected an item, nothing to do
            if (ui.item) {
                return;
            }

            // Search for a match (case-insensitive)
            var value = this.input.val(),
              valueLowerCase = value.toLowerCase(),
              valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });

            // Found a match, nothing to do
            if (valid) {
                return;
            }

            // Remove invalid value
            this.input
              .val("")
              .attr("title", value + " didn't match any item")
              .tooltip("open");
            this.element.val("");
            this._delay(function () {
                this.input.tooltip("close").attr("title", "");
            }, 2500);
            this.input.autocomplete("instance").term = "";
        },

        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        },

    });
    $("#getjobviewmodel_EmployeeJobId").combobox();
    $("#toggle").click(function () {
        $("#getjobviewmodel_EmployeeJobId").toggle();
    });
    $(".custom-combobox-input ").attr("placeholder", "Search Job");
    if (FSM.JobId != '' && FSM.JobId != undefined) {
        $('#getjobviewmodel_EmployeeJobId').val(FSM.JobId);
        $(".custom-combobox-input ").val(FSM.JobNo);
    }

})
$(".btnclear").click(function () {

    $(".ddlstockname ").val("");
    $("#UnitOfMeasure").val("");
    $(".stockprice").val("");
    $("#Quantity").val("")
    $(".errordiv").html("");

})

$(document).on('click', '.change-status-purchase', function (event) {
    event.preventDefault();
    var purchaseOrderId = $(this).attr('data-purchaseid');
    var checkstatus = $('.status-purchase').html();
    var purchasestatus = '';
    if (checkstatus == "[ Approved ]") {
        purchasestatus = "NotApprove";
    }
    else {
        purchasestatus = "Approve";
    }

    var data = { PurchaseOrderId: purchaseOrderId, PurchaseStatus: purchasestatus };
    $.get(FSM.PurchaseStatusUrl, data, function myfunction(result) {
        $('.status-purchase').html(result.purchaseStatus);
    })
    .fail(function () {
        alert("error");
    });
});




$(document).on('click', '.btnPrintExport', function () {
    var rowCount = $('.tblstock tr').length;
    var flag = false;
    var OrderId = FSM.OrderId;
    jobId = $("#getjobviewmodel_EmployeeJobId").val();
    var supplier = $(".ddlsupplier").val();
    if (supplier != '0' && supplier != 'undefined' && supplier != "" && supplier != null) {
        $(".errorsupplier").html("");
        if (rowCount > 1) {
            var tabledata = $('.tblstock tr:gt(0)').map(function () {
                return {
                    StockId: "",
                    PurchaseItem: $(this.cells[1]).text(),
                    UnitMeasure: $(this.cells[2]).text(),
                    Quantity: $(this.cells[3]).text(),
                    Price: $(this.cells[4]).text(),
                    ItemId: $(this.cells[6]).text(),
                    Cost: $("#Cost").val(),
                    JobId: jobId,
                    Supplierid: $(".ddlsupplier").val(),
                    Description: $(".description").val(),
                    purchaseid: OrderId,
                };
            }).get();
            $.ajax({
                type: 'POST',
                url: common.SitePath + "/Employee/Purchase/PurchaseExport",
                data: JSON.stringify({ values: tabledata }),
                contentType: 'application/json; charset=utf-8',
                traditional: true,
                async: false,
                success: function (data) {
                    //window.location.href = 
                    if (globalvar.Export != "Preview") {
                        window.open(common.SitePath + '/Employee/Purchase/PurchaseExport?Purchaseorderid=' + data.TempPurchaseOrderId);
                    }
                    else {
                        globalvar.Export = "";
                        window.open(common.SitePath + '/Employee/Purchase/PurchaseExportPreview?Purchaseorderid=' + data.TempPurchaseOrderId);
                    }
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

$(".btnPreviewExport").click(function () {
    globalvar.Export = "Preview";
    $('.btnPrintExport').click();
});
