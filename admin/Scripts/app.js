
function loadNamespace() {
	var namespace = $('input[name=namespace]').val();
	$.ajax({
		type: 'POST',
		url: '/RegexBuilder/LoadNamespace',
		data: { id: namespace },
		success: function (data) {
			console.log(JSON.parse(data));
		}
	});
}