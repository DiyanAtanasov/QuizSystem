(function () {
    //update form
    var trigger = document.getElementById("quiz-create-trigger");
    var quizCreateForm = document.getElementById('quiz-create-form');
    var quizDeleteForm = document.getElementById('quiz-delete-form');
    eventUtils.setEvent(trigger, 'click', function () {
        if (!this.className) {
            quizCreateForm.className = 'active-form';
            quizDeleteForm.className = 'active-form';
            this.className = 'activated';
            this.innerHTML = 'Exit';
        } else {
            quizCreateForm.className = '';
            quizDeleteForm.className = '';
            this.className = '';
            this.innerHTML = 'Edit Quiz';
        }
    });

    //delete action
    eventUtils.setEvent(quizDeleteForm, 'submit', function (evt) {
        htmlUtils.showInOverlayBox(htmlUtils.confirmWindow('Do you want to DELETE this quiz ?',
            function () {
                htmlUtils.hideOverlayBox(4);
                quizDeleteForm.submit();
            }, function () {
                htmlUtils.hideOverlayBox(4);
            }),4);

        eventUtils.preventDefault(evt);
    });

    //validation
    eventUtils.setEvent(quizCreateForm, 'submit', function (evt) {
        if (!javascriptValidator.validateContainerElement(this, 2)) {
            eventUtils.preventDefault(evt);
            return;
        }

       repository.quizzes.update(htmlUtils.extractFormData(this)).then(
            function (s) {
                htmlUtils.overlayMessage({ toEl: quizCreateForm, autoHide: true, update: true, level: 10 });
                document.getElementById('quiz-title').innerHTML =
                    '"' + htmlUtils.escape(document.getElementById('Title').value) + '"';
            }, function (e) {
                htmlUtils.overlayMessage({ toEl:quizCreateForm, isError: true, message : e, update : true, level : 10 });
            });

        var loader = document.createElement('div');
        loader.className = 'loader';

        htmlUtils.showInOverlayBox(loader, 10, this.parentElement);
        eventUtils.preventDefault(evt);
    });
}())