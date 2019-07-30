$(document).ready(function () {
    $(".ddlmultiselect").multiselect();

    var gmarkers = [];
    var map;
   initialize();

    function initialize() {
        var mapProp = {
            center: new google.maps.LatLng(20.593684, 78.96288), //India Lat and Lon
            zoom: 2,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
    }

    setTimeout(
    function () {
       google.maps.event.trigger(map, "resize");
    }, 500);
   

   // google.maps.event.addDomListener(window, 'load', initialize);

    $("#srchTrackUser").click(function () {
        var userid = [];
        $.each($("#tempAssignTo2 option:selected"), function () {
            var id = $(this).val();
            if (id != "") {
                userid.push("''" + id + "''");
            }
            else {
                userid.push("" + id + "");
            }
        });

        for (i = 0; i < gmarkers.length; i++) {
            gmarkers[i].setMap(null);
        }

        $.ajax({
            type: "POST",
            url: common.SitePath + "/Admin/Dashboard/trackMapLatitudeLongitude" + '?' + '&UserId=' + userid + "",
            contentType: "application/json; charset=utf-8",
            data: {},
            dataType: "json",
            success: function (result) {
                for (i = 0; i < result.length; i++) {
                    var latlng = new google.maps.LatLng(result[i].Latitude, result[i].Longitude);
                    Title = result[i].UserName;
                    var marker = new google.maps.Marker({
                        position: latlng,
                        title: Title,
                        map: map,
                        draggable: true,
                    });
                    map.setCenter(latlng);
                    map.setZoom(5);
                    gmarkers.push(marker);

                    var contentString = "<b>Employee Id:</b>" + result[i].EID + "</br>" + "<b>User Name:</b>" + result[i].UserName;
                    var infowindow = new google.maps.InfoWindow();
                    google.maps.event.addDomListener(marker, 'click', (function (marker, contentString,latlng, infowindow) {
                        return function () {
                            infowindow.setPosition(latlng);
                            infowindow.setContent(contentString);
                            infowindow.open(map);
                        };
                    })(marker,contentString,latlng ,infowindow));
                }
            }
        });
    })
});
