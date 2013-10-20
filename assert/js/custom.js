function showqrcode() {
	var opt = { text : window.location.href,width:100,height:100 };
	$('#gEwm').show();
	$('#ewm').hide();
	if(!Modernizr.canvas){
		$.extend(opt,{ render : "table" });
	}
	if($('#qrcode').html() == ''){
		$('#qrcode').qrcode(opt);
	}
	return false;
}

$(function(){
	
	$('#close').click(function(e){
		e.preventDefault();
		$('#gEwm').hide();
		$('#ewm').show();
		return false;
	});

	$('#ewm').click(function(e){
		e.preventDefault();
		var opt = { text : window.location.href,width:100,height:100 };
		$('#gEwm').show();
		$('#ewm').hide();
		if(!Modernizr.canvas){
			$.extend(opt,{ render : "table" });
		}
		if($('#qrcode').html() == ''){
			$('#qrcode').qrcode(opt);
		}
		return false;
	});

	$('#gEwm').hide();
})