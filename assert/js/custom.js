$(function(){
	
	var opt = { text : window.location.href, width:150, height:150 };
	if(!Modernizr.canvas){
		$.extend(opt,{ render : "table" });
	}

	$('#gEwm').html('').qrcode(opt);

	
})