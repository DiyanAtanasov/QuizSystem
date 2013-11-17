using QuizSystem.Model;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QuizSystem.Web.Libs.Helpers;
using System.Data.Entity;

namespace QuizSystem.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            IQueryable<Quiz> query = this.context.Quizzes.All();

            if (this.User != null)
            {
                string userId = this.User.Identity.GetUserId();
                query = query.Where(x => x.State == QuizState.Active &&
                            !x.Results.Any(r => r.UserId == userId));
            }
          
            HomeQuizzesViewModel model = new HomeQuizzesViewModel();

            model.NewestQuizzes = query.OrderByDescending(x => x.LastModfication)
                           .Take(6)
                           .Select(ModelConvertor.QuizToHomeViewModel);

            model.MostRatedQuizzes = query.OrderByDescending(x => x.Rating)
                           .Take(6)
                           .Select(ModelConvertor.QuizToHomeViewModel);

            model.MostCommentedQuizzes = query.OrderByDescending(x => x.Comments.Count)
                           .Take(6)
                           .Select(ModelConvertor.QuizToHomeViewModel);

            model.MostQuestionsQuizzes = query.OrderByDescending(x => x.Questions.Count)
                           .Take(6)
                           .Select(ModelConvertor.QuizToHomeViewModel);

            return View(model);
        }

        [HttpGet]
        public ActionResult About()
        {
            var messages = this.context.Messages.All()
                .OrderByDescending(x => x.PublishDate)
                .Select(ModelConvertor.MessageToViewModel);

            return View(messages);
        }

        [Authorize]
        [HttpPost]
        public ActionResult About(MessageCreateModel model)
        {
            string userId = this.User.Identity.GetUserId();

            var lastMessage = this.context.Messages.All()
                .FirstOrDefault(x => x.UserId == userId &&
                    DbFunctions.DiffSeconds(x.PublishDate, DateTime.Now) < 120);

            if(lastMessage == null)
            {
                this.context.Messages.Add(
                new UserMessage
                {
                    Content = model.Content,
                    PublishDate = DateTime.Now,
                    UserId = userId
                });

                this.context.SaveChanges();
            }
            else
            {
                double remainingTime = (lastMessage.PublishDate.AddMinutes(2) - DateTime.Now).TotalSeconds;
                this.ModelState.AddModelError("",
                    String.Format("You can post message again in {0:F0} seconds.", remainingTime));
            }
            

            var messages = this.context.Messages.All()
                .OrderByDescending(x => x.PublishDate)
                .Select(ModelConvertor.MessageToViewModel);

            return View(messages);
        }
    }
}