$(document).ready(function () {
  
  //  $('#ApplyBonus').prop('checked', true);

    //$('#AddColor').click(function () {   //show Color grid
    //    $('.color_collapse').toggle();
    //});

    //$('#AddSize').click(function () {    //show Size grid
    //    $('.size_collapse').toggle();
    //});

    //$('#AddProducts').click(function () {  //show Product grid
    //    $('.product_collapse').toggle();
    //});

    $("#AddColorGrid").click(function () {   //Add color in color grid
        var txtColor = $("#txtColor").val();
        if (txtColor != "") {
            var row = "<tr>";
            row = row + "<td style='display:none' class='colrID'></td> \
                    <td style='display:none'></td> \
                    <td>" + txtColor + "</td> \
                    <td style='width: 20%;'><button type='button' id='dltcolorRow' style='margin: 0px' class='btn btn-default btn-sm'>\
                    <span class='glyphicon glyphicon-remove-sign'></span> \
                     </button></td>"
            $(".color_files tbody").append(row);
            $("#txtColor").val("")
        }
    });

    $("#AddSizeGrid").click(function () {//Add size in  grid
        var txtSize = $("#txtSize").val();
        if (txtSize != "") {
            var row = "<tr>";
            row = row + "<td style='display:none'></td> \
                    <td style='display:none'></td> \
                    <td>" + txtSize + "</td> \
                    <td style='width: 20%;'><button type='button' id='ditsizeRow' style='margin: 0px' class='btn btn-default btn-sm'>\
                    <span class='glyphicon glyphicon-remove-sign'></span> \
                     </button></td>"
            $(".size-files tbody").append(row);
            $("#txtSize").val("")
        }
    });

    $("#AddProductsGrid").click(function () {   //Add size in size grid
        var txtProducts = $("#txtProducts").val();
        if (txtProducts != "") {
            var row = "<tr>";
            row = row + "<td style='display:none'></td> \
                   <td style='display:none'></td> \
                    <td>" + txtProducts + "</td> \
 <td style='width: 20%;'><button type='button' id='dltproductRow' style='margin: 0px' class='btn btn-default btn-sm'>\
                    <span class='glyphicon glyphicon-remove-sign'></span> \
                     </button></td>"

            $(".products_files tbody").append(row);
            $("#txtProducts").val("")
        }
    });

    $("#AddExtraGrid").click(function () {   //Add size in size grid
        var txtProducts = $("#txtextras").val();
        if (txtProducts != "") {
            var row = "<tr>";
            row = row + "<td style='display:none'></td> \
                   <td style='display:none'></td> \
                    <td>" + txtProducts + "</td> \
 <td style='width: 20%;'><button type='button' id='dltextaproductRow' style='margin: 0px' class='btn btn-default btn-sm'>\
                    <span class='glyphicon glyphicon-remove-sign'></span> \
                     </button></td>"

            $(".Extras_files tbody").append(row);
            $("#txtProducts").val("")
        }
    });


    var textarea = document.querySelector('.edtdescription');
    textarea.addEventListener('keydown', autosize);
   
    $('.edtdescription').keydown(function () {
        var el = this;

        setTimeout(function () {
            el.style.cssText = 'height:auto; padding:0';
            // for box-sizing other than "content-box" use:
            // el.style.cssText = '-moz-box-sizing:content-box';
            el.style.cssText = 'height:' + el.scrollHeight + 'px';
        }, 0);
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
    $('.edtdescription').trigger('keydown');
   
});


$(document).on("click", "#dltextaproductRow", function () {
    var id = $(this).closest('tr').find("td:first").html();
    if (id != null && id != 'undefined' && id != '') {
        $(this).closest('tr').remove();
        CommonDelete(id, 'jclExtraproduct');
    }
    else {
        $(this).closest('tr').remove();
    }
});

$(document).on("click", "#dltproductRow", function () {
    var id = $(this).closest('tr').find("td:first").html();
    if (id != null && id != 'undefined' && id != '') {
        $(this).closest('tr').remove();
        CommonDelete(id, 'jclproduct');
    }
    else {
        $(this).closest('tr').remove();
    }
});
$(document).on("click", "#ditsizeRow", function () {
    var id = $(this).closest('tr').find("td:first").html();

    if (id != null && id != 'undefined' && id != '') {
        $(this).closest('tr').remove();
        CommonDelete(id, 'jclsize');
    }
    else {
        $(this).closest('tr').remove();
    }
});
$(document).on("click", "#dltcolorRow", function () {
    var id = $(this).closest('tr').find("td:first").html();

    if (id != null && id != 'undefined' && id != '') {
        $(this).closest('tr').remove();
        CommonDelete(id, 'jclcolor');
    }
    else {
        $(this).closest('tr').remove();
    }

});

function CommonDelete(id, type) {
    $.ajax({
        url: common.SitePath + "/Employee/JCL/DeleteJCLITems",
        data: { ItemID: id, Type: type },
        type: 'Get',
        success: function (data) {
            //$('.assign-job-message').css('color', 'green');
            //$('.assign-job-message').html("Updated succesfully");
            //$('.assign-job-message').show();
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

$(document).on("click", ".saveJCL", function () {
    
    var JCLID = $("#jclID").val();
    var itemName = $("#ItemName").val();
    var price = $("#Price").val();;

    if (itemName == '' || itemName == 'undefined' || itemName == null) {
        $("#ItemError").text("*Item Name is required");
        return false;
    }
    if (price == '' || price == 'undefined' || price == null) {
        $("#PriceError").text("*Price is required");
        return false;
    }
    
    var category = $("#Category").val();
    var material = $("#Material").val();
    var quantity = $("#DefaultQty").val();
    var price = $("#Price").val();
    var description = $("#Description").val();
    var Applybonus = $("#Applybonus").is(":checked");
    var fdata = new FormData();
    if (JCLID != undefined) {
        fdata.append("JCLId", JCLID);
    }
    fdata.append("ItemName", itemName);
    fdata.append("category", category);
    fdata.append("material", material);
    fdata.append("DefaultQty", quantity)
    fdata.append("price", price)
    fdata.append("description", description);
    fdata.append("Applybonus", Applybonus);
    var URL;
    if (JCLID != null && JCLID != "" && JCLID != undefined) {
        URL = common.SitePath + "/Employee/JCL/EditJCL"
    }
    else {
        URL = common.SitePath + "/Employee/JCL/AddJCL"
    }
    $.ajax({
        type: 'POST',
        url: URL,
        processData: false,
        contentType: false,
        dataType: "json",
        data: fdata,
        success: function (data) {
            var Jclid = data.data;
            var colorCount = $('.color_files tr').length;
            var tabledata = null;
            var tabledataproduct = null;
            var tabledataSize = null;
            var tableExtradataproduct=null;
            if (colorCount > 1) {

                tabledata = $('.color_files tr:gt(0)').map(function () {
                    return {
                        ColorId: $(this).find("td:first").html(),
                        JCLId: Jclid,
                        ColorName: $(this.cells[2]).text(),
                    };
                }).get();

            }
            colorCount = $('.size-files tr').length;
            if (colorCount > 1) {
                tabledataSize = $('.size-files tr:gt(0)').map(function () {
                    return {
                        SizeId: $(this).find("td:first").html(),
                        JCLId: Jclid,
                        Size: $(this.cells[2]).text(),
                    };
                }).get();
            }
            var rowCount = $('.products_files tr').length;
            if (rowCount > 1) {
                tabledataproduct = $('.products_files tr:gt(0)').map(function () {
                    return {
                        ProductId: $(this).find("td:first").html(),
                        JCLId: Jclid,
                        ProductName: $(this.cells[2]).text(),
                    };
                }).get();
            }
            
            var rowCount = $('.Extras_files tr').length;
            if (rowCount > 1) {
                tableExtradataproduct = $('.Extras_files tr:gt(0)').map(function () {
                    return {
                        ProductId: $(this).find("td:first").html(),
                        JCLId: Jclid,
                        ProductName: $(this.cells[2]).text(),
                    };
                }).get();
            }
            
            $.ajax({
                type: 'POST',
                url: common.SitePath + "/Employee/JCL/AddJCLItems",
                data: JSON.stringify({ JClColorList: tabledata, JCLSizeList: tabledataSize, JCLProductsList: tabledataproduct, JclExtrproductList: tableExtradataproduct }),
                contentType: 'application/json; charset=utf-8',
                traditional: true,
                async: true,
                success: function (data) {
                    window.location.href = common.SitePath + '/Employee/JCL/GETJCLList';
                },
            });
        }
        ,
        error: function () {
        },
        complete: function (data) {
        }
    });
});











//    var itemName = $("#ItemName").val();
//    var category = $("#Category").val();
//    var material = $("#Material").val();
//    var quantity = $("#DefaultQty").val();
//    var price = $("#Price").val();
//    var callOut = $("#CallOutFree").val();
//    var description = $("#Description").val();
//    var docs;
//    var a = [];


//    if (itemName == "") {
//        $("#ItemName").text("Item name email is required");
//        return false;
//    }
//    else {
//        $("#ItemName").text("");
//    }

//    var data = new FormData();
//    for (var i = 0; i < files.length; i++)
//        data.append("files[" + i + "]", files[i]);
//    data.append("itemName", itemName);
//    data.append("category", category);
//    data.append("material", material);
//    data.append("quantity", quantity)
//    data.append("price", price)
//    data.append("callOut", callOut)
//    data.append("description", description);

//    $.ajax({
//        type: 'POST',
//        url: common.SitePath + "/Employee/JCL/AddJCL",
//        processData: false,
//        contentType: false,
//        dataType: "json",
//        data: data,
//        success: function (data) {
//            if (data.responseText != "") {
//            }
//        },
//        error: function () {
//        }
//    })
//})

