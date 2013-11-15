var categoriesCrudAjax = (function () {
    var root;

    var actions = {
        create: function (el) {
            if (!javascriptValidator.validateContainerElement(el, 2)) {
                return;
            }

            var newCat = {};
            newCat.name = htmlUtils.findChild(el, { tagName: 'INPUT', type: 'text' }).value;
            var loader = document.createElement('div');
            loader.className = 'loader';
            htmlUtils.showInOverlayBox(loader);

            repository.categories.update(newCat).then(
                function (s) {
                    htmlUtils.overlayMessage({ autoHide: true, update: true });

                    root.appendChild(document.createElement('li'));
                    root.lastChild.innerHTML = s;

                    var old = document.getElementById('selected');
                    if (old) {
                        old.removeAttribute('id');
                    };

                    root.lastChild.id = 'selected';
                    root.lastChild.scrollIntoView(true);

                    htmlUtils.findChild(el, { tagName: 'INPUT', type: 'text' }).value = '';
                }, function (e) {
                    htmlUtils.overlayMessage({ message: e, isError: true, update: true });
                }
                );
        },
        edit: function (el) {
            var old = document.getElementById('selected');
            if (old) {
                old.removeAttribute('id');
            };
            el.parentElement.parentElement.id = 'selected';
        },
        update: function (el) {
            if (!javascriptValidator.validateContainerElement(el, 2)) {
                return;
            }

            var newCat = {};
            newCat.id = el.parentElement.id.substring(2);
            newCat.name = htmlUtils.findChild(el, { tagName: 'INPUT', type: 'text' }).value;

            var loader = document.createElement('div');
            loader.className = 'loader';
            htmlUtils.showInOverlayBox(loader);

            repository.categories.update(newCat).then(
                function (s) {
                    htmlUtils.overlayMessage({ autoHide: true, update: true });
                    el.parentElement.parentElement.id = '';
                    el.parentElement.parentElement.innerHTML = s;
                }, function (e) {
                    htmlUtils.overlayMessage({ message: e, isError: true, update: true });
                });
        },
        'delete': function (el) {
            var content = htmlUtils.findChild(el, { tagName: 'P' }).innerHTML;
            var confirmation = htmlUtils.confirmWindow("Do you want to DELETE Category '" + content + "' ?",
                function () {
                    var loader = document.createElement('div');
                    loader.className = 'loader';
                    htmlUtils.updateOverlayBox(loader);

                    repository.categories.remove({
                        id: el.parentElement.id.substring(2),
                        name: content
                    }).then(
                        function (s) {
                            htmlUtils.overlayMessage({ autoHide: true, update: true });
                            root.removeChild(el.parentElement.parentElement);
                        }, function (e) {
                            htmlUtils.overlayMessage({ message: e, isError: true, update: true });
                        });
                }, function () {
                    htmlUtils.hideOverlayBox();
                });

            htmlUtils.showInOverlayBox(confirmation);
        },
        cancel: function (el) {
            el.parentElement.parentElement.id = '';
        }
    };


    function init(listId) {
        root = document.getElementById(listId);

        eventUtils.setEvent(root, 'click', function (evt) {
            if (evt.target.type == 'button') {
                var command = evt.target.value.toLowerCase();
                if (actions[command]) {
                    actions[command](evt.target.parentElement);
                }
            }
        });
    }

    return {
        init: init
    }
}());