var javascriptValidator = (function () {
    var validationFunctions = {
        required: function (value, validator) {
            return value && value != '';
        },
        minlength: function (value, validator) {
            if (value && value != '') {
                return value.length >= validator.min;
            }
            return true;
        },
        maxlength: function (value, validator) {
            if (value && value != '') {
                return value.length <= validator.max;
            }
            return true;
        },
        regularexpression: function (value, validator) {
            if (value && value != '') {
                return value.match(new RegExp(validator.pattern));
            }
            return true;
        },
        compare: function (value, validator) {
            var otherValue = document.getElementById(validator.other).value;
            return otherValue == value;
        },
        stringlength: function (value, validator) {
            if (value && value != '') {
                return value.length >= validator.min && value.length <= validator.max;
            }
        },
        gt: function (value, validator) {
            if (value && value != '') {
                return parseFloat(value) > parseFloat(validator.value);
            }

            return true;
        }
    };

    function validateContainer(id,maxErrors) {
        return validateContainerElement(document.getElementById(id), maxErrors);
    }

    function validateContainerElement(el, maxErrors) {
        var fields = {
            inputs: el.getElementsByTagName('input'),
            textareas: el.getElementsByTagName('textarea'),
            selects: el.getElementsByTagName('select')
        }

        var isValid = true;

        for (var fieldType in fields) {
            var isFieldValid;

            for (var i = 0; i < fields[fieldType].length; i++) {
                isFieldValid = validateField(fields[fieldType][i], maxErrors);

                if (isValid && !isFieldValid) {
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    function validateField(el, maxErrors) {
        maxErrors = maxErrors || 0;

        var contextData = el.getAttribute('data-v-context');
        if (!contextData) {
            return true;
        }

        contextData = contextData.split(';');

        var errorBox = el.nextSibling;

        while (errorBox && errorBox.className != 'errors-container') {
            errorBox = errorBox.nextSibling;
        }

        if (errorBox) {
            el.parentElement.removeChild(errorBox);
        }

        errorBox = initialiseErrorContainer();
      
        var validators = createValidators(contextData);
        var isValid = true;
        var errorsCount = 0;

        for (var i = 0; i < validators.length; i++) {

            if (!validate(el.value, validators[i])) {
                if (errorBox) {
                    addError(el, errorBox.lastChild, validators[i].method);
                }

                if (isValid) {
                    isValid = false;
                }

                errorsCount++;

                if (errorsCount == maxErrors) { break; }
            }
        }

        if (isValid) {
            el.className = el.className.replace(' invalid', '');
        } else {
            //just for nice layout
            if (maxErrors > 0) {
                var errorsWrap = errorBox.lastChild;
                var line = (maxErrors * 24) / errorsWrap.childNodes.length;
                for (var i = 0; i < errorsWrap.childNodes.length; i++) {
                    errorsWrap.childNodes[i].style.lineHeight = line + 'px';
                }
            }
            if (!el.className.match('.* invalid.*')) {
                el.className = el.className + ' invalid';
            }
            el.parentElement.appendChild(errorBox);
        }
      
        return isValid;
    }

    function initialiseErrorContainer() {
        var container = document.createElement('div');
        container.className = 'errors-container';

        var metaEl = document.createElement('div');
        metaEl.className = 'arrow-div';
        container.appendChild(metaEl);

        metaEl = document.createElement('div');
        metaEl.className = 'errors-wrapper';
        container.appendChild(metaEl);
        
        return container;
    }

    function createValidators(data) {
        var validator = [];
        var obj;
        var metaData;
        var parsedData;

        for (var i = 0; i < data.length; i++) {
            if (!data[i] || data[i] == '') {
                continue;
            }

            parsedData = data[i].split(',');
            obj = {};

            for (var j = 0; j < parsedData.length; j++) {
                metaData = parsedData[j].split(':');
                obj[metaData[0]] = metaData[1];
            }

            validator.push(obj);
        }

        return validator;
    }

    function validate(value, validator) {
        var func = validationFunctions[validator.method.toLowerCase()];

        if (!func) {
            return true;
        }

        return func(value, validator);
    }

    function addError(el, errorBox, errorType) {
        var errorText = el.getAttribute('data-v-err-' + errorType);

        var errorElement = document.createElement('span');
        errorElement.className = 'error-element';
        errorElement.innerHTML = errorText;

        errorBox.appendChild(errorElement);
    }

    function dynamicValidation(containerId) {
        var el = document.getElementById(containerId);

        eventUtils.setEvent(el, 'change', function (evt) {
            if (evt.target.tagName == 'INPUT' || evt.target.tagName == 'TEXTAREA' || evt.target.tagName == 'SELECT') {
                validateField(evt.target, 2);
            }

            eventUtils.stopPropagation(evt);
        });
    }

    function validateForm(id) {
        var form = document.getElementById(id);
        if (!form) { return; }

        dynamicValidation(id);

        eventUtils.setEvent(form, 'submit', function (evt) {
            if (!validateContainerElement(form, 2)) {
                eventUtils.preventDefault(evt);
                return;
            }
        });
    }

   
    return {
        validateElement: validateField,
        validateContainer: validateContainer,
        validateContainerElement: validateContainerElement,
        dynamicValidation: dynamicValidation,
        validateForm: validateForm
    }
}());
