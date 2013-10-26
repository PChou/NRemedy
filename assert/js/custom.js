$(function(){
	
	var opt = { text : window.location.href, width:100, height:100 };
	if(!Modernizr.canvas){
		$.extend(opt,{ render : "table" });
	}

	$('#gEwm').html('').qrcode(opt);

})