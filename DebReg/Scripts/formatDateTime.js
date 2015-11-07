var formatDateTime = function () {
    $(function () {
        $('[data-datetime]', document).each(function () {
            // the date construct will automatically convert to local time
            var localDate = new Date(parseInt($(this).attr('data-datetime')));

            $(this).html(localDate.toLocaleDateString() + " " + localDate.toLocaleTimeString());
        });
        $('[data-date]', document).each(function () {
            // the date construct will automatically convert to local time
            var localDate = new Date(parseInt($(this).attr('data-date')));

            $(this).html(localDate.toLocaleDateString());
        });
    });
}
formatDateTime();
