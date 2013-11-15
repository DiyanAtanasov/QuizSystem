var repository = (function () {
    var questions = function (quizId) {
        return (function (id) {
            var defaults = {
                headers: { 'Content-Type': 'application/json'}
            };
           
            function create(question) {
                var options = defaults;
                options.url = '/UserQuizzes/' + id + '/Questions/CreateQuestion';
                options.data = JSON.stringify(question);
                return httpRequester.post(options);
            }

            function update(question) {
                var options = defaults;
                options.url = '/UserQuizzes/' + id + '/Questions/UpdateQuestion';
                options.data = JSON.stringify(question);
                return httpRequester.post(options);
            }

            function remove(question) {
                var options = defaults;
                options.url = '/UserQuizzes/' + id + '/Questions/RemoveQuestion';
                options.data = JSON.stringify(question);
                return httpRequester.post(options);
            }

            function all() {
                var options = defaults;
                options.url = '/UserQuizzes/' + id + '/Questions/AllQuestions';
                return httpRequester.get(options);
            }

            return {
                create: create,
                update: update,
                remove: remove,
                all: all
            }
        }(quizId))
    }

    var categories = (function () {
        var defaults = {
            headers: { 'Content-Type': 'application/json' }
        };
        
        function update(category) {
            var options = defaults;
            options.url = '/Administration/Categories/Update';
            options.data = JSON.stringify(category);
            return httpRequester.post(options)
        }

        function remove(category) {
            var options = defaults;
            options.url = '/Administration/Categories/Remove';
            options.data = JSON.stringify(category);
            return httpRequester.post(options)
        }

        return {
            update: update,
            remove: remove
        }
    }());

    var quizzes = (function () {
        var defaults = {};

        function create(params) {
            var options = defaults;
            options.url = '/UserQuizzes/Create';
            options.data = params;
            options.cache = false;
            return httpRequester.post(options);
        }

        function remove(params) {
            var options = defaults;
            options.url = '/UserQuizzes/Remove';
            options.data = params;
            return httpRequester.post(options);
        }

        function update(params) {
            var options = defaults;
            options.url = '/UserQuizzes/Edit';
            options.data = params;
            return httpRequester.post(options);
        }

        function updateGrid(params) {
            var options = defaults;
            options.url = '/UserQuizzes/UpdateGrid?' + params;
            return httpRequester.get(options);
        }

        return {
            create: create,
            remove: remove,
            update: update,
            updateGrid: updateGrid
        }
    }());

    var allQuizzes = (function () {
        var defaults = {};

        function updateGrid(params) {
            var options = defaults;
            options.url = '/Quizzes/UpdateGrid?' + params;
            return httpRequester.get(options);
        }

        function vote(params) {
            var options = defaults;
            options.url = '/Quizzes/Vote';
            options.data = params;
            return httpRequester.post(options);
        }

        function getComments(params) {
            var options = defaults;
            options.url = '/Quizzes/UpdateCommentsGrid?' + params;
            return httpRequester.get(options);
        }

        function postComment(params) {
            var options = defaults;
            options.url = '/Quizzes/Comment';
            options.data = params;
            return httpRequester.post(options);
        }

        return {
            updateGrid: updateGrid,
            vote: vote,
            getComments: getComments,
            postComment : postComment
        }
    }());

    return {
        questions: questions,
        categories: categories,
        quizzes: quizzes,
        allQuizzes: allQuizzes
    }
}());