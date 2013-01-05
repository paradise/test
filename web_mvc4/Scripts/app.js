function isVisible(elem) {
    return $(elem).css('display') != 'none';
};

var TC = {
    triggerEvents: function () {
        $('html, body').click(function (ev) {
            if ($(ev.target).parents('#login').length === 0) {
                var elem = $('#login-popup');
                if (isVisible(elem)) {
                    elem.toggle();
                }
            }
        });
    },

    showLogin: function () {
        $('#login-popup').toggle();
    }
};

var cons = {
    write: function (s) {
        var ct = $('#console-text'),
            cs = $('#console-layout');
        ct.append('<p> >' + s + '</p>');
        cs.scrollTop(ct.height());
    }
}

$(function () {
    TC.triggerEvents();
});
