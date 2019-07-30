var addjobs = {
    getPrvdate1: "",
    FSM1: "",
    BindSupervisoronRefresh: function () {
        $('#superVisor').empty();
        assignUsers = ""
        assignUsertexts = "Select";
        var opts = new Option(assignUsertexts, assignUsers);
        $('#superVisor').append(opts);
        $(".ddljobsAssignlist").each(function () {
            assignUser = $(this).val();
            assignUsertext = $("option:selected", this).text();
            var checkSuperVisorExist = 1;
            $("#superVisor option").each(function () {
                superUser = $(this).val();
                if (assignUser == superUser) {
                    checkSuperVisorExist = 0;
                }
            })
            if (assignUsertext != "Select" && checkSuperVisorExist == 1) {
                var opt = new Option(assignUsertext, assignUser);
                $('#superVisor').append(opt);
            }
        })
    }
}


$(document).ready(function () {
    $(".timepick").hide();
    //$('#tempAssignTo2 option').prop("disabled", true);
    // disable OTRWjobNotes
    var approveStatus = $("#approveStatus").val();
    $('.AssignDateBooked').datepicker({
        dateFormat: 'dd/mm/yy'
    });

    //$("#JobType option[value='5']").hide();   //hide JobType Value(Call-Back)
    //$("#JobType option[value='6']").hide();  //hide JobType Value(Re-Quote)
    //$("#JobType option[value='7']").hide();  //hide JobType Value(Check Measure)

    var currentjobtype = $("#currentJobtype").val();   //get Current job type
    var status = $("#Status").val();    //get Currnet Status

    //if (currentjobtype == "1" && status == "15") {
    //    $("#JobType option[value='3']").hide();
    //    $("#JobType option[value='4']").hide();

    //    $("#JobType option[value='7']").show();
    //}
    //else if (currentjobtype == "6" && status == "15") {
    //    $("#JobType option[value='1']").show();
    //    $("#JobType option[value='3']").hide();
    //    $("#JobType option[value='4']").hide();

    //    $("#JobType option[value='7']").show();
    //}
    //else if (currentjobtype == "1") {
    //    $("#JobType option[value='3']").hide();
    //    $("#JobType option[value='4']").hide();

    //    $("#JobType option[value='6']").show();
    //}
    //else if (currentjobtype == "7" && approveStatus == "True") {
    //    $("#JobType option[value='1']").hide();
    //    $("#JobType option[value='3']").hide();
    //    $("#JobType option[value='4']").hide();
    //    $("#JobType option[value='5']").hide();
    //    $("#JobType option[value='6']").hide();

    //    $("#JobType option[value='7']").show();
    //}
    //else if ((currentjobtype == "2" && status == "15") || currentjobtype == "5") {
    //    $("#JobType option[value='5']").show();
    //    if (currentjobtype == "5") {
    //        $(".timepick").hide();
    //    }
    //}
    //else if (currentjobtype == "6" || currentjobtype == "7") {
    //    //$("#JobType option[value='1']").hide();
    //    //$("#JobType option[value='3']").hide();
    //    //$("#JobType option[value='4']").hide();

    //    $("#JobType option[value='6']").show();
    //    $("#JobType option[value='7']").show();

    //    $(".timepick").hide();
    //}


    var FSM1 = 1;
    var workType = 1;
    BindOTRWWithWorkType(workType, FSM1);

    if (FSM.SiteDetailId != "")   //For Redirect Wizard
    {
        $(".ddlassign1").hide();
        $("#tempAssignTo").val("");
        //$("#tempAssignTo2").multiselect('clearSelection');
        $(".ddlassign2").show();
        $(".OtrwReqDiv").show();
    }

    var otrwjobnoteId = $('#OTRWjobNotes').prev("div").prop("id");
    $('#' + otrwjobnoteId + '').attr('contenteditable', 'false');


    //$('.timepicker').timepicker({
    //    timeFormat: 'h:mm p',
    //    interval: 60,
    //    minTime: '10',
    //    maxTime: '6:00pm',
    //    defaultTime: '11',
    //    startTime: '10:00',
    //    dynamic: false,
    //    dropdown: true,
    //    scrollbar: true
    //});
    //$('#StartTime').timepicker({
    //    'timeFormat': 'h:i A',
    //    minTime: '06:00am',
    //    maxTime: '08:00pm', step: 30
    //});

    //$('.AssignStartTime').timepicker({
    //    'timeFormat': 'h:i A',
    //    minTime: '06:00am',
    //    maxTime: '08:00pm', step: 30
    //})

    //var assignUserCount = FSM.AssignInfoCount;
    //for (var i = 0; i < assignUserCount; i++) {

    //    var actualvalue = $("#AssignInfo_" + i + "__AssignStartTime").val();
    //    if (actualvalue != '' && actualvalue != 'undefined' && actualvalue != null) {
    //        var time = actualvalue.split(' ')[1];
    //        var hours = time.split(':')[0];
    //        var minutes = time.split(':')[1];
    //        var ampm = hours >= 12 ? 'PM' : 'AM';
    //        hours = hours % 12;
    //        hours = hours ? hours : 12; // the hour '0' should be '12'
    //        //  minutes = minutes < 10 ? '0' + minutes : minutes;
    //        hours = hours < 10 ? '0' + hours : hours;
    //        var strTime = hours + ':' + minutes + ' ' + ampm;
    //        $("#AssignInfo_" + i + "__AssignStartTime").val(strTime);
    //    }
    //}

    //var datebooked = $('#DateBooked').val();
    //var actualvalue = $("#StartTime").val();
    //var s = $(this).val();
    //var time = actualvalue.split(' ')[1];
    //$("#StartTime").val(time);


    $('#Status').attr('disabled', 'disabled');

    //$("#tempAssignTo").multiselect();
    $(".ddlmultiselect").multiselect();

    //tinymce editor
    //    tinymce.init({
    //        selector: 'textarea',
    //        width: "200",
    //        height: "100",
    //        theme: 'modern',
    //        plugins: [
    //      'advlist autolink lists link image charmap print preview hr anchor pagebreak',
    //  'searchreplace wordcount visualblocks visualchars code fullscreen',
    //'insertdatetime media nonbreaking save table contextmenu directionality',
    //'emoticons template paste textcolor colorpicker textpattern imagetools codesample'
    //        ],
    //        toolbar1: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image',
    //        toolbar2: 'print preview media | forecolor backcolor emoticons | codesample',
    //        image_advtab: true,
    //        templates: [
    //    { title: 'Test template 1', content: 'Test 1' },
    //    {
    //        title: 'Test template 2', content: 'Test 2'
    //    }
    //        ],

    //    });
    //auto populate list
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
            $("#CustomerInfoId").empty();
            var SearchTerm = request.term;
            var customerlist = [];

            // converting json to array
            //var customerlist = jQuery.parseJSON(FSM.CustomerListVal);
            var customerlist;
            var selectelmnt = this.element;
            // filtering values based on some value
            //customerlist = jQuery.grep(customerlist, function (item) {
            //    var text = item.Text;
            //    if (text != '' && text != undefined) {
            //        var index = text.toLowerCase().indexOf(SearchTerm.toLowerCase());
            //        return index > -1 ? true : false;
            //    }
            //});

            // taking first twenty values if greater than 10
            //customerlist = customerlist.length > 10 ? customerlist.slice(0, 20) : customerlist;

            // getting values
            $.ajax({
                url: FSM.GetCustomer,
                type: 'GET',
                data: { SearchTerm: SearchTerm },
                async: false,
                success: function (result) {
                    customerlist = result.customerData;
                },
                error: function (ex) {
                    alert("something seems wrong!")
                }
            })

            window.setTimeout(function () {
                for (var i = 0; i < customerlist.length; i++) {
                    $("#CustomerInfoId").append("<option value='" + customerlist[i].Value + "'>" + customerlist[i].Text + "</option>");
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
            //for (var i = 0; i < customerlist.length; i++) {
            //    $("#CustomerInfoId").append("<option value='" + customerlist[i].Value + "'>" + customerlist[i].Text + "</option>");
            //}

            //var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            //response(selectelmnt.children("option").map(function () {
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
    $("#CustomerInfoId").combobox();
    $("#CustomerInfoId").combobox({
        select: function (event, ui) {
            var s = $(this).val();
            Bindsitedetail(s);
            BindlinkJobList(s);
        }
    });
    $("#toggle").on("click", function () {
        $("#CustomerInfoId").toggle();
    });

    $(".custom-combobox-input ").attr("placeholder", "Search Customer");

    //if (FSM.JobType == 'Support') {
    //    $('#LinkJobId').removeAttr('disabled');
    //}
    //else {
    //$('#LinkJobId').attr('disabled', 'disabled');
    //}
    if (FSM.CustomerGeneralInfoId != "00000000-0000-0000-0000-000000000000" && FSM.CustomerGeneralInfoId != undefined) {
        $('#LinkJobId').removeAttr('disabled');
    }
    //else {
    //    $('#LinkJobId').attr('disabled', 'disabled');
    //}

    $(".custom-combobox-input").keyup(function () {
        var genearlInfoId = $(this).val();
        if (genearlInfoId == "") {
            $('#tempSiteId').val('').empty();
            $('#tempSiteId').append("<option value=''>(Select)</option>")
        }
    })
    if (FSM.ShowMsg != '' && FSM.ShowMsg != undefined) {
        FSM.ShowMsg = '';
        $('.jobalert').empty();
        $('.jobalert').css('color', 'green');
        $('.jobalert').html("<strong>Record saved successfully !</strong>");
        $('.jobalert').show();
        $(".tabs li").removeClass("ui-state-disabled");
        window.setTimeout(function () {
            $('.jobalert').hide();
        }, 4000)
        FSM.ShowMsg = '';
    }
    if (FSM.JobStatus == 'Assigned' && FSM.MultiplePeople == 1) {
        $(".ddlassign1").hide();
        $("#tempAssignTo").val("");
        $(".OtrwReqDiv").show();
        $(".ddlassign2").show();
        $('.multiselect').removeAttr('disabled');
    }
    else if (FSM.JobStatus != 'Assigned' && FSM.MultiplePeople == 1) {
        $(".ddlassign1").hide();
        $("#tempAssignTo").val("");
        $(".OtrwReqDiv").show();
        $(".ddlassign2").show();
        $('.multiselect').removeAttr('disabled');
        //   $('.multiselect').attr('disabled', 'disabled');
    }
    else if (FSM.JobStatus == 'Assigned') {
        $(".ddlassign2").hide();
       // $(".OtrwReqDiv").hide();
        $("#OtrwReuired").val();
        $("#tempAssignTo2").multiselect('clearSelection');
        $('#tempAssignTo').removeAttr('disabled');
        $(".ddlassign1").show();
    }
    else if (FSM.JobStatus == 'Completed') {
        $('#DateBooked').attr('Readonly', true);
    }
    else {
        $('#tempAssignTo').attr('disabled', 'disabled');
        $('#tempAssignTo2').attr('disabled', 'disabled');
    }
    if ($(".ddlassign2").is(':visible')) {
        $("#AssignOtrwLabel").text('Assign To OTRW 1');
    }
    else {
        $("#AssignOtrwLabel").text('Assign To OTRW');
    }

    if (FSM.CustomerId != '' && FSM.CustomerId != undefined) {
        $('#CustomerInfoId').val(FSM.CustomerId);
        $(".custom-combobox-input ").val(FSM.CustomerName);
        Bindsitedetail(FSM.CustomerId);
    }

    window.setTimeout(function () {
        $('span.overlay').fadeOut('slow');
    }, 200)


    //Upadte Quote Status Approved Or Not Approved
    $(document).on('click', '.change-status-job', function (event) {
        event.preventDefault();
        var jobid = $(this).attr('Empjobid');

        $.ajax({
            url: common.SitePath + "/Employee/CustomerJob/QuoteDataPdfGenerate",
            data: { JobId: jobid },
            type: 'Get',
            async: true,
            success: function (data) {
                var JobStatusUrl = common.SitePath + "/Employee/CustomerJob/ChangeJobApprovedStatus";

                var checkstatus = $('.status-job').html();
                var jobstatus = '';
                if (checkstatus == "[ Approved ]") {
                    jobstatus = "NotApprove";
                }
                else {
                    jobstatus = "Approve";
                }

                var data = {
                    JobId: jobid, JobStatus: jobstatus
                };
                $.get(JobStatusUrl, data, function myfunction(result) {
                    $('.status-job').html(result.jobstatus);
                    if (result.status == 0) {
                        alert("Quote cannot be approved until quote invoice is created!");
                        $('.status-job').html(checkstatus);
                    }
                    else if (result.jobstatus == "[ Approved ]") {
                        $('#approveStatus').val(true);
                    }
                    else {
                        $('#approveStatus').val(false);
                    }

                })
                    .fail(function () {
                        alert("error");
                    });
            },
            error: function () {
            }
        })


    });
})

function BindlinkJobList(CustomerInfoId) {
    var data = { CustomerInfoId: CustomerInfoId };

    if (CustomerInfoId != undefined) {
        $.get(FSM.GetLinkJobUrl, data, function (result) {
            $('#LinkJobId').empty();
            $('#LinkJobId').removeAttr('disabled');
            $('#LinkJobId').append("<option value=''>(Select)</option>")
            for (var i = 0; i < result.length; i++) {
                var opt = new Option(result[i].Text, result[i].Value);
                $('#LinkJobId').append(opt);
            }
        });
    }
}

function Bindsitedetail(CustomerInfoId) {

    var data = { CustomerInfoId: CustomerInfoId };

    if (CustomerInfoId != undefined) {
        $.get(FSM.FillSiteListUrl, data, function (result) {
            $('.fillsitecombo').empty();
            $('.fillsitecombo').html(result);
        });
    }
}



$(document).on("click", ".savejob", function (event) {
    //Maximum jobNo check
    debugger
    var currentjobtype = $("#currentJobtype").val();
    if ($('#JobType').val() == '2' && currentjobtype != $('#JobType').val()) {
        if ($('#DateBooked').val() == "") {
            $('.hdnStatus').val('13');
        }
        else if ($('#DateBooked').val() != "" && $('.JobAssign-Table tbody tr').length > 0) {
            $('.hdnStatus').val('5');
        }
    }

    var CurrentJobNo = $("#JobNo").val();
    var empJobId = $("#empJobId").val();
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/GetMaximumJobNo",
        type: 'GET',
        data: { currentJobNo: CurrentJobNo },
        success: function (dataSuccess) {
            if (dataSuccess.result != CurrentJobNo && empJobId == "00000000-0000-0000-0000-000000000000") {
                alert("Job No " + CurrentJobNo + " is created by some other user. You have assinged a new Job No " + dataSuccess.result + ".");
                var editJobNo = dataSuccess.result;
                $("#JobNo").val(editJobNo);
            }

            //Record Save Without Date Pop Up
            var dateBooked = $("#DateBooked").val();
            if (dateBooked == null || dateBooked == "") {
                $(".NoDatePopUp").modal('show');
                $(".alertmsg").text("Are you sure you want to proceed without selecting date booked?");
                $(document).on('click', ".btnYesCnfrm", function () {
                    $(".NoDatePopUp").modal('hide');
                    SaveFormData();
                    SaveJob(event);
                });
                $(document).on('click', ".btnNoCnfrm", function () {
                    $(".NoDatePopUp").modal('hide');
                });
            }
            else {
                SaveFormData();
                SaveJob(event);
            }
        },
    });

});

function SaveJob(event) {
    $(".commonpopup").modal('hide');
    $('#tempAssignTo2 option').prop("disabled", false);
    event.stopImmediatePropagation();
    var isSiteDisable = document.getElementById("tempSiteId").hasAttribute("disabled");
    $('#tempSiteId').removeAttr("disabled");
    //var isAssignedToDisable = document.getElementById("tempAssignTo").hasAttribute("disabled");
    //$('#tempAssignTo').removeAttr("disabled");
    //var JobNotes = document.getElementById('JobNotes_ifr').contentWindow.document.body.innerHTML;
    //var OperationNotes = document.getElementById('OperationNotes_ifr').contentWindow.document.body.innerHTML;
    var JobNotes = $("#JobNotes").val();
    var OperationNotes = $('#OperationNotes').val();
    var otrwJobNotes = $('#OTRWjobNotes').val();
    var formdata = new FormData($('#frmSaveJob').get(0));
    if (JobNotes != "<p>&nbsp;<br></p>") {
        formdata.append("Job_Notes", JobNotes);
    }
    if (OperationNotes != "<p>&nbsp;<br></p>") {
        formdata.append("Operation_Notes", OperationNotes);
    }
    if (otrwJobNotes != "<p>&nbsp;<br></p>") {
        formdata.append("OTRW_Notes", otrwJobNotes);
    }
    $.ajax({
        url: $('#frmSaveJob').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        cache: false,
        success: function (result) {
            if (isSiteDisable) {
                $('#tempSiteId').attr("disabled", "disabled");
            }
            else {
                $('#tempSiteId').removeAttr("disabled");
            }
            //if (isAssignedToDisable) {
            //    $('#tempAssignTo').attr("disabled", "disabled");
            //}
            //else {
            //    $('#tempAssignTo').removeAttr("disabled");
            //}
            if (result.status == "saved") {
                if (result.SavedId != '' && result.SavedId != undefined) {
                    FSM.SavedId = result.SavedId;
                    window.location.href = FSM.SaveJobInfoUrl + "?id=" + result.SavedId + "&showmsg=Yes";
                    //window.location.href = FSM.SaveJobInfoUrl + "/" + result.SavedId;
                }
                else if (result.savedId != '' && result.savedId != undefined) {
                    FSM.SavedId = result.savedId;
                    window.location.href = FSM.SaveJobInfoUrl + "?id=" + result.savedId + "&showmsg=Yes";
                }
                else {
                    $('#jobmsgDv').empty();
                    $(window).scrollTop(0);
                    $('.jobalert').empty();
                    $('.jobalert').css('color', 'green');
                    $('.jobalert').html(result.msg);
                    $('.jobalert').show();
                    $(".tabs li").removeClass("ui-state-disabled");
                    window.setTimeout(function () {
                        $('.jobalert').hide();
                        location.reload();
                    }, 4000)
                }
            }
            else {
                $(window).scrollTop(0);
                $('#jobmsgDv').empty();

                var ErrorList = "<ul style='list-style:none;'>"
                $(result.errors).each(function (i) {
                    ErrorList = ErrorList + "<li>" + result.errors[i].ErrorMessage + "</li>";
                });
                ErrorList = ErrorList + "</ul>"
                $(window).scrollTop(0);
                $('#jobmsgDv').css('color', 'red');
                $('#jobmsgDv').html(ErrorList);
                $('#jobmsgDv').show();
            }
            $("#jobfileuploader").val("");

        },
        error: function () {
            alert('something went wrong !');
        }
    });
}

//$(document).on("change", "#JobType", function () {
//    var cmbVal = $(this).val();
//    Bindsitedetail();

//    // 3 represents supportjob
//    if (parseInt(cmbVal) == 3) {
//        $('#LinkJobId').removeAttr('disabled');
//        $('#CustomerInfoId').val('');
//        $(".custom-combobox-input").val('');
//        $(".custom-combobox-input").attr("readonly", "readonly");
//        $("#tempSiteId").val('');
//        $(".cmbsitelist").val('');
//        $('#tempSiteId').attr("disabled", "disabled");

//    }
//    else {
//        $('#LinkJobId').attr('disabled', 'disabled');
//        $('#LinkJobId').val('');
//        //$('#CustomerInfoId').val('');
//        //$(".custom-combobox-input").val('');
//        $(".custom-combobox-input").removeAttr("readonly");
//        //$("#tempSiteId").val('');
//        $(".cmbsitelist").val('');
//        $('#tempSiteId').removeAttr("disabled");
//    }
//});

//$(document).on("change", "#LinkJobId", function () {
//    var cmbVal = $(this).val();
//    var data = { LinkJobId: cmbVal };

//    $.get(FSM.GetCustomerUrl, data, function (result) {
//        if (result.Customer != null || result.Customer != undefined) {
//            $('#CustomerInfoId').val(result.Customer.CustomerGeneralInfoId);
//            $(".custom-combobox-input ").val(result.Customer.CustomerLastName);
//            $(".custom-combobox-input ").attr("readonly", "readonly");
//            Bindsitedetail(result.Customer.CustomerGeneralInfoId);
//            window.setTimeout(function () {
//                $('#tempSiteId').val(result.Customer.SiteId);
//                $(".cmbsitelist").val(result.Customer.SiteId);
//                $('#tempSiteId').attr("disabled", "disabled");
//            }, 1000)
//        }
//        else {
//            window.setTimeout(function () {
//                $('#CustomerInfoId').val('');
//                $(".custom-combobox-input").val('');
//                $(".custom-combobox-input").attr("readonly", "readonly");
//                $('#tempSiteId').val('');
//                $(".cmbsitelist").val('');
//                $('#tempSiteId').attr("disabled", "disabled");
//            }, 1000)
//        }

var OtrwReq = $("#OtrwReuired").val();
if (OtrwReq > 1) {
    $('#superVisor').removeAttr('disabled');
}
else {
    //$('#superVisor').empty();
    $('#superVisor').html("");
    $('#superVisor').append("<option value=''>(Select)</option>")
    $('#superVisor').attr('disabled', 'disabled');
}

//    });

//});

$(document).on('click', '.viewJobDocs', function (event) {
    event.preventDefault();
    var jobid = $(this).attr("Jobid");
    var data = {
        JobId: jobid
    };

    $.get(FSM.GetJobDocuments, data, function (result) {
        $(".modaldoc").empty();
        var doc = "";
        if (result.JobDocs.length > 0) {
            for (var i = 0; i < result.JobDocs.length; i++) {
                doc = doc + "<p class='docjobName',style=float: left;width: 66% ;padding: 5px 0 0 0;>" +
                      result.JobDocs[i]["DocName"] + "</p><button class='btndownload btn-success' data-attr='" +
                      result.JobDocs[i]["Id"] + "' >Download</button><button class='btndeletedoc btn-danger " +
                      "deldoc' data-attr='" + result.JobDocs[i]["Id"] + "' data-jobid='" + jobid + "'>Delete</button></br>";
            }
        }
        else {
            doc = "<p>No documents uploaded regarding the job!</p>";
        }
        $(".modaldoc").html(doc);
        $("#myModal").modal('show');
    });

});

$(document).on('click', '.btndeletedoc', function () {
    var documentid = $(this).attr("data-attr");
    var jobid = $(this).attr("data-jobid");
    var data = {
        DocId: documentid, JobId: jobid
    };

    var check = confirm("Are you sure to delete the doc?");
    if (check) {
        $.ajax({
            url: FSM.DeletejobDocument,
            data: data,
            type: 'GET',
            async: false,
            success: function (result) {
                $(".modaldoc").empty();
                var doc = "";
                if (result.JobDocs.length > 0) {
                    for (var i = 0; i < result.JobDocs.length; i++) {
                        doc = doc + "<p class='docjobName',style=float: left;width: 66% ;padding: 5px 0 0 0;>" +
                              result.JobDocs[i]["DocName"] + "</p><button class='btndownload btn-success' data-attr='" +
                              result.JobDocs[i]["Id"] + "' >Download</button><button class='btndeletedoc btn-danger " +
                              "deldoc' data-attr='" + result.JobDocs[i]["Id"] + "' data-jobid='" + jobid + "'>Delete</button></br>";
                    }
                }
                else {
                    doc = "<p>No documents uploaded regarding the job!</p>";
                }
                $(".modaldoc").html(doc);
                $("#myModal").modal('show');
            },
            error: function () {
                alert("something seems wrong!")
            }
        })
    }
})

$(document).on('click', '.btndownload', function () {
    var documentid = $(this).attr("data-attr");
    var data = {
        Id: documentid
    };

    $.ajax({
        url: FSM.DownloadDocument,
        data: data,
        type: 'POST',
        async: false,
        success: function (data) {
            window.location = FSM.DownloadDocument + "?Id=" + documentid;
        },
        error: function (data) {
            alert("something seems wrong!")
        }
    })
})

$(document).on('change', '#Status', function () {
    var statusVal = $(this).val();
    var workType = $("#WorkType").val();
    if (workType > 0) {
        if (statusVal == '5') {
            $('#tempAssignTo').removeAttr('disabled');
            $('.multiselect').removeAttr('disabled');
        }
        else if (statusVal == '7') {
            $('#tempAssignTo').attr('disabled', 'disabled');
            $('.multiselect').attr('disabled', 'disabled');
        }
        else {
            $('#tempAssignTo').val("");
            $('#tempAssignTo').attr('disabled', 'disabled');
            $('.multiselect').attr('disabled', 'disabled');
        }
    }
    else {
        $("#jobmsgDv").show();
        $("#jobmsgDv").html("Please select work type first !");
        window.setTimeout(function () {
            $('#jobmsgDv').hide();
        }, 4000)
        $("#Status").val();
        return false;
    }
});

$("#AddCustInJob").on('click', function (e) {
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/_AddNewCustomer",
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowNewCustomerPopup").html(data);
            $("#modalAddCustomer").modal("show");
            e.preventDefault();
            e.stopPropagation();
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$("#AddSiteInJob").on('click', function (event) {
    $(".jobalert").html('');
    var selectCustomer = $(".custname").val();
    if (selectCustomer == "" || selectCustomer == null) {
        $(".jobalert").show();
        $(".jobalert").append("<Strong>please select customer</Strong>");
        return false;
    }

    var customerId = $(".custname").val();
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/_AddNewCustomerSite",
        data: {
            GeneralInfoId: customerId
        },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowNewCustomerPopup").html(data);
            $("#modalAddSite").modal("show");
            event.preventDefault();
            return false;
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

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
            FSM.CustomerListVal = data;
            $('.jobalert').empty();
            $('.jobalert').css('color', 'green');
            $('.jobalert').html("<Strong>Record saved successfully !</Strong>");
            $('.jobalert').show();
            $("#modalAddCustomer").modal("hide");
            window.setTimeout(function () {
                $('.jobalert').hide();
            }, 3000)
        },
        error: function () {
            alert("something seems wrong");
        }
    })
});
$(document).on("click", "#custcancel", function () {
    $("#modalAddCustomer").modal("hide");
});


$(document).on("click", ".sitesaveBtns", function () {
    $('.text-validation').html("");
    var customerGeneralInfoId = $("#CustGeneralId").val();
    var Unit = $("#siteUnit").val();
    var Street = $("#siteStreet").val();
    var Name = $("#siteName").val();
    var Type = $("#siteType").val();
    var Suburb = $("#siteSuburb").val();
    var State = $("#siteState").val();
    var PostalCode = $("#siteCode").val();
    var tabledata = {
        "CustomerGeneralInfoId": customerGeneralInfoId, "Unit": Unit, "Street": Street, "StreetName": Name,
        "StreetType": Type, "Suburb": Suburb, "State": State, "PostalCode": PostalCode
    }

    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/_AddNewCustomerSite",
        data: {
            model: tabledata
        },
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
            } else {
                var customerId = $('#CustomerInfoId').val();
                if (customerId != '' && customerId != undefined) {
                    Bindsitedetail(customerId);
                }
                $('.jobalert').empty();
                $('.jobalert').css('color', 'green');
                $('.jobalert').html("<Strong>Record saved successfully !</Strong>");
                $('.jobalert').show();
                $("#modalAddSite").modal("hide");
                window.setTimeout(function () {
                    $('.jobalert').hide();
                },
                    3000);
            }

        },
        error: function () {
            alert("something seems wrong");
        }
    });
});
$(document).on("click", "#sitecancel", function () {
    $("#modalAddSite").modal("hide");
});

$(document).on('change', '#WorkType', function () {
    var workType = $(this).val();
    var FSM1 = 0;
    if (workType != "") {
        BindOTRWWithWorkType(workType, FSM1);
        $('#superVisor').empty();
        $('#superVisor').append("<option value=''>(Select)</option>")
    }

});

$(document).on("click", "#btnextendjob", function () {
    var jobId = $("#empJobId").val();
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/_ExtendJob",
        data: { JobId: jobId },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowCallBack").empty();
            $("#divShowCallBack").html(data);
            $("#modalExtendJobCalender").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});


function BindOTRWWithWorkType(workType, FSM1) {
    var data = {
        WorkType: workType
    };

    if (workType != undefined) {
        $.get(FSM.GetWorkTypeOTRW, data, function (result) {
            if (FSM1 != 1) {
                $("#tempAssignTo2").multiselect('destroy');
                $('#tempAssignTo2').html("");
                for (var i = 0; i < result.length; i++) {
                    var opt = new Option(result[i].Text, result[i].Value);
                    //opt.disabled = true;
                    $('#tempAssignTo2').append(opt).multiselect('rebuild');
                }

                $("#tempAssignTo").empty();
                $('#tempAssignTo').append("<option value=''>Select OTRW User</option>")
                for (var i = 0; i < result.length; i++) {
                    var opt = new Option(result[i].Text, result[i].Value);
                    $("#tempAssignTo").append(opt);
                }
                $('#tempAssignTo').removeAttr('disabled');
                $('.multiselect').removeAttr('disabled');

                //var statusVal = $("#Status").val();
                //if (statusVal == '5') {
                // $('#tempAssignTo').removeAttr('disabled');
                //$('.multiselect').removeAttr('disabled');
                //}
                //else {
                //    $('#tempAssignTo').attr('disabled', 'disabled');
                //    $('.multiselect').attr('disabled', 'disabled');
                //}
            }
            $(".dropdown-menu input").on("click", function () {
                var value = $(this).attr("value");
                var text = $(this).parent().text();
                if ((this.checked) == true) {
                    var opt = new Option(text, value);
                    //  $('#superVisor').append(opt);
                }
                else {
                    $("#superVisor option[value=" + value + "]").remove();
                }

            });
        });
    }
}

$("#tempAssignTo2").change(function () {
    var OtrwReq = $("#OtrwReuired").val();
    var numberOfChecked = $('input:checkbox:checked').length;
    if (OtrwReq == "") {
        $("#jobmsgDv").show();
        $("#jobmsgDv").html("Please select atleast 1 OTRW required field !");
        window.setTimeout(function () {
            $('#jobmsgDv').hide();
        }, 4000)
        $("#tempAssignTo2").multiselect('clearSelection');

        $('#superVisor').empty();
        $('#superVisor').append("<option value=''>(Select)</option>")
    }
    if (numberOfChecked > OtrwReq) {
        $("#jobmsgDv").show();
        $("#jobmsgDv").html("Maximum " + OtrwReq + " OTRW users allowed");
        window.setTimeout(function () {
            $('#jobmsgDv').hide();
        }, 4000)
        $("#tempAssignTo2").multiselect('clearSelection');

        $('#superVisor').empty();
        $('#superVisor').append("<option value=''>(Select)</option>")
    }
})
$("#OtrwReuired").change(function () {
    debugger;
    var OtrwReq = $("#OtrwReuired").val();
    var workType = $("#WorkType").val();
    if (workType > 0) {
        $('#tempAssignTo').removeAttr('disabled');
        $('.multiselect').removeAttr('disabled');
    }
    else {
        $("#jobmsgDv").show();
        $("#jobmsgDv").html("Please select work type first !");
        window.setTimeout(function () {
            $('#jobmsgDv').hide();
        }, 4000)
        $("#Status").val();
        return false;
    }
    $("#tempAssignTo2").multiselect('clearSelection');

    $('#superVisor').empty();
    $('#superVisor').append("<option value=''>(Select)</option>")
    if (OtrwReq > 1) {
        $('#superVisor').removeAttr('disabled');
    }
    else {
        //$('#superVisor').empty();
        $('#superVisor').html("");
        $('#superVisor').append("<option value=''>(Select)</option>")
        $('#superVisor').attr('disabled', 'disabled');
    }
})

$(".dropdown-menu input").on("click", function () {
    var value = $(this).attr("value");
    var text = $(this).parent().text();
    if ((this.checked) == true) {
        var opt = new Option(text, value);
        $('#superVisor').append(opt);
    }
    else {
        $("#superVisor option[value=" + value + "]").remove();
    }

});

$('#DateBooked').datepicker({
    beforeShowDay: $.datepicker.noWeekends,
    //minDate: 0,
    dateFormat: 'dd/mm/yy',
    onSelect: function (date) {
        var a = $('#DateBooked').val();
        var currentjob = $("#currentJobtype").val();
        var changeJobtype = $("#JobType").val();
        $("#JobCategory").val("Booked");
        if (currentjob == changeJobtype) {
            if ($('#IsJobStart').val() == "True") { $('#IsJobStart').val('true'); }
            if ($('#IsJobStart').val() == 'true' || $('#IsJobStart').val() == true) {
                alert('Date booked cannot be changed as job already been started.');
                $('#DateBooked').val(addjobs.getPrvdate1);
                return false;
            }
            else {
                if ($('.JobAssign-Table tbody tr').length > 0) {
                    if ($('.JobAssign-Table tbody tr').find('.AssignDateBooked').val() != a) {
                        $('.JobAssign-Table tbody tr').find('.AssignDateBooked').val(a);
                    }
                }
                else {
                    $(".DateChangepopup").modal('show');
                    $(".alertmsg").text("Are you sure you want to change the date of booking?");
                }
            }
        }

        $(document).on('click', ".btnYes", function () {


            $(".DateChangepopup").modal('hide');
            // $(".AssignDateBooked").val($("#DateBooked").val())
            //var datebooked = $('#DateBooked').val();
            //var actualvalue = $("#StartTime").val();
            //var s = $(this).val();
            //var time = actualvalue.split(' ')[1];
            //$("#StartTime").val(datebooked + " " + time)

        });
        $(document).on('click', ".btnNo", function () {
            var getPrvdate2 = addjobs.getPrvdate1;
            $('#DateBooked').val(getPrvdate2);
            if (getPrvdate2 == "") {
                $("#JobCategory").val("Stand By");
            }
            $(".DateChangepopup").modal('hide');
            return false;
        });
    }

});

$('#DateBooked').click(function () {
    var CurrentDate = $(this).val();
    addjobs.getPrvdate1 = CurrentDate;
});
$('#DateBooked').keyup(function () {
    var a = $('#DateBooked').val();
    if (a == "") {
        $("#JobCategory").val("Stand By");
    }
});

function callBackJob(jobid) {
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/_CallBackJobPopUp",
        data: {
            JobId: jobid
        },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowCallBack").empty();
            $("#divShowCallBack").html(data);
            $("#modalCallBackJobCalender").modal("show");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

$(document).on("click", "#UpdateCallBackJob", function () {
    var updateDate = $("#DateBookedCalender").val();
    updateDate = new Date(updateDate).toLocaleDateString();

    var jobId = $('#jobIdCal').val();
    var OTRWRequired = $("#OtrwRequired").val();
    if (updateDate == "" || updateDate == "Invalid Date") {
        $("#ErrorDv").empty();
        $(window).scrollTop(0);
        $("#ErrorDv").css("display", "block");
        $('#ErrorDv').css('color', 'red');
        $("#ErrorDv").html("<strong>Date field is required !</strong>");
        $("#ErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (OTRWRequired == "") {
        $("#ErrorDv").empty();
        $(window).scrollTop(0);
        $("#ErrorDv").css("display", "block");
        $('#ErrorDv').css('color', 'red');
        $("#ErrorDv").html("<strong>Please Select OTRW Required !</strong>");
        $("#ErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }

    $("#UpdateCallBackJob").val("Updating...");
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/UpdateCallBackJob",
        data: {
            JobId: jobId, UpdateDateBooked: updateDate, OTRWRequired: OTRWRequired
        },
        type: 'Get',
        async: false,
        success: function (data) {
            $(window).scrollTop(0);
            $("#modalCallBackJobCalender").hide();
            $("#jobmsgDv").show();
            $('#jobmsgDv').css('color', 'green');
            $('#jobmsgDv').html("Updated succesfully");
            window.setTimeout(function () {
                $('#jobmsgDv').hide();
                location.reload();
            }, 4000)
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).on('change', '#JobType', function (event) {
    var status = $("#Status").val();
    var value = $(this).val();
    var approveStatus = $("#approveStatus").val();
    $(".timepick").show();

    var currentjob = $("#currentJobtype").val();
    //if (currentjob != '' && currentjob != '0' && currentjob != 'undefined' && (status != '15' || status != '11')) {
    //    if (currentjob != "1" && value != '5' && value == 1) {
    //        $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
    //        return false;
    //    }
    //}
    //if (currentjob != '' && currentjob != '0' && currentjob != 'undefined' && (status == '15' || status == '11')) {
    //    if (currentjob != "1" && value != '5') {
    //        $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
    //        return false;
    //    }
    //}
    //if (currentjob == "5") {
    //    $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
    //    $("#jobmsgDv").show();
    //    $("#jobmsgDv").html("Call back type job cannot change any other job type!");
    //    window.setTimeout(function() {
    //        $('#jobmsgDv').hide();
    //    },
    //        4000)
    //    $(".timepick").hide();
    //    return false;
    //}
    //if (currentjob == "1") {
    //    if ((value == "2") && (approveStatus != "true" && approveStatus != "True")) {
    //        $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
    //        $("#jobmsgDv").show();
    //        $("#jobmsgDv").html("This type job cannot change this job type!");
    //        window.setTimeout(function () {
    //            $('#jobmsgDv').hide();
    //        },
    //            4000)
    //        $(".timepick").hide();
    //        return false;
    //    }
    //}
    //if (currentjob == "1") {
    //    if (value != "2" && value != "7"&& approveStatus != "True") {

    //        $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
    //        $("#jobmsgDv").show();
    //        $("#jobmsgDv").html("This type job cannot change this job type!");
    //        window.setTimeout(function() {
    //                $('#jobmsgDv').hide();
    //            },
    //            4000)
    //        $(".timepick").hide();
    //        return false;
    //    }
    //}
    if (value == '5' || value == '7') {
        $(".timepick").hide();
    }
    //if (status == '15') {
    if (value != '' && value != '0' && value != 'undefined') {
        BindJobTypeConfirmation(value, currentjob);      //jobType change confirmation
    }
    //}
});
function BindJobTypeConfirmation(changeJobtype, prevType) {
    $(".jobTypeConfirmationPopUp").modal('show');
    $(".alertmsgType").text("It will create a new job.do you want to proceed?");
    $(".btnTypeYesCnfrm").attr("JobType", changeJobtype);
    $(".btnTypeYesCnfrm").attr("prevJobType", prevType);
    $(".btnTypeNoCnfrm").attr("prevJobType", prevType);
    $(".btnTypeNoCnfrm").attr("JobType", changeJobtype);
    $(".modal-title").html("Job Type!");
    
}    //job type confirmation pop up

//job type change confirmation
$(document).on('click', ".btnTypeYesCnfrm", function () {
    var oldJobStatus = "";
    var empJobId = $("#empJobId").val();
    $("#changeJobType").val("True");
    var chngeJobType = $(this).attr("JobType");
    var currentjob = $("#currentJobtype").val();
    if ($('#JobType').val() == '2' && currentjob != $('#JobType').val()) {
        oldJobStatus = $('#Status').val();
            if ($('#DateBooked').val() == "") {
                $('.hdnStatus').val('13');
                $("#JobCategory").val("Stand By");
            }
            else if ($('#DateBooked').val() != "" && $('.JobAssign-Table tbody tr').length > 0) {
                $('.hdnStatus').val('5');
            }
            $('#Status').parent().parent().hide();
    }
    //Cehck if quote is approved and quote invoice or invoice is created
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/GetJobStatus",
        data: {
            JobId: empJobId, ChangeJobType: chngeJobType
        },
        type: 'Get',
        async: false,
        success: function (data) {
            if (data == 0 && chngeJobType != "1") {
                $('#JobType>option:eq("' + chngeJobType + '")').prop('selected', true);
                $("#DateBooked").val("");
                $("#JobCategory").val("Stand By");
                $(".jobTypeConfirmationPopUp").modal('hide');
                $(".JobAssign-Table tbody tr").detach();
            }
            else if (data == 1) {
                $(".jobTypeConfirmationPopUp").modal('hide');
                alert("New job  cannot be created untill Quote is appproved!")
                $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
                $('#Status').parent().parent().show();
                $('#Status').val(oldJobStatus);
            }
            else if (data == 0 && chngeJobType == "1") {
                $(".jobTypeConfirmationPopUp").modal('hide');
                alert("New Quote job cannot be created for existing Do job!")
                $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
                $('#Status').parent().parent().show();
                $('#Status').val(oldJobStatus);
            }
            else {
                $(".jobTypeConfirmationPopUp").modal('hide');
                alert("Job with same job type already exists!")
                $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
                $('#Status').parent().parent().show();
                $('#Status').val(oldJobStatus);
            }

        },
        error: function () {
            alert("something seems wrong");
        }
    });






});

//job type change confirmation cancel
$(document).on('click', ".btnTypeNoCnfrm", function () {
    $("#changeJobType").val("False");
    var prevJobType = $(this).attr("prevJobType");
    var chngeJobType = $(this).attr("JobType");

    $('#JobType>option:eq("' + chngeJobType + '")').prop('selected', true);
    $(".jobTypeConfirmationPopUp").modal('hide');
});

$(".assignJobUsers").click(function () {
    $('.document_collapse').toggle();
});

//add new row in table
$("#addNewLine").click(function () {
    //add new row for assign to
    var rowcount = $('.JobAssign-Table tr').length;
    debugger;
    var otrwRequired = $("#OtrwReuired").val();
    if (otrwRequired == "") {
        $("#jobmsgDv").empty();
        $(window).scrollTop(0);
        $("#jobmsgDv").css("display", "block");
        $('#jobmsgDv').css('color', 'red');
        $("#jobmsgDv").html("<strong>Please Select OTRW Required !</strong>");
        $("#jobmsgDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    if (parseInt(rowcount) > otrwRequired) {
        $("#jobmsgDv").show();
        $("#jobmsgDv").html("Maximum " + otrwRequired + " OTRW users allowed");
        window.setTimeout(function () {
            $('#jobmsgDv').hide();
        }, 4000)

        $('#superVisor').empty();
        $('#superVisor').append("<option value=''>(Select)</option>");
        return false;
    }

    if (parseInt(rowcount) > 0) {
        var lastrow = $('.JobAssign-Table tr:last');
        var jobAssign = $(lastrow).find('.ddljobsAssignlist').val();
        var jobTime = $(lastrow).find('.AssignStartTime').val();
        var jobDate = $(lastrow).find('.AssignDateBooked').val();
        var workType = $("#WorkType").val();

        if (workType == 'undefined' || workType == null || workType == '' || workType == '0') {
            alert("Please select work type first.");
            return;
        }

        if (parseInt(rowcount) > 1) {
            if (jobAssign == 'undefined' || jobAssign == '' || jobAssign == null) {
                alert("Please select assign to.");
                return;
            }

            else if (jobTime == 'undefined' || jobTime == null || jobTime == '') {
                alert("Please select start time.");
                return;
            }

            else if (workType == 'undefined' || workType == null || jobTime == '') {
                alert("Please select start time.");
                return;
            }

            else {
                BindTableOTRW(workType);
            }
        }
        else {

            BindTableOTRW(workType);
        }
    }

});

//save table data in temp table
function SaveFormData() //Save Assign job info
{
    var tabledata = null;
    var itemcount = $('.JobAssign-Table tr').length;

    if (itemcount > 1) {
        $('.JobAssign-Table tr:gt(0)').find('.otrwnotes').val();
        tabledata = $('.JobAssign-Table tr:gt(0)').map(function () {
            return {

                Id: $(this).parent().find('.assignUserId').attr('Value'),
                AssignTo: $(this).find('.ddljobsAssignlist').val(),
                AssignStartTime: $(this).find('.AssignStartTime').val(),
                AssignDateBooked: $(this).find('.AssignDateBooked').val(),
                DateBooked: $('#DateBooked').val(),
                OTRWNotes: $(this).find('.otrwnotes').html(),
                OTRWStatus: $(this).find('.otrwjobstatus').val(),
            };
        }).get();
        $.ajax({
            type: 'POST',
            url: common.SitePath + "/Employee/CustomerJob/InsertAssignToForJob",
            data: JSON.stringify({ assignJobList: tabledata }),
            contentType: 'application/json; charset=utf-8',
            traditional: true,
            async: false,
            success: function (data) {

            },
        });
    }
}

//Delete assign job
$(document).on("click", ".deleteRow", function () {
    //var assignid = $(this).closest('tr').find("td:first").html();
    var assignid = $(this).parent().find('.assignUserId').attr('Value');
    var confirmdelete = confirm('Are you sure want to delete the time slot?');
    if (confirmdelete) {

        if (assignid == null || assignid == 'undefined' || assignid == '') {
            var id = $(this).closest('tr').find("td:first");
            $(this).closest('tr').remove();
            $('#superVisor').empty();

            assignUser = ""
            assignUsertext = "Select";
            var opts = new Option(assignUsertext, assignUser);
            $('#superVisor').append(opts);
            $(".ddljobsAssignlist").each(function () {
                assignUser = $(this).val();
                assignUsertext = $("option:selected", this).text();

                var checkSuperVisorExist = 1;
                $("#superVisor option").each(function () {
                    superUser = $(this).val();
                    if (assignUser == superUser) {
                        checkSuperVisorExist = 0;
                    }
                })
                if (assignUsertext != "Select" && checkSuperVisorExist == 1) {
                    var opt = new Option(assignUsertext, assignUser);
                    $('#superVisor').append(opt);
                }
            });

            return;
        }
        else {
            assignid = $(this).parent().find('.assignUserId').attr('Value'); //value of Assign To id
        }

        if (assignid != null && assignid != 'undefined' && assignid != '') {
            $(this).closest('tr').remove();

            $.ajax({
                url: common.SitePath + "/Employee/CustomerJob/DeleteAssignJob",
                data: { assignUserId: assignid },
                type: 'Get',
                success: function (data) {

                },
                error: function () {
                    alert("something seems wrong");
                }
            });
        }
    }
});

//change assign user then get time value
$(document).on("change", ".ddljobsAssignlist", function () {
    var dateBook = $("#DateBooked").val();
    var $CurrentRow = $(this).parent().parent();          //get current row
    var assignUser = $(this).val();
    var currrentRowDate = $(this).parent().parent().find('.AssignDateBooked').val();
    var assignUsertext = $("option:selected", this).text();
    var assignIndex = $(this).closest("tr").index();     //get current row index
    var $tr = $(this).closest('tr');
    var $otrw = $tr.find('.otrwnotes');
    var $otrjobstatus = $tr.find('.otrwjobstatus');
    //get value of supervisor
    var isexist = 0;
    var jobId = $("#empJobId").val();
    //$("#superVisor option").each(function () {
    $('table.JobAssign-Table tr').each(function () {
        //user = $(this).val();
        index = $(this).closest("tr").index();
        user = $(this).parent().parent().find('.ddljobsAssignlist').val();
        datebook = $(this).parent().parent().find('.AssignDateBooked').val();
        if (assignUser == user && datebook == currrentRowDate && index != assignIndex) {
            $(window).scrollTop(0);
            $("#jobmsgDv").css("display", "block");
            $('#jobmsgDv').css('color', 'red');
            $("#jobmsgDv").html("<strong>Job is already assigned to the given user!</strong>");
            $("#jobmsgDv").delay(4000).fadeOut(function () {
            });
            isexist = 1;
            $CurrentRow.remove();
            addjobs.BindSupervisoronRefresh();
            return false;
        }
    });

    if (isexist != 1) {

        addjobs.BindSupervisoronRefresh();

        // var tableRows = $(".JobAssign-Table tbody tr");    //get table all rows
        //tableRows.each(function (n) {                     //n= index
        //    var user = $(this).find(".ddljobsAssignlist").val();
        //    if (user == assignUser && assignIndex != n) {
        //        $CurrentRow.remove();
        //        $("#jobmsgDv").empty();
        //        $(window).scrollTop(0);
        //        $("#jobmsgDv").css("display", "block");
        //        $('#jobmsgDv').css('color', 'red');
        //        $("#jobmsgDv").html("<strong>Job is already assigned to the given user!</strong>");
        //        $("#jobmsgDv").delay(4000).fadeOut(function () {
        //        });
        //        return false;
        //    }
        //});

        var $TimePicker = $(this).parent().parent().find('.AssignStartTime');
        $TimePicker.timepicker("");

        $TimePicker.timepicker().unbind();
        if (assignUser == "") {
            return false;
        }

        if (currrentRowDate == "") {
            $("#jobmsgDv").empty();
            $(window).scrollTop(0);
            $("#jobmsgDv").css("display", "block");
            $("#jobmsgDv").css('color', "red");
            $("#jobmsgDv").html("<strong>Please Select Date Of Assignment !</strong>");
            $("#jobmsgDv").delay(4000).fadeOut(function () {
            });
            return false;

        } else {
            $.ajax({
                url: common.SitePath + "/Employee/CustomerJob/GetJobAssignEndTime",
                data: {
                    assignTo: assignUser, jobDateBooked: currrentRowDate, JobId: jobId,
                },
                type: "Get",
                success: function (data) {
                    debugger;
                    var timePickerStart = data.date;
                    var strTime = "";
                    if (data.OTRWJobstatus != "")
                    {
                        $otrjobstatus.html(data.OTRWJobstatus);
                    }
                    if (data.OTRWNotes != "") {

                        $otrw.html(data.OTRWNotes);
                    }
                    if (data.error == "0") {   //if error
                        alert(data.msg);
                    }
                    else if (timePickerStart == null || timePickerStart == "") {
                        strTime = "06:00AM";
                    } else {
                        var time = timePickerStart.split(' ')[1];
                        var hours = time.split(':')[0];
                        if (hours == "20" || hours > 20) {
                            return false;
                        }
                        var minutes = time.split(':')[1];
                        var ampm = hours >= 12 ? 'PM' : 'AM';
                        hours = hours % 12;
                        hours = hours ? hours : 12; // the hour '0' should be '12'
                        hours = hours < 10 ? '0' + hours : hours;
                        if (minutes > "00" && minutes < "29") {
                            minutes = "30";
                        } else if (minutes > "31" && minutes < "59") {
                            hours = parseInt(hours) + 1;
                            minutes = "00";
                        }
                        strTime = hours + ':' + minutes + ' ' + ampm;
                    }
                    $TimePicker.timepicker({    //timepicker 
                        'timeFormat': 'h:i A',
                        minTime: strTime,
                        maxTime: '08:00PM',
                        step: 30

                    });
                    $TimePicker.timepicker('option', 'minTime', strTime);  //bind min time again 
                }
            });
        }

    }
});

function BindTableOTRW(workType) {
    var dateBooked = $("#DateBooked").val();

    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/GetOTRWLIST",
        data: {
            WorkType: workType
        },
        type: 'Post',
        async: false,
        success: function (data) {
            var otrwList = jQuery.parseJSON(data.data);

            var otrwMethod = "<select class='ddljobsAssignlist form-control Create_Customer' ><option value=''>Select</option>";   //bind OTRW
            for (var i = 0; i < otrwList.length; i++) {
                otrwMethod = otrwMethod + '<option value=' + otrwList[i]["Value"] + '>' + otrwList[i]["Text"] + '</option>';
            }
            otrwMethod = otrwMethod + "</select>";
            var row = "<tr>";
            row = row + "<td class='AssignId' style='display:none'></td>\
                            <td id='' class=''><input type='text' class='AssignDateBooked form-control Create_Customer' value='" + dateBooked + "'></input></td>\
                            <td>" + otrwMethod + "</td>\
                            <td id='getStartTime' class=''><input type='text' class='AssignStartTime timepicker form-control Create_Customer'></input></td>\
                            <td class='deleteRow' style='color:blue'>x</td>\
                           <td class='otrwnotes' style='display:none'></td>\
                            <td class='otrwjobstatus' style='display:none'></td>";
            $(".JobAssign-Table tbody").append(row);

            $('.AssignDateBooked').datepicker({
                dateFormat: 'dd/mm/yy'
            });

            //$('.AssignStartTime').timepicker({
            //    'timeFormat': 'h:i A',
            //    minTime: '06:00am',
            //    maxTime: '08:00pm',
            //    step: 30
            //});
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}


//assign date booked change than select otrw user again
$(document).on("change", ".AssignDateBooked", function () {
    var assignUserId = $(this).parent().parent().find('.assignUserId').attr('Value');
    if (assignUserId == "" || assignUserId == undefined || assignUserId == 'undefined') {
        $(this).parent().parent().find('.ddljobsAssignlist').val("");
        $(this).parent().parent().find('.AssignStartTime ').val("");
    }
});








