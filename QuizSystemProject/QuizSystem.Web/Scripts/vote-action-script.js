(function () {
    var voteAction = document.getElementById('vote-action');
    if (!voteAction) { return; }

    eventUtils.setEvent(voteAction, 'click', function (evt) {
        if (evt.target.tagName == 'A') {
            eventUtils.preventDefault(evt);

            repository.allQuizzes.vote(evt.target.getAttribute('href').split('?')[1]).then(
                function (s) {
                    var el = document.getElementById('votes-count');
                    var oldVal = parseInt(el.innerHTML);
                    el.innerHTML = oldVal + parseInt(s);

                    htmlUtils.overlayMessage({ autoHide: true, level: 1, update: true });
                    evt.target.parentElement.style.visibility = 'hidden';
                },
                function (e) {
                    htmlUtils.overlayMessage({ level: 1, update: true, isError: true, message: e });
                });

            var loader = document.createElement('div');
            loader.className = 'loader';

            htmlUtils.showInOverlayBox(loader);
        }
    });
}());