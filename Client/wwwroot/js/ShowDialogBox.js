window.ShowDialog = function () {
    var dialog = document.getElementById('my-dialog');
    if (!dialog.showModal) {
        dialogPolyfill.registerDialog(dialog);
    }
    dialog.showModal();
};