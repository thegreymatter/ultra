$(function () {


	
	$('#start-load-run').click(function () {
		// TODO: add basic validation

		var servers = [];
		$('.servers-list input').each(function (index, element) {
			var serverCheckbox = $(element);
			if (serverCheckbox.prop('checked'))
				servers.push(serverCheckbox.attr('id').replace('server_', ''));
		});

		$.ajax({
			url: '/-/start-run',
			type: 'POST',
			traditional: true,
			data: {
				filename: $('#loadrun_script').val(),
				domain: $('#loadrun_domain').val(),
				duration: $('#loadrun_duration').val(),
				rampup: $('#loadrun_rampup').val(),
				servers: servers
			},
			success: function () {
				$('.running-load').show();
			},
			error: function () {

			}
		});
	});

	$('#load-runs-history .delete').click(function () {
		var loadRunRow = $(this).parents('.load-run');
		var loadRunId = $(this).parents('tr').data('loadrunid');
		if (confirm('Are you sure you want to delete this load run ?')) {
			$.ajax({
				url: '/-/delete-run',
				type: 'POST',
				data: { loadRunId: loadRunId },
				success: function () {
					loadRunRow.next('.load-run-details').remove();
					loadRunRow.remove();
				},
				error: function () {

				}
			});
		}
	});

	$('tr.load-run').click(function () {
		var detailsRow = $(this).next('.load-run-details');
		if (detailsRow.hasClass('hidden')) {
			detailsRow.removeClass('hidden');
			drawDistributionChart(detailsRow);
		} else
			detailsRow.addClass('hidden');

	});

	$('div.runlabel').editable(function (value, settings) {
		var id = $(this).parents('tr').data('loadrunid');
		$.ajax({
			url: '/-/save-label',
			type: 'POST',
			data: { newLabel: value, loadRunId: id },
			success: function () {

			},
			error: function () {
				
			}
		});
		return (value);
	}, {
		submit: 'Save',
	});
	
	$('div.runremarks').editable(function (value, settings) {
		var id = $(this).parents('tr').data('loadrunid');
		$.ajax({
			url: '/-/save-remark',
			type: 'POST',
			data: { newRemark: value, loadRunId: id },
			success: function () {

			},
			error: function () {

			}
		});
		return (value);
	}, {
		submit: 'Save',
		type: 'textarea',
		rows: 5,
		placeholder:'Click to enter remarks'
	});

	$('tr.load-run-row .show-run-details').click(function () {
		var runOutputFile = $(this).parents('.load-run-row').data('output-file');
		window.location.href = '/show-analysis/' + runOutputFile;
	});
	
	function drawDistributionChart(row) {
		var data = new google.visualization.DataTable();
		data.addColumn('string', 'Thread Group');
		data.addColumn('number', 'Requests Count');
		

		var options = {
			'title': 'Requests Distribution',
			'legend': { position: 'left', textStyle: { color: 'blue', fontSize: 10 } },

			'width': 600,
			'height': 450
		};
		
		var runOutputFile = $(row.context).data('output-file');
		$.ajax({
			url: '/distribution-graph/' + runOutputFile,
			type: 'POST',
			success: function (response) {
				data.addRows(response);
				var distributionChart = new google.visualization.PieChart(row.find('#distribution_pie').get(0));
				distributionChart.draw(data, options);
				
			},
			error: function (response) {
				row.find('#distribution_pie').text("Errror occured while loading Distribution chart");
			}
		});



	}

});