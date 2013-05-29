$(function() {

	$('#save-config').click(function() {
		var form = $('#edit-configuration-form');
		$.post('/-/save-configuration', form.serialize())
			.success(function() {
				alert('Configuration saved successfully!');
			})
			.fail(function() {
				alert('Error while saving configuration');
			});
	});

});