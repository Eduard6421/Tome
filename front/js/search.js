
window.onload = function () {

    // $(".search-result").hide();
    $(".search-result-summary-container").hide();
    $(".tome-description").show();
    
	$(".hide-panel").click(function(e) {
		e.preventDefault();

		$(this).parent().parent().find(".tome-description").slideToggle();
		$(this).parent().parent().find(".search-result-summary-container").slideToggle();
	});

};