window.ShowDialog = function () {
    var dialog = document.getElementById('my-dialog').showModal();
};

window.potvrdaBrisanja = function () {
    return confirm('Da li ste sigurni da želite da obrišete ovaj proizvod?');
};

