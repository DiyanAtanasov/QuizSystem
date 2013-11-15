(function () {
    //grid filters form ajax
    eventUtils.setEvent(document.getElementById('filters-form'), 'submit', function (evt) {
        var loader = document.createElement('div');
        loader.className = 'loader';

        var gridContainer = document.getElementById("grid-container");
        repository.allQuizzes.updateGrid('?' + htmlUtils.extractFormData(this)).then(
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
            if (evt.target.innerHTML == 'Solve' || evt.target.innerHTML == 'Details') {
                return;
            }

            var that = this;

            var loader = document.createElement('div');
            loader.className = 'loader';

           repository.allQuizzes.updateGrid(evt.target.getAttribute('href').split('?')[1]).then(
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