var OrderList = {


    // for up button
    ArrowUp: function (EmployeeId, Order) {
        var Type = "Up";
        $.ajax({
            url: common.SitePath + "/Admin/Dashboard/UpdateSetOrder",
            data: { OTRWID: EmployeeId, OrderType: Type, CurrentOrderNo: Order },
            type: 'Get',
            async: false,
            success: function (data) {

            },
            error: function () {
                alert("something seems wrong");
            }
        });
    },

    //for down button
    ArrowDown: function (EmployeeId, Order) {
        var Type = "Down";
        $.ajax({
            url: common.SitePath + "/Admin/Dashboard/UpdateSetOrder",
            data: { OTRWID: EmployeeId, OrderType: Type, CurrentOrderNo: Order },
            type: 'Get',
            async: false,
            success: function (data) {

            },
            error: function () {
                alert("something seems wrong");
            }
        });
    }
}


$(document).ready(function () {

    var $table = $(".arrowUp").parent().parent().parent().parent().parent();                
    $table.find('tbody').find('tr:first').find('.arrowUp').attr('disabled', 'disabled');       //disbled first up arrow
    $table.find('tbody').find('tr:last').find('.arrowDown').attr('disabled', 'disabled');      //disbled last down arrow

    $(".arrowUp,.arrowDown").click(function () {
        var EmployeeId = $(this).attr('Employeeid');
        var Order = $(this).attr('Order');
        var row = $(this).parents("tr:first");
        if ($(this).is(".up")) {
            row.insertBefore(row.prev());  //row insert Up side
            OrderList.ArrowUp(EmployeeId, Order)
        } else {
            row.insertAfter(row.next());  //row insert Down Side
            OrderList.ArrowDown(EmployeeId, Order);
        }
        $('.arrowUp').removeAttr('disabled');
        $('.arrowDown').removeAttr('disabled');
        var $table = $(".arrowUp").parent().parent().parent().parent().parent();
        $table.find('tbody').find('tr:first').find('.arrowUp').attr('disabled', 'disabled');          //disbled first up arrow
        $table.find('tbody').find('tr:last').find('.arrowDown').attr('disabled', 'disabled');           //disbled down up arrow
    });
});
$('#modalSetOrder').modal({
    backdrop: 'static',
    keyboard: false
});
