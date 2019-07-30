var s = {
    jobid: 0
}

$(document).ready(function () {
    $("#EndDate,#StartDate").datepicker({
    });
    $('#ddPageSize').val(FSM.SelectedVal);
    if (parseInt(FSM.HasGridRecords) > 0) {
        if ($(".pagination").length > 0) {
            $('#ddPageSize').parent().css('margin-top', '-72px');
        }
        else {
            $('#ddPageSize').parent().css('margin-top', '8px');
        }
    }
    else {
        $('#ddPageSize').parent().css('margin-top', '0px');
    }
    if (FSM.Message != "" && FSM.Message != 0) {
        $(".jobalert").css("display", "block");
        if (FSM.Message == "1") {
            $(".jobalert").html("<strong>Record added Successfully!</strong>");
        }
        else if (FSM.Message == "2") {
            $(".jobalert").html("<strong>Record Updated Successfully!</strong>");
        }
        else if (FSM.Message == "3") {
            $(".jobalert").html("<strong>Record Deleted Successfully!</strong>");
        }
        FSM.Message = "";
        $(".jobalert").delay(2000).fadeOut();
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
            $("#CustomerInfoId").empty();
            var SearchTerm = request.term;
            var customerlist = [];

            // converting json to array
            var customerlist = jQuery.parseJSON(FSM.CustomerListVal);

            // filtering values based on some value
            customerlist = jQuery.grep(customerlist, function (item) {
                var text = item.Text;
                if (text != '' && text != undefined) {
                    var index = text.toLowerCase().indexOf(SearchTerm.toLowerCase());
                    return index > -1 ? true : false;
                }
            });

            // taking first twenty values if greater than 10
            customerlist = customerlist.length > 10 ? customerlist.slice(0, 20) : customerlist;

            // binding values again to combo
            for (var i = 0; i < customerlist.length; i++) {
                $("#CustomerInfoId").append("<option value='" + customerlist[i].Value + "'>" + customerlist[i].Text + "</option>");
            }

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

    $(".custom-combobox-input ").attr("placeholder", "Search Customer");

    $(".custom-combobox-input ").val(FSM.CustomerName);
    // making sure custom serach combo load cleanly
    $(".customer_head_tab").removeClass("job-customer-visible");

});

function ViewEmployeejobdocuments(jid) {
    $.ajax({
        url: common.SitePath + "/Employee/Job/ViewJobDocuments",
        data: { id: jid },
        type: 'POST',
        async: false,
        success: function (data) {
            $(".modaldoc").empty();
            var response = jQuery.parseJSON(data.list);
            var doc = "";
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    doc = doc + "<p class='docjobName',style=float: left;width: 66% ;padding: 5px 0 0 0;>" + response[i]["DocName"] + "</p><button class='btndownload btn-success' data-attr='" + response[i]["Id"] + "' >Download</button><button class='btndeletedoc btn-danger deldoc' data-attr='" + response[i]["Id"] + "'>Delete</button></br>";
                }
            }
            else {
                doc = "<p>No documents uploaded regarding the job!</p>";
            }
            $(".modaldoc").append(doc);
            $("#myModal").modal('show');

        },
        error: function () {
            alert("something seems wrong");
        }
    })

}

$(document).on('click', '.btnviewdoc', function () {
    var jid = $(this).attr("Jobid");
    s.jobid = jid;
    ViewEmployeejobdocuments(jid);

});


$(document).on('click', '.btndownload', function () {
    var documentid = $(this).attr("data-attr");
    $.ajax({
        url: common.SitePath + "/Employee/Job/DownloadDocuments",
        data: { docid: documentid },
        type: 'POST',
        async: false,
        success: function (data) {
            window.location = common.SitePath + "/Employee/Job/DownloadDocuments?docid=" + documentid;
        },
        error: function () {
            alert("something seems wrong!")
        }
    })
})

$(document).on('click', '.btndeletedoc', function () {

    var documentid = $(this).attr("data-attr");
    var check = confirm("Are you sure to delete the doc?");
    if (check) {
        $.ajax({
            url: common.SitePath + "/Employee/Job/DeletejobDocumentByDocId",
            data: { docid: documentid },
            type: 'GET',
            async: false,
            success: function (data) {
                ViewEmployeejobdocuments(s.jobid);
            },
            error: function () {
                alert("something seems wrong!")
            }

        })
    }
})


$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    var keyword = $('#Keyword').val();
    var jobtype = $('#JobType').val();
    window.location.href = FSM.URL + "?page_size=" + page_size + "&keyword=" + keyword + "&Jobtype=" + jobtype;
});


$('.grid-header-title a').on('click', function () {

    var elementparam = $(this).attr('href');

    var index = elementparam.indexOf("grid-column");

    if (index > 1) {
        var paramarray = elementparam.split("&");

        elementparam = '?';
        $.each(paramarray, function (i, item) {

            if (item.indexOf("grid-column") > -1 || item.indexOf("grid-dir") > -1) {
                elementparam = elementparam + item + '&';
            }
        });

        elementparam = elementparam.substring(0, (elementparam.length - 1));
    }

    var jobtype = $('#JobType').val();
    var startDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();
    var SOL = $('#SOL').is(':checked');
    var LNC = $('#LNC').is(':checked');
    var UnsentInv = $('#UnsentInv').is(':checked');
    var Contracted = $('#Contracted').is(':checked');
    var keyword = $('#Keyword').val();

    var Jobtype = $('#JobType').val();
    if (Jobtype != "" && Jobtype != undefined && elementparam.indexOf('Jobtype=') < 0) {
        elementparam = elementparam + '&Jobtype=' + Jobtype;
    }
    if (keyword != "" && keyword != undefined && elementparam.indexOf('keyword=') < 0) {
        elementparam = elementparam + '&keyword=' + keyword;
    }



    if (startDate != "" && startDate != undefined && elementparam.indexOf('startDate=') < 0) {
        elementparam = elementparam + '&startDate=' + startDate;
    }
    if (EndDate != "" && EndDate != undefined && elementparam.indexOf('EndDate=') < 0) {
        elementparam = elementparam + '&EndDate=' + EndDate;
    }

    if (SOL != "" && SOL != undefined && elementparam.indexOf('SOL=') < 0) {
        elementparam = elementparam + '&SOL=' + SOL;
    }

    if (LNC != "" && LNC != undefined && elementparam.indexOf('LNC=') < 0) {
        elementparam = elementparam + '&LNC=' + LNC;
    }

    if (UnsentInv != "" && UnsentInv != undefined && elementparam.indexOf('UnsentInv=') < 0) {
        elementparam = elementparam + '&UnsentInv=' + UnsentInv;
    }
    if (Contracted != "" && Contracted != undefined && elementparam.indexOf('Contracted=') < 0) {
        elementparam = elementparam + '&Contracted=' + Contracted;
    }
    var pagenum = $('.active span').text();
    if (pagenum != "" && pagenum != undefined && elementparam.indexOf('grid-page=') < 0) {
        elementparam = elementparam + '&grid-page=' + pagenum;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0 && elementparam.indexOf('page_size=') < 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);

});

$('.grid-footer a').on('click', function myfunction() {

    var elementparam = $(this).attr('href');

    var index = elementparam.indexOf("grid-page");
    var paramarray = elementparam.split("&");

    if (index > 1) {

        elementparam = '?';
        $.each(paramarray, function (i, item) {

            if (item.indexOf("grid-page") > -1) {
                elementparam = elementparam + item + '&';
            }
        });

        elementparam = elementparam.substring(0, (elementparam.length - 1));
    }
    else {
        elementparam = paramarray[0];
    }
    var jobtype = $('#JobType').val();
    var startDate = $('#StartDate').val();
    var EndDate = $('#EndDate').val();
    var SOL = $('#SOL').is(':checked');
    var LNC = $('#LNC').is(':checked');
    var UnsentInv = $('#UnsentInv').is(':checked');
    var Jobtype = $('#JobType').val();
    var Contracted = $('#Contracted').is(':checked');
    var keyword = $('#Keyword').val();
    if (Jobtype != "" && Jobtype != undefined) {
        elementparam = elementparam + '&JobType=' + Jobtype;
    }

    if (startDate != "" && startDate != undefined) {
        elementparam = elementparam + '&startDate=' + startDate;
    }
    if (EndDate != "" && EndDate != undefined) {
        elementparam = elementparam + '&EndDate=' + EndDate;
    }
    if (keyword != "" && keyword != undefined) {
        elementparam = elementparam + '&keyword=' + keyword;
    }

    if (SOL != "" && SOL != undefined) {
        elementparam = elementparam + '&SOL=' + SOL;
    }

    if (LNC != "" && LNC != undefined) {
        elementparam = elementparam + '&LNC=' + LNC;
    }
    if (UnsentInv != "" && UnsentInv != undefined) {
        elementparam = elementparam + '&UnsentInv=' + UnsentInv;
    }
    if (Contracted != "" && Contracted != undefined) {
        elementparam = elementparam + '&Contracted=' + Contracted;
    }

    var pagesize = parseInt($('#ddPageSize').val());
    if (pagesize != "" && pagesize != undefined && pagesize > 0) {
        elementparam = elementparam + '&page_size=' + pagesize;
    }

    $(this).attr('href', elementparam);


});

function Deletejob(jobid) {
    $.ajax({
        url: common.SitePath + "/Employee/Job/CheckJobInvoice",
        data: { Id: jobid },
        type: 'Get',
        async: false,
        success: function (data) {
            if (data === 0) {
                $(".commonpopupformsg").modal('show');
                $(".alertmsg").text("Please delete invoice of this customer first.");
                return false;
            }
            else {
                $(".commonpopup").modal('show');
                $(".alertmsg").text("Deleting a job will delete all its related data. Are you sure to delete Record?");
                $(".btnconfirm").attr("id", jobid);
                $(".modal-title").html("Delete Job!");
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Employee/Job/DeleteEmployeeJob",
        data: { id: id },
        async: false,
        success: function (data) {
            if (data.result === 0) {
                $(".commonpopup").modal('hide');
                $(".jobalert").css("display", "block");
                $(".jobalert").html("<strong>Job Deleted Successfully!</strong>");
                $(".jobalert").delay(2000).fadeOut(function () { window.location.href = common.SitePath + "/Employee/Job/ViewEmployeeJobs"; });
            }
            else {
                $(".commonpopup").modal('hide');
                $(".commonpopupformsg").modal('show');
                $(".alertmsg").text("Job status is " + data.status + ".So job cannot be deleted");
                return false;
            }
        },
        error: function () {
            alert("something seems wrong");
            $(".commonpopup").modal('hide');
        }
    });
})

$(document).on('dblclick', '.cssEditJob', function () {
    // getting customer id
    var id = $(this).find('td:eq(0)').text();

    window.location = FSM.EditJob + "/" + id;
});

//Quick View Button "v"
$(".Quickbtngrid").bind('click', function () {
    var quickbtn = $(this).children('.qucikbtn').text();
    if (quickbtn == "v") {
        $('.QuickDiv').remove();
        $(this).children('.qucikbtn').text(">");
        return false;
    }
    $('.qucikbtn').text(">");
    $(this).children('.qucikbtn').text("v");
    var jobId = $(this).attr("JobId");
    var customerNote;
    var operationNote;
    var jobNote;
    var jobType;
    var price;
    var paid;
    var approved;
    var time;
    $.ajax({
        url: common.SitePath + "/Employee/CustomerJob/GetQuickViewJobData",
        data: { JobId: jobId },
        type: 'Get',
        async: false,
        success: function (data) {
            customerNote = data.CustomerNotes, operationNote = data.OperationNotes, jobNote = data.JobNotes,
            jobType = data.DisplayType, price = data.InvoicePrice, paid = data.Paid, approved = data.ApprovedBy, time = data.TimeTaken
        },
        error: function () {
            alert("something seems wrong");
        }
    });
    $('.QuickDiv').remove();
    $('<tr class="QuickDiv"><td colspan="10"><table><tbody><tr><td><b>Customer Notes: </b> ' + customerNote + '</td> <td><b>Operation Notes: </b> ' + operationNote + '</td></tr><tr><td><b>Job Notes: </b> ' + jobNote + '</td><td><b>Job Type: </b> ' + jobType + '</td></tr><tr><td><b>Invoice Price: </b> ' + price + '</td><td><b>Paid: </b> ' + paid + '</td></tr><tr><td><b>Time Taken: </b> ' + time + '</td> <td><b>Approved By: </b> ' + approved + '</td></tr></tbody></table></td></tr>').insertAfter($(this).closest('tr'));
});