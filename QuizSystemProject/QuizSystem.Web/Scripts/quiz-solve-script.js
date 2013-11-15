(function () {
    var form = document.getElementById('quiz-solve-form');
    eventUtils.setEvent(form, 'submit', function (evt) {
        var elements = this.elements;

        var index = 0;
        var groupIndex;
        var isSelected;
        var allAnswered = true;

        while (elements[index]) {
            if (elements[index].type == 'radio') {
                groupIndex = index;
                isSelected = false;

                while (elements[groupIndex].type == 'radio'
                    && elements[groupIndex].name == elements[index].name) {

                    if (elements[groupIndex].checked) {
                        isSelected = true;
                    }

                    groupIndex++;
                }

                if (!isSelected) {
                    allAnswered = false;
                    elements[groupIndex - 1].parentElement.parentElement.parentElement.className = 'unanswered';
                }

                index = groupIndex;
                continue;
            }

            index++;
        }

        if (!allAnswered) {
            htmlUtils.overlayMessage({ isError: true, message: 'There is still unanswered questions.' });
            eventUtils.preventDefault(evt);
        }
    });
    eventUtils.setEvent(form, 'click', function (evt) {
        var el = evt.target;

        if (el.tagName != 'LI') {
            el = el.parentElement;
        }

        if (el.tagName == 'LI') {

            if (el.className == 'selected') {
                eventUtils.stopPropagation(evt);
                return;
            }

            var oldSelect = htmlUtils.findChild(el.parentElement, { className: 'selected' });
            if (oldSelect) { oldSelect.className = '' }

            el.className = 'selected';
            if (el.parentElement.parentElement.className) {
                el.parentElement.parentElement.className = ' ';
            }

            el = el.firstChild;
            while (el && el.type != 'radio') {
                el = el.nextSibling;
            }

            if (el) { el.checked = true; }

        }
    });
}())