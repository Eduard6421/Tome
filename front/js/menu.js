
window.onload = function () {
    $("nav").hide();
	$(".open-nav").click(function() {
		$("nav").slideToggle();
		$("nav ul").toggleClass("open");
		$(".open-nav").toggleClass("close");
	});
	console.log('yo');
};