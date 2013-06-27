$(function () {

	$(document).ready(function () {
		// bind 'myForm' and provide a simple callback function 
		var options = {
			success: function () { window.location.href = "/"; },
			error: function (ex) {
				
				if (ex.responseText) {
					var error = JSON.parse(ex.responseText);
					if (error && error.Error && error.Error.Message)
						alert("Error occured while submitting report. \nDetails: "+error.Error.Message);
				}
			}
		};


		$('#submitform').ajaxForm(options);
	});

});