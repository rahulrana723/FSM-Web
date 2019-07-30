
$(document).ready(function () {
    $(".ddlmultiselect").multiselect();

    //$('#DateBooked').datepicker({
    //    dateFormat: 'dd/mm/yy'
    //});
    $('#DateBookedJob').datepicker({ minDate: 0, dateFormat: 'dd/mm/yy' });
    $('#Status').attr('disabled', 'disabled');

    //tinymce editor
    tinymce.init({
        selector: 'textarea',
        width: "200",
        height: "100",
        theme: 'modern',
        plugins: [
      'advlist autolink lists link image charmap print preview hr anchor pagebreak',
  'searchreplace wordcount visualblocks visualchars code fullscreen',
'insertdatetime media nonbreaking save table contextmenu directionality',
'emoticons template paste textcolor colorpicker textpattern imagetools codesample'
        ],
        toolbar1: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image',
        toolbar2: 'print preview media | forecolor backcolor emoticons | codesample',
        image_advtab: true,
        templates: [
    { title: 'Test template 1', content: 'Test 1' },
    {
        title: 'Test template 2', content: 'Test 2'
    }
        ],

    });
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
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
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
        }
    });
    $("#toggle").on("click", function () {
        $("#CustomerInfoId").toggle();
    });

    $(".custom-combobox-input ").attr("placeholder", "Search Customer");

    if (FSM.JobType == 'Support') {
        $('#LinkJobId').removeAttr('disabled');
    }
    else {
        $('#LinkJobId').attr('disabled', 'disabled');
    }

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
    if (FSM.JobStatus == 'Assigned') {
        $('#tempAssignTo').removeAttr('disabled');
    }
    else {
        $('#tempAssignTo').attr('disabled', 'disabled');
    }

})

function Bindsitedetail(CustomerInfoId) {
    var data = { CustomerInfoId: CustomerInfoId };

    $.get(FSM.FillSiteListUrl, data, function (result) {
        $('.fillsitecombo').empty();
        $('.fillsitecombo').html(result);
    });
}

$(document).on("click", ".savejob", function (event) {

    event.stopImmediatePropagation();

    var isSiteDisable = document.getElementById("tempSiteId").hasAttribute("disabled");
    $('#tempSiteId').removeAttr("disabled");
    var isAssignedToDisable = document.getElementById("tempAssignTo").hasAttribute("disabled");
    $('#tempAssignTo').removeAttr("disabled");
    var JobNotes = document.getElementById('JobNotes_ifr').contentWindow.document.body.innerHTML;
    var OperationNotes = document.getElementById('OperationNotes_ifr').contentWindow.document.body.innerHTML;
    var Status = $('#Status').removeAttr("disabled");

    var formdata = new FormData($('#frmSaveInvoiceJob').get(0));
    formdata.append("Job_Notes", JobNotes);
    formdata.append("Operation_Notes", OperationNotes);
    $.ajax({
        url: $('#frmSaveInvoiceJob').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            $('#Status').attr('disabled', 'disabled');
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
                    }, 4000)
                }
            }
            else {
                $(window).scrollTop(0);
                $('.jobalert').empty();

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

});

$(document).on("change", "#JobType", function () {
    var cmbVal = $(this).val();
    Bindsitedetail();

    // 3 represents supportjob
    if (parseInt(cmbVal) == 3) {
        $('#LinkJobId').removeAttr('disabled');
        $('#CustomerInfoId').val('');
        $(".custom-combobox-input").val('');
        $(".custom-combobox-input").attr("readonly", "readonly");
        $("#tempSiteId").val('');
        $(".cmbsitelist").val('');
        $('#tempSiteId').attr("disabled", "disabled");

    }
    else {
        $('#LinkJobId').attr('disabled', 'disabled');
        $('#LinkJobId').val('');
        $('#CustomerInfoId').val('');
        $(".custom-combobox-input").val('');
        $(".custom-combobox-input").removeAttr("readonly");
        $("#tempSiteId").val('');
        $(".cmbsitelist").val('');
        $('#tempSiteId').removeAttr("disabled");
    }
});

$(document).on("change", "#LinkJobId", function () {
    var cmbVal = $(this).val();
    var data = { LinkJobId: cmbVal };

    $.get(FSM.GetCustomerUrl, data, function (result) {
        if (result.Customer != null || result.Customer != undefined) {
            $('#CustomerInfoId').val(result.Customer.CustomerGeneralInfoId);
            $(".custom-combobox-input ").val(result.Customer.CustomerLastName);
            $(".custom-combobox-input ").attr("readonly", "readonly");
            Bindsitedetail(result.Customer.CustomerGeneralInfoId);
            window.setTimeout(function () {
                $('#tempSiteId').val(result.Customer.SiteId);
                $(".cmbsitelist").val(result.Customer.SiteId);
                $('#tempSiteId').attr("disabled", "disabled");
            }, 1000)
        }
        else {
            window.setTimeout(function () {
                $('#CustomerInfoId').val('');
                $(".custom-combobox-input").val('');
                $(".custom-combobox-input").attr("readonly", "readonly");
                $('#tempSiteId').val('');
                $(".cmbsitelist").val('');
                $('#tempSiteId').attr("disabled", "disabled");
            }, 1000)
        }

    });

});

$(document).on('click', '.viewJobDocs', function (event) {
    event.preventDefault();
    var jobid = $(this).attr("Jobid");
    var data = { JobId: jobid };

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
        $("#invoiceModal").modal('show');
    });

});

$(document).on('click', '.btndeletedoc', function () {
    var documentid = $(this).attr("data-attr");
    var jobid = $(this).attr("data-jobid");
    var data = { DocId: documentid, JobId: jobid };

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
                $("#invoiceModal").modal('show');
            },
            error: function () {
                alert("something seems wrong!")
            }
        })
    }
})

$(document).on('click', '.btndownload', function () {
    var documentid = $(this).attr("data-attr");
    var data = { Id: documentid };

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
    if (statusVal == '5') {
        $('#tempAssignTo').removeAttr('disabled');
    }
    else {
        $('#tempAssignTo').attr('disabled', 'disabled');
    }
});