var colors = ["#A4BDFC", "#7AE7BF", "#DBADFF", "#FF887C", "#FBD75B", "#FFB878", "#46D6DB", "#E1E1E1", "#5484ED", "#51B749","#DC2127"];
$(function () {

   
  
    $("#calendar").fullCalendar(
        {
            customButtons: {
                myCustomButton: {
                    text: 'Нова подія',
                    click: function () {
                        $("#newEventForm").dialog(
                            {
                                show: {
                                    effect: "drop",
                                    duration: 500
                                },
                                hide: {
                                    effect: "drop",
                                    duration: 50
                                }
                            }
                        );
                    }
                }
            },
            header: {
                left: 'title myCustomButton'
            },
            dayClick: function () {
               
            },
            viewRender: function () {
                Load();
            },
            eventClick:function(calEvent) {

                $.get("/Calendar/GetEvent", { Id: calEvent.id }).done(
                    function (data) {
                        $("input[name='EventId']").val(data.Id);
                    ///Start DateTime
                        {
                            var date = new Date(data.Start.DateTime.match(/\d+/)[0] * 1);
                            var month = date.getMonth()+1;
                            if ((month + "").length == 1) {
                                month = "0" + month + "";
                            }
                            var day = date.getDate();
                            if ((day + "").length == 1) {
                                day = "0" + day + "";
                            }
                            $("#editStartDate").val(date.getFullYear() + "-" + month + "-" + day);
                            
                                h = date.getHours(),
                                m = date.getMinutes();
                            if (h < 10) h = '0' + h;
                            if (m < 10) m = '0' + m; 
                            $("#editStartTime").val(h+":"+m);
                        }
                    ////End DateTime
                        {
                            
                            var dateE = new Date(data.End.DateTime.match(/\d+/)[0] * 1);
                            var monthE = dateE.getMonth()+1;
                            if ((monthE + "").length == 1) {
                                monthE = "0" + monthE + "";
                            }
                            var dayE = dateE.getDate();
                            if ((dayE + "").length == 1) {
                                dayE = "0" + dayE + "";
                            }
                            $("#editEndDate").val(dateE.getFullYear() + "-" + monthE + "-" + dayE);

                            h = dateE.getHours(),
                                m = dateE.getMinutes();
                            if (h < 10) h = '0' + h;
                            if (m < 10) m = '0' + m;
                            $("#editEndTime").val(h + ":" + m);

                        }
                        $("#editLocation").val(data.Location);
                        $("#editDesc").val(data.Summary);
                        var colorId = data.ColorId;
                        $("#editColorId").children("option[value='" + colorId + "']").attr("selected", "selected");
                        $("#editColorId").change();
                    }
                );

                $("#editEventForm").dialog();
                

            }
        }
    );

    $("#deleteEventBtn").click(
        function () {
            var EventId = $("input[name='EventId']").val();
            $.get("/Calendar/DeleteEvent", { EventId: EventId }).done(
                function () {
                    location.reload();
                }
            );
            return false;
        }
    );

    function Load() {
        var dt = $("#calendar").fullCalendar('getDate');
        var date = new Date(dt).toLocaleDateString();
        var dateArr = date.split(".");
        $.get("/Calendar/LoadEvents", { Year: dateArr[2], Month: dateArr[1] }).done(
            function (data) {
                
                for (var i = 0; i < data.length; i++) {
                    $('#calendar').fullCalendar('renderEvent', {
                        id:data[i].Id,
                        title: data[i].Summary,
                        start: data[i].Start.DateTimeRaw,
                        end: data[i].End.DateTimeRaw,
                        color: colors[data[i].ColorId-1]
                    });
                }
            }
        );
    }
  

    $(".ColorId").change(
        function () {
            var color = $(this).children("option:selected").css("background-color");
          
            $(".color").css("background-color",color);
        }
    );

   
});