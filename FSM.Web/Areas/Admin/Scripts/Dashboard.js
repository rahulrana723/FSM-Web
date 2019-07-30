
var locations = [];
Ismonthselect = false;
IsDaySelected = true;
Ismonthselect = false;
CurrentselectedDate = '';

var Dashboard = {
    TotalLocation: 0,
    sitename: "",
    map: null,
    lat: -33.868820,
    lng: 151.209296,
    marker: null,
    IsJob: 0,
    JobId: "",
    JId: "",
    status: "",
    intervalUnit: "",
    SuperVisorOTRWId: "",
    SuperVisorJobId: "",
    CurrentTileResourceId: "",
    CurrentSourceId: "",
    OTRWId: "",
    EventId: "",
    GlobalElement: null,
    JobStatus: "",
    ScrollLeft: 0,

    //******************Render Calender function***********************************
    RenderCalender: function (List, Event) {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var currentHrs = today.getHours();
        var jobid = '';
        var jobval = '';
        var jobstartdate = '';
        var jobstarttime = '';
        var jobendtime = '';
        var eventresource = '';
        var eventmovetime = '';
        var eventdroptime = '';
        var allowrendercheck = [];
        var viewname = '';

        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        var today = yyyy + '-' + mm + '-' + dd;

        $('#calendar').fullCalendar({
            nowIndicator: true,
            editable: true,
            displayEventTime: false,
            minTime: '06:00:00',
            maxTime: '21:00:00',
            timezone: 'local',
            weekends: false,
            slotEventOverlap: false,
            eventOverlap: false,
            droppable: true, // this allows things to be dropped onto the calendar
            drop: function (date, allDay, ev, ui, resource) {
           

             
                var starttime = date._i[3];
                var endtime = 0;
                var JobId = $(this).find('a').attr("jobId");
                Dashboard.JobId = $(this).find('a').attr("jobIntId");
                if (JobId != undefined) {
                    if (ui.intervalUnit == "month" || ui.intervalUnit == "week") {
                        //AssignJob(date._d, JobId);
                    }
                    else {
                        var UserId = ui;
                        var hasJob = JobExist(ui, date._d.toLocaleDateString(), starttime, endtime, JobId);
                        if (hasJob.output == false) {
                            var assignJobUser = CheckAssignJobExist(ui, date._d.toLocaleDateString(), starttime, endtime, JobId);

                            if (assignJobUser.output == true) {
                                if (assignJobUser.notAssignedReason == "User already has job assigned!") {
                                    var WarningReason = assignJobUser.notAssignedReason;
                                    WarningMsgPopUp(WarningReason);
                                    return false;
                                }
                                AssignJobToOTRW(date._d.toLocaleDateString(), JobId, UserId, starttime, endtime);
                                var calendarDate = $('#calendar').fullCalendar('getDate');

                                GetJobsOnDate(calendarDate);
                                checkSuperVisor(JobId, UserId);
                            }
                            else if (hasJob.output == false && assignJobUser.output == false) {
                                $(this).remove();
                                AssignJobToOTRW(date._d.toLocaleDateString(), JobId, UserId, starttime, endtime);
                                checkSuperVisor(JobId, UserId);
                            }
                        }
                        else if (hasJob.output == true) {
                            var WarningReason = hasJob.notAssignedReason;
                            WarningMsgPopUp(WarningReason);
                        }
                       
                    }
                    var calendarDate = $('#calendar').fullCalendar('getDate');
                    GetJobsOnDate(calendarDate);
                   
                }
            },

            eventDragStop: function (event, jsEvent, ui, resource) {
                var trashEl = jQuery('#external-events');
                var ofs = trashEl.offset();

                var x1 = ofs.left;
                var x2 = ofs.left + trashEl.outerWidth(true);
                var y1 = ofs.top;
                var y2 = ofs.top + trashEl.outerHeight(true);

                //if (jsEvent.pageX >= x1 && jsEvent.pageX <= x2 &&
                //    jsEvent.pageY >= y1 && jsEvent.pageY <= y2) {
                //$('#calendar').fullCalendar('removeEvents', event._id);

                //var elemntli = "<li onclick='jobinfo(" + event.data_JobVal + ");' class='fc-event ui-draggable \
                //                ui-draggable-handle'><a jobintid='" + event.data_Job + "' jobid='" +
                //                event.data_JobId + "' href='#'><span class='" + event.data_SpanClass + "'></span><p>Job # "
                //                + event.data_JobVal + "</p><small>" + event.data_CustomerName + "</small>\
                //                <small>" + event.data_JobType + "</small><small>" + event.data_Status + "</small>\
                //                <small>" + event.data_Date + "</small><small>" + event.data_Suburb + "</small></a></li>";

                //$('#external-events ul').prepend(elemntli);
                //Dashboard.externalevents();
                //var data = {
                //    Id: event.data_JobId
                //};
                //$.get(FSM.BindBackEventUrl, data, function (result) {
                //    $(window).scrollTop(0);
                //    $('.assign-job-msg').css('color', 'green');
                //    $('.assign-job-msg').html(result.msg);
                //    $('.assign-job-msg').show();
                //    window.setTimeout(function () {
                //        $('.assign-job-msg').hide();
                //    }, 4000)
                //});

                //}
            },
            eventDrop: function (event, delta, revertFunc) {
               var eventdroptime = event.start.format();
                var tempDate = event.start.format();

                if (event.end == null) {
                    var endsTime = tempDate;
                }
                else {
                    var endsTime = event.end.format();
                }
                jobstarttime = tempDate.substring(tempDate.indexOf('T') + 1, 13);
                jobendtime = endsTime.substring(endsTime.indexOf('T') + 1, 13);

                if (event.data_JobId != "00000000-0000-0000-0000-000000000000" && jobstarttime > parseInt('05')) {
                    jobstartdate = new Date(tempDate).toLocaleDateString();
                    jobenddate = new Date(endsTime).toLocaleDateString();

                    jobval = event.data_JobVal;
                    jobid = event.data_JobId;
                    eventmovetime = '0';
                    eventresource = event.resourceId;

                    var totaltime = parseInt(jobstarttime);
                    var totalendtime = parseInt(jobendtime);

                    if (eventmovetime != '' && eventmovetime != undefined) {
                        totaltime = parseInt(jobstarttime) + parseInt(eventmovetime);
                        totalendtime = parseInt(jobendtime) + parseInt(eventmovetime);
                    }
                    var hasJob = JobExist(eventresource, jobstartdate, totaltime, totalendtime, event.data_JobId);
                    if (hasJob.output == true) {
                        var WarningReason = hasJob.notAssignedReason;
                        WarningMsgPopUp(WarningReason);
                        revertFunc();
                        return false;

                    }
                }
                else {
                    revertFunc();
                }
            },
            customButtons: {
                Refresh: {
                    text: 'Refresh',
                    click: function () {
                        Dashboard.SearchJob();
                    }
                },

                reschedule: {
                    text: 'Reschedule',
                    click: function () {
                        $.ajax({
                            url: common.SitePath + "/Admin/Dashboard/_ShowReschduleCalender",
                            data: {},
                            type: 'Get',
                            async: false,
                            success: function (data) {
                                $("#divShowReschduleCalender").empty();
                                $("#divShowReschduleCalender").html(data);
                                $("#modalReschduleCalender").modal("show");
                            },
                            error: function () {
                                alert("something seems wrong");
                            }
                        });
                    }
                }
            },
            aspectRatio: 2.0,
            //scrollTime: currentHrs > 15 ? '10:20' : '06:00',
            //header: {
            //    left: 'today prev,next',
            //    center: 'title',
            //    right: 'timelineDay,timelineThreeDays,agendaWeek,month'
            //},
            defaultView: 'agendaDay',
            //defaultView: 'timelineDay',
            header: {
                left: 'today prev,next,reschedule,Refresh',
                center: 'title',
                right: 'agendaDay,agendaWeek,month'
            },
            //views: {
            //    timelineThreeDays: {
            //        type: 'agendaDay',
            //        duration: {
            //            days: 3
            //        }
            //    },
            //    timelineOneDays: {
            //        type: 'agenda',
            //        duration: { days: 1 },
            //        buttonText: '1 day'
            //    }
            //},
            resourceAreaWidth: '15%',
            resourceColumns: [
                {
                    labelText: 'Employees',
                    field: 'title'
                },
            ],

            eventMouseover: function (data, event, view) {

                // tooltip = '<div class="tooltiptopicevent" style="width:auto;height:auto;background:#feb811;position:absolute;z-index:10001;padding:10px 10px 10px 10px ; line-height: 200%;">' + data.title + '</br>' + 'Start: ' + new Date(data.start).toUTCString() + '</br>' + 'End:' + new Date(data.end).toUTCString() + '</div>';
                tooltip = '<div class="tooltiptopicevent" style="width:auto;height:auto;background:#feb811;position:absolute;z-index:10001;padding:10px 10px 10px 10px ; line-height: 200%;">' + data.title + '</br>' + 'Start Time: ' + new Date(data.start).getHours() + ":00:00";
                $("body").append(tooltip);
                $(this).mouseover(function (e) {
                    $(this).css('z-index', 10000);
                    $('.tooltiptopicevent').fadeIn('500');
                    $('.tooltiptopicevent').fadeTo('10', 1.9);
                }).mousemove(function (e) {
                    $('.tooltiptopicevent').css('top', e.pageY + 10);
                    $('.tooltiptopicevent').css('left', e.pageX + 20);
                });
            },
            eventMouseout: function (data, event, view) {
                $(this).css('z-index', 8);

                $('.tooltiptopicevent').remove();

            },
            dayClick: function (date, allDay, jsEvent, view) {
                if (allDay) {
                    // Clicked on the entire day
                    $('#calendar').fullCalendar('changeView', 'agendaDay'/* or 'basicDay' */);
                    $('#calendar').fullCalendar('gotoDate', date.format());
                    $(".DayEvent").show();
                    $(".MonthEvent").hide();
                    Dashboard.intervalUnit = "day";
                    var calendarDate = $('#calendar').fullCalendar('getDate');
                    GetJobsOnDate(calendarDate);
                }
            },
            eventResizeStart: function (event, jsEvent, ui, view) {
                event.changing = true;
            },
            //****************** End Time Calculation *********************//

            eventResize: function (event, delta, revertFunc, jsEvent, ui, view) {
                var resouceId = event.resourceId;
                var jobid = event.data_Job.split('#')[1].trim("")
                var empJobId = event.data_JobId;
                //var JobId = event.JobId.split(':');
                //var arr = JobId[1].trim();
                //var arr = jobid;
                var arr = empJobId;
                var endTime = event.end.format();
                var startTime = event.start.format();
                SaveEndTime(arr, endTime, startTime, resouceId);
                event.changing = false;
            },

            eventResizeStop: function (event, jsEvent, ui, view) {
                event.changing = false;
            },

            eventResizeEnd: function (event, jsEvent, ui, view) {
                event.changing = false; // Event is finished being changed
            },


            //*************************************************************//
            eventDragStart: function (event, jsEvent, ui, view) {
                Dashboard.CurrentTileResourceId = event.resourceId;
                Dashboard.CurrentSourceId = event.resourceId;
            },
            viewRender: function (view) {
                viewname = view.name;
                if (view.name == 'agendaDay') {
                    $('.fc-day').css('background-color', '#fcf8e3');
                    $(".Availablehour").text("");
                    $(".workinghours").text("");
                    GetDaysAvailableHours();

                }

                if (view.name == 'agendaWeek') {
                    //var startdate = $('#calendar').fullCalendar('getView').intervalStart._d;
                    //var EndDate = $('#calendar').fullCalendar('getView').intervalEnd._d;
                    //Dashboard.GetAvailabelHoursWeek(startdate, EndDate);
                }

                if (view.name == 'month') {
                    //$('.fc-day').css('background-color', '#fcf8e3');
                    //var startdate = $('#calendar').fullCalendar('getView').intervalStart._d
                    //Dashboard.GetAvailableHoursMonth(startdate);
                }
            },
            resources: List,
            events: Event,
            eventClick: function (calEvent, jsEvent, view) {
                if (view.type == "month") {

                    $('#calendar').fullCalendar('changeView', 'agendaDay'/* or 'basicDay' */);
                    $('#calendar').fullCalendar('gotoDate', calEvent._start._i);
                    $(".DayEvent").show();
                    $(".MonthEvent").hide();
                    Dashboard.intervalUnit = "day";
                }
                else if (calEvent.title != "" && calEvent.title != undefined && calEvent.JobId != "On Leave") {

                    Dashboard.JobId = calEvent.data_JobVal;
                    Dashboard.JId = calEvent.data_JobId;
                    Showjobinfo(Dashboard.JobId, Dashboard.JId);
                }


            },
            eventRender: function (event, element) {

                var title = element.find('.fc-title');
                 title.html(title.text());
                element.bind('mousedown', { OTRWId: event.resourceId, JId: event.data_JobId, EventId: event._id }, function (event) {
                    if (event.which == 3) {

                        Dashboard.OTRWId = event.data.OTRWId;
                        Dashboard.JId = event.data.JId;
                        Dashboard.EventId = event.data.EventId;
                    }
                });
                element.addClass("DayEvent");
                var temporaryDate = event._start._d;
                var starttime = event._start._i[3];
                var endtime = 0;
                var hasJob = '';
                var assignJobUser = '';
                if (event._id.indexOf('_fc') >= 0) {
                    if (allowrendercheck.indexOf(event._id) < 0) {
                        hasJob = JobExist(event.resourceId, event._start._d.toLocaleDateString(), starttime, endtime, event.data_JobId);
                        assignJobUser = CheckAssignJobExist(event.resourceId, event._start._d.toLocaleDateString(), starttime, endtime, event.data_JobId);
                        allowrendercheck.push(event._id);
                    }
                }
                else {
                    hasJob.output = false;
                    assignJobUser.output = false;
                }
                if (hasJob.output) {
                   
                    $('#calendar').fullCalendar('removeEvents', event._id);
                }
                if (assignJobUser.output && assignJobUser.notAssignedReason == "User already has job assigned!") {
                    $('#calendar').fullCalendar('removeEvents', event._id);
                }

            },
            resourceRender: function (resourceObj, labelTds, bodyTds) {
                labelTds.on('click', function () {
                    Dashboard.fullMapWidth();
                    var userid = resourceObj.id;
                    var calendarDate = $('#calendar').fullCalendar('getDate');
                    Dashboard.GetJobsOnFullMap(calendarDate, userid);
                });
            },
            eventAfterAllRender: function (ui) {
                $('#calendar').scrollLeft(Dashboard.ScrollLeft);
                if (jobval != '' && jobval != undefined) {
                    var endtime = 0;
                    var resourceid;
                    if (eventresource != '' && eventresource != undefined) {
                        resourceid = eventresource;
                    }
                    else {
                        resourceid = $(".fc-content span:contains('Job_Id=" + jobval + "')").closest('tr').attr('data-resource-id');
                    }

                    var data = {
                        Id: jobid,
                        ResourceId: resourceid
                    };
                    var totaltime = parseInt(jobstarttime);
                    if (eventmovetime != '' && eventmovetime != undefined) {
                        totaltime = parseInt(jobstarttime) + parseInt(eventmovetime);
                    }
                    var totalendtime = parseInt(jobendtime);
                    if (eventmovetime != '' && eventmovetime != undefined) {
                        totalendtime = parseInt(jobendtime) + parseInt(eventmovetime);
                    }
                    var hasJob = JobExist(resourceid, jobstartdate, totaltime, totalendtime, jobid);
                    if (hasJob.output == false) {
                        if (eventresource != '' && eventresource != undefined) {
                            AssignJobToOTRW(jobstartdate, jobid, eventresource, totaltime, totalendtime);
                        }
                        else {
                            $.get(FSM.ChangeJobResourceUrl, data, function (result) {
                                $(window).scrollTop(0);
                                $('.assign-job-msg').css('color', 'green');
                                $('.assign-job-msg').html(result.msg);
                                $('.assign-job-msg').show();
                                window.setTimeout(function () {
                                    $('.assign-job-msg').hide();
                                }, 4000)
                            });
                        }
                    }
                    else if (true) {
                        //alert(hasJob.notAssignedReason);
                    }
                    jobval = '';
                    jobid = '';
                    jobstartdate = '';
                    jobstarttime = '';
                    jobendtime = '';
                    eventresource = '';
                    eventmovetime = '';
                }

                $("span:contains('On Leave')").parent('div.fc-content').css('height', '32px');
                $("span:contains('On Leave')").parent('div.fc-content').parent().css('background-color', 'grey');
                $("span:contains('On Leave')").parent('div.fc-content').parent().css('border-color', 'grey');

                if (viewname == "month") {
                    $("a span:contains('On Leave')").parent().parent().hide();
                    $("a span:contains('Job_Id')").parent().parent().hide();
                }
                else if (viewname == "agendaWeek") {
                    $("div.fc-title:contains('On Leave')").parent().parent().hide();
                    $("div.fc-title:contains('Job_Id')").parent().parent().hide();
                }
            }
        });
        $("span:contains('On Leave')").parent('div.fc-content').css('height', '32px');
        $(".fc-left span").html("");
        $(".fc-left").append('<span style="display:none;" class="loader-Dashboard">Loading...</span>');




    },

    //*********External event function of jobs which need to drag on Calender*******
    externalevents: function () {

        $('#external-events .fc-event').each(function () {
            var smalllist = $(this).find('small');
            var eventData = $(this)[0].innerText.split("\n");
            // store data so the calendar knows to render an event upon drop
            $(this).data('event', {
                //title: $.trim("JobId: " + $(this).find('a').attr("title")), 
                title: $.trim($(this).find('a').attr("title")),// use the element's text as the event title
                stick: true, // maintain when user navigates (see docs on the renderEvent method)
                //duration: +"6:00",
                
                duration: Math.floor(parseFloat($(this).find('a').attr("expanttime"))) + ":00",
                data_Job: eventData[0],
                data_CustomerName: smalllist[0].innerHTML,
                data_JobType: smalllist[1].innerHTML,
                data_Status: smalllist[2].innerHTML,
                data_Date: smalllist[3].innerHTML,
                data_Suburb: smalllist[4].innerHTML,
                data_JobVal: $(this).find('a').attr("jobIntId"),
                data_JobId: $(this).find('a').attr("jobid"),
                data_SpanClass: $(this).find('span').attr('class')
            });
            // make the event draggable using jQuery UI
            $(this).draggable({
                zIndex: 9999,
                revert: true, // will cause the event to go back to its
                revertDuration: 0 // original position after the drag
            });
        });
    },

    //******************initialize function to load the map*************************
    initialize: function (ZoomLevel) {
        Dashboard.map = new google.maps.Map(document.getElementById('map-canvas'), {
            center: new google.maps.LatLng(Dashboard.lat, Dashboard.lng),
            zoom: ZoomLevel,
            mapTypeId: google.maps.MapTypeId.HYBRID
        });

        //if (Dashboard.IsJob == 1) {
        //    Dashboard.marker = new google.maps.Marker({
        //        position: new google.maps.LatLng(Dashboard.lat, Dashboard.lng),
        //        map: Dashboard.map,
        //        title: 'click to see site detial',
        //        content: Dashboard.sitename
        //    });

        //    Dashboard.marker.infowindow = new google.maps.InfoWindow();
        //    Dashboard.marker.addListener('click', function (event) {
        //        this.infowindow.setPosition(event.latLng);
        //        this.infowindow.setContent(this.content);
        //        this.infowindow.open(Dashboard.map, this);
        //    });
        //}

        //for (i = 0; i < locations.length; i++) {
        // if (Dashboard.sitename[i] != "") {
        // if (locations[i].lat != 0) {
        // var contentString = '<div id="content"><b>JobId: </b>' + locations[i].jobId + ' <br/><b>Site Address: </b>' + Dashboard.sitename[i] + '</div>';
        // marker[i] = new google.maps.Marker({
        // position: new google.maps.LatLng(locations[i].lat, locations[i].lng),
        // map: map,
        // title: 'click to see site detial',
        // content: contentString
        // });

        // marker[i].infowindow = new google.maps.InfoWindow();
        // marker[i].addListener('click', function (event) {
        // this.infowindow.setPosition(event.latLng);
        // this.infowindow.setContent(this.content);
        // this.infowindow.open(map, this);
        // });
        // }
        // }
        //}
    },

    //******************Function to set Map with full width*************************
    fullMapWidth: function () {
        $("#accordion2").css("width", "100%");
        $("#map-canvas").css("width", $("#map-canvas").parent().width());
        $("#map-canvas").css("margin", "15px 0");
        $('#accordion2').removeClass('is-collapsed');
        $('#accordion3').addClass('is-collapsed');
    },

    //******************Function to get jobs on Full Map****************************
    GetJobsOnFullMap: function (date, AssignId) {
        Dashboard.initialize(10);
        var userId;
        if (AssignId == '' || AssignId == undefined) {
            userId = "";
        }
        else {
            userId = AssignId;
        }

        if (date == "" || date == undefined) {
            date = ($.datepicker.formatDate('dd M yy', new Date()))
        }
        else {
            date = ($.datepicker.formatDate('dd M yy', date._d))
        }
        $.ajax({
            url: common.SitePath + '/Admin/Dashboard/GetJobsOnFullMap',
            type: "GET",
            dataType: "JSON",
            data: {
                Date: date, userId: userId
            },
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                $(".jobinfo").html("");
                if (Dashboard.marker != null) {
                    Dashboard.marker.setMap(null);
                }
                for (i = 0; i < data.length; i++) {

                    Dashboard.sitename = '<b>JobId: </b>' + response[i]["JobNo"] + '<br/><b>Site Detial: </b>' + response[i]["StreetName"];
                    if (/^\s+$/.test(Dashboard.sitename)) {
                        Dashboard.sitename = "";
                        //string contains only whitespace
                    }
                    if (response[i]["Latitude"] != 0 && response[i]["Latitude"] != null)
                        Dashboard.lat = response[i]["Latitude"];

                    if (response[i]["Longitude"] != 0 && response[i]["Longitude"] != null)
                        Dashboard.lng = response[i]["Longitude"];

                    if (Dashboard.map != null && response[i]["Latitude"] != 0 && response[i]["Latitude"] != null && response[i]["Longitude"] != 0 && response[i]["Longitude"] != null) {

                        Dashboard.map.setCenter(new google.maps.LatLng(Dashboard.lat, Dashboard.lng));
                        Dashboard.marker = new google.maps.Marker({
                            position: new google.maps.LatLng(Dashboard.lat, Dashboard.lng),
                            map: Dashboard.map,
                            title: 'click to see site detial',
                            content: Dashboard.sitename
                        });

                        Dashboard.marker.infowindow = new google.maps.InfoWindow();
                        Dashboard.marker.addListener('click', function (event) {
                            this.infowindow.setPosition(event.latLng);
                            this.infowindow.setContent(this.content);
                            this.infowindow.open(Dashboard.map, this);
                        });
                    }
                }
            }
        });
    },

    //******************Function to reschedule job*********************************
    GetRescheduleJobUpdate: function (resourceId) {
        $.ajax({
            url: common.SitePath + "/Admin/Dashboard/_ShowReschduleCalender",
            data: { ResourceId: resourceId },
            type: 'Get',
            async: false,
            success: function (data) {
                $("#divShowReschduleCalender").empty();
                $("#divShowReschduleCalender").html(data);
                $("#modalReschduleCalender").modal("show");
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    },

    //******************Function to extend job*********************************
    ExtendJob: function (jobId) {
        $.ajax({
            url: common.SitePath + "/Admin/Dashboard/_ExtendJobPopUp",
            data: { JobId: jobId },
            type: 'Get',
            async: false,
            success: function (data) {
                $("#divShowReschduleCalender").empty();
                $("#divShowReschduleCalender").html(data);
                $("#modalExtendJobCalender").modal("show");
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    },

    //******************Function to Reassign a job*********************************
    ReAssignedJob: function (jobId, OTRWId) {
        $.ajax({
            url: common.SitePath + "/Admin/Dashboard/_ReAssignedJobPopUp",
            data: { JobId: jobId, OTRWID: OTRWId },
            type: 'Get',
            async: false,
            success: function (data) {
                $("#divShowReschduleCalender").empty();
                $("#divShowReschduleCalender").html(data);
                $("#modalReAssignedJobPopUp").modal("show");
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    },

    //******************Function to unassign a job*********************************
    UnAssignedJob: function (jobId, OTRWId) {
        var calendarDate = $('#calendar').fullCalendar('getDate');
        var date = calendarDate._d.toLocaleDateString();
        $.ajax({
            url: common.SitePath + "/Admin/Dashboard/UnAssignedJob",
            data: { JobId: jobId, OTRWID: OTRWId, UnAssignedDate: date },
            type: 'Get',
            async: false,
            success: function (data) {
                $('.assign-job-message').css('color', 'green');
                $('.assign-job-message').html("Updated successfully");
                $('.assign-job-message').show();
                window.setTimeout(function () {
                }, 4000)
                //location.reload();
                $('#calendar').fullCalendar('removeEvents', Dashboard.EventId);
                Dashboard.SearchJob();
                Dashboard.GetJobsOnFullMap(calendarDate, '');
                GetJobsOnDate(calendarDate);
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    },

    //******************Function to delete a job*********************************
    DeleteJob: function (jobId) {
        $.ajax({
            url: common.SitePath + "/Employee/Job/CheckJobInvoice?Module=Jobs",
            data: { Id: jobId },
            type: 'Get',
            async: false,
            success: function (data) {
                if (data === 0) {
                    $(".commonpopup").modal('show');
                    $(".alertmsg").text("Deleting a job will delete all its related data. Are you sure to delete Record?");
                    $(".btnconfirm").attr("id", jobId);
                    $(".modal-title").html("Delete Job!");
                }
                else {
                    $(".commonpopup").modal('show');
                    $(".alertmsg").text("Deleting a job will delete all its related data. Are you sure to delete Record?");
                    $(".btnconfirm").attr("id", jobId);
                    $(".modal-title").html("Delete Job!");
                }
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    },


    //******************Function to Set Order*********************************
    SetOrder: function () {
        $.ajax({
            url: common.SitePath + "/Admin/Dashboard/_SetOrderPopUp",
            data: {},
            type: 'Get',
            async: false,
            success: function (data) {
                $("#divShowReschduleCalender").empty();
                $("#divShowReschduleCalender").html(data);
                $("#modalSetOrder").modal("show");
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    },
    //******************Function to search a job*********************************
    SearchJob: function (RefralVal) {
        var statustoggle = Dashboard.status;
        var srchkey = $("#dashboardserchbox").val();
        var JobtypeVal = $('#JobTypes').val();
        if (JobtypeVal == "") {
            JobtypeVal = "null";
        }
        //var StatusVal = $('#ddljobStatus').val();
        if (statustoggle == "") {
            StatusVal = "";
        }
        else {
            StatusVal = "" + statustoggle + "";
        }
        var datepick;
        if (RefralVal == "today") {
            var fullDate = new Date()

            //datepick = fullDate.datepicker({ dateFormat: 'dd M yy' }).val();
            //$('#calendar').fullCalendar('gotoDate', datepick);

            //var calendarDate = $('#calendar').fullCalendar('getDate');
            datepick = ($.datepicker.formatDate('dd M yy', fullDate))
        }
        else {
            var dateSearch = $('#DateBooked').val();
            if (dateSearch == "") {
                datepick = $('#calendar').fullCalendar('getDate');
                datepick = ($.datepicker.formatDate('dd M yy', datepick._d));
            }
            else {
                datepick = $('#DateBooked').datepicker({ dateFormat: 'dd M yy' }).val();
                $('#calendar').fullCalendar('gotoDate', datepick);

                var calendarDate = $('#calendar').fullCalendar('getDate');
                datepick = ($.datepicker.formatDate('dd M yy', calendarDate._d))

            }
        }
        if (datepick == "") {
            datepick = "null";
        }

        $.ajax({
            type: 'GET',
            url: common.SitePath + "/Admin/Dashboard/GetEmployeeJobsUsingJobType",
            data: {
                Jobtype: JobtypeVal, Status: StatusVal, selectdate: datepick, searchkey: srchkey
            },
            dataType: "JSON",
            async: false,
            success: function (data) {
                $(".loader-Dashboard").show();
                if (Dashboard.intervalUnit == "month") {
                    $(".MonthEvent").show();
                    $(".DayEvent").hide();
                }
                else if (Dashboard.intervalUnit == "day") {
                    $(".DayEvent").show();
                    $(".MonthEvent").hide();
                }
                $(this).attr("disabled", "disabled");
                Dashboard.fullMapWidth();
                var calendarDate = $('#calendar').fullCalendar('getDate');
                Dashboard.GetJobsOnFullMap(calendarDate, '');
                GetJobsOnDate(calendarDate);
                $('.loader-Dashboard').fadeOut('slow');
                $(this).removeAttr("disabled");

                var response = jQuery.parseJSON(data.list);
                Dashboard.status = "";
                $(".divjob").html("");
                var jobs = "<ul>";
                for (i = 0; i < data.length; i++) {
                    var jobtype = response[i]["JobType"];
                    switch (jobtype) {
                        case 1:

                            jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip2"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                     '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                            //    jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip2"><a title="' + response[i]["title"] + '"ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '" ><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                            break;
                        case 2:
                            jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip3"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                  '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                            //jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip3"><a title="' + response[i]["title"] + '"ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '"><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                            break;
                        case 3:
                            jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip4"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                      '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                            //jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip4"><a title="' + response[i]["title"] + '"ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '"><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                            break;
                        case 4:
                            jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip8"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                  '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                            // jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip8"><a title="' + response[i]["title"] + '"ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '"><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                            break;
                        default:
                            jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                  '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                            //  jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event"><a title="' + response[i]["title"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '"><span class=""></span><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                            break;
                    }
                }
                jobs = jobs + "</ul>";
                $(".divjob").append(jobs);
                var calenderList = data.CalenderList;

                if (calenderList.length > 0) {
                    $('#calendar').fullCalendar('removeEvents');                       //remove events
                    $.each(calenderList[0]["EmployeeList"], function (i, p) {          //remove all resource 
                        $('#calendar').fullCalendar('removeResource', p.id);
                    });

                    $.each(calenderList[0]["EmployeeList"], function (i, p) {          //bind all resource
                        $('#calendar').fullCalendar('addResource', p);
                    });

                    $('#calendar').fullCalendar('addEventSource', calenderList[0]["EventDetail"]);
                    $('#calendar').fullCalendar('rerenderEvents');
                }
                else {
                    $('#calendar').fullCalendar('removeEvents');
                    $('#calendar').fullCalendar('rerenderEvents');
                }

                if (StatusVal != "5") {
                    Dashboard.externalevents();
                }
            }
        });
    },

    //******************Function to get available hours a week*********************************
    GetAvailabelHoursWeek: function (startdate, EndDate) {
        $.ajax({
            url: common.SitePath + '/Admin/Dashboard/GetAvailableHoursWeek',
            type: "GET",
            dataType: "JSON",
            data: {
                startingdate: startdate, Endingdate: EndDate
            },
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                for (i = 0; i < response.length; i++) {
                    var RoofHoursAvailable = response[i]["RoofHoursAvailable"];
                    var CleaningHoursAvailable = response[i]["CleaningHoursAvailable"];
                    $(".RoofHoursAvailable").text("Roof Hours Available: " + RoofHoursAvailable);
                    $(".CleaningHoursAvailable").text("Cleaning Hours Available: " + CleaningHoursAvailable);
                }
            },
            error: function () {
            }
        });
    },

    //******************Function to get available hours of a month*******************************
    GetAvailableHoursMonth: function (monthDate) {
        $.ajax({
            url: common.SitePath + '/Admin/Dashboard/GetAvailableHourMonths',
            type: "GET",
            dataType: "JSON",
            data: {
                monthdate: monthDate
            },
            success: function (data) {
                var response = jQuery.parseJSON(data.list);
                for (i = 0; i < response.length; i++) {
                    var RoofHoursAvailable = response[i]["RoofHoursAvailable"];
                    var CleaningHoursAvailable = response[i]["CleaningHoursAvailable"];
                    $(".RoofHoursAvailable").text("Roof Hours Available: " + RoofHoursAvailable);
                    $(".CleaningHoursAvailable").text("Cleaning Hours Available: " + CleaningHoursAvailable);
                }

            },
            error: function () {
            }
        });
    },


    //******************Function to unassign a job a particular Otrw*********************************
    UnAssignedJobOtrw: function (date, OTRWId, jobid) {
        $.ajax({
            url: common.SitePath + "/Admin/Dashboard/UnassignOtrwJob",
            data: { Date: date, JobId: jobid, OTRWID: OTRWId },
            type: 'Get',
            async: false,
            success: function (data) {
                $('.assign-job-message').css('color', 'green');
                $('.assign-job-message').html("Unassigned successfully");
                $('.assign-job-message').show();
                window.setTimeout(function () {
                }, 2000)
                //location.reload();
                $('#calendar').fullCalendar('removeEvents', Dashboard.EventId);
                Dashboard.SearchJob();
                var calendarDate = $('#calendar').fullCalendar('getDate');
                Dashboard.GetJobsOnFullMap(calendarDate, '');
                GetJobsOnDate(calendarDate);
            },
            error: function () {
                alert("something seems wrong");
            }
        });
    },
}

//****ready*******
$(document).ready(function () {


    setInterval(function () {
        Dashboard.SearchJob() // this will run after every 15 minutes
    }, 150000);

    $(".slide-toggle").click(function () {
        $(".dashboard_left_block2").toggle("slide", { direction: "left" }, 1000);
        // $('.dashboard_left_block2').toggle("slow");
    });

    $('#LogDate,#ReContactDate').datepicker({
        dateFormat: 'dd/mm/yy',
        minDate: 0
    });
    var a = ($(this).attr('data-resource-id'));
    $(function () {
        $.contextMenu({
            selector: '.DayEvent',
            build: function ($trigger, e) {
                var jobId = Dashboard.JId;
                if (jobId == "00000000-0000-0000-0000-000000000000")
                {
                    return false;
                }
                $.ajax({
                    url: common.SitePath + "/Admin/Dashboard/GetCurrentJobStatus",
                    data: { JobId: jobId },
                    type: 'Get',
                    async: false,
                    success: function (data) {
                        if (data == 15) {
                            menuItems = {
                                "CallBackJob": { name: "Call Back Job", icon: "paste" },
                                "Reassign": { name: "Reschedule", icon: "edit" },
                                "UnAssign": { name: "un-assign", icon: "delete" },
                                "DeleteJob": { name: "Delete Job", icon: "delete" },
                                "quit": {
                                    name: "Quit", icon: function () {
                                        return 'context-menu-icon context-menu-icon-quit';
                                    }
                                }
                            }
                        }
                        else {
                            menuItems = {
                                "extendJob": { name: "Extend Job", icon: "paste" },
                                "Reassign": { name: "Reschedule", icon: "edit" },
                                "UnAssign": { name: "un-assign", icon: "delete" },
                                "DeleteJob": { name: "Delete Job", icon: "delete" },
                                "quit": {
                                    name: "Quit", icon: function () {
                                        return 'context-menu-icon context-menu-icon-quit';
                                    }
                                }
                            }
                        }
                    },
                });
                return {               
                    callback: function (key, options) {
                        if (key == "extendJob" || key == "CallBackJob") {
                            var jobId = Dashboard.JId;
                            Dashboard.ExtendJob(jobId);
                        }
                        if (key == "Reassign") {
                            var jobId = Dashboard.JId;
                            var OTRWId = Dashboard.OTRWId;
                            Dashboard.ReAssignedJob(jobId, OTRWId);
                        }
                        if (key == "UnAssign") {
                            $(".context-menu-root").hide();
                            var jobId = Dashboard.JId;
                            var OTRWId = Dashboard.OTRWId;
                            Dashboard.UnAssignedJob(jobId, OTRWId);
                        }
                        if (key == "DeleteJob") {
                            $(".context-menu-root").hide();
                            var jobId = Dashboard.JId;
                            var OTRWId = Dashboard.OTRWId;
                            //Dashboard.UnAssignedJob(jobId, OTRWId);
                            Dashboard.DeleteJob(jobId);
                        }
                    },
                    items: menuItems
                }
            },
        });


        $('.context-menu-one').on('click', function (e) {
            console.log('clicked', this);
        })
    });

    $(function () {
        $.contextMenu({
            selector: '.fc-resource-cell',
            callback: function (key, options) {
                if (key == "reschedule") {
                    var datepick;
                    var resourceId = ($(this).attr('data-resource-id'));
                    Dashboard.GetRescheduleJobUpdate(resourceId);
                }

                if (key == "Unassign") {

                    var resourceId = ($(this).attr('data-resource-id'));
                    var date = $('#calendar').fullCalendar('getDate');
                    date = date._d.toLocaleDateString();
                    var jobId = Dashboard.JId;

                    Dashboard.UnAssignedJobOtrw(date, resourceId, Dashboard.JId);
                }
                if (key == "SetOrder") {
                    Dashboard.SetOrder();
                }
            },
            items: {
                "reschedule": { name: "Reschedule", icon: "reschedule" },
                "Unassign": { name: "Unassign", icon: "Unassign" },
                "SetOrder": { name: "Set order", icon: "Order" },
                "sep1": "---------",
                "quit": {
                    name: "Quit", icon: function () {
                        return 'context-menu-icon context-menu-icon-quit';
                    }
                }
            }
        });


        $('.context-menu-one').on('click', function (e) {
            console.log('clicked', this);
        })
    });

    Dashboard.fullMapWidth();
    $('#DateBooked').datepicker({});

    var param1 = "";
    GetEmployyeJobs(param1);

    $('#calendar').find('.fc-today-button').click(function () {
        var Referal = "today";

        Dashboard.SearchJob(Referal);

    });

    $('#calendar').on('mouseup', function (event) {
        Dashboard.ScrollLeft = $('#calendar').scrollLeft();
    });
    var cookVal = $.cookie('saveCalanderDate');
    if (cookVal != undefined && cookVal != "") {
        GetJobsOnDate(cookVal);
    }

    Dashboard.GetJobsOnFullMap('', '');

    $(".joblist").change(function () {
        var sortedorder = $(this).val();
    })

    $('.fc-prev-button').click(function () {
        $(".loader-Dashboard").show();
        if (Dashboard.intervalUnit == "month") {
            $(".MonthEvent").show();
            $(".DayEvent").hide();
        }
        else if (Dashboard.intervalUnit == "day") {
            $(".DayEvent").show();
            $(".MonthEvent").hide();
        }
        $(this).attr("disabled", "disabled");
        Dashboard.fullMapWidth();
        var calendarDate = $('#calendar').fullCalendar('getDate');
        Dashboard.GetJobsOnFullMap(calendarDate, '');
        GetJobsOnDate(calendarDate);
        $('.loader-Dashboard').fadeOut('slow');
        $(this).removeAttr("disabled");
    });

    $('.fc-next-button').click(function () {
        $(".loader-Dashboard").show();
        if (Dashboard.intervalUnit == "month") {
            $(".MonthEvent").show();
            $(".DayEvent").hide();
        }
        else if (Dashboard.intervalUnit == "day") {
            $(".DayEvent").show();
            $(".MonthEvent").hide();
        }
        $(this).attr("disabled", "disabled");
        Dashboard.fullMapWidth();
        var calendarDate = $('#calendar').fullCalendar('getDate');
        Dashboard.GetJobsOnFullMap(calendarDate, '');
        GetJobsOnDate(calendarDate);
        $('.loader-Dashboard').fadeOut('slow');
        $(this).removeAttr("disabled");
    });

    $(".fc-month-button").click(function () {
        Dashboard.intervalUnit = "month";
        $(".MonthEvent").show();
        $(".DayEvent").hide();
    });

    $(".fc-agendaDay-button").click(function () {
        Dashboard.intervalUnit = "day";
        $(".DayEvent").show();
        $(".MonthEvent").hide();
    });

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

                  if (wasOpen) {
                      return;
                  }
                  input.autocomplete("search", "");
              });
        },

        _source: function (request, response) {
            $("#CustomerId").empty();
            var SearchTerm = request.term;
            var CustomerList = [];

            var CustomerList = jQuery.parseJSON(FSM.CustomerListVal);

            CustomerList = jQuery.grep(CustomerList, function (item) {
                var text = item.Text;
                if (text != '' && text != undefined) {
                    var index = text.toLowerCase().indexOf(SearchTerm.toLowerCase());
                    return index > -1 ? true : false;
                }
            });


            CustomerList = CustomerList.length > 10 ? CustomerList.slice(0, 20) : CustomerList;

            // binding values again to combo
            for (var i = 0; i < CustomerList.length; i++) {
                $("#CustomerId").append("<option value='" + CustomerList[i].Value + "'>" + CustomerList[i].Text + "</option>");
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

    $("#CustomerId").combobox();

    $("#CustomerId").combobox({
        select: function (event, ui) {
            var s = $(this).val();
            BindJobByCustomerId(s);
        }
    });

    $("#toggle").on("click", function () {
        $("#CustomerId").toggle();
    });

    $(".custom-combobox-input ").attr("placeholder", "Search Customer");
    if (FSM.CustomerId != '' && FSM.CustomerId != undefined) {
        $('#CustomerId').val(FSM.CustomerId);
        $(".custom-combobox-input ").val(FSM.CustomerLastName);
        BindJobByCustomerId(FSM.CustomerId);
    }
});


//************Delete job click on confirm button*****************//

$(document).on('click', ".btnconfirm", function () {
    var id = $(this).attr("id");
    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Admin/Dashboard/DeleteEmployeeJob",
        data: { id: id },
        async: false,
        success: function (data) {
            if (data.result === 0) {
                $(".commonpopup").modal('hide');
                $('.assign-job-msg').css('color', 'green');
                $(".assign-job-msg").html("<strong>Job Deleted Successfully!</strong>");
                $('.assign-job-msg').show();
                $(".assign-job-msg").delay(6000).fadeOut(function () { });
                Dashboard.SearchJob();
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
});


$(document).on('click', '.assign-job', function () {
    var formdata = new FormData($('#formAssignJob').get(0));
    $("#modalAssigntootrw").modal("hide");

    $.ajax({
        url: $('#formAssignJob').attr("action"),
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            $(window).scrollTop(0);
            $('.assign-job-msg').css('color', 'green');
            $('.assign-job-msg').html(result.msg);
            $('.assign-job-msg').show();
            window.setTimeout(function () {
                $('.assign-job-msg').hide();
            }, 4000)
        },
        error: function () {
            alert('something went wrong !');
        }
    });

});

$(document).on('click', '.accordion-title', function () {
    var iscollapsed = $(this).attr('aria-expanded')
    if (iscollapsed == 'true') {
        $(this).removeClass('accordionClose');
        $(this).addClass('accordionOpen');
    }
    else {
        $(this).removeClass('accordionOpen');
        $(this).addClass('accordionClose');
    }

})

$("#ddlsearch").change(function () {
    var searchval = $("#ddlsearch").val();
    if (searchval == 1) {
        $("#ddljobtypediv").show();

        $("#ddlstatusdiv").hide();
        var StatusVal = $('#ddljobStatus').val();
        if (StatusVal != "") {
            $('#ddljobStatus').val("");
        }

        $("#ddldatepickdiv").hide();
        var datepicked = $('#DateBooked').val();
        if (datepicked != "") {
            $('#DateBooked').val("");
        }
    }
    else if (searchval == 2) {
        $("#ddljobtypediv").hide();
        var JobtypeVal = $('#JobTypes').val();
        if (JobtypeVal != "") {
            $('#JobTypes').val("");
        }
        $("#ddlstatusdiv").show();

        $("#ddldatepickdiv").hide();
        var datepicked = $('#DateBooked').val();
        if (datepicked != "") {
            $('#DateBooked').val("");
        }
    }
    else if (searchval == 3) {
        $("#ddljobtypediv").hide();
        var JobtypeVal = $('#JobTypes').val();
        if (JobtypeVal != "") {
            $('#JobTypes').val("");
        }
        $("#ddlstatusdiv").hide();
        var StatusVal = $('#ddljobStatus').val();
        if (StatusVal != "") {
            $('#ddljobStatus').val("");
        }

        $("#ddldatepickdiv").show();
    }
    else {
        $("#ddljobtypediv").hide();
        var JobtypeVal = $('#JobTypes').val();
        if (JobtypeVal != "") {
            $('#JobTypes').val("");
        }

        $("#ddlstatusdiv").hide();
        var StatusVal = $('#ddljobStatus').val();
        if (StatusVal != "") {
            $('#ddljobStatus').val("");
        }

        $("#ddldatepickdiv").hide();
        var datepicked = $('#DateBooked').val();
        if (datepicked != "") {
            $('#DateBooked').val("");
        }
        return false;
    }
});

$("#jobsrch").click(function () {
    Dashboard.SearchJob();
});

$(document).on("keypress", "#dashboardserchbox", function (e) {

    var key = e.which;
    if (key == 13) // the enter key code
    {
        $('#jobsrch').click();
        return false;
    }
});

$("#StatusBooked").click(function () {
    $(this).css('box-shadow', '3px 1px 54px rgb(183, 183, 183) inset');
    $("#StatusHold").css('box-shadow', 'inset 8px 1px 13px #dcdcdc');
    Dashboard.status = 3;
    $('#jobsrch').click();
});

$("#StatusHold").click(function () {
    $(this).css('box-shadow', '3px 1px 54px rgb(183, 183, 183) inset');
    $("#StatusBooked").css('box-shadow', 'inset 8px 1px 13px #dcdcdc');
    Dashboard.status = 13;
    $('#jobsrch').click();
});

$('.btnaddRem').unbind('click');
$(".btnaddRem").bind("click", function () {
    $('.btnaddRem').text("Loading...");
    var ReminderDate = $('#calendar').fullCalendar('getDate');
    ReminderDate = ($.datepicker.formatDate('dd MM yy', ReminderDate._d));


    $.ajax({
        data: { ReminderDate: ReminderDate },
        url: common.SitePath + '/Admin/Dashboard/_DashboardReminderCreate',
        type: 'Get',
        async: true,
        success: function (data) {
            $("#divRemPopup").html(data);
            $("#modalReminder").modal("show");
            $('.btnaddRem').text("Send Messages");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).on("change", "#Job_Id", function () {
    var JobId = $(this).val();
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/GetSiteByJobId",
        data: { JobId: JobId },
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (result) {
            $("#SiteId").show();
            $("#SiteId").empty();
            for (var i = 0; i < result.SiteNameList.length; i++) {
                var opt = new Option(result.SiteNameList[i].Text, result.SiteNameList[i].Value);
                $("#SiteId").append(opt);
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$("#btnPreview").click(function () {
    var CustomerID = $("#CustomerId").val();
    var JobID = $("#Job_Id").val();
    var SiteID = $("#SiteId").val();
    var LogDate = $("#LogDate").val();
    var ReContactDate = $("#ReContactDate").val();
    var Note = $("#note").val();
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/Export",
        data: { CustomerID: CustomerID, JobID: JobID, SiteID: SiteID, LogDate: LogDate, ReContactDate: ReContactDate, Note: Note },
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (result) {
            return true;
        },
        error: function () {
            return false;
        }
    });
});

$(document).on("click", "#UpdateRescheduleDate", function () {
    var datepick;
    datepick = $('#calendar').fullCalendar('getDate');
    datepick = ($.datepicker.formatDate('dd M yy', datepick._d));
    datepick = new Date(datepick).toLocaleDateString();

    var updateDate = $("#reschduleCalender").val();
    updateDate = new Date(updateDate).toLocaleDateString();

    var resourceId = $('#resourceIdCal').val();
    if (resourceId == "") {
        resourceId = "00000000-0000-0000-0000-000000000000";
    }
    var AssignJobTo = $('#tempAssignTo').val();
    if (AssignJobTo == "") {
        AssignJobTo = "00000000-0000-0000-0000-000000000000";
    }

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
    $("#UpdateRescheduleDate").val("Updating...");
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/UpdateRescheduleJob",
        data: { CurrentDate: datepick, UpdateRescheduleDate: updateDate, ResourceId: resourceId, AssignTo: AssignJobTo },
        type: 'Get',
        async: false,
        success: function (data) {
            if (data.result === 1) {
                $(window).scrollTop(0);
                $("#modalReschduleCalender").hide();
                $('.assign-job-msg').css('color', 'green');
                $('.assign-job-msg').html("Updated successfully");
                $('.assign-job-msg').show();
                window.setTimeout(function () {
                    $('.assign-job-msg').hide();
                    location.reload();
                }, 4000)
            }
            else if (data.error === "0") {
                $("#UpdateRescheduleDate").val("Update");
                $("#ErrorDv").empty();
                $(window).scrollTop(0);
                $("#ErrorDv").css("display", "block");
                $('#ErrorDv').css('color', 'red');
                $("#ErrorDv").html("<strong>"+data.msg+"</strong>");
                $("#ErrorDv").delay(4000).fadeOut(function () {
                });
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).on("click", "#SuperVisorYes", function () {
    var OTRWID = Dashboard.SuperVisorOTRWId;
    var jobId = Dashboard.SuperVisorJobId;
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/CreateSuperVisor",
        data: { JobId: jobId, UserId: OTRWID },
        type: 'Get',
        async: false,
        success: function (data) {
            if (data === 1) {
                $("#modalCreateSuperVisor").modal('hide');
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).on("click", "#SuperVisorNo", function () {
    $("#modalCreateSuperVisor").modal('hide');
});

$(document).on("click", "#UpdateExtendJob", function () {
    var updateDate = $("#DateBookedCalender").val();
    //updateDate = new Date(updateDate).toLocaleDateString();

    var EstimateHour = $("#estimatedHours").val();

    var jobId = $('#jobIdCal').val();
    //var OTRWRequired = $("#OtrwReuired").val();
    var userid = [];
    $.each($("#tempAssignTo2 option:selected"), function () {
        var id = $(this).val();
        userid.push(id);
    });

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

    if (EstimateHour == "" || EstimateHour == null) {
        $("#ErrorDv").empty();
        $(window).scrollTop(0);
        $("#ErrorDv").css("display", "block");
        $('#ErrorDv').css('color', 'red');
        $("#ErrorDv").html("<strong>Estimated hours is required !</strong>");
        $("#ErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }

    //if (userid.length == 0) {
    //    $("#ErrorDv").empty();
    //    $(window).scrollTop(0);
    //    $("#ErrorDv").css("display", "block");
    //    $('#ErrorDv').css('color', 'red');
    //    $("#ErrorDv").html("<strong>Please select atleast 1 OTRW !</strong>");
    //    $("#ErrorDv").delay(4000).fadeOut(function () {
    //    });
    //    return false;
    //}
    //if (OTRWRequired == "") {
    //    $("#ErrorDv").empty();
    //    $(window).scrollTop(0);
    //    $("#ErrorDv").css("display", "block");
    //    $('#ErrorDv').css('color', 'red');
    //    $("#ErrorDv").html("<strong>Please Select OTRW Required !</strong>");
    //    $("#ErrorDv").delay(4000).fadeOut(function () {
    //    });
    //    return false;
    //}

    $("#UpdateExtendJob").val("Updating...");
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/UpdateExtendJob" + '?' + '&AssignTo2=' + userid,
        data: { JobId: jobId, UpdateDateBooked: updateDate, EstimatedHours: EstimateHour },
        type: 'Get',
        async: false,
        success: function (data) {

            $(window).scrollTop(0);
            if (data.Status == "Success") {
                $("#modalExtendJobCalender").hide();
                $('.assign-job-msg').css('color', 'green');
                $('.assign-job-msg').html("Updated successfully");
                $('.assign-job-msg').show();
                window.setTimeout(function () {
                    $('.assign-job-msg').hide();
                    location.reload();
                }, 4000)
            }
            else {
                $("#UpdateExtendJob").val("Update");
                $("#ErrorDv").css("display", "block");
                $('#ErrorDv').css('color', 'red');
                $("#ErrorDv").html("<strong>" + data.Required + "</strong>");
                $("#ErrorDv").delay(4000).fadeOut(function () { });
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$(document).off("click", "#UpdateReAssignJob").on("click", "#UpdateReAssignJob", function () {
    var updateDate = $("#DateBookedCalender").val();
    updateDate = new Date(updateDate).toLocaleDateString();
    datepick = $('#calendar').fullCalendar('getDate');
    datepick = new Date(datepick).toLocaleDateString();
    var newAssignto = $("#tempAssignTo").val();
    var jobId = $('#jobIdCal').val();
    var oldAssignto = $("#assignIdCal").val();
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
    if (newAssignto == "") {
        $("#ErrorDv").empty();
        $(window).scrollTop(0);
        $("#ErrorDv").css("display", "block");
        $('#ErrorDv').css('color', 'red');
        $("#ErrorDv").html("<strong>Please Select OTRW User!</strong>");
        $("#ErrorDv").delay(4000).fadeOut(function () {
        });
        return false;
    }
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/UpdateAssignJob",
        data: { JobId: jobId, UpdateDateBooked: updateDate, CurrentDate: datepick, NewAssignTo: newAssignto, OldAssignTo: oldAssignto },
        type: 'Get',
        async: false,
        success: function (data) {
            $('.assign-job-message').css('color', 'green');
            $('.assign-job-message').html("Updated successfully");
            $('.assign-job-message').show();
            window.setTimeout(function () {
                $("#modalReAssignedJobPopUp").hide();
            }, 4000)
            location.reload();
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$('#trackUser').unbind('click');
$("#trackUser").bind('click', function () {
    $('#trackUser').text("Loading...");
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/_GetTrackUserPopup",
        data: {},
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowReschduleCalender").empty();
            $("#divShowReschduleCalender").html(data);
            $("#modalTrackUserPopUp").modal("show");
            $('#trackUser').text("Track User");
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

$("#TemplateMessageId").change(function () {
    var tempmessage = $("#TemplateMessageId option:selected").text();
    $("#Note").val(tempmessage);
});

$(document).on("click", '.fc-icon-left-single-arrow', function () {//prev
    if (Ismonthselect) {
        Ismonthselect = true;
        IsDaySelected = false;
        IsweekSelected = false;
        Getmonthyear();
    }
    else if (IsweekSelected) {
        Ismonthselect = false;
        IsDaySelected = false;
        IsweekSelected = true;
        GetweeksAvailableHour();
    }
    else if (IsDaySelected) {
        Ismonthselect = false;
        IsDaySelected = true;
        IsweekSelected = false;
        GetDaysAvailableHours();
    }
});

$(document).on("click", '.fc-icon-right-single-arrow', function () {//next

    if (Ismonthselect) {
        Ismonthselect = true;
        IsDaySelected = false;
        IsweekSelected = false;
        Getmonthyear();
    }
    else if (IsweekSelected) {
        Ismonthselect = false;
        IsDaySelected = false;
        IsweekSelected = true;
        GetweeksAvailableHour();
    }
    else if (IsDaySelected) {
        Ismonthselect = false;
        IsDaySelected = true;
        IsweekSelected = false;
        GetDaysAvailableHours();
    }
});

$(document).on("click", '.fc-agendaWeek-button', function () {//prev
    GetweeksAvailableHour();
});

$(document).on("click", '.fc-agendaDay-button', function () {//prev

    GetDaysAvailableHours();
});

$(document).on("click", '.fc-month-button', function () {//month
    Getmonthyear();
});
$(document).off("click", "#btncrossOrder").on("click", '#btncrossOrder', function () {
    Dashboard.SearchJob()
});


//******************Function to get employee jobs**********************************
function GetEmployyeJobs(sortedorder) {
    $.ajax({
        url: common.SitePath + '/Admin/Dashboard/GetEmployeeJobsInfo',
        type: "GET",
        dataType: "JSON",
        async: false,
        data: {
            param: sortedorder
        },
        success: function (data) {
            var calenderList = data.CalenderList;
            var response = jQuery.parseJSON(data.list);
            $(".divjob").html("");
            var jobs = "<ul>";
            for (i = 0; i < data.length; i++) {
                var jobtype = response[i]["JobType"];
                var jobstatus = response[i]["Status"];
                var EmployeeId = response[i]["EmployeeId"];
                switch (jobtype) {
                    case 1:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip2"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                 '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                        break;
                    case 2:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip3"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '"jobIntId="' + response[i]["JobNo"] +
                                                 '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                    case 3:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip4"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                             '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                    case 4:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip8"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                 '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                    default:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '"  jobIntId="' + response[i]["JobNo"] +
                            '" jobId="' + response[i]["Id"] + '"S><span class=""></span><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small> <small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                }
            }
            jobs = jobs + "</ul>";
            $(".divjob").append(jobs);

            if (calenderList.length > 0) {
                Dashboard.RenderCalender(calenderList[0]["EmployeeList"], calenderList[0]["EventDetail"]);
            }
            else {
                Dashboard.RenderCalender([], []);
            }
            Dashboard.externalevents();
        }
    });
}

//******************Function for waning Message Popup*******************************
function WarningMsgPopUp(WarningReason) {    //Show Error Msg 
    $(".modalWarningMsg").modal('show');
    $(".alertwarningmsg").text(WarningReason);
    $(".modal-title").html("Warning !");
}

//******************Function for assigning a job************************************
function AssignJob(date, JobId) {                        // Job Assign 
    $(".assign-job-div").empty();
    var assigneddate = new Date(date).toLocaleDateString();

    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/AssignJob",
        data: {
            JobId: JobId, AssignedDate: assigneddate
        },
        type: 'Get',
        async: false,
        success: function (data) {
            $(".assign-job-div").html(data);
            $("#modalAssigntootrw").modal("show");
        },
        error: function (data) {
            alert("something seems wrong");
        }
    });
}

//**********Function to check if any job assign on that particular time************
function CheckAssignJobExist(assignedTo, date, starttime, endtime, jobid) {
    //  Check Assign job Exist
    if (starttime.toString() > 24) {
        starttime = 0;
    }

    var output = false;
    var notAssignedReason = '';
    var assigneddate = date;
    jobid = jobid == '' || jobid == undefined ? '00000000-0000-0000-0000-000000000000' : jobid;

    var data = { 'AssignTo': assignedTo, 'AssignedDate': assigneddate, 'StartTime': starttime, 'EndTime': endtime, 'JobId': jobid };
    jQuery.ajax({
        url: FSM.UserHasAssignJobUrl,
        data: data,
        success: function (result) {
            if (result.CheckAssignJobExist == true) {
                output = true;
                notAssignedReason = result.Reason;
            }
        },
        async: false
    });
    return { output: output, notAssignedReason: notAssignedReason };
}

//**********Function to check if any job already exists****************************
function JobExist(assignedTo, date, starttime, endtime, jobid) {
    if (starttime.toString() > 24 || starttime.toString() == "O") {
        starttime = 0;
    }
    var output = false;
    var notAssignedReason = '';
    var assigneddate = date;
    jobid = jobid == '' || jobid == undefined ? '00000000-0000-0000-0000-000000000000' : jobid;
    var sourceID = Dashboard.CurrentSourceId;

    var data = { 'AssignTo': assignedTo, 'AssignedDate': assigneddate, 'StartTime': starttime, 'EndTime': endtime, 'SourceId': sourceID, 'JobId': jobid };
    jQuery.ajax({
        url: FSM.UserHasJobUrl,
        data: data,
        success: function (result) {
            //Dashboard.CurrentSourceId = "";
            if (result.JobExist == true) {
                output = true;
                notAssignedReason = result.Reason;
            }
        },
        async: false
    });
    return { output: output, notAssignedReason: notAssignedReason };
}

//**********Function to assign job to OTRW uesr*************************************
function AssignJobToOTRW(date, jobId, assignedTo, starttime, endtime) {
    var assigneddate = date;
    var currentResourceId = Dashboard.CurrentTileResourceId;
    var formdata = new FormData();
    formdata.append("AssignedDate", assigneddate);
    formdata.append("Assigned_To", assignedTo);
    formdata.append("JobId", jobId);
    formdata.append("StartTime", starttime);
    formdata.append("EndTime", endtime);
    formdata.append("CurrentResourceId", currentResourceId);

    $.ajax({
        url: FSM.AsignJobUrl,
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        async: false,
        success: function (result) {
            $(window).scrollTop(0);
            $('.assign-job-msg').css('color', 'green');
            $('.assign-job-msg').html(result.msg);
            $('.assign-job-msg').show();
            window.setTimeout(function () {
                $('.assign-job-msg').hide();
            }, 4000)
            //location.reload();
        },
        error: function () {
            alert('something went wrong !');
        }
    });
}

//**********Function to check if user is supervisor of a job*************************
function checkSuperVisor(JobId, UserId) {
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/_ShowSuperVisorPopUp",
        data: { JobId: JobId },
        type: 'Get',
        async: false,
        success: function (data) {
            $("#divShowReschduleCalender").empty();
            $("#divShowReschduleCalender").html(data);
            $("#modalCreateSuperVisor").modal("show");
            Dashboard.SuperVisorOTRWId = UserId;
            Dashboard.SuperVisorJobId = JobId;
            GetDaysAvailableHours();
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

//**********Function to show job into************************************************
var DELAY = 700, clicks = 0, timer = null;
function Showjobinfo(job_id, JobId, e) {
    var dashboardCalendarDate = $('#calendar').fullCalendar('getDate');   //Get Current Date Calender And Save Into Cookies

    $.cookie('saveCalanderDate', dashboardCalendarDate._d, { path: '/' });

    if (job_id == '' || job_id == '' || job_id == '0' || job_id == 'undefined') {
        return false;
    }
    clicks++;  //count clicks
    if (clicks === 1) {
        timer = setTimeout(function () {
            var isInt = parseInt(job_id);
            if (isNaN(isInt)) {
                var jobidindex = job_id.indexOf('=');
                job_id = job_id.substring(jobidindex + 1);
            }
            Dashboard.initialize(10);
            $.ajax({
                url: common.SitePath + '/Admin/Dashboard/GeJobinfoByJobId',
                type: "GET",
                dataType: "JSON",
                data: {
                    JobId: job_id, ID: JobId
                },
                success: function (data) {
                    var response = jQuery.parseJSON(data.list);
                    Dashboard.IsJob = 1;

                    $(".jobinfo").html("");
                    var jobdiv = "<div><a href='#accordion3' aria-expanded='false' aria-controls='accordion3' class='accordionClose accordion-title accordionTitle js-accordionTrigger'>Job Description</a></div>";
                    jobdiv = jobdiv + "<div class='outer_table'>";
                    jobdiv += '<ul>';
                    for (i = 0; i < data.length; i++) {

                        jobdiv += '<li>';
                        jobdiv += "<label>Site Notes</label><span>" + response[i]["SiteNotes"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Job Notes</label><span>" + response[i]["JobNotes"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Customer Notes</label><span>" + response[i]["CustomerNotes"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Operation Notes</label><span>" + response[i]["OperationNotes"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>File Name</label><span>" + response[i]["CustomerName"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Address</label><span>" + response[i]["Address"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Contact Name</label><span>" + response[i]["ContactName"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Contact No</label><span>" + response[i]["ContactNo"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Site Name</label><span>" + response[i]["StreetName"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Job Type</label><span>" + response[i]["JobTypeName"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Work Type</label><span>" + response[i]["WorkTypeName"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Prefer Time</label><span>" + response[i]["PreferTimeText"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Created By</label><span>" + response[i]["BookedByName"] + "</span>";
                        jobdiv += '</li>';
                        //jobdiv += '<li>';
                        //jobdiv += "<label>Job Notes</label><span>" + response[i]["JobNotes"] + "</span>";
                        //jobdiv += '</li>';
                        //jobdiv += '<li>';
                        //jobdiv += "<label>Operation Notes</label><span>" + response[i]["OperationNotes"] + "</span>";
                        //jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Job No</label><span>" + response[i]["JobNo"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Date Booked</label><span>" + response[i]["Datetimetext"] + "</span>";
                        jobdiv += '</li>';
                        jobdiv += '<li>';
                        jobdiv += "<label>Status</label><span>" + response[i]["StatusText"] + "</span>";
                        jobdiv += '</li>';

                        Dashboard.sitename = '<b>JobId: </b>' + response[i]["JobNo"] + '<br/><b>Site Detial: </b>' + response[i]["jobAddress"];
                        if (/^\s+$/.test(Dashboard.sitename)) {
                            Dashboard.sitename = "";
                            //string contains only whitespace
                        }

                        if (response[i]["Latitude"] != 0)
                            Dashboard.lat = response[i]["Latitude"];
                        else
                            Dashboard.lat = 38.889931;
                        if (response[i]["Longitude"] != 0)
                            Dashboard.lng = response[i]["Longitude"];
                        else
                            Dashboard.lng = -77.009003;

                        if (Dashboard.map != null) {
                            $("#accordion2").css("width", "51%");
                            $("#map-canvas").css("width", $("#map-canvas").parent().width());
                            $("#map-canvas").css("margin", "15px 0");
                            if (Dashboard.marker != null) {
                                Dashboard.marker.setMap(null);
                            }
                            google.maps.event.trigger(Dashboard.map, 'resize');
                            Dashboard.map.setZoom(15);
                            Dashboard.map.setCenter(new google.maps.LatLng(Dashboard.lat, Dashboard.lng));

                            if (response[i]["Latitude"] != 0 && response[i]["Latitude"] != null && response[i]["Longitude"] != 0 && response[i]["Longitude"] != null) {
                                Dashboard.marker = new google.maps.Marker({
                                    position: new google.maps.LatLng(Dashboard.lat, Dashboard.lng),
                                    map: Dashboard.map,
                                    title: 'click to see site detial',
                                    content: Dashboard.sitename
                                });

                                Dashboard.marker.infowindow = new google.maps.InfoWindow();
                                Dashboard.marker.addListener('click', function (event) {
                                    this.infowindow.setPosition(event.latLng);
                                    this.infowindow.setContent(this.content);
                                    this.infowindow.open(Dashboard.map, this);
                                });
                            }
                            else {
                                Dashboard.map.setCenter(new google.maps.LatLng(-30.000233, 136.209152));
                                Dashboard.map.setZoom(3);
                            }
                        }
                        jobdiv = jobdiv + "</ul>";
                        $(".jobinfo").append(jobdiv);
                    }
                    $('#accordion2').removeClass('is-collapsed');
                    $('#accordion3').removeClass('is-collapsed');
                }
            });

            clicks = 0;             //after action performed, reset counter

        }, DELAY);

    } else {
        clearTimeout(timer);//prevent single-click action
        clicks = 0;
        window.location = common.SitePath + '/Employee/CustomerJob/SaveJobInfo/' + JobId + '?Module=Jobs';
        //window.open(common.SitePath + '/Employee/CustomerJob/SaveJobInfo/' + JobId + '?Module=Jobs');
    }
}

//**********Function to save end time************************************************
function SaveEndTime(JobNo, EndTime, StartTime, ResourceId) {
    debugger;
    $.ajax({
        type: 'POST',
        url: common.SitePath + "/Admin/Dashboard/SaveEndTime",
        data: {
            JobId: JobNo, EndTime: EndTime, StartTime: StartTime, ResourceId: ResourceId
        },
        dataType: "JSON",
        async: false,
        success: function (data) {
            Dashboard.JobExist();
        }
    });
}

//**********Function to job on the particular date************************************
function GetJobsOnDate(calendarDate) {
    if (calendarDate._d != undefined) {
        if (calendarDate == "" || calendarDate == undefined) {
            calendarDate = ($.datepicker.formatDate('dd M yy', new Date()))
        }
        else {
            calendarDate = ($.datepicker.formatDate('dd M yy', calendarDate._d))
        }
    }
    else {
        var date = new Date(calendarDate);
        calendarDate = ($.datepicker.formatDate('dd M yy', date))
        $("#calendar").fullCalendar('gotoDate', calendarDate);
    }
    var srchkey = $("#dashboardserchbox").val();
    var JobtypeVal = $('#JobTypes').val();
    if (JobtypeVal == "") {
        JobtypeVal = "null";
    }
    var StatusVal = $('#ddljobStatus').val();
    if (StatusVal == "") {
        StatusVal = "null";
    }

    $.ajax({
        type: 'GET',
        url: common.SitePath + "/Admin/Dashboard/GetEmployeeJobsUsingJobType",
        data: {
            Jobtype: JobtypeVal, Status: StatusVal, selectdate: calendarDate, searchkey: srchkey
        },
        dataType: "JSON",
        async: false,
        success: function (data) {
            var response = jQuery.parseJSON(data.list);
            $(".divjob").html("");
            var jobs = "<ul>";
            for (i = 0; i < data.length; i++) {
                var jobtype = response[i]["JobType"];
                switch (jobtype) {
                    case 1:

                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip2"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                 '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';


                        //    jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip2"><a title="' + response[i]["title"] + '"ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '" ><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                    case 2:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip3"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                              '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                        //jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip3"><a title="' + response[i]["title"] + '"ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '"><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                    case 3:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip4"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                                  '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                        //jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip4"><a title="' + response[i]["title"] + '"ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '"><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                    case 4:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip8"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                              '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                        // jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event colored_strip8"><a title="' + response[i]["title"] + '"ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '"><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                    default:
                        jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event"><a title="' + response[i]["title"] + '" ExpantTime="' + response[i]["EstimatedHrsPerUser"] + '" jobIntId="' + response[i]["JobNo"] +
                                              '" jobId="' + response[i]["Id"] + '"><p style="border:1px solid black;color:red;float:right;margin-top: -16px;padding:2px;">' + response[i]["WetRequiredName"] + ',' + response[i]["StoreysName"] + ',' + response[i]["DisplayOTRWAssignCount"] + '</p><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small> <small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small><small><b>Suburb:&nbsp&nbsp</b>' + response[i]["suburb"] + ' </small></a></li>';
                        //  jobs = jobs + '<li onclick="Showjobinfo(' + response[i]["JobNo"] + ',\'' + response[i]["Id"] + '\');" class="fc-event"><a title="' + response[i]["title"] + '" jobIntId="' + response[i]["JobNo"] + '" jobId="' + response[i]["Id"] + '"><span class=""></span><p>Job # ' + response[i]["JobNo"] + '</p><small><b>Required OTRW:&nbsp&nbsp</b>' + response[i]["OTRWRequired"] + ' </small><small><b>Assign OTRW:&nbsp&nbsp</b>' + response[i]["OTRWAssignCount"] + ' </small><small><b>File Name:&nbsp&nbsp</b>' + response[i]["CustomerName"] + ' </small><small><b>Address:&nbsp&nbsp</b>' + response[i]["Address"] + ' </small><small><b>Work Type:&nbsp&nbsp</b>' + response[i]["WorkTypeName"] + ' </small><small><b>Job Type:&nbsp&nbsp</b>' + response[i]["JobTypeName"] + ' </small><small><b>Status:&nbsp&nbsp</b>' + response[i]["StatusText"] + ' </small><small><b>Date:&nbsp&nbsp</b>' + response[i]["Datetimetext"].split(' ')[0] + ' </small></a></li>';
                        break;
                }
            }
            jobs = jobs + "</ul>";
            $(".divjob").append(jobs);
            var calenderList = data.CalenderList;
            if (calenderList.length > 0) {

                $('#calendar').fullCalendar('removeEvents');
                $('#calendar').fullCalendar('addEventSource', calenderList[0]["EventDetail"]);
                $('#calendar').fullCalendar('rerenderEvents');
                // Dashboard.RenderCalender(calenderList[0]["EmployeeList"], calenderList[0]["EventDetail"]);
                Dashboard.externalevents();
            }
            else {
                $('#calendar').fullCalendar('removeEvents');
                // $('#calendar').fullCalendar('addEventSource', calenderList[0]["EventDetail"]);
                $('#calendar').fullCalendar('rerenderEvents');
                Dashboard.RenderCalender([], []);

            }
            if (StatusVal != "5") {
                Dashboard.externalevents();
            }
        }

    });
}

//**********Function to bind job by customerId****************************************
function BindJobByCustomerId(CustomerId) {
    $.ajax({
        url: common.SitePath + "/Admin/Dashboard/GetJobByCustomerId",
        data: { CustomerGeneralInfoId: CustomerId },
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (result) {
            $("#Job_Id").empty();
            $("#SiteId").empty();
            for (var i = 0; i < result.JobList.length; i++) {
                var opt = new Option(result.JobList[i].Text, result.JobList[i].Value);
                $("#Job_Id").append(opt);
            }
            for (var i = 0; i < result.SiteNameList.length; i++) {
                var opt = new Option(result.SiteNameList[i].Text, result.SiteNameList[i].Value);
                $("#SiteId").append(opt);
            }
        },
        error: function () {
            alert("something seems wrong");
        }
    });
}

//**********Function to get availabe hours of a month*********************************
function Getmonthyear() {
    Ismonthselect = true;
    IsDaySelected = false;
    IsweekSelected = false;
    var monthyear = $('.fc-center').text();
    mothyr = monthyear.split(' ');
    var month = mothyr[0];
    var year = mothyr[1];
    var monthNumber = new Date(Date.parse(month + " 1, 2012")).getMonth() + 1;
    $.ajax({
        url: common.SitePath + '/Admin/Dashboard/GetAvailableHourMonths',
        type: "GET",
        dataType: "JSON",
        data: {
            Month: monthNumber, Year: year
        },
        success: function (data) {
            var response = jQuery.parseJSON(data.list);
            if (response.length > 0) {
                for (i = 0; i < response.length; i++) {
                    var RoofHoursAvailable = response[i]["RoofHoursAvailable"];
                    var CleaningHoursAvailable = response[i]["CleaningHoursAvailable"];
                    $(".RoofHoursAvailable").text("Roof Hours Available: " + RoofHoursAvailable);
                    $(".CleaningHoursAvailable").text("Cleaning Hours Available: " + CleaningHoursAvailable);
                }
            }
            else {
                $(".RoofHoursAvailable").text("Roof Hours Available: " + 0);
                $(".CleaningHoursAvailable").text("Cleaning Hours Available: " + 0);
            }

        },
        error: function () {

        }
    });
}

//**********Function to get available hour of a week**********************************
function GetweeksAvailableHour() {
    IsDaySelected = false;
    Ismonthselect = false;
    IsweekSelected = true;

    var weekrange = $('.fc-center').text();
    var daysrange = weekrange.split('–');
    var firstmonthrange = daysrange[0].split(' ');
    var firstmonth = firstmonthrange[0];
    firstmonth = new Date(Date.parse(firstmonth + " 1, 2012")).getMonth() + 1;
    var firstdate = firstmonthrange[1];

    var Secondmonthrange = daysrange[1].split(' ')
    var EndDate = '';
    var year = '';
    var secondmonth = '';
    if (Secondmonthrange.length == 4) {
        EndDate = Secondmonthrange[2].replace(',', '');
        year = Secondmonthrange[3];
        secondmonth = Secondmonthrange[1];

        secondmonth = new Date(Date.parse(secondmonth + " 1, 2012")).getMonth() + 1;
    }
    else {
        EndDate = Secondmonthrange[1].replace(',', '');;
        year = Secondmonthrange[2];
        secondmonth = firstmonth;
        secondmonth = new Date(Date.parse(secondmonth + " 1, 2012")).getMonth() + 1;
    }


    $.ajax({
        url: common.SitePath + '/Admin/Dashboard/GetAvailableHoursWeek',
        type: "GET",
        dataType: "JSON",
        data: {
            firstdate: firstdate, seconddate: EndDate, firstmonth: firstmonth, secondmonth: secondmonth, Year: year, Formatteddata: weekrange
        },
        success: function (data) {
            var response = jQuery.parseJSON(data.list);
            if (response.length > 0) {
                for (i = 0; i < response.length; i++) {
                    var RoofHoursAvailable = response[i]["RoofHoursAvailable"];
                    var CleaningHoursAvailable = response[i]["CleaningHoursAvailable"];
                    $(".RoofHoursAvailable").text("Roof Hours Available: " + RoofHoursAvailable);
                    $(".CleaningHoursAvailable").text("Cleaning Hours Available: " + CleaningHoursAvailable);
                }
            }
            else {
                $(".RoofHoursAvailable").text("Roof Hours Available: " + 0);
                $(".CleaningHoursAvailable").text("Cleaning Hours Available: " + 0);
            }

        },
        error: function () {

        }
    });
}

//**********Function to get available hours of a Day************************************
function GetDaysAvailableHours() {

    IsDaySelected = true;
    Ismonthselect = false;
    IsweekSelected = false;
    var monthyear = $('.fc-center').text();
    mothyr = monthyear.split(' ');
    var month = mothyr[0];
    var date = mothyr[1].replace(',', '');
    var year = mothyr[2];
    var monthNumber = new Date(Date.parse(month + " 1, 2012")).getMonth() + 1;


    $.ajax({
        url: common.SitePath + '/Admin/Dashboard/GetAvailableHoursDays',
        type: "GET",
        dataType: "JSON",
        data: {
            Year: year, Month: monthNumber, Day: date
        },
        success: function (data) {
            var response = jQuery.parseJSON(data.list);
            if (response.length > 0) {

                for (i = 0; i < response.length; i++) {
                    var RoofHoursAvailable = response[i]["RoofHoursAvailable"];
                    var CleaningHoursAvailable = response[i]["CleaningHoursAvailable"];
                    $(".RoofHoursAvailable").text("Roof Hours Available: " + RoofHoursAvailable);
                    $(".CleaningHoursAvailable").text("Cleaning Hours Available: " + CleaningHoursAvailable);
                }
            }
            else {
                $(".RoofHoursAvailable").text("Roof Hours Available: " + 0);
                $(".CleaningHoursAvailable").text("Cleaning Hours Available: " + 0);
            }
        },
        error: function () {

        }
    });
}