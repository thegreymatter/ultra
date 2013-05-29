$(function() {

	$('#analyze-output').click(function(result) {
		// TODO: add basic validation
		var analyzeForm = $('#analyze-output-form');
		$.post(analyzeForm[0].action, analyzeForm.serialize())
			.success(function() {
				window.location.href = '/';
			})
			.fail(function(ex) {
				if (ex.responseText) {
					var error = JSON.parse(ex.responseText);
					if (error && error.Error && error.Error.Message)
						alert(error.Error.Message);
				}
			});
	});

});