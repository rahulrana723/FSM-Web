$(document).ready(function () {
    $("#EmployeeType").attr("disabled", "disabled");
    var categoryId = $("#CategoryId").val();
    if(categoryId==""){
        $("#SubCategoryId").prop("disabled", true);
    }
    $('#EffectiveDate').datepicker({
        dateFormat: 'dd/mm/yy'
        //dateFormat: "dd/M/yy",  
        //changeMonth: true,  
        //changeYear: true,  
        //yearRange: "-60:+0"  
    });
    $('#BaseRate,#ActualRate').attr('Maxlength', '10');
    $("#MobileNumber").attr('Maxlength', '10');
});
$("#EID").change(function () {
    var EID = $(this).val();
    Employee.GetAllEmployeeId(EID)

})

//Bind Rate SubCategory 
$("#CategoryId").change(function () {
    var CategoryId = $(this).val();
    Category.GetAllCategoryId(CategoryId)
})
$("#SubCategoryId").change(function () {
    var subCategoryId = $(this).val();
    SubCategory.GetAllSubCategoryId(subCategoryId)
})

$('form').submit(function () {
    var EID = $("#EID").val();
    var subcategoryId=$("#SubCategoryId").val();
    // Check if empty of not
    if (EID === 'Select') {
        $(".Eidval").text("Please select EID");
        return false;
    }
    else {
        $(".Eidval").text("");
    }
    // Check if empty of not category
    if (subcategoryId === "") {
        $(".CategoryVal").text("Please select Rate sub category");
        return false;
    }
    else {
        $(".CategoryVal").text("");
    }
});


var Category = {

    GetAllCategoryId: function (CategoryId) {
        $(".CategoryVal").text("");
        if (CategoryId != "") {

            $.ajax({
                type: "POST",
                url: common.SitePath + "/Employee/Employee/GetCategoryDetailByCategoryId",
                data: JSON.stringify({ CategoryId: CategoryId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (result) {
                    if (result != null) {
                        $('#SubCategoryId').empty();
                        $("#SubCategoryId").prop("disabled", false);
                        $('#SubCategoryId').append("<option value=''>(Select)</option>")
                        for (var i = 0; i < result.length; i++) {
                            var opt = new Option(result[i].Text, result[i].Value);
                            $('#SubCategoryId').append(opt);
                        }
                    }
                },
                Error: {

                }
            });

        }
        else {
            $('#SubCategoryId').empty();
            $('#SubCategoryId').append("<option value=''>(Select)</option>")
            $("#SubCategoryId").prop("disabled", true);
        }
    }

}

var SubCategory = {

    GetAllSubCategoryId: function (subCategoryId) {
        $(".CategoryVal").text("");
        if (subCategoryId != "") {

            $.ajax({
                type: "POST",
                url: common.SitePath + "/Employee/Employee/GetCategoryDetailBySubCategoryId",
                data: JSON.stringify({ SubCategoryId: subCategoryId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (result) {
                    if (result != null) {
                        var BaseRate = result.BaseRate;
                        var ActualRate = result.ActualRate;
                        $("#BaseRate").val(BaseRate);
                        $("#ActualRate").val(ActualRate);
                    }
                },
                Error: {

                }
            });

        }
        else {
           
            $("#BaseRate").val("0.00");
            $("#ActualRate").val("0.00");
        }
    }

}

var Employee = {

    GetAllEmployeeId: function (EmpID) {
        $(".Eidval").text("");
        if (EmpID != 'Select') {

            $.ajax({
                type: "POST",
                url: common.SitePath + "/Employee/Employee/GetEmployeeDetailByEID",
                data: JSON.stringify({ EID: EmpID }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (result) {
                    var name = "";
                    if (result.FirstName != null)
                        name = result.FirstName;
                    if (result.LastName != null)
                        name = name + " " + result.LastName;
                    $("#EmployeeName").val(name);
                    if (result.Role == "31cf918d-b8fe-4490-b2d7-27324bfe89b4") {
                        $("#EmployeeType").val(1);
                        $("#EmployeeRateID").val(1);
                    }
                    else {
                        $("#EmployeeType").val(2);
                        $("#EmployeeRateID").val(2);
                    }
                    //$("#EmployeeType").attr("disabled");
                    $("#MobileNumber").val(result.HomeAddressMobile);

                },
                Error: {

                }
            });

        }
        else {
            $("#EmployeeName").val("");
            $("#MobileNumber").val("");
        }
    }

}

$('#BaseRate,#ActualRate').keypress(function (event) {
    var $this = $(this);
    if ((event.which != 46 || $this.val().indexOf('.') != -1) &&
       ((event.which < 48 || event.which > 57) &&
       (event.which != 0 && event.which != 8))) {
        event.preventDefault();
    }

    var text = $(this).val();
    if ((event.which == 46) && (text.indexOf('.') == -1)) {
        setTimeout(function () {
            if ($this.val().substring($this.val().indexOf('.')).length > 3) {
                $this.val($this.val().substring(0, $this.val().indexOf('.') + 3));
            }
        }, 1);
    }

    if ((text.indexOf('.') != -1) &&
        (text.substring(text.indexOf('.')).length > 2) &&
        (event.which != 0 && event.which != 8) &&
        ($(this)[0].selectionStart >= text.length - 2)) {
        event.preventDefault();
    }
});
$("#MobileNumber").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");


