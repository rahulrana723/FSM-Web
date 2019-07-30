$(document).ready(function () {

    $('.ddlmultiselect')
   .multiselect({
       //  allSelectedText: 'All',
       maxHeight: 200,
       includeSelectAllOption: true
   })
   //.multiselect('selectAll', ture)
   .multiselect('updateButtonText');
    $('.ddlmultiselectjob')
  .multiselect({
      //  allSelectedText: 'All',
      maxHeight: 200,
      includeSelectAllOption: true, nonSelectedText: 'Select Job'   
  })
  //.multiselect('selectAll', ture)
  .multiselect('updateButtonText');
    $('#StartDate').datepicker({
        dateFormat: 'dd/mm/yy'
    });
    $('#EndDate').datepicker({
        dateFormat: 'dd/mm/yy'
    });

    $(".showhidedate").css("display", "none");
    $(".showhidejob").css("display", "none");
    $(".empdiv").css("display", "none");
    $(".showhideoperations").css("display", "none");

    var ReportType = $("#ReportType").val();
    
        if (ReportType == "2") {
            $(".showhidedate").css("display", "block");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "block");
            $(".showhideoperations").css("display", "none");
        }
       else if (ReportType == "6") {
            $(".showhidedate").css("display", "block");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "block");
            $(".showhideoperations").css("display", "none");
        }
        else if (ReportType == "1") {
            $(".showhidedate").css("display", "none");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "block");
            $(".showhideoperations").css("display", "none");
        }

        else if (ReportType == "3") {
            $(".showhidedate").css("display", "block");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "none");
            $(".showhideoperations").css("display", "none");
        }
        else if (ReportType == "4") {

            $(".showhidedate").css("display", "block");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "none");
            $(".showhideoperations").css("display", "none");
        }

        else if (ReportType == "5") {
            $(".showhidedate").css("display", "none");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "none");
            $(".showhideoperations").css("display", "block");
        }
    
    $("#ReportType").change(function () {
        var reportType = $(this).val();
        if(reportType=="2")
        {
            $(".showhidedate").css("display", "block");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "block");
            $(".showhideoperations").css("display", "none");
        }
        else if (reportType == "6") {
            $(".showhidedate").css("display", "block");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "block");
            $(".showhideoperations").css("display", "none");
        }
        else if(reportType=="1")
        {
            $(".showhidedate").css("display", "none");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "block");
            $(".showhideoperations").css("display", "none");
            }
        else if (reportType == "3") {
            $(".showhidedate").css("display", "block");
            $(".empdiv").css("display", "none");
            $(".showhidejob").css("display", "none");
            $(".showhideoperations").css("display", "none");
        }

        else if (reportType == "4") {

          
            $(".showhidedate").css("display", "block");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "none");
            $(".showhideoperations").css("display", "none");

        }

        else if (reportType == "5") {
            $(".showhidedate").css("display", "none");
            $(".showhidejob").css("display", "none");
            $(".empdiv").css("display", "none");
            $(".showhideoperations").css("display", "block");
        }
    })
});