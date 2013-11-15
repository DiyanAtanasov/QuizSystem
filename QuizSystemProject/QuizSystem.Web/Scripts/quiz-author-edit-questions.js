var QuestionsManager = Class.create(function (quizId) {
    this.quizId = quizId;
    this.data = [];
    this.repository = repository.questions(this.quizId);
    this.container = document.createElement('ul');
    this.container.id = 'questions-list';
    this.createButton = this.getCreateButton();
    this.actionPanel = this.getActionPanel();
    this.questionsCountContainer;
    this.popUp = this.initPopUp();
    this.active = {};
    if (!navigator.appVersion.match(/.*MSIE 7\.0;.*/)) {
        this.setEvents();
    } else {
        this.setClickEvents();
        this.container.className += ' clickable';
    }
}, {
    render: function () {
        var that = this;
        this.repository.all().then(
            function (s) {
               htmlUtils.hideOverlayBox();
                that.data = JSON.parse(s);
                that.questionsCountContainer.innerHTML = that.data.length;
                for (var i = 0; i < that.data.length; i++) {
                    that.renderQuestion(i);
                }
            }, function (e) {
               htmlUtils.overlayMessage({ isError: true, message: 'Questions failed to load!', update: true, level: 1 });
            });
            
        var l = document.createElement('div');
        l.className = 'questions-loader';
        l.innerHTML = 'Loading questions...';
        htmlUtils.showInOverlayBox(l, 1, this.container.parentElement);
    },
    renderQuestion: function (index) {
        this.container.appendChild(document.createElement('li'));
        this.container.lastChild.id = 'q-' + index;
        this.container.lastChild.appendChild(document.createElement('div'));
        this.container.lastChild.lastChild.className = 'clearfix';
        this.container.lastChild.lastChild.appendChild(document.createElement('h4'));
        this.container.lastChild.lastChild.firstChild.innerHTML = index + 1;
        this.container.lastChild.lastChild.appendChild(document.createElement('p'));
        this.addText(this.container.lastChild.lastChild.lastChild, this.data[index].content);
    },
    updateQuestion: function (index, question) {
        if (index || index == 0) {
            this.data[index] = question;
            this.addText(this.container.childNodes[index].firstChild.lastChild, this.data[index].content);
        } else {
            this.data.push(question);
            this.renderQuestion(this.data.length - 1);
        }
    },
    deleteQuestion: function (index) {
        if (this.data.length == 100) {
            this.createButton.style.display = '';
        }
        this.data.splice(index, 1);
        for (var i = index; i < this.data.length; i++) {
            this.container.childNodes[i].firstChild.firstChild.innerHTML = i + 1;
            this.addText(this.container.childNodes[i].firstChild.lastChild, this.data[i].content);
        }
        this.questionsCountContainer.innerHTML = this.data.length;
        this.container.removeChild(this.container.lastChild);

    },
    initPopUp: function () {
        var that = this;

        var container = document.createElement('div');
        container.id = 'question-pop-up';

        container.appendChild(document.createElement('div'));
        container.lastChild.className = 'clearfix';
        container.lastChild.appendChild(document.createElement('span'));
        container.lastChild.lastChild.innerHTML = 'Total Answers : ';
        container.lastChild.appendChild(document.createElement('span'));
        container.lastChild.lastChild.className = 'total-answers';

        container.appendChild(document.createElement('div'));
        container.lastChild.className = 'answer-box clearfix';
        container.lastChild.appendChild(document.createElement('span'));
        container.lastChild.lastChild.innerHTML = 'Right Answer :';
        container.lastChild.appendChild(document.createElement('div'));
        container.lastChild.lastChild.className = 'answer-wrapper';
        container.lastChild.lastChild.appendChild(document.createElement('p'));

        container.appendChild(document.createElement('div'));
        container.lastChild.className = 'action-panel clearfix';
        container.lastChild.appendChild(htmlUtils.button('Edit', function () {
            var index = parseInt(container.parentElement.id.substring(2));

            var panel = questionEditPanel.create(
                that.questionToModel(that.data[index]), function (updated) {
                    var loader = document.createElement('div');
                    loader.className = 'loader';
                    htmlUtils.showInOverlayBox(loader, 2);
                   
                    that.repository.update(updated).then(
                        function (newQuestion) {
                            htmlUtils.overlayMessage({ level: 2, autoHide: true, update: true });
                            that.updateQuestion(index, JSON.parse(newQuestion));
                            that.updatePopUp(index);
                            htmlUtils.hideOverlayBox();
                        }, function (error) {
                            htmlUtils.overlayMessage({ message: error, level: 2, isError: true, update: true });
                        });
                }, function () {
                    htmlUtils.hideOverlayBox();
                });

            htmlUtils.showInOverlayBox(panel);
        }));

        container.lastChild.appendChild(htmlUtils.button('Delete', function () {
            var confirmWindow = htmlUtils.confirmWindow('Do you want to DELETE this Question ?',
            function () {
                var loader = document.createElement('div');
                loader.className = 'loader';
                htmlUtils.showInOverlayBox(loader);
                htmlUtils.showContent(container, true);

                var index = parseInt(container.parentElement.id.substring(2));
                that.repository.remove(that.data[index]).then(
                       function () {
                           htmlUtils.overlayMessage({ autoHide: true, update: true });
                           that.deleteQuestion(index);
                           eventUtils.fireEvent(container, 'mouseout');
                       }, function (e) {
                           htmlUtils.overlayMessage({ message: e, isError: true, update: true });
                       });
            }, function () {
                htmlUtils.showContent(container, true);
            });

            htmlUtils.hideContent(container, true);
            container.appendChild(confirmWindow);
        }));

        return container;
    },
    updatePopUp: function (index) {
        this.popUp.childNodes[0].lastChild.innerHTML = this.data[index].answers.length;
        this.popUp.childNodes[1].lastChild.lastChild.innerHTML = "";
        for (var i = 0; i < this.data[index].answers.length; i++) {
            if (this.data[index].answers[i].id == this.data[index].rightId) {
                this.popUp.childNodes[1].lastChild.lastChild.innerHTML =
                            htmlUtils.escape(this.data[index].answers[i].content);
            }
        }

        if (this.popUp.lastChild.className == 'confirm-window') {
            this.popUp.removeChild(this.popUp.lastChild);
            htmlUtils.showContent(this.popUp, true);
        }
    },
    getCreateButton: function () {
        var that = this;
        var button = htmlUtils.button('Create New Question', function () {
            var newQuestion = {};
            newQuestion.id = 0;
            var panel = questionEditPanel.create(newQuestion, function (created) {
                var loader = document.createElement('div');
                loader.className = 'loader';
                htmlUtils.showInOverlayBox(loader, 2);

                that.repository.create(created).then(
                    function (s) {
                        that.updateQuestion(null, JSON.parse(s));
                        if (that.data.length > 100) {
                            that.createButton.style.display = 'none';
                        }
                        that.questionsCountContainer.innerHTML = that.data.length;
                        htmlUtils.overlayMessage({ level: 2, autoHide: true, update: true });
                        htmlUtils.hideOverlayBox();
                    }, function (e) {
                        htmlUtils.overlayMessage({ message: e, isError: true, level: 2, update: true });
                        htmlUtils.hideOverlayBox();
                    });
            }, function () {
                htmlUtils.hideOverlayBox();
            })
            htmlUtils.showInOverlayBox(panel);
        });

        button.className = 'large-btn submit';
        return button;
    },
    addText: function (el, content) {
        el.innerHTML = htmlUtils.escape(content);
    },
    getActionPanel : function () {
        var container = document.createElement('div');
        container.className = 'questions-info';
        
        container.appendChild(document.createElement('span'));
        container.lastChild.innerHTML = ' Questions';
        container.appendChild(document.createElement('span'));
        container.lastChild.className = 'questions-count';
        this.questionsCountContainer = container.lastChild;
        container.appendChild(this.createButton);

        return container;
    },
    questionToModel: function (question) {
        var obj = {};
        obj.id = question.id;
        obj.content = question.content;
        obj.answers = [];
        for (var i = 0; i < question.answers.length; i++) {
            obj.answers.push({
                content: question.answers[i].content,
                isCorrect: question.rightId == question.answers[i].id ? true : false,
                id: question.answers[i].id
            });
        }

        obj.answers.sort(function (x, y) {
            return x.content > y.content ? 1 : -1;
        });

        return obj;
    },
    setEvents: function () {
        var that = this;
        eventUtils.setEvent(this.container, 'mouseover', function (evt) {
            var el = evt.target;
            for (var i = 0; i < 3; i++) {
                if (!el) { break; }

                if (el.tagName == 'LI') {
                    if (el.id != that.active.id) {
                        that.active.className = ' ';
                        that.active = el;
                        that.active.className = 'active-question';
                        that.updatePopUp(parseInt(that.active.id.substr(2)));
                        that.active.appendChild(that.popUp);
                    }

                    break;
                }

                el = el.parentElement;
            }

            eventUtils.stopPropagation(evt);
        });

        eventUtils.setEvent(this.container, 'mouseout', function (evt) {
            var el = evt.toElement;

            if (el) {
                for (var i = 0; i < 6; i++) {
                    if (!el) {
                        break;
                    }

                    if (el.tagName == 'UL') {
                        eventUtils.stopPropagation(evt);
                        return;
                    }

                    el = el.parentElement;
                }
            }

            that.active.className = ' ';
            if (that.active.childNodes && that.active.childNodes.length == 2) {
                that.active.removeChild(that.popUp);
                that.active = {};
            }

            eventUtils.stopPropagation(evt);
        });
    },
    setClickEvents: function () {
        var that = this;
        eventUtils.setEvent(this.container, 'click', function (evt) {
            var el = evt.target;
            if (el && el.tagName == 'LI') {
                if (el.id != that.active.id) {
                    that.active.className = ' ';
                    that.active = el;
                    that.active.className = 'active-question';
                    that.updatePopUp(parseInt(that.active.id.substr(2)));
                    that.active.appendChild(that.popUp);
                }
                else {
                    that.active.className = ' ';
                    if (that.active.childNodes && that.active.childNodes.length == 2) {
                        that.active.removeChild(that.popUp);
                        that.active = {};
                    }
                }

                eventUtils.stopPropagation(evt);
            }

            if (el && el.tagName != 'INPUT' && el.parentElement) {
                eventUtils.fireEvent(el.parentElement, 'click');
            }
        });
    }
});


//Edit Panel

var questionEditPanel = (function () {
    var data;
    var mainContainer;
    var answersHeader;
    var answersBox;
    var answersList;
    var rightAnswer;
    var MAX_ANSWERS = 6;

    function create(question, onUpdate, onCancel) {
        data = question;
        if (!data.answers) { data.answers = []; }
        mainContainer = document.createElement('div');
        mainContainer.tabIndex = 1;
        mainContainer.className = 'question-edit-box';

        mainContainer.appendChild(document.createElement('h2'));
        mainContainer.lastChild.innerHTML = 'Question';

        var content = document.createElement('textarea');
        var contentPlaceholder = 'Ask your question here ...';

        if (!data.content || data.content == '') {
            content.id = 'empty';
            content.value = contentPlaceholder;
        }
        else {
            addText(content, data.content);
        }

        content.className += ' inactive';

        eventUtils.setEvent(content, 'mouseup', function (evt) {
            if (this.className.match(' inactive')) {
                this.className = this.className.replace(' inactive', ' active');
                if (this.id == 'empty') {
                    this.value = '';
                }
                this.id = '';
                htmlUtils.focusTextArea(this);
            }
        });

        eventUtils.setEvent(content, 'blur', function () {
            this.className += this.className.match(' active') ?
               this.className.replace(' active', ' inactive') : ' inactive';

            if (!this.value || this.value == '' || this.value.match(/^\s+$/)) {
                this.id = 'empty';
                this.value = contentPlaceholder;
                data.content = '';
            } else {
                this.id = '';
                data.content = this.value;
            }
        });

        mainContainer.appendChild(htmlUtils.createValidationWrapperOn(content, null,
            [{ method: 'required', message: 'A question must have a content.' },
            { method: 'maxlength', max: 300, message: 'Question can be maximum 300 characters long.' }]));

        answersBox = document.createElement('div');

        var answersValidation = htmlUtils.createElement({ tag: 'input', type: 'hidden' });

        answersHeader = htmlUtils.createValidationWrapperOn(answersValidation, null,
          [{ method: 'gt', value: 1, message: 'A question must have atleast 2 answers.' },
          { method: 'required', message: 'A question must have 1 correct answer.' }]);

        if (countAnswers() < MAX_ANSWERS) {
            answersHeader.appendChild(getAnswerCreateBtn());
        }
      
        answersHeader.appendChild(document.createElement('h3'));
        answersHeader.lastChild.innerHTML = 'Question Answers ';

        answersBox.appendChild(answersHeader);

        answersList = document.createElement('ul');
        renderAnswers(answersList);
        answersBox.appendChild(answersList);

        mainContainer.appendChild(answersBox);
        mainContainer.appendChild(getCommandPanel({
            name: 'Save',
            className: 'accept-btn',
            action: function () {
                if (isAnswerSelected()) {
                    answersValidation.value = countAnswers();
                } else {
                    answersValidation.value = '';
                }

                if (content.id == 'empty') {
                    content.value = '';
                }

                if (!javascriptValidator.validateContainerElement(mainContainer, 2)) {
                    if (content.id == 'empty') {
                        content.value = contentPlaceholder;
                    }
                    return;
                }

                onUpdate(data);
            }
        }, {
            name: 'Cancel',
            className: 'reject-btn',
            action: function () {
                onCancel(data);
            }
        }));

        return mainContainer;
    }

    function renderAnswers(container) {
        for (var i = 0; i < data.answers.length; i++) {
            if (!data.answers[i]) { continue; }

            container.appendChild(getAnswerViewContainer(data.answers[i],
                (function (index) {
                    return function () {
                        if (data.answers.length == MAX_ANSWERS) {
                            answersHeader.appendChild(getAnswerCreateBtn());
                        }

                        data.answers.splice(index, 1);

                        while (container.firstChild) {
                            container.removeChild(container.firstChild);
                        }

                        renderAnswers(container);
                    }
                }(i))));
        }
    }

    function getAnswerViewContainer(answer, onDelete) {
        var container = document.createElement('li');

        var status = htmlUtils.radio('q-' + data.id);
        if (answer.isCorrect) {
            status.defaultChecked = answer.isCorrect;
            rightAnswer = answer;
            container.id = 'right-answer-row';
        }

        var statusWrapper = document.createElement('span');
        statusWrapper.appendChild(status);

        eventUtils.setEvent(statusWrapper, 'click', function () {
            var oldAnswerRow = document.getElementById('right-answer-row');
            if (oldAnswerRow) { oldAnswerRow.removeAttribute('id') }
            this.parentElement.id = 'right-answer-row';
            answer.isCorrect = true;
            if (rightAnswer) {
                rightAnswer.isCorrect = false;
            }
            rightAnswer = answer;

            this.firstChild.checked = true;
        });

        var contentWrapper = document.createElement('p');
        addText(contentWrapper, answer.content);

        var commandPanel = getCommandPanel(
            {
                name: ' ',
                className: 'edit-img-btn',
                action: function () {
                    var editContainer = getAnswerEditContainer(answer, function (newValue) {
                        mainContainer.removeChild(editContainer);
                        htmlUtils.showContent(mainContainer);
                        answer.content = newValue;
                        data.answers.sort(function (x, y) {
                            return x.content > y.content ? 1 : -1;
                        });
                        addText(contentWrapper, answer.content);
                    }, function (newValue) {
                        mainContainer.removeChild(editContainer);
                        htmlUtils.showContent(mainContainer);
                    });

                    htmlUtils.hideContent(mainContainer);
                    mainContainer.appendChild(editContainer);

                    eventUtils.setEvent(editContainer.lastChild.previousSibling, 'keydown', function (evt) {
                        if (evt.keyCode == 13) {
                            eventUtils.fireEvent(this.nextSibling.firstChild, 'click');
                            eventUtils.preventDefault(evt);
                        }

                        eventUtils.stopPropagation(evt);
                    });

                    htmlUtils.focusTextArea(editContainer.lastChild.previousSibling.firstChild);
                }
            }, {
                name: ' ',
                className: 'delete-img-btn',
                action: function () {
                    var deleteContainer = document.createElement('div');
                    deleteContainer.className = 'answer-delete';

                    deleteContainer.appendChild(document.createElement('h2'));
                    deleteContainer.lastChild.innerHTML = 'Do you want to DELETE this answer ?';
                    
                    deleteContainer.appendChild(document.createElement('p'));
                    addText(deleteContainer.lastChild, '"' + answer.content + '"');

                    deleteContainer.appendChild(getCommandPanel(
                        {
                            name: 'Yes',
                            action: function () {
                                onDelete();
                                deleteContainer.parentElement.removeChild(deleteContainer);
                                htmlUtils.showContent(mainContainer);
                            }
                        }, {
                            name: 'No',
                            action: function () {
                                deleteContainer.parentElement.removeChild(deleteContainer);
                                htmlUtils.showContent(mainContainer);
                            }
                        }));

                    htmlUtils.hideContent(mainContainer);
                    mainContainer.appendChild(deleteContainer);
                }
            });

        container.appendChild(statusWrapper);
        container.appendChild(commandPanel);
        container.appendChild(contentWrapper);

        return container;
    }

    function getAnswerEditContainer(answer, onUpdate, onCancel) {
        var container = document.createElement('div');
        container.className = 'answer-edit';

        container.appendChild(document.createElement('h2'));
        container.lastChild.innerHTML = 'Answer For :';

        container.appendChild(document.createElement('p'));
        addText(container.lastChild, '"' + (data.content ? data.content : "") + '"');

        var isUnique = htmlUtils.createElement({ tag: 'input', type: 'hidden' });
        isUnique.value = true;
        container.appendChild(htmlUtils.createValidationWrapperOn(isUnique, null,
            [{ method: 'required', message: 'Question already contains this answer.' }]));

        var content = document.createElement('textarea');
        content.value = answer.content;
        container.appendChild(
            htmlUtils.createValidationWrapperOn(content, null,
            [{ method: 'required', message: 'Answer content is required.' },
            { method: 'maxlength', max: 100, message: 'Answer can be maximum 100 characters.' },
            { method: 'regularexpression', pattern: "^.*\\S+.*$", message: 'Answer content can`t be only white space.' }]));

        var commandPanel = getCommandPanel(
            {
                name: 'Update',
                action: function () {
                    isUnique.value = containsAnswer(content.value) ? '' : true;

                    if (!javascriptValidator.validateContainerElement(container, 2)) {
                        return;
                    }

                    onUpdate(content.value);
                    mainContainer.focus();
                }
            }, {
                name: 'Cancel',
                action: function () {
                    onCancel();
                    mainContainer.focus();
                }
            });

        container.appendChild(commandPanel);

        return container;
    }

    function getAnswerCreateBtn() {
        var createBtn = htmlUtils.button('Create New Answer', function () {
            var newAnswer = {};
            newAnswer.id = 0;
            newAnswer.content = '';
            newAnswer.isCorrect = false;
            var editContainer = getAnswerEditContainer(newAnswer, function (newValue) {
                newAnswer.content = newValue;
                data.answers.push(newAnswer);
                data.answers.sort(function (x, y) {
                    return x.content > y.content ? 1 : -1;
                });

                if (countAnswers() >= MAX_ANSWERS) {
                    answersHeader.removeChild(createBtn);
                }

                while (answersList.firstChild) {
                    answersList.removeChild(answersList.firstChild);
                }

                renderAnswers(answersList);

                mainContainer.removeChild(editContainer);
                htmlUtils.showContent(mainContainer);
            }, function (newValue) {
                mainContainer.removeChild(editContainer);
                htmlUtils.showContent(mainContainer);
            });

            htmlUtils.hideContent(mainContainer);
            mainContainer.appendChild(editContainer);

            eventUtils.setEvent(editContainer.lastChild.previousSibling, 'keydown', function (evt) {
                if (evt.keyCode == 13) {
                    eventUtils.fireEvent(this.nextSibling.firstChild, 'click');
                    eventUtils.preventDefault(evt);
                }
                eventUtils.stopPropagation(evt);
            });

            editContainer.lastChild.previousSibling.firstChild.focus();
        });

        eventUtils.setEvent(mainContainer, 'keydown', function (evt) {
            if (evt.keyCode == 13 && createBtn.parentElement) {
                if (evt.target.tagName == 'TEXTAREA') {
                    return;
                }
                eventUtils.fireEvent(createBtn, 'click');

                eventUtils.preventDefault(evt);
            }
        });

        createBtn.className = 'large-btn';
        return createBtn;
    }

    function getCommandPanel() {
        var container = document.createElement('div');
        container.className = 'command-tab';

        for (var i = 0; i < arguments.length; i++) {
            container.appendChild(htmlUtils.button(arguments[i].name, arguments[i].action));
            if (arguments[i].className) {
                container.lastChild.className = arguments[i].className;
            }
        }

        return container;
    }

    function containsAnswer(value) {
        for (var i = 0; i < data.answers.length; i++) {
            if (data.answers[i].content == value) {
                return true;
            }
        }
        return false;
    }

    function addText(el, content) {
        el.innerHTML = htmlUtils.escape(content);
    }

    function isAnswerSelected() {
        var inputs = answersList.getElementsByTagName('input');
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].type == 'radio' && inputs[i].checked == true) {
                return true;
            }
        }
        return false;
    }

    function countAnswers() {
        var count = 0;
        for (var i = 0; i < data.answers.length; i++) {
            if (data.answers[i]) { count++; }
        }
        return count;
    }

       return {
        create: create
    }
}());