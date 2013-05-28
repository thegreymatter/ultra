$(function() {

	$('#analyze-output').click(function() {
		// TODO: add basic validation
		var analyzeForm = $('#analyze-output-form');
		$.post(analyzeForm[0].action, analyzeForm.serialize())
			.success(function() {
				window.location.href = '/';
			})
			.fail(function() {
				alert('Error: failed to analyze output file!');
			});
	});

});