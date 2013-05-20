$(function() {

	$('#save-config').click(function() {
		$.post('/-/save-configuration', $('#edit-configuration-form').serialize());
	});

});