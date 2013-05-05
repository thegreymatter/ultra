$(function() {

	$('#start-load-run').click(function() {
		// TODO: add basic validation
		$.ajax({
			url: '/-/start-run',
			type: 'POST',
			data: {
				filename: $('#loadrun_script').val(),
				domain: $('#loadrun_domain').val(),
				duration: $('#loadrun_duration').val(),
				rampup: $('#loadrun_rampup').val()
			},
			success: function() {

			},
			error: function() {

			}
		});
	});

});