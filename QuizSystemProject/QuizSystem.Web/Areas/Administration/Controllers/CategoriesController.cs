using QuizSystem.Model;
using QuizSystem.Web.Areas.Administration.Models;
using QuizSystem.Web.Extensions.FiltersExtensions;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace QuizSystem.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            this.ValidateAdmin();

            var mainCategories =
                this.context.Categories.All()
                .Select(x => new CategoryAdminModel { Id = x.Id, Name = x.Name, QuizzesCount = x.Quizzes.Count })
                .ToList();

            return View(mainCategories);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult Update(CategoryAdminModel model)
        {
            ValidateAjaxRequest();

            if (this.context.Categories.All().Any(x => String.Compare(x.Name, model.Name, true) == 0))
            {
                throw new HttpException(400, String.Format("Category with name '{0}' already exists", model.Name));
            }

            Category newCategory;

            if (model.Id <= 0)
            {
                newCategory = this.context.Categories.Add(new Category { Name = model.Name });
            }
            else
            {
                newCategory = this.context.Categories.Update(new Category { Id = model.Id, Name = model.Name });
            }

            this.context.SaveChanges();

            return PartialView("_EditCategory", new CategoryAdminModel { Id = newCategory.Id, Name = newCategory.Name });
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult Remove(CategoryAdminModel model)
        {
            this.ValidateAjaxRequest();

            this.context.Categories.Remove(model.Id);
            this.context.SaveChanges();

            return new HttpStatusCodeResult(200);
        }

        [NonAction]
        private void ValidateAjaxRequest()
        {
            if (!User.IsInRole("Admin"))
            {
                throw new HttpException(400, "You are not authorised for this operation.");
            }

            if (!this.ModelState.IsValid)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Validation Error(s) has occured.");
                foreach (var value in this.ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        sb.AppendFormat(" {0}", error.ErrorMessage);
                    }
                }

                throw new HttpException(400, sb.ToString());
            }
        }
	}
}