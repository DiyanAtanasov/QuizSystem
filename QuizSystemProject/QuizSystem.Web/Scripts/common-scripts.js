//JSON object
if (!window.JSON) {
    window.JSON = {};
    window.JSON.stringify = function (obj) {
        var result = [];
        if (obj instanceof Array) {
            result.push('[');
            for (var i = 0; i < obj.length; i++) {
                result.push(JSON.stringify(obj[i]));
                result.push(',');
            }
            result.pop();
            result.push(']');

            return result.join('');
        };

        result.push('{');

        for (var prop in obj) {
            result.push('"' + prop + '":');
            if (obj[prop] instanceof Array) {
                result.push('[');
                for (var i = 0; i < obj[prop].length; i++) {
                    result.push(JSON.stringify(obj[prop][i]));
                    result.push(',');
                }
                result.pop();
                result.push(']');
            }
            else {
                if (typeof (obj[prop]) === 'number' || typeof (obj[prop]) === 'boolean') {
                    result.push(obj[prop]);
                } else {
                    result.push('"' + JSON.escape(obj[prop]) + '"');
                }
            }

            result.push(',');
        }

        result.pop();
        result.push('}');

        return result.join('');
    }

    window.JSON.escape = function (str) {
        console.log(str);
        return str
        .replace(/[\\]/g, '\\\\')
        .replace(/["]/g, '\\"')
        .replace(/[\/]/g, '\\/')
        .replace(/[\b]/g, '\\b')
        .replace(/[\f]/g, '\\f')
        .replace(/[\n]/g, '\\n')
        .replace(/[\r]/g, '\\r')
        .replace(/[\t]/g, '\\t');
    }

    window.JSON.parse = function (str) {
        str = str.replace(/^\s*/, '');
        str = str.replace(/\w+\s*$/, '');
        return (new Function("return " + str))();
    }
}

//OOP 
var Class = (function () {
    function create(init, properties) {
        var newInstance = function () {
            init.apply(this, arguments);
        }

        newInstance.prototype = {};
        newInstance.prototype.init = init;

        for (var prop in properties) {
            newInstance.prototype[prop] = properties[prop];
        }

        return newInstance;
    }

    function copyParentPrototype(parentPrototype) {
        function F() { };

        F.prototype = parentPrototype;

        return new F();
    }

    Function.prototype.extend = function (properties) {
        for (var prop in properties) {
            this.prototype[prop] = properties[prop];
        }
    }

    Function.prototype.inherit = function (parent) {

        var originalPrototype = this.prototype;

        this.prototype = copyParentPrototype(parent.prototype);

        this.prototype._super = parent.prototype;

        for (var prop in originalPrototype) {
            this.prototype[prop] = originalPrototype[prop];
        }
    }

    return {
        create: create
    }
}());

//Event handling
var eventUtils = (function (){
    function preventDefault(evt) {
        if (evt.preventDefault) {
            evt.preventDefault();
        } else {
            evt.returnValue = false;
        }
    }

    function stopPropagation(evt) {
        if (evt.stopPropagation) {
            evt.stopPropagation();
        } else {
            evt.cancelBubble = true;
        }
    }

    function setEvent(el, eventType, eventHandler) {
        var handler = function (evt) {
            evt = evt ? evt : window.event;
            if (evt.srcElement) {
                evt.target = evt.srcElement;
            }

            if (evt.relatedTarget) {
                evt.toElement = evt.relatedTarget;
            }

            eventHandler.call(el, evt);
        }

        if (document.attachEvent) {
            el.attachEvent("on" + eventType, handler);
        }
        else if (document.addEventListener) {
            el.addEventListener(eventType, handler, false);
        }
        else {
            el["on" + eventType] = handler;
        }
    }

    function fireEvent(obj, evt) {
        if (document.createEvent) {
            var evtObj = document.createEvent('MouseEvents');
            evtObj.initEvent(evt, true, false);
            obj.dispatchEvent(evtObj);
        }
        else if (document.createEventObject) {
            var evtObj = document.createEventObject();
            obj.fireEvent('on' + evt, evtObj);
        }
    }

    return {
        preventDefault: preventDefault,
        stopPropagation: stopPropagation,
        setEvent: setEvent,
        fireEvent : fireEvent
    }
}());

//Promise 
var SimplePromise = Class.create(function () {
    this.succesChain = [];
    this.errorChain = [];
}, {
    then: function (success, error) {
        if (success) { this.succesChain.push(success) }
        if (error) { this.errorChain.push(error) }
        return this;
    },
    resolve: function (data) {
        if (this.succesChain[0]) {
            var result = this.succesChain[0](data);
            this.succesChain.shift();
            this.resolve(result);
        }
    },
    reject: function (data) {
        if (this.errorChain[0]) {
            var result = this.errorChain[0](data);
            this.errorChain.shift();
            this.reject(result);
        }
    }
});

//Http Ajax Requester
var httpRequester = (function () {

    if (!window.XMLHttpRequest) {
        window.XMLHttpRequest = new ActiveXObject("Microsoft.XMLHTTP");
    }

    var httpGet = function (options) {
        var simplePromise = new SimplePromise();

        var req = new XMLHttpRequest();
        req.open('get', options.url);
        req.setRequestHeader('X-Requested-With', 'XMLHttpRequest');

        if (options.headers) {
            for(var header in options.headers)
            {
                req.setRequestHeader(header, options.headers[header]);
            }
        }
        else {
            req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
        }

        req.onreadystatechange = function () {
            if (req.readyState == 4) {
                if (req.status >= 200 && req.status < 300) {
                    simplePromise.resolve(req.responseText);
                } else {
                    simplePromise.reject(req.responseText);
                }
            }
        };

        req.send(null);

        return simplePromise;
    }

    var httpPost = function (options) {
        var simplePromise = new SimplePromise();

        var req = new XMLHttpRequest();
        req.open('post', options.url);
        req.setRequestHeader('X-Requested-With', 'XMLHttpRequest');

        if (options.headers) {
            for (var header in options.headers) {
                req.setRequestHeader(header, options.headers[header]);
            }
        }
        else {
            req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
        }

        //1223 status IE Fix
        req.onreadystatechange = function () {
            if (req.readyState == 4) {
                if (req.status >= 200 && req.status < 300 || req.status == 1223) {
                    simplePromise.resolve(req.responseText);
                } else {
                    simplePromise.reject(req.responseText);
                }
            }
        };

        req.send(options.data);

        return simplePromise;
    }

    return {
        get: httpGet,
        post: httpPost
    }
}());

//Html Helper
var htmlUtils = (function () {
    function makeButton(text, func) {
        var btn = document.createElement('input');
        btn.setAttribute('type', 'button');
        btn.value = text;
        if (func) {
            eventUtils.setEvent(btn, 'click', function (evt) {
                func(evt);
                btn.blur();
            })
        }
        return btn;
    }

    function makeRadio(group) {
        var radio = document.createElement('input');
        radio.setAttribute('type', 'radio');
        radio.setAttribute('name', group);
        return radio;
    }

    function makeConfirmWindow(message, success, reject) {
        var container = document.createElement('div');
        container.className = 'confirm-window';

        container.appendChild(document.createElement('p'));
        container.lastChild.innerHTML = escapeString(message);

        container.appendChild(document.createElement('div'));

        container.lastChild.appendChild(makeButton('Yes',function () {
            container.parentElement.removeChild(container);
            success();
        }));

        container.lastChild.appendChild(makeButton('No', function () {
            container.parentElement.removeChild(container);
            reject();
        }));

        return container;
    }

    function createElement(options) {
        var el = document.createElement(options.tag);

        for (var attr in options) {
            if (attr == 'tag') { continue; }
            el.setAttribute(attr, options[attr]);
        }

        return el;
    }

    function hideContent(el, byVisibility) {
        if (!byVisibility) {
            for (var i = 0; i < el.childNodes.length; i++) {
                el.childNodes[i].style.display = 'none';
            }
        } else {
            for (var i = 0; i < el.childNodes.length; i++) {
                el.childNodes[i].style.visibility = 'hidden';
            }
        }
    }

    function showContent(el, byVisibility) {
        if (!byVisibility) {
            for (var i = 0; i < el.childNodes.length; i++) {
                el.childNodes[i].style.display = '';
            }
        } else {
            for (var i = 0; i < el.childNodes.length; i++) {
                el.childNodes[i].style.visibility = '';
            }
        }
    }

    function addValidationTo(el, validators) {
        var context = [];
        for (var i = 0; i < validators.length; i++) {
            for (var prop in validators[i]) {
                if (prop == 'message') {
                    el.setAttribute('data-v-err-' + validators[i].method, validators[i][prop]);
                    continue;
                }

                context.push(prop + ':' + validators[i][prop]);
                context.push(',');
            }
            context.pop();
            context.push(';');
        }

        el.setAttribute('data-v-context', context.join(''));

        return el;
    }

    function validationWrapper(element, label, validators) {
        var container = document.createElement('div');
        container.className = 'form-tab';
        if (label) {
            var lb = document.createElement('label');
            lb.innerHTML = label;
            container.appendChild(lb);
        }
        addValidationTo(element, validators);
        container.appendChild(element);

        return container;
    }

    function escapeString(str) {
        return str.replace(/</g, '&#60;').replace(/>/g, '&#62;');
    }

    function focusTextArea(el) {
        if (typeof el.selectionStart == "number") {
            el.selectionStart = el.selectionEnd = el.value.length;
            el.focus();
        } else if (typeof el.createTextRange != "undefined") {
            el.focus();
            var range = el.createTextRange();
            range.collapse(false);
            range.select();
        }
        
        el.scrollTop = el.scrollHeight;
    }

    function showInOverlayBox(el, level, toEl) {
        level = level || 1;

        var surface = document.getElementById('overlay-surface-' + level);
        var box = document.getElementById('overlay-box-' + level);
        if (!surface) {
            surface = document.createElement('div');
            surface.id = 'overlay-surface-' + level;
            surface.className = 'overlay-surface';
            surface.style.zIndex = level * 10;
        }
        if (!box) {
            box = document.createElement('div');
            box.id = 'overlay-box-' + level;
            box.className = 'overlay-box';
            box.appendChild(el);
            box.style.zIndex = level * 11;
        }

        if (toEl) {
            //to show properly in elements
            surface.style.position = 'absolute';
            box.style.position = 'absolute';
        } else {
            toEl = document.body;
        }

        toEl.className = ' overlay-root';

        toEl.appendChild(surface);
        toEl.appendChild(box);

        var wCoeff = (box.clientWidth / surface.clientWidth) * 100;
        wCoeff = (100 - wCoeff) / 2;
        box.style.left = wCoeff + '%';
        if (wCoeff < 0) {
            box.style.width = '95%';
            wCoeff = 2.5;
        }

        box.style.left = wCoeff + '%';

        var hCoeff = (box.clientHeight / surface.clientHeight) * 100;
        hCoeff = (100 - hCoeff) / 2;
        box.style.top = hCoeff + '%';
    }

    function hideOverlayBox(level) {
        level = level || 1;
        var surface = document.getElementById('overlay-surface-' + level);
        var box = document.getElementById('overlay-box-' + level);

        if (surface.parentElement.className = ' overlay-root') {
            surface.parentElement.removeAttribute('class');
        } else {
            surface.parentElement.className =
                surface.parentElement.className.replace(' overlay-root', '');
        }

        surface.parentElement.removeChild(surface);
        box.parentElement.removeChild(box);
    }

    function updateOverlayBox(el, level) {
        level = level || 1;
        var surface = document.getElementById('overlay-surface-' + level);
        var box = document.getElementById('overlay-box-' + level);

        if (box) {
            while (box.firstChild) {
                box.removeChild(box.firstChild);
            }

            box.appendChild(el);
        }

        var wCoeff = (box.clientWidth / surface.clientWidth) * 100;
        wCoeff = (100 - wCoeff) / 2;
        box.style.left = wCoeff >= 0 ? wCoeff : 0 + '%';
        if (wCoeff < 0) {
            box.style.width = '95%';
            wCoeff = 2.5;
        }

        box.style.left = wCoeff + '%';

        var hCoeff = (box.clientHeight / surface.clientHeight) * 100;
        hCoeff = (100 - hCoeff) / 2;
        box.style.top = hCoeff + '%';
    }

    function overlayMessage(options) {
        var container = document.createElement('div');
        container.className = options.isError ? 'error-msg' : 'success-msg';

        container.appendChild(document.createElement('h4'));
        container.lastChild.innerHTML = options.isError ? 'Error' : 'Success';

        if (options.message) {
            container.appendChild(document.createElement('p'));
            container.lastChild.innerHTML = escapeString(options.message);
        }

        if (options.autoHide) {
            hideElement(container, options.duration  || 1000 , function () {
                hideOverlayBox(options.level);
            });
        } else {
            container.appendChild(makeButton('Continue', function () {
                hideOverlayBox(options.level);
            }))
        }

        if (options.update) {
            updateOverlayBox(container, options.level);
        } else {
            showInOverlayBox(container, options.level, options.parent);
        }
    }

    function extractFormData(root) {
        var form = root;

        var formDataString = '';

        var inputs = form.getElementsByTagName('input');
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].getAttribute('type') == 'radio' && !inputs[i].checked) {
                continue;
            }

            formDataString += encodeURIComponent(inputs[i].name) + '=' + encodeURIComponent(inputs[i].value) + '&';
        }

        var selects = form.getElementsByTagName('select');
        for (var i = 0; i < selects.length; i++) {
            formDataString += encodeURIComponent(selects[i].name) + '=' + encodeURIComponent(selects[i].value) + '&';
        }

        var areas = form.getElementsByTagName('textarea');
        for (var i = 0; i < areas.length; i++) {
            formDataString += encodeURIComponent(areas[i].name) + '=' + encodeURIComponent(areas[i].value) + '&';
        }

        formDataString = formDataString.substring(0, formDataString.length - 1);

        return formDataString;
    }

    function flashElement() {

    }

    function hideElement(el, time, callback) {
        time = time || 1000;
        el.style.opacity = 1;
        var op = 1;
        var coeff = 1 / (time / 10);
        var hideInetval = setInterval(function () {
            op -= coeff;
            el.style.opacity = op;

            if (op <= 0) {
                clearInterval(hideInetval);
                el.style.opacity = 0;
                el.style.display = 'none';
                if (callback) { callback() }
            }
        }, 10);
    }

    function findChild(el, properties) {
        var isFound;
        for (var i = 0; i < el.childNodes.length; i++) {
            isFound = true;
            for (var prop in properties) {
                if (el.childNodes[i][prop] != properties[prop]) {
                    isFound = false;
                    break;
                }
            }

            if (isFound) { return el.childNodes[i]; }

            if (el.childNodes[i].childNodes.length > 0) {

                var result = findChild(el.childNodes[i], properties);

                if (result) {
                    return result;
                }
            }
        }
    }

    return {
        button: makeButton,
        radio: makeRadio,
        confirmWindow: makeConfirmWindow,
        escape: escapeString,
        hideContent: hideContent,
        showContent: showContent,
        createElement: createElement,
        createValidationWrapperOn: validationWrapper,
        addValidationTo: addValidationTo,
        focusTextArea: focusTextArea,
        showInOverlayBox: showInOverlayBox,
        hideOverlayBox: hideOverlayBox,
        updateOverlayBox: updateOverlayBox,
        overlayMessage: overlayMessage,
        extractFormData: extractFormData,
        findChild : findChild
    }
}());