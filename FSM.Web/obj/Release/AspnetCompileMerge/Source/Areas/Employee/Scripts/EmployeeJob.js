$(document).ready(function () {
    $('#DateBooked').datepicker({
        minDate: 0
    });
    $(document).ready(function () {
        var availableTags = [];
        $.ajax({
            url: "/Employee/Job/GetCustomerListforauto",
            data: {},
            type: 'GET',
            async: false,
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                for (i = 0; i < data.length; i++) {
                    availableTags.push(response[i]["LastName"]);
                }
            },
            error: function () {
            }
        })
        $("#tags").autocomplete({
            source: availableTags
        });
        $(".custom-combobox-input ").attr("placeholder", "Search Customer");
        $("#btnaddjobs").click(function () {
            var imgVal = $('#jobfileuploader').val();
            if (imgVal != '') {
                var size = $('#jobfileuploader')[0].files[0].size;
                var Uploader = $("input:file")[0].files.length;
                if (Uploader > 5) {
                    alert("Maximum 5 documents uploaded.");
                    return false;
                }
                else if (size > 2097152) {
                    alert("Maximum file size 2 MB");
                    return false;
                }
                else {
                    return true;
                }
            }
        })
        //check for support job
        var jobtype = $("#JobType").val();
        if (jobtype == 3 || jobtype == "Support") {
            $(".jobslist").css("display", "block");
            $(".ddlsitedetail").attr("disabled", "disabled");
            $(".custom-combobox-input ").attr("readonly", "readonly");
        }
        else {
            $(".jobslist").css("display", "none");
            $(".ddlsitedetail").attr("disabled", false);
            $(".custom-combobox-input ").removeAttr("readonly");
        }
    })
    $("#btnaddPurchaseOrder").click(function () {
        $("#jobpurchaseorder").modal('show');
    })
    $("#EstimatedHours").keypress(function (evt) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    });
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
    $("#combobox").combobox();
    $("#combobox").combobox({
        select: function (event, ui) {
            var s = $(this).val();
            Bindsitedetail(s);
        }
    });
    $("#toggle").on("click", function () {
        $("#combobox").toggle();
    });
});
//change event of auto complete list
$(document).on('change', '.ddlcustlastname', function () {
    var custid = $(this).val();
    $.ajax({
        url: common.SitePath + "/Employee/Job/GetSiteDetailById",
        data: { CustomerGeneralinfoid: custid },
        type: 'Get',
        async: false,
        success: function (data) {
            var response = jQuery.parseJSON(data.list);
            $(".ddlsitedetail").html("");
            var site = "<option val='0'>(Select)</option>";
            for (i = 0; i < data.length; i++) {
                $(".ddlsitedetail").append(
	                    $('<option></option>').val(response[i]["SitesId"]).html(response[i]["SiteName"])
                        );
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
})
//validation on the requried fields
function checkvalidation() {
    var custval = $(".ddlcustlastname").val();
    var siteid = $(".ddlsitedetail").val();
    var JobType = $("#JobType").val();
    var LinkedJobId = $("#LinkedJobId").val();
    var DateBooked = $("#DateBooked").val();
    $("#Customernameerror").css("display", "none").html("");
    $("#CustomerSiteerror").text("").css("display", "none");

    if (JobType == 3 || JobType == 'Support') {
        if (LinkedJobId == "" || LinkedJobId == undefined || LinkedJobId == '0') {
            $("#Linkjoberror").text("please select Link Job").css("color", "red").css("display", "block");
            return false;
        }
    }
    else if (custval == "" || custval == undefined) {
        $("#Linkjoberror").css("display", "none").html("");
        $("#Customernameerror").text("please select file name").css("color", "red").css("display", "block");
        return false;
    }
    else if (siteid == "" || siteid == undefined || siteid == '0') {
        $("#Linkjoberror").css("display", "none").html("");
        $("#Customernameerror").css("display", "none").html("");
        $("#CustomerSiteerror").text("please select customer Site").css("color", "red").css("display", "block");
        return false;
    }

    else if (DateBooked == "" || DateBooked == undefined || DateBooked == '0' || DateBooked == 'NaN') {
        $("#Linkjoberror").css("display", "none").html("");
        $("#Customernameerror").css("display", "none").html("");
        $("#CustomerSiteerror").css("display", "none").html("");
        $(".BookeddateError").text("please select Booked Date").css("color", "red").css("display", "block");
        return false;
    }
    else {
        $(".BookeddateError").text("").css("display", "none");
        return true;
    }

}

//binding of sites
function Bindsitedetail(custid) {
    $.ajax({
        url: common.SitePath + "/Employee/Job/GetSiteDetailById",
        data: { CustomerGeneralinfoid: custid },
        type: 'Get',
        async: false,
        success: function (data) {
            var response = jQuery.parseJSON(data.list);
            $(".ddlsitedetail").html("");
            var site = "<option val='0'>(Select)</option>";
            for (i = 0; i < data.length; i++) {
                $(".ddlsitedetail").append(
                        $('<option></option>').val(response[i]["SitesId"]).html(response[i]["SiteName"])
                        );
            }
            $('.ddlsitedetail').prepend('<option value="0" selected="selected">(Select)</option>');
        },
        error: function () {
            alert("something seems wrong");
        }
    });


}
//jobtype change event
$(document).on("change", "#JobType", function () {
    var jobtype = $(this).val();
    if (jobtype == 3 || jobtype == "Support") {
        $(".jobslist").css("display", "block");
        $(".ddlsitedetail").attr("disabled", "disabled");
        $(".custom-combobox-input ").attr("readonly", "readonly");
    }
    else {
        $(".jobslist").css("display", "none");
        $(".ddlsitedetail").attr("disabled", false);
        $(".custom-combobox-input ").removeAttr("readonly");
    }
})
//joblist change event
$(document).on("change", ".ddljobslist", function () {
    var jobId = $(this).val();
    $.ajax({
        url: common.SitePath + "/Employee/Job/GetJobDetailByJobID",
        data: { id: jobId },
        type: 'GET',
        async: false,
        success: function (data) {
            var response = jQuery.parseJSON(data);
            $('.ddlcustlastname').val(response.CustomerGeneralInfoId);
            $(".custom-combobox-input ").val(response.CustomerLastName);
            $(".custom-combobox-input ").attr("readonly", "readonly");
            Bindsitedetail(response.CustomerGeneralInfoId);
            $(".ddlsitedetail").val(response.SiteId);
            $("#SupportjobSiteId").val(response.SiteId);
            $(".ddlsitedetail").attr("disabled", "disabled");
        }
    })
})

//View Job Site Details
$(document).on("click", "#btnSitedetail", function () {
    var SiteId = $(".ddlsitedetail ").val();
    if (SiteId != '' && SiteId != 'undefined' && SiteId != "" && SiteId!='00000000-0000-0000-0000-000000000000') {
        $.ajax({
            url: common.SitePath + "/Employee/Job/_ViewjobSiteDetail",
            data: { SiteId: SiteId },
            type: 'GET',
            async: false,
            success: function (data) {
                $("#divShowSiteDetaildiv").html(data);
                $("#modalSitedetail").modal("show");
                $(".bottom_btn").show();
            }
        })
    }
    else {
        $(".commonpopup").modal('show');
        $(".alertmsg").text("Please select Site. ");
        $(".modal-title").html("Site Info !");
        $(".bottom_btn").hide();
    }
})
//download button
$(document).on('click', '.btndownloadsitedoc', function () {
    var documentid = $(this).attr("documentid");
    $.ajax({
        url: common.SitePath + "/Employee/Job/DownloadSiteDocuments",
        data: { SiteDocid: documentid },
        type: 'GET',
        async: false,
        success: function (data) {
            window.open(common.SitePath + data);
        },
        error: function () {
            alert("something seems wrong!")
        }
    })
})

