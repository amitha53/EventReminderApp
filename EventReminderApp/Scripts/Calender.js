$(document).ready(function () {
    var events = [];
    var eventSelected = null;
    FetchEventAndRenderCalender();
    function FetchEventAndRenderCalender() {
        events = [];
        $.ajax({
            type: "GET",
            url: "/Account/GetEvents",
            success: function (data) {
                $.each(data, function (i, v) {
                    events.push({
                        eventID: v.EventID,
                        //userID: v.UserID,
                        title: v.Subject,
                        description: v.Description,
                        start: moment(v.StartDate),
                        end: moment(v.EndDate),
                    });
                });
                // console.log(events);
                GenerateCalender(events);
            },
            error: function (error) {
                alert('failed');
            }
        });
    }

    function GenerateCalender(events) {
        console.log(events);
        $('#calender').fullCalendar({
            contentHeight: 400,
            defaultDate: new Date(),
            timeFormat: 'h(:mm)a',
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,basicWeek,basicDay,agenda'
            },
            eventLimit: true,
            eventColor: 'green',
            events: [
                {
                    title: 'Meeting',
                    description: 'Zoom meeting at 10 am',
                    start: '2020-06-15 10:10 am',
                    end: '2020-06-16 10:00 am'
                },
                {
                    title: 'Bday of sis',
                    description: 'buy gift',
                    start: '2020-06-18 04:10 pm',
                    end: '2020-06-19 04:10 pm'
                }
            ],
           eventClick: function (calEvent, jsEvent, view) {
               // $('#myModal #eventTitle').text(calEvent.title);

               // $('#myModal #pDetails').text(calEvent.description);

               // $('#myModal').modal();
            }
        })

    }

    $('#btnEdit').click(function () {
        openEditForm();
    });

    function openEditForm() {
        if (eventSelected != null) {
            $('#hdEventID').val(eventSelected.eventID);
            // $('#hdUserID').val(eventSelected.userID);
            $('#txtSubject').val(eventSelected.title);
            $('#txtDescription').val(eventSelected.description);
            $('#dtStart').val(eventSelected.start.format("MM-DD-YYYY HH:mm a"));
            $('#dtEnd').val(eventSelected.end.format("MM-DD-YYYY HH:mm a"));
        }
        $('#myModal').modal('hide');
        $('#myModalEdit').modal();
    }

    $('#btnSave').click(function () {
        if ($('#txtSubject').val().trim() == "") {
            alert('Subject required');
            return;
        }
        if ($('#dtStart').val().trim() == "") {
            alert('Start date required');
            return;
        }
        if ($('#dtEnd').val().trim() == "") {
            alert('End date required');
            return;
        }
        else {
            var startDate = moment($('#dtStart').val(), "MM-DD-YYYY HH:mm a").toDate();
            var endDate = moment($('#dtEnd').val(), "MM-DD-YYYY HH:mm a").toDate();
            if (startDate > endDate) {
                alert('Invalid end date');
                return;
            }
        }

        var data = {
            EventID: $('#hdEventID').val(),
            //UserID: $('#hdUserID').val(),
            Subject: $('#txtSubject').val().trim(),
            Description: $('#txtDescription').val(),
            StartDate: $('#dtStart').val(),
            EndDate: $('#dtEnd').val()
        }
        console.log(data);
        // var datatosend = JSON.stringify(data);
        SaveEvent(data);
    });
    function SaveEvent(data) {
        console.log(data);
        $.ajax({
            type: "POST",
            url: "/Account/SaveEvent",
            data: data,
            success: function (data) {
                if (data.status) {
                    FetchEventAndRenderCalender();
                    $('#myModalSave').modal('hide');
                }

            },
            error: function () {
                alert('Failed');
            }
        });
    }

    $('#btnDelete').click(function () {
        if (eventSelected != null && confirm('Are you sure?')) {
            $.ajax({
                type: "POST",
                url: "/Account/DeleteEvent",
                data: { 'eventID': eventSelected.eventID },
                success: function (data) {
                    if (data.status) {
                        FetchEventAndRenderCalender();
                        $('#myModal').modal('hide');
                    }
                },
                error: function () {
                    alert('Failed');
                }
            });
        }
    });
});