$(function () {
    $("#tabs").tabs();
    if (FSM.CustomerGeneralInfoId == "" || FSM.CustomerGeneralInfoId == undefined || FSM.CustomerGeneralInfoId == "null") {
        $("#tabs").tabs("disable", 1);
        $("#tabs").tabs("disable", 2);
        $("#tabs").tabs("disable", 3);
        $("#tabs").tabs("disable", 4);
        $("#tabs").tabs("disable", 5);
        $("#tabs").tabs("disable", 6);
        $("#tabs").tabs("disable", 7);
    }
    else {
        $("#tabs").tabs("enable", 1);
        $("#tabs").tabs("enable", 2);
        $("#tabs").tabs("enable", 3);
        $("#tabs").tabs("enable", 4);
        $("#tabs").tabs("enable", 5);
        $("#tabs").tabs("enable", 6);
        $("#tabs").tabs("enable", 7);
        
        if (FSM.ActiveTab =="General Info") {
            $("#tabs").tabs('option', 'active', 0);
        }
        else if (FSM.ActiveTab == "Customer Sites") {
            $("#tabs").tabs('option', 'active', 1);
        }
        else if (FSM.ActiveTab == "Contacts") {
            $("#tabs").tabs('option', 'active', 2);
        }
        else if (FSM.ActiveTab =="Billing Address") {
            $("#tabs").tabs('option', 'active', 3);
        }
        else if (FSM.ActiveTab == "Contact") {
            $("#tabs").tabs('option', 'active', 4);
        }
        else if (FSM.ActiveTab == "Documents") {
            $("#tabs").tabs('option', 'active', 5);
        }
        else if (FSM.ActiveTab == "Job History") {
            $("#tabs").tabs('option', 'active', 6);
        }
        else if (FSM.ActiveTab == "Invoice History") {
            $("#tabs").tabs('option', 'active', 7);
        }
    }
});