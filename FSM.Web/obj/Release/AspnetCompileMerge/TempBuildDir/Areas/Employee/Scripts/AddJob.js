var addjobs = {
    getPrvdate1: "",
    FSM1: ""
}


$(document).ready(function () {
    //$('#tempAssignTo2 option').prop("disabled", true);
    // disable OTRWjobNotes
    var approveStatus = $("#approveStatus").val();

    $("#JobType option[value='5']").hide();   //hide JobType Value(Call-Back)
    $("#JobType option[value='6']").hide();  //hide JobType Value(Re-Quote)
    $("#JobType option[value='7']").hide();  //hide JobType Value(Check Measure)

    var currentjobtype = $("#currentJobtype").val();   //get Current job type
    var status = $("#Status").val();    //get Currnet Status

    if (currentjobtype == "1" && status == "15") {
        $("#JobType option[value='3']").hide();
        $("#JobType option[value='4']").hide();

        $("#JobType option[value='7']").show();
    }
    else if (currentjobtype == "6" && status == "15") {
        $("#JobType option[value='1']").show();
        $("#JobType option[value='3']").hide();
        $("#JobType option[value='4']").hide();

        $("#JobType option[value='7']").show();
    }
    else if (currentjobtype == "1") {
        $("#JobType option[value='3']").hide();
        $("#JobType option[value='4']").hide();

        $("#JobType option[value='6']").show();
    }
    else if (currentjobtype == "7" && approveStatus == "True") {
        $("#JobType option[value='1']").hide();
        $("#JobType option[value='3']").hide();
        $("#JobType option[value='4']").hide();
        $("#JobType option[value='5']").hide();
        $("#JobType option[value='6']").hide();

        $("#JobType option[value='7']").show();
    }
    else if ((currentjobtype == "2" && status == "15") || currentjobtype == "5") {
        $("#JobType option[value='5']").show();
        if (currentjobtype == "5") {
            $(".timepick").hide();
        }
    }
    else if (currentjobtype == "6" || currentjobtype == "7") {
        //$("#JobType option[value='1']").hide();
        //$("#JobType option[value='3']").hide();
        //$("#JobType option[value='4']").hide();

        $("#JobType option[value='6']").show();
        $("#JobType option[value='7']").show();

        $(".timepick").hide();
    }


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
    $('#StartTime').timepicker({
        'timeFormat': 'h:i A',
        minTime: '06:00am',
        maxTime: '08:00pm', step: 30
    });

    var actualvalue = $("#StartTime").val();
    if (actualvalue != '' && actualvalue != 'undefined' && actualvalue != null) {
        var time = actualvalue.split(' ')[1];
        var hours = time.split(':')[0];
        var minutes = time.split(':')[1];
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        //  minutes = minutes < 10 ? '0' + minutes : minutes;
        hours = hours < 10 ? '0' + hours : hours;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        $("#StartTime").val(strTime);
    }
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
        if (genearlInfoId == "")
        {
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
        $(".OtrwReqDiv").hide();
        $("#OtrwReuired").val("");
        $("#tempAssignTo2").multiselect('clearSelection');
        $('#tempAssignTo').removeAttr('disabled');
        $(".ddlassign1").show();
    }
    else if (FSM.JobStatus == 'Completed') {
        $('#DateBooked').attr('disabled', 'disabled');
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
        var JobStatusUrl = common.SitePath + "/Employee/CustomerJob/ChangeJobApprovedStatus";
        var jobid = $(this).attr('Empjobid');
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
            if (result.jobstatus == "[ Approved ]") {
                $('#approveStatus').val(true);
            }
            else {
                $('#approveStatus').val(false);
            }

        })
    .fail(function () {
        alert("error");
    });
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
                $(".alertmsg").text("Are you sure you want to proceed without selecting date?");
                $(document).on('click', ".btnYesCnfrm", function () {
                    $(".NoDatePopUp").modal('hide');
                    SaveJob(event);
                });
                $(document).on('click', ".btnNoCnfrm", function () {
                    $(".NoDatePopUp").modal('hide');
                });
            }
            else {
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
    var isAssignedToDisable = document.getElementById("tempAssignTo").hasAttribute("disabled");
    $('#tempAssignTo').removeAttr("disabled");
    //var JobNotes = document.getElementById('JobNotes_ifr').contentWindow.document.body.innerHTML;
    //var OperationNotes = document.getElementById('OperationNotes_ifr').contentWindow.document.body.innerHTML;

    var JobNotes = $("#JobNotes").val();
    var OperationNotes = $('#OperationNotes').val();
    var formdata = new FormData($('#frmSaveJob').get(0));
    if (JobNotes != "<p>&nbsp;<br></p>") {
        formdata.append("Job_Notes", JobNotes);
    }
    if (OperationNotes != "<p>&nbsp;<br></p>") {
        formdata.append("Operation_Notes", OperationNotes);
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
            if (isAssignedToDisable) {
                $('#tempAssignTo').attr("disabled", "disabled");
            }
            else {
                $('#tempAssignTo').removeAttr("disabled");
            }
            if (result.status == "saved") {
                if (result.SavedId != '' && result.SavedId != undefined) {
                    FSM.SavedId = result.SavedId;
                    window.location.href = FSM.SaveJobInfoUrl + "?id=" + result.SavedId + "&showmsg=Yes";
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
            }
            else {
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
                }, 3000)
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

$(document).on('change', '#WorkType', function () {
    var workType = $(this).val();
    var FSM1 = 0;
    if (workType != "") {
        BindOTRWWithWorkType(workType, FSM1);
        $('#superVisor').empty();
        $('#superVisor').append("<option value=''>(Select)</option>")
    }

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
                    $('#superVisor').append(opt);
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
    minDate: 0,
    dateFormat: 'dd/mm/yy',
    onSelect: function (date) {
        var a = $('#DateBooked').val();
        $("#JobCategory").val("Booked");
        $(".DateChangepopup").modal('show');
        $(".alertmsg").text("Are you sure you want to change the date of booking?");

        $(document).on('click', ".btnYes", function () {
            $(".DateChangepopup").modal('hide');
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
    if (currentjob != '' && currentjob != '0' && currentjob != 'undefined' && (status == '15' || status == '11')) {
        if (currentjob != "1" && value != '5') {
            $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
        }
    }
    //if (value == "5") {
    //    $('#JobType>option:eq("' + value + '")').prop('selected', false);
    //    $("#jobmsgDv").show();
    //    $("#jobmsgDv").html("Call back type job not created !");
    //    window.setTimeout(function () {
    //        $('#jobmsgDv').hide();
    //    }, 4000)
    //}
    if (currentjob == "5") {
        $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
        $("#jobmsgDv").show();
        $("#jobmsgDv").html("Call back type job cannot change any other job type!");
        window.setTimeout(function () {
            $('#jobmsgDv').hide();
        }, 4000)
        $(".timepick").hide();
    }
    if (currentjob == "7") {
        if (value != "2" && value != "7") {

            $('#JobType>option:eq("' + currentjob + '")').prop('selected', true);
            $("#jobmsgDv").show();
            $("#jobmsgDv").html("This type job cannot change this job type!");
            window.setTimeout(function () {
                $('#jobmsgDv').hide();
            }, 4000)
            $(".timepick").hide();
        }
    }
    if (value == '5' || value == '7') {
        $(".timepick").hide();
    }
})


