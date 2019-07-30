
var InvoiceData = {
    ISValidjclGriddata: true,
    BillingDropDownChange: function () {
        var BillingAddressVal = $("#BillingFileName").val();
        if (BillingAddressVal == "") {
            $("#BillingEmail").val('');
            $("#billingMobile").val('');
            return false;
        }

        $.ajax({
            url: common.SitePath + "/Employee/Invoice/GetBillingDetail",
            data: { BillingAddressId: BillingAddressVal },
            type: 'POST',
            async: false,
            success: function (data) {
                $("#BillingEmail").val(data.EmailId);
                $("#billingMobile").val(data.PhoneNo1);
            },
            error: function (data) {
                alert("something seems wrong!")
            }
        })
    }
}
$(document).ready(function () {
    debugger;
    $(".ddlmultiselect").multiselect();

    $('#DateBooked').datepicker({
        minDate: 0,
        dateFormat: 'dd/mm/yy'
    })
    $('#InvoiceDate').datepicker({
        minDate: 0,
        dateFormat: 'dd/mm/yy'
    })

    $('.edtPayDate').datepicker({
        dateFormat: 'dd/mm/yy',
        startDate: '01/01/1996'
    })
    $('#SupportjobDateBooked').datepicker({
        minDate: 0,
        dateFormat: 'dd/mm/yy'
    })
    var selectedAddress = $("#BillingFileName").val();

    if (selectedAddress != '' && selectedAddress != null && selectedAddress != 'Select') {
        InvoiceData.BillingDropDownChange();
    }

    //Get JCL Item Amount
    var Quantity = 0;
    var sum = 0;
    var price = $(this).parent().parent().find('.jclprice').html()
    var Total = parseFloat(price) * parseFloat(Quantity);
    $(this).parent().parent().find('.jclTotalprice').html(Total);
    $('.jclTotalprice').each(function () {
        sum += parseFloat($(this).html());
    });
    $(".subTotalLbl").text(sum);
    var Gst = parseFloat(parseFloat(sum * 10) / 100);
    $(".lblGst").text(Gst);
    var Gtotal = parseFloat(sum) + parseFloat(Gst);
    $(".TotalWithGSTLbl").text(Gtotal);
    //Get Amount Paid And Show Total Price After AMount Paid Selection
    var amountPaid = $("#amountPaid").val();
    var advancepaid = 0;
    if (amountPaid != '' && amountPaid != null && amountPaid != 'undefined') {
        advancepaid = parseFloat(Gtotal * amountPaid) / 100;
    }
    $(".amountPay").text(advancepaid);
    var balanceDue = (Gtotal) - ((Gtotal * amountPaid) / 100);
    $(".balanceDue").text(balanceDue);
    //balance amount in payment

    if ($("#createInvoiceViewModel_JobType").val() == 'Quote') {
        var MainBalance = $('.TotalWithGSTLbl').html();
        var sum = 0;
        $('.edtPayAmount').each(function () {
            sum += parseFloat($(this).val());
        });
        $(".TotalAmountPAid").text(sum);
        var balanceamt = parseFloat(MainBalance) - parseFloat(sum);
        $(".owingbalanceAmt").text(balanceamt);
    }
    else {
        var MainBalance = $('.TotalWithGSTLbl').html();
        var sum = 0;
        $('.edtPayAmount').each(function () {
            sum += parseFloat($(this).val());
        });
        $(".TotalAmountPAid").text(sum);
        var balanceamt = parseFloat(MainBalance) - parseFloat(sum);
        $(".owingbalanceAmt").text(balanceamt);
    }
    var textarea = document.querySelector('.edtdescription');
    textarea.addEventListener('keydown', autosize);

    $('.edtdescription').each(function () {
        var el = this;

        setTimeout(function () {
            el.style.cssText = 'height:auto; padding:0';
            // for box-sizing other than "content-box" use:
            // el.style.cssText = '-moz-box-sizing:content-box';
            el.style.cssText = 'height:' + el.scrollHeight + 'px';
        }, 0);
    });

    $('.edtdescription').keydown(function () {
        var el = this;

        setTimeout(function () {
            el.style.cssText = 'height:auto; padding:0';
            // for box-sizing other than "content-box" use:
            // el.style.cssText = '-moz-box-sizing:content-box';
            el.style.cssText = 'height:' + el.scrollHeight + 'px';
        }, 0);
    });



});

//$(document).ready(function () {
//    $("#JclMappingViewModel_JCLInfo_0__DefaultQty").keydown(function (e) {
//        // Allow: backspace, delete, tab, escape, enter and .
//        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
//            // Allow: Ctrl/cmd+A
//            (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
//            // Allow: Ctrl/cmd+C
//            (e.keyCode == 67 && (e.ctrlKey === true || e.metaKey === true)) ||
//            // Allow: Ctrl/cmd+X
//            (e.keyCode == 88 && (e.ctrlKey === true || e.metaKey === true)) ||
//            // Allow: home, end, left, right
//            (e.keyCode >= 35 && e.keyCode <= 39)) {
//            // let it happen, don't do anything
//            return;
//        }
//        // Ensure that it is a number and stop the keypress
//        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
//            e.preventDefault();
//        }
//    });
//});

$(document).on('keydown', "#JclMappingViewModel_JCLInfo_0__DefaultQty", function (event) {
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


function autosize() {
    var el = this;

    setTimeout(function () {
        el.style.cssText = 'height:auto; padding:0';
        // for box-sizing other than "content-box" use:
        // el.style.cssText = '-moz-box-sizing:content-box';
        el.style.cssText = 'height:' + el.scrollHeight + 'px';
    }, 0);
}

$('.editquantity').attr("maxlength", "6");

$('.edtprice').attr("maxlength", "6");

$("#PhoneNo").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");

$("#BillingPostalCode").on("keypress keyup blur", function (event) {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});
$("#SitePostalCode").on("keypress keyup blur", function (event) {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});

$(".dltrow").click(function () {
    var Itemamount = $(this).closest('tr').find('.Amount').text();
    var price = $("#createInvoiceViewModel_Price").val();
    var totalPrice = price - Itemamount;
    $("#createInvoiceViewModel_Price").val(totalPrice);
    $(this).closest('tr').remove();

});

$(".dltrowdb").click(function () {
    var Itemamount = $(this).closest('tr').find('.Amount').text();
    $(this).closest('tr').remove();
    var Itemid = $(this).parents('tr').find('input[type="hidden"]').val();
    var price = $("#createInvoiceViewModel_Price").val();
    var totalPrice = price - Itemamount;
    $.ajax({
        url: common.SitePath + "/Employee/Invoice/DeleteinvoiceItem",
        data: { ItemId: Itemid },
        type: 'POST',
        async: false,
        success: function (data) {
            $("#createInvoiceViewModel_Price").val(totalPrice);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});
$("#btnCancelForJob").on("click", function (e) {
    var id = FSM.JobId; // form id
    window.location.href = common.SitePath + '/Employee/CustomerJob/SaveJobInfo?id=' + id + '&activetab=General Info'
});

$(document).on('click', '.change-status-invoice', function (event) {
    event.preventDefault();
    var InvoiceStatusUrl = common.SitePath + "/Employee/Invoice/ChangeInvoiceStatus";
    var invoiceId = $(this).attr('data-invoiceid');
    var checkstatus = $('.status-invoice').html();
    var invoicestatus = '';
    if (checkstatus == "[ Approved ]") {
        invoicestatus = "NotApprove";
    }
    else {
        invoicestatus = "Approve";
    }

    var data = { InvoiceId: invoiceId, InvoiceStatus: invoicestatus };
    $.get(InvoiceStatusUrl, data, function myfunction(result) {
        $('.status-invoice').html(result.invoicestatus);
    })
    .fail(function () {
        alert("error");
    });
});

$(document).on("click", ".saveInvoice", function (event) {
    event.stopImmediatePropagation();
    debugger;
    SaveFormData(1);
    SavePaymentsData(1);
    if (InvoiceData.ISValidjclGriddata) {

        var formdata = new FormData($('#contactform').get(0));
        $.ajax({
            url: $('#contactform').attr("action"),
            type: 'POST',
            data: formdata,
            processData: false,
            contentType: false,
            cache: false,
            success: function (result) {
                if (result.status == "saved") {
                    $('#invoicemsgDv').empty();
                    $(window).scrollTop(0);
                    $('.jobalert').empty();
                    $('.jobalert').css('color', 'green');
                    $('.jobalert').html(result.msg);
                    $('.jobalert').show();
                    $(".tabs li").removeClass("ui-state-disabled");
                    window.setTimeout(function () {
                        $('.jobalert').hide();
                        location.reload();
                    }, 3000)
                }
                else {
                    $(window).scrollTop(0);
                    $('#invoicemsgDv').empty();

                    var ErrorList = "<ul style='list-style:none;'>"
                    $(result.errors).each(function (i) {
                        ErrorList = ErrorList + "<li>" + result.errors[i].ErrorMessage + "</li>";
                    });
                    ErrorList = ErrorList + "</ul>"
                    $(window).scrollTop(0);
                    $('#invoicemsgDv').css('color', 'red');
                    $('#invoicemsgDv').html(ErrorList);
                    $('#invoicemsgDv').show();
                }
            },
            error: function () {
                alert('something went wrong !');
            }
        });
    }
});

$("#BillingFileName").change(function () {


    InvoiceData.BillingDropDownChange();

    //var BillingAddressVal = $("#BillingFileName").val();
    //if (BillingAddressVal == "") {
    //    $("#BillingEmail").val('');
    //    $("#billingMobile").val('');
    //    return false;
    //}

    //$.ajax({
    //    url: common.SitePath + "/Employee/Invoice/GetBillingDetail",
    //    data: { BillingAddressId: BillingAddressVal },
    //    type: 'POST',
    //    async: false,
    //    success: function (data) {
    //        $("#BillingEmail").val(data.EmailId);
    //        $("#billingMobile").val(data.PhoneNo1);
    //    },
    //    error: function (data) {
    //        alert("something seems wrong!")
    //    }
    //})
});

$('.btnEditItembtn').click(function () {
    if ($(this).text() == "Edit") {
        $(this).parent().find('td:eq(3)').html('<input type="text" class="priceVal">');  //replace price label into text box
        $(this).text("Update");  //button name change
    }
    else {
        var SubTotalAMount = $(".subTotal").val();  //Get Total AMount Without GST
        var TotalAMount = $(".TotalWithGST").val(); //Get Total AMount With GST

        var AmountAudVal = $(this).parent().find('.clsAmountAUD').val(); //Get Previous Amount AUD Value 
        var PriceId = $(this).parent().find('.clsItemsPrice').attr('id');  //Price Id
        var AmountAudId = $(this).parent().find('.clsAmountAUD').attr('id');  //Amount Aud Id
        var AmountGstId = $(this).parent().find('.clsAmountGST').attr('id');  //Amount GST Id
        var QuantityVal = $(this).parent().find('.clsQty').val();  // Get Quantity Value
        var editPrice = $(".priceVal").val();    //Get Price text Box Value

        var totalAmount = (QuantityVal) * (editPrice);  //Get total Amount
        var totalAmountGST = (totalAmount) + ((totalAmount * 10) / 100);   //Get Total Amount With GST

        $("#" + PriceId).val(editPrice)  //price value updated
        $("#" + AmountAudId).val(totalAmount)  //price value updated
        $("#" + AmountGstId).val(totalAmountGST)  //price value updated

        $(this).parent().find('td:eq(3)').html('<Label>' + editPrice + '</Label>');  //display in label price value
        $(this).parent().find('td:eq(4)').html('<Label>' + totalAmount + '</Label>');  // display in label total amount value
        $(this).parent().find('td:eq(5)').html('<Label>' + totalAmountGST + '</Label>');  // display in label total amount GST value

        var updateSubTotal = SubTotalAMount - AmountAudVal + totalAmount;  //Get Sub Total Amount After Edit Price
        $('.subTotal').val(updateSubTotal);  //Update SubTotal 
        $(".subTotalLbl").text(updateSubTotal)
        var updateTotalGST = updateSubTotal + (updateSubTotal * 10) / 100;  //Get Total AMount With GST After Edit Price
        $('.TotalWithGST').val(updateTotalGST)   // Update Total 
        $(".TotalWithGSTLbl").text(updateTotalGST)
        $(this).text("Edit");   //button name change
    }
});

$("#addNewLine").click(function () {   //Add color in color grid
    var JobId = $("#empJobId").val();

    $.ajax({
        url: common.SitePath + "/Employee/JCL/GETJCLItemList",
        data: {},
        type: 'Post',
        async: false,
        success: function (data) {
            var jclitem = jQuery.parseJSON(data.JCLList);
            //bind jcl item
            var jclitems = "<select class='tblddlJCL form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select JCL Item</option>";
            for (var i = 0; i < jclitem.length; i++) {
                jclitems = jclitems + '<option value=' + jclitem[i]["Value"] + '>' + jclitem[i]["Text"] + '</option>';
            }
            jclitems = jclitems + "</select>";
            //bind color items
            var coloritems = "<select class='tblddlcolor form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Color</option>";
            coloritems = coloritems + "</select>";
            //bind product items
            var productitem = "<select class='tblddlproduct form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Product</option>";
            productitem = productitem + "</select>";
            //bind size items
            var sizeitems = "<select class='tblddlsize form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Size</option>";
            sizeitems = sizeitems + "</select>";
            var row = "<tr>";
            row = row + "<td class='Mappingid' style='display:none'></td>\
                            <td class='jclid' style='display:none'></td>\
                            <td class='jobid' style='display:none'></td>\
                            <td class='colorid' style='display:none'></td>\
                            <td class='sizeid' style='display:none'></td>\
                             <td class='productid' style='display:none'></td>\
                             <td>" + jclitems + "</td>\
                            <td class='jcldescription'><textarea   class='edtdescription form-control text-box'></textarea></td>\
                            <td >"+ coloritems + "</td>\
                            <td >" + sizeitems + "</td>\
                            <td >"+ productitem + "</td>\
                            <td class='jclquantity'><input type='text' maxlength='6' class='editquantity form-control text-box single-line'></input></td>\
                            <td class='jclprice'><input type='text' maxlength='6' class='edtprice form-control text-box single-line'></input></td>\
                            <td class='jclTotalprice'></td>\
                            <td class='dltrowjcl' style='color:blue'>x</td>";

            $(".InvoiceJCL_table tbody").append(row);

            $('.edtdescription').keydown(function () {
                var el = this;

                setTimeout(function () {
                    el.style.cssText = 'height:auto; padding:0';
                    // for box-sizing other than "content-box" use:
                    // el.style.cssText = '-moz-box-sizing:content-box';
                    el.style.cssText = 'height:' + el.scrollHeight + 'px';
                }, 0);
            });
        },
        error: function () {
            alert("something seems wrong");
        }
    })
});
$(document).on("change", ".tblddlcolor", function () { var parenttr = $(this).parent().parent("tr"); var itemid = $(this).val(); parenttr.find(".colorid").html(itemid); })
$(document).on("change", ".tblddlproduct", function () { var parenttr = $(this).parent().parent("tr"); var itemid = $(this).val(); parenttr.find(".productid").html(itemid); })
$(document).on("change", ".tblddlsize", function () { var parenttr = $(this).parent().parent("tr"); var itemid = $(this).val(); parenttr.find(".sizeid").html(itemid); })


$(document).on("change", ".tblddlJCL", function () {
    var itemid = $(this).val();

    if (itemid == '' || itemid == 'undefined' || itemid == null) {
        return false;
        var parenttr = $(this).parent().parent("tr");
        var coloritems = "<select class='tblddlcolor form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Color</option>";
        coloritems = coloritems + "</select>";
        //bind product items
        var productitem = "<select class='tblddlproduct form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Product</option>";
        productitem = productitem + "</select>";
        //bind size items
        var sizeitems = "<select class='tblddlsize form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Size</option>";
        sizeitems = sizeitems + "</select>";
        $(parenttr).find('td .tblddlcolor').html(coloritems);
        $(parenttr).find('td .tblddlproduct').html(productitem);
        $(parenttr).find('td .tblddlsize').html(sizeitems);
        $(parenttr).find('.editquantity').val(0);
        $(parenttr).find('.edtprice').val(0);
        $(parenttr).find('.edtdescription').val("");
        var sum = 0;
        var Quantity = 0;
        var sum = 0;
        var price = $(this).parent().parent().find('.jclprice').html()
        var Total = parseFloat(price) * parseFloat(Quantity);
        $(this).parent().parent().find('.jclTotalprice').html(Total);
        $('.jclTotalprice').each(function () {
            sum += parseFloat($(this).html());
        });
        $(".subTotalLbl").text(sum);
        var Gst = parseFloat(parseFloat(sum * 10) / 100);
        $(".lblGst").text(Gst);

        var Gtotal = parseFloat(sum) + parseFloat(Gst);
        $(".TotalWithGSTLbl").text(Gtotal);
        $("#createInvoiceViewModel_Price").val(Gtotal);
        //Get Amount Paid And Show Total Price After AMount Paid Selection
        var amountPaid = $("#amountPaid").val();
        var advancepaid = 0;
        if (amountPaid != '' && amountPaid != null && amountPaid != 'undefined') {
            advancepaid = parseFloat(Gtotal * amountPaid) / 100;
        }
        $(".amountPay").text(advancepaid);
        var balanceDue = (Gtotal) - ((Gtotal * amountPaid) / 100);
        $(".balanceDue").text(balanceDue);

        $('.edtdescription').keydown(function () {
            var el = this;

            setTimeout(function () {
                el.style.cssText = 'height:auto; padding:0';
                // for box-sizing other than "content-box" use:
                // el.style.cssText = '-moz-box-sizing:content-box';
                el.style.cssText = 'height:' + el.scrollHeight + 'px';
            }, 0);
        });
    }
    else {
        var parenttr = $(this).parent().parent("tr");
        parenttr.find(".jclid").html(itemid);
        $.ajax({
            url: common.SitePath + "/Employee/Invoice/GetGCLItemDetail",
            data: { JCLId: itemid },
            type: 'GET',
            async: false,
            success: function (data) {

                var colorlist = jQuery.parseJSON(data.Colorlist);
                var Sizelist = jQuery.parseJSON(data.Sizlist);
                var productList = jQuery.parseJSON(data.Productlist);
                var jclitem = jQuery.parseJSON(data.JCLList);
                var JclInfo = jQuery.parseJSON(data.JcLinfo);
                //bind color items
                var coloritems = "<select class='tblddlcolor form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Color</option>";
                for (var i = 0; i < colorlist.length; i++) {

                    coloritems = coloritems + '<option value=' + colorlist[i]["Value"] + '>' + colorlist[i]["Text"] + '</option>';
                }
                coloritems = coloritems + "</select>";
                $(parenttr).find('.tblddlcolor').html(coloritems);
                //bind product items
                var productitem = "<select class='tblddlproduct form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Product</option>";
                for (var i = 0; i < productList.length; i++) {

                    productitem = productitem + '<option value=' + productList[i]["Value"] + '>' + productList[i]["Text"] + '</option>';
                }
                productitem = productitem + "</select>";
                parenttr.find("td .tblddlproduct").html(productitem);
                //bind size items
                var sizeitems = "<select class='tblddlsize form-control Create_Customer' id='invoiceJclItemListViewModel_1__JCLId' ><option value=''>Select Size</option>";
                for (var i = 0; i < Sizelist.length; i++) {

                    sizeitems = sizeitems + '<option value=' + Sizelist[i]["Value"] + '>' + Sizelist[i]["Text"] + '</option>';
                }
                sizeitems = sizeitems + "</select>";
                parenttr.find("td .tblddlsize").html(sizeitems);
                //bind the jcl properties
                for (var i = 0; i < JclInfo.length; i++) {
                    parenttr.find(".jclquantity").html("<input type='text' class='editquantity form-control text-box single-line' value=" + JclInfo[i]["DefaultQty"] + "> </input>");
                    parenttr.find(".jcldescription").html("<textarea  class='edtdescription form-control' value=" + JclInfo[i]["Description"] + " >" + JclInfo[i]["Description"] + " </textarea>");
                    //parenttr.find(".jcldescription").html(JclInfo[i]["Description"]);
                    parenttr.find(".jclprice").html("<input type='text' maxlength='6' class='edtprice form-control text-box single-line' value=" + JclInfo[i]["Price"] + "> </input>");
                    //parenttr.find(".jclprice").html(JclInfo[i]["Price"]);
                    parenttr.find(".jclTotalprice").html(JclInfo[i]["TotalPrice"]);
                }
                //update total
                var sum = 0;
                $('.jclTotalprice').each(function () {
                    sum += parseFloat($(this).html());
                });

                $(".subTotalLbl").text(sum);
                var Gst = parseFloat(parseFloat(sum * 10) / 100);
                $(".lblGst").text(Gst);
                var Gtotal = parseFloat(sum) + parseFloat(Gst);
                $(".TotalWithGSTLbl").text(Gtotal);
                $("#createInvoiceViewModel_Price").val(Gtotal);
                //Get Amount Paid And Show Total Price After AMount Paid Selection
                var amountPaid = $("#amountPaid").val();
                var advancepaid = 0;
                if (amountPaid != '' && amountPaid != null && amountPaid != 'undefined') {
                    advancepaid = parseFloat(Gtotal * amountPaid) / 100;
                }
                $(".amountPay").text(advancepaid);
                var balanceDue = (Gtotal) - ((Gtotal * amountPaid) / 100);
                $(".balanceDue").text(balanceDue);
                $('.edtdescription').keydown(function () {
                    var el = this;

                    setTimeout(function () {
                        el.style.cssText = 'height:auto; padding:0';
                        // for box-sizing other than "content-box" use:
                        // el.style.cssText = '-moz-box-sizing:content-box';
                        el.style.cssText = 'height:' + el.scrollHeight + 'px';
                    }, 0);
                });
            },
            error: function () {
                alert("something seems wrong");
            }
        })
    }

});
$(document).on("change", "#amountPaid", function () {
    //update the totals
    var sum = 0;
    $('.jclTotalprice').each(function () {
        sum += parseFloat($(this).html());
    });
    $(".subTotalLbl").text(sum);
    var Gst = parseFloat(parseFloat(sum * 10) / 100);
    $(".lblGst").text(Gst);
    var Gtotal = parseFloat(sum) + parseFloat(Gst);
    $(".TotalWithGSTLbl").text(Gtotal);
    $("#createInvoiceViewModel_Price").val(Gtotal);
    //Get Amount Paid And Show Total Price After AMount Paid Selection

    var amountPaid = $("#amountPaid").val();
    var advancepaid = 0;
    if (amountPaid != '' && amountPaid != null && amountPaid != 'undefined') {
        advancepaid = parseFloat(Gtotal * amountPaid) / 100;
    }
    $(".amountPay").text(advancepaid);

    var balanceDue = (Gtotal) - ((Gtotal * amountPaid) / 100);
    $(".balanceDue").text(balanceDue);

});

$(document).on("keydown", '.editquantity', function (e) {

    $(this).attr("maxlength", "6");
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl+A, Command+A
        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
        (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
});



$(document).on("keydown", '.edtprice', function (e) {

    $(this).attr("maxlength", "6");
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl+A, Command+A
        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
        (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
});

$(document).on('keyup', '.editquantity', function () {
    $(this).attr("maxlength", "6");

    var Quantity = $(this).val();
    var sum = 0;
    var price = $(this).parent().parent().find('.edtprice').val();
    var Total = parseFloat(price) * parseFloat(Quantity);
    $(this).parent().parent().find('.jclTotalprice').html(Total);
    $('.jclTotalprice').each(function () {
        sum += parseFloat($(this).html());
    });
    $(".subTotalLbl").text(sum);
    var Gst = parseFloat(parseFloat(sum * 10) / 100);
    $(".lblGst").text(Gst);
    var Gtotal = parseFloat(sum) + parseFloat(Gst);
    $(".TotalWithGSTLbl").text(Gtotal);
    $("#createInvoiceViewModel_Price").val(Gtotal);

    //Get Amount Paid And Show Total Price After AMount Paid Selection
    var amountPaid = $("#amountPaid").val();
    var advancepaid = 0;
    if (amountPaid != '' && amountPaid != null && amountPaid != 'undefined') {
        advancepaid = parseFloat(Gtotal * amountPaid) / 100;
    }
    $(".amountPay").text(advancepaid);
    var balanceDue = (Gtotal) - ((Gtotal * amountPaid) / 100);
    $(".balanceDue").text(balanceDue);
})

$(document).on('keyup', '.edtprice', function () {

    $(this).attr("maxlength", "6");
    var price = $(this).val();
    var sum = 0;
    var Quantity = $(this).parent().parent().find('.editquantity').val();
    var Total = parseFloat(price) * parseFloat(Quantity);
    $(this).parent().parent().find('.jclTotalprice').html(Total);
    $('.jclTotalprice').each(function () {
        sum += parseFloat($(this).html());
    });
    $(".subTotalLbl").text(sum);
    var Gst = parseFloat(parseFloat(sum * 10) / 100);
    $(".lblGst").text(Gst);
    var Gtotal = parseFloat(sum) + parseFloat(Gst);
    $(".TotalWithGSTLbl").text(Gtotal);
    $("#createInvoiceViewModel_Price").val(Gtotal);

    //Get Amount Paid And Show Total Price After AMount Paid Selection
    var amountPaid = $("#amountPaid").val();
    var advancepaid = 0;
    if (amountPaid != '' && amountPaid != null && amountPaid != 'undefined') {
        advancepaid = parseFloat(Gtotal * amountPaid) / 100;
    }
    $(".amountPay").text(advancepaid);
    var balanceDue = (Gtotal) - ((Gtotal * amountPaid) / 100);
    $(".balanceDue").text(balanceDue);
})
$(document).on("click", ".dltrowjcl", function () {
    var id = $(this).closest('tr').find("td:first").html();
    var rowIndex = $(this).parent().parent().children().index($(this).parent());

    $(".InvoicePopUp").modal('show');
    $(".alertmsg").text("Are you sure to delete Invoice item?");
    $(".btnconfirm").attr("id", id);
    $(".btnconfirm").attr("rowIndex", rowIndex);
    $(".modal-title").html("Delete Invoice Item!");
});

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    var RowIndex = $(this).attr("rowIndex");
    if (id != null && id != 'undefined' && id != '') {
        $(".InvoiceJCL_table").find("tr:eq(" + RowIndex + ")").remove();
        $.ajax({
            url: common.SitePath + "/Employee/Invoice/DeleteJclITemFromInvoice",
            data: { InvoiceJckMappingid: id },
            type: 'Get',
            success: function (data) {
                $(".InvoicePopUp").modal('hide');
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    }
    else {
        //$(this).closest('tr').remove();
        $(".InvoiceJCL_table").find("tr:eq(" + RowIndex + ")").remove();
        $(".InvoicePopUp").modal('hide');
    }
    //update the totals
    var sum = 0;
    $('.jclTotalprice').each(function () {
        sum += parseFloat($(this).html());
    });
    $(".subTotalLbl").text(sum);
    var Gst = parseFloat(parseFloat(sum * 10) / 100);
    $(".lblGst").text(Gst);
    var Gtotal = parseFloat(sum) + parseFloat(Gst);
    $(".TotalWithGSTLbl").text(Gtotal);
    $("#createInvoiceViewModel_Price").val(Gtotal);

    //Get Amount Paid And Show Total Price After AMount Paid Selection
    var amountPaid = $("#amountPaid").val();
    var advancepaid = 0;
    if (amountPaid != '' && amountPaid != null && amountPaid != 'undefined') {
        advancepaid = parseFloat(Gtotal * amountPaid) / 100;
    }
    $(".amountPay").text(advancepaid);
    var balanceDue = (Gtotal) - ((Gtotal * amountPaid) / 100);
    $(".balanceDue").text(balanceDue);


    var length = $(".InvoiceJCL_table tr").length;
    if (parseInt(length) > 0) {
        $(".AmountTotalDv").css("display", "block");
    }
    else {
        $(".AmountTotalDv").css("display", "none");
    }
})

$(".btnsubmit").click(function () {
    debugger;
    SaveFormData(0);
    
        SavePaymentsData(0);
    
})

function SaveFormData(index)//if 0 from create otherwise editinvoice
{

    var tabledata = null;
    var itemcount = $('.InvoiceJCL_table tr').length;

    if (itemcount > 1) {
        var ISvalidjcl = true, ISvalidcolor = true, ISvalidproduct = true, ISvalidsize = true;
        $(".InvoiceJCL_table tr").each(function (index) {
            if (index != 0) {
                var Id = $(this).find("td:first").html();

                var JobId = $('#createInvoiceViewModel_EmployeeJobId').val();
                var GCLID = $(this.cells[1]).text();
                if (GCLID == '' || GCLID == 'undefined' || GCLID == 'undefined') {
                    $(".commonpopup").modal('show');
                    $(".alertmsg").html("<b>please Select Item </b>");
                    $(".bottom_btn").hide();
                    $('.modal-title').text("Select Item");
                    $(".commonpopup").modal('show');
                    ISvalidjcl = false;
                }
                //{ alert('Please select JCL  Item'); ISvalidjcl = false; }
                var Description = $(this).find('.edtdescription').val();
                var Colorid = $(this.cells[3]).text();
                if (Colorid == '' || Colorid == 'undefined' || Colorid == 'undefined')
                    //{ alert('Please select Color Item'); ISvalidcolor = false; }
                    var Sizeid = $(this.cells[4]).text();
                if (Sizeid == '' || Sizeid == 'undefined' || Sizeid == 'undefined')
                    //{ alert('Please select Sizeid Item'); ISvalidsize = false; }
                    var Productid = $(this.cells[5]).text();
                if (Productid == '' || Productid == 'undefined' || Productid == 'undefined')
                    //{ alert('Please select Product size Item'); ISvalidproduct = false; }
                    var DefaultQty = $(this).find('.editquantity').val();
                var Price = $(this).find('.edtprice').val();
                var TotalPrice = $(this.cells[13]).text();
            }
        });


        if (ISvalidjcl) {

            tabledata = $('.InvoiceJCL_table tr:gt(0)').map(function () {
                return {
                    Id: $(this).find("td:first").html(),
                    JobId: $('#createInvoiceViewModel_EmployeeJobId').val(),
                    GCLID: $(this.cells[1]).text(),
                    Description: $(this).find('.edtdescription').val(),
                    Colorid: $(this.cells[3]).text(),
                    Sizeid: $(this.cells[4]).text(),
                    Productid: $(this.cells[5]).text(),
                    DefaultQty: $(this).find('.editquantity').val(),
                    Price: $(this).find('.edtprice').val(),
                    TotalPrice: $(this.cells[13]).text(),
                };
            }).get();
            $.ajax({
                type: 'POST',
                url: common.SitePath + "/Employee/Invoice/InsertJclITemForInvoice",
                data: JSON.stringify({ jclitems: tabledata }),
                contentType: 'application/json; charset=utf-8',
                traditional: true,
                async: false,
                success: function (data) {

                },
            });
            if (index == 0)
                //  $("#contactform").submit();
                InvoiceData.ISValidjclGriddata = true;

        }
        else {
            InvoiceData.ISValidjclGriddata = false;
            return false;
        }
    }
    //else if (index == 0)
    //    $("#contactform").submit();
    // $("#formId").submit();
}


$("#addNewPayment").click(function () {
    var rowcount = $('.InvoicePayment_table tr').length;
    var PaymentAmount = '', PaymentDate = '';
    if (parseInt(rowcount) > 0) {
        var lastrow = $('.InvoicePayment_table tr:last');
        var PaymentDate = $(lastrow).find('.edtPayDate').val();
        var PaymentAmount = $(lastrow).find('.edtPayAmount').val();
        var PaymentMethod = $(lastrow).find('.paymentMethod').val();
        var Reference = $(lastrow).find('.edtPayDesc').val();
        var PaymentNote = $(lastrow).find('.edtPaYNote').val();
        if (parseInt(rowcount) > 1) {
            if (PaymentDate == 'undefined' || PaymentDate == '' || PaymentDate == null) {

                alert("Please Select Payment Date.")
                return;
            }
            else if (PaymentAmount == 'undefined' || PaymentAmount == null || PaymentAmount == '') {
                alert("Please Enter Payment Amount.")
                return;
            }
            else {

                $.ajax({
                    url: common.SitePath + "/Employee/Invoice/GETPaymentMethod",
                    data: {},
                    type: 'Post',
                    async: false,
                    success: function (data) {
                        var paymentMethodList = jQuery.parseJSON(data.data);
                        //bind jcl item
                        var paymentMethod = "<select class='form-control paymentMethod' ><option value=''>Select</option>";
                        for (var i = 0; i < paymentMethodList.length; i++) {
                            paymentMethod = paymentMethod + '<option value=' + paymentMethodList[i]["Value"] + '>' + paymentMethodList[i]["Key"] + '</option>';
                        }
                        paymentMethod = paymentMethod + "</select>";
                        var row = "<tr>";
                        row = row + "<td class='paymentId' style='display:none'></td>\
                            <td class='paymentInvoiceId' style='display:none'></td>\
                            <td class='paymentDate'><input type='text'  class='edtPayDate form-control text-box single-line'></input></td>\
                            <td class='paymentAmount'><input type='text'  class='edtPayAmount form-control text-box single-line'></input></td>\
                             <td>" + paymentMethod + "</td>\
                            <td class='paymentReference'><input type='text'  class='edtPayDesc form-control text-box single-line'></input></td>\
                            <td class='paymentNote'><input type='text'  class='edtPaYNote form-control text-box single-line'></input></td>\
                            <td class='dlttblRowPayment' style='color:blue'>x</td>";
                        $(".InvoicePayment_table tbody").append(row);
                        $('.edtPayDate').datepicker({
                            dateFormat: 'dd/mm/yy',
                            startDate: '01/01/1996'
                        })
                    },
                    error: function () {
                        alert("something seems wrong");
                    }
                });
            }
        }
        else {

            $.ajax({
                url: common.SitePath + "/Employee/Invoice/GETPaymentMethod",
                data: {},
                type: 'Post',
                async: false,
                success: function (data) {
                    var paymentMethodList = jQuery.parseJSON(data.data);
                    //bind jcl item
                    var paymentMethod = "<select class='form-control paymentMethod' ><option value=''>Select</option>";
                    for (var i = 0; i < paymentMethodList.length; i++) {
                        paymentMethod = paymentMethod + '<option value=' + paymentMethodList[i]["Value"] + '>' + paymentMethodList[i]["Key"] + '</option>';
                    }
                    paymentMethod = paymentMethod + "</select>";
                    var row = "<tr>";
                    row = row + "<td class='paymentId' style='display:none'></td>\
                            <td class='paymentInvoiceId' style='display:none'></td>\
                            <td class='paymentDate'><input type='text'  class='edtPayDate form-control text-box single-line'></input></td>\
                            <td class='paymentAmount'><input type='text'  class='edtPayAmount form-control text-box single-line'></input></td>\
                             <td>" + paymentMethod + "</td>\
                            <td class='paymentReference'><input type='text'  class='edtPayDesc form-control text-box single-line'></input></td>\
                            <td class='paymentNote'><input type='text' class='edtPaYNote form-control text-box single-line'></input></td>\
                            <td class='dlttblRowPayment' style='color:blue'>x</td>";
                    $(".InvoicePayment_table tbody").append(row);
                    $('.edtPayDate').datepicker({
                        minDate: 0,
                        dateFormat: 'dd/mm/yy'
                    })
                },
                error: function () {
                    alert("something seems wrong");
                }
            });
        }
    }

});


$(document).on("click", ".dlttblRowPayment", function () {
    debugger;
    var id = $(this).closest('tr').find("td:first").html();
    var confirmdelete = confirm('Are you sure to delete payment detail?');
    if (confirmdelete) {
        if (id == null || id == 'undefined' || id == '') {
            var id = $(this).closest('tr').find("td:first");
            $(this).closest('tr').remove();
            return;
        }
        else {
            id = $(this).closest('tr').find("td:first").html(); //value of invoice id
        }

        if (id != null && id != 'undefined' && id != '') {
            $(this).closest('tr').remove();
            //calculate Owing Amount

            var MainBalance = "";
            if ($("#createInvoiceViewModel_JobType").val() == 'Quote') {
                MainBalance = $('.TotalWithGSTLbl').html();
            }
            else {
                MainBalance = $('.TotalWithGSTLbl').html();
            }
            var sum = 0;
            $('.edtPayAmount').each(function () {
                var Value = $(this).val();
                if (Value == '' || Value == 'NaN' || Value == 'undefined' || Value == null)
                    Value = parseFloat(0);
                else
                    Value = parseFloat($(this).val());
                sum += parseFloat(Value);
            });
            $(".TotalAmountPAid").text(sum);
            var balanceamt = parseFloat(MainBalance) - parseFloat(sum);
            $(".owingbalanceAmt").text(balanceamt);
            //

            $.ajax({
                url: common.SitePath + "/Employee/Invoice/DeletePaymentFromInvoice",
                data: { InvoicePaymentid: id },
                type: 'Get',
                success: function (data) {

                },
                error: function () {
                    alert("something seems wrong");
                }
            });
        }
        else {

            // $(this).closest('tr').remove();

        }
    }
});


function SavePaymentsData(index)//if 0 from create otherwise editinvoice
{
    debugger;
    var tabledata = null;
    var itemcount = $('.InvoicePayment_table tr').length;
    var ISvalidjcl = true;
    if (itemcount > 1) {
        var ISvalidcolor = true, ISvalidproduct = true, ISvalidsize = true;
        ISvalidjcl = true;
        $(".InvoicePayment_table tr").each(function (index) {
            if (index != 0) {
                var Id = $(this).find("td:first").html();

                var InvoiceId = $(this.cells[1]).text();
                //{ alert('Please select JCL  Item'); ISvalidjcl = false; }
                var Date = $(this).find('.edtPayDate').val();

                var Amount = $(this).find('.edtPayAmount').val();


                var method = $(this.cells[4]).text();
                var PaymentMethod = $(this).find('.paymentMethod').val();
                if (method == '' || method == 'undefined' || method == 'undefined')
                    //{ alert('Please select Color Item'); ISvalidcolor = false; }
                    var Reference = $(this).find('.edtPayDesc').val();
                var Note = $(this).find('.edtPaYNote').val();
            }
        });


        if (ISvalidjcl) {

            tabledata = $('.InvoicePayment_table tr:gt(0)').map(function () {
                return {

                    Id: $(this).find("td:first").html(),
                    InvoiceId: $('#InvoiceIdPayment').val(),
                    PaymentDate: $(this).find('.edtPayDate').val(),
                    PaymentAmount: $(this).find('.edtPayAmount').val(),
                    PaymentMethod: $(this).find('.paymentMethod').val(),
                    Reference: $(this).find('.edtPayDesc').val(),
                    PaymentNote: $(this).find('.edtPaYNote').val(),
                };
            }).get();
            $.ajax({
                type: 'POST',
                url: common.SitePath + "/Employee/Invoice/InsertPaymentForInvoice",
                data: JSON.stringify({ PaymentList: tabledata }),
                contentType: 'application/json; charset=utf-8',
                traditional: true,
                async: false,
                success: function (data) {

                },
            });
            if (index == 0)
                $("#contactform").submit();
            InvoiceData.ISValidjclGriddata = true;

        }
        else {
            InvoiceData.ISValidjclGriddata = false;
        }
        //else {
        //    InvoiceData.ISValidjclGriddata = false;
        //    return false;
        //}
    }
    else if (index == 0)
        $("#contactform").submit();
    // $("#formId").submit();
}

//Calculating Owing Balance

$(document).on('keyup', '.edtPayAmount', function () {
    var MainBalance = '';
    if ($("#createInvoiceViewModel_JobType").val() == 'Quote') {
        MainBalance = $('.TotalWithGSTLbl').html();
    }
    else {
        MainBalance = $('.TotalWithGSTLbl').html();
    }
    var amount = $(this).val();
    var sum = 0;
    $('.edtPayAmount').each(function () {
        var Value = $(this).val();
        if (Value == '' || Value == 'NaN' || Value == 'undefined' || Value == null)
            Value = parseFloat(0);
        else
            Value = parseFloat($(this).val());
        sum += parseFloat(Value);
    });
    $(".TotalAmountPAid").text(sum);
    var balanceamt = parseFloat(MainBalance) - parseFloat(sum);
    $(".owingbalanceAmt").text(balanceamt);
});