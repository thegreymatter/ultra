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

	$('#load-runs-history .delete').click(function() {
		var loadRunId = $(this).parents('tr').data('loadrunid');
		if (confirm('Are you sure you want to delete this load run ?')) {
			$.ajax({
				url: '/-/delete-run',
				type: 'POST',
				data: { loadRunId: loadRunId },
				success: function() {

				},
				error: function() {

				}
			});
		}
	});

});