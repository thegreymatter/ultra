$(function() {

	$('#save-config').click(function() {
		var form = $('#edit-configuration-form');
		$.post('/-/save-configuration', form.serialize());
	});

});