$(document).ready(function() {
	// Header Scroll
	//$(window).on('scroll', function() {
	//	var scroll = $(window).scrollTop();

	//	if (scroll >= 50) {
	//		$('#header').addClass('fixed');
	//	} else {
	//		$('#header').removeClass('fixed');
	//	}
	//});

	// Waypoints
	$('.work').waypoint(function() {
		$('.work').addClass('animated fadeIn');
	}, {
		offset: '75%'
	});
	$('.download').waypoint(function() {
		$('.download .btn').addClass('animated tada');
	}, {
		offset: '75%'
	});
	
	$('.download-arrow').waypoint(function() {
		$('.download-arrow .img').addClass('animated flip');
	}, {
		offset: '75%'
	});
	$('.download-arrow1').waypoint(function() {
		$('.download-arrow1 .img').addClass('animated flip');
	}, {
		offset: '75%'
	});
	$('.download-arrow2').waypoint(function() {
		$('.download-arrow2 .img').addClass('animated flip');
	}, {
		offset: '75%'
	});
	$('.download-arrow3').waypoint(function() {
		$('.download-arrow3 .img').addClass('animated flip');
	}, {
		offset: '75%'
	});
	$('.download-arrow4').waypoint(function() {
		$('.download-arrow4 .img').addClass('animated flip');
	}, {
		offset: '75%'
	});

	// Fancybox
	$('.work-box').fancybox();

	// Flexslider
	$('.flexslider').flexslider({
		animation: "fade",
		directionNav: false,
	});

	// Page Scroll
	var sections = $('section')
		nav = $('nav[role="navigation"]');

	$(window).on('scroll', function () {
	  	var cur_pos = $(this).scrollTop();
	  	sections.each(function() {
	    	var top = $(this).offset().top - 76
	        	bottom = top + $(this).outerHeight();
	    	if (cur_pos >= top && cur_pos <= bottom) {
	      		nav.find('a').removeClass('active');
	      		nav.find('a[href="#'+$(this).attr('id')+'"]').addClass('active');
	    	}
	  	});
	});
	nav.find('a').on('click', function () {
	  	var $el = $(this)
	    	id = $el.attr('href');
		$('html, body').animate({
			scrollTop: $(id).offset().top - 75
		}, 500);
	  return false;
	});

	// Mobile Navigation
	$('.nav-toggle').on('click', function() {
		$(this).toggleClass('close-nav');
		nav.toggleClass('open');
		return false;
	});	
	nav.find('a').on('click', function() {
		$('.nav-toggle').toggleClass('close-nav');
		nav.toggleClass('open');
	});
	
	$(".content").hide();
	var defaultslide = "one"
	$("#content-"+defaultslide).show();
	var clickbtn = $(".features-menu a").get(0);
	$(clickbtn).addClass("activebtn");
	
	// *********** PAYMENT PAGE NAVIGATION SCRIPT ***********//

	$(".payment-content").hide();
	
	var firstcontent = $(".payment-content").get(0);
	$(firstcontent).show();
	
	$(".payment-tab").on('click', function() {
		$(".payment-content").hide();
		$(this).find(".payment-content").show();
	});
	
	$(".page_1").hide();
	$("#reg_1").show();
});

// *********** FEATURE PAGE NAVIGATION SCRIPT ***********//

function featurefunc(currentslide, btnid)
{
	$(".content").hide();
	$(".features-menu a").removeClass("activebtn");	
	var clickbtn = $(".features-menu a").get(btnid);
	$(clickbtn).addClass("activebtn");	
	$("#content-"+currentslide).show();
}
function show_reg(slide)
{
	$(".page_1").hide();
	$("#reg_"+slide).show();
}
