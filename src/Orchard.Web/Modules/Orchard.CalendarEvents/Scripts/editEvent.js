$(function () {

    var allDayEventInput = $('#Event_AllDayEvent');

    allDayEventInput.on('click', function () {
        checkAllDayEvent();
    });

    var $startDatePicker = $('#startDatePicker').pickadate({
        editable: true
    });
    var $startTimePicker = $('#startTimePicker').pickatime({
        editable: true
    });
    var $eventStartDate = $('#EventStartDate');

    var $endDatePicker = $('#endDatePicker').pickadate({
        editable: true
    });
    var $endTimePicker = $('#endTimePicker').pickatime({
        editable: true
    });
    var $eventEndDate = $('#EventEndDate');

    var startDatePicker = $startDatePicker.pickadate('picker');
    var startTimePicker = $startTimePicker.pickatime('picker');
    var endDatePicker = $endDatePicker.pickadate('picker');
    var endTimePicker = $endTimePicker.pickatime('picker');

    startDatePicker.on({
        close: function () {
            if (this.get('select')) {
                $eventStartDate.val(getDate(startDatePicker, startTimePicker));
                endDatePicker.set('min', this.get('select'));
            } else {
                $eventStartDate.val(null);
                startTimePicker.clear();
                endDatePicker.set('min', false);
            }
        }
    });

    startTimePicker.on({
        close: function () {
            $eventStartDate.val(getDate(startDatePicker, startTimePicker));
        }
    });

    endDatePicker.on({
        close: function (thingSet) {
            if (this.get('select')) {
                startDatePicker.set('max', this.get('select'));
                $eventEndDate.val(getDate(endDatePicker, endTimePicker));
            } else {
                startDatePicker.set('max', false);
                endTimePicker.clear();
                $eventEndDate.val(null);
            }
        }
    });
    endTimePicker.on({
        close: function () {
            $eventEndDate.val(getDate(endDatePicker, endTimePicker));
        }
    });

    function checkAllDayEvent() {
        console.log('checked all day event');
        if (allDayEventInput.is(":checked")) {
            $(".timePicker").hide();
            if (startDatePicker.get('select'))
                $eventStartDate.val(startDatePicker.get('select', 'm/d/yyyy') + " 12:00 AM");
            if (endDatePicker.get('select'))
                $eventEndDate.val(endDatePicker.get('select', 'm/d/yyyy') + " 12:00 AM");
        } else {
            $(".timePicker").show();
            if (startDatePicker.get('select'))
                $eventStartDate.val(getDate(startDatePicker, startTimePicker));
            if (endDatePicker.get('select'))
                $eventEndDate.val(getDate(endDatePicker, endTimePicker));
        }   
    }

    function getDate(datePicker, timePicker) {
        var stringToConvert = null;

        if (!datePicker.get('select') && timePicker.get('select'))
            datePicker.set('select', true);

        if (datePicker.get('select'))
            stringToConvert = datePicker.get('select', 'm/d/yyyy');

        if (timePicker.get('select')) {
            stringToConvert += " " + timePicker.get('select', 'h:i A');
        }
        else if (!allDayEventInput.is(":checked") && datePicker.get('select')) {
            stringToConvert += " 12:00 AM";
        }

        return stringToConvert;
    }

    checkAllDayEvent();
    
    //Check if dates exist on load (editing an item)
    if ($eventStartDate.val() != '1/1/0001 12:00:00 AM' && $eventStartDate.val() != '') {
        startDatePicker.set('select', new Date($eventStartDate.val()));
        startTimePicker.set('select', new Date($eventStartDate.val()));
    }
    if ($eventEndDate.val() != '1/1/0001 12:00:00 AM' && $eventEndDate.val() != '') {
        endDatePicker.set('select', new Date($eventEndDate.val()));
        endTimePicker.set('select', new Date($eventEndDate.val()));
    }
});