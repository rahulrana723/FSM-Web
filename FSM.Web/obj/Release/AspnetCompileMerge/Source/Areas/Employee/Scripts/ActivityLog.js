$(document).ready(function () {
    $('#StartDate').datepicker({
        dateFormat: 'yy/mm/dd',
    });
    $('#EndDate').datepicker({
        dateFormat: 'yy/mm/dd',
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
});

$('#ddPageSize').on('change', function () {
    var page_size = $('#ddPageSize').val();
    window.location.href = FSM.URL + "?page_size=" + page_size;
});
