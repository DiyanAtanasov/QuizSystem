(function () {
    //create form UI
    var trigger = document.getElementById("quiz-create-trigger");
    var quizCreateForm = document.getElementById('quiz-create-form');

    eventUtils.setEvent(trigger, 'click', function () {
        if (!this.className) {
            quizCreateForm.className = 'active-form';
            this.className = 'activated';
            this.innerHTML = 'Exit';
        } else {
            quizCreateForm.className = '';
            this.className = '';
            this.innerHTML = 'New Quiz';
        }
    });

    //create form ajax
    eventUtils.setEvent(quizCreateForm, 'submit', function (evt) {
        if (!javascriptValidator.validateContainerElement(this, 2)) {
            eventUtils.preventDefault(evt);
            return;
        }

        var loader = document.createElement('div');
        loader.className = 'loader';

        repository.quizzes.create(htmlUtils.extractFormData(this)).then(
            function (s) {
                window.location = s;
            }, function (e) {
                htmlUtils.overlayMessage({ isError: true, message: e, level: 3, update: true });
            });

        htmlUtils.showInOverlayBox(loader, 3, this.parentElement);
        eventUtils.preventDefault(evt);
    });

    //grid filters form ajax
    eventUtils.setEvent(document.getElementById('filters-form'), 'submit', function (evt) {
        var loader = document.createElement('div');
        loader.className = 'loader';

        var gridContainer = document.getElementById("grid-container");
        repository.quizzes.updateGrid('?' + htmlUtils.extractFormData(this)).then(
            function (s) {
                gridContainer.innerHTML = s;
            }, function (e) {
                htmlUtils.overlayMessage({ isError: true, message: e, level: 3, update: true });
            });

        htmlUtils.showInOverlayBox(loader, 3, gridContainer);
        eventUtils.preventDefault(evt);
    });

    //grid ajax
    eventUtils.setEvent(document.getElementById("grid-container"), "click", function (evt) {
        if (evt.target.tagName == 'A') {
            if (evt.target.innerHTML == 'Edit' ||
                evt.target.innerHTML == 'Preview' ||
                evt.target.innerHTML == 'Details') {
                return;
            }

            var that = this;

            var loader = document.createElement('div');
            loader.className = 'loader';

           repository.quizzes.updateGrid(evt.target.getAttribute('href').split('?')[1]).then(
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