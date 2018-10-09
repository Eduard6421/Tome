$('.entry').hover(function() {
    var li = $('li', this);
    for (var i = 0; i < $(li).length; i++) {
        $($(li)[i]).stop().animate({
            'height': '18px'
        }, 300 + (i * 150));
    }
}, function() {
    $('li', this).stop().animate({
        'height': '5px'
    }, 250);
});