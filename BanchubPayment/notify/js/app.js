
$('ok').click( function() {
	$('.popovers').popover();
});

window.setTimeout(function() {
	$(".alert").fadeTo(2000, 500).slideUp(500, function(){
		$(this).remove(); 
	});
// 500 : Time will remain on the screen
}, 500);
