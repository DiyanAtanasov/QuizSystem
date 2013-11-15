using QuizSystem.Web.Extensions.FiltersExtensions;
using QuizSystem.Web.Libs.DataPager;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QuizSystem.Web.Libs.Helpers;
using QuizSystem.Web.Areas.Administration.Models;
using QuizSystem.Model;

namespace QuizSystem.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuizzesController : BaseController
    {
        [ValidateInput(false)]
        public ActionResult Index()
        {
            this.ValidateAdmin();

            IEnumerable<Category> categories =
                this.context.Categories.All().ToList();

            List<SelectListItem> categoriesFilterListItems = new List<SelectListItem>();
            categoriesFilterListItems.Add(new SelectListItem { Text = "All", Value = "", Selected = true });
            categoriesFilterListItems.AddRange(categories.Select(x => new SelectListItem { Value = x.Name, Text = x.Name }));

            List<SelectListItem> stateFilterListItems = new List<SelectListItem>();
            stateFilterListItems.Add(new SelectListItem { Text = "All", Value = "", Selected = true });
            foreach (var value in Enum.GetValues(typeof(QuizState)))
            {
                stateFilterListItems.Add(new SelectListItem { Value = ((int)value).ToString(), Text = value.ToString() });
            }

            this.ViewData.Add("categoriesFilter", categoriesFilterListItems);
            this.ViewData.Add("statesFilter", stateFilterListItems);

            IQueryable<QuizAdminViewModel> quizzes =
                this.context.Quizzes.All()
                .Select(ModelConvertor.QuizToAdminViewModel);

            var pager =
                new SimpleDataPager<QuizAdminViewModel>(quizzes, 10)
                .ProcessUrlParameters(this.Request.RequestType == "GET" ? this.Request.QueryString : this.Request.Form)
                .Load();

            return View(pager);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(int quizId)
        {
            this.ValidateAdmin();

            Quiz quiz = this.context.Quizzes.GetById(quizId);
            quiz.State = QuizState.Active;
            quiz.LastModfication = DateTime.Now;
            this.context.Quizzes.Update(quiz);
            this.context.SaveChanges();
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int quizId)
        {
            this.ValidateAdmin();

            Quiz quiz = this.context.Quizzes.GetById(quizId);
            quiz.State = QuizState.Rejected;
            quiz.LastModfication = DateTime.Now;
            this.context.Quizzes.Update(quiz);
            this.context.SaveChanges();

            return RedirectToAction("Index");
        }
	}
}