$(document).ready(function () {
    //$('.validatehrs').mask("99:99");

    if (FSM.AddEmployeeStatus == "0") {
        $(".new_emp").show();                    //detail btn click show div
    }

    var categoryId = $("#CategoryId").val();
    if (categoryId == "") {
        $("#SubCategoryId").prop("disabled", true);
    }
    $('#BaseRate,#ActualRate').attr('Maxlength', '10');

    $(".ddlmultiselect").multiselect();
  
    var count = $("#list_files tr").length;
    if (parseInt(count) <= 2) {
        $("#list_files").append("<tr class='emptytr'> <td colspan='3'>No Record Available</td></tr>");
    }


    $('.addDatePickr').datepicker({ dateFormat: 'dd/mm/yy' });
    $('#VehicleManufactringYear').datepicker({ dateFormat: 'dd/mm/yy' });
    $('#VehicleDateChecked').datepicker({ dateFormat: 'dd/mm/yy' });
    $('#BirthDate').datepicker({ dateFormat: 'dd/mm/yy', maxDate: 0 });

    $("#Detailbutton").click(function () {
        $(".new_emp").show();                    //detail btn click show div
    });
    $('#Username').on('keypress', function (e) {
        if (e.which == 32)
            return false;
    });
    $(".validatehrs").attr('maxlength', '2');

    $("#sameashome").click(copyData);  //checkboxclick copy home address data in mailing address 
    $("#homepostcode,#mailpostcode").attr('maxlength', '9');
    $("#homepostcode,#mailpostcode").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");
    $("#homemobile,#mailmobile,#EmergencyMobile").attr('MaxLength', '10');
    $("#homemobile,#mailmobile,#Homelandline,#maillandline,#EmergencyMobile").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");
    //$(".validatehrs").attr("onkeypress", "return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46");

    $(".validatehrs").keydown(function (event) {
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

    var selectedrole = $('#Role option:selected').text();
    if (selectedrole == 'OPERATIONS') {
        $('.hourly-visible').css('display', 'block');
        $('#HourlyRate').removeAttr("disabled");
    }
    else {
        $('.hourly-visible').css('display', 'none');
        $('#HourlyRate').attr("disabled", "disabled");
    }


    $('#Employee').prop('checked', true);
    $(".Tfntogle").show();

    if ($("#Contractor").is(":checked")) {
        $(".Contracttogle").show();
    } else {
        $(".Contracttogle").hide();
        $("#BusinessName").val("");
        $("#ABN").val("");
    }
});
function copyData() {                                       //copy home address data into mailing address
    var HomeTitle = $("#hometitle").val();
    var HomeFName = $("#homefname").val();
    var HomelName = $("#homelname").val();
    var HomeWork = $("#homework").val();
    var HomeMobile = $("#homemobile").val();
    var HomeLandline = $("#homelandline").val();
    var HomeEmail = $("#txtHomeAddress").val();
    var HomeFax = $("#homefax").val();
    var HomeUnit = $("#homeunit").val();
    var HomeStreetNo = $("#homestreetno").val();
    var HomeStreetName = $("#homestreetname").val();
    var HomeStreetType = $("#homestreettype").val();
    var HomeSuburb = $("#homesuburb").val();
    var HomeState = $("#homestate").val();
    var HomePostCode = $("#homepostcode").val();

    if (this.checked == true) {
        $("#mailtitle").val(HomeTitle);
        $("#mailfname").val(HomeFName);
        $("#maillname").val(HomelName);
        $("#mailwork").val(HomeWork);
        $("#mailmobile").val(HomeMobile);
        $("#maillandline").val(HomeLandline);
        $("#txtmailAddress").val(HomeEmail);
        $("#mailfax").val(HomeFax);
        $("#mailunit").val(HomeUnit);
        $("#mailstreetno").val(HomeStreetNo);
        $("#mailstreetname").val(HomeStreetName);
        $("#mailstreettype").val(HomeStreetType);
        $("#mailsuburb").val(HomeSuburb);
        $("#mailstate").val(HomeState);
        $("#mailpostcode").val(HomePostCode);
    }
}

$(document).on('change', '#Role', function () {
    var selectedrole = $('#Role option:selected').text();
    if (selectedrole == 'OPERATIONS') {
        $('.hourly-visible').css('display', 'block');
        $('#HourlyRate').removeAttr("disabled");
    }
    else {
        $('.hourly-visible').css('display', 'none');
        $('#HourlyRate').attr("disabled", "disabled");
    }
});

$('#SignaturePicture').change(function () {
    $("#imguploader").show();
    var fileUpload = $("#SignaturePicture").get(0);
    var id = fileUpload.id;
    var files = fileUpload.files;

    // Create FormData object
    var fileData = new FormData();
    fileData.append(files[0].name, files[0]);
    DisplayImages(fileData, id);
    $("#imguploader").hide();

});

$('#ProfilePicture').change(function () {
    $("#imguploader").show();
    var fileUpload = $("#ProfilePicture").get(0);
    var id = fileUpload.id;
    var files = fileUpload.files;

    // Create FormData object
    var fileData = new FormData();
    fileData.append(files[0].name, files[0]);

    DisplayImages(fileData, id);
    $("#imguploader").hide();
});

$('#DrivingLicense').change(function () {
    var fileUpload = $("#DrivingLicense").get(0);
    var id = fileUpload.id;
    var files = fileUpload.files;

    // Create FormData object
    var fileData = new FormData();
    fileData.append(files[0].name, files[0]);

    DisplayImages(fileData, id);
});

$('#BankDetail').change(function () {
    var fileUpload = $("#BankDetail").get(0);
    var id = fileUpload.id;
    var files = fileUpload.files;

    // Create FormData object
    var fileData = new FormData();
    fileData.append(files[0].name, files[0]);

    DisplayImages(fileData, id);
});

$('#Insurance').change(function () {
    var fileUpload = $("#Insurance").get(0);
    var id = fileUpload.id;
    var files = fileUpload.files;

    // Create FormData object
    var fileData = new FormData();
    fileData.append(files[0].name, files[0]);

    DisplayImages(fileData, id);
});

function DisplayImages(fileData, id) {
    $.ajax({
        url: FSM.AddEmployeeURL,
        type: "POST",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: fileData,
        success: function (result) {
            $(".emptytr").remove();
            var count = $("#list_files tr").length;

            if (id == "SignaturePicture") {
                for (var i = 0; i < count; i++) {
                    i = i + 2;
                    if ($("#list_files tr:eq('"+i+"')").find('td').eq(0).text() == "Signature Picture") {
                        $("#list_files tr:eq('" + i+ "')").remove();
                    }
                }
            }
            else if (id == "ProfilePicture") {
                for (var i = 0; i < count; i++) {
                    if ($("#list_files tr:eq('" + i + "')").find('td').eq(0).text() == "Profile Picture") {
                        $("#list_files tr:eq('" + i + "')").remove();
                    }
                }
            }
            else if (id == "DrivingLicense") {
                for (var i = 0; i < count; i++) {
                    if ($("#list_files tr:eq('" + i + "')").find('td').eq(0).text() == "Driving License") {
                        $("#list_files tr:eq('" + i + "')").remove();
                    }
                }
            }
            else if (id == "BankDetail") {
                for (var i = 0; i < count; i++) {
                    if ($("#list_files tr:eq('" + i + "')").find('td').eq(0).text() == "Bank Detail") {
                        $("#list_files tr:eq('" + i + "')").remove();
                    }
                }
            }
            else if (id == "Insurance") {
                for (var i = 0; i < count; i++) {
                    if ($("#list_files tr:eq('" + i + "')").find('td').eq(0).text() == "Insurance") {
                        $("#list_files tr:eq('" + i + "')").remove();
                    }
                }
            }

            var fname = result;

            if (id == "SignaturePicture") {
                $("#list_files").append("<tr><td>" + "Signature Picture" + "</td><td>" + fname + "</td><td>" + "<img src='" + FSM.AddEmployeeROOT + fname + "' width='74' height='74'/>" + "</td></tr>");
                //$('#divSignaturePic').empty();
                //$('#divSignaturePic').append("<img src='" + FSM.AddEmployeeROOT + fname + "'width='74' height='74'  />");
            }
            else if (id == "ProfilePicture") {
                //$('#divProfilePic').empty();
                $("#list_files").append("<tr><td>" + "Profile Picture" + "</td><td>" + fname + "</td><td>" + "<img src='" + FSM.AddEmployeeROOT + fname + "' width='74' height='74'/>" + "</td></tr>");
            }
            else if (id == "DrivingLicense") {
                //$('#divDrivingLicenseImg').empty();
                $("#list_files").append("<tr><td>" + "Driving License" + "</td><td>" + fname + "</td><td>" + "<img src='" + FSM.AddEmployeeROOT + fname + "' width='74' height='74'/>" + "</td></tr>");
            }
            else if (id == "BankDetail") {
                //$('#divBankDetailDoc').empty();
                $("#list_files").append("<tr><td>" + "Bank Detail" + "</td><td>" + fname + "</td><td>" + "<img src='" + FSM.AddEmployeeROOT + fname + "' width='74' height='74'/>" + "</td></tr>");
            }
            else if (id == "Insurance") {
                //$('#divInsurance').empty();
                $("#list_files").append("<tr><td>" + "Insurance" + "</td><td>" + fname + "</td><td>" + "<img src='" + FSM.AddEmployeeROOT + fname + "' width='74' height='74'/>" + "</td></tr>");
            }

        },
        error: function (err) {
            alert(err.statusText);
        }
    });
}

$(document).on('blur', '.EmpUserName',      //validate o duplicate username
      function () {
          var EmpUserName = $(this).val();
          if (EmpUserName != "") {
              $.ajax({
                  url: common.SitePath + "/Employee/Employee/CheckUserNameExist",
                  data: { UserName: EmpUserName },
                  type: 'Get',
                  async: false,
                  success: function (data) {
                      if (data === 0) {
                          alert("Username not available ! Please choose different one.");
                          $(".EmpUserName").val("");
                      }
                  },
                  error: function () {
                      alert("something seems wrong");
                  }
              });
          }
      });

$(document).on('blur', '.EmpEmail', function () {                                   //validate on duplicate email
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    var Email = $(this).val();
    if (Email != "") {
        var result = regex.test(Email);
        if (result == false) {
            $(".EmpEmail").val("");
            alert("Please Provide a Valid email id ");
            return false;
        }
        $.ajax({
            url: common.SitePath + "/Employee/Employee/CheckEmployeeEmailExist",
            data: { EmpEmail: Email },
            type: 'Get',
            async: false,
            success: function (data) {
                if (data === 0) {
                    alert("Email ID alreday Exist ! Please Choose a different one");
                    $(".EmpEmail").val("");
                }
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    }

});

$("#Employee").click(function () {  //Employee Checkbox click TFN Show Hide
    if ($("#Employee").is(":checked")) {
        $(".Tfntogle").show();
    } else {
        $(".Tfntogle").hide();
        $("#TFN").val("");
    }
});

$("#Contractor").click(function () {   //Contractor Checkbox click ABN And Bussiness Name Show Hide
    if ($(this).is(":checked")) {
        $(".Contracttogle").show();
    } else {
        $(".Contracttogle").hide();
        $("#BusinessName").val("");
        $("#ABN").val("");
    }
});

$("#UploadFiles").change(function () {
    var searchval = $("#UploadFiles").val();
    if (searchval == 1) {
        $(".signaturefile").show();
        $(".profilefile").hide();
        $(".drivingfile").hide();
        $(".bankfile").hide();
        $(".insurancefile").hide();
        //var StatusVal = $('#ddljobStatus').val();
        //if (StatusVal != "") {
        //    $('#ddljobStatus').val("");
        //}

    }
    else if (searchval == 2) {
        $(".signaturefile").hide();
        $(".profilefile").show();
        $(".drivingfile").hide();
        $(".bankfile").hide();
        $(".insurancefile").hide();
        //var datepicked = $('#DateBooked').val();
        //if (datepicked != "") {
        //    $('#DateBooked').val("");
        //}
    }
    else if (searchval == 3) {
        $(".signaturefile").hide();
        $(".profilefile").hide();
        $(".drivingfile").show();
        $(".bankfile").hide();
        $(".insurancefile").hide();
        var StatusVal = $('#ddljobStatus').val();
        //if (StatusVal != "") {
        //    $('#ddljobStatus').val("");
        //}

        //$("#ddldatepickdiv").show();
    }
    else if (searchval == 4) {
        $(".signaturefile").hide();
        $(".profilefile").hide();
        $(".drivingfile").hide();
        $(".bankfile").show();
        $(".insurancefile").hide();
        //var StatusVal = $('#ddljobStatus').val();
        //if (StatusVal != "") {
        //    $('#ddljobStatus').val("");
        //}

        $("#ddldatepickdiv").show();
    }
    else if (searchval == 5) {
        $(".signaturefile").hide();
        $(".profilefile").hide();
        $(".drivingfile").hide();
        $(".bankfile").hide();
        $(".insurancefile").show();
        //var StatusVal = $('#ddljobStatus').val();
        //if (StatusVal != "") {
        //    $('#ddljobStatus').val("");
        //}

        $("#ddldatepickdiv").show();
    }
    else {
        $(".signaturefile").hide();
        $(".profilefile").hide();
        $(".drivingfile").hide();
        $(".bankfile").hide();
        $(".insurancefile").hide();
        //var StatusVal = $('#ddljobStatus').val();
        //if (StatusVal != "") {
        //    $('#ddljobStatus').val("");
        //}
        //return false;
    }
});

//Bind Rate SubCategory 
$("#CategoryId").change(function () {
    var CategoryId = $(this).val();
    Category.GetAllCategoryId(CategoryId)
})
$("#SubCategoryId").change(function () {
    var subCategoryId = $(this).val();
    SubCategory.GetAllSubCategoryId(subCategoryId)
})

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
                        $("#BaseRate").val("0.00");
                        $("#ActualRate").val("0.00");
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
            $("#BaseRate").val("0.00");
            $("#ActualRate").val("0.00");
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



