using QuizSystem.Model;
using QuizSystem.Web.Extensions.FiltersExtensions;
using QuizSystem.Web.Libs.DataPager;
using QuizSystem.Web.Libs.Helpers;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;

namespace QuizSystem.Web.Controllers
{
    [Authorize]
    public class QuizzesController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var categories = this.context.Categories.All().ToList();

            List<SelectListItem> categoriesFilterListItems = new List<SelectListItem>();
            categoriesFilterListItems.Add(new SelectListItem { Text = "All", Value = "", Selected = true });
            categoriesFilterListItems.AddRange(categories.Select(x => new SelectListItem { Value = x.Name, Text = x.Name }));

            this.ViewData.Add("categoriesFilter", categoriesFilterListItems);

            string userId = this.User.Identity.GetUserId();

            var activeQuizzes = this.context.Quizzes.All()
                .Where(x => x.State == QuizState.Active && 
                    !x.Results.Any(r => r.UserId == userId))
                .Select(ModelConvertor.QuizToViewModel);

            var dataPager = new SimpleDataPager<QuizViewModel>(activeQuizzes, 10)
                .Sort("PublishDate", SortingDirection.Descending)
                .Load();

            return View(dataPager);
        }

        [HttpGet]
        [ActionName("UpdateGrid")]
        [NoCache]
        [HandleAjaxError]
        public ActionResult GetQuzzesGird()
        {
            string userId = this.User.Identity.GetUserId();

            var activeQuizzes = this.context.Quizzes.All()
                .Where(x => x.State == QuizState.Active && 
                    !x.Results.Any(r => r.UserId == userId))
                .Select(ModelConvertor.QuizToViewModel);

            var dataPager = new SimpleDataPager<QuizViewModel>(activeQuizzes, 10)
                .ProcessUrlParameters(this.Request.QueryString)
                .Load();

            return PartialView("_GridView", dataPager);
        }

        [HttpGet]
        public ActionResult Solve(int quizId)
        {
            QuizSolveModel quiz = this.context.Quizzes.All()
                .Where(x => x.Id == quizId && x.State == QuizState.Active)
                .Select(ModelConvertor.QuizToSolveModel)
                .First();

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Solve()
        {
            string userId = this.User.Identity.GetUserId();

            int quizId = int.Parse(this.Request.Form.Get("quizId"));

            QuizSolvedModel solvedQuiz =
                this.context.Quizzes.All()
                .Where(x => x.Id == quizId && x.State == QuizState.Active)
                .Select(ModelConvertor.QuizToSolvedModel)
                .First();

            Dictionary<int,int> answeredQuestions =
                this.Request.Form.AllKeys
                .Where(x => Regex.IsMatch(x, @"^q-\d+$"))
                .ToDictionary(x => int.Parse(x.Substring(2)), x => int.Parse(this.Request.Form.Get(x)));

            for (int i = 0; i < solvedQuiz.Questions.Count; i++)
            {
                solvedQuiz.Questions[i].SelectedAnswerId = 
                    answeredQuestions[solvedQuiz.Questions[i].Id];

                if (solvedQuiz.Questions[i].RightAnswerId == solvedQuiz.Questions[i].SelectedAnswerId)
                {
                    solvedQuiz.Points++;
                }
            }

            this.ManageResults(userId, solvedQuiz);

            solvedQuiz.IsVotableByUser = 
                !this.context.Votes.All()
                .Any(x => x.QuizId == quizId && x.UserId == userId);

            return View("Solved", solvedQuiz);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult Vote(int quizId, int value)
        {
            string userId = this.User.Identity.GetUserId();
            Quiz quiz = this.context.Quizzes.GetById(quizId);

            if (userId == null || quiz.State != QuizState.Active)
            {
                throw new HttpException(400, "You are not authorized.");
            }

            if (this.context.Votes.All().Any(x => x.QuizId == quizId && x.UserId == userId))
            {
                throw new HttpException(400, "You already voted for this quiz.");
            }

            quiz.Rating += value;
            quiz.Votes.Add(new Vote { UserId = userId, Value = value });

            this.context.Quizzes.Update(quiz);
            this.context.SaveChanges();

            return Content(value.ToString());
        }

        public ActionResult Archive()
        {
            var categories = this.context.Categories.All().ToList();

            List<SelectListItem> categoriesFilterListItems = new List<SelectListItem>();
            categoriesFilterListItems.Add(new SelectListItem { Text = "All", Value = "", Selected = true });
            categoriesFilterListItems.AddRange(categories.Select(x => new SelectListItem { Value = x.Name, Text = x.Name }));

            this.ViewData.Add("categoriesFilter", categoriesFilterListItems);

            string userId = this.User.Identity.GetUserId();

            var activeQuizzes = this.context.Results.All()
                .Where(x => x.UserId == userId && x.Quiz.State == QuizState.Active)
                .Select(ModelConvertor.ResultToQuizArchiveModel);

            var dataPager = new SimpleDataPager<QuizArchiveModel>(activeQuizzes, 10)
                .ProcessUrlParameters(this.Request.RequestType == "GET" ? this.Request.QueryString : this.Request.Form)
                .Load();

            return View(dataPager);
        }

        [AllowAnonymous]
        public ActionResult Details(int quizId)
        {
            QuizDetailsModel model = 
                this.context.Quizzes.All()
                .Where(x => x.Id == quizId && x.State == QuizState.Active)
                .Select(ModelConvertor.QuizToDetailsModel)
                .First();

            string userId = this.User.Identity.GetUserId();

            model.UserCanComment = this.context.Results.All().Any(r => r.QuizId == quizId && r.UserId == userId);

            if (model.UserCanComment)
            {
                model.UserCanVote = !this.context.Votes.All().Any(v => v.QuizId == quizId && v.UserId == userId);
            }

            var commentsQuery =
                this.context.Comments.All()
                .Where(x => x.QuizId == quizId)
                .Select(ModelConvertor.CommentToViewModel);

            var commentsPager = new SimpleDataPager<CommentViewModel>(commentsQuery, 4)
                .Sort("PublishDate", SortingDirection.Descending).Load();

            commentsPager.UrlParameters.Add("quizId", quizId);

            model.Comments = commentsPager;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Comment")]
        [HandleAjaxError]
        public ActionResult PostComment(CommentCreateModel model)
        {
            string userId = this.User.Identity.GetUserId();
            double timeInterval = this.TimeFromLastComment(userId, model.QuizId);
 
            if (timeInterval > 0)
            {
                    throw new HttpException(400,
                    String.Format("Spam is not cool... You can comment on this quiz after {0:F0} seconds", timeInterval));
            }

            this.context.Comments.Add(
                new Comment
                {
                    QuizId = model.QuizId,
                    Content = model.Content,
                    PublishDate = DateTime.Now,
                    UserId = userId
                });

            this.context.SaveChanges();

            return RedirectToAction("UpdateCommentsGrid", new { quizId = model.QuizId });
        }

        [HttpGet]
        [NoCache]
        [ActionName("UpdateCommentsGrid")]
        [HandleAjaxError]
        public ActionResult GetCommentsGird(int quizId)
        {
            string userId = this.User.Identity.GetUserId();

            var comments = this.context.Comments.All()
                .Where(x => x.QuizId == quizId)
                .Select(ModelConvertor.CommentToViewModel);

            var dataPager = new SimpleDataPager<CommentViewModel>(comments, 4)
                .ProcessUrlParameters(this.Request.QueryString)
                .Sort("PublishDate", SortingDirection.Descending)
                .Load();

            dataPager.UrlParameters.Add("quizId", quizId);

            return PartialView("_CommentsView", dataPager);
        }

        [NonAction]
        private double TimeFromLastComment (string userId, int quizId, int minMinutes = 2)
        {
            string key = "c-quiz" + quizId;
            DateTime interval;
            if (this.Session[key] == null)
            {
                Comment lastComment =
                    this.context.Comments.All()
                    .Where(c => c.QuizId == quizId && c.UserId == userId)
                    .OrderByDescending(c => c.PublishDate).FirstOrDefault();

                if (lastComment == null) { return 0; }

                interval = lastComment.PublishDate.AddMinutes(minMinutes);
            }
            else
            {
                interval = (DateTime)this.Session[key];
            }

            double timeSpan = (interval - DateTime.Now).TotalSeconds;

            if (timeSpan > 0)
            {
                this.Session.Add(key, interval);
            }
            else if(this.Session[key] != null)
            {
                this.Session.Remove(key);
            }

            return timeSpan;
        }

        [NonAction]
        private void ManageResults(string userId, QuizSolvedModel solvedQuiz)
        {
            QuizResult result =
                this.context.Results.All()
                .FirstOrDefault(x => x.UserId == userId && x.QuizId == solvedQuiz.Id);

            if (result == null)
            {
                result = new QuizResult
                {
                    UserId = userId,
                    QuizId = solvedQuiz.Id,
                    FirstResult = solvedQuiz.CalculateResult(),
                };

                result.LastResult = result.FirstResult;

                this.context.Results.Add(result);
                this.context.SaveChanges();
            }
            else
            {
                result.LastResult = solvedQuiz.CalculateResult();
                this.context.Results.Update(result);
                this.context.SaveChanges();
            }
        }
	}
}