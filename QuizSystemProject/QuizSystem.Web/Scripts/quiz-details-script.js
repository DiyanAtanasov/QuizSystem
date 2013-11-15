(function () {
    var commentsForm = document.getElementById('post-comment-form');
    var commentsGrid = document.getElementById("comments-view-container");

    if (commentsForm) {
        eventUtils.setEvent(commentsForm, 'submit', function (evt) {
            if (!javascriptValidator.validateContainerElement(this, 2)) {
                eventUtils.preventDefault(evt);
                return;
            }

            var loader = document.createElement('div');
            loader.className = 'loader';

            var that = this;
            repository.allQuizzes.postComment(htmlUtils.extractFormData(this)).then(
                function (s) {
                    commentsGrid.innerHTML = s;
                    var txt = htmlUtils.findChild(that, { tagName: 'TEXTAREA' });

                    if (txt) { txt.value = '' }

                }, function (e) {
                    htmlUtils.overlayMessage({ message: e, isError: true, level: 2, update: true });
                });

            htmlUtils.showInOverlayBox(loader, 2, commentsGrid);
            eventUtils.preventDefault(evt);
        });
    }

    eventUtils.setEvent(commentsGrid, "click", function (evt) {
        if (evt.target.tagName == 'A') {
            var that = this;

            var loader = document.createElement('div');
            loader.className = 'loader';

            repository.allQuizzes.getComments(evt.target.getAttribute('href').split('?')[1]).then(
                     function (s) {
                         that.innerHTML = s;
                     },
                     function (e) {
                         htmlUtils.overlayMessage({ message: e, isError: true, level: 2, update: true });
                     });

            htmlUtils.showInOverlayBox(loader, 2, that);

            eventUtils.preventDefault(evt);
        }
    });
}());